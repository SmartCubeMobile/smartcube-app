namespace SmartCubeMobile;

public partial class App : Application
{
    public static double SignInWidth { get; } = 1200;
    public static double SignInHeight { get; } = 800;

    public App()
    {
        try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            System.IO.File.WriteAllText(
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "maui_crash.txt"),
                $"App constructor: {ex}");
            throw;
        }

        // Demo mode (--demo): fill the separate demo folder with made-up data before anything reads it.
        if (Services.AppPaths.IsDemo)
        {
            try { Services.DemoData.Seed(); } catch { }
        }

        // Encrypt any local data still stored in plain form (older files, documents), and tidy up
        // decrypted copies left from viewing documents. Runs before any page reads the data.
        try
        {
            SecureFile.EncryptExistingFiles();
            SecureFile.CleanOpenedCopies();
        }
        catch { }
    }

    // Keep the window inside the screen's usable area (taskbar/title bar allowed for) so nothing
    // is clipped on smaller laptop displays.
    private static (double Width, double Height) FitToScreen(double width, double height)
    {
        try
        {
            var d = DeviceDisplay.Current.MainDisplayInfo;
            if (d.Width > 0 && d.Height > 0 && d.Density > 0)
            {
                var maxW = d.Width / d.Density - 40;
                var maxH = d.Height / d.Density - 110;
                return (Math.Max(900, Math.Min(width, maxW)), Math.Max(600, Math.Min(height, maxH)));
            }
        }
        catch { }
        return (width, height);
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        try
        {
#if MOCK_DATA
            if (Services.AppPaths.IsDemo)
            {
                // No sign-in: a made-up user and made-up data. "--tour" also saves a screenshot of each section.
                Services.SessionService.StartDemoSession();
                var dashboard = new SmartCubeDashboard();
                var demoWindow = new Window(new NavigationPage(dashboard)) { Title = "SmartCube", X = 40, Y = 0, Width = 1440, Height = 810 };
                if (Environment.GetCommandLineArgs().Any(a => a.Equals("--tour", StringComparison.OrdinalIgnoreCase)))
                    _ = dashboard.RunDemoTour(@"D:\SmartCubeMobile\Website-Screenshots");
                return demoWindow;
            }

            var login = new LoginPage();
            var (width, height) = FitToScreen(SignInWidth, SignInHeight);
            return new Window(new NavigationPage(login))
            {
                Title = "SmartCube",
                Width = width,
                Height = height
            };
#else
            var signinVM = new SignInViewModel();
            var signInView = new SignIn();
            signInView.Initialize(signinVM);
            var page = new ContentPage
            {
                Content = signInView,
                BackgroundColor = Color.FromArgb("#0F172A")
            };

            return new Window(new NavigationPage(page))
            {
                Title = "SmartCube Mobile",
                Width = SignInWidth,
                Height = SignInHeight
            };
#endif
        }
        catch (Exception ex)
        {
            System.IO.File.WriteAllText(
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "maui_crash.txt"),
                $"CreateWindow: {ex}");
            throw;
        }
    }
}
