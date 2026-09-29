using System.Security.Cryptography;
using System.Text;

namespace SmartCubeMobile
{
    // Drop-in replacement for File.ReadAllText/WriteAllText for the app's local data files.
    // Files are encrypted with Windows DPAPI for the current Windows user, so they are unreadable
    // if copied to another machine or opened by another account. Existing plain files are read
    // as-is and become encrypted the next time they are saved.
    public static class SecureFile
    {
        private const string Magic = "SCDP1:";
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("SmartCubeMobile.LocalData.v1");

        public static string ReadAllText(string path)
        {
            var raw = File.ReadAllText(path);
            if (!raw.StartsWith(Magic, StringComparison.Ordinal)) return raw;
            var cipher = Convert.FromBase64String(raw[Magic.Length..]);
            var plain = ProtectedData.Unprotect(cipher, Entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plain);
        }

        public static void WriteAllText(string path, string contents)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            var cipher = ProtectedData.Protect(Encoding.UTF8.GetBytes(contents ?? ""), Entropy, DataProtectionScope.CurrentUser);
            File.WriteAllText(path, Magic + Convert.ToBase64String(cipher));
        }

        public static bool IsEncrypted(string path)
        {
            try
            {
                using var reader = new StreamReader(path);
                var buffer = new char[Magic.Length];
                var n = reader.Read(buffer, 0, buffer.Length);
                return n == Magic.Length && new string(buffer) == Magic;
            }
            catch { return false; }
        }

        // ---------------- Binary files (PDFs, images) ----------------
        // Layout: "SCDPB1" + DPAPI(file bytes). Plain files are returned unchanged, so old documents still open.
        private static readonly byte[] BinMagic = Encoding.ASCII.GetBytes("SCDPB1");

        public static bool IsEncryptedBinary(string path)
        {
            try
            {
                using var fs = File.OpenRead(path);
                var head = new byte[BinMagic.Length];
                return fs.Read(head, 0, head.Length) == head.Length && head.AsSpan().SequenceEqual(BinMagic);
            }
            catch { return false; }
        }

        public static void WriteAllBytes(string path, byte[] contents)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            var cipher = ProtectedData.Protect(contents ?? Array.Empty<byte>(), Entropy, DataProtectionScope.CurrentUser);
            var tmp = path + ".tmp";
            using (var fs = File.Create(tmp))
            {
                fs.Write(BinMagic, 0, BinMagic.Length);
                fs.Write(cipher, 0, cipher.Length);
            }
            File.Move(tmp, path, true);
        }

        public static byte[] ReadAllBytes(string path)
        {
            var raw = File.ReadAllBytes(path);
            if (raw.Length < BinMagic.Length || !raw.AsSpan(0, BinMagic.Length).SequenceEqual(BinMagic)) return raw;
            return ProtectedData.Unprotect(raw.AsSpan(BinMagic.Length).ToArray(), Entropy, DataProtectionScope.CurrentUser);
        }

        // Copies a file into the app's storage, encrypted.
        public static void CopyEncrypted(string sourcePath, string destPath) =>
            WriteAllBytes(destPath, File.ReadAllBytes(sourcePath));

        // ---------------- Opening a document ----------------
        // Documents are decrypted to a private temp folder only while they are being viewed.
        // Copies older than two hours are removed on the next start (CleanOpenedCopies).
        private static string OpenDir => Path.Combine(Path.GetTempPath(), "SmartCube-open");

        public static string DecryptForViewing(string path, string displayName = null)
        {
            if (!IsEncryptedBinary(path)) return path;
            Directory.CreateDirectory(OpenDir);
            var name = Path.GetFileName(string.IsNullOrWhiteSpace(displayName) ? path : displayName);
            if (string.IsNullOrWhiteSpace(Path.GetExtension(name))) name += Path.GetExtension(path);
            foreach (var c in Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
            var target = Path.Combine(OpenDir, $"{Guid.NewGuid():N}"[..6] + "_" + name);
            File.WriteAllBytes(target, ReadAllBytes(path));
            return target;
        }

        public static void CleanOpenedCopies()
        {
            try
            {
                if (!Directory.Exists(OpenDir)) return;
                foreach (var f in Directory.GetFiles(OpenDir))
                    try { if (File.GetLastWriteTimeUtc(f) < DateTime.UtcNow.AddHours(-2)) File.Delete(f); } catch { }
            }
            catch { }
        }

        // ---------------- One-off migration ----------------
        // Encrypts any data file still in plain form: JSON files become SCDP1 text, documents become SCDPB1.
        // Safe to run on every start; files already encrypted are skipped after a header check.
        public static (int Json, int Binary) EncryptExistingFiles()
        {
            int json = 0, bin = 0;
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var jsonDirs = new[] { Path.Combine(local, "SmartCube"), FileSystem.AppDataDirectory };
            foreach (var dir in jsonDirs.Where(Directory.Exists))
            {
                foreach (var f in Directory.GetFiles(dir, "*.json"))
                {
                    try
                    {
                        if (IsEncrypted(f)) continue;
                        var text = File.ReadAllText(f);
                        if (string.IsNullOrWhiteSpace(text)) continue;
                        WriteAllText(f, text);
                        json++;
                    }
                    catch { }
                }
            }

            var binDirs = new[] { Path.Combine(local, "SmartCube", "Documents"), Path.Combine(FileSystem.AppDataDirectory, "SupplierDownloads") };
            foreach (var dir in binDirs.Where(Directory.Exists))
            {
                foreach (var f in Directory.GetFiles(dir))
                {
                    try
                    {
                        if (f.EndsWith(".tmp", StringComparison.OrdinalIgnoreCase) || IsEncryptedBinary(f)) continue;
                        WriteAllBytes(f, File.ReadAllBytes(f));
                        bin++;
                    }
                    catch { }
                }
            }
            return (json, bin);
        }
    }
}
