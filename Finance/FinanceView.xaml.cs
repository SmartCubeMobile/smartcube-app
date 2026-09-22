#if WINFORMS
using SmartDashboard;
using System.Windows.Forms;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
#endif

#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Windows.Foundation;
#endif

#if WINUI
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
#endif

#if ANDROIDX
using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AndroidX.AppCompat.App;
#endif

#if SMARTMAUI
using Microsoft.Maui;
using System.Runtime.CompilerServices;
#endif

// The second I hear that fucking bitch thumping about upstairs ... I cannot think
// Then she comes bashing and crashing and huffing and puffing and bustling down into the kitchen



namespace SmartCubeMobile
{
#if WINFORMS
    internal partial class FinanceView : UserControl
    {
#endif
#if WPF  || WINUI
    public partial class FinanceView : UserControl
    {
#endif
#if ANDROIDX
    public partial class FinanceView : Android.Widget.RelativeLayout
    {
#endif
#if SMARTMAUI
    public partial class FinanceView : ContentView
    {
#endif

#if WINFORMS

        private static WebView2 FinanceWebView = new WebView2();
#endif
#if ANDROIDX

        private static WebView FinanceWebView;
#endif
        private static SignInViewModel signinviewmodel;
        private MainViewModel ourviewmodel;
        private FinanceViewModel financeviewmodel;

#if SMARTMAUI        
        private static LongPressHandler rbBanksLP;
        private static LongPressHandler rbSavingsLP;
        private static LongPressHandler rbInvestmentsLP;
        private static LongPressHandler rbCryptosLP;
#endif
#if ANDROIDX
        public FinanceView(SignInViewModel signinvm, MainViewModel mainvm, FinanceViewModel financevm, Context context) : base(context)
        {
            // But Android never creates a 'FinanceView'(!) because it has FinanceView.axml
            // ... but it DOES need some of the routines in here ...
#else
        public FinanceView(SignInViewModel signinvm, MainViewModel mainvm, FinanceViewModel financevm)
        {
#endif            
            signinviewmodel = signinvm;
            ourviewmodel = mainvm;
            financeviewmodel = financevm;

#if WPF  || WINUI || SMARTMAUI
            try
            {
                InitializeComponent();
                // Because you can't have the DataContext set on the UserControl iteself <= Chimps!
#if WPF  || WINUI
                DataContext = financeviewmodel;
#endif

#if SMARTMAUI
                BindingContext = financeviewmodel;

                this.Loaded += OnLoaded;
                this.Unloaded += OnUnloaded;
#endif
#if WPF || WINUI
                //ButtonAutoSwitchFinance.Click += new RoutedEventHandler((s, e) => SmartFinanceV2025.ButtonAutoSwitchFinanceClick(s, e, ourviewmodel, financeviewmodel));
#endif
#if WINUI
                financeviewmodel.xamlRoot = this.XamlRoot;
#endif
                //FinanceViewModel.WalletItem wallet1 = new FinanceViewModel.WalletItem();
                //wallet1.Content = "Arculus";
                //wallet1.Value = "r123";
                //financeviewmodel.FinanceWalletsList.Add(wallet1);
                //FinanceViewModel.WalletItem wallet2 = new FinanceViewModel.WalletItem();
                //wallet2.Content = "Tangem";
                //wallet2.Value = "r987";
                //financeviewmodel.FinanceWalletsList.Add(wallet2);                
#if WINUI
                // Because you can't have the DataContext set on the UserControl iteself <= Chimps!
                DataContext = financeviewmodel;
                financeviewmodel.Datagrid = this.FinanceTransactionsListView;


#endif
                // NEED TO PUT IN LOGIC THAT WHEN THE PROFILES ARE
                // UPDATED, WE REFRESH THE TRANSACTIONS PAGE TO REFLECT THIS
#if WPF
                financeviewmodel.FuckingGrid = FinanceGrid;
                // Get the first column (you can choose another one by index or other logic)
                GridViewColumn originalColumn = FinanceGrid.Columns[0];

                // Create a new GridViewColumn and copy the properties from the original column
                GridViewColumn copiedColumn = new GridViewColumn
                {
                    Header = originalColumn.Header,
                    DisplayMemberBinding = originalColumn.DisplayMemberBinding,
                    // Optionally, you can copy other properties like CellTemplate or CellEditingTemplate if needed
                    CellTemplate = originalColumn.CellTemplate
                    //CellEditingTemplate = originalColumn.CellEditingTemplate
                };
                financeviewmodel.FuckingGridColumn = copiedColumn;
#endif
                // Previously done in XAML but I couldn't
                // for the LIFE of me find out how I passed
                // extra model parameters to this bollocks ...
#if WPF
                FinanceBorder.KeyUp += (s, e) => OnKeyUp(s, e, signinviewmodel, ourviewmodel);
                FinanceTransactionsListView.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(GridViewColumnHeader_Click));

                FinanceProviders.ItemsSource = financeviewmodel.FinanceProvidersList;



                // See the Notes at the start of these
                // Event Handlers which describes
                // what complete and utter garbage this
                // Chimp-derived shite is ...
                StartingDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>((s, e) => StartingDateChanged(s, e, ourviewmodel, financeviewmodel));
                ResetDates.Click += new RoutedEventHandler((s, e) => Finance_ResetDates(s, e, ourviewmodel, financeviewmodel));
                EndingDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>((s, e) => EndingDateChanged(s, e, ourviewmodel, financeviewmodel));
            
#endif

#if WINUI
                this.Loaded += new RoutedEventHandler((s, e) => FinanceViewLoaded(s, e, financeviewmodel));

                StartingDate.SelectedDateChanged += new TypedEventHandler<DatePicker, DatePickerSelectedValueChangedEventArgs>((s, e) => StartingDateChanged(s, e, ourviewmodel, financeviewmodel));
                ResetDates.Click += new RoutedEventHandler((s, e) => Finance_ResetDates(s, e, ourviewmodel, financeviewmodel));
                EndingDate.SelectedDateChanged += new TypedEventHandler<DatePicker, DatePickerSelectedValueChangedEventArgs>((s, e) => EndingDateChanged(s, e, ourviewmodel, financeviewmodel));
#endif
#if SMARTMAUI
                SubmitButton.Clicked += new EventHandler((s, e) => Finance_ButtonSubmitClick(s, e, ourviewmodel, financeviewmodel));
                CancelButton.Clicked += new EventHandler((s, e) => Finance_ButtonCancelClick(s, e, ourviewmodel, financeviewmodel));

                StartingDate.DateSelected += (s, e) => StartingDateChanged(s, e, ourviewmodel, financeviewmodel);
                ResetDatesButton.Clicked += (s, e) => Finance_ResetDates(s, e, ourviewmodel, financeviewmodel);
                EndingDate.DateSelected += (s, e) => EndingDateChanged(s, e, ourviewmodel, financeviewmodel);



                //FinanceDebitsCreditsChart.Clicked += (s, e) => ChartsMouseDoubleClick(s, e, ourviewmodel, financeviewmodel);
                DebitsCreditsButton.Clicked += (s, e) => TopChartsClicked(s, e, ourviewmodel, financeviewmodel);
                GraphsButton.Clicked += (s, e) => TopChartsClicked(s, e, ourviewmodel, financeviewmodel);

#if SMARTMAUI
                TapGestureRecognizer tapWebView = new TapGestureRecognizer();
                tapWebView.Tapped += (s, e) =>
                {
                    // Do something
                    SideChartsClicked(s, e, ourviewmodel, financeviewmodel);
                };
                this.WebViewButton.GestureRecognizers.Add(tapWebView);

                TapGestureRecognizer tapTransactions = new TapGestureRecognizer();
                tapTransactions.Tapped += (s, e) =>
                {
                    // Do something
                    SideChartsClicked(s, e, ourviewmodel, financeviewmodel);
                };
                this.TransactionsButton.GestureRecognizers.Add(tapTransactions);

                TapGestureRecognizer tapCryptos = new TapGestureRecognizer();
                tapCryptos.Tapped += (s, e) =>
                {
                    // Do something
                    SideChartsClicked(s, e, ourviewmodel, financeviewmodel);
                };
                this.CryptosButton.GestureRecognizers.Add(tapCryptos);
#endif

#if WINDOWS
                SubmitButton.SetBinding(ToolTipProperties.TextProperty, "SubmitButton");
                CancelButton.SetBinding(ToolTipProperties.TextProperty, "CancelButton");
                FinanceInstitutions.SetBinding(ToolTipProperties.TextProperty, "FinanceInstitutions");
                FinanceProviders.SetBinding(ToolTipProperties.TextProperty, "FinanceProviders");
                FinanceAccounts.SetBinding(ToolTipProperties.TextProperty, "FinanceAccount");
                RBBanks.SetBinding(ToolTipProperties.TextProperty, "FinanceBanks");
                RBSavings.SetBinding(ToolTipProperties.TextProperty, "FinanceSavings");
                RBInvestments.SetBinding(ToolTipProperties.TextProperty, "FinanceInvestments");
                RBCryptos.SetBinding(ToolTipProperties.TextProperty, "FinanceCryptos");
                ButtonAutoSwitchFinance.SetBinding(ToolTipProperties.TextProperty, "ButtonAutoSwitch");
                StartingDate.SetBinding(ToolTipProperties.TextProperty, "StartingDate");
                ResetDatesButton.SetBinding(ToolTipProperties.TextProperty, "ResetDatesTooltip");
                EndingDate.SetBinding(ToolTipProperties.TextProperty, "EndingDate");
                ResultsStartDate.SetBinding(ToolTipProperties.TextProperty, "ResultsStartDate");
                ResultsEndDate.SetBinding(ToolTipProperties.TextProperty, "ResultsEndDate");
                PostCodeLabel.SetBinding(ToolTipProperties.TextProperty, "PostCodeTooltip");
                Culture.SetBinding(ToolTipProperties.TextProperty, "Culture");
                ExchangeRates.SetBinding(ToolTipProperties.TextProperty, "ExchangeRates");
#endif

#endif
#if WPF  || WINUI
                Addresses.SelectionChanged += new SelectionChangedEventHandler((s, e) => Finance_AddressSelectionChanged(s, e, ourviewmodel, financeviewmodel));
                CulturePicker.SelectionChanged += new SelectionChangedEventHandler((s, e) => FinanceCultures_SelectionChanged(s, e, ourviewmodel, financeviewmodel));

                FinanceTransactionsListView.SelectionChanged += new SelectionChangedEventHandler((s, e) => TransactionsSelectionChanged(s, e, ourviewmodel, financeviewmodel));
#endif
#if WPF
                
                FinanceTransactionsListView.MouseDoubleClick += new MouseButtonEventHandler((s, e) => FinanceTransactionsMouseDoubleClick(s, e, ourviewmodel, financeviewmodel));

                FinanceInstitutions.MouseEnter += new MouseEventHandler((s, e) => MouseEnterInstitutions(s, e, ourviewmodel, financeviewmodel));
                FinanceInstitutions.MouseLeave += new MouseEventHandler((s, e) => MouseLeaveInstitutions(s, e, ourviewmodel, financeviewmodel));
                FinanceInstitutions.MouseRightButtonUp += new MouseButtonEventHandler((s, e) => FinanceMouseRightButtonDown(s, e, ourviewmodel, financeviewmodel));
                FinanceInstitutions.SelectionChanged += new SelectionChangedEventHandler((s, e) => FinanceInstitutions_SelectionChanged(s, e, ourviewmodel, financeviewmodel));
                //FinanceInstitutions.DropDownClosed += new EventHandler((s, e) => FinanceInstitutions_DropDownClosed(s, e, financeviewmodel));


                FinanceInstitutions.MouseDoubleClick += new MouseButtonEventHandler((s, e) => Finance_InstitutionMouseDoubleClick(s, e, signinviewmodel, ourviewmodel, financeviewmodel));

                FinanceInstitutions.PreviewMouseDoubleClick += (s, e) =>
                {
                    // Avoid if click is on the dropdown part
                    if (e.OriginalSource is FrameworkElement fe &&
                        fe.TemplatedParent is ToggleButton)
                        return;

                    Finance_InstitutionMouseDoubleClick(s, e, signinviewmodel, ourviewmodel, financeviewmodel);
                };
                FinanceProviders.DropDownClosed += new EventHandler((s, e) => FinanceProviders_DropDownClosed(s, e, financeviewmodel));

                FinanceTransactionsListView.SelectionChanged += new SelectionChangedEventHandler((s, e) => FinanceTransactionSelected(s, e, financeviewmodel));


                FinanceAccounts.DropDownClosed += new EventHandler((s, e) => FinanceAccounts_DropDownClosed(s, e, ourviewmodel, financeviewmodel));
                FinanceTransactionGroups.DropDownClosed += new EventHandler((s, e) => FinanceTransactionGroups_DropDownClosed(s, e, ourviewmodel, financeviewmodel));

                FinanceDebitsCreditsChart.MouseDoubleClick += new MouseButtonEventHandler((s, e) => ChartsMouseDoubleClick(s, e, signinviewmodel, ourviewmodel, financeviewmodel));

                SubmitButton.Click += new RoutedEventHandler((s, e) => Finance_ButtonSubmitClick(s, e, ourviewmodel, financeviewmodel));
                CancelButton.Click += new RoutedEventHandler((s, e) => Finance_ButtonCancelClick(s, e, ourviewmodel, financeviewmodel));

                // Not now, later for ListView
                //FinanceTransactionsGrid.Sorting += new DataGridSortingEventHandler((s, e) => GridSorted(e, financeviewmodel));

                FinanceTransactionsTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => FinanceTransactionsTabMouseDoubleClick(s, e, ourviewmodel, financeviewmodel));
                FinanceDebitsCreditsChart.MouseDoubleClick += new MouseButtonEventHandler((s, e) => ChartsMouseDoubleClick(s, e, signinviewmodel, ourviewmodel, financeviewmodel));

                FinanceCryptoTransactionsTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => FinanceCryptoTabItemsTabMouseDoubleClick(s, e, ourviewmodel, financeviewmodel));
                FinanceCryptoAccountsTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => FinanceCryptoTabItemsTabMouseDoubleClick(s, e, ourviewmodel, financeviewmodel));
                FinanceCryptoAddressesTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => FinanceCryptoTabItemsTabMouseDoubleClick(s, e, ourviewmodel, financeviewmodel));
                FinanceCryptoExchangeRatesTab.MouseDoubleClick += new MouseButtonEventHandler((s, e) => FinanceCryptoTabItemsTabMouseDoubleClick(s, e, ourviewmodel, financeviewmodel));

#endif
#if WINUI
                SubmitButton.Click += new RoutedEventHandler((s, e) => Finance_ButtonSubmitClick(s, e, ourviewmodel, financeviewmodel));
                CancelButton.Click += new RoutedEventHandler((s, e) => Finance_ButtonCancelClick(s, e, ourviewmodel, financeviewmodel));

                FinanceInstitutions.PointerEntered += new PointerEventHandler((s, e) => MouseEnterInstitutions(s, e, ourviewmodel, financeviewmodel));
                FinanceInstitutions.PointerExited += new PointerEventHandler((s, e) => MouseLeaveInstitutions(s, e, ourviewmodel, financeviewmodel));
                
                FinanceInstitutions.RightTapped += new RightTappedEventHandler((s, e) => FinanceMouseRightButtonDown(s, e, ourviewmodel, financeviewmodel));
                FinanceInstitutions.SelectionChanged += new SelectionChangedEventHandler((s, e) => FinanceInstitutions_SelectionChanged(s, e, ourviewmodel, financeviewmodel));

                //FinanceProviders.PointerPressed += new PointerEventHandler((s, e) => MyComboBox_PointerPressed(s, e, ourviewmodel, financeviewmodel));
                FinanceProviders.AddHandler(UIElement.PointerPressedEvent,
    new PointerEventHandler((s, e) => MyComboBox_PointerPressed(s, e, ourviewmodel, financeviewmodel)),
    true);


                FinanceProviders.DropDownClosed += new EventHandler<object>((s, e) => FinanceProviders_DropDownClosed(s, e, financeviewmodel));                
                FinanceAccounts.DropDownClosed += new EventHandler<object>((s, e) => FinanceAccounts_DropDownClosed(s, e, ourviewmodel, financeviewmodel));
                FinanceTransactionsGroups.DropDownClosed += new EventHandler<object>((s, e) => FinanceTransactionGroups_DropDownClosed(s, e, ourviewmodel, financeviewmodel));

                FinanceTransactionsListView.DoubleTapped += new DoubleTappedEventHandler((s, e) => FinanceTransactionsMouseDoubleClick(s, e, ourviewmodel, financeviewmodel));
                FinanceTransactionsListView.SelectionChanged += new SelectionChangedEventHandler((s, e) => TransactionsSelectionChanged(s, e, ourviewmodel, financeviewmodel));
                
                FinanceTransactionsListView.Sorting += new EventHandler<DataGridColumnEventArgs>((s, e) => GridSorted(s, e, financeviewmodel));

                FinanceTransactionsTab.DoubleTapped += new DoubleTappedEventHandler((s, e) => FinanceTransactionsTabMouseDoubleClick(s, e, ourviewmodel, financeviewmodel));
                // Done differently for WINUI it seems
                //FinanceDebitsCreditsChart.MouseDoubleClick += new MouseButtonEventHandler((s, e) =>ChartsMouseDoubleClick(s, e, ourviewmodel, financeviewmodel));
#endif
#if WPF  || WINUI
                //So we share the rates across ALL Views
                FrontEndGUI.SetLabelDataContext(ourviewmodel, this.GBP);
#endif
#if WPF
                GBP.SetBinding(Label.ContentProperty, new Binding()
#endif
#if WINUI
                GBP.SetBinding(TextBlock.TextProperty, new Binding()
#endif
#if WPF  || WINUI
                {
                    Path = new PropertyPath("GBPRate"),
                    Source = ourviewmodel
                });
#endif

#if SMARTMAUI
                GBP.SetBinding(Label.TextProperty, new Binding
                {
                    Path = "GBPRate",
                    Source = ourviewmodel
                });
#endif
                
                // So we share the rates across ALL Views
                FrontEndGUI.SetLabelDataContext(ourviewmodel, this.EUR);
#if WPF
                EUR.SetBinding(Label.ContentProperty, new Binding()
#endif
#if WINUI
                EUR.SetBinding(TextBlock.TextProperty, new Binding()
#endif
#if WPF  || WINUI
                {
                    Path = new PropertyPath("EURRate"),
                    Source = ourviewmodel
                });
#endif
#if SMARTMAUI
                EUR.SetBinding(Label.TextProperty, new Binding
                {
                    Path = "EURRate",
                    Source = ourviewmodel
                });
#endif

                // So we share the rates across ALL Views
                FrontEndGUI.SetLabelDataContext(ourviewmodel, this.USD);
#if WPF
                USD.SetBinding(Label.ContentProperty, new Binding()
#endif
#if WINUI
                USD.SetBinding(TextBlock.TextProperty, new Binding()
#endif
#if WPF  || WINUI
                {
                    Path = new PropertyPath("USDRate"),
                    Source = ourviewmodel
                });
#endif

#if SMARTMAUI
                USD.SetBinding(Label.TextProperty, new Binding
                {
                    Path = "USDRate",
                    Source = ourviewmodel
                });
#endif
                // So we share the rates across ALL Views
                FrontEndGUI.SetLabelDataContext(ourviewmodel, this.JPY);
#if WPF
                JPY.SetBinding(Label.ContentProperty, new Binding()
#endif
#if WINUI
                JPY.SetBinding(TextBlock.TextProperty, new Binding()
#endif
#if WPF  || WINUI
                {
                    Path = new PropertyPath("JPYRate"),
                    Source = ourviewmodel
                });
#endif
#if SMARTMAUI
                JPY.SetBinding(Label.TextProperty, new Binding
                {
                    Path = "JPYRate",
                    Source = ourviewmodel
                });
#endif

#if WPF
                ///  ===============================
                ///  If this works it'll be a fucking miracle ...
                SolidColorBrush backgroundtransparentBrush = (SolidColorBrush)ourviewmodel.transparentColour;

                //financeviewmodel.CellStyle = new Style(typeof(DataGridCell));
                //financeviewmodel.CellStyle.Setters.Add(new Setter
                //{
                //    Property = DataGridCell.BackgroundProperty,
                //    Value = ourviewmodel.orangeColour
                //});
                //financeviewmodel.CellStyle.Setters.Add(new Setter
                //{
                //    Property = DataGridCell.HorizontalAlignmentProperty,
                //    Value = System.Windows.HorizontalAlignment.Stretch
                //});
                //financeviewmodel.CellStyle.Setters.Add(new Setter
                //{
                //    Property = DataGridCell.VerticalAlignmentProperty,
                //    Value = System.Windows.VerticalAlignment.Stretch
                //});
                //financeviewmodel.CellStyle.Setters.Add(new Setter
                //{
                //    Property = DataGridCell.BorderThicknessProperty,
                //    Value = new Thickness(0)
                //});
                //financeviewmodel.CellStyle.Setters.Add(new Setter
                //{
                //    Property = DataGridCell.MarginProperty,
                //    Value = new Thickness(0, 0, 0, 0)
                //});
#endif
            }
            catch (Exception ex)
            {
                GiveUp(ourviewmodel, financeviewmodel, ex.Message);
                return;
            }

            // All this fucking bollocks just because they couldn't put a fucking proper DataGrid in XamarShit Forms ....

#if WPF  || WINUI
            // https://stackoverflow.com/questions/1483892/how-to-bind-to-a-passwordbox-in-mvvm
            // Mike McKechnie
            //if (MyInstitutionPicker != null)    // <= Was SelectedItem??? 
            //{
            //    MyInstitutionPicker.Focus();
            //}
#endif
#endif

                return;
        }

#if SMARTMAUI
        private void MultiSelectPicker_Closed(object sender, EventArgs e)
        {
            if (sender == FinanceProviders)
            {
                // Providers dropdown closed
                FinanceProviders_DropDownClosed(sender, e, financeviewmodel);

            }
            else if (sender == FinanceAccounts)
            {
                // Accounts dropdown closed
                FinanceAccounts_DropDownClosed(sender, e, financeviewmodel);
            }
            return;
        }

        private void CurrencyTapped(object sender, TappedEventArgs e)
        {
            if (sender is not Border border)
                return;

            var code = border.AutomationId;

            OnCurrencySelected(code);
        }

        private void OnCurrencySelected(string code)
        {
            List<CurrencyValues> totals_found =
                new List<CurrencyValues>
                (from totals in financeviewmodel.currencytotals
                 where totals.Code == code
                 select totals);

            if (totals_found.Count() == 1)
            {
                financeviewmodel.PaidIn = totals_found.First().PaidIn.ToString();
                financeviewmodel.PaidOut = totals_found.First().PaidOut.ToString();
                financeviewmodel.Difference = (totals_found.First().PaidIn - totals_found.First().PaidOut).ToString();

            }
            return;
        }

#endif

        public void FinanceCleanup()
        {
#if WPF
            StartingDate.SelectedDateChanged -= new EventHandler<SelectionChangedEventArgs>((s, e) => StartingDateChanged(s, e, ourviewmodel, financeviewmodel));
            ResetDates.Click -= new RoutedEventHandler((s, e) => Finance_ResetDates(s, e, ourviewmodel, financeviewmodel));
            EndingDate.SelectedDateChanged -= new EventHandler<SelectionChangedEventArgs>((s, e) => EndingDateChanged(s, e, ourviewmodel, financeviewmodel));
#endif
#if WINUI
            StartingDate.SelectedDateChanged -= new TypedEventHandler<DatePicker, DatePickerSelectedValueChangedEventArgs>((s, e) => StartingDateChanged(s, e, ourviewmodel, financeviewmodel));
            ResetDates.Click -= new RoutedEventHandler((s, e) => Finance_ResetDates(s, e, ourviewmodel, financeviewmodel));
            EndingDate.SelectedDateChanged -= new TypedEventHandler<DatePicker, DatePickerSelectedValueChangedEventArgs>((s, e) => EndingDateChanged(s, e, ourviewmodel, financeviewmodel));
#endif
#if SMARTMAUI
            StartingDate.DateSelected -= (s, e) => StartingDateChanged(s, e, ourviewmodel, financeviewmodel);
            EndingDate.DateSelected -= (s, e) => EndingDateChanged(s, e, ourviewmodel, financeviewmodel);
#endif
        }

#if SMARTMAUI
        private void OnLoaded(object sender, EventArgs e)
        {
            RadioButtonTappedEvents(this, ourviewmodel, financeviewmodel, true);
            RadioButtonCheckedEvents(this, ourviewmodel, financeviewmodel, true);
        }

        private void OnUnloaded(object sender, EventArgs e)
        {
            RadioButtonTappedEvents(this, ourviewmodel, financeviewmodel, false);
            RadioButtonCheckedEvents(this, ourviewmodel, financeviewmodel, false);
        }

        internal void SideChartsClicked(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
            Label abc = sender as Label;
            string compar = abc.ClassId.ToString();
            if (!string.IsNullOrEmpty(compar))
            {
                switch (compar)
                {
                    case "W":
                        financeviewmodel.WebViewVisible = true;
                        financeviewmodel.DebitsCreditsVisible = false;
                        financeviewmodel.GraphsVisible = false;
                        financeviewmodel.CryptosVisible = false;
                        break;
                    case "T":
                        financeviewmodel.WebViewVisible = false;
                        financeviewmodel.DebitsCreditsVisible = true;
                        financeviewmodel.GraphsVisible = false;
                        financeviewmodel.CryptosVisible = false;
                        break;
                    case "C":
                        financeviewmodel.WebViewVisible = false;
                        financeviewmodel.DebitsCreditsVisible = false;
                        financeviewmodel.GraphsVisible = false;
                        financeviewmodel.CryptosVisible = true;
                        break;
                    default:
                        break;

                }
            }
            return;
        }

        internal void TopChartsClicked(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
            Button abc = sender as Button;
            string compar = abc.CommandParameter.ToString();
            if (!string.IsNullOrEmpty(compar))
            {
                switch (compar)
                {
                    case "D":
                        financeviewmodel.DebitsCreditsVisible = true;
                        financeviewmodel.GraphsVisible = false;
                        break;
                    case "G":
                        financeviewmodel.DebitsCreditsVisible = false;
                        financeviewmodel.GraphsVisible = true;
                        break;
                    default:
                        break;
                }
            }
            return;
        }
#endif

#if WINUI 
        private void FinanceViewLoaded(object sender, RoutedEventArgs e, FinanceViewModel financeviewmodel)
        {
            if (this.XamlRoot != null)
            {
                financeviewmodel.xamlRoot = this.XamlRoot;
            }
        }
#endif
        //private void FinanceTransactionsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        internal static async void GiveUp(MainViewModel ourviewmodel, FinanceViewModel financeviewmodel, string message)
        {
            ourviewmodel.errorMessage = message;
            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, SmartParametersV2016.Finance.ToString() + ourviewmodel.errorMessage))
            {
                return;
            }
            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
            return;
        }

#if WPF
        internal void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader columnHeader = e.OriginalSource as GridViewColumnHeader;

            string fieldName = SmartFinanceV2025.FindFieldName(sender, columnHeader);
            if (fieldName != "")
            {
                // Identify which ListView triggered the event
                if (sender == FinanceTransactionsListView)
                {
                    GridView gridView = FinanceTransactionsListView.View as GridView;
                    int columnIndex = gridView.Columns.IndexOf(columnHeader.Column);

                    SmartFinanceV2025.ReorderTransactions(financeviewmodel, fieldName, columnIndex);

                }
                if ((string)sender == "Don't delete this")
                {
                    //SmartFinanceV2025.RebuildCryptoTransactions(financeviewmodel, fieldName);
                }
            }
            else
            {
                MessageBox.Show("No binding found for this column.");
            }
            return;
        }


#endif
#if WINUI

        private DateTime lastClickTime;
        private const int DoubleClickTimeMilliseconds = 300;
            
        private void MyComboBox_PointerPressed(object sender, PointerRoutedEventArgs e, 
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {
            DateTime now = DateTime.Now;
            if ((now - lastClickTime).TotalMilliseconds <= DoubleClickTimeMilliseconds)
            {
                // Double-click detected
                ComboBox abc = sender as ComboBox;
                ComboBoxItem xyz = abc.SelectedItem as ComboBoxItem;
                if (xyz != null)
                {
                    Finance_ProvidersMouseDoubleClick(sender, e, ourviewmodel, financeviewmodel);
                }
                if (abc.Items.Count > 0)
                {
                    FinanceProviders.SelectedIndex = 0;
                    Finance_ProvidersMouseDoubleClick(sender, e, ourviewmodel, financeviewmodel);
                }
                var selectedItem = FinanceProviders.SelectedItem as ComboBoxItem;
                if (selectedItem != null)
                {
                    Finance_ProvidersMouseDoubleClick(sender, e, ourviewmodel, financeviewmodel);
                }
            }
            lastClickTime = now;
            return;
        }
#endif

#if WINUI
        internal void TabClicked(object sender, ItemClickEventArgs e)
        {
            if (sender != null &&
                e != null)
            {
                //if (financeviewmodel.TabTitles.Count == 0)
                //{
                //    return;
                //}
                TabTitle cust = e.ClickedItem as TabTitle;
                int totalsIndex = -1;
                foreach (SmartData.Currencies abcd in ourviewmodel.currenciesList)
                {
                    if (cust.Name == abcd.ISOCURRENCYSYMBOL)
                    {
                        totalsIndex = abcd.ORDINAL;
                        break;
                    }
                }
                if (totalsIndex >= 0)
                {
#if WPF || SMARTMAUI
                    financeviewmodel.TabContent.Children.Clear();
                    financeviewmodel.TabContent.Children.Add(financeviewmodel.grids[totalsIndex]);
#endif
#if WINUI
                    financeviewmodel.TabControlPanel.TabItems.Clear();
                    financeviewmodel.TabControlPanel.TabItems.Add(financeviewmodel.grids[totalsIndex]);
#endif
                }
            }
            return;
        }

#endif
#if WPF
        internal static void GridSorted(DataGridSortingEventArgs e,
                                 FinanceViewModel financeviewmodel)
        {
            if (e.Column.SortMemberPath == "TRANSACTION_DATE")
            {
                financeviewmodel.bookingsort = !financeviewmodel.bookingsort;
                SmartFinanceV2025.FinanceUpdateTransactions(financeviewmodel);
                e.Handled = true;
            }
            return;
        }
#endif

#if WINUI
        internal void GridSorted(object sender, DataGridColumnEventArgs e,
                                                    FinanceViewModel financeviewmodel)
        {
            if (sender != null && e != null)
            {
                if (e.Column.ToString() == "TRANSACTION_DATE")
                {
                    financeviewmodel.bookingsort = !financeviewmodel.bookingsort;
                    SmartFinanceV2025.FinanceUpdateTransactions(financeviewmodel);
                }
            }
            return;
        }
#endif

#if WINFORMS
        internal static void RadioButtonCheckedEvents(MainProcess comp,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    bool onoroff)
#endif
#if WPF  || WINUI
        internal static void RadioButtonCheckedEvents(FinanceView comp,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    bool onoroff)
#endif
#if ANDROIDX
        internal static void RadioButtonCheckedEvents(View comp,
                                                    AppCompatActivity meterActivity,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    bool onoroff)
#endif
#if SMARTMAUI
        internal static void RadioButtonCheckedEvents(FinanceView comp,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    bool onoroff)
#endif
        {
            // https://stackoverflow.com/questions/22813608/wpf-button-mouseleftbuttondown-doesnt-work-at-all
            // To describe why we use PreviewMouseLeftButtonDown and not MouseLeftButtonDown
#if WINFORMS
            if (onoroff)
            {
                // Traps both Left and Right buttons
                comp.RBBanks.MouseDown += new MouseEventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, comp, ourviewmodel, financeviewmodel));
                //comp.RBBanks.Click += new EventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, comp, ourviewmodel, financeviewmodel));
                comp.RBSavings.MouseDown += new MouseEventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, comp, ourviewmodel, financeviewmodel));
                comp.RBInvestments.MouseDown += new MouseEventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, comp, ourviewmodel, financeviewmodel));
                comp.RBCryptos.MouseDown += new MouseEventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, comp, ourviewmodel, financeviewmodel));

                comp.FinanceRBNON.MouseClick += new MouseEventHandler((s, e) => MainProcess.FinanceRBCheckedChanged(s, e, comp));
                comp.FinanceRBGBP.MouseClick += new MouseEventHandler((s, e) => MainProcess.FinanceRBCheckedChanged(s, e, comp));
                comp.FinanceRBEUR.MouseClick += new MouseEventHandler((s, e) => MainProcess.FinanceRBCheckedChanged(s, e, comp));
                comp.FinanceRBUSD.MouseClick += new MouseEventHandler((s, e) => MainProcess.FinanceRBCheckedChanged(s, e, comp));
                comp.FinanceRBJPY.MouseClick += new MouseEventHandler((s, e) => MainProcess.FinanceRBCheckedChanged(s, e, comp));
            }
            else
            {
                comp.RBBanks.MouseDown -= new MouseEventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, comp, ourviewmodel, financeviewmodel));
                comp.RBSavings.MouseDown -= new MouseEventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, comp, ourviewmodel, financeviewmodel));
                comp.RBInvestments.MouseDown -= new MouseEventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, comp, ourviewmodel, financeviewmodel));
                comp.RBCryptos.MouseDown -= new MouseEventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, comp, ourviewmodel, financeviewmodel));

                //comp.radioButtonFinanceUK.MouseClick -= new MouseEventHandler((s, e) => MainProcess.radioButtonFinance_CultureCheckedChanged(s, e, comp));
                //comp.radioButtonFinanceFR.MouseClick -= new MouseEventHandler((s, e) => MainProcess.radioButtonFinance_CultureCheckedChanged(s, e, comp));
                //comp.radioButtonFinanceUS.MouseClick -= new MouseEventHandler((s, e) => MainProcess.radioButtonFinance_CultureCheckedChanged(s, e, comp));
                //comp.radioButtonFinanceCA.MouseClick -= new MouseEventHandler((s, e) => MainProcess.radioButtonFinance_CultureCheckedChanged(s, e, comp));

            }
#endif

#if WPF
            if (onoroff)
            {
                comp.RBBanks.PreviewMouseLeftButtonDown += new MouseButtonEventHandler((s, e) => comp.Finance_CheckedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBSavings.PreviewMouseLeftButtonDown += new MouseButtonEventHandler((s, e) => comp.Finance_CheckedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBInvestments.PreviewMouseLeftButtonDown += new MouseButtonEventHandler((s, e) => comp.Finance_CheckedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBCryptos.PreviewMouseLeftButtonDown += new MouseButtonEventHandler((s, e) => comp.Finance_CheckedChanged(s, e, ourviewmodel, financeviewmodel));

                comp.RBNON.Click += new RoutedEventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel));
                comp.RBGBP.Click += new RoutedEventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel));
                comp.RBEUR.Click += new RoutedEventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel));
                comp.RBUSD.Click += new RoutedEventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel));
                comp.RBJPY.Click += new RoutedEventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel));

            }
#endif
#if WINUI
            // We only ever come here ONCE because it seems that the Chimps
            // never reduce the eventhandler count when you remove an eventhandler
            // so what happens is you get an event raised for every time you
            // define the eventhandler irrespective of the times you undefine it
            // Make sense? Of course not. The Chimp fuckers who wrote this bollocks
            // need fucking shooting
            if (onoroff)
            {
                comp.RBBanks.Tapped += new TappedEventHandler((s, e) => comp.Finance_CheckedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBSavings.Tapped += new TappedEventHandler((s, e) => comp.Finance_CheckedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBInvestments.Tapped += new TappedEventHandler((s, e) => comp.Finance_CheckedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBCryptos.Tapped += new TappedEventHandler((s, e) => comp.Finance_CheckedChanged(s, e, ourviewmodel, financeviewmodel));

                comp.RBNON.Click += new RoutedEventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel));
                comp.RBGBP.Click += new RoutedEventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel));
                comp.RBEUR.Click += new RoutedEventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel));
                comp.RBUSD.Click += new RoutedEventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel));
                comp.RBJPY.Click += new RoutedEventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel));

            }
#endif
#if SMARTMAUI
            // We only ever come here ONCE because it seems that the Chimps
            // never reduce the eventhandler count when you remove an eventhandler
            // so what happens is you get an event raised for every time you
            // define the eventhandler irrespective of the times you undefine it
            // Make sense? Of course not. The Chimp fuckers who wrote this bollocks
            // need fucking shooting
            if (onoroff)
            {
                comp.RBBanks.CheckedChanged += (s, e) => comp.Finance_CheckedChanged(s, e, ourviewmodel, financeviewmodel);
                comp.RBSavings.CheckedChanged += (s, e) => comp.Finance_CheckedChanged(s, e, ourviewmodel, financeviewmodel);
                comp.RBInvestments.CheckedChanged += (s, e) => comp.Finance_CheckedChanged(s, e, ourviewmodel, financeviewmodel);
                comp.RBCryptos.CheckedChanged += (s, e) => comp.Finance_CheckedChanged(s, e, ourviewmodel, financeviewmodel);

                comp.RBNON.CheckedChanged += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel);
                comp.RBGBP.CheckedChanged += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel);
                comp.RBEUR.CheckedChanged += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel);
                comp.RBUSD.CheckedChanged += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel);
                comp.RBJPY.CheckedChanged += (s, e) => RadioButtonRatesClicked(s, e, ourviewmodel, financeviewmodel);

            }
#endif
#if ANDROIDX
            // We only ever come here ONCE because it seems that the Chimps
            // never reduce the eventhandler count when you remove an eventhandler
            // so what happens is you get an event raised for every time you
            // define the eventhandler irrespective of the times you undefine it
            // Make sense? Of course not. The Chimp fuckers who wrote this bollocks
            // need fucking shooting
            if (onoroff)
            {
                financeviewmodel.RBBanks.Click += new EventHandler((s, e) => FinanceView.Finance_CheckedChanged(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.RBSavings.Click += new EventHandler((s, e) => FinanceView.Finance_CheckedChanged(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.RBInvestments.Click += new EventHandler((s, e) => FinanceView.Finance_CheckedChanged(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.RBCryptos.Click += new EventHandler((s, e) => FinanceView.Finance_CheckedChanged(s, e, meterActivity, ourviewmodel, financeviewmodel));

                financeviewmodel.FinanceRBNON.Click += new EventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.FinanceRBGBP.Click += new EventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.FinanceRBEUR.Click += new EventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.FinanceRBUSD.Click += new EventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.FinanceRBJPY.Click += new EventHandler((s, e) => FinanceView.RadioButtonRatesClicked(s, e, meterActivity, ourviewmodel, financeviewmodel));

            }
#endif
            return;
        }



#if WINFORMS
        internal static void RadioButtonTappedEvents(MainProcess comp, bool onoroff)
#endif
#if WPF  || WINUI
        internal static void RadioButtonTappedEvents(FinanceView comp, 
                                                    MainViewModel ourviewmodel, FinanceViewModel financeviewmodel, bool onoroff)
#endif
#if ANDROIDX
        internal static void RadioButtonTappedEvents(View comp, AppCompatActivity meterActivity, 
                                                    MainViewModel ourviewmodel, FinanceViewModel financeviewmodel, bool onoroff)
#endif
#if SMARTMAUI
        internal static void RadioButtonTappedEvents(FinanceView comp,
                                                    MainViewModel ourviewmodel, FinanceViewModel financeviewmodel, bool onoroff)
#endif
        {
#if WINFORMS
            // Don't need anything here
#endif
#if WPF
            if (onoroff)
            {
                comp.RBBanks.MouseRightButtonDown += new MouseButtonEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBSavings.MouseRightButtonDown += new MouseButtonEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBInvestments.MouseRightButtonDown += new MouseButtonEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBCryptos.MouseRightButtonDown += new MouseButtonEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));

            }
            else
            {
                comp.RBBanks.MouseRightButtonDown -= new MouseButtonEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBSavings.MouseRightButtonDown -= new MouseButtonEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBInvestments.MouseRightButtonDown -= new MouseButtonEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBCryptos.MouseRightButtonDown -= new MouseButtonEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
            }
#endif
#if WINUI
            if (onoroff)
            {

                comp.RBBanks.RightTapped += new RightTappedEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBSavings.RightTapped += new RightTappedEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBInvestments.RightTapped += new RightTappedEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBCryptos.RightTapped += new RightTappedEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
            }
            else
            {
                comp.RBBanks.RightTapped -= new RightTappedEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBSavings.RightTapped -= new RightTappedEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBInvestments.RightTapped -= new RightTappedEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));
                comp.RBCryptos.RightTapped -= new RightTappedEventHandler((s, e) => comp.Finance_TappedChanged(s, e, ourviewmodel, financeviewmodel));    
            }
#endif
#if SMARTMAUI

            // All this bollocks just trying to get a 'longpress' on the Radio button
            if (onoroff)
            {
                rbBanksLP = new LongPressHandler(comp.RBBanks, () =>
                            comp.Finance_TappedChanged(comp.RBBanks, ourviewmodel, financeviewmodel));
                rbSavingsLP = new LongPressHandler(comp.RBSavings, () =>
                            comp.Finance_TappedChanged(comp.RBSavings, ourviewmodel, financeviewmodel));
                rbInvestmentsLP = new LongPressHandler(comp.RBInvestments, () =>
                            comp.Finance_TappedChanged(comp.RBInvestments, ourviewmodel, financeviewmodel));
                rbCryptosLP = new LongPressHandler(comp.RBCryptos, () =>
                            comp.Finance_TappedChanged(comp.RBCryptos, ourviewmodel, financeviewmodel));
            }
            else
            {
                // Cant really unsubscribe to a Longpress
                // but this is the next beast thing
                rbBanksLP?.Detach();
                rbBanksLP = null;
                rbSavingsLP?.Detach();
                rbSavingsLP = null;
                rbInvestmentsLP?.Detach();
                rbInvestmentsLP = null;
                rbCryptosLP?.Detach();
                rbCryptosLP = null;
            }    
#endif

#if ANDROIDX
            if (onoroff)
            {
                financeviewmodel.RBBanks.LongClick += new EventHandler<LongClickEventArgs>((s, e) => FinanceView.Finance_TappedChanged(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.RBSavings.LongClick += new EventHandler<LongClickEventArgs>((s, e) => FinanceView.Finance_TappedChanged(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.RBInvestments.LongClick += new EventHandler<LongClickEventArgs>((s, e) => FinanceView.Finance_TappedChanged(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.RBCryptos.LongClick += new EventHandler<LongClickEventArgs>((s, e) => FinanceView.Finance_TappedChanged(s, e, meterActivity, ourviewmodel, financeviewmodel));
            }
            else
            {
                financeviewmodel.RBBanks.LongClick -= new EventHandler<LongClickEventArgs>((s, e) => FinanceView.Finance_TappedChanged(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.RBSavings.LongClick -= new EventHandler<LongClickEventArgs>((s, e) => FinanceView.Finance_TappedChanged(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.RBInvestments.LongClick -= new EventHandler<LongClickEventArgs>((s, e) => FinanceView.Finance_TappedChanged(s, e, meterActivity, ourviewmodel, financeviewmodel));
                financeviewmodel.RBCryptos.LongClick -= new EventHandler<LongClickEventArgs>((s, e) => FinanceView.Finance_TappedChanged(s, e, meterActivity, ourviewmodel, financeviewmodel));
            }
#endif
        }

#if SMARTMAUI
        // What a load of complete and utter bollocks this MAUI shit is ...        
        public class LongPressHandler
        {
            private readonly View _view;
            private readonly Action _action;
            private readonly int _delayMs;

            private IDispatcherTimer _timer;
            private TapGestureRecognizer _recognizer;

            public LongPressHandler(View view, Action action, int delayMs = 600)
            {
                _view = view;
                _action = action;
                _delayMs = delayMs;

                Attach();
            }

            private void Attach()
            {
                _recognizer = new TapGestureRecognizer();

                _recognizer.Tapped += OnTapped;

                _view.GestureRecognizers.Add(_recognizer);
            }

            private void OnTapped(object sender, EventArgs e)
            {
                _timer?.Stop();

                _timer = _view.Dispatcher.CreateTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(_delayMs);

                _timer.Tick += (_, _) =>
                {
                    _timer.Stop();
                    _action();
                };

                _timer.Start();
            }

            public void Detach()
            {
                if (_recognizer != null)
                {
                    _recognizer.Tapped -= OnTapped;
                    _view.GestureRecognizers.Remove(_recognizer);
                }

                _timer?.Stop();
                _timer = null;
            }
        }
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
#if WINFORMS
        internal void ChartsMouseDoubleClick(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WPF
        internal void ChartsMouseDoubleClick(object sender, RoutedEventArgs e,
                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WINUI || SMARTMAUI
        internal void ChartsMouseDoubleClick(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
        {
            if (sender != null &&
#if WINFORMS || SMARTMAUI
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WPF  || WINUI
                FrontEndGUI.DecodeRoutedEventFlags(e) != null)
#endif
            {
                if (1 == 1) //e.ClickCount == 1)
                {
                    string[] tabinfo = FrontEndGUI.DecodeTabControlFinance(this, sender);
                    if (tabinfo.Length == 2)
                    {
#if WINFORMS || SMARTMAUI
                        SmartFinanceV2025.ChartsMouseDoubleClick_Actual(ourviewmodel,
                                                                        financeviewmodel,
                                                                        tabinfo);
#endif
#if WPF
                        SmartFinanceV2025.ChartsMouseDoubleClick_Actual(signinviewmodel,
                                                                        ourviewmodel,
                                                                        financeviewmodel,
                                                                        tabinfo);
#endif
#if WINUI
                        SmartFinanceV2025.ChartsMouseDoubleClick_Actual(signinviewmodel,
                                                                        ourviewmodel,
                                                                        financeviewmodel,
                                                                        tabinfo);
#endif
                    }
                }
#if WPF
                // Not sure what effect THIS has
                e.Handled = true;
#endif
            }
            return;
        }
#endif

#if WPF  || WINUI || SMARTMAUI

#if WPF
        internal void FinanceProviders_DropDownClosed(object sender, EventArgs e,
                                                FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal void FinanceProviders_DropDownClosed(object sender, object e,
                                                FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal void FinanceProviders_DropDownClosed(object sender, EventArgs e,
                                                FinanceViewModel financeviewmodel)
#endif
        {
            if (sender != null &&
                e != null)
            {
                financeviewmodel.IDDdropDownClicked = true;
#if WPF
                ComboBox ProvidersComboBox = sender as ComboBox;
#endif
                if (sender != null && e != null)
                {
                    SmartFinanceV2025.Finance_ProvidersChanged_Actual(ourviewmodel,
                                                                    financeviewmodel);
                }
                // Reset selection temporarily to allow re-selection of the same item
                ////int selectedIndex = FinanceProviders.SelectedIndex;
                // Clear selection momentarily to force SelectionChanged event to trigger
                ////FinanceProviders.SelectedIndex = -1;
                // Restore the previous selection again forcing SelectionChanged event to trigger
                ////FinanceProviders.SelectedIndex = selectedIndex;
            }
            return;
        }
#endif

#if WPF  || WINUI || SMARTMAUI

#if WPF
        internal static void FinanceAccounts_DropDownClosed(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal void FinanceAccounts_DropDownClosed(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal void FinanceAccounts_DropDownClosed(object sender, EventArgs e,
                                                FinanceViewModel financeviewmodel)
#endif
        {
            if (sender != null && e != null)
            {
                SmartFinanceV2025.Finance_AccountsChanged_Actual(ourviewmodel,
                                                                financeviewmodel);
            }
            return;
        }
#endif

#if WPF  || WINUI

#if WPF
        internal static void FinanceTransactionGroups_DropDownClosed(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal void FinanceTransactionGroups_DropDownClosed(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
        {
            if (sender != null && e != null)
            {
                SmartFinanceV2025.Finance_TransactionGroupsChanged_Actual(ourviewmodel,
                                                                        financeviewmodel);
            }
            return;
        }
#endif

#if WPF
        // I haven't got the time, the energy OR the patience
        // to fuck around trying to defeat these OBVIOUS WPF bugs
        // to deal with when this event handler is called.  It seems
        // to be called EVEN WHEN its being defined right at the start
        // of the Event Handler being defined and it fucks about being
        // called every 5 seconds (of its own accord) thereafter.
        // Its a pile of crap .. but at least it seems to set the start
        // and end dates correctly when I change the Address to cover
        // different ranges.  I KNOW it's full of bugs, peculiarities
        // and Chimp-infested shit ...
#endif
#if WINFORMS
        internal static void StartingDateChanged(object sender, EventArgs e,
                                                MainProcess process_components,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {
#endif
#if WPF
        internal static void StartingDateChanged(object sender,
                                        SelectionChangedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal void StartingDateChanged(object sender,
                                        DatePickerSelectedValueChangedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel)
        {
#endif
#if ANDROIDX
        internal static void StartingDateChanged(object sender, EventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel)
        {
#endif
#if SMARTMAUI
        internal void StartingDateChanged(object sender,
                                        DateChangedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel)
        {
#endif
            // Sometimes (!!) we come here when we're least expecting it ...
            if (sender != null && e != null)
            {
#if WINFORMS
                DateTimePicker datepicker = (DateTimePicker)sender;
                if (datepicker.Enabled &&
                    datepicker.Value != SmartParametersV2016.defaultDate)
                {
                    // Has the date changed through the keyboard
                    if (SmartTimeV2016.ConvertDateTime(datepicker.Value.ToString()) != financeviewmodel.StartDate)
                    {
#endif
#if ANDROIDX
                DateTime selectedDate = FrontEndGUI.DecodeDatePicker(sender);
                if (selectedDate != financeviewmodel.StartDate.DateTime)
                {
                    financeviewmodel.StartDate.DateTime = selectedDate;
#endif
#if WPF  || WINUI || SMARTMAUI

                DatePicker datepicker = (DatePicker)sender;
#if WPF  || WINUI
                if (datepicker.IsEnabled &&
                    datepicker.SelectedDate != SmartParametersV2016.defaultDate)
#endif
#if SMARTMAUI
                if (datepicker.IsEnabled &&
                    datepicker.Date != SmartParametersV2016.defaultDate)
#endif
                {
                    // Has the date changed through the keyboard
#if WPF
                    if (datepicker.IsMouseOver)
                    {
#endif
#if SMARTMAUI
                    if (datepicker.Date != financeviewmodel.StartDate)
                    {
#endif
#if WINUI
                    // The SelectedDate is TWO-WAY in WINUI in order to
                    // update financeviewmodel ....
                    if (datepicker.IsEnabled)
                    {
#endif

#endif
#if ANDROIDX
                    if (true)
                    {
#endif

                            SmartFinanceV2025.Finance_SubsetTransactions(
#if WINFORMS
                                                    process_components,

                                                    process_components.comboBoxGUIFinanceProviders,
                                                    process_components.comboBoxGUIFinanceAccounts,
                                                    process_components.comboBoxGUIFinanceTransactionGroups,
                                                    process_components.TransactionsDataGrid,
#endif
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    "",
                                                    false, // Don't respect Addresses
#if WINFORMS
                                                    //SmartTimeV2016.ConvertDateTime(datepicker.Value.ToString()),


                                                    financeviewmodel.StartDate,
                                                    financeviewmodel.EndDate);
#endif
#if WPF  || WINUI || SMARTMAUI
                                                    financeviewmodel.StartDate,
                                                    financeviewmodel.EndDate);
#endif
#if ANDROIDX
                                                    financeviewmodel.StartDate.DateTime,
                                                    financeviewmodel.EndDate.DateTime);
#endif

#if WPF
                        // Not sure what effect THIS has - nothing!
                        e.Handled = true;
#endif
                    }
                }
            }
            return;
        }

#if WPF || SMARTMAUI
        // I haven't got the time, the energy OR the patience
        // to fuck around trying to defeat these OBVIOUS WPF bugs
        // to deal with when this event handler is called.  It seems
        // to be called EVEN WHEN its being defined right at the start
        // of the Event Handler being defined and it fucks about being
        // called every 5 seconds (of its own accord) thereafter.
        // Its a pile of crap .. but at least it seems to set the start
        // and end dates correctly when I change the Address to cover
        // different ranges.  I KNOW it's full of bugs, peculiarities
        // and Chimp-infested shit ...
#endif
#if WINFORMS
        internal static void EndingDateChanged(object sender, EventArgs e,
                                            MainProcess process_components,
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel)
        {
#endif
#if WPF
        internal static void EndingDateChanged(object sender,
                                        SelectionChangedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal void EndingDateChanged(object sender,
                                        DatePickerSelectedValueChangedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel)
        {
#endif
#if ANDROIDX
        internal static void EndingDateChanged(object sender, EventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel)
        {
#endif
#if SMARTMAUI
        internal void EndingDateChanged(object sender,
                                                DateChangedEventArgs e,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {

#endif
            // Sometimes (!!) we come here when we're least expecting it ...
            if (sender != null && e != null)
            {
#if WINFORMS
                DateTimePicker datepicker = (DateTimePicker)sender;
                if (datepicker.Enabled &&
                    datepicker.Value != SmartParametersV2016.defaultMaxdate)
                {
                    // Has the date changed through the keyboard
                    if (SmartTimeV2016.ConvertDateTime(datepicker.Value.ToString()) != financeviewmodel.EndDate)
                    {
#endif
#if ANDROIDX
                DateTime selectedDate = FrontEndGUI.DecodeDatePicker(sender);
                if (selectedDate !=
                    financeviewmodel.EndDate.DateTime)
                {
                    financeviewmodel.EndDate.DateTime = selectedDate;
#endif
#if WPF  || WINUI || SMARTMAUI
                DatePicker datepicker = (DatePicker)sender;
#if WPF  || WINUI
                if (datepicker.IsEnabled &&
                    datepicker.SelectedDate != SmartParametersV2016.defaultMaxdate)
#endif
#if SMARTMAUI
                if (datepicker.IsEnabled &&
                    datepicker.Date != SmartParametersV2016.defaultMaxdate)
#endif
                {
                    // Has the date changed through the keyboard
#if WPF
                    if (datepicker.IsMouseOver)
                    {
#endif
#if SMARTMAUI
                    if (datepicker.Date != financeviewmodel.EndDate)
                    {
#endif
#if WINUI
                    // The SelectedDate is TWO-WAY in WINUI in order to
                    // update financeviewmodel ....
                    if (datepicker.IsEnabled)
                    {
#endif
#endif
#if ANDROIDX
                    if (true)
                    {
#endif
                            // Crude, but effective??
                            SmartFinanceV2025.Finance_SubsetTransactions(
#if WINFORMS
                                                        process_components,

                                                        process_components.comboBoxGUIFinanceProviders,
                                                        process_components.comboBoxGUIFinanceAccounts,
                                                        process_components.comboBoxGUIFinanceTransactionGroups,
                                                        process_components.TransactionsDataGrid,
#endif
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        "",
                                                        false, // Don't respect Addresses

#if WINFORMS
                                                        //SmartTimeV2016.ConvertDateTime(datepicker.Value.ToString()),

                                                        financeviewmodel.StartDate,
                                                        financeviewmodel.EndDate);

#endif
#if WPF  || WINUI || SMARTMAUI
                                                        financeviewmodel.StartDate,
                                                        financeviewmodel.EndDate);
#endif
#if ANDROIDX
                                                        financeviewmodel.StartDate.DateTime,
                                                        financeviewmodel.EndDate.DateTime);
#endif
#if WPF
                        // Not sure what effect THIS has - nothing!
                        e.Handled = true;
#endif
                    }
                }
            }
            return;
        }


#if WPF || SMARTMAUI
        // I haven't got the time, the energy OR the patience
        // to fuck around trying to defeat these OBVIOUS WPF bugs
        // to deal with when this event handler is called.  It seems
        // to be called EVEN WHEN its being defined right at the start
        // of the Event Handler being defined and it fucks about being
        // called every 5 seconds (of its own accord) thereafter.
        // Its a pile of crap .. but at least it seems to set the start
        // and end dates correctly when I change the Address to cover
        // different ranges.  I KNOW it's full of bugs, peculiarities
        // and Chimp-infested shit ...
#endif
#if WINFORMS
        internal static void Finance_ResetDates(object sender, EventArgs e,
                                                MainProcess process_components,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {
#endif
#if WPF
        internal static void Finance_ResetDates(object sender,
                                        EventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal void Finance_ResetDates(object sender,
                                        RoutedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel)
        {
#endif
#if ANDROIDX
        internal static void Finance_ResetDates(object sender, EventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel)
        {
#endif
#if SMARTMAUI
        internal static void Finance_ResetDates(object sender, EventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel)
        {
#endif
            // Sometimes (!!) we come here when we're least expecting it ...
            if (sender != null && e != null)
            {
#if WINFORMS
                Button resetdates = (Button)sender;
                if (resetdates.Enabled)
                {
#endif
#if WPF  || WINUI || SMARTMAUI
                Button resetdates = (Button)sender;
                if (resetdates.IsEnabled)
                {
#endif
#if ANDROIDX
                Button resetdates = (Button)sender;
                if (resetdates.Enabled)
                {
#endif
                    SmartFinanceV2025.Finance_ResetDates_Actual(
#if WINFORMS
                                                                    process_components,
#endif
                                                                    ourviewmodel,
                                                                    financeviewmodel);
                }
            }
            return;
        }

#if WINFORMS
        internal static void Providers_Changed(object sender, EventArgs e, MainProcess process_components,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {
            if (sender != null && e != null)
            {
                SmartFinanceV2025.Finance_ProvidersChanged_Actual(
                                                                    process_components,
                                                                    ourviewmodel,
                                                                    financeviewmodel);
                // Amazingly ... this seems to stop the event firing TWICE
                // .. but also, not being ambushed by FUCKING CHIMPS helps as well.
            }
            return;
        }
#endif

#if WINFORMS
        internal static void Accounts_Changed(object sender, EventArgs e, MainProcess process_components,
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)
        {
            if (sender != null && e != null)
            {
                SmartFinanceV2025.Finance_AccountsChanged_Actual(
                                                                    process_components,
                                                                    ourviewmodel,
                                                                    financeviewmodel);
                // Amazingly ... this seems to stop the event firing TWICE
                // .. but also, not being ambushed by FUCKING CHIMPS helps as well.
            }
            return;
        }
#endif

#if WINFORMS
        internal static void TransactionGroups_Changed(object sender, EventArgs e, MainProcess process_components,
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)
        {
            if (sender != null && e != null)
            {
                SmartFinanceV2025.Finance_TransactionGroupsChanged_Actual(
                                                                    process_components,
                                                                    ourviewmodel,
                                                                    financeviewmodel);
                // Amazingly ... this seems to stop the event firing TWICE
                // .. but also, not being ambushed by FUCKING CHIMPS helps as well.
            }
            return;
        }
#endif

#if WINFORMS
        internal static async void Finance_CheckedChanged(object sender, MouseEventArgs e,
                                                            MainProcess components,
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel)
#endif
#if WPF
        internal async void Finance_CheckedChanged(object sender, MouseButtonEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal async void Finance_CheckedChanged(object sender, RoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        internal static async void Finance_CheckedChanged(object sender, EventArgs e,
                                                    AppCompatActivity meterActivity,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal async void Finance_CheckedChanged(object sender, CheckedChangedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif

        {
            // Even though CategoriesEnabled is FALSE here
            // (so - in theory - the button is DISABLED ...)
            // it STILL calls this routine!!!  What a pile
            // of shit this stuff is ....
            if (sender != null && e != null)
            {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                RadioButton radiobutton = sender as RadioButton;
#endif
#if ANDROIDX
                RadioButton radiobutton = sender as RadioButton;
#endif
#if WINFORMS
                if (radiobutton.Enabled)
#endif
#if WPF  || WINUI || SMARTMAUI
                if (radiobutton.IsEnabled)
#endif
#if ANDROIDX
                if (radiobutton.Enabled)
#endif

                {
#if WINFORMS
                    // Despite putting AutoClick to false this Chimp-deranged
                    // bollocks STILL raises and Event even when all
                    // events are off.  Its a pile of shite ...
                    if (true)//(bool)radiobutton.Checked)
                    {
#endif
#if ANDROIDX
                        // Despite putting AutoClick to false this Chimp-deranged
                        // bollocks STILL raises and Event even when all
                        // events are off.  Its a pile of shite ...
                        if ((bool)radiobutton.Checked)
                        {
#endif
#if WPF || SMARTMAUI
                    // When the Preview Left Button is pressed the button ISN'T checked
                    // Completely different from WINUI ... but what else can you expect from Chimps??
                    if ((bool)radiobutton.IsChecked)// || !(bool)radiobutton.IsChecked)
                    {
#endif
#if WINUI
                    // When the LeftButton is pressed the button IS checked
                    // Completely different from WPF ... but what else can you expect from Chimps??
                    if ((bool)radiobutton.IsChecked)
                    {
#endif
#if WINFORMS
                        // Don't fuck about with the Events
                        // Otherwise the Chimps generate multiple events
                        if (radiobutton.Tag != null)
#endif
#if ANDROIDX
                        // Turn em off
                        // RadioButtonCheckedEvents(this, false);

                        if (radiobutton.Tag != null)
#endif
#if WPF 
                        if (radiobutton.Tag != null)
#endif
#if SMARTMAUI
                        if (radiobutton.Value != null)
#endif
                        {
#if WINFORMS || WPF  || WINUI
                            char classid = Convert.ToChar(radiobutton.Tag.ToString());
#endif
#if SMARTMAUI
                            char classid = Convert.ToChar(radiobutton.Value.ToString());
#endif
#if ANDROIDX
                            char classid = Convert.ToChar(radiobutton.Tag.ToString());
#endif
                            financeviewmodel.categoryCodes = "";
                            foreach (SmartFinance.Categories category_row in financeviewmodel.PLO.finance_categoriesList)
                            {
                                if (category_row.CATEGORY_CODE == classid)
                                {
                                    switch (category_row.CATEGORY_CODE)
                                    {
                                        case 'B':
#if WINFORMS
                                            if (!components.RBBanks.Checked)
                                            {
                                                components.RBBanks.Checked = true;
                                                category_row.CHECKED = SmartParametersV2016.lastChecked;
                                                category_row.Updated = true;
                                            }
#endif
#if WPF  || WINUI || SMARTMAUI
                                            if (!(bool)this.RBBanks.IsChecked)
                                            {
                                                this.RBBanks.IsChecked = true;
                                            }
                                            category_row.CHECKED = SmartParametersV2016.lastChecked;
                                            category_row.Updated = true;

#endif
#if ANDROIDX
                                            if (!(bool)financeviewmodel.RBBanks.Checked)
                                            {
                                                financeviewmodel.RBBanks.Checked = true;
                                            }
                                            category_row.CHECKED = SmartParametersV2016.lastChecked;
                                            category_row.Updated = true;
#endif
                                            SmartFinanceV2025.FixCategoryTypes(financeviewmodel, category_row);
                                            financeviewmodel.categoryCodes = category_row.CATEGORY_CODE.ToString();
                                            financeviewmodel.ProvidersName = SmartParametersV2016.BanksProvidersName;
#if WPF  || WINUI || SMARTMAUI
                                            FinanceProviders.ItemsSource = financeviewmodel.FinanceProvidersList;

#endif
                                            break;
                                        case 'S':
#if WINFORMS
                                            if (!components.RBSavings.Checked)
                                            {
                                                components.RBSavings.Checked = true;
                                                category_row.CHECKED = SmartParametersV2016.lastChecked;
                                                category_row.Updated = true;
                                            }
#endif
#if WPF  || WINUI || SMARTMAUI
                                            if (!(bool)this.RBSavings.IsChecked)
                                            {
                                                this.RBSavings.IsChecked = true;
                                            }
                                            category_row.CHECKED = SmartParametersV2016.lastChecked;
                                            category_row.Updated = true;
#endif
#if ANDROIDX
                                            if (!(bool)financeviewmodel.RBSavings.Checked)
                                            {
                                                financeviewmodel.RBSavings.Checked = true;
                                            }
                                            category_row.CHECKED = SmartParametersV2016.lastChecked;
                                            category_row.Updated = true;
#endif
                                            SmartFinanceV2025.FixCategoryTypes(financeviewmodel, category_row);
                                            financeviewmodel.categoryCodes = category_row.CATEGORY_CODE.ToString();
                                            financeviewmodel.ProvidersName = SmartParametersV2016.SavingsProvidersName;
#if WPF  || WINUI || SMARTMAUI
                                            FinanceProviders.ItemsSource = financeviewmodel.FinanceProvidersList;
#endif
                                            break;
                                        case 'I':
#if WINFORMS
                                            if (!components.RBInvestments.Checked)
                                            {
                                                components.RBInvestments.Checked = true;
                                                category_row.CHECKED = SmartParametersV2016.lastChecked;
                                                category_row.Updated = true;
                                            }
#endif
#if WPF  || WINUI || SMARTMAUI
                                            if (!(bool)this.RBInvestments.IsChecked)
                                            {
                                                this.RBInvestments.IsChecked = true;
                                            }
                                            category_row.CHECKED = SmartParametersV2016.lastChecked;
                                            category_row.Updated = true;
#endif
#if ANDROIDX
                                            if (!(bool)financeviewmodel.RBInvestments.Checked)
                                            {
                                                financeviewmodel.RBInvestments.Checked = true;
                                            }
                                            category_row.CHECKED = SmartParametersV2016.lastChecked;
                                            category_row.Updated = true;
#endif
                                            SmartFinanceV2025.FixCategoryTypes(financeviewmodel, category_row);
                                            financeviewmodel.categoryCodes = category_row.CATEGORY_CODE.ToString();
                                            financeviewmodel.ProvidersName = SmartParametersV2016.InvestmentsProvidersName;
#if WPF  || WINUI || SMARTMAUI
                                            FinanceProviders.ItemsSource = financeviewmodel.FinanceProvidersList;
#endif
                                            break;
                                        case 'C':
#if WINFORMS
                                            if (!components.RBCryptos.Checked)
                                            {
                                                components.RBCryptos.Checked = true;
                                                category_row.CHECKED = SmartParametersV2016.lastChecked;
                                                category_row.Updated = true;
                                            }
#endif
#if WPF  || WINUI || SMARTMAUI
                                            if (!(bool)this.RBCryptos.IsChecked)
                                            {
                                                this.RBCryptos.IsChecked = true;
                                            }
                                            category_row.CHECKED = SmartParametersV2016.lastChecked;
                                            category_row.Updated = true;
#endif
#if ANDROIDX
                                            if (!(bool)financeviewmodel.RBCryptos.Checked)
                                            {
                                                financeviewmodel.RBCryptos.Checked = true;
                                            }
                                            category_row.CHECKED = SmartParametersV2016.lastChecked;
                                            category_row.Updated = true;
#endif
                                            SmartFinanceV2025.FixCategoryTypes(financeviewmodel, category_row);
                                            financeviewmodel.categoryCodes = category_row.CATEGORY_CODE.ToString();
                                            financeviewmodel.ProvidersName = SmartParametersV2016.CryptosProvidersName;
#if WPF  || WINUI || SMARTMAUI
                                            //FinanceProviders.ItemsSource = financeviewmodel.FinanceWalletsList;
#endif
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (category_row.CATEGORY_CODE)
                                    {
                                        case 'B':
#if WINFORMS
                                            if (components.RBBanks.Checked)
                                            {
                                                components.RBBanks.Checked = false;
                                                category_row.CHECKED = SmartParametersV2016.unChecked;
                                                category_row.Updated = true;
                                            }
#endif
#if WPF  || WINUI || SMARTMAUI
                                            if ((bool)this.RBBanks.IsChecked)
                                            {
                                                this.RBBanks.IsChecked = false;
                                                category_row.CHECKED = SmartParametersV2016.unChecked;
                                                category_row.Updated = true;
                                            }
#endif
#if ANDROIDX
                                            if ((bool)financeviewmodel.RBBanks.Checked)
                                            {
                                                financeviewmodel.RBBanks.Checked = false;
                                                category_row.CHECKED = SmartParametersV2016.unChecked;
                                                category_row.Updated = true;
                                            }
#endif
                                            SmartFinanceV2025.FixCategoryTypes(financeviewmodel, category_row);
                                            break;
                                        case 'S':
#if WINFORMS
                                            if (components.RBSavings.Checked)
                                            {
                                                components.RBSavings.Checked = false;
                                                category_row.CHECKED = SmartParametersV2016.unChecked;
                                                category_row.Updated = true;
                                            }
#endif
#if WPF  || WINUI || SMARTMAUI
                                            if ((bool)this.RBSavings.IsChecked)
                                            {
                                                this.RBSavings.IsChecked = false;

                                                category_row.CHECKED = SmartParametersV2016.unChecked;
                                                category_row.Updated = true;
                                            }
#endif
#if ANDROIDX
                                            if ((bool)financeviewmodel.RBSavings.Checked)
                                            {
                                                financeviewmodel.RBSavings.Checked = false;

                                                category_row.CHECKED = SmartParametersV2016.unChecked;
                                                category_row.Updated = true;
                                            }
#endif
                                            SmartFinanceV2025.FixCategoryTypes(financeviewmodel, category_row);
                                            break;
                                        case 'I':
#if WINFORMS
                                            if (components.RBInvestments.Checked)
                                            {
                                                components.RBInvestments.Checked = false;
                                                category_row.CHECKED = SmartParametersV2016.unChecked;
                                                category_row.Updated = true;
                                            }
#endif
#if WPF  || WINUI || SMARTMAUI
                                            if ((bool)this.RBInvestments.IsChecked)
                                            {
                                                this.RBInvestments.IsChecked = false;

                                                category_row.CHECKED = SmartParametersV2016.unChecked;
                                                category_row.Updated = true;
                                            }
#endif
#if ANDROIDX
                                            if ((bool)financeviewmodel.RBInvestments.Checked)
                                            {
                                                financeviewmodel.RBInvestments.Checked = false;

                                                category_row.CHECKED = SmartParametersV2016.unChecked;
                                                category_row.Updated = true;
                                            }
#endif
                                            SmartFinanceV2025.FixCategoryTypes(financeviewmodel, category_row);
                                            break;
                                        case 'C':
#if WINFORMS
                                            if (components.RBCryptos.Checked)
                                            {
                                                components.RBCryptos.Checked = false;
                                                category_row.CHECKED = SmartParametersV2016.unChecked;
                                                category_row.Updated = true;
                                            }
#endif
#if WPF  || WINUI || SMARTMAUI
                                            if ((bool)this.RBCryptos.IsChecked)
                                            {
                                                this.RBCryptos.IsChecked = false;

                                                category_row.CHECKED = SmartParametersV2016.unChecked;
                                                category_row.Updated = true;
                                            }
#endif
#if ANDROIDX
                                            if ((bool)financeviewmodel.RBCryptos.Checked)
                                            {
                                                financeviewmodel.RBCryptos.Checked = false;

                                                category_row.CHECKED = SmartParametersV2016.unChecked;
                                                category_row.Updated = true;
                                            }
#endif
                                            SmartFinanceV2025.FixCategoryTypes(financeviewmodel, category_row);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }

                            // I'm going to keep this next test in, just in case I ever encounter that situation again
                            // where the fucking event handler decides to call itself TWICE (or more) as the chimps seem 
                            // to insist it has to ..

                            // Despite everything and all the pain
                            // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!

                            if (!await SmartFinanceV2025.Finance_RadioButtonChecked_Actual(
#if WINFORMS
                                                                                        components,
#endif
#if ANDROIDX
                                                                                        meterActivity,
#endif
                                                                                        ourviewmodel,
                                                                                        financeviewmodel))
                            {
                                GiveUp(ourviewmodel, financeviewmodel, "RadioButton switch failed");
                            }
                            else
                            {
#if WINFORMS
                                // Don't turn this on otherwise
                                // the fuckingChimps generate multiple events!!!
#endif
#if ANDROIDX
                                // See the notes inside
                                // RadioButtonCheckedEvents(this, true);
#endif

                                SmartRoutinesV2018.FocusOpenClose(ourviewmodel);
                            }
                        }

#if WPF
                        // Not sure what effect THIS has
                        // No WINUI
                        e.Handled = true;
#endif
                    }
                }
            }
            return;
        }

        // For reasons which escape me but the Microshit Chimpanzees will guess at,
        // this event is called even when there is nothing in the drop-down
#if WINFORMS
        internal void Finance_AddressSelectionChanged(object sender, object e,
                                                        MainProcess process_components,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel)
#endif
#if WPF  || WINUI
        internal static void Finance_AddressSelectionChanged(object sender, RoutedEventArgs e,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        internal static void Finance_AddressSelectionChanged(object sender, object e,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal static void Finance_AddressSelectionChanged(object sender, object e,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel)
#endif

        {
            if (sender != null &&
#if WINFORMS || WPF || SMARTMAUI
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WINUI
                FrontEndGUI.DecodeSelectionChangedEventFlags(e) != null)
#endif
#if ANDROIDX
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
            {
                int index = FrontEndGUI.DecodeSenderIndex(sender);
                // I don't believe it... this bollocks is ACTUALLY WORKING
                if (index == financeviewmodel.AddressSelectedIndex)
                {
                    // This just calls Transactions Subset with respect Dates
                    SmartFinanceV2025.Finance_AddressesChanged_Actual(
#if WINFORMS
                                                                    process_components,
#endif
                                                                    ourviewmodel,
                                                                    financeviewmodel);
                }
#if WPF
                // Not sure what effect THIS has (stops it bubbling up)
                e.Handled = true;
#endif
            }
            return;
        }

#if ANDROIDX
        // FinanceTransactionSelected is done in the Transactions Fragment
        // as an ItemClick event (hopefully)
        //DataGrid abc = (DataGrid)sender;
        // I don't believe it... this bollocks is ACTUALLY WORKING
        // SmartFinance.TransactionsView xyz = abc.SelectedItem as SmartFinance.TransactionsView;
        // financeviewmodel.SortCode = xyz.SORTCODE;
        // financeviewmodel.AccountNo = xyz.ACCOUNT_NO;
        // return;
#endif

#if WINFORMS || WPF  || WINUI || SMARTMAUI
#if WINFORMS
        internal void FinanceTransactionSelected(object sender, EventArgs e, FinanceViewModel financeviewmodel)
#endif

#if WPF  || WINUI
        internal void FinanceTransactionSelected(object sender, SelectionChangedEventArgs e, FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal void FinanceTransactionSelected(object sender, SelectionChangedEventArgs e, FinanceViewModel financeviewmodel)
#endif
        {
            if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
            {
#if WINFORMS
                ComboBox abc = (ComboBox)sender;
#endif
#if WPF  || WINUI
                ListView abc = (ListView)sender;
#endif
#if SMARTMAUI
                CollectionView abc = (CollectionView)sender;
#endif
                // I don't believe it... this bollocks is ACTUALLY WORKING
                SmartFinance.CommonTransactionsView xyz = abc.SelectedItem as SmartFinance.CommonTransactionsView;
                financeviewmodel.SortCode = xyz.SORTCODE;
                financeviewmodel.AccountNo = xyz.ACCOUNT_NO;
#if WPF
                // Not sure what effect THIS has (stops it bubbling up)
                e.Handled = true;
#endif
            }
            return;
        }
#endif

#if WINFORMS
        internal static async void FinanceCultures_SelectionChanged(object sender, EventArgs e,
                                                                MainProcess components,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel)

        {
#endif
#if WPF
        internal static async void FinanceCultures_SelectionChanged(object sender, RoutedEventArgs e,
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal async void FinanceCultures_SelectionChanged(object sender, SelectionChangedEventArgs e,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel)
        {
#endif
#if ANDROIDX
        internal async static void FinanceCultures_SelectionChanged(object sender, EventArgs e,
#if ANDROIDX
                                                            AppCompatActivity meterActivity,
#endif
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel)
        {
#endif
#if SMARTMAUI
        internal static async void FinanceCultures_SelectionChanged(object sender, EventArgs e,
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel)
        {
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            // For WINFORMS, if you modify the comboBox items in here, you
            // get an infinite chimp-generated loop ... you couldn't make this shit up,
            // you really couldn't
#if WINFORMS || WPF  || WINUI
            ComboBox spinner = sender as ComboBox;
#endif
#if SMARTMAUI
            Picker spinner = sender as Picker;
#endif
#if WPF  || WINUI || SMARTMAUI
            // Well ...why is this Bollocks Chimp-trap here?  Because
            // whilst evrything works FINE with WPF, the WINUI bollocks
            // a) has to have the ComboBox Editable="true" and ALSO (!!!)
            // b) when you update using Notify in the financeviewmodel,
            // it generates the "SelectionChanged" event for reasons best known
            // to its chimpy self.  So I trap the fact the SelectedIndex is -1
            // and return.  Then it comes BACK to the SelectionChanged routine
            // (so thats THREE Chimp-times) and exits gracefully because the
            // culture_mode hasn't changed ...  You couldn't make this chimp-fuck
            // shit up, you really couldn't ...
            if (spinner.SelectedIndex == -1)
            {
                return;
            }
#endif
#endif
#if ANDROIDX
            // By LUCK more than by anything else!!!!
            Spinner spinner = sender as Spinner;
#endif
#if WINFORMS
            if (spinner.Enabled)
#endif
#if WPF  || WINUI || SMARTMAUI
            if (spinner.IsEnabled)
#endif
#if ANDROIDX
            if (spinner.Enabled)
#endif
            {
#if WINFORMS
                if (FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WPF  || WINUI
                if (FrontEndGUI.DecodeRoutedEventFlags(e) != null)
#endif
#if SMARTMAUI
                if (FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if ANDROIDX
                if (FrontEndGUI.DecodeCheckedChangedEventFlags(e) != null)
#endif
                {

#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    MainViewModel.CultureItem citem = spinner.SelectedItem as MainViewModel.CultureItem;
                    string culture_code = citem.Content;
                    int position = spinner.SelectedIndex;
#endif
#if ANDROIDX
                    int position = spinner.SelectedItemPosition;
                    string culture_code = financeviewmodel.FinanceCulturesList[position].Content;
#endif
                    if (culture_code != financeviewmodel.FinanceCultureCode)
                    {
                        SmartData.CultureView cview = SmartSpikeV2017.Lookup_CULTURE_Info(ourviewmodel.cultureviewList,
                                                                                        culture_code);
                        // Time for a change
                        financeviewmodel.FinanceCultureCode = culture_code;
                        financeviewmodel.CultureINF = cview.CULTUREINFO;

                        // Adjust the drop down
                        if (financeviewmodel.LastCultureSelectedIndex >= 0)
                        {
                            financeviewmodel.FinanceCulturesList[financeviewmodel.LastCultureSelectedIndex].Colour =
                                                ourviewmodel.blackColour;
                            financeviewmodel.FinanceCulturesList[position].Colour = ourviewmodel.greenColour;
                            financeviewmodel.LastCultureSelectedIndex = position;
                        }
                        // Adjust it here, as it is an index into the transactions
                        if (!await SmartFinanceV2025.Finance_CultureChanged_Actual(
#if WINFORMS
                                                                                components,
#endif
#if ANDROIDX
                                                                                meterActivity,
#endif
                                                                                ourviewmodel,
                                                                                financeviewmodel))
                        {
                            // Do nothing for the time being ...
                            financeviewmodel.errorMessage = "Cannot change Finance culture";
                            return;
                        }
                    }
                }
            }
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            else
            {

                financeviewmodel.LastCultureSelectedIndex = spinner.SelectedIndex;

            }
#endif
            return;
        }

#if WINFORMS
        internal static async void RadioButtonRatesClicked(object sender, EventArgs e,
                                                    MainProcess components,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WPF
        internal static async void RadioButtonRatesClicked(object sender, RoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal static async void RadioButtonRatesClicked(object sender, RoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        internal async static void RadioButtonRatesClicked(object sender, EventArgs e,
                                                    AppCompatActivity meterActivity,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal static async void RadioButtonRatesClicked(object sender, CheckedChangedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif

        {
            RadioButton radiobutton = sender as RadioButton;

            string targetCurrency =
#if WINFORMS
                radiobutton.Text.ToString();
#endif
#if WPF  || WINUI || SMARTMAUI
            radiobutton.Content.ToString();
            financeviewmodel.WorkingCurrency = targetCurrency;
#endif
#if ANDROIDX
            radiobutton.Tag.ToString();
#endif
            if (targetCurrency != "")
            {
                SmartData.CultureView cview = SmartSpikeV2017.Lookup_Currency_OrdinalNew(ourviewmodel.cultureviewList,
                                                                        ourviewmodel.currenciesList,
                                                                        targetCurrency);
                if (cview.CURRENCY_ORDINAL >= 0)
                {
                    if (financeviewmodel.CurrencyOrdinal != cview.CURRENCY_ORDINAL)
                    {
                        financeviewmodel.CurrencyOrdinal = cview.CURRENCY_ORDINAL;
                        financeviewmodel.ConvertToSymbol = cview.ISOCURRENCYSYMBOL;
                        await SmartFinanceV2025.Finance_RateChanged_Actual(
#if WINFORMS
                                                            components,
#endif
#if ANDROIDX
                                                             meterActivity,
#endif
                                                            ourviewmodel,
                                                            financeviewmodel,
                                                            targetCurrency);


#if ANDROIDX
                        if (radiobutton != financeviewmodel.FinanceRBNON)
                        {
                            financeviewmodel.FinanceRBNON.SetTextColor(ourviewmodel.blackColour);
                        }
                        else
                        {
                            financeviewmodel.FinanceRBNON.SetTextColor(ourviewmodel.greenColour);
                        }
                        if (radiobutton != financeviewmodel.FinanceRBGBP)
                        {
                            financeviewmodel.FinanceRBGBP.SetTextColor(ourviewmodel.blackColour);
                        }
                        else
                        {
                            financeviewmodel.FinanceRBGBP.SetTextColor(ourviewmodel.greenColour);
                        }
                        if (radiobutton != financeviewmodel.FinanceRBEUR)
                        {
                            financeviewmodel.FinanceRBEUR.SetTextColor(ourviewmodel.blackColour);
                        }
                        else
                        {
                            financeviewmodel.FinanceRBEUR.SetTextColor(ourviewmodel.greenColour);
                        }
                        if (radiobutton != financeviewmodel.FinanceRBUSD)
                        {
                            financeviewmodel.FinanceRBUSD.SetTextColor(ourviewmodel.blackColour);
                        }
                        else
                        {
                            financeviewmodel.FinanceRBUSD.SetTextColor(ourviewmodel.greenColour);
                        }
                        if (radiobutton != financeviewmodel.FinanceRBJPY)
                        {
                            financeviewmodel.FinanceRBJPY.SetTextColor(ourviewmodel.blackColour);
                        }
                        else
                        {
                            financeviewmodel.FinanceRBJPY.SetTextColor(ourviewmodel.greenColour);
                        }
#endif
                    }
                }
#if WPF
                //e.Handled = true;
#endif
            }
            return;
        }

#if WINFORMS
        internal static async void Finance_TappedChanged(object sender, EventArgs e,
                                                        MainProcess components,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel)
#endif
#if WPF
        internal async void Finance_TappedChanged(object sender, MouseButtonEventArgs e,
                                                        MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal async void Finance_TappedChanged(object sender, RightTappedRoutedEventArgs e,
                                                        MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        internal static async void Finance_TappedChanged(object sender, LongClickEventArgs e,
                                                        AppCompatActivity meterActivity,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal async void Finance_TappedChanged(object sender, MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
        {
            // Even though CategoriesEnabled is FALSE here
            // (so - in theory - the button is DISABLED ...)
            // it STILL calls this routine!!!  What a pile
            // of shit this stuff is ....
            if (sender != null)
            {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                RadioButton radiobutton = sender as RadioButton;
#endif
#if ANDROIDX
                RadioButton radiobutton = sender as RadioButton;
#endif
#if WINFORMS
                // RadioButtonCheckedEvents(components, false); <= DON'T NEED THIS
                bool ischecked = (bool)radiobutton.Checked;
#endif
#if WPF || SMARTMAUI
                //RadioButtonCheckedEvents(this, false);
                bool ischecked = (bool)radiobutton.IsChecked;
#endif
#if ANDROIDX
                bool ischecked = (bool)radiobutton.Checked;
#endif
#if WPF || WINFORMS  || WINUI
                if (radiobutton.Tag != null)
#endif
#if SMARTMAUI
                if (radiobutton.Value != null)
#endif
                {
#if WINFORMS || WPF  || WINUI
                    char classid = Convert.ToChar(radiobutton.Tag.ToString());
#endif
#if ANDROIDX
                    char classid = Convert.ToChar(radiobutton.Tag.ToString());
#endif
#if SMARTMAUI
                    char classid = Convert.ToChar(radiobutton.Value.ToString());
#endif
                    foreach (SmartFinance.Categories category_row in financeviewmodel.PLO.finance_categoriesList)
                    {
                        if (classid == category_row.CATEGORY_CODE)
                        {
                            switch (classid)
                            {
                                case 'B':
#if WINFORMS
                                    if ((bool)components.RBBanks.Checked)
                                    {
                                        components.RBBanks.Checked = false;
                                        category_row.CHECKED = SmartParametersV2016.unChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes = financeviewmodel.categoryCodes.Replace(classid.ToString(), "");
                                    }
                                    else
                                    {
                                        components.RBBanks.Checked = true;
                                        category_row.CHECKED = SmartParametersV2016.lastChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes += classid.ToString();
                                    }
#endif
#if WPF  || WINUI || SMARTMAUI
                                    if ((bool)this.RBBanks.IsChecked)
                                    {
                                        this.RBBanks.IsChecked = false;
                                        category_row.CHECKED = SmartParametersV2016.unChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes = financeviewmodel.categoryCodes.Replace(classid.ToString(), "");
                                    }
                                    else
                                    {
                                        this.RBBanks.IsChecked = true;
                                        category_row.CHECKED = SmartParametersV2016.lastChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes += classid.ToString();
                                    }
#endif
#if ANDROIDX
                                    if ((bool)financeviewmodel.RBBanks.Checked)
                                    {
                                        financeviewmodel.RBBanks.Checked = false;
                                        category_row.CHECKED = SmartParametersV2016.unChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes = financeviewmodel.categoryCodes.Replace(classid.ToString(), "");
                                    }
                                    else
                                    {
                                        financeviewmodel.RBBanks.Checked = true;
                                        category_row.CHECKED = SmartParametersV2016.lastChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes += classid.ToString();
                                    }
#endif
                                    SmartFinanceV2025.FixCategoryTypes(financeviewmodel, category_row);
                                    break;
                                case 'S':
#if WINFORMS
                                    if ((bool)components.RBSavings.Checked)
                                    {
                                        components.RBSavings.Checked = false;
                                        category_row.CHECKED = SmartParametersV2016.unChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes = financeviewmodel.categoryCodes.Replace(classid.ToString(), "");
                                    }
                                    else
                                    {
                                        components.RBSavings.Checked = true;
                                        category_row.CHECKED = SmartParametersV2016.lastChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes += classid.ToString();
                                    }
#endif
#if WPF  || WINUI || SMARTMAUI
                                    if ((bool)this.RBSavings.IsChecked)
                                    {
                                        this.RBSavings.IsChecked = false;
                                        category_row.CHECKED = SmartParametersV2016.unChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes = financeviewmodel.categoryCodes.Replace(classid.ToString(), "");
                                    }
                                    else
                                    {
                                        this.RBSavings.IsChecked = true;
                                        category_row.CHECKED = SmartParametersV2016.lastChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes += classid.ToString();
                                    }
#endif
#if ANDROIDX
                                    if ((bool)financeviewmodel.RBSavings.Checked)
                                    {
                                        financeviewmodel.RBSavings.Checked = false;
                                        category_row.CHECKED = SmartParametersV2016.unChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes = financeviewmodel.categoryCodes.Replace(classid.ToString(), "");
                                    }
                                    else
                                    {
                                        financeviewmodel.RBSavings.Checked = true;
                                        category_row.CHECKED = SmartParametersV2016.lastChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes += classid.ToString();
                                    }
#endif
                                    SmartFinanceV2025.FixCategoryTypes(financeviewmodel, category_row);
                                    break;
                                case 'I':
#if WINFORMS
                                    if ((bool)components.RBInvestments.Checked)
                                    {
                                        components.RBInvestments.Checked = false;
                                        category_row.CHECKED = SmartParametersV2016.unChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes = financeviewmodel.categoryCodes.Replace(classid.ToString(), "");
                                    }
                                    else
                                    {
                                        components.RBInvestments.Checked = true;
                                        category_row.CHECKED = SmartParametersV2016.lastChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes += classid.ToString();
                                    }
#endif
#if WPF  || WINUI || SMARTMAUI
                                    if ((bool)this.RBInvestments.IsChecked)
                                    {
                                        this.RBInvestments.IsChecked = false;
                                        category_row.CHECKED = SmartParametersV2016.unChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes = financeviewmodel.categoryCodes.Replace(classid.ToString(), "");
                                    }
                                    else
                                    {
                                        this.RBInvestments.IsChecked = true;
                                        category_row.CHECKED = SmartParametersV2016.lastChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes += classid.ToString();
                                    }
#endif
#if ANDROIDX
                                    if ((bool)financeviewmodel.RBInvestments.Checked)
                                    {
                                        financeviewmodel.RBInvestments.Checked = false;
                                        category_row.CHECKED = SmartParametersV2016.unChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes = financeviewmodel.categoryCodes.Replace(classid.ToString(), "");
                                    }
                                    else
                                    {
                                        financeviewmodel.RBInvestments.Checked = true;
                                        category_row.CHECKED = SmartParametersV2016.lastChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes += classid.ToString();
                                    }
#endif
                                    SmartFinanceV2025.FixCategoryTypes(financeviewmodel, category_row);
                                    break;
                                case 'C':
#if WINFORMS
                                    if ((bool)components.RBCryptos.Checked)
                                    {
                                        components.RBCryptos.Checked = false;
                                        category_row.CHECKED = SmartParametersV2016.unChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes = financeviewmodel.categoryCodes.Replace(classid.ToString(), "");
                                    }
                                    else
                                    {
                                        components.RBCryptos.Checked = true;
                                        category_row.CHECKED = SmartParametersV2016.lastChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes += classid.ToString();
                                    }
#endif
#if WPF  || WINUI || SMARTMAUI
                                    if ((bool)this.RBCryptos.IsChecked)
                                    {
                                        this.RBCryptos.IsChecked = false;
                                        category_row.CHECKED = SmartParametersV2016.unChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes = financeviewmodel.categoryCodes.Replace(classid.ToString(), "");
                                    }
                                    else
                                    {
                                        this.RBCryptos.IsChecked = true;
                                        category_row.CHECKED = SmartParametersV2016.lastChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes += classid.ToString();
                                    }
#endif
#if ANDROIDX
                                    if ((bool)financeviewmodel.RBCryptos.Checked)
                                    {
                                        financeviewmodel.RBCryptos.Checked = false;
                                        category_row.CHECKED = SmartParametersV2016.unChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes = financeviewmodel.categoryCodes.Replace(classid.ToString(), "");
                                    }
                                    else
                                    {
                                        financeviewmodel.RBCryptos.Checked = true;
                                        category_row.CHECKED = SmartParametersV2016.lastChecked;
                                        category_row.Updated = true;
                                        financeviewmodel.categoryCodes += classid.ToString();
                                    }
#endif
                                    SmartFinanceV2025.FixCategoryTypes(financeviewmodel, category_row);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        }
                    }


                    // I'm going to keep this next test in, just in case I ever encounter that situation again
                    // where the fucking event handler decides to call itself TWICE (or more) as the chimps seem 
                    // to insist it has to ..

                    // Despite everything and all the pain
                    // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
                    //if (!(await SmartFinanceV2025.Finance_Switchover(ourviewmodel,
                    //                                                financeviewmodel,
                    //                                                false)))      // Reset all
                    //{
                    //    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                    //    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Finance Switchover");
                    //}
                    //else
                    //{
                    if (!await SmartFinanceV2025.Finance_RadioButtonChecked_Actual(
#if WINFORMS
                                                                                components,
#endif
#if ANDROIDX
                                                                                meterActivity,
#endif
                                                                                ourviewmodel,
                                                                                financeviewmodel))
                    {
                        GiveUp(ourviewmodel, financeviewmodel, "RadioButton switch failed");
                    }
                    else
                    {
#if WINFORMS
                        // RadioButtonCheckedEvents(components, true); |<= DON'T need this
#endif
#if ANDROIDX
                        //RadioButtonCheckedEvents(this, true);
#endif

                        SmartRoutinesV2018.FocusOpenClose(ourviewmodel);
                    }
                }
#if WPF
                // Not sure what effect THIS has
                // No WINUI
                e.Handled = true;
#endif
            }
            return;
        }

#if WINFORMS
        internal static async Task<bool> Finance_ButtonSubmitClick(object sender,
                                                                        EventArgs e,
                                                                        MainProcess components,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WPF  || WINUI
        internal async void Finance_ButtonSubmitClick(object sender, RoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        [RequiresUnreferencedCode("Calls SmartCubeMobile.SmartFinanceV2025.FinanceButtonSubmitClickActual(AppCompatActivity, MainViewModel, FinanceViewModel)")]
        internal static async void Finance_ButtonSubmitClick(object sender, EventArgs e,
                                                    AppCompatActivity meterActivity,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal async void Finance_ButtonSubmitClick(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
        {
#if WINFORMS
            bool status = true;
#endif



            // Are BOTH of these false? Yes? then carry on
            // Phase1 = false means loading phase finished
            // Phase 3 = true means Connect not in progress
            if (!ourviewmodel.Phase1 && ourviewmodel.Phase3)
            //if (sender != null && FrontEndGUI.DecodeEventFlags(e) != null)
            {
                // Are we in Normal or in the middle of a Cancel?
                if (FrontEndGUI.CompareLedColour(ourviewmodel, 6, ourviewmodel.orangeColour))
                {
                    // Turn us to a Submit. This blocks Connects
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.greenColour);

                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel, "Finance Submit chosen");

                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Finance Submit chosen"))
                    {
#if WINFORMS
                        return false;
#endif
#if WPF  || WINUI || SMARTMAUI

                        return;
#endif
                    }

                    // Set Phase 2 to say we are starting a scrape
                    // This will block a Phase 3 Connect starting whilst we are in the middle
                    financeviewmodel.taskList = new List<Task>();
                    if (!await SmartFinanceV2025.FinanceButtonSubmitClickActual(
#if WINFORMS
                                                        FinanceWebView,
                                                        components.textBoxConsole,
                                                        components,
#endif
#if SMARTMAUI
                                                        new WebView(),//this.FinanceWebView,
#endif
#if WPF  || WINUI
                                                        this.FinanceWebView,
#endif
#if ANDROIDX
                                                        FinanceWebView,
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                        financeviewmodel))
                    {
#if WINFORMS
                        status = false;
#endif
                        // Failure isn't a reason to crash ... there could
                        // be a MILLION and ONE network reasons why a Submit fails 
                        await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel, SmartParametersV2016.Finance.ToString() + " Finance Submit failed");

                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel,
                                                                financeviewmodel.financeToken,
                                                                0, 0, "Finance Submit failed"))
                        {
#if WINFORMS
                            return false;
#endif
#if WPF  || WINUI || SMARTMAUI

                            return;
#endif
                        }
                    }
                    else
                    {

                        //FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.whiteColour);

                        if (!await SmartFinanceScrapeV2023.PostProcessing(
#if WINFORMS
                                                                components,
#endif
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                
                                                                ourviewmodel,
                                                                financeviewmodel))
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                    meterActivity,
#endif
                                                                    ourviewmodel, SmartParametersV2016.Finance.ToString() + financeviewmodel.errorMessage);

                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel,
                                                                financeviewmodel.financeToken,
                                                                0, 0, financeviewmodel.errorMessage))
                            {
#if WINFORMS
                                return false;
#endif
#if WPF  || WINUI || SMARTMAUI

                                return;
#endif
                            }
                        }
                        else
                        {
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                    meterActivity,
#endif
                                                                    ourviewmodel,
                                                                    SmartParametersV2016.Finance.ToString() + " Finance Submit PostProcessing succeeded");

                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, SmartParametersV2016.Finance.ToString() + " Finance Submit PostProcessing succeeded"))
                            {
#if WINFORMS
                                return false;
#endif
#if WPF  || WINUI || SMARTMAUI

                                return;
#endif
                            }
                        }
                    }
                }
#if WPF
                // No WINUI
                // Not sure what effect THIS has (stops it bubbling up)
                e.Handled = true;
#endif
            }
#if WINFORMS
            return status;

#endif
            // This will allow any Phase 3 Connects to happen
#if WPF  || WINUI || ANDROIDX || SMARTMAUI
            return;
#endif
        }

#if WINFORMS || WPF
        internal static async void Finance_ButtonCancelClick(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if ANDROIDX
        internal async void Finance_ButtonCancelClick(object sender, EventArgs e,
                                                        AppCompatActivity meterActivity,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal async void Finance_ButtonCancelClick(object sender, RoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if SMARTMAUI
        internal static async void Finance_ButtonCancelClick(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif

#if WINFORMS
            WebBrowser Scraper = new WebBrowser();
#endif
            // Find the category given the screen
            if ((sender != null) && (e != null))
            {
                // Are we in the middle of a Submit?
                if (FrontEndGUI.CompareLedColour(ourviewmodel, 6, ourviewmodel.greenColour))
                {
                    // Turn us back to Normal. This allows Connects
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.orangeColour);

                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                            meterActivity,
#endif
                                                            ourviewmodel, "Finance Cancel chosen");

                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Finance Cancel chosen"))
                    {
                        return;
                    }


                    // Not sure what this does but its now in Actual routine
                    //if (financeviewmodel.financeToken != CancellationToken.None)
                    //{
                    //    SmartFinanceV2025.Fix_financeToken(financeviewmodel);
                    //}

                    if (!await SmartFinanceV2025.FinanceButtonCancelClickActual(
#if ANDROIDX
                                                                                meterActivity,
#endif
                                                                                ourviewmodel,
                                                                                financeviewmodel))
                    {
                        await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel, "Cancel failed");

                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Cancel failed"))
                        {
                            return;
                        }
                    }
                }
#if WPF  || WINUI
                // Not sure what effect THIS has
                // No e.Handled = true;
#endif
            }
            return;
        }

#if WINFORMS
        internal void MouseEnterInstitutions(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WPF
        internal static void MouseEnterInstitutions(object sender, RoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal void MouseEnterInstitutions(object sender, PointerRoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        internal void MouseEnterInstitutions(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal static void MouseEnterInstitutions(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif

        {
            if (sender != null && e != null)
            {
                SmartFinanceV2025.MouseEnterInstitutionsFinance_Actual(ourviewmodel, financeviewmodel);

#if WPF         // No WINUI
                // Not sure what effect THIS has
                e.Handled = true;
#endif
            }
            return;
        }

#if WINFORMS
        internal void MouseLeaveInstitutions(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WPF
        internal static void MouseLeaveInstitutions(object sender, RoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal void MouseLeaveInstitutions(object sender, PointerRoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        internal void MouseLeaveInstitutions(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal static void MouseLeaveInstitutions(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif

        {
            if (sender != null && e != null)
            {
                SmartFinanceV2025.MouseLeaveInstitutionsFinance_Actual(ourviewmodel, financeviewmodel);
#if WPF         // No WINUI
                // Not sure what effect THIS has
                e.Handled = true;
#endif
            }
            return;
        }

#if WINFORMS || WPF
        internal static void TransactionsSelectionChanged(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal void TransactionsSelectionChanged(object sender, SelectionChangedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if ANDROIDX
        internal void TransactionsSelectionChanged(object sender, EventArgs e,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel)
        {
#endif
#if SMARTMAUI
        internal static void TransactionsSelectionChanged(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if WINFORMS
            //ListView datagridtransactions = (ListView)sender;
#endif
#if WPF || SMARTMAUI
            if (financeviewmodel.childWindow != null)
            {
#if WPF
                if (financeviewmodel.childWindow.IsVisible)
                {
                    financeviewmodel.childWindow.Close();
                }
#endif
#if WPF || SMARTMAUI
                financeviewmodel.childWindow.Close();
#endif
            }
#endif
#if WPF  || WINUI || SMARTMAUI
            //financeviewmodel.FinanceTransactionsPosition = (int)datagridtransactions.SelectedItem;    //This Will fail
#endif
#if ANDROIDX
                //financeviewmodel.FinanceTransactionsPosition = (int)datagridtransactions.SelectedItem;    //This Will fail
#endif
            // Not sure what effect THIS has
#if WPF
            //e.Handled = true;
#endif
            return;
        }

        // This swops the tabs
#if WINFORMS
        //        internal void FinanceTransactionsTabMouseDoubleClickFinance(object sender, EventArgs e)
        //        {
        //            if (sender != null && e != null)
        //            {
        //                if (1 == 1)
        //                {
        //                    Label tablabel = (Label)sender;
        //                    if (tablabel != null)
        //                    {
        //                        SmartFinanceV2025.Finance_TabMouseDoubleClick_Actual(tablabel,
        //                                                                    ourviewmodel,
        //                                                                    financeviewmodel);

        //                    }
        //                }

        //                // Not sure what effect THIS has
        //                //e.Handled = true;
        //            }
        //            return;
        //        }
#endif

#if WINFORMS
        // When you choose a Tab Label (Completely untested!!)
        internal static async void FinanceTransactionsTabMouseDoubleClick(object sender, EventArgs e,
                                                        int width,
                                                        int height,
                                                        DataGridView FinanceTransactionsGrid,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WPF  || WINUI
        // When you choose a Tab Label
        internal async void FinanceTransactionsTabMouseDoubleClick(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        // When you choose a Tab Label
        internal async void FinanceTransactionsTabMouseDoubleClick(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        // When you choose a Tab Label
        internal async void FinanceTransactionsTabMouseDoubleClick(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
        {
            if (sender != null &&
#if WINFORMS
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WPF
                FrontEndGUI.DecodeMouseButtonEventFlags(e) != null)
#endif
#if WINUI
                FrontEndGUI.DecodeDoubleTappedRoutedEventFlags(e) != null)
#endif
#if ANDROIDX
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if SMARTMAUI
                true) // Not sure how to handle this for Maui ....
#endif
            {
                string tabLabel = FrontEndGUI.DecodeTabLabel(sender);
                if (tabLabel != null)
                {
#if WINFORMS
                    //List<DataGridViewColumn> popup_columns = new List<DataGridViewColumn>();
#endif
#if WPF  || WINUI || SMARTMAUI
                    //string label = tablabel.DataContext.ToString();
                    //List<DataGridTextColumn> popup_columns = new List<DataGridTextColumn>();
#endif
                    //ListView popup_columns = new ListView();

                    if (financeviewmodel.FinanceTransactions.Count > 0)
                    {
#if WINFORMS
                        await SmartFinanceV2025.Finance_TabMouseDoubleClick_Actual(tabLabel,
                                                                ourviewmodel,
                                                                financeviewmodel,
                                                                width,
                                                                height);
                        //popup_columns);
#endif
#if WPF  || WINUI || SMARTMAUI
                        await SmartFinanceV2025.Finance_TabMouseDoubleClick_Actual(tabLabel,
                                                                ourviewmodel,
                                                                financeviewmodel,
                                                                this);
#endif
#if ANDROIDX
                        await SmartFinanceV2025.Finance_TabMouseDoubleClick_Actual(this,
                                                                   tabLabel,
                                                                   ourviewmodel,
                                                                   financeviewmodel);
#endif
                    }
                }
#if WPF
                // No WINUI
                // Not sure what effect THIS has
                FrontEndGUI.DecodeMouseButtonEventFlags(e).Handled = true;
#endif
            }
            return;
        }

#if WINFORMS || WPF  || WINUI
        // When you choose a Tab Label
        internal async void FinanceCryptoTabItemsTabMouseDoubleClick(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        // When you choose a Tab Label
        internal async void FinanceCryptoTabItemsTabMouseDoubleClick(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        // When you choose a Tab Label
        internal async void FinanceCryptoTabItemsTabMouseDoubleClick(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
        {
            if (sender != null &&
#if WINFORMS
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WPF
                FrontEndGUI.DecodeMouseButtonEventFlags(e) != null)
#endif
#if WINUI
                FrontEndGUI.DecodeDoubleTappedRoutedEventFlags(e) != null)
#endif
#if ANDROIDX
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if SMARTMAUI
                true) // Not sure how to handle this for Maui =:-(
#endif
            {
                string tabLabel = FrontEndGUI.DecodeTabLabel(sender);
                if (tabLabel != null)
                {
#if WINFORMS
                    //List<DataGridViewColumn> popup_columns = new List<DataGridViewColumn>();
#endif
#if WPF  || WINUI || SMARTMAUI
                    //string label = tablabel.DataContext.ToString();
                    //List<DataGridTextColumn> popup_columns = new List<DataGridTextColumn>();
#endif
                    //ListView popup_columns = new ListView();

                    if (financeviewmodel.FinanceCryptoAddresses.Count > 0)
                    {
#if WINFORMS
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel,
                                                        "Tab Mouse DoubleClick");
                        //SmartFinanceV2025.FinanceCryptoAddresses_TabMouseDoubleClick_Actual(tabLabel,
                        //                                        ourviewmodel,
                        //                                        financeviewmodel);

                        //popup_columns);
#endif
#if WPF  || WINUI || SMARTMAUI
                        await SmartFinanceV2025.FinanceCryptoTabItems_TabMouseDoubleClick_Actual(tabLabel,
                                                                ourviewmodel,
                                                                financeviewmodel,
                                                                this);
#endif
#if ANDROIDX
                        await SmartFinanceV2025.FinanceCryptoTabItems_TabMouseDoubleClick_Actual(this,
                                                                   tabLabel,
                                                                   ourviewmodel,
                                                                   financeviewmodel);
#endif
                    }
                }
#if WPF
                // No WINUI
                // Not sure what effect THIS has
                FrontEndGUI.DecodeMouseButtonEventFlags(e).Handled = true;
#endif
            }
            return;
        }

        // This drills down on a transaction cell
#if WINFORMS
        // Its ok to use async void on Event Handlers
        internal void FinanceTransactionsMouseDoubleClick(object sender, EventArgs e,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel)
        {
            if (sender != null && e != null)
            {
                DataGridView datagrid = (DataGridView)sender;
                if (datagrid.CurrentCell != null)
                {

                    // Find the category given the screen
                    SmartFinanceV2025.Finance_TransactionsMouseDoubleClick_Actual(datagrid,
                                                                                ourviewmodel,
                                                                                financeviewmodel);
                }

                // Not sure what effect THIS has
                //e.Handled = true;
            }
            return;
        }
#endif
#if WPF  || WINUI || SMARTMAUI
#if WPF
        internal void FinanceTransactionsMouseDoubleClick(object sender, MouseButtonEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal void FinanceTransactionsMouseDoubleClick(object sender, DoubleTappedRoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal void FinanceTransactionsMouseDoubleClick(object sender, TappedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif

        {
#if WPF || SMARTMAUI
            if (sender != null)// && e.RightButton.HasFlag)//  ClickCount == 1)
            {
#if WPF
                ListView viewlist = (ListView)sender;
#endif
#if SMARTMAUI

                ListView viewlist = (ListView)sender;
#endif

#endif
#if WINUI
            if (sender != null) // && e.ClickCount == 1)
            {
                ListView viewlist = (ListView)sender;
#endif

                if (viewlist.SelectedItem != null)
                {

                    // Find the category given the screen
                    SmartFinanceV2025.Finance_TransactionsMouseDoubleClick_Actual(viewlist,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                this);
                }
#if WPF
                // Not sure what effect THIS has
                e.Handled = true;
#endif
            }
            return;
        }
#endif

#if ANDROIDX
        internal async void FinanceTransactionsMouseDoubleClick(object sender, EventArgs e,
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel)

        {
            if (sender != null && e != null)
            {
                Android.Widget.ListView viewlist = (Android.Widget.ListView)sender;
                if (viewlist.SelectedItem != null)
                {

                    // Find the category given the screen
                    try
                    {
                        SmartFinanceV2025.Finance_TransactionsMouseDoubleClick_Actual(this,
                                                                                    viewlist,
                                                                                    ourviewmodel,
                                                                                    financeviewmodel);
                        await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Transactions Mouse DoubleClick");
                    }
                    catch (Exception)
                    {
                        FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    }
                }
            }
            return;
        }
#endif


        // For reasons which entirely elude me ... these Microshit chimpanzees have
        // decided to fire this event whenever I switch screens/UserControls etc. using
        // SwitchView.  Nothing is documented, nothing is explained and I can't trace where
        // this event is being fired from - I can only assume its the Microshit monkeys
        // and their usual, unrelenting, unremitting bollocks at work ...
        // Once I get the Android version of this working, I will dump those cunts in an INSTANT

        // For reasons which escape me but the Microshit chimpanzees will guess at,
        // this event is called even when there is nothing in the drop-down
#if WINFORMS
        internal static void FinanceInstitutions_SelectionChanged(object sender, MouseEventArgs e,
                                                    MainProcess process_components,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)

#endif
#if WPF  || WINUI
        internal void FinanceInstitutions_SelectionChanged(object sender, SelectionChangedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        
#endif
#if ANDROIDX
        internal void FinanceInstitutions_SelectionChanged(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        
#endif
#if SMARTMAUI
        internal void FinanceInstitutions_SelectionChanged(object sender, SelectionChangedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        
#endif

        {
            if (sender != null && e != null)
            {
                //bool status = false;
#if WINFORMS || WPF
                ComboBox institution = (ComboBox)sender;
                // Dont fink this is needed? It gets set automatically??
                //financeviewmodel.InstitutionSelectedIndex = institution.SelectedIndex;
#endif
#if SMARTMAUI
                Picker institution = (Picker)sender;
                // Dont fink this is needed? It gets set automatically??
                //financeviewmodel.InstitutionSelectedIndex = institution.SelectedIndex;
#endif
                if (!financeviewmodel.IDDisInitializing &&
                    financeviewmodel.InstitutionSelectedIndex != -1)
                {
                    int selected_index = financeviewmodel.InstitutionSelectedIndex;
                    // I don't believe it... this bollocks is ACTUALLY WORKING
                    FinanceViewModel.InstitutionItem institution_item = financeviewmodel.FinanceInstitutionsList[selected_index];
                    // Popup Connection Changed screen for WINFORMS                    
                    SmartFinanceV2025.Finance_InstitutionsChanged_Actual(
#if ANDROIDX
                                        sender as Spinner,
#endif
#if WINFORMS
                                    process_components,
#endif
#if WPF || WINUI  || SMARTMAUI
                                    this,
#endif
                                    ourviewmodel,
                                    financeviewmodel,
                                    institution_item);
                }
#if WPF
                // No WINUI
                // Not sure what effect THIS has
                e.Handled = true;
#endif
            }
            return;
        }

#if ANDROIDX
        internal static void FinanceInstitutions_SelectionChanged(object sender, AdapterView.ItemSelectedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
            if (sender != null && 
                e != null)
            {
                if (financeviewmodel.InstitutionsEnabled)
                {
                    Spinner InstitutionsSpinner = sender as Spinner;
                    if (financeviewmodel.IDDisInitializing)                        
                    {
                        financeviewmodel.IDDisInitializing = false;
                        return; // Ignore any of these conditions
                    }
                    int position = e.Position;
                    FinanceViewModel.InstitutionItem institution_item = financeviewmodel.FinanceInstitutionsList[position];
                    //if (financeviewmodel.FIPConnectionPage.IsShowing)
                    //{
                    //    financeviewmodel.FIPConnectionPage.Dismiss();
                    //}
                    IViewParent parent = InstitutionsSpinner.Parent;
                    SmartFinanceV2025.Finance_InstitutionsChanged_Actual(InstitutionsSpinner,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                institution_item);
                }
            }
            return;
        }
#endif

        //#if WPF  || WINUI
        //        internal void FinanceInstitutions_DropDownClosed(object sender, EventArgs e,
        //                                                FinanceViewModel financeviewmodel)
        //        {
        //            //if (sender != null &&
        //            //    e != null)
        //            //{
        //            //    financeviewmodel.IDDdropDownClicked = true;
        //            //    ComboBox InstitutionsComboBox = sender as ComboBox;
        //            //    // Reset selection temporarily to allow re-selection of the same item
        //            //    int selectedIndex = InstitutionsComboBox.SelectedIndex;
        //            //    // Clear selection momentarily to force SelectionChanged event to trigger
        //            //    InstitutionsComboBox.SelectedIndex = -1;
        //            //    // Restore the previous selection again forcing SelectionChanged event to trigger
        //            //    InstitutionsComboBox.SelectedIndex = selectedIndex;
        //            //}
        //            return;
        //        }
        //#endif



        // For reasons which escape me but the Microshit chimpanzees will guess at,
        // this event is called even when there is nothing in the drop-down
#if WINFORMS
        internal static void FinanceProviders_SelectionChanged(object sender, MouseEventArgs e,
                                                    MainProcess process_components,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif

#if WPF
        internal void FinanceMouseRightButtonDown(object sender, RoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal void FinanceMouseRightButtonDown(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        internal static void FinanceMouseRightButtonDown(object sender, View.TouchEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal async void FinanceMouseRightButtonDown(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
#endif

        {
#if ANDROIDX
            if (e.Event.ButtonState.HasFlag(MotionEventButtonState.Secondary))
            {
                // Right mouse button was clicked
                Console.WriteLine("Right click detected on Spinner");
                e.Handled = true;
            }
#endif

            if (sender != null &&
#if WINFORMS
                 FrontEndGUI.DecodeMouseEventFlags(e) != null)
#endif
#if WPF
                FrontEndGUI.DecodeRoutedEventFlags(e) != null)
#endif
#if SMARTMAUI
                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
#if WINUI
                FrontEndGUI.DecodeRightTappedRoutedEventFlags(e) != null)
#endif
#if ANDROIDX

                FrontEndGUI.DecodeEventFlags(e) != null)
#endif
            {
                FinanceViewModel.InstitutionItem institution_item = new FinanceViewModel.InstitutionItem();
#if ANDROIDX
                if (sender is Spinner spinner)
                {
                    int position = spinner.SelectedItemPosition;

                    if (position >= 0 &&
                        position < financeviewmodel.FinanceInstitutions.Count)
                    {
                        institution_item =
                            financeviewmodel.FinanceInstitutionsList[position];

                        Console.WriteLine(institution_item.Content);
                    }
                }
#endif
                //FinanceViewModel.InstitutionItem institution_item = sp.SelectedItem;// FrontEndGUI.DecodeInstitutionItem(sender);
#if WINFORMS
                SmartFinanceV2025.Finance_Institution_Selected_Actual(ourviewmodel,
                                                                    financeviewmodel,
                                                                    institution_item.Content);
#endif
#if WPF
                SmartFinanceV2025.Finance_Institution_Selected_Actual(ourviewmodel,
                                                                    financeviewmodel,
                                                                    this,
                                                                    institution_item.Content);
#endif
#if WINUI
                SmartFinanceV2025.Finance_Institution_Selected_Actual(ourviewmodel,
                                                                    financeviewmodel,
                                                                    institution_item.Content);
#endif
#if ANDROIDX
                SmartFinanceV2025.Finance_Institution_Selected_Actual(ourviewmodel,
                                                               financeviewmodel,
                                                               institution_item.Value);
#endif
#if SMARTMAUI
                await SmartFinanceV2025.Finance_Institution_Selected_Actual(ourviewmodel,
                                                                    financeviewmodel,
                                                                    this,
                                                                    institution_item.Content);
#endif

#if WPF
                // Not sure what effect THIS has (stops it bubbling up)
                e.Handled = true;
#endif
            }
            return;
        }

        //#if WINFORMS
        //        internal static void Finance_InstitutionMouseDoubleClick(object sender, MouseEventArgs e, MainProcess process_components,
        //                                                    MainViewModel ourviewmodel,
        //                                                    FinanceViewModel financeviewmodel)
        //        {
        //#endif
        //#if WPF
        //        internal async void Finance_InstitutionMouseDoubleClick(object sender, MouseEventArgs e,
        //                                                    MainViewModel ourviewmodel,
        //                                                    FinanceViewModel financeviewmodel)
        //        {
        //#endif
        //#if WINUI
        //        internal async void Finance_InstitutionMouseDoubleClick(object sender, DoubleTappedRoutedEventArgs e,
        //                                                    MainViewModel ourviewmodel,
        //                                                    FinanceViewModel financeviewmodel)
        //        {
        //#endif
        //#if ANDROIDX
        //        //internal async Task<bool> Finance_InstitutionMouseDoubleClick(object sender, EventArgs e,
        //        //                                            MainViewModel ourviewmodel,
        //        //                                            FinanceViewModel financeviewmodel)
        //        //{
        //#endif
        //#if WPF  || WINUI
        //            if (sender != null && e != null)
        //            {
        //                if (financeviewmodel.InstitutionsEnabled)
        //                {
        //                    ComboBox InstitutionsComboBox = sender as ComboBox;
        //                    if (financeviewmodel.IDDisInitializing ||
        //                        InstitutionsComboBox.SelectedIndex == -1 ||
        //                        !financeviewmodel.IDDdropDownClicked)
        //                    {
        //                        return; // Ignore any of these conditions
        //                    }
        //                    if (financeviewmodel.FCLPopup != null)
        //                    {
        //#if WPF
        //                        if (financeviewmodel.FCLPopup.IsOpen)
        //                        {
        //                            financeviewmodel.FCLPopup.IsOpen = false;
        //                        }
        //#endif
        //                    }
        //#if WPF  || WINUI
        //                    // This ISN'T perfect ... but I can live with it
        //                    // for the time being
        //                    // Don't forget to CLOSE the Popup IF you just 
        //                    // decide to switch screens OR closedown
        //                    //
        //#if WPF
        //                    Point position = InstitutionsComboBox.PointToScreen(new Point(0, 0));

        //#endif
        //#if WINUI 
        //                    GeneralTransform transform = InstitutionsComboBox.TransformToVisual(null); // Null means relative to the window's root
        //                    Point position = transform.TransformPoint(new Windows.Foundation.Point(0, 0));

        //#endif
        //                    //// Set the popup's offsets
        //                    financeviewmodel.HorizontalOffset = position.X;
        //                    financeviewmodel.VerticalOffset = position.Y + (InstitutionsComboBox.RenderSize.Height); // Adjust for element's height
        //#endif

        //                    FinanceViewModel.InstitutionItem institution_item = InstitutionsComboBox.SelectedItem as FinanceViewModel.InstitutionItem;
        //#if WINFORMS || WPF  || WINUI
        //                    await SmartFinanceV2025.Finance_InstitutionsPopup_Actual(
        //#if WINFORMS
        //                                                                                    process_components,
        //#endif
        //#if WPF  || WINUI
        //                                                                                this,
        //#endif
        //                                                                                ourviewmodel,
        //                                                                                financeviewmodel,
        //                                                                                institution_item);
        //#endif
        //#if WPF
        //                    // This is needed because the Chimps insist on raising
        //                    // the SelectionChanged event BEFORE the DropDownClosed event
        //                    financeviewmodel.IDDdropDownClicked = false;
        //                    // Not sure what effect THIS has
        //                    e.Handled = true;
        //#endif

        //                }

        //            }
        //            return;
        //        }
        //#endif
        //#if WINFORMS
        //        }
        //#endif


#if WINFORMS
        internal static async void Finance_InstitutionMouseDoubleClick(object sender, MouseEventArgs e, MainProcess process_components,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if WPF
        internal async void Finance_InstitutionMouseDoubleClick(object sender, MouseEventArgs e,
                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal async void Finance_InstitutionMouseDoubleClick(object sender, PointerRoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if ANDROIDX
        internal static async void Finance_InstitutionMouseDoubleClick(object sender, EventArgs e,

                                                    AppCompatActivity meterActivity, 
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if SMARTMAUI
        internal async void Finance_InstitutionMouseDoubleClick(object sender, TappedEventArgs e,
                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif

            if (sender != null && e != null)
            {
                if (financeviewmodel.InstitutionsEnabled)
                {
#if WPF || SMARTMAUI
#if WPF
                    ComboBox InstitutionsComboBox = sender as ComboBox;
#endif
#if SMARTMAUI
                    Picker InstitutionsComboBox = sender as Picker;
#endif
                    if (financeviewmodel.IDDisInitializing ||
                        InstitutionsComboBox.SelectedIndex == -1)// ||
                        //!financeviewmodel.IDDdropDownClicked)
                    {
                        return; // Ignore any of these conditions
                    }
                    if (financeviewmodel.FCLWindow != null)
                    {
#if WPF  || WINUI
                        if (financeviewmodel.FCLWindow.IsLoaded)
                        {
                            Console.WriteLine("already open");
                            //financeviewmodel.FCLWindow.IsOpen = false;
                        }
#endif
                    }
#endif
#if WPF  || WINUI || SMARTMAUI
                        // This ISN'T perfect ... but I can live with it
                        // for the time being
                        // Don't forget to CLOSE the Popup IF you just 
                        // decide to switch screens OR closedown
                        //
#if WPF
                    System.Windows.Point position = InstitutionsComboBox.PointToScreen(new System.Windows.Point(0, 0));
                    
#endif
#if WINUI 
                    GeneralTransform transform = FinanceInstitutions.TransformToVisual(null); // Null means relative to the window's root
                    Point position = transform.TransformPoint(new Windows.Foundation.Point(0, 0));
#endif
                        FinanceViewModel.InstitutionItem institution_item = FinanceInstitutions.SelectedItem as FinanceViewModel.InstitutionItem;
                    //// Set the popup's offsets
#if WPF || WINUI 
                    financeviewmodel.HorizontalOffset = position.X;
                    financeviewmodel.VerticalOffset = position.Y + (FinanceInstitutions.RenderSize.Height); // Adjust for element's height
#endif
                    institution_item = FinanceInstitutions.SelectedItem as FinanceViewModel.InstitutionItem;
#endif
#if ANDROIDX || WINFORMS
#if ANDROIDX
                    Spinner InstitutionsComboBox = sender as Spinner;
#endif
                    FinanceViewModel.InstitutionItem institution_item = new FinanceViewModel.InstitutionItem();
                    institution_item.Value = "1004";
                    institution_item.Content = "1004";
#endif
                    await SmartFinanceV2025.Finance_InstitutionsPopup_Actual(
#if WINFORMS
                                                                                process_components,
#endif
#if WPF  || WINUI || SMARTMAUI
                                                                                this,
#endif
#if ANDROIDX
                                                                                meterActivity,
#endif
                                                                                signinviewmodel,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                institution_item);
#if WPF
                    // This is needed because the Chimps insist on raising
                    // the SelectionChanged event BEFORE the DropDownClosed event
                    financeviewmodel.IDDdropDownClicked = false;
                    // Not sure what effect THIS has
                    e.Handled = true;
#endif
                }
            }
            return;
        }

#if WINFORMS
        internal async void Finance_ProvidersMouseDoubleClick(object sender, MouseEventArgs e, MainProcess process_components,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if WPF
        internal async void Finance_ProvidersMouseDoubleClick(object sender, MouseEventArgs e,
                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal async void Finance_ProvidersMouseDoubleClick(object sender, PointerRoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if ANDROIDX
        internal static async void Finance_ProvidersMouseDoubleClick(object sender, EventArgs e,

                                                    AppCompatActivity meterActivity, 
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if SMARTMAUI
        internal async void Finance_ProvidersMouseDoubleClick(object sender, TappedEventArgs e,
                                                    SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
            if (sender != null && e != null)
            {
                if (financeviewmodel.ProvidersEnabled)
                {
#if WPF
                    ComboBox ProvidersComboBox = sender as ComboBox;
#endif
#if ANDROIDX
                    MultiSpinner ProvidersComboBox = sender as MultiSpinner;
#endif
#if SMARTMAUI
                    Picker ProvidersComboBox = sender as Picker;
#endif
#if !(ANDROIDX || WINFORMS)
                    if (financeviewmodel.IDDisInitializing)// ||
                        //FinanceProviders.SelectedIndex == -1)// ||
                                                                //!financeviewmodel.IDDdropDownClicked)
                    {
                        return; // Ignore any of these conditions
                    }
#endif
#if !(ANDROIDX || WINFORMS)
                    if (financeviewmodel.FCLWindow != null)
                    {
#if WPF  || WINUI
                        if (financeviewmodel.FCLWindow.IsLoaded)
                        {
                            Console.WriteLine("already open");
                            //financeviewmodel.FCLWindow.IsOpen = false;
                        }
#endif
                    }
#endif

#if WPF  || WINUI || SMARTMAUI
                    // This ISN'T perfect ... but I can live with it
                    // for the time being
                    // Don't forget to CLOSE the Popup IF you just 
                    // decide to switch screens OR closedown
                    //
#if WPF
                    System.Windows.Point position = ProvidersComboBox.PointToScreen(new System.Windows.Point(0, 0));

#endif
#if WINUI 
                    GeneralTransform transform = FinanceProviders.TransformToVisual(null); // Null means relative to the window's root
                    Point position = transform.TransformPoint(new Windows.Foundation.Point(0, 0));
#endif
                    FinanceViewModel.ProviderItem provider_item = new FinanceViewModel.ProviderItem();//FinanceProviders.SelectedItem as FinanceViewModel.ProviderItem;
                    //// Set the popup's offsets
#if WPF || WINUI 
                    financeviewmodel.HorizontalOffset = position.X;
                    financeviewmodel.VerticalOffset = position.Y + (FinanceProviders.RenderSize.Height); // Adjust for element's height
#endif
                    provider_item = new FinanceViewModel.ProviderItem();// FinanceProviders.SelectedItem as FinanceViewModel.ProviderItem;
#endif
#if ANDROIDX || WINFORMS
                    FinanceViewModel.ProviderItem provider_item = new FinanceViewModel.ProviderItem();
                    provider_item.Value = "1004";
                    provider_item.Content = "1004";
#endif

                    await SmartFinanceV2025.Finance_ProvidersPopup_Actual(
#if WINFORMS
                                                                                    process_components,
#endif
#if WPF  || WINUI || SMARTMAUI
                                                                                this,
#endif
#if ANDROIDX
                                                                                meterActivity,
#endif
                                                                                signinviewmodel,
                                                                                ourviewmodel,
                                                                                financeviewmodel,
                                                                                provider_item);
#if WPF
                    // This is needed because the Chimps insist on raising
                    // the SelectionChanged event BEFORE the DropDownClosed event
                    financeviewmodel.IDDdropDownClicked = false;
                    // Not sure what effect THIS has
                    e.Handled = true;
#endif
                }

            }
            return;
        }

        //        // Here
#if WINFORMS
        //        private void ExchangeRatesClicked(object sender, EventArgs e)
        //        {
#endif
#if WPF
        //        internal void ExchangeRatesClicked(object sender, RoutedEventArgs e)
        //        {
#endif
#if WINUI
        //        internal async void ExchangeRatesClicked(object sender, RoutedEventArgs e)
        //        {
#endif
#if ANDROIDX
        //        internal void ExchangeRatesClicked(object sender, EventArgs e)
        //        {
#endif
#if SMARTMAUI
        //        internal void ExchangeRatesClicked(object sender, RoutedEventArgs e)
        //        {
#endif
        //            // Its ok to use async void on Event Handlers
#if WINFORMS || WPF || SMARTMAUI
        //            //            internal async void Utility_ButtonExchangeRatesClick(object sender, EventArgs e)
#endif
#if WINUI
        //            //        internal void Utility_ButtonExchangeRatesClick(object sender, RoutedEventArgs e)
#endif

        //            // Make sure passed parameters are valid
        //            if ((sender != null) &&
        //            (e != null))
        //            {
        //                // WTF IS THIS????!!!!???
        //                DateTime target_date = SmartTimeV2016.ConvertDateTime("2022-01-01");

        //                ourviewmodel.ExchangeRatesReduced = SmartSpikeV2017.ExchangeRates_Reduced(ourviewmodel,
        //                                                                target_date);
        //                if (ourviewmodel.ExchangeRatesReduced.Count > 0)
        //                {
#if WINFORMS || WPF || SMARTMAUI
        //                    if (!SmartFinanceV2025.Finance_ButtonExchangeRatesClick_Actual())
#endif
#if WINUI
        //                    if (!await SmartFinanceV2025.Finance_ButtonExchangeRatesClick_Actual(
#endif
#if ANDROIDX
        //                    if (!SmartFinanceV2025.Finance_ButtonExchangeRatesClick_Actual(
#endif
#if WINUI
        //                                                                (ContentDialog)this.Parent,
#endif
#if ANDROIDX
        //                                                                this,
        //                                                                ourviewmodel,
        //                                                                financeviewmodel))
#endif
#if WINUI
        //                                                                ourviewmodel,
        //                                                                financeviewmodel))
#endif
        //                    {
        //                        if (ourviewmodel.trace)
        //                        {
        //                            GiveUp("ExchangeRates popup failed");
        //                        }
        //                    }
        //                }
#if WPF
        //                // Not sure what effect THIS has
        //                e.Handled = true;
#endif
        //            }
        //            return;
        //        }


#if WPF
        internal async void OnKeyUp(object sender, KeyEventArgs e,
                                    SignInViewModel signinviewmodel,
                                    MainViewModel ourviewmodel)
        {
            if (e.Key == Key.Right)
            {
                // Go right
                await SmartRoutinesV2018.ButtonRightClickedActual(signinviewmodel, ourviewmodel);
            }
            else
            {
                if (e.Key == Key.Left)
                {
                    // Go Left
                    await SmartRoutinesV2018.ButtonLeftClickedActual(signinviewmodel, ourviewmodel);
                }
            }
            return;
        }
#endif

#if WINUI
        // https://stackoverflow.com/questions/46962632/use-swipe-gesture-in-WINUI
        // https://stackoverflow.com/questions/45550684/horizontal-swipe-gesture-on-WINUI
        // Thanks to Justin XL 

        private void SwipeableTextBlock_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (sender != null && e != null)
            {
                if (e.IsInertial)
                {
                    var swipedDistance = e.Cumulative.Translation.X;

                    if (Math.Abs(swipedDistance) <= 2) return;

                    if (swipedDistance > 0)
                    {
                        SmartRoutinesV2018.OnSwipedRightActual(sender, e, signinviewmodel, ourviewmodel);
                    }
                    else
                    {
                        SmartRoutinesV2018.OnSwipedLeftActual(sender, e, signinviewmodel, ourviewmodel);
                    }
                }
            }
            return;
        }

        private void SwipeableTextBlock_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (sender != null && e != null)
            {
                // Left in for future compatibility
            }
            return;
        }
#endif
#if SMARTMAUI
        private async void OnRightKey(object sender, EventArgs e)
        {
            await SmartRoutinesV2018.ButtonRightClickedActual(signinviewmodel, ourviewmodel);
        }

        private async void OnLeftKey(object sender, EventArgs e)
        {
            await SmartRoutinesV2018.ButtonLeftClickedActual(signinviewmodel, ourviewmodel);
        }
#endif
#if WINFORMS
        internal static void Chart_MouseLeave(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if WPF
        internal static void Chart_MouseLeave(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal static void Chart_MouseLeave(object sender, PointerRoutedEventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)

        {
#endif
#if ANDROIDX
        internal static void Chart_MouseLeave(object sender, EventArgs e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)

        {
#endif
#if SMARTMAUI
        internal static void Chart_MouseLeave(object sender, object e,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#endif
            if (sender != null &&
                FrontEndGUI.DecodeMouseEventFlags(e) != null)
            {
                // Remove the popup from the screen
#if WPF || SMARTMAUI
                financeviewmodel.childWindow.Close();
#endif
#if WINUI
                financeviewmodel.childWindow.IsOpen = false;
#endif
                // The chart is re-instated after in Chart_WindowClosed
#if WPF
                // No WINUI
                FrontEndGUI.DecodeMouseEventFlags(e).Handled = true;
#endif
            }
            return;
        }

#if WPF // || WINUI
        internal static void Chart_MouseDoubleClick(object sender, object e,
                                                    FinanceViewModel financeviewmodel)
        {
            if (sender != null &&
                FrontEndGUI.DecodeMouseButtonEventFlags(e) != null)
            {
                
                // This absolute fucking useless bollocks #1
                switch (FrontEndGUI.DecodeMouseButtonEventFlags(e).ClickCount)
                {
                    case 0:
                    case 1:
                        // Remove the popup from the screen
                        financeviewmodel.childWindow.Close();
                        // The chart is re-instated in Chart_WindowClosed - no its not anymore
                        break;
                    default:
                        break;
                }
                // Not sure what effect THIS has
                FrontEndGUI.DecodeMouseButtonEventFlags(e).Handled = true;
            }
            return;
        }
#endif

#if SMARTMAUI
        internal static void Chart_MouseDoubleClick(object sender, object e,
                                                    FinanceViewModel financeviewmodel)
        {
            if (sender != null &&
                FrontEndGUI.DecodeMouseButtonEventFlags(e) != null)
            {
                EventArgs rays = FrontEndGUI.DecodeMouseButtonEventFlags(e);
                var bob = Convert.ToUInt16(rays.ToString());
                // This absolute fucking useless bollocks #1
                switch (bob)
                {
                    case 0:
                    case 1:
                        // Remove the popup from the screen
                        financeviewmodel.childWindow.Close();
                        // The chart is re-instated in Chart_WindowClosed - no its not anymore
                        break;
                    default:
                        break;
                }
            }
            return;
        }
#endif
#if XAMARINX || SMARTMAUI // Perhaps WINUI ? Not sure NO
        // All I know is, I did all this shit for XAMARSHIT to get
        // a drop down with colour I could Select
        // Thanks to Patil2421 https://forums.XAMARSHIT.com/discussion/91188/handling-button-double-click-double-tap
        //Declare a variable
        public int ButtonCount = 0;

        // Institution Button Click Event:
        public void InstitutionButtonClick(object sender, EventArgs e)
        {
            if (sender != null && e != null)
            {
                if (ButtonCount < 1)
                {
                    TimeSpan tt = new TimeSpan(0, 0, 1);
                    //Application.Current.Dispatcher.StartTimer(tt, TestHandleFunction(sender, e));
                }
                ButtonCount++;
            }
            return;
        }

        //public async Task<Func<bool>> TestHandleFunction(object sender, EventArgs e)
        //{
        //    if (ButtonCount > 1)
        //    {
        //        //Your action for Double Click here
        //        //DisplayAlert("", "Two Clicks", "OK");
        //        await Finance_InstitutionMouseDoubleClick(sender, e);
        //    }
        //    else
        //    {
        //        //Your action for Single Click here
        //        //DisplayAlert("", "One Click", "OK");
        //        InstitutionsStartUp(); // (sender, e);
        //    }
        //    ButtonCount = 0;
        //    return () => false;
        //}

        //public async void Finance_InstitutionMouseDClick(object sender, EventArgs e)
        //{
        //    await Finance_InstitutionMouseDoubleClick(sender, e);
        //    return;
        //}


#if WINFORMS || WPF  || WINUI
        public async void InstitutionsStartUp() // (object sender, EventArgs e)
        {
            await InstitutionsStartUp_Actual(); // (sender, e);
            return;
        }

        public async Task<bool> InstitutionsStartUp_Actual() //(object sender, EventArgs e)
        {
            if (financeviewmodel.FinanceInstitution_Selections.Length == 0)
            {
                financeviewmodel.FinanceInstitution_Selections = (dynamic[])financeviewmodel.FinanceInstitutionsList.ToArray();
            }
            //if (financeviewmodel.institutions_frame == nll)
            //{
            financeviewmodel.InstitutionWidth = financeviewmodel.institutions_frame.Width;
            financeviewmodel.InstitutionTranslationX = financeviewmodel.institutions_frame.TranslationX;
            financeviewmodel.InstitutionTranslationY = financeviewmodel.institutions_frame.TranslationY;

#if WINFORMS  || WINUI
            double screenCoordinateX = financeviewmodel.institutions_frame.X;
            double screenCoordinateY = financeviewmodel.institutions_frame.Y;
#endif
#if WPF
            double screenCoordinateX = financeviewmodel.institutions_frame.ActualWidth;
            double screenCoordinateY = financeviewmodel.institutions_frame.ActualHeight;
#endif
            // Get the view's parent (if it has one...)
            if (financeviewmodel.institutions_frame.Parent.GetType() != typeof(App))

            {
#if WINFORMS  || WINUI
                VisualElement parent = (VisualElement)financeviewmodel.institutions_frame.Parent;
#endif
#if WPF
                var parent = financeviewmodel.institutions_frame.Parent;
#endif
                // Loop back through all parents
                while (parent != null)
                {
                    // Add in the coordinates of the parent with respect to ITS parent
                    screenCoordinateX += parent.X;
                    screenCoordinateY += parent.Y;
                    // If the parent of this parent isn't the app itself, get the parent's parent.
                    if (parent.Parent.GetType() == typeof(App))
                    {
                        break;
                    }
                    else
                    {
                        parent = (VisualElement)parent.Parent;
                    }
                }
            }

            // Return the final coordinates...which are the global SCREEN coordinates of the view
            financeviewmodel.InstitutionTranslationX = screenCoordinateX;
            financeviewmodel.InstitutionTranslationY = screenCoordinateY + financeviewmodel.institutions_frame.Height;
            //}

            await FinanceInstitutionDropdown.OpenFinanceInstitutions(
                                                                financeviewmodel,
                                                                SmartParametersV2016.institutionprompt,
                                                                sb => financeviewmodel.FinanceInstitutions_Text = sb,
                                                                cb => financeviewmodel.FinanceInstitutionsColour = cb);
            financeviewmodel.ArrowButton = "expander_close.png";
            return true;
        }

        internal async void ProvidersStartUp(object sender, EventArgs e)
        {
            // If 'e' has been set to null, do not 'process' tapped event
            if (sender != null && e != null)
            {
                if (financeviewmodel.FinanceProvider_Selections.Count() == 0)
                {
                    financeviewmodel.FinanceProvider_Selections = (dinamic[])financeviewmodel.FinanceProvidersList.ToArray();

                    if (financeviewmodel.providers_frame.Width != -1)
                    {
                        financeviewmodel.ProviderWidth = financeviewmodel.providers_frame.Width;
                        financeviewmodel.ProviderTranslationX = financeviewmodel.providers_frame.TranslationX;
                        financeviewmodel.ProviderTranslationY = financeviewmodel.providers_frame.TranslationY;
                        double screenCoordinateX = financeviewmodel.providers_frame.X;
                        double screenCoordinateY = financeviewmodel.providers_frame.Y;
                        // Get the view's parent (if it has one...)
                        if (financeviewmodel.providers_frame.Parent.GetType() != typeof(App))
                        {
#if WINFORMS  || WINUI
                            VisualElement parent = (VisualElement)financeviewmodel.providers_frame.Parent;
#endif
#if WPF
                            var parent = financeviewmodel.providers_frame.Parent;
#endif
                            // Loop through all parents
                            while (parent != null)
                            {
                                // Add in the coordinates of the parent with respect to ITS parent
                                screenCoordinateX += parent.X;
                                screenCoordinateY += parent.Y;
                                // If the parent of this parent isn't the app itself, get the parent's parent.
                                if (parent.Parent.GetType() == typeof(App))
                                {
                                    break;
                                }
                                else
                                {
                                    parent = (VisualElement)parent.Parent;
                                }
                            }
                        }

                        // Return the final coordinates...which are the global SCREEN coordinates of the view
                        financeviewmodel.ProviderTranslationX = screenCoordinateX;
                        financeviewmodel.ProviderTranslationY = screenCoordinateY + financeviewmodel.providers_frame.Height;
                    }
                }
            }
            await FinanceProviderDropdown.OpenFinanceProviders((ContentPage)this.Parent,
                                                                            financeviewmodel,
                                                                SmartParametersV2016.providersprompt,
                                                                sb => financeviewmodel.FinanceProviders_Text = sb);
            financeviewmodel.ArrowButton = "expander_close.png";
            return;
        }

        internal async void AccountsStartUp(object sender, EventArgs e)
        {
            // If 'e' has been set to null, do not 'process' tapped event
            if (sender != null && e != null)
            {
                if (financeviewmodel.FinanceAccount_Selections.Length == 0)
                {
                    financeviewmodel.FinanceAccount_Selections = (dinamic[])financeviewmodel.FinanceAccountsList.ToArray();

                    if (financeviewmodel.accounts_frame.Width != -1)
                    {
                        financeviewmodel.AccountWidth = financeviewmodel.accounts_frame.Width;
                        financeviewmodel.AccountTranslationX = financeviewmodel.accounts_frame.TranslationX;
                        financeviewmodel.AccountTranslationY = financeviewmodel.accounts_frame.TranslationY;
                        double screenCoordinateX = financeviewmodel.accounts_frame.X;
                        double screenCoordinateY = financeviewmodel.accounts_frame.Y;
                        // Get the view's parent (if it has one...)
                        if (financeviewmodel.accounts_frame.Parent.GetType() != typeof(App))
                        {
#if WINFORMS  || WINUI
                            VisualElement parent = (VisualElement)financeviewmodel.accounts_frame.Parent;
#endif
#if WPF
                            var parent = financeviewmodel.accounts_frame.Parent;
#endif
                            // Loop through all parents
                            while (parent != null)
                            {
                                // Add in the coordinates of the parent with respect to ITS parent
                                screenCoordinateX += parent.X;
                                screenCoordinateY += parent.Y;
                                // If the parent of this parent isn't the app itself, get the parent's parent.
                                if (parent.Parent.GetType() == typeof(App))
                                {
                                    break;
                                }
                                else
                                {
                                    parent = (VisualElement)parent.Parent;
                                }
                            }
                        }

                        // Return the final coordinates...which are the global SCREEN coordinates of the view
                        financeviewmodel.AccountTranslationX = screenCoordinateX;
                        financeviewmodel.AccountTranslationY = screenCoordinateY + financeviewmodel.accounts_frame.Height;
                    }
                }
            }

            await FinanceAccountDropdown.OpenFinanceAccounts(
                                                                financeviewmodel,
                                                                SmartParametersV2016.accountsprompt,
                                                                sb => financeviewmodel.FinanceAccounts_Text = sb);
            financeviewmodel.ArrowButton = "expander_close.png";
            return;
        }
#endif        //} // Don't know WHY!!!!
#endif

#if ANDROIDX
        //Color ihavenoidea = Color.FromHex("#F2F2F2");

        public void OnCellAppearing(object sender, EventArgs e, FinanceViewModel financeviewmodel)
        {
            if (financeviewmodel.contrastColour)
            {
                financeviewmodel.LabelColour = Android.Graphics.Color.LightSteelBlue;
            }
            else
            {
                financeviewmodel.LabelColour = Android.Graphics.Color.White;
            }

            //                ViewCell view_cell = (ViewCell)sender;           
            //            IReadOnlyList<IVisualTreeElement> abc = view_cell.View.GetVisualTreeDescendants();

            //            foreach (IVisualTreeElement ivte in abc)
            //            {
            //                if (ivte.GetType().ToString() == "Grid")
            //                {
            //                    IReadOnlyList<IVisualTreeElement> def = ivte.GetVisualTreeDescendants();
            //                    foreach (IVisualTreeElement xyz in def)
            //                    {
            //                        if (xyz.GetType().ToString() == "Label")
            //                        {
            //                            Label label = xyz as Label;
            //                            if (financeviewmodel.contrastColour)
            //                            {
            //                                label.BackgroundColor = Colors.LightSteelBlue;
            //                            }
            //                            else
            //                            {
            //#if X
            //                                label.BackgroundColor = Color.White;
            //#xelse
            //                                label.BackgroundColor = Colors.White;
            //#endif
            //                            }
            //                        }
            //                    }
            //                    //foreach (Label label in ivte.GetVisualChildren())
            //                    //{
            //                    //}
            //                }
            //            }
            //Grid grid = view_cell.LogicalChildren[0] as Grid;
            //foreach (Label label in grid.Children)
            //{
            //    if (financeviewmodel.contrastColour)
            //    {
            //        label.BackgroundColor = Colors.LightSteelBlue;
            //    }
            //    else
            //    {
            //        label.BackgroundColor = Colors.White;
            //    }
            //}
            // Switch from contrast to white and vice-versa
            financeviewmodel.contrastColour = !financeviewmodel.contrastColour;
        }
#endif

#if ANDROIDX
        public class CultureItemSelectedListener : Java.Lang.Object, AdapterView.IOnItemSelectedListener
        {
            private AppCompatActivity activity;
            private MainViewModel ourviewmodel;
            private FinanceViewModel financeviewmodel;
            
            public CultureItemSelectedListener(AppCompatActivity meterActivity, MainViewModel mainvm, FinanceViewModel financevm)
            {
                this.activity = meterActivity;
                this.ourviewmodel = mainvm;
                this.financeviewmodel = financevm;
                return;

            }
            public void OnItemSelected(AdapterView parent, View view, int position, long id)
            {
                // Change the selected item's color to GREEN
                ((TextView)view.FindViewById<TextView>(Resource.Id.textView)).SetTextColor(ourviewmodel.greenColour); //Change selected text color
                // And call the "SelectedItemChanged" routine directly
                EventArgs e = new EventArgs();
                FinanceView.FinanceCultures_SelectionChanged(financeviewmodel.FinanceCultures, e,
                                                                    this.activity,
                                                                    ourviewmodel,
                                                                    financeviewmodel);
                return;
            }

            public void OnNothingSelected(AdapterView parent)
            {
            }
        }
#endif
#if (WPF) && CRYPTOS
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            financeviewmodel.WalletTotalsIndex = -1;
            TextBox tb = sender as TextBox;
            if (tb.DataContext is SmartFinance.CryptoWalletTotals cryptowallet)
            {
                financeviewmodel.WalletTotalsIndex = financeviewmodel.FinanceWalletTotals.IndexOf(cryptowallet);
            }
            tb.SelectAll();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Automatically saved via binding
        }

        private async void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var tb = sender as TextBox;
                TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                tb.MoveFocus(request);
                if (financeviewmodel.WalletTotalsIndex >= 0)
                {
                    financeviewmodel.FinanceWalletTotals[financeviewmodel.WalletTotalsIndex].CRYPTO_WALLET_NAME = tb.Text;
                    SmartFinance.CryptoWalletTotals updatedTotal = financeviewmodel.FinanceWalletTotals[financeviewmodel.WalletTotalsIndex];
                    updatedTotal.Updated = true;
                    financeviewmodel.PLO.crypto_wallettotals_changesList.Add(updatedTotal);
                    // This SHOULD get all the Inserts and Updates in one fell swoop
                    if (!await SmartFinanceScrapeV2023.DoAllSmartFinance(ourviewmodel,
                                financeviewmodel,
                                true,
                                SmartParametersV2016.sqliteformat))
                    {
                        financeviewmodel.errorMessage = "Cannot update wallet totals(1) - cannot continue";
#if WPF
                        await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
#endif
                        // This is a bit fatal, because if we can't do this, there's
                        // every chance we can't do lots of things we need to further on
                        //return false;
                    }
                    else
                    {
                        SmartFinanceV2025.UpdateCryptoLedgers(financeviewmodel);
                    }
                }
            }
            return;
        }
#endif
    }
}