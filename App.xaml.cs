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
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        try
        {
#if MOCK_DATA
            var login = new LoginPage();
            return new Window(new NavigationPage(login))
            {
                Title = "SmartCube",
                Width = SignInWidth,
                Height = SignInHeight
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
