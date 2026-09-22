using SmartCubeMobile.Services;

namespace SmartCubeMobile;

public partial class LoginPage : ContentPage
{
    private bool _autoTried;

    public LoginPage()
    {
        InitializeComponent();
        UpdateServerLabel();
    }

    private void UpdateServerLabel()
    {
        var host = SessionService.ServerBase.Replace("https://", "").Replace("http://", "");
        ServerLabel.Text = $"Server: {host}";
    }

    private async void OnServerTapped(object sender, EventArgs e)
    {
        var value = await DisplayPromptAsync("Server",
            "Address of the SmartCube server. Leave blank to use the default.",
            placeholder: SessionService.DefaultServer, initialValue: SessionService.ServerBase, maxLength: 200);
        if (value == null) return;
        value = value.Trim();
        if (value.Length > 0 && !value.StartsWith("http://") && !value.StartsWith("https://"))
            value = "https://" + value;
        SessionService.ServerBase = value;
        UpdateServerLabel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(120);
        Card.Scale = 0.94;
        _ = Card.FadeTo(1, 450, Easing.CubicOut);
        _ = Card.ScaleTo(1, 500, Easing.SpringOut);

        if (_autoTried) return;
        _autoTried = true;

        if (!string.IsNullOrEmpty(SessionService.DeviceToken))
        {
            SetBusy(true, "Signing you in…");
            var (ok, error) = await SessionService.TryDeviceLogin();
            if (ok)
            {
                await EnterDashboard();
                return;
            }
            SetBusy(false, error);
        }
    }

    private async void OnSignInClicked(object sender, EventArgs e) => await SignIn();
    private void OnLoginCompleted(object sender, EventArgs e) => PasswordEntry.Focus();
    private async void OnPasswordCompleted(object sender, EventArgs e) => await SignIn();
    private void OnRememberLabelTapped(object sender, EventArgs e) => RememberCheck.IsChecked = !RememberCheck.IsChecked;

    private void OnShowPasswordClicked(object sender, EventArgs e)
    {
        PasswordEntry.IsPassword = !PasswordEntry.IsPassword;
        ShowPasswordBtn.Text = PasswordEntry.IsPassword ? "Show" : "Hide";
    }

    private async void OnCreateAccountTapped(object sender, EventArgs e)
    {
        try { await Launcher.Default.OpenAsync(new Uri($"{SessionService.ServerBase}/site/register.html")); }
        catch { }
    }

    private async Task SignIn()
    {
        var login = LoginEntry.Text?.Trim();
        var password = PasswordEntry.Text;
        if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
        {
            ShowStatus("Enter your username or email and your password.");
            return;
        }

        SetBusy(true, "Signing you in…");
        var (ok, error) = await SessionService.Login(login, password, RememberCheck.IsChecked);
        if (!ok)
        {
            SetBusy(false, error);
            PasswordEntry.Text = "";
            PasswordEntry.Focus();
            return;
        }
        await EnterDashboard();
    }

    private void SetBusy(bool busy, string status)
    {
        SignInBtn.IsEnabled = !busy;
        LoginEntry.IsEnabled = !busy;
        PasswordEntry.IsEnabled = !busy;
        RememberCheck.IsEnabled = !busy;
        SignInBtn.Text = busy ? "Please wait…" : "Sign In";
        StatusLabel.TextColor = busy ? Color.FromArgb("#94A3B8") : Color.FromArgb("#F87171");
        ShowStatus(status);
    }

    private void ShowStatus(string text)
    {
        StatusLabel.Text = text ?? "";
        StatusLabel.IsVisible = !string.IsNullOrEmpty(text);
    }

    private async Task EnterDashboard()
    {
        await Card.FadeTo(0, 250, Easing.CubicIn);
        Page next = SessionService.Current?.MustChangePassword == true
            ? new ChangePasswordPage(forced: true)
            : new SmartCubeDashboard();
        Navigation.InsertPageBefore(next, this);
        await Navigation.PopAsync(false);
    }
}
