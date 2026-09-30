using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Dashboard
{
    public partial class AddSupplierFlow : ContentPage
    {
        public event Action SupplierAdded;

        private string _selectedType;
        private string _selectedIcon;
        private List<DateTime> _monthOptions = new();

        private static readonly Dictionary<string, (string Icon, string Placeholder)> TypeInfo = new()
        {
            ["Electricity"] = ("⚡", "e.g. 24.50p/kWh · 53.35p/day standing"),
            ["Gas"] = ("🔥", "e.g. 6.24p/kWh · 31.43p/day standing"),
            ["Water"] = ("💧", "e.g. £1.7463/m³ · 21.25p/day standing"),
            ["Broadband"] = ("📡", "e.g. 18 month contract · ends Mar 2027"),
            ["Mobile"] = ("📱", "e.g. Unlimited mins & texts · 40GB data"),
            ["Council Tax"] = ("🏛", "e.g. Band C · 10 monthly payments"),
            ["Insurance"] = ("🛡", "e.g. Home contents · annual renewal"),
            ["Other"] = ("📋", "e.g. Monthly subscription"),
        };

        public AddSupplierFlow()
        {
            InitializeComponent();
            SetupTypeCards();
            SetupMonthPicker();
        }

        private void SetupMonthPicker()
        {
            var now = DateTime.Now;
            _monthOptions.Clear();
            for (int i = 0; i < 12; i++)
            {
                var month = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
                _monthOptions.Add(month);
                BillMonthPicker.Items.Add(month.ToString("MMM yyyy"));
            }
            BillMonthPicker.SelectedIndex = 0;
        }

        private void SetupTypeCards()
        {
            var cards = new (Border Card, string Type)[]
            {
                (ElecOption, "Electricity"),
                (GasOption, "Gas"),
                (WaterOption, "Water"),
                (BroadbandOption, "Broadband"),
                (MobileOption, "Mobile"),
                (CouncilTaxOption, "Council Tax"),
                (InsuranceOption, "Insurance"),
                (OtherOption, "Other"),
            };

            foreach (var (card, type) in cards)
            {
                var t = type;
                ToolTipProperties.SetText(card, $"Add a {type} supplier or bill.");
                card.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await card.ScaleTo(0.97, 80, Easing.CubicOut);
                        await card.ScaleTo(1.0, 80, Easing.CubicOut);
                        SelectType(t);
                    })
                });
            }
        }

        private async void SelectType(string type)
        {
            _selectedType = type;
            var info = TypeInfo[type];
            _selectedIcon = info.Icon;

            FormIcon.Text = info.Icon;
            FormTitle.Text = type;
            PageTitle.Text = $"Add {type}";
            PageSubtitle.Text = "Enter supplier details";
            StepIndicator.Text = "Step 2 of 2";
            TariffDetailEntry.Placeholder = info.Placeholder;

            Step1.IsVisible = false;
            Step2.IsVisible = true;
            Step2.Opacity = 0;
            await Step2.FadeTo(1, 250, Easing.CubicOut);
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            var name = SupplierNameEntry.Text?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                await DisplayAlert("Missing", "Please enter a supplier name.", "OK");
                return;
            }

            if (!decimal.TryParse(MonthlyCostEntry.Text?.Trim(), out var cost))
            {
                await DisplayAlert("Missing", "Please enter a valid bill amount.", "OK");
                return;
            }

            var supplier = new MockSupplier
            {
                Name = name,
                Type = _selectedType,
                Icon = _selectedIcon,
                Tariff = TariffNameEntry.Text?.Trim() ?? "",
                TariffDetail = TariffDetailEntry.Text?.Trim() ?? "",
                MonthlyCost = cost,
                AccountRef = AccountRefEntry.Text?.Trim() ?? "",
            };

            MockDataService.AddSupplier(supplier);

            var billMonth = BillMonthPicker.SelectedIndex >= 0
                ? _monthOptions[BillMonthPicker.SelectedIndex]
                : DateTime.Now;

            MockDataService.AddBill(new MockUtilityBill
            {
                BillDate = billMonth,
                Supplier = name,
                FuelType = _selectedType,
                Amount = cost,
                Period = billMonth.ToString("MMM yyyy"),
                Address = MockDataService.GetProperty()?.Address ?? "",
            });

            SupplierAdded?.Invoke();
            await Navigation.PopAsync();
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            if (Step2.IsVisible)
            {
                Step2.IsVisible = false;
                Step1.IsVisible = true;
                PageTitle.Text = "Add Supplier";
                PageSubtitle.Text = "Choose supplier type";
                StepIndicator.Text = "Step 1 of 2";
                return;
            }
            await Navigation.PopAsync();
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Add Supplier",
                "Use this when a supplier can't be added by logging in. Pick what kind of bill it is, then enter the supplier name, tariff and the amount and month of a bill. Add Supplier saves it as a new supplier and adds that bill to your history, all stored locally on this PC.",
                "OK");
        }
    }
}
