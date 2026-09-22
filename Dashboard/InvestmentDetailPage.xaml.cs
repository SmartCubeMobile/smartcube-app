using SmartCubeMobile.MockData;
using System.Globalization;

namespace SmartCubeMobile.Dashboard
{
    public partial class InvestmentDetailPage : ContentPage
    {
        private readonly MockInvestment investment;
        private readonly CultureInfo culture = new("en-GB");
        private string activePeriod = "1M";

        public InvestmentDetailPage(MockInvestment inv)
        {
            InitializeComponent();
            investment = inv;
            LoadSummary();
            BuildPeriodButtons();
            LoadChart("1M");
            LoadPurchases();
        }

        private void LoadSummary()
        {
            HeaderSymbol.Text = investment.Symbol;
            HeaderName.Text = investment.Name;
            HeaderType.Text = investment.Type.ToUpper();
            HeaderType.TextColor = investment.Type switch
            {
                "ETF" => Color.FromArgb("#3B82F6"),
                "Investment Trust" => Color.FromArgb("#8B5CF6"),
                _ => Color.FromArgb("#06B6D4"),
            };

            var price = investment.PricePence / 100m;
            var value = investment.Shares * price;
            var cost = investment.Shares * investment.CostBasisPence / 100m;
            var pnl = value - cost;
            var pnlPct = cost > 0 ? pnl / cost * 100 : 0;

            CurrentPrice.Text = price.ToString("C", culture);
            MarketValue.Text = value.ToString("C", culture);
            TotalCost.Text = cost.ToString("C", culture);
            TotalPnl.Text = $"{(pnl >= 0 ? "+" : "")}{pnl.ToString("C", culture)} ({pnlPct:F1}%)";
            TotalPnl.TextColor = pnl >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");
            SharesHeld.Text = investment.Shares.ToString("N0");
            DivYield.Text = $"{investment.DividendYield:F1}%";
        }

        private void BuildPeriodButtons()
        {
            var periods = new[] { "1D", "1W", "1M", "3M", "6M", "1Y" };
            foreach (var p in periods)
            {
                var btn = new Button
                {
                    Text = p,
                    FontSize = 11,
                    Padding = new Thickness(10, 4),
                    CornerRadius = 8,
                    HeightRequest = 30,
                    MinimumHeightRequest = 28,
                    BackgroundColor = p == activePeriod ? Color.FromArgb("#3B82F6") : Color.FromArgb("#1C2744"),
                    TextColor = p == activePeriod ? Colors.White : Color.FromArgb("#94A3B8"),
                };
                var period = p;
                btn.Clicked += (s, e) =>
                {
                    activePeriod = period;
                    foreach (var child in PeriodButtons.Children.OfType<Button>())
                    {
                        child.BackgroundColor = child.Text == period ? Color.FromArgb("#3B82F6") : Color.FromArgb("#1C2744");
                        child.TextColor = child.Text == period ? Colors.White : Color.FromArgb("#94A3B8");
                    }
                    LoadChart(period);
                };
                PeriodButtons.Children.Add(btn);
            }
        }

        private void LoadChart(string period)
        {
            var data = MockDataService.GetPriceHistory(investment.Symbol, period);
            if (data.Count == 0) return;

            var low = data.Min(p => p.PricePence) / 100m;
            var high = data.Max(p => p.PricePence) / 100m;
            ChartLow.Text = $"Low: {low.ToString("C", culture)}";
            ChartHigh.Text = $"High: {high.ToString("C", culture)}";

            ChartView.Drawable = new PriceChartDrawable(data);
            ChartView.Invalidate();
        }

        private void LoadPurchases()
        {
            var purchases = MockDataService.GetPurchaseHistory(investment.Symbol);
            var now = DateTime.Now;

            foreach (var p in purchases)
            {
                var held = now - p.Date;
                var heldStr = held.Days > 365
                    ? $"{held.Days / 365}y {held.Days % 365 / 30}m"
                    : held.Days > 30
                        ? $"{held.Days / 30}m {held.Days % 30}d"
                        : $"{held.Days}d";

                var costPerShare = p.PricePence / 100m;
                var totalCost = p.Shares * costPerShare + p.FeePence / 100m;
                var currentVal = p.Shares * investment.PricePence / 100m;
                var profit = currentVal - totalCost;
                var profitPct = totalCost > 0 ? profit / totalCost * 100 : 0;

                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                    Stroke = Color.FromArgb("#1E2D4A"),
                    Padding = new Thickness(14, 10),
                };

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 14,
                };

                var dateStack = new VerticalStackLayout
                {
                    Spacing = 2, VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = p.Date.ToString("dd MMM yyyy"), TextColor = Color.FromArgb("#F1F5F9"), FontSize = 13, FontAttributes = FontAttributes.Bold },
                        new Label { Text = $"Held {heldStr}", TextColor = Color.FromArgb("#64748B"), FontSize = 11 },
                    }
                };

                var detailStack = new VerticalStackLayout
                {
                    Spacing = 2, VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = $"{p.Shares} shares @ {costPerShare.ToString("C", culture)}", TextColor = Color.FromArgb("#94A3B8"), FontSize = 12 },
                        new Label { Text = $"Fee: {(p.FeePence / 100m).ToString("C", culture)}", TextColor = Color.FromArgb("#64748B"), FontSize = 11 },
                    }
                };
                Grid.SetColumn(detailStack, 1);

                var profitStack = new VerticalStackLayout
                {
                    Spacing = 2, VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label
                        {
                            Text = $"{(profit >= 0 ? "+" : "")}{profit.ToString("C", culture)}",
                            TextColor = profit >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444"),
                            FontSize = 13, FontAttributes = FontAttributes.Bold,
                            HorizontalTextAlignment = TextAlignment.End,
                        },
                        new Label
                        {
                            Text = $"{(profitPct >= 0 ? "+" : "")}{profitPct:F1}%",
                            TextColor = profit >= 0 ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444"),
                            FontSize = 11,
                            HorizontalTextAlignment = TextAlignment.End,
                        },
                    }
                };
                Grid.SetColumn(profitStack, 2);

                grid.Children.Add(dateStack);
                grid.Children.Add(detailStack);
                grid.Children.Add(profitStack);
                card.Content = grid;
                PurchaseList.Children.Add(card);
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }

    public class PriceChartDrawable : IDrawable
    {
        private readonly List<MockPricePoint> data;

        public PriceChartDrawable(List<MockPricePoint> data) => this.data = data;

        public void Draw(ICanvas canvas, RectF rect)
        {
            if (data == null || data.Count < 2) return;

            float pad = 4;
            float w = rect.Width - pad * 2;
            float h = rect.Height - pad * 2;

            var min = data.Min(p => p.PricePence);
            var max = data.Max(p => p.PricePence);
            if (max == min) max = min + 1;
            var range = max - min;

            bool positive = data.Last().PricePence >= data.First().PricePence;
            var lineColour = positive ? Color.FromArgb("#22C55E") : Color.FromArgb("#EF4444");
            var fillColour = positive ? Color.FromArgb("#22C55E18") : Color.FromArgb("#EF444418");

            // Fill area
            var fillPath = new PathF();
            for (int i = 0; i < data.Count; i++)
            {
                float x = pad + w * i / (data.Count - 1);
                float y = pad + h - h * (float)((data[i].PricePence - min) / range);
                if (i == 0) fillPath.MoveTo(x, y);
                else fillPath.LineTo(x, y);
            }
            fillPath.LineTo(pad + w, pad + h);
            fillPath.LineTo(pad, pad + h);
            fillPath.Close();
            canvas.FillColor = fillColour;
            canvas.FillPath(fillPath);

            // Line
            canvas.StrokeColor = lineColour;
            canvas.StrokeSize = 2;
            for (int i = 1; i < data.Count; i++)
            {
                float x0 = pad + w * (i - 1) / (data.Count - 1);
                float y0 = pad + h - h * (float)((data[i - 1].PricePence - min) / range);
                float x1 = pad + w * i / (data.Count - 1);
                float y1 = pad + h - h * (float)((data[i].PricePence - min) / range);
                canvas.DrawLine(x0, y0, x1, y1);
            }

            // Current price dot
            float lastX = pad + w;
            float lastY = pad + h - h * (float)((data.Last().PricePence - min) / range);
            canvas.FillColor = lineColour;
            canvas.FillCircle(lastX, lastY, 4);
        }
    }
}
