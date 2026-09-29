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
