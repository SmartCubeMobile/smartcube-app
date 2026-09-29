using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace SmartCubeMobile.Services
{
    // One lock, three keys (see Dad's memo).
    //
    // All local data is encrypted with a random 256-bit data key (DEK). The DEK itself is stored three
    // times in keys.json, each copy locked by a different key:
    //   Password slot  - key derived from the SmartCube password (PBKDF2-SHA256, 600k rounds)
    //   Recovery slot  - key derived from the recovery key the user saved (shown once)
    //   Device slot    - key = SHA-256(server device key || local secret); the server releases its half
    //                    only after a correct PIN, and the local secret is DPAPI-bound to this Windows user
    // Any one slot opens the data. SmartCube (the server) never sees the DEK, the password-derived key or
    // the recovery key. keys.json holds only wrapped (encrypted) keys, so it is safe to copy with a backup.
    public static class KeyVault
    {
        private static readonly string DataDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SmartCube");
        public static readonly string KeysPath = Path.Combine(DataDir, "keys.json");

        private const int PasswordIterations = 600_000;
        private const int RecoveryIterations = 100_000;
        private static readonly byte[] DeviceEntropy = Encoding.UTF8.GetBytes("SmartCube.DeviceSlot.v1");
        private const string LegacyDpapiMagic = "SCDP1:";
        private static readonly byte[] LegacyDpapiEntropy = Encoding.UTF8.GetBytes("SmartCubeMobile.LocalData.v1");

        private static byte[] _dek;

        public static bool Exists => File.Exists(KeysPath);
        public static bool IsUnlocked => _dek != null;
        internal static byte[] DataKey => _dek;

        // Set when a new recovery key has been created and not yet shown to the user.
        public static string PendingRecoveryKey { get; private set; }

        // The user ticked "I've saved my recovery key". Until then the key isn't final: if the app closes
        // first, the next unlock replaces it with a fresh one and shows that instead.
        public static void ConfirmRecoveryKey()
        {
            PendingRecoveryKey = null;
            var kf = Load();
            if (kf == null || kf.RecoveryConfirmed) return;
            kf.RecoveryConfirmed = true;
            Save(kf);
        }

        public static string Owner => Load()?.Owner;
        public static bool HasDeviceSlot => Load()?.Device != null;

        private class Slot
        {
            public string Salt { get; set; }
            public int Iterations { get; set; }
            public string Wrapped { get; set; }
        }
        private class DeviceSlot
        {
            public string LocalSecret { get; set; }   // DPAPI (CurrentUser) protected
            public string Wrapped { get; set; }
        }
        private class KeyFile
        {
            public int Version { get; set; } = 1;
            public string Owner { get; set; }
            public DateTime Created { get; set; }
            public DateTime? RecoveryCreated { get; set; }
            public bool RecoveryConfirmed { get; set; }
            public Slot Password { get; set; }
            public Slot Recovery { get; set; }
            public DeviceSlot Device { get; set; }
        }

        public enum UnlockResult { Unlocked, Created, WrongPassword, WrongOwner }

        // ---------------- Unlocking ----------------

        // After a password sign-in. Creates the vault on first use (existing data is backed up first).
        public static UnlockResult UnlockWithPassword(string owner, string password)
        {
            var kf = Load();
            if (kf == null && Exists)
                throw new InvalidOperationException("This PC's key file (keys.json) couldn't be read, so your data can't be opened. Nothing has been changed.");
            if (kf == null)
            {
                BackupDataFolder("before-keys");
                _dek = RandomNumberGenerator.GetBytes(32);
                var recovery = NewRecoveryKey();
                kf = new KeyFile
                {
                    Owner = owner,
                    Created = DateTime.UtcNow,
                    RecoveryCreated = DateTime.UtcNow,
                    Password = WrapWithSecret(_dek, password, PasswordIterations),
                    Recovery = WrapWithSecret(_dek, NormaliseRecoveryKey(recovery), RecoveryIterations),
                };
                Save(kf);
                PendingRecoveryKey = recovery;
                return UnlockResult.Created;
            }

            if (kf.Password != null)
            {
                var dek = UnwrapWithSecret(kf.Password, password);
                if (dek != null)
                {
                    _dek = dek;
                    if (!string.Equals(kf.Owner, owner, StringComparison.OrdinalIgnoreCase)) { kf.Owner = owner; Save(kf); }
                    ReissueIfUnconfirmed(kf);
                    return UnlockResult.Unlocked;
                }
            }
            return string.Equals(kf.Owner, owner, StringComparison.OrdinalIgnoreCase)
                ? UnlockResult.WrongPassword      // password was reset since this PC last used it
                : UnlockResult.WrongOwner;        // a different account's data lives on this PC
        }

        public static bool UnlockWithRecoveryKey(string recoveryKey)
        {
            var kf = Load();
            if (kf?.Recovery == null) return false;
            var dek = UnwrapWithSecret(kf.Recovery, NormaliseRecoveryKey(recoveryKey));
            if (dek == null) return false;
            _dek = dek;
            if (!kf.RecoveryConfirmed) { kf.RecoveryConfirmed = true; Save(kf); }   // they clearly have it
            return true;
        }

        // A recovery key the user never confirmed saving is replaced, and the new one is shown next.
        private static void ReissueIfUnconfirmed(KeyFile kf)
        {
            if (kf.RecoveryConfirmed || _dek == null) return;
            var recovery = NewRecoveryKey();
            kf.Recovery = WrapWithSecret(_dek, NormaliseRecoveryKey(recovery), RecoveryIterations);
            kf.RecoveryCreated = DateTime.UtcNow;
            Save(kf);
            PendingRecoveryKey = recovery;
        }

        public static bool UnlockWithDevice(string serverDeviceKeyBase64)
        {
            var kf = Load();
            if (kf?.Device == null || string.IsNullOrEmpty(serverDeviceKeyBase64)) return false;
            try
            {
                var local = ProtectedData.Unprotect(Convert.FromBase64String(kf.Device.LocalSecret), DeviceEntropy, DataProtectionScope.CurrentUser);
                var dek = Unwrap(kf.Device.Wrapped, DeviceKek(serverDeviceKeyBase64, local));
                if (dek == null) return false;
                _dek = dek;
                ReissueIfUnconfirmed(kf);
                return true;
            }
            catch { return false; }
        }

        public static void Lock()
        {
            if (_dek != null) CryptographicOperations.ZeroMemory(_dek);
            _dek = null;
        }

        // ---------------- Changing slots (vault must be unlocked) ----------------

        public static void SetPassword(string owner, string password)
        {
            RequireUnlocked();
            var kf = Load();
            kf.Password = WrapWithSecret(_dek, password, PasswordIterations);
            if (!string.IsNullOrEmpty(owner)) kf.Owner = owner;
            Save(kf);
        }

        public static void SetDeviceSlot(string serverDeviceKeyBase64)
        {
            RequireUnlocked();
            if (string.IsNullOrEmpty(serverDeviceKeyBase64)) return;
            var kf = Load();
            var local = RandomNumberGenerator.GetBytes(32);
            kf.Device = new DeviceSlot
            {
                LocalSecret = Convert.ToBase64String(ProtectedData.Protect(local, DeviceEntropy, DataProtectionScope.CurrentUser)),
                Wrapped = Wrap(_dek, DeviceKek(serverDeviceKeyBase64, local)),
            };
            Save(kf);
        }

        public static void RemoveDeviceSlot()
        {
            var kf = Load();
            if (kf?.Device == null) return;
            kf.Device = null;
            Save(kf);
        }

        // "Start over": the old data and keys are moved aside (never deleted), then SmartCube starts empty.
        public static string MoveDataAsideAndReset()
        {
            Lock();
            var target = DataDir + "_locked_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
            if (Directory.Exists(DataDir)) Directory.Move(DataDir, target);
            Directory.CreateDirectory(DataDir);
            return target;
        }

        // ---------------- Recovery key text ----------------

        // 24 characters from a 32-letter alphabet without look-alikes (no I, L, O, U): 120 bits.
        private const string Alphabet = "0123456789ABCDEFGHJKMNPQRSTVWXYZ";

        private static string NewRecoveryKey()
        {
            var bytes = RandomNumberGenerator.GetBytes(24);
            var chars = bytes.Select(b => Alphabet[b % 32]).ToArray();
            return string.Join("-", Enumerable.Range(0, 6).Select(i => new string(chars, i * 4, 4)));
        }

        public static string NormaliseRecoveryKey(string key)
        {
            var s = (key ?? "").ToUpperInvariant()
                .Replace("-", "").Replace(" ", "")
                .Replace('O', '0').Replace('I', '1').Replace('L', '1').Replace('U', 'V');
            return s;
        }

        public static bool LooksLikeRecoveryKey(string key) => NormaliseRecoveryKey(key).Length == 24;

        // ---------------- Crypto helpers ----------------

        private static Slot WrapWithSecret(byte[] dek, string secret, int iterations)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var kek = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(secret ?? ""), salt, iterations, HashAlgorithmName.SHA256, 32);
            return new Slot { Salt = Convert.ToBase64String(salt), Iterations = iterations, Wrapped = Wrap(dek, kek) };
        }

        private static byte[] UnwrapWithSecret(Slot slot, string secret)
        {
            try
            {
                var kek = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(secret ?? ""), Convert.FromBase64String(slot.Salt), slot.Iterations, HashAlgorithmName.SHA256, 32);
                return Unwrap(slot.Wrapped, kek);
            }
            catch { return null; }
        }

        private static byte[] DeviceKek(string serverDeviceKeyBase64, byte[] local)
        {
            var server = Convert.FromBase64String(serverDeviceKeyBase64);
            return SHA256.HashData(server.Concat(local).ToArray());
        }

        // AES-256-GCM: base64(nonce[12] | tag[16] | ciphertext)
        private static string Wrap(byte[] plain, byte[] key)
        {
            var nonce = RandomNumberGenerator.GetBytes(12);
            var tag = new byte[16];
            var ct = new byte[plain.Length];
            using (var aes = new AesGcm(key, 16)) aes.Encrypt(nonce, plain, ct, tag);
            return Convert.ToBase64String(nonce.Concat(tag).Concat(ct).ToArray());
        }

        private static byte[] Unwrap(string wrapped, byte[] key)
        {
            try
            {
                var all = Convert.FromBase64String(wrapped);
                var nonce = all[..12];
                var tag = all[12..28];
                var ct = all[28..];
                var plain = new byte[ct.Length];
                using (var aes = new AesGcm(key, 16)) aes.Decrypt(nonce, ct, tag, plain);
                return plain;
            }
            catch { return null; }   // wrong key: authentication tag doesn't match
        }

        private static void RequireUnlocked()
        {
            if (_dek == null) throw new InvalidOperationException("SmartCube's data is locked.");
        }

        // ---------------- keys.json ----------------

        private static KeyFile Load()
        {
            try
            {
                if (!File.Exists(KeysPath)) return null;
                var text = File.ReadAllText(KeysPath);
                // A build on 29 Sep 2026 wrapped keys.json in the Windows lock by mistake; unwrap it.
                if (text.StartsWith(LegacyDpapiMagic, StringComparison.Ordinal))
                    text = Encoding.UTF8.GetString(ProtectedData.Unprotect(
                        Convert.FromBase64String(text[LegacyDpapiMagic.Length..]), LegacyDpapiEntropy, DataProtectionScope.CurrentUser));
                return JsonConvert.DeserializeObject<KeyFile>(text);
            }
            catch { return null; }
        }

        private static void Save(KeyFile kf)
        {
            Directory.CreateDirectory(DataDir);
            var tmp = KeysPath + ".tmp";
            File.WriteAllText(tmp, JsonConvert.SerializeObject(kf, Formatting.Indented));
            File.Move(tmp, KeysPath, true);
        }

        // Full copy of the data folder before the first conversion, so nothing can be lost.
        private static void BackupDataFolder(string label)
        {
            try
            {
                if (!Directory.Exists(DataDir)) return;
                var target = DataDir + "_backup_" + label + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                foreach (var src in Directory.GetFiles(DataDir, "*", SearchOption.AllDirectories))
                {
                    var dst = Path.Combine(target, Path.GetRelativePath(DataDir, src));
                    Directory.CreateDirectory(Path.GetDirectoryName(dst));
                    File.Copy(src, dst, true);
                }
            }
            catch { }
        }
    }
}
