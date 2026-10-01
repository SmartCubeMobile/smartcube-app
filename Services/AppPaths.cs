namespace SmartCubeMobile.Services
{
    // Where the app keeps its local data.
    //
    // Demo mode (start the app with --demo) uses a separate folder filled with made-up data, so the
    // real data is never read or written and nothing personal appears in screenshots or demos.
    public static class AppPaths
    {
        public static readonly bool IsDemo =
            Environment.GetCommandLineArgs().Any(a => a.Equals("--demo", StringComparison.OrdinalIgnoreCase));

        private static readonly string RealLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // Replaces %LocalAppData%: the app's "SmartCube" folder sits under this.
        public static string Local => IsDemo ? Path.Combine(RealLocal, "SmartCubeDemo") : RealLocal;

        // Replaces FileSystem.AppDataDirectory (crypto connections/cache, supplier downloads).
        public static string AppData => IsDemo ? Path.Combine(RealLocal, "SmartCubeDemo", "AppData") : FileSystem.AppDataDirectory;

        // The main data folder (bills, caches, profile...).
        public static string Data => Path.Combine(Local, "SmartCube");
    }
}
