using SmartCubeMobile.Services;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace SmartCubeMobile.Dashboard
{
    public partial class ConnectBankPage : ContentPage
    {
        // The client id is public (it's in the bank-consent URL). The client secret lives on the
        // SmartCube server, which does the token exchange for us.
        private const string TL_CLIENT_ID = "smartcube-3d8e01";
        private const string TL_REDIRECT_URI = "http://localhost:3000/callback";

        public event Action BankConnected;
        private CancellationTokenSource _listenerCts;

        public ConnectBankPage()
        {
            InitializeComponent();
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            if (SmartDataService.Connections.Count > 0)
            {
                var anyExpired = SmartDataService.Connections.Any(c => c.Status == "Expired");
                var anyConnected = SmartDataService.Connections.Any(c => c.Status == "Connected");

                if (anyExpired && !anyConnected)
                {
                    StatusIcon.Text = "\U0001F7E0";
                    StatusLabel.Text = "Session Expired";
                }
                else if (anyExpired)
                {
                    StatusIcon.Text = "\U0001F7E0";
                    StatusLabel.Text = "Partially Connected";
                }
                else
                {
                    StatusIcon.Text = "\U0001F7E2";
                    StatusLabel.Text = "Connected";
                }

                var bankNames = SmartDataService.Connections
                    .Select(c =>
                    {
                        if (c.Provider.StartsWith("TrueLayer:"))
                            return c.Provider.Substring(10);
                        if (c.Provider == "TrueLayer")
                            return "Bank via TrueLayer";
                        return c.Provider;
                    })
                    .ToList();
                StatusDetail.Text = bankNames.Count == 1
                    ? bankNames[0]
                    : string.Join(", ", bankNames);

                ConnectionsCard.IsVisible = true;
                ConnectionsList.Children.Clear();

                foreach (var conn in SmartDataService.Connections)
                {
                    var row = new Border
                    {
                        BackgroundColor = Color.FromArgb("#1C2744"),
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                        Stroke = Colors.Transparent,
                        Padding = new Thickness(14, 10),
                    };

                    var grid = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition(GridLength.Auto),
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto),
                            new ColumnDefinition(GridLength.Auto),
                        },
                        ColumnSpacing = 12,
                    };

                    var icon = new Label
                    {
                        Text = "\U0001F3E6",
                        FontSize = 18,
                        VerticalOptions = LayoutOptions.Center,
                    };
                    var displayName = conn.Provider.StartsWith("TrueLayer:")
                        ? conn.Provider.Substring(10)
                        : conn.Provider == "TrueLayer"
                            ? "Bank via TrueLayer"
                            : conn.Provider;
                    var isExpired = conn.Status == "Expired";
                    var statusText = isExpired ? "Expired" : conn.Status;
                    var dateText = isExpired
                        ? "Reconnect to refresh"
                        : $"Connected {conn.ConnectedAt:dd MMM yyyy HH:mm}";

                    var info = new VerticalStackLayout
                    {
                        Spacing = 1, VerticalOptions = LayoutOptions.Center,
                        Children =
                        {
                            new Label { Text = displayName, TextColor = Color.FromArgb("#F1F5F9"), FontSize = 14, FontAttributes = FontAttributes.Bold },
                            new Label { Text = dateText, TextColor = Color.FromArgb(isExpired ? "#F59E0B" : "#64748B"), FontSize = 11 },
                        }
                    };
                    var status = new Label
                    {
                        Text = statusText,
                        TextColor = isExpired ? Color.FromArgb("#F59E0B") : (conn.Status == "Connected" ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444")),
                        FontSize = 12, FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.Center,
                    };
                    var provider = conn.Provider;
                    var disconnectBtn = new Button
                    {
                        Text = "Remove",
                        BackgroundColor = Color.FromArgb("#3B1A1A"),
                        TextColor = Color.FromArgb("#EF4444"),
                        FontSize = 10,
                        CornerRadius = 6,
                        Padding = new Thickness(8, 2),
                        HeightRequest = 26,
                        VerticalOptions = LayoutOptions.Center,
                    };
                    ToolTipProperties.SetText(disconnectBtn, $"Disconnect {provider} and remove its accounts and transactions from SmartCube.");
                    disconnectBtn.Clicked += async (s, ev) =>
                    {
                        var confirm = await DisplayAlert("Remove Connection",
                            $"Disconnect {provider}? Your accounts and transactions from this provider will be removed.",
                            "Remove", "Cancel");
                        if (!confirm) return;
                        SmartDataService.DisconnectBank(provider);
                        RefreshStatus();
                        BankConnected?.Invoke();
                    };

                    Grid.SetColumn(info, 1);
                    Grid.SetColumn(status, 2);
                    Grid.SetColumn(disconnectBtn, 3);

                    grid.Children.Add(icon);
                    grid.Children.Add(info);
                    grid.Children.Add(status);
                    grid.Children.Add(disconnectBtn);
                    row.Content = grid;
                    ConnectionsList.Children.Add(row);
                }
            }
            else
            {
                StatusIcon.Text = "\U0001F534";
                StatusLabel.Text = "Not Connected";
                StatusDetail.Text = "No bank accounts linked";
                ConnectionsCard.IsVisible = false;
            }
        }

        private async void OnTrueLayerConnectClicked(object sender, EventArgs e)
        {
            var authCode = TLAuthCodeEntry.Text?.Trim();

            if (string.IsNullOrEmpty(authCode))
            {
                TLErrorLabel.Text = "Auth Code is required. Tap 'Open Bank Login' to start.";
                TLErrorLabel.IsVisible = true;
                return;
            }

            TLErrorLabel.IsVisible = false;
            TLConnectBtn.IsEnabled = false;
            TLConnectBtn.Text = "Connecting...";

            var success = await SmartDataService.ConnectTrueLayerWithCode(authCode, TL_REDIRECT_URI);

            TLConnectBtn.IsEnabled = true;
            TLConnectBtn.Text = "Connect";

            if (success)
            {
                TLAuthCodeEntry.Text = "";
                RefreshStatus();
                BankConnected?.Invoke();
                await DisplayAlert("Connected", "Bank account linked successfully via TrueLayer. Your live data will now appear in Banking.", "OK");
            }
            else
            {
                var error = SmartDataService.LastError ?? "Connection failed.";
                TLErrorLabel.Text = error;
                TLErrorLabel.IsVisible = true;
            }
        }

        private async void OnOpenTrueLayerAuthClicked(object sender, EventArgs e)
        {
            TLErrorLabel.IsVisible = false;
            TLOpenAuthBtn.IsEnabled = false;
            TLOpenAuthBtn.Text = "Waiting for authorisation...";

            _listenerCts?.Cancel();
            _listenerCts = new CancellationTokenSource();

            string authCode = null;
            TcpListener tcp = null;

            try
            {
                tcp = new TcpListener(IPAddress.Loopback, 3000);
                tcp.Start();

                var authUrl = $"https://auth.truelayer.com/?response_type=code&client_id={Uri.EscapeDataString(TL_CLIENT_ID)}&scope=info%20accounts%20balance%20transactions%20cards%20offline_access&redirect_uri={Uri.EscapeDataString(TL_REDIRECT_URI)}&providers=uk-ob-all";
                await Launcher.OpenAsync(new Uri(authUrl));

                using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(_listenerCts.Token);
                timeoutCts.CancelAfter(TimeSpan.FromMinutes(5));

                var client = await tcp.AcceptTcpClientAsync(timeoutCts.Token);
                using var stream = client.GetStream();

                var buffer = new byte[8192];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, timeoutCts.Token);
                var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                var firstLine = request.Split('\n')[0];
                var urlPart = firstLine.Split(' ').Length > 1 ? firstLine.Split(' ')[1] : "";

                if (urlPart.Contains("code="))
                {
                    var queryString = urlPart.Contains("?") ? urlPart.Substring(urlPart.IndexOf('?') + 1) : "";
                    var queryParams = HttpUtility.ParseQueryString(queryString);
                    authCode = queryParams["code"];
                }

                var responseHtml = "<html><body style='font-family:system-ui;text-align:center;padding:60px;background:#0A0F1E;color:#E2E8F0'>"
                    + "<h1 style='color:#22C55E'>&#10003; Authorised</h1>"
                    + "<p>You can close this tab and return to SmartCube.</p></body></html>";
                var httpResponse = $"HTTP/1.1 200 OK\r\nContent-Type: text/html\r\nContent-Length: {Encoding.UTF8.GetByteCount(responseHtml)}\r\nConnection: close\r\n\r\n{responseHtml}";
                var responseBytes = Encoding.UTF8.GetBytes(httpResponse);
                await stream.WriteAsync(responseBytes);
                await stream.FlushAsync();
                client.Close();
            }
            catch (OperationCanceledException)
            {
                if (_listenerCts.IsCancellationRequested)
                    return;
                TLErrorLabel.Text = "Timed out waiting for bank authorisation. Try again.";
                TLErrorLabel.IsVisible = true;
                return;
            }
            catch (Exception ex)
            {
                TLErrorLabel.Text = $"Error: {ex.Message}";
                TLErrorLabel.IsVisible = true;
                return;
            }
            finally
            {
                try { tcp?.Stop(); } catch { }
                TLOpenAuthBtn.IsEnabled = true;
                TLOpenAuthBtn.Text = "Link Bank Account";
            }

            if (string.IsNullOrEmpty(authCode))
            {
                TLErrorLabel.Text = "No auth code received. The bank may have denied access.";
                TLErrorLabel.IsVisible = true;
                return;
            }

            TLConnectBtn.IsEnabled = false;
            TLConnectBtn.Text = "Connecting...";

            var success = await SmartDataService.ConnectTrueLayerWithCode(authCode, TL_REDIRECT_URI);

            TLConnectBtn.IsEnabled = true;
            TLConnectBtn.Text = "Connect";

            if (success)
            {
                TLAuthCodeEntry.Text = "";
                RefreshStatus();
                BankConnected?.Invoke();
                await DisplayAlert("Connected", "Bank account linked successfully via TrueLayer. Your live data will now appear in Banking.", "OK");
            }
            else
            {
                var error = SmartDataService.LastError ?? "Connection failed.";
                TLErrorLabel.Text = error;
                TLErrorLabel.IsVisible = true;
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            _listenerCts?.Cancel();
            await Navigation.PopAsync();
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Bank Connections",
                "This screen links your UK bank accounts using TrueLayer, so SmartCube never sees your bank password. Tap 'Link Bank Account' to authorise access in your browser; the authorisation code returns automatically, or you can paste it in yourself. Once connected, your accounts and transactions appear in Banking. Use 'Remove' to disconnect a bank and delete its accounts and transactions from this PC.",
                "OK");
        }
    }
}
