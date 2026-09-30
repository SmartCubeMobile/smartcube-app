using System.Globalization;
using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;

namespace SmartCubeMobile.Dashboard.Faces
{
    public partial class SubscriptionsFace : ContentView, IAnimatedFace
    {
        private static readonly CultureInfo _gbp = new("en-GB");
        public SubscriptionsFace()
        {
            InitializeComponent();
            LoadData();
            _ = DetectLiveSubscriptions();
        }

        private async Task DetectLiveSubscriptions()
        {
            if (!SmartDataService.HasLiveConnection) return;
            try
            {
                var transactions = await SmartDataService.GetTransactions();
                if (transactions.Count > 0)
                {
                    MockDataService.DetectSubscriptionsFromLiveTransactions(transactions);
                    MainThread.BeginInvokeOnMainThread(() => LoadData());
                }
            }
            catch { }
        }

        private void LoadData()
        {
            var subs = MockDataService.GetSubscriptions();
            var active = subs.Where(s => s.Status == "Active").ToList();

            var totalMonthly = active.Sum(s => s.MonthlyCost);
            TotalMonthlyLabel.Text = totalMonthly.ToString("C2", _gbp);
            TotalYearlyLabel.Text = (totalMonthly * 12).ToString("C2", _gbp);
            SubCountLabel.Text = active.Count.ToString();

            SubsList.Children.Clear();
            foreach (var sub in active.OrderByDescending(s => s.MonthlyCost))
                SubsList.Children.Add(CreateSubCard(sub));

            var cancelled = subs.Where(s => s.Status == "Cancelled").ToList();
            if (cancelled.Count > 0)
            {
                SubsList.Children.Add(new Label
                {
                    Text = "Cancelled",
                    TextColor = Color.FromArgb("#64748B"),
                    FontSize = 13,
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 8, 0, 0)
                });
                foreach (var sub in cancelled)
                    SubsList.Children.Add(CreateSubCard(sub));
            }

            CategoryList.Children.Clear();
            var groups = active.GroupBy(s => s.Category).OrderByDescending(g => g.Sum(s => s.MonthlyCost));
            foreach (var g in groups)
            {
                var total = g.Sum(s => s.MonthlyCost);
                var bar = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    }
                };

                bar.Add(new Label
                {
                    Text = $"{g.Key} ({g.Count()})",
                    TextColor = Color.FromArgb("#CBD5E1"),
                    FontSize = 12,
                }, 0);

                bar.Add(new Label
                {
                    Text = total.ToString("C2", _gbp) + "/mo",
                    TextColor = Color.FromArgb("#F59E0B"),
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.End,
                }, 1);

                CategoryList.Children.Add(bar);

                var pct = totalMonthly > 0 ? (double)(total / totalMonthly) : 0;
                var barBg = new Border
                {
                    BackgroundColor = Color.FromArgb("#1E293B"),
                    HeightRequest = 6,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 3 },
                    Stroke = Colors.Transparent,
                };
                var barFill = new Border
                {
                    BackgroundColor = Color.FromArgb("#3B82F6"),
                    HeightRequest = 6,
                    WidthRequest = Math.Max(4, pct * 280),
                    HorizontalOptions = LayoutOptions.Start,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 3 },
                    Stroke = Colors.Transparent,
                };
                var barStack = new Grid();
                barStack.Add(barBg);
                barStack.Add(barFill);
                CategoryList.Children.Add(barStack);
            }
        }

        private View CreateSubCard(Subscription sub)
        {
            var isActive = sub.Status == "Active";
            var card = new Border
            {
                BackgroundColor = Color.FromArgb(isActive ? "#1A2332" : "#151B28"),
                Stroke = Color.FromArgb("#1E293B"),
                StrokeThickness = 1,
                Padding = new Thickness(12, 10),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            };

            var grid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(new GridLength(36)),
                    new ColumnDefinition(GridLength.Star),
                    new ColumnDefinition(GridLength.Auto),
                    new ColumnDefinition(GridLength.Auto),
                },
                ColumnSpacing = 10,
            };

            var (color, abbrev) = SupplierBranding.Get(sub.Name);
            var icon = new Border
            {
                WidthRequest = 32,
                HeightRequest = 32,
                BackgroundColor = Color.FromArgb(color),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 6 },
                Stroke = Colors.Transparent,
                VerticalOptions = LayoutOptions.Center,
                Content = new Label
                {
                    Text = abbrev,
                    TextColor = Colors.White,
                    FontSize = 10,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                }
            };

            var details = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };
            details.Add(new Label
            {
                Text = sub.Name,
                TextColor = Color.FromArgb(isActive ? "#E2E8F0" : "#64748B"),
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                TextDecorations = isActive ? TextDecorations.None : TextDecorations.Strikethrough,
            });

            var subLine = sub.Category;
            if (sub.NextRenewal.HasValue && isActive)
                subLine += $" · Renews {sub.NextRenewal.Value:dd MMM}";
            details.Add(new Label
            {
                Text = subLine,
                TextColor = Color.FromArgb("#64748B"),
                FontSize = 11,
            });

            var costLabel = new Label
            {
                Text = sub.MonthlyCost.ToString("C2", _gbp) + "/mo",
                TextColor = Color.FromArgb(isActive ? "#F59E0B" : "#475569"),
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center,
            };

            grid.Add(icon, 0);
            grid.Add(details, 1);
            grid.Add(costLabel, 2);

            var btnStack = new HorizontalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.Center };

            if (isActive)
            {
                var cancelBtn = new Button
                {
                    Text = "Cancel",
                    BackgroundColor = Color.FromArgb("#3B1A1A"),
                    TextColor = Color.FromArgb("#EF4444"),
                    FontSize = 10,
                    CornerRadius = 6,
                    Padding = new Thickness(8, 2),
                    HeightRequest = 26,
                };
                ToolTipProperties.SetText(cancelBtn, "Mark this subscription as cancelled and see how to cancel it with the provider.");
                cancelBtn.Clicked += (s, e) => OnCancelClicked(sub);
                btnStack.Add(cancelBtn);
            }

            var removeBtn = new Button
            {
                Text = "Remove",
                BackgroundColor = Color.FromArgb("#1E293B"),
                TextColor = Color.FromArgb("#94A3B8"),
                FontSize = 10,
                CornerRadius = 6,
                Padding = new Thickness(8, 2),
                HeightRequest = 26,
            };
            ToolTipProperties.SetText(removeBtn, "Remove this subscription from your list entirely.");
            removeBtn.Clicked += (s, e) => OnRemoveClicked(sub);
            btnStack.Add(removeBtn);

            grid.Add(btnStack, 3);

            card.Content = grid;
            return card;
        }

        private async void OnCancelClicked(Subscription sub)
        {
            var msg = $"Cancel {sub.Name} ({sub.MonthlyCost:C2}/mo)?\n\n";

            if (!string.IsNullOrEmpty(sub.CancelNotes))
                msg += sub.CancelNotes + "\n\n";

            if (!string.IsNullOrEmpty(sub.CancelUrl))
                msg += $"Online: {sub.CancelUrl}\n";
            if (!string.IsNullOrEmpty(sub.CancelPhone))
                msg += $"Phone: {sub.CancelPhone}\n";

            if (sub.PaymentMethod == "Direct Debit")
                msg += "\nYou can also cancel the Direct Debit via your bank.";

            var result = await Application.Current.Windows[0].Page.DisplayAlert(
                "Cancel Subscription", msg, "Mark as Cancelled", "Keep Active");

            if (result)
            {
                MockDataService.UpdateSubscriptionStatus(sub.Id, "Cancelled");
                LoadData();
            }
        }

        private async void OnRemoveClicked(Subscription sub)
        {
            var result = await Application.Current.Windows[0].Page.DisplayAlert(
                "Remove Subscription",
                $"Remove {sub.Name} from your list?",
                "Remove", "Cancel");
            if (result)
            {
                MockDataService.RemoveSubscription(sub.Id);
                LoadData();
            }
        }

        private async void OnAddSubscriptionClicked(object sender, EventArgs e)
        {
            string name = await Application.Current.Windows[0].Page.DisplayPromptAsync(
                "Add Subscription", "Subscription name:", placeholder: "e.g. Disney+");
            if (string.IsNullOrWhiteSpace(name)) return;

            string cost = await Application.Current.Windows[0].Page.DisplayPromptAsync(
                "Monthly Cost", "Amount per month (£):", keyboard: Keyboard.Numeric);
            if (string.IsNullOrWhiteSpace(cost) || !decimal.TryParse(cost, out var amount)) return;

            string category = await Application.Current.Windows[0].Page.DisplayActionSheet(
                "Category", "Cancel", null,
                "Streaming", "Music", "Gaming", "TV & Broadband",
                "Health & Fitness", "Bills", "Software", "Other");
            if (string.IsNullOrWhiteSpace(category) || category == "Cancel") return;

            MockDataService.KnownSubscriptions.TryGetValue(name, out var info);

            var sub = new Subscription
            {
                Id = Guid.NewGuid().ToString("N")[..8],
                Name = name.Trim(),
                MonthlyCost = amount,
                Category = info.Category ?? category,
                NextRenewal = DateTime.Now.AddMonths(1),
                CancelUrl = info.CancelUrl,
                CancelPhone = info.CancelPhone,
                CancelNotes = info.CancelNotes,
            };

            MockDataService.AddSubscription(sub);
            LoadData();
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page != null) await page.DisplayAlert("Subscriptions", "This section tracks your recurring subscriptions, whether added by hand or detected automatically from your transactions. It totals your spend per month and per year, and groups it by category on the right. Tap + Add to record a subscription, Cancel to mark one as cancelled and see how to cancel it with the provider, or Remove to delete it from the list. All of this data is stored locally on this PC.", "OK");
        }

        public async Task PlayEntryAnimation()
        {
            AnimationHelper.PrepareForEntry(SummaryCard, SubsListCard, CategoryCard, TipsCard);
            _ = AnimationHelper.AnimateEntry(SummaryCard, 0, 400);
            _ = AnimationHelper.AnimateEntry(SubsListCard, 150, 400);
            _ = AnimationHelper.AnimateEntry(CategoryCard, 200, 400);
            await AnimationHelper.AnimateEntry(TipsCard, 300, 400);
        }
    }
}
