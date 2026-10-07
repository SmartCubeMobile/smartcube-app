using SmartCubeMobile.Dashboard.Faces;
using SmartCubeMobile.Services;

namespace SmartCubeMobile
{
    public partial class SmartCubeDashboard : ContentPage
    {
        private readonly Button[] navButtons;
        private readonly string[] faceNames = { "Banking", "Utility", "Crypto", "Exchange", "Investments", "Charts", "Subscriptions", "Insurance", "News", "Profile", "Help & support" };
        private readonly string[] faceSubtitles = {
            "Accounts and transactions",
            "Energy bills and usage",
            "Wallets and holdings",
            "Currency rates",
            "Stocks, shares and funds",
            "Financial visualisations",
            "Manage your recurring payments",
            "Car, house and life policies",
            "Market and finance news",
            "Settings and preferences",
            "Contact us, see replies and join live chat"
        };

        private SupportFace supportFace;
        private IDispatcherTimer supportTimer;
        private int? pendingSupportTicket;

        private BankingFace bankingFace;
        private UtilityFace utilityFace;
        private CryptoFace cryptoFace;
        private ExchangeFace exchangeFace;
        private InvestmentsFace investmentsFace;
        private ChartsFace chartsFace;
        private SubscriptionsFace subscriptionsFace;
        private InsuranceFace insuranceFace;
        private NewsFace newsFace;
        private ProfileFace profileFace;

        private int activeFace = -1;
        private bool isTransitioning;
        private IDispatcherTimer clockTimer;

        public SmartCubeDashboard()
        {
            InitializeComponent();

            UserLabel.Text = SessionService.DisplayName;
            UserInitials.Text = SessionService.InitialsFor(SessionService.DisplayName);

            // Test accounts show a banner while their activity is being shared.
            TestBanner.IsVisible = TestActivity.Active;
            TestActivity.ActiveChanged += () => MainThread.BeginInvokeOnMainThread(() => TestBanner.IsVisible = TestActivity.Active);

            navButtons = new[] { NavBanking, NavUtility, NavCrypto, NavExchange, NavInvestments, NavCharts, NavSubscriptions, NavInsurance, NavNews, NavProfile, NavSupport };
            NavSupport.Clicked += (s, e) => SwitchFace(10);

            NavBanking.Clicked += (s, e) => SwitchFace(0);
            NavUtility.Clicked += (s, e) => SwitchFace(1);
            NavCrypto.Clicked += (s, e) => SwitchFace(2);
            NavExchange.Clicked += (s, e) => SwitchFace(3);
            NavInvestments.Clicked += (s, e) => SwitchFace(4);
            NavCharts.Clicked += (s, e) => SwitchFace(5);
            NavSubscriptions.Clicked += (s, e) => SwitchFace(6);
            NavInsurance.Clicked += (s, e) => SwitchFace(7);
            NavNews.Clicked += (s, e) => SwitchFace(8);
            NavProfile.Clicked += (s, e) => SwitchFace(9);

            bankingFace = new BankingFace();
            utilityFace = new UtilityFace();
            cryptoFace = new CryptoFace();
            exchangeFace = new ExchangeFace();
            investmentsFace = new InvestmentsFace();
            chartsFace = new ChartsFace();
            subscriptionsFace = new SubscriptionsFace();
            insuranceFace = new InsuranceFace();
            newsFace = new NewsFace();
            profileFace = new ProfileFace();
            supportFace = new SupportFace();

            SwitchFace(9);
            StartClock();
            if (!AppPaths.IsDemo) StartSupportWatch();
            _ = SmartDataService.EnsureTokensFresh();
        }

        // Demo mode with --tour: shows each section in turn and saves a screenshot of it (made-up data only).
        public async Task RunDemoTour(string outDir)
        {
            if (!AppPaths.IsDemo) return;
            try
            {
                Directory.CreateDirectory(outDir);
                await Task.Delay(7000);

                async Task Shot(string name)
                {
                    var shot = await Screenshot.Default.CaptureAsync();
                    if (shot == null) return;
                    using var source = await shot.OpenReadAsync(ScreenshotFormat.Png);
                    using var file = File.Create(Path.Combine(outDir, name + ".png"));
                    await source.CopyToAsync(file);
                }
                async Task Go(int face, string name, int waitMs = 5000)
                {
                    while (isTransitioning) await Task.Delay(200);
                    SwitchFace(face);
                    await Task.Delay(waitMs);
                    await Shot(name);
                }

                var chatOnly = Environment.GetCommandLineArgs().Any(a => a.Equals("--chatonly", StringComparison.OrdinalIgnoreCase));
                if (chatOnly) goto Chat;
                if (Environment.GetCommandLineArgs().Any(a => a.Equals("--qronly", StringComparison.OrdinalIgnoreCase)))
                {
                    foreach (var (source, name) in new[] { ("Ledger", "15-wallet-qr"), ("Coinbase", "16-exchange-qr") })
                    {
                        await Navigation.PushAsync(new Dashboard.QrCodePage(source));
                        await Task.Delay(3000);
                        await Shot(name);
                        await Navigation.PopAsync();
                        await Task.Delay(1000);
                    }
                    File.WriteAllText(Path.Combine(outDir, "tour-done.txt"), DateTime.Now.ToString("s"));
                    return;
                }

                await Shot("10-profile");
                await Go(0, "02-banking");
                bankingFace.OpenAnalysis();
                await Task.Delay(2500);
                await Shot("03-banking-analysis");
                bankingFace.CloseAnalysis();
                await Go(1, "04-utility-bills");
                await Go(2, "05-crypto-portfolio", 8000);
                await Go(4, "06-investments");
                await Go(5, "07-charts", 7000);
                await Go(6, "08-subscriptions");
                await Go(7, "09-insurance");

                while (isTransitioning) await Task.Delay(200);
                SwitchFace(9);
                await Task.Delay(3000);
                profileFace.OpenFirstVehicleMaintenance();
                await Task.Delay(3000);
                await Shot("11-mot-vehicle");
                profileFace.CloseMaintenance();

                await Go(8, "12-money-news", 9000);

                pendingSupportTicket = DemoData.ChatTicketId;
                await Go(10, "13-support");

            Chat:
                // Live chat is a web page inside the app; it is captured from outside (see chat-ready.txt).
                if (!string.IsNullOrEmpty(DemoData.ChatUrl))
                {
                    await Navigation.PushAsync(new Dashboard.SupportChatPage(DemoData.ChatUrl, DemoData.ChatReference, DemoData.ChatSubject));
                    await Task.Delay(4000);
                    File.WriteAllText(Path.Combine(outDir, "chat-ready.txt"), "ready");
                    await Task.Delay(25000);
                    await Navigation.PopAsync();
                }
                File.WriteAllText(Path.Combine(outDir, "tour-done.txt"), DateTime.Now.ToString("s"));
            }
            catch (Exception ex)
            {
                try { File.WriteAllText(Path.Combine(outDir, "tour-done.txt"), "FAILED: " + ex); } catch { }
            }
        }

        private async void SwitchFace(int index)
        {
            if (index == activeFace || isTransitioning) return;
            isTransitioning = true;

            bool isFirstLoad = activeFace == -1;
            activeFace = index;

            for (int i = 0; i < navButtons.Length; i++)
            {
                navButtons[i].Style = (Style)(i == index
                    ? Resources.MergedDictionaries.FirstOrDefault()?["ActiveNavButton"]
                        ?? Application.Current.Resources["ActiveNavButton"]
                    : Resources.MergedDictionaries.FirstOrDefault()?["NavButton"]
                        ?? Application.Current.Resources["NavButton"]);
            }

            double indicatorY = index * 56 + 10;
            _ = NavIndicator.TranslateTo(0, indicatorY, isFirstLoad ? 0u : 300, Easing.CubicOut);

            if (!isFirstLoad)
            {
                await Task.WhenAll(
                    FaceContent.FadeTo(0, 150, Easing.CubicIn),
                    FaceContent.TranslateTo(-20, 0, 150, Easing.CubicIn)
                );
            }

            HeaderTitle.Opacity = 0;
            HeaderTitle.TranslationY = -8;
            HeaderTitle.Text = faceNames[index];

            HeaderSubtitle.Opacity = 0;
            HeaderSubtitle.Text = faceSubtitles[index];

            var face = GetFace(index);
            FaceContent.Content = face;
            FaceContent.Opacity = 0;
            FaceContent.TranslationX = 40;

            _ = HeaderTitle.FadeTo(1, 300, Easing.CubicOut);
            _ = HeaderTitle.TranslateTo(0, 0, 300, Easing.CubicOut);
            _ = HeaderSubtitle.FadeTo(1, 350, Easing.CubicOut);

            await Task.WhenAll(
                FaceContent.FadeTo(1, 350, Easing.CubicOut),
                FaceContent.TranslateTo(0, 0, 350, Easing.CubicOut)
            );

            if (index == 0)
                _ = bankingFace.ReloadAsync();
            if (index == 6)
                subscriptionsFace.Refresh();
            if (index == 10)
            {
                var open = pendingSupportTicket; pendingSupportTicket = null;
                _ = supportFace.RefreshAsync(open);
            }

            if (face is IAnimatedFace animatedFace)
                _ = animatedFace.PlayEntryAnimation();

            isTransitioning = false;
        }

        private View GetFace(int index) => index switch
        {
            0 => bankingFace,
            1 => utilityFace,
            2 => cryptoFace,
            3 => exchangeFace,
            4 => investmentsFace,
            5 => chartsFace,
            6 => subscriptionsFace,
            7 => insuranceFace,
            8 => newsFace,
            9 => profileFace,
            10 => supportFace,
            _ => bankingFace
        };

        // Checks for replies or live-chat invitations from support every two minutes (and shortly after
        // start-up), and offers to open them once per new event.
        private void StartSupportWatch()
        {
            supportTimer = Dispatcher.CreateTimer();
            supportTimer.Interval = TimeSpan.FromMinutes(2);
            supportTimer.Tick += async (s, e) => await CheckSupport();
            supportTimer.Start();
            Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(8), async () => await CheckSupport());
        }

        private bool checkingSupport;
        private async Task CheckSupport()
        {
            if (checkingSupport || SessionService.Current == null) return;
            checkingSupport = true;
            try
            {
                var (ok, _, tickets, _) = await SupportService.GetMyTickets();
                NavSupport.Text = ok && tickets.Any(SupportService.HasNews) ? "\U0001F4AC•" : "\U0001F4AC";
                if (!ok || activeFace == 10) return;
                var t = tickets.FirstOrDefault(SupportService.ShouldNotify);
                if (t == null) return;
                var what = t.ChatOffered ? "has invited you to a live chat about" : "has replied to";
                var go = await DisplayAlert("SmartCube support", $"Support {what} {t.Reference}: {t.Subject}", t.ChatOffered ? "Open" : "View", "Later");
                if (go) { pendingSupportTicket = t.Id; SwitchFace(10); }
            }
            catch { }
            finally { checkingSupport = false; }
        }

        private void StartClock()
        {
            UpdateClock();
            clockTimer = Dispatcher.CreateTimer();
            clockTimer.Interval = TimeSpan.FromSeconds(30);
            clockTimer.Tick += (s, e) => UpdateClock();
            clockTimer.Start();
        }

        private void UpdateClock()
        {
            ClockLabel.Text = DateTime.Now.ToString("HH:mm");
            DateLabel.Text = DateTime.Now.ToString("dd MMM");
        }

        private CancellationTokenSource _loadingPulseCts;

        public async Task ShowLoading(string message, string detail = "")
        {
            LoadingMessage.Text = message;
            LoadingDetail.Text = detail;
            LoadingBar.WidthRequest = 0;
            LoadingOverlay.Opacity = 0;
            LoadingOverlay.IsVisible = true;
            await LoadingOverlay.FadeTo(1, 200, Easing.CubicOut);
            _ = PulseLoadingImage();
            _ = AnimateLoadingBar();
        }

        public async Task UpdateLoading(string message, string detail = "", double progress = -1)
        {
            LoadingMessage.Text = message;
            LoadingDetail.Text = detail;
            if (progress >= 0)
            {
                _loadingPulseCts?.Cancel();
                LoadingBar.WidthRequest = 260 * Math.Min(progress, 1.0);
            }
        }

        public async Task HideLoading()
        {
            _loadingPulseCts?.Cancel();
            await LoadingOverlay.FadeTo(0, 300, Easing.CubicIn);
            LoadingOverlay.IsVisible = false;
        }

        private async Task PulseLoadingImage()
        {
            _loadingPulseCts?.Cancel();
            _loadingPulseCts = new CancellationTokenSource();
            var ct = _loadingPulseCts.Token;
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await LoadingImage.ScaleTo(1.05, 800, Easing.SinInOut);
                    if (ct.IsCancellationRequested) break;
                    await LoadingImage.ScaleTo(0.95, 800, Easing.SinInOut);
                }
            }
            catch (TaskCanceledException) { }
            LoadingImage.Scale = 1.0;
        }

        private async Task AnimateLoadingBar()
        {
            var ct = _loadingPulseCts?.Token ?? CancellationToken.None;
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    for (double w = 0; w <= 260 && !ct.IsCancellationRequested; w += 2.6)
                    {
                        LoadingBar.WidthRequest = w;
                        await Task.Delay(30, ct);
                    }
                    LoadingBar.WidthRequest = 0;
                }
            }
            catch (TaskCanceledException) { }
        }
    }
}
