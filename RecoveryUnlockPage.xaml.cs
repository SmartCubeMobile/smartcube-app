using SmartCubeMobile.Services;

namespace SmartCubeMobile;

// Shown after a successful password sign-in when that password no longer opens the data on this PC
// (it was reset on the website, or the data belongs to another account). The recovery key, or the PIN
// on a remembered PC, opens the data key; it is then re-locked with the current password.
public partial class RecoveryUnlockPage : ContentPage
{
    private readonly string _username;
    private readonly string _password;
    private readonly bool _remember;
    private bool _busy;

    public RecoveryUnlockPage(KeyVault.UnlockResult reason, string username, string password, bool remember)
    {
        InitializeComponent();
        _username = username;
        _password = password;
        _remember = remember;

        if (reason == KeyVault.UnlockResult.WrongOwner)
        {
            TitleLabel.Text = "This PC holds another account's data";
            IntroLabel.Text = $"The SmartCube data on this PC belongs to the account \"{KeyVault.Owner}\". " +
                              "Sign in as that account, or enter that data's recovery key to move it to your account.";
        }
        else
        {
            TitleLabel.Text = "Your password has changed";
            IntroLabel.Text = "Your SmartCube password was changed since this PC last opened your data, so it can't unlock it any more. " +
                              "Enter your recovery key and your data will be re-locked with your new password.";
        }

        PinBtn.IsVisible = reason == KeyVault.UnlockResult.WrongPassword
                           && KeyVault.HasDeviceSlot
                           && SessionService.DeviceHasPin
                           && !string.IsNullOrEmpty(SessionService.DeviceToken);
    }

    private async void OnUnlockClicked(object sender, EventArgs e)
    {
        if (_busy) return;
        var key = KeyEntry.Text ?? "";
        if (!KeyVault.LooksLikeRecoveryKey(key)) { ShowStatus("That doesn't look like a recovery key: it's 24 letters and numbers, usually in six groups of four."); return; }

        SetBusy(true, "Checking your recovery key…");
        var ok = await Task.Run(() => KeyVault.UnlockWithRecoveryKey(key));
        if (!ok) { SetBusy(false, "That recovery key doesn't open the data on this PC. Check each group of four and try again."); return; }
        await Relock();
    }

    private async void OnUsePinClicked(object sender, EventArgs e)
    {
        if (_busy) return;
        var pin = await DisplayPromptAsync("Your PIN", "Enter the PIN you use to open SmartCube on this PC.",
            "Unlock", "Cancel", placeholder: "PIN", maxLength: 6, keyboard: Keyboard.Numeric);
        if (string.IsNullOrWhiteSpace(pin)) return;

        SetBusy(true, "Checking your PIN…");
        var r = await SessionService.TryDeviceLogin(pin.Trim());
        if (!r.Ok)
        {
            SetBusy(false, r.Error ?? "Could not check the PIN.");
            if (r.Forgotten) PinBtn.IsVisible = false;
            return;
        }
        var ok = await Task.Run(() => KeyVault.UnlockWithDevice(r.DeviceKey));
        if (!ok) { SetBusy(false, "Your PIN is right, but this PC's key didn't open the data. Use your recovery key instead."); return; }
        await Relock();
    }

    // Data key is open: lock it with the current password, then carry on into the app.
    private async Task Relock()
    {
        SetBusy(true, "Re-locking your data with your new password…");
        try { await Task.Run(() => KeyVault.SetPassword(_username, _password)); }
        catch (Exception ex) { SetBusy(false, "Could not update the lock: " + ex.Message); return; }
        await LoginPage.ContinueAfterUnlock(Navigation, _remember);
    }

    private async void OnNoKeyTapped(object sender, EventArgs e)
    {
        if (_busy) return;
        var start = await DisplayAlert("Without your recovery key",
            "Your data on this PC can only be opened with your old password, your recovery key, or (on a remembered PC) your PIN. " +
            "SmartCube can't open it for you.\n\n" +
            "If you can't find any of these, you can start again with an empty SmartCube. Your old data is moved into a separate folder, not deleted, so it can still be opened if the recovery key turns up.",
            "Start again", "Keep looking");
        if (!start) return;

        var sure = await DisplayAlert("Start again?",
            "SmartCube will open empty on this PC and show you a new recovery key. Continue?", "Start again", "Cancel");
        if (!sure) return;

        SetBusy(true, "Setting up a fresh start…");
        try
        {
            var movedTo = await Task.Run(KeyVault.MoveDataAsideAndReset);
            SessionService.ForgetThisDevice();
            var result = await Task.Run(() => KeyVault.UnlockWithPassword(_username, _password));   // creates a new vault
            await DisplayAlert("Fresh start", $"Your old data has been kept in:\n{movedTo}", "OK");
            await LoginPage.ContinueAfterUnlock(Navigation, _remember);
        }
        catch (Exception ex)
        {
            SetBusy(false, "Could not start again: " + ex.Message);
        }
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        if (_busy) return;
        await SessionService.Logout();
        var window = Application.Current?.Windows.FirstOrDefault();
        if (window != null) window.Page = new NavigationPage(new LoginPage());
    }

    private void SetBusy(bool busy, string status)
    {
        _busy = busy;
        UnlockBtn.IsEnabled = !busy;
        PinBtn.IsEnabled = !busy;
        KeyEntry.IsEnabled = !busy;
        StatusLabel.TextColor = busy ? Color.FromArgb("#94A3B8") : Color.FromArgb("#F87171");
        ShowStatus(status);
    }

    private void ShowStatus(string text)
    {
        StatusLabel.Text = text ?? "";
        StatusLabel.IsVisible = !string.IsNullOrEmpty(text);
    }
}
