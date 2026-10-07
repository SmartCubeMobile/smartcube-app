using QRCoder;
using SmartCubeMobile.Dashboard.Faces;
using SmartCubeMobile.Services;

namespace SmartCubeMobile.Dashboard
{
    // QR codes for one crypto source (a wallet or an exchange account), so its details can be scanned
    // with a phone instead of typed.
    //   Wallet:   the QR is the full address, exactly as any wallet app expects. An address is public:
    //             it lets someone view the wallet or send coins to it, never spend from it.
    //   Exchange: the QR holds the API key and secret, which give access to the account, so it stays
    //             hidden until the user confirms (and enters their PIN when this PC has one), and it
    //             hides itself again after a minute.
    public class QrCodePage : ContentPage
    {
        private const int RevealSeconds = 60;
        private readonly VerticalStackLayout _list = new() { Spacing = 14 };

        public QrCodePage(string sourceName)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            BackgroundColor = Color.FromArgb("#0A0F1E");

            var back = new Button
            {
                Text = "← Back", BackgroundColor = Color.FromArgb("#1E2D4A"), TextColor = Color.FromArgb("#CBD5E1"),
                FontSize = 13, CornerRadius = 8, Padding = new Thickness(14, 0), HeightRequest = 34, VerticalOptions = LayoutOptions.Center,
            };
            back.Clicked += async (s, e) => await Navigation.PopAsync();
            ToolTipProperties.SetText(back, "Go back to your crypto portfolio.");

            var header = new HorizontalStackLayout
            {
                Spacing = 14,
                Children =
                {
                    back,
                    new VerticalStackLayout
                    {
                        Spacing = 2,
                        Children =
                        {
                            new Label { Text = "QR codes", FontSize = 22, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#F1F5F9") },
                            new Label { Text = sourceName, FontSize = 13, TextColor = Color.FromArgb("#94A3B8") },
                        }
                    },
                }
            };

            Content = new ScrollView
            {
                Content = new VerticalStackLayout { Padding = new Thickness(24, 20), Spacing = 18, Children = { header, _list } }
            };

            Build(sourceName);
        }

        private void Build(string sourceName)
        {
            var connections = CryptoStorageService.LoadConnections()
                .Where(c => c.Symbol != "LABEL" && CryptoFace.GetConnectionDisplayName(c) == sourceName)
                .ToList();

            if (connections.Count == 0)
            {
                _list.Add(Card(new Label
                {
                    Text = "SmartCube has no saved address or API key for this source, so there is nothing to put in a QR code. " +
                           "That happens when the assets were added by hand, or the source was added before addresses were saved; remove it and add it again by address to get a QR code.",
                    FontSize = 13, TextColor = Color.FromArgb("#94A3B8"), LineBreakMode = LineBreakMode.WordWrap,
                }));
                return;
            }

            foreach (var c in connections.Where(c => c.Type == "wallet" && !string.IsNullOrWhiteSpace(c.Address)))
                _list.Add(WalletCard(c));
            foreach (var c in connections.Where(c => c.Type == "exchange"))
                _list.Add(ExchangeCard(c));
        }

        #region Building blocks

        private static Border Card(View content) => new()
        {
            BackgroundColor = Color.FromArgb("#131B2E"), Stroke = Color.FromArgb("#1E2D4A"), StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            Padding = new Thickness(20), MaximumWidthRequest = 860, HorizontalOptions = LayoutOptions.Start, Content = content,
        };

        // Dark modules on white with a quiet zone, which is what phone cameras read most reliably.
        private static ImageSource QrImage(string text, QRCodeGenerator.ECCLevel level = QRCodeGenerator.ECCLevel.M)
        {
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(text, level);
            var png = new PngByteQRCode(data).GetGraphic(10);
            return ImageSource.FromStream(() => new MemoryStream(png));
        }

        private static Border QrFrame(ImageSource source, double size) => new()
        {
            BackgroundColor = Colors.White, Stroke = Colors.Transparent, Padding = new Thickness(10),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
            WidthRequest = size, HeightRequest = size, VerticalOptions = LayoutOptions.Start,
            Content = new Image { Source = source, Aspect = Aspect.AspectFit },
        };

        private static Button CopyButton(string label, string value, string tip)
        {
            var btn = new Button
            {
                Text = label, BackgroundColor = Color.FromArgb("#1E3A5F"), TextColor = Color.FromArgb("#60A5FA"),
                FontSize = 12, FontAttributes = FontAttributes.Bold, CornerRadius = 8, Padding = new Thickness(14, 0), HeightRequest = 32,
                HorizontalOptions = LayoutOptions.Start,
            };
            ToolTipProperties.SetText(btn, tip);
            btn.Clicked += async (s, e) =>
            {
                await Clipboard.Default.SetTextAsync(value);
                btn.Text = "Copied";
                await Task.Delay(1500);
                btn.Text = label;
            };
            return btn;
        }

        #endregion

        private static View WalletCard(SavedConnection c)
        {
            var title = string.IsNullOrEmpty(c.Label) ? $"{c.Symbol} wallet" : $"{c.Label} · {c.Symbol}";
            var info = new VerticalStackLayout
            {
                Spacing = 8, VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label { Text = title, FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#F1F5F9") },
                    new Label { Text = $"{c.Symbol} address", FontSize = 11, TextColor = Color.FromArgb("#64748B") },
                    new Label { Text = c.Address, FontSize = 13, FontFamily = "Consolas", TextColor = Color.FromArgb("#E2E8F0"), LineBreakMode = LineBreakMode.CharacterWrap },
                    CopyButton("Copy address", c.Address, "Copy the full wallet address to the clipboard."),
                    new Label
                    {
                        Text = "Scan the code with a phone to pick up this address without typing it. A wallet address is safe to share: " +
                               "it lets someone view the wallet or send coins to it, never take coins out.",
                        FontSize = 12, TextColor = Color.FromArgb("#94A3B8"), LineBreakMode = LineBreakMode.WordWrap,
                    },
                }
            };
            var grid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) },
                ColumnSpacing = 22,
            };
            var qr = QrFrame(QrImage(c.Address), 240);
            ToolTipProperties.SetText(qr, "QR code of this wallet's address.");
            grid.Add(qr, 0);
            grid.Add(info, 1);
            return Card(grid);
        }

        private View ExchangeCard(SavedConnection c)
        {
            var name = c.Label ?? c.ExchangeName ?? "Exchange";
            var hasKey = !string.IsNullOrWhiteSpace(c.ApiKey) && !string.IsNullOrWhiteSpace(c.Secret);
            var needsPin = !AppPaths.IsDemo && SessionService.DeviceHasPin && !string.IsNullOrEmpty(SessionService.DeviceToken);

            var status = new Label { FontSize = 12, TextColor = Color.FromArgb("#F87171"), IsVisible = false, LineBreakMode = LineBreakMode.WordWrap };
            var pinEntry = new Entry
            {
                Placeholder = "Your SmartCube PIN", IsPassword = true, Keyboard = Keyboard.Numeric, MaxLength = 6,
                FontSize = 14, TextColor = Color.FromArgb("#F1F5F9"), PlaceholderColor = Color.FromArgb("#475569"),
                BackgroundColor = Color.FromArgb("#0A0F1E"), WidthRequest = 200, HorizontalOptions = LayoutOptions.Start, IsVisible = needsPin,
            };
            ToolTipProperties.SetText(pinEntry, "The PIN you use to open SmartCube on this PC. It is checked before the API key is shown.");

            var showBtn = new Button
            {
                Text = "Show QR code", BackgroundColor = Color.FromArgb("#7C2D12"), TextColor = Color.FromArgb("#FDBA74"),
                FontSize = 13, FontAttributes = FontAttributes.Bold, CornerRadius = 8, Padding = new Thickness(16, 0), HeightRequest = 36,
                HorizontalOptions = LayoutOptions.Start, IsEnabled = hasKey,
            };
            ToolTipProperties.SetText(showBtn, "Show a QR code containing this exchange account's API key and secret for one minute.");

            var qrHolder = new ContentView { IsVisible = false };
            var countdown = new Label { FontSize = 12, TextColor = Color.FromArgb("#FDBA74"), IsVisible = false };
            int showing = 0;   // increases on every reveal/hide, so an old countdown stops itself

            void Hide()
            {
                showing++;
                qrHolder.Content = null;
                qrHolder.IsVisible = false;
                countdown.IsVisible = false;
                showBtn.Text = "Show QR code";
                pinEntry.Text = "";
                pinEntry.IsVisible = needsPin;
            }

            showBtn.Clicked += async (s, e) =>
            {
                if (qrHolder.IsVisible) { Hide(); return; }
                status.IsVisible = false;

                var go = await DisplayAlert("Show API key as a QR code?",
                    $"This QR code contains the API key and secret for {name}. Anyone who scans or photographs it gets the same access to the account as SmartCube has.\n\n" +
                    "Only show it when nobody else can see your screen, and never share a picture of it. It hides itself after one minute.",
                    "Show it", "Cancel");
                if (!go) return;

                if (needsPin)
                {
                    var pin = pinEntry.Text?.Trim();
                    if (string.IsNullOrEmpty(pin)) { status.Text = "Enter your SmartCube PIN first."; status.IsVisible = true; pinEntry.Focus(); return; }
                    showBtn.IsEnabled = false;
                    var check = await SessionService.TryDeviceLogin(pin);
                    showBtn.IsEnabled = true;
                    if (!check.Ok)
                    {
                        pinEntry.Text = "";
                        status.Text = check.Forgotten
                            ? "Too many wrong PINs: this PC is no longer remembered. Sign in again with your password."
                            : check.PinRequired ? $"That PIN isn't right.{(check.AttemptsLeft > 0 ? $" {check.AttemptsLeft} attempt(s) left." : "")}"
                            : check.Error ?? "Couldn't check your PIN.";
                        status.IsVisible = true;
                        return;
                    }
                }

                var payload = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    smartcube = "exchange", exchange = c.ExchangeName ?? name, label = c.Label, apiKey = c.ApiKey, secret = c.Secret,
                });
                qrHolder.Content = QrFrame(QrImage(payload, QRCodeGenerator.ECCLevel.L), 320);
                qrHolder.IsVisible = true;
                pinEntry.IsVisible = false;
                showBtn.Text = "Hide QR code";
                countdown.IsVisible = true;

                var mine = ++showing;
                for (int left = RevealSeconds; left > 0 && mine == showing; left--)
                {
                    countdown.Text = $"Hides in {left} second{(left == 1 ? "" : "s")}";
                    await Task.Delay(1000);
                }
                if (mine == showing) Hide();
            };

            Disappearing += (s, e) => Hide();

            var body = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label { Text = $"{name} · exchange account", FontSize = 17, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#F1F5F9") },
                    new Label
                    {
                        Text = hasKey
                            ? "An exchange account has no single address. Its QR code holds the API key and secret SmartCube uses to read the account, so another device can be set up without typing them. " +
                              "Treat it like a password: it is hidden until you ask for it."
                            : "SmartCube has no API key saved for this exchange account, so there is nothing to put in a QR code.",
                        FontSize = 12, TextColor = Color.FromArgb("#94A3B8"), LineBreakMode = LineBreakMode.WordWrap,
                    },
                    pinEntry, showBtn, status, countdown, qrHolder,
                }
            };
            return Card(body);
        }
    }
}
