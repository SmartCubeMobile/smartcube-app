using SmartCubeMobile.Dashboard;
using SmartCubeMobile.Services;

namespace SmartCubeMobile.Dashboard.Faces
{
    public partial class NewsFace : ContentView, IAnimatedFace
    {
        public NewsFace()
        {
            InitializeComponent();
            _ = LoadNewsAsync();
        }

        public async Task PlayEntryAnimation()
        {
            _ = AnimationHelper.FadeIn(NewsCard, 50, 400);
        }

        private async Task LoadNewsAsync()
        {
            try
            {
                var news = await FinanceNewsService.GetNewsAsync(15);
                MainThread.BeginInvokeOnMainThread(() => BuildNewsList(news));
            }
            catch
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    NewsList.Children.Add(new Label
                    {
                        Text = "Unable to load news",
                        TextColor = Color.FromArgb("#94A3B8"),
                        FontSize = 12,
                        HorizontalTextAlignment = TextAlignment.Center,
                    });
                });
            }
            finally
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    NewsSpinner.IsRunning = false;
                    NewsSpinner.IsVisible = false;
                });
            }
        }

        private void BuildNewsList(List<NewsItem> news)
        {
            NewsList.Children.Clear();
            if (news.Count == 0)
            {
                NewsList.Children.Add(new Label
                {
                    Text = "No news available",
                    TextColor = Color.FromArgb("#94A3B8"),
                    FontSize = 12,
                    HorizontalTextAlignment = TextAlignment.Center,
                });
                return;
            }

            foreach (var item in news)
            {
                var timeAgo = FormatTimeAgo(item.Published);

                var (arrowText, arrowColor) = item.Sentiment switch
                {
                    "bullish" => ("▲", "#22C55E"),
                    "bearish" => ("▼", "#EF4444"),
                    _ => ("●", "#64748B"),
                };

                var sourceColor = item.Source switch
                {
                    "CoinTelegraph" => ("#1A2E1A", "#34D399"),
                    "BBC Business" => ("#1E293B", "#94A3B8"),
                    _ => ("#1E293B", "#94A3B8"),
                };

                var row = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(32, GridUnitType.Absolute)),
                        new ColumnDefinition(GridLength.Star),
                    },
                    Padding = new Thickness(0, 10),
                    ColumnSpacing = 8,
                };

                var indicators = new VerticalStackLayout
                {
                    Spacing = 2,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Center,
                    Padding = new Thickness(0, 2, 0, 0),
                };

                indicators.Children.Add(new Label
                {
                    Text = item.Icon,
                    FontSize = 16,
                    HorizontalTextAlignment = TextAlignment.Center,
                });

                indicators.Children.Add(new Label
                {
                    Text = arrowText,
                    TextColor = Color.FromArgb(arrowColor),
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center,
                });

                row.Add(indicators, 0);

                var content = new VerticalStackLayout { Spacing = 4 };

                var header = new HorizontalStackLayout { Spacing = 8 };

                header.Children.Add(new Border
                {
                    BackgroundColor = Color.FromArgb(sourceColor.Item1),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(6, 1),
                    Content = new Label
                    {
                        Text = item.Source,
                        TextColor = Color.FromArgb(sourceColor.Item2),
                        FontSize = 9,
                    },
                    VerticalOptions = LayoutOptions.Center,
                });

                header.Children.Add(new Label
                {
                    Text = timeAgo,
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 10,
                    VerticalOptions = LayoutOptions.Center,
                });

                content.Children.Add(header);

                content.Children.Add(new Label
                {
                    Text = item.Title,
                    TextColor = Color.FromArgb("#E2E8F0"),
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold,
                    LineBreakMode = LineBreakMode.WordWrap,
                    MaxLines = 2,
                });

                if (!string.IsNullOrEmpty(item.Summary))
                {
                    content.Children.Add(new Label
                    {
                        Text = item.Summary,
                        TextColor = Color.FromArgb("#94A3B8"),
                        FontSize = 11,
                        LineBreakMode = LineBreakMode.WordWrap,
                        MaxLines = 2,
                    });
                }

                if (!string.IsNullOrEmpty(item.Url))
                {
                    var link = new Label
                    {
                        Text = "Read full article →",
                        TextColor = Color.FromArgb("#60A5FA"),
                        FontSize = 10,
                    };
                    var tapGesture = new TapGestureRecognizer();
                    var url = item.Url;
                    tapGesture.Tapped += async (s, ev) =>
                    {
                        try { await Launcher.Default.OpenAsync(new Uri(url)); } catch { }
                    };
                    link.GestureRecognizers.Add(tapGesture);
                    content.Children.Add(link);
                }

                row.Add(content, 1);
                NewsList.Children.Add(row);

                NewsList.Children.Add(new BoxView
                {
                    HeightRequest = 1,
                    Color = Color.FromArgb("#1E293B"),
                });
            }
        }

        private async void OnRefreshNewsClicked(object sender, EventArgs e)
        {
            RefreshNewsBtn.IsEnabled = false;
            NewsSpinner.IsRunning = true;
            NewsSpinner.IsVisible = true;
            FinanceNewsService.ClearCache();
            await LoadNewsAsync();
            RefreshNewsBtn.IsEnabled = true;
        }

        private static string FormatTimeAgo(DateTime published)
        {
            if (published == default) return "";
            var diff = DateTime.UtcNow - published.ToUniversalTime();
            if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes}m ago";
            if (diff.TotalHours < 24) return $"{(int)diff.TotalHours}h ago";
            return $"{(int)diff.TotalDays}d ago";
        }
    }
}
