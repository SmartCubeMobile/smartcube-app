using SmartCubeMobile.Services;

namespace SmartCubeMobile;

// Shows a newly created recovery key once. The user must confirm they've saved it before continuing.
public partial class RecoveryKeyPage : ContentPage
{
    private readonly string _key;
    private readonly Func<Task> _then;

    public RecoveryKeyPage(string recoveryKey, Func<Task> then)
    {
        InitializeComponent();
        _key = recoveryKey;
        _then = then;
        KeyLabel.Text = recoveryKey;
    }

    protected override bool OnBackButtonPressed() => true;   // must be acknowledged

    private async void OnCopyClicked(object sender, EventArgs e)
    {
        try
        {
            await Clipboard.Default.SetTextAsync(_key);
            CopyBtn.Text = "Copied";
        }
        catch { }
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(docs, "SmartCube recovery key.txt");
            File.WriteAllText(path,
                "SmartCube recovery key\r\n" +
                "======================\r\n\r\n" +
                $"    {_key}\r\n\r\n" +
                $"Account: {SessionService.Current?.Username}\r\n" +
                $"Created: {DateTime.Now:dd MMMM yyyy}\r\n\r\n" +
                "Use this if you forget your SmartCube password, or to open your data on a new or reset PC.\r\n" +
                "SmartCube cannot recover this key for you. Print it and keep it with your important papers,\r\n" +
                "then delete this file from the PC.\r\n");
            SavedLabel.Text = $"Saved to {path}. Print it, then delete the file from this PC.";
            SavedLabel.IsVisible = true;
        }
        catch (Exception ex)
        {
            SavedLabel.Text = "Could not save the file: " + ex.Message;
            SavedLabel.TextColor = Color.FromArgb("#F87171");
            SavedLabel.IsVisible = true;
        }
    }

    private void OnSavedChanged(object sender, CheckedChangedEventArgs e) => ContinueBtn.IsEnabled = e.Value;
    private void OnSavedLabelTapped(object sender, EventArgs e) => SavedCheck.IsChecked = !SavedCheck.IsChecked;

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        if (!SavedCheck.IsChecked) return;
        ContinueBtn.IsEnabled = false;
        try { KeyVault.ConfirmRecoveryKey(); } catch { }
        if (_then != null) await _then();
    }
}
