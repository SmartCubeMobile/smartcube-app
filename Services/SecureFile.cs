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
    }
}
