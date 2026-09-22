using SmartCubeMobile.Dashboard.Faces;
using SmartCubeMobile.Services;

namespace SmartCubeMobile
{
    public partial class SmartCubeDashboard : ContentPage
    {
        private readonly Button[] navButtons;
        private readonly string[] faceNames = { "Banking", "Utility", "Crypto", "Exchange", "Investments", "Charts", "Subscriptions", "Insurance", "News", "Profile" };
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
            "Settings and preferences"
        };

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

            navButtons = new[] { NavBanking, NavUtility, NavCrypto, NavExchange, NavInvestments, NavCharts, NavSubscriptions, NavInsurance, NavNews, NavProfile };

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

            SwitchFace(9);
            StartClock();
            _ = SmartDataService.EnsureTokensFresh();
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
            _ => bankingFace
        };

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
