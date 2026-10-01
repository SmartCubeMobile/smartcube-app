namespace SmartCubeMobile.Services
{
    // Plain-text activity/error log for the Connect Supplier browser flow.
    // %LocalAppData%\SmartCube\logs\supplier.log
    public static class SupplierLog
    {
        private static readonly object _lock = new();

        public static string Path
        {
            get
            {
                var dir = System.IO.Path.Combine(
                    SmartCubeMobile.Services.AppPaths.Local, "SmartCube", "logs");
                System.IO.Directory.CreateDirectory(dir);
                return System.IO.Path.Combine(dir, "supplier.log");
            }
        }

        public static void Write(string message)
        {
            try
            {
                lock (_lock)
                {
                    var file = Path;
                    // Keep it from growing forever: trim to the last ~400 KB once it passes 1 MB.
                    var fi = new System.IO.FileInfo(file);
                    if (fi.Exists && fi.Length > 1_000_000)
                    {
                        var text = System.IO.File.ReadAllText(file);
                        System.IO.File.WriteAllText(file, text.Substring(text.Length - 400_000));
                    }
                    System.IO.File.AppendAllText(file, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}  {message}{Environment.NewLine}");
                }
            }
            catch { }
        }

        public static void Error(string context, Exception ex) =>
            Write($"ERROR {context}: {ex.GetType().Name}: {ex.Message}");

        public static async Task Open()
        {
            try
            {
                if (!System.IO.File.Exists(Path)) Write("Log created.");
                await Launcher.Default.OpenAsync(new OpenFileRequest("Supplier log", new ReadOnlyFile(Path)));
            }
            catch (Exception ex)
            {
                Write($"Could not open log in viewer: {ex.Message}");
            }
        }
    }
}
