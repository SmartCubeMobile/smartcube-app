using SmartCubeMobile.Services;

namespace SmartCubeMobile;

// PIN for a remembered device. The server checks the PIN (five wrong tries forget the device),
// so the saved device token on its own can't sign anyone in.
public partial class PinPage : ContentPage
{
    public enum Mode
    {
        Unlock,             // app start on a remembered PC
        CreateNew,          // after a password sign-in with "Remember this device" ticked
        CreateForExisting,  // a PC remembered before PINs existed
        Change,             // from Profile > Session
    }

    private readonly Mode _mode;
    private bool _busy;

    public PinPage(Mode mode)
    {
        InitializeComponent();
        _mode = mode;

        switch (mode)
        {
            case Mode.Unlock:
                TitleLabel.Text = "Enter your PIN";
                IntroLabel.Text = "This PC is remembered. Enter your SmartCube PIN to open your account.";
                PrimaryBtn.Text = "Unlock";
                SecondaryLink.Text = "Use my password instead";
                ToolTipProperties.SetText(SecondaryLink, "Sign in with your username and password instead of the PIN.");
                break;
            case Mode.CreateNew:
                TitleLabel.Text = "Create a PIN";
                IntroLabel.Text = "Next time, open SmartCube on this PC with this PIN instead of your password.";
                PinLabel.Text = "New PIN";
                ConfirmSection.IsVisible = true;
                PrimaryBtn.Text = "Save PIN";
                SecondaryLink.Text = "Don't remember this PC";
                ToolTipProperties.SetText(SecondaryLink, "Skip the PIN; you'll sign in with your password each time.");
                break;
            case Mode.CreateForExisting:
                TitleLabel.Text = "Add a PIN to this PC";
                IntroLabel.Text = "SmartCube now protects remembered PCs with a PIN. Choose one to keep this PC remembered.";
                PinLabel.Text = "New PIN";
                ConfirmSection.IsVisible = true;
                PrimaryBtn.Text = "Save PIN";
                SecondaryLink.Text = "Forget this PC instead";
                ToolTipProperties.SetText(SecondaryLink, "Stop remembering this PC; you'll sign in with your password next time.");
                break;
            case Mode.Change:
                TitleLabel.Text = "Change your PIN";
                IntroLabel.Text = "Choose a new PIN for opening SmartCube on this PC.";
                PinLabel.Text = "New PIN";
                ConfirmSection.IsVisible = true;
                PrimaryBtn.Text = "Save new PIN";
                SecondaryLink.Text = "Cancel";
                ToolTipProperties.SetText(SecondaryLink, "Keep your current PIN.");
                break;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(150);
        PinEntry.Focus();
    }

    private void OnPinCompleted(object sender, EventArgs e)
    {
        if (ConfirmSection.IsVisible) ConfirmEntry.Focus();
        else OnPrimaryClicked(sender, e);
    }

    private async void OnPrimaryClicked(object sender, EventArgs e)
    {
        if (_busy) return;
        var pin = PinEntry.Text?.Trim() ?? "";

        if (_mode == Mode.Unlock)
        {
            if (pin.Length < 4) { ShowStatus("Enter your 4 to 6 digit PIN."); return; }
            SetBusy(true, "Unlocking…");
            var r = await SessionService.TryDeviceLogin(pin);
            if (r.Ok)
            {
                LoginPage.GoToApp();
                return;
            }
            SetBusy(false, r.Error ?? "Could not unlock.");
            PinEntry.Text = "";
            if (r.Forgotten)
            {
                await DisplayAlert("Sign in with your password", r.Error ?? "This PC is no longer remembered.", "OK");
                await Navigation.PopAsync();
                return;
            }
            PinEntry.Focus();
            return;
        }

        // Create / change
        var problem = SessionService.PinProblem(pin);
        if (problem != null) { ShowStatus(problem); PinEntry.Focus(); return; }
        if (pin != (ConfirmEntry.Text?.Trim() ?? "")) { ShowStatus("The two PINs don't match."); ConfirmEntry.Text = ""; ConfirmEntry.Focus(); return; }

        SetBusy(true, "Saving your PIN…");
        var (ok, error) = _mode == Mode.CreateNew
            ? await SessionService.RegisterDevice(pin)
            : await SessionService.SetDevicePin(pin);
        if (!ok)
        {
            SetBusy(false, error ?? "Could not save the PIN.");
            return;
        }

        if (_mode == Mode.Change)
        {
            await DisplayAlert("PIN changed", "Use your new PIN next time you open SmartCube on this PC.", "OK");
            await Navigation.PopAsync();
            return;
        }
        LoginPage.GoToApp();
    }

    private async void OnSecondaryTapped(object sender, EventArgs e)
    {
        if (_busy) return;
        switch (_mode)
        {
            case Mode.Unlock:
                await Navigation.PopAsync();          // back to the password sign-in
                break;
            case Mode.CreateNew:
                LoginPage.GoToApp();                  // signed in, just not remembered
                break;
            case Mode.CreateForExisting:
                SessionService.ForgetThisDevice();    // signed in this time; password next time
                LoginPage.GoToApp();
                break;
            case Mode.Change:
                await Navigation.PopAsync();
                break;
        }
    }

    private void SetBusy(bool busy, string status)
    {
        _busy = busy;
        PrimaryBtn.IsEnabled = !busy;
        PinEntry.IsEnabled = !busy;
        ConfirmEntry.IsEnabled = !busy;
        StatusLabel.TextColor = busy ? Color.FromArgb("#94A3B8") : Color.FromArgb("#F87171");
        ShowStatus(status);
    }

    private void ShowStatus(string text)
    {
        StatusLabel.Text = text ?? "";
        StatusLabel.IsVisible = !string.IsNullOrEmpty(text);
    }
}
