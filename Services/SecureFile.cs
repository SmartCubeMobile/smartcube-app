using System.Security.Cryptography;
using System.Text;
using SmartCubeMobile.Services;

namespace SmartCubeMobile
{
    // Read/write for the app's local data files.
    //
    // Formats (all are read; the best available is written):
    //   "SCK1:"  text, AES-256-GCM with the KeyVault data key (password / recovery key / PIN-protected PC)
    //   "SCKB1"  binary (documents), same key
    //   "SCDP1:" text, Windows DPAPI for this Windows user   (older files, and files needed before sign-in)
    //   "SCDPB1" binary, DPAPI
    //   anything else: legacy plain text
    // Once the vault is unlocked, writes use the vault key and EncryptExistingFiles converts older files.
    public static class SecureFile
    {
        private const string Magic = "SCDP1:";
        private const string VaultMagic = "SCK1:";
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("SmartCubeMobile.LocalData.v1");
        private static readonly byte[] BinMagic = Encoding.ASCII.GetBytes("SCDPB1");
        private static readonly byte[] VaultBinMagic = Encoding.ASCII.GetBytes("SCKB1");

        // Files read before sign-in must stay on the Windows-only lock.
        private static bool MustStayDpapi(string path)
        {
            var name = Path.GetFileName(path);
            return name.Equals("device.json", StringComparison.OrdinalIgnoreCase)
                || name.StartsWith("keys.json", StringComparison.OrdinalIgnoreCase);
        }

        // ---------------- Text ----------------

        public static string ReadAllText(string path)
        {
            var raw = File.ReadAllText(path);
            if (raw.StartsWith(VaultMagic, StringComparison.Ordinal))
                return Encoding.UTF8.GetString(VaultDecrypt(Convert.FromBase64String(raw[VaultMagic.Length..])));
            if (raw.StartsWith(Magic, StringComparison.Ordinal))
            {
                var plain = ProtectedData.Unprotect(Convert.FromBase64String(raw[Magic.Length..]), Entropy, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(plain);
            }
            return raw;
        }

        public static void WriteAllText(string path, string contents)
        {
            if (!KeyVault.IsUnlocked || MustStayDpapi(path)) { WriteAllTextDpapi(path, contents); return; }
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            TestActivity.BeforeWrite(path);   // test accounts only; no-op otherwise
            var cipher = VaultEncrypt(Encoding.UTF8.GetBytes(contents ?? ""));
            AtomicWriteText(path, VaultMagic + Convert.ToBase64String(cipher));
            TestActivity.AfterWrite(path, contents);
        }

        public static void WriteAllTextDpapi(string path, string contents)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            var cipher = ProtectedData.Protect(Encoding.UTF8.GetBytes(contents ?? ""), Entropy, DataProtectionScope.CurrentUser);
            AtomicWriteText(path, Magic + Convert.ToBase64String(cipher));
        }

        // True for either encrypted text format.
        public static bool IsEncrypted(string path) => TextFormat(path) is "vault" or "dpapi";

        private static string TextFormat(string path)
        {
            try
            {
                using var reader = new StreamReader(path);
                var buffer = new char[6];
                var n = reader.Read(buffer, 0, 6);
                var head = new string(buffer, 0, n);
                if (head.StartsWith(VaultMagic, StringComparison.Ordinal)) return "vault";
                if (head.StartsWith(Magic, StringComparison.Ordinal)) return "dpapi";
                return "plain";
            }
            catch { return "unknown"; }
        }

        // ---------------- Binary (documents) ----------------

        private static string BinaryFormat(string path)
        {
            try
            {
                using var fs = File.OpenRead(path);
                var head = new byte[6];
                var n = fs.Read(head, 0, 6);
                if (n >= 5 && head.AsSpan(0, 5).SequenceEqual(VaultBinMagic)) return "vault";
                if (n == 6 && head.AsSpan().SequenceEqual(BinMagic)) return "dpapi";
                return "plain";
            }
            catch { return "unknown"; }
        }

        public static bool IsEncryptedBinary(string path) => BinaryFormat(path) is "vault" or "dpapi";

        public static void WriteAllBytes(string path, byte[] contents)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            byte[] magic, cipher;
            if (KeyVault.IsUnlocked) { magic = VaultBinMagic; cipher = VaultEncrypt(contents ?? Array.Empty<byte>()); }
            else { magic = BinMagic; cipher = ProtectedData.Protect(contents ?? Array.Empty<byte>(), Entropy, DataProtectionScope.CurrentUser); }
            var tmp = path + ".tmp";
            using (var fs = File.Create(tmp))
            {
                fs.Write(magic, 0, magic.Length);
                fs.Write(cipher, 0, cipher.Length);
            }
            File.Move(tmp, path, true);
        }

        public static byte[] ReadAllBytes(string path)
        {
            var raw = File.ReadAllBytes(path);
            if (raw.Length >= VaultBinMagic.Length && raw.AsSpan(0, VaultBinMagic.Length).SequenceEqual(VaultBinMagic)
                && !(raw.Length >= BinMagic.Length && raw.AsSpan(0, BinMagic.Length).SequenceEqual(BinMagic)))
                return VaultDecrypt(raw[VaultBinMagic.Length..]);
            if (raw.Length >= BinMagic.Length && raw.AsSpan(0, BinMagic.Length).SequenceEqual(BinMagic))
                return ProtectedData.Unprotect(raw.AsSpan(BinMagic.Length).ToArray(), Entropy, DataProtectionScope.CurrentUser);
            return raw;
        }

        public static void CopyEncrypted(string sourcePath, string destPath) =>
            WriteAllBytes(destPath, File.ReadAllBytes(sourcePath));

        // ---------------- Vault crypto: nonce[12] | tag[16] | ciphertext ----------------

        private static byte[] VaultEncrypt(byte[] plain)
        {
            var key = KeyVault.DataKey ?? throw new InvalidOperationException("SmartCube's data is locked.");
            var nonce = RandomNumberGenerator.GetBytes(12);
            var tag = new byte[16];
            var ct = new byte[plain.Length];
            using (var aes = new AesGcm(key, 16)) aes.Encrypt(nonce, plain, ct, tag);
            var result = new byte[12 + 16 + ct.Length];
            nonce.CopyTo(result, 0); tag.CopyTo(result, 12); ct.CopyTo(result, 28);
            return result;
        }

        private static byte[] VaultDecrypt(byte[] data)
        {
            var key = KeyVault.DataKey ?? throw new InvalidOperationException("SmartCube's data is locked. Sign in to open it.");
            var plain = new byte[data.Length - 28];
            using (var aes = new AesGcm(key, 16)) aes.Decrypt(data.AsSpan(0, 12), data.AsSpan(28), data.AsSpan(12, 16), plain);
            return plain;
        }

        private static void AtomicWriteText(string path, string text)
        {
            var tmp = path + ".tmp";
            File.WriteAllText(tmp, text);
            File.Move(tmp, path, true);
        }

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

        // ---------------- Conversion ----------------
        // Before sign-in: plain files -> Windows lock. After the vault is unlocked: everything -> vault key.
        // Safe to run repeatedly; files already in the target format are skipped after a header check.
        public static (int Json, int Binary) EncryptExistingFiles()
        {
            int json = 0, bin = 0;
            var vault = KeyVault.IsUnlocked;
            var local = SmartCubeMobile.Services.AppPaths.Local;
            var jsonDirs = new[] { Path.Combine(local, "SmartCube"), SmartCubeMobile.Services.AppPaths.AppData };
            foreach (var dir in jsonDirs.Where(Directory.Exists))
            {
                foreach (var f in Directory.GetFiles(dir, "*.json"))
                {
                    try
                    {
                        if (Path.GetFileName(f).StartsWith("keys.json", StringComparison.OrdinalIgnoreCase)) continue;   // KeyVault's own file
                        var fmt = TextFormat(f);
                        if (MustStayDpapi(f))
                        {
                            if (fmt == "plain") { WriteAllTextDpapi(f, File.ReadAllText(f)); json++; }
                            continue;
                        }
                        if (fmt == "vault" || (!vault && fmt == "dpapi")) continue;
                        var text = ReadAllText(f);
                        if (string.IsNullOrWhiteSpace(text)) continue;
                        WriteAllText(f, text);
                        json++;
                    }
                    catch { }
                }
            }

            var binDirs = new[] { Path.Combine(local, "SmartCube", "Documents"), Path.Combine(SmartCubeMobile.Services.AppPaths.AppData, "SupplierDownloads") };
            foreach (var dir in binDirs.Where(Directory.Exists))
            {
                foreach (var f in Directory.GetFiles(dir))
                {
                    try
                    {
                        if (f.EndsWith(".tmp", StringComparison.OrdinalIgnoreCase)) continue;
                        var fmt = BinaryFormat(f);
                        if (fmt == "vault" || (!vault && fmt == "dpapi")) continue;
                        WriteAllBytes(f, ReadAllBytes(f));
                        bin++;
                    }
                    catch { }
                }
            }
            return (json, bin);
        }
    }
}
