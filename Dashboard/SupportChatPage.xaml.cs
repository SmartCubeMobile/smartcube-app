namespace SmartCubeMobile.Dashboard
{
    // Live chat with support, shown inside the app. It hosts the same chat page the email button opens
    // (site/chat.html in its compact "embed" mode), so the customer and admin sides stay identical.
    public partial class SupportChatPage : ContentPage
    {
        public SupportChatPage(string chatUrl, string reference, string subject)
        {
            InitializeComponent();
            SubtitleLabel.Text = string.IsNullOrEmpty(reference) ? "" : $"{reference} · {subject}";
            var url = chatUrl + (chatUrl.Contains('?') ? "&" : "?") + "embed=1";
            ChatView.Source = new UrlWebViewSource { Url = url };
        }

        private void OnNavigated(object sender, WebNavigatedEventArgs e) => Busy.IsRunning = false;

        private async void OnBackClicked(object sender, EventArgs e)
        {
            // Unload the page so the chat connection closes straight away and support sees you left.
            ChatView.Source = new HtmlWebViewSource { Html = "<html><body style='background:#0A0F1E'></body></html>" };
            await Navigation.PopAsync();
        }
    }
}
