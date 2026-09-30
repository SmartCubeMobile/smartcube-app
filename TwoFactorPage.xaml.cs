using SmartCubeMobile.Services;

namespace SmartCubeMobile;

public partial class TwoFactorPage : ContentPage
{
    private string[] _codes = Array.Empty<string>();

    public TwoFactorPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Load();
    }

    private async Task Load()
    {
        ShowStatus(null);
        var (ok, error, enabled, codesLeft) = await SessionService.GetTwoFactorStatus();
        if (!ok)
        {
            IntroLabel.Text = "Could not check two-factor status.";
            ShowStatus(error);
            return;
        }

        if (enabled)
        {
            IntroLabel.Text = "Your account asks for a code from your authenticator app every time you sign in on a new device.";
            ManageStatus.Text = $"Two-factor is ON · {codesLeft} recovery code{(codesLeft == 1 ? "" : "s")} left";
            ManageSection.IsVisible = true;
            SetupSection.IsVisible = false;
            return;
        }

        IntroLabel.Text = "Add a second check at sign-in so a stolen password alone can't get into your account.";
        var (sOk, sError, key, png) = await SessionService.StartTwoFactorSetup();
        if (!sOk)
        {
            ShowStatus(sError);
            return;
        }
        KeyLabel.Text = key;
        if (png != null) QrImage.Source = ImageSource.FromStream(() => new MemoryStream(png));
        SetupSection.IsVisible = true;
        SetupCodeEntry.Focus();
    }

    private async void OnEnableClicked(object sender, EventArgs e)
    {
        var code = SetupCodeEntry.Text?.Replace(" ", "").Trim();
        if (string.IsNullOrEmpty(code) || code.Length != 6)
        {
            ShowStatus("Enter the 6-digit code from the app.");
            return;
        }
        EnableBtn.IsEnabled = false;
        EnableBtn.Text = "Checking…";
        var (ok, error, codes) = await SessionService.EnableTwoFactor(code);
        EnableBtn.IsEnabled = true;
        EnableBtn.Text = "Turn On Two-Factor";
        if (!ok)
        {
            ShowStatus(error);
            SetupCodeEntry.Text = "";
            SetupCodeEntry.Focus();
            return;
        }
        _codes = codes;
        CodesLabel.Text = string.Join("\n", codes);
        SetupSection.IsVisible = false;
        CodesSection.IsVisible = true;
        ShowStatus(null);
    }

    private async void OnCopyCodesClicked(object sender, EventArgs e)
    {
        try
        {
            await Clipboard.Default.SetTextAsync("SmartCube recovery codes\n" + string.Join("\n", _codes));
            CopyBtn.Text = "Copied";
        }
        catch { }
    }

    private async void OnDisableClicked(object sender, EventArgs e)
    {
        var code = DisableCodeEntry.Text?.Trim();
        if (string.IsNullOrEmpty(code))
        {
            ShowStatus("Enter a code first.");
            return;
        }
        var confirm = await DisplayAlert("Turn off two-factor?",
            "Your account will only be protected by your password.", "Turn Off", "Cancel");
        if (!confirm) return;

        DisableBtn.IsEnabled = false;
        var (ok, error) = await SessionService.DisableTwoFactor(code);
        DisableBtn.IsEnabled = true;
        if (!ok)
        {
            ShowStatus(error);
            return;
        }
        await DisplayAlert("Two-factor off", "Two-factor authentication has been turned off.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnCloseClicked(object sender, EventArgs e) => await Navigation.PopAsync();

    private void ShowStatus(string text)
    {
        StatusLabel.Text = text ?? "";
        StatusLabel.IsVisible = !string.IsNullOrEmpty(text);
    }
}
