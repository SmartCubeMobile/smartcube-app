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

    // Password sign-in needed to unlock the data (e.g. first start after the vault arrived, or a PIN
    // unlock that couldn't open the data). Skips the automatic PIN screen.
    public LoginPage(string message, string username = null) : this()
    {
        _autoTried = true;
        if (!string.IsNullOrEmpty(username)) LoginEntry.Text = username;
        StatusLabel.TextColor = Color.FromArgb("#94A3B8");
        StatusLabel.Text = message;
        StatusLabel.IsVisible = !string.IsNullOrEmpty(message);
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

        if (string.IsNullOrEmpty(SessionService.DeviceToken)) return;

        // Remembered PC with a PIN: ask for the PIN (the server checks it).
        if (SessionService.DeviceHasPin)
        {
            await Navigation.PushAsync(new PinPage(PinPage.Mode.Unlock), false);
            return;
        }

        // PC remembered before PINs existed: sign in once more, then ask for a PIN to keep it remembered.
        SetBusy(true, "Signing you in…");
        var r = await SessionService.TryDeviceLogin();
        if (r.Ok)
        {
            if (r.NeedsPin) { SetBusy(false, null); await Navigation.PushAsync(new PinPage(PinPage.Mode.CreateForExisting)); }
            else GoToApp();
            return;
        }
        if (r.PinRequired)   // the server has a PIN for it even though this PC didn't know
        {
            SetBusy(false, null);
            await Navigation.PushAsync(new PinPage(PinPage.Mode.Unlock), false);
            return;
        }
        SetBusy(false, r.Error);
    }

    // Leave the sign-in screens for the app (or the forced password change).
    public static void GoToApp()
    {
        var window = Application.Current?.Windows.FirstOrDefault();
        if (window == null) return;

        // Never open the app with the data still locked: ask for the password instead.
        if (KeyVault.Exists && !KeyVault.IsUnlocked)
        {
            window.Page = new NavigationPage(new LoginPage(
                "Enter your password to unlock your data on this PC.", SessionService.Current?.Username));
            return;
        }

        Page next = SessionService.Current?.MustChangePassword == true
            ? new ChangePasswordPage(forced: true)
            : new SmartCubeDashboard();
        window.Page = new NavigationPage(next);
    }

    // Data key is open: convert older files to it, add the "this PC" slot if remembered, show a new
    // recovery key if one was just created, then offer a PIN (if asked to remember) and open the app.
    public static async Task ContinueAfterUnlock(INavigation nav, bool remember)
    {
        try { await Task.Run(SecureFile.EncryptExistingFiles); } catch { }
        try { SessionService.SyncProfileFromSession(); } catch { }
        await SessionService.EnsureDeviceSlot();

        if (KeyVault.PendingRecoveryKey != null)
        {
            await nav.PushAsync(new RecoveryKeyPage(KeyVault.PendingRecoveryKey,
                then: () => NextAfterUnlock(nav, remember)));
            return;
        }
        await NextAfterUnlock(nav, remember);
    }

    private static async Task NextAfterUnlock(INavigation nav, bool remember)
    {
        var alreadyRemembered = !string.IsNullOrEmpty(SessionService.DeviceToken) && SessionService.DeviceHasPin;
        if (remember && !alreadyRemembered)
            await nav.PushAsync(new PinPage(PinPage.Mode.CreateNew));
        else
            GoToApp();
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

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        try { await Launcher.Default.OpenAsync(new Uri($"{SessionService.ServerBase}/site/forgot-password.html")); }
        catch { }
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

        var code = CodeSection.IsVisible ? CodeEntry.Text?.Trim() : null;
        if (CodeSection.IsVisible && string.IsNullOrEmpty(code))
        {
            ShowStatus("Enter the code from your authenticator app.");
            CodeEntry.Focus();
            return;
        }

        SetBusy(true, "Signing you in…");
        var (ok, error, twoFactor) = await SessionService.Login(login, password, RememberCheck.IsChecked, code);
        if (!ok)
        {
            if (twoFactor)
            {
                // Password was right; now ask for the authenticator code (keep the password).
                SetBusy(false, string.IsNullOrEmpty(code) ? null : error);
                StatusLabel.TextColor = Color.FromArgb("#94A3B8");
                if (string.IsNullOrEmpty(code)) ShowStatus("Two-factor is on for this account.");
                CodeSection.IsVisible = true;
                CodeEntry.Text = "";
                CodeEntry.Focus();
                return;
            }
            SetBusy(false, error);
            PasswordEntry.Text = "";
            CodeSection.IsVisible = false;
            PasswordEntry.Focus();
            return;
        }

        // Open the data with the password (creating the vault and a recovery key on first use).
        SetBusy(true, "Unlocking your data…");
        var username = SessionService.Current?.Username ?? login;
        KeyVault.UnlockResult unlock;
        try { unlock = await Task.Run(() => KeyVault.UnlockWithPassword(username, password)); }
        catch (Exception ex) { SetBusy(false, "Could not unlock your data: " + ex.Message); return; }

        var remember = RememberCheck.IsChecked;
        SetBusy(false, null);
        PasswordEntry.Text = "";

        if (unlock is KeyVault.UnlockResult.WrongPassword or KeyVault.UnlockResult.WrongOwner)
        {
            await Navigation.PushAsync(new RecoveryUnlockPage(unlock, username, password, remember));
            return;
        }
        await ContinueAfterUnlock(Navigation, remember);
    }

    private void SetBusy(bool busy, string status)
    {
        SignInBtn.IsEnabled = !busy;
        LoginEntry.IsEnabled = !busy;
        PasswordEntry.IsEnabled = !busy;
        CodeEntry.IsEnabled = !busy;
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
