using SmartCubeMobile.Services;

namespace SmartCubeMobile;

public partial class ChangePasswordPage : ContentPage
{
    private readonly bool _forced;

    public ChangePasswordPage(bool forced)
    {
        InitializeComponent();
        _forced = forced;
        if (forced)
        {
            IntroLabel.Text = "You signed in with a temporary password. Please choose a new one to continue.";
            CancelBtn.IsVisible = false;
        }
    }

    private void OnCurrentCompleted(object sender, EventArgs e) => NewEntry.Focus();
    private void OnNewCompleted(object sender, EventArgs e) => ConfirmEntry.Focus();
    private async void OnConfirmCompleted(object sender, EventArgs e) => await Save();
    private async void OnSaveClicked(object sender, EventArgs e) => await Save();

    private void OnShowClicked(object sender, EventArgs e)
    {
        var show = NewEntry.IsPassword;
        NewEntry.IsPassword = ConfirmEntry.IsPassword = !show;
        ShowBtn.Text = show ? "Hide" : "Show";
    }

    private async void OnCancelClicked(object sender, EventArgs e) => await Navigation.PopAsync();

    private async Task Save()
    {
        var current = CurrentEntry.Text ?? "";
        var next = NewEntry.Text ?? "";
        if (current.Length == 0 || next.Length == 0)
        {
            ShowStatus("Enter your current password and a new one.");
            return;
        }
        if (next != (ConfirmEntry.Text ?? ""))
        {
            ShowStatus("New passwords do not match.");
            return;
        }
        if (next == current)
        {
            ShowStatus("The new password must be different from the current one.");
            return;
        }

        SetBusy(true);
        var (ok, error) = await SessionService.ChangePassword(current, next);
        SetBusy(false);
        if (!ok)
        {
            ShowStatus(error);
            return;
        }

        if (_forced)
        {
            var dashboard = new SmartCubeDashboard();
            Navigation.InsertPageBefore(dashboard, this);
            await Navigation.PopAsync(false);
        }
        else
        {
            await DisplayAlert("Password Changed", "Your password has been updated for the app and the website.", "OK");
            await Navigation.PopAsync();
        }
    }

    private void SetBusy(bool busy)
    {
        SaveBtn.IsEnabled = CancelBtn.IsEnabled = !busy;
        CurrentEntry.IsEnabled = NewEntry.IsEnabled = ConfirmEntry.IsEnabled = !busy;
        SaveBtn.Text = busy ? "Please wait…" : "Change Password";
        if (busy) ShowStatus(null);
    }

    private void ShowStatus(string text)
    {
        StatusLabel.Text = text ?? "";
        StatusLabel.IsVisible = !string.IsNullOrEmpty(text);
    }
}
