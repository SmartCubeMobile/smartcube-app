using Newtonsoft.Json;

namespace SmartCubeMobile.Services
{
    public class StoredDocument
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string FilePath { get; set; }
        public string OriginalFileName { get; set; }
        public DateTime DateAdded { get; set; }
        public long FileSizeBytes { get; set; }
    }

    public static class DocumentStorageService
    {
        private static readonly string _docsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SmartCube", "Documents");

        private static readonly string _indexPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SmartCube", "documents.json");

        private static readonly string _reportsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "SmartCubeMobile");

        private static List<StoredDocument> _documents;

        public static List<StoredDocument> GetAll()
        {
            EnsureLoaded();
            ScanForReports();
            return _documents.OrderByDescending(d => d.DateAdded).ToList();
        }

        public static StoredDocument AddDocument(string sourceFilePath, string category, string displayName = null)
        {
            EnsureLoaded();
            Directory.CreateDirectory(_docsDir);

            var fileName = Path.GetFileName(sourceFilePath);
            var ext = Path.GetExtension(fileName);
            var id = Guid.NewGuid().ToString("N")[..8];
            var safeName = $"{category}_{id}{ext}";
            var destPath = Path.Combine(_docsDir, safeName);

            // Stored encrypted for this Windows user; opened via SecureFile.DecryptForViewing.
            var originalSize = new FileInfo(sourceFilePath).Length;
            SecureFile.CopyEncrypted(sourceFilePath, destPath);

            var doc = new StoredDocument
            {
                Id = id,
                Name = displayName ?? fileName,
                Category = category,
                FilePath = destPath,
                OriginalFileName = fileName,
                DateAdded = DateTime.Now,
                FileSizeBytes = originalSize,
            };

            _documents.Add(doc);
            Save();
            return doc;
        }

        public static void RemoveDocument(string id)
        {
            EnsureLoaded();
            var doc = _documents.FirstOrDefault(d => d.Id == id);
            if (doc == null) return;

            try { if (File.Exists(doc.FilePath)) File.Delete(doc.FilePath); } catch { }
            _documents.Remove(doc);
            Save();
        }

        private static void ScanForReports()
        {
            if (!Directory.Exists(_reportsDir)) return;

            try
            {
                var pdfs = Directory.GetFiles(_reportsDir, "*.pdf");
                foreach (var pdf in pdfs)
                {
                    if (_documents.Any(d => d.FilePath == pdf)) continue;

                    var fi = new FileInfo(pdf);
                    _documents.Add(new StoredDocument
                    {
                        Id = Guid.NewGuid().ToString("N")[..8],
                        Name = Path.GetFileNameWithoutExtension(pdf).Replace("_", " "),
                        Category = "Report",
                        FilePath = pdf,
                        OriginalFileName = fi.Name,
                        DateAdded = fi.CreationTime,
                        FileSizeBytes = fi.Length,
                    });
                }
            }
            catch { }
        }

        private static void EnsureLoaded()
        {
            if (_documents != null) return;

            try
            {
                if (File.Exists(_indexPath))
                {
                    var json = SecureFile.ReadAllText(_indexPath);
                    _documents = JsonConvert.DeserializeObject<List<StoredDocument>>(json) ?? new();
                    _documents.RemoveAll(d => !File.Exists(d.FilePath));
                }
                else
                {
                    _documents = new();
                }
            }
            catch
            {
                _documents = new();
            }
        }

        private static void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(_indexPath);
                Directory.CreateDirectory(dir);
                SecureFile.WriteAllText(_indexPath, JsonConvert.SerializeObject(_documents, Formatting.Indented));
            }
            catch { }
        }

        public static string FormatSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024.0):F1} MB";
        }
    }
}
