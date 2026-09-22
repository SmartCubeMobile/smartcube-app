#if WPF
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.System;
#endif



#if WINUI
using iText.Layout.Element;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
#endif

#if ANDROIDX
using Android.Views;
#endif

#if SMARTMAUI
using System.Diagnostics;
#endif



namespace SmartCubeMobile
{
#if WPF  || WINUI || SMARTMAUI
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
#if WPF
    public partial class FinanceConnectionsLogins : Window
#endif
#if WINUI
    public sealed partial class FinanceConnectionsLogins : ContentDialog
#endif
#if SMARTMAUI
    public partial class FinanceConnectionsLogins : ContentPage
#endif
    {
        private SignInViewModel signinviewmodel;
        private MainViewModel ourviewmodel;
        private FinanceViewModel financeviewmodel;

        public string DefaultInstitutionName = "";
        public short DefaultInstitutionCode = 0;

        public bool IsReady = false;
        private bool _keyHandlerBusy;
        public FinanceConnectionsLogins(SignInViewModel signinvm,
                                            MainViewModel mainvm,
                                            FinanceViewModel financevm,
                                            short institutionCode,
                                            string institutionName)
        {
#if WPF || WINUI
            InitializeComponent();
#endif
            signinviewmodel = signinvm;
            ourviewmodel = mainvm;
            financeviewmodel = financevm;

            DefaultInstitutionCode = institutionCode;
            DefaultInstitutionName = institutionName;
            InitializeViewModels();
            WireEvents();
            LoadStartupData();
            IsReady = true;
            return;
        }

        private void InitializeViewModels()
        {
#if WPF || WINUI
            DataContext = financeviewmodel;
#endif
#if SMARTMAUI
            BindingContext = financeviewmodel;
#endif
            financeviewmodel.CreatedBrands = new List<BrandItem>();
            foreach (SmartFinance.Brands brand in financeviewmodel.PLO.brandsList)
            {
                if (brand.INSTITUTION_CODE == DefaultInstitutionCode)
                {
                    BrandItem item = new BrandItem()
                    {
                        InstitutionCode = brand.INSTITUTION_CODE,
                        BRAND_CODE = brand.BRAND_CODE,
                        BrandName = brand.BRAND_NAME
                    };
                    financeviewmodel.CreatedBrands.Add(item);
                }
            }
            financeviewmodel.ConnectionsViewList = new List<SmartFinance.Connections>();
            financeviewmodel.LoginsViewList = new List<SmartFinance.Logins>();

            return;
        }

#if WPF
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (!financeviewmodel.suppressClose)
            {
                Close();
            }
        }
#endif

#if WPF  || WINUI
        private void WireEvents()
        {
            //MyCombo.SelectionChanged += (s, e) =>
            //    SmartFinanceV2025.InstitutionSelectionChanged(
            //        s, e, ourviewmodel, financeviewmodel);
            ConnectionAdd.Click += async (s, e) =>
                await AddConnectionCoreAsyncWrapper(signinviewmodel, ourviewmodel, financeviewmodel, DefaultInstitutionCode, DefaultInstitutionName);
            LoginAdd.Click += async (s, e) =>
                await AddLogin_Click(s, e, ourviewmodel, financeviewmodel, this);
            Finish.Click += Finish_Click;
        }
#endif
#if SMARTMAUI
        private void WireEvents()
        {
            //MyCombo.SelectionChanged += (s, e) =>
            //    SmartFinanceV2025.InstitutionSelectionChanged(
            //        s, e, ourviewmodel, financeviewmodel);
            ConnectionAdd.Clicked += async (s, e) =>
                await AddConnectionCoreAsyncWrapper(signinviewmodel, ourviewmodel, financeviewmodel, DefaultInstitutionCode, DefaultInstitutionName);
            LoginAdd.Clicked += async (s, e) =>
                await AddLogin_Click(s, e, ourviewmodel, financeviewmodel, this);
            Finish.Clicked += Finish_Click;
        }
#endif
        internal async Task AddConnectionCoreAsyncWrapper(SignInViewModel signinviewmodel,
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    short DefaultInstitutionCode,
                                                    string DefaultInstitutionName)
        {
            SmartFinance.Connections connection = await SmartFinanceV2025.AddConnectionCoreAsync(
                                                        signinviewmodel,
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        DefaultInstitutionCode,
                                                        DefaultInstitutionName,
                                                        this);

            if (connection != null)
            {
                RefreshConnections(financeviewmodel, connection, "Parameter1TextBox");
            }
        }

#if WPF
        internal void RefreshConnections(FinanceViewModel financeviewmodel,
                                        SmartFinance.Connections refocus = null,
                                        string textBoxName = null)
        {
            if (Keyboard.FocusedElement is TextBox tb)
                tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            CollectionViewSource
                .GetDefaultView(financeviewmodel.ConnectionsViewList)
                ?.Refresh();

            if (refocus != null && !string.IsNullOrEmpty(textBoxName))
            {
                FocusParameter(refocus, textBoxName);
            }
        }
#endif
#if SMARTMAUI
        internal void RefreshConnections(
                                        FinanceViewModel financeviewmodel,
                                        SmartFinance.Connections refocus = null,
                                        string textBoxName = null)
        {
            // Force CollectionView refresh if needed
            var existing = financeviewmodel.ConnectionsViewList;

            financeviewmodel.ConnectionsViewList =
                new List<SmartFinance.Connections>(existing);

            // Optional refocus
            if (refocus != null && !string.IsNullOrEmpty(textBoxName))
            {
                FocusParameter(refocus, textBoxName);
            }
        }
#endif
#if WINUI
        internal void RefreshConnections(FinanceViewModel financeviewmodel, SmartFinance.Connections refocus = null, string textBoxName = null)
        {
            var focused = Microsoft.UI.Xaml.Input.FocusManager.GetFocusedElement();

            if (focused is Microsoft.UI.Xaml.Controls.TextBox tb)
            {
                tb.GetBindingExpression(
                    Microsoft.UI.Xaml.Controls.TextBox.TextProperty
                )?.UpdateSource();
            }

            // In WinUI, ObservableCollection usually updates automatically.
            // Only refresh if you're using a view wrapper.
            if (financeviewmodel.ConnectionsViewList is ICollectionView view)
            {
                var list = financeviewmodel.ConnectionsViewList;

                financeviewmodel.ConnectionsViewList = null;
                financeviewmodel.ConnectionsViewList = list;
            }

            if (refocus != null && !string.IsNullOrEmpty(textBoxName))
            {
                FocusParameter(refocus, textBoxName);
            }
        }

#endif
        private void LoadStartupData()
        {
            SmartFinanceV2025.LoadConnections(financeviewmodel, DefaultInstitutionCode, DefaultInstitutionName);
            SmartFinanceV2025.LoadLogins(financeviewmodel, DefaultInstitutionCode, DefaultInstitutionName);
            return;
        }

#if WPF
        private void Finish_Click(object sender, RoutedEventArgs e)
#endif
#if WINUI
        private async void Finish_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
#endif
#if SMARTMAUI
        private void Finish_Click(object sender, EventArgs e)
#endif
        {
            // Remove all drafts first
            RemoveAbandonedDrafts();
            // Force refresh
            RefreshConnections(financeviewmodel);

            // Persist valid rows
            foreach (SmartFinance.Connections conn in financeviewmodel.ConnectionsViewList)
            {
                if (!conn.Delete)
                {
                    SaveConnection(conn);
                }
            }
            financeviewmodel.suppressClose = true;
#if WPF
            MessageBox.Show(this, "Connections saved successfully.", "Finish", MessageBoxButton.OK, MessageBoxImage.Information);
#endif
#if WINUI
            ContentDialog dialog = new ContentDialog
            {
                Title = "Saved success",
                Content = "Connections saved successfully.",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                XamlRoot = this.XamlRoot
            };

            ContentDialogResult result = await dialog.ShowAsync();
#endif
#if SMARTMAUI
            //MessageBox.Show(this, "Connections saved successfully.", "Finish", MessageBoxButton.OK, MessageBoxImage.Information);
            DisplayAlert(
                "Finish",
                "Connections saved successfully.",
                "OK");

#endif
            financeviewmodel.suppressClose = false;
#if WPF
            this.Close();
#endif
#if WINUI
            this.Hide();
#endif
        }
        
        private bool ResolveProviderSelection(SmartFinance.Connections connection,
                                            BrandItem brand)
        {
            if (connection == null || brand == null)
                return false;

            // Apply provider
            connection.BRAND_CODE = brand.BRAND_CODE;
            connection.BrandName = brand.BrandName;

            // Get available login methods
            var availableMethods = SmartFinanceV2025.ConnectionsGetAvailableMethods(financeviewmodel,
                                                                connection.INSTITUTION_CODE,
                                                                connection.BRAND_CODE);
            if (!availableMethods.Any())
                return false;

            connection.AvailableLoginMethods = availableMethods;

            if (availableMethods.Count == 1)
            {
                // 🔐 Auto-select but DO NOT COMMIT
                connection.LOGIN_METHOD = availableMethods[0];
                connection.IsLoginMethodLocked = true;
            }
            else
            {
                connection.LOGIN_METHOD = 0;
                connection.IsLoginMethodLocked = false;
            }

            // Enable parameters only after login finalized
#if WPF || WINUI
            connection.Parameter1Visibility = Visibility.Collapsed;
            connection.Parameter2Visibility = Visibility.Collapsed;
            connection.Parameter3Visibility = Visibility.Collapsed;
#endif

#if ANDROIDX
            connection.Parameter1Visibility = ViewStates.Gone;
            connection.Parameter2Visibility = ViewStates.Gone;
            connection.Parameter3Visibility = ViewStates.Gone;
#endif
#if SMARTMAUI
            connection.Parameter1Visibility = Visibility.Collapsed;
            connection.Parameter2Visibility = Visibility.Collapsed;
            connection.Parameter3Visibility = Visibility.Collapsed;
#endif
            connection.IsNew = false;
            connection.IsPendingCompletion = true;

            return true;
        }

        internal void FocusParameter1(SmartFinance.Connections connection,
                            string textBoxName)
        {
#if WPF
            Dispatcher.BeginInvoke(new Action(() =>
            {
                FocusInternal(connection, textBoxName);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
#endif
#if WINUI
            DispatcherQueue.TryEnqueue(() =>
            {
                FocusInternal(connection, textBoxName);
            });
#endif
#if SMARTMAUI
            Dispatcher.Dispatch(() =>
            {
                //await Task.Delay(50);

                FocusInternal(connection, textBoxName);
            });
#endif
        }

        internal void FocusParameter(SmartFinance.Connections connection, string textBoxName)
        {
            if (connection == null || ConnectionsListView == null)
                return;

#if WPF
            Dispatcher.BeginInvoke(new Action(() =>
            {
                FocusInternal(connection, textBoxName);
            }), System.Windows.Threading.DispatcherPriority.Loaded);
#endif
#if WINUI
            DispatcherQueue.TryEnqueue(() =>
            {
                FocusInternal(connection, textBoxName);
            });
#endif
#if SMARTMAUI
            Dispatcher.Dispatch(() =>
            {
                //await Task.Delay(50);

                FocusInternal(connection, textBoxName);
            });
#endif
        }

#if WPF  || WINUI || SMARTMAUI
        private void FocusInternal(SmartFinance.Connections connection, string textBoxName)
        {
#if WPF || WINUI
            ConnectionsListView.ScrollIntoView(connection);
            ConnectionsListView.UpdateLayout();

            //Weird that the commented out line USED to work!
           ListViewItem container = ConnectionsListView.ItemContainerGenerator
                   .ContainerFromItem(connection) as ListViewItem;
            if (container == null)
            {
                return;
            }

            TextBox textBox = FindVisualChild<TextBox>(
                container,
                tb => tb.Name == textBoxName && tb.Visibility == Visibility.Visible);

            if (textBox != null)
            {
#endif
#if WPF
                textBox.Focus();
                textBox.CaretIndex = textBox.Text?.Length ?? 0;
#endif
#if WINUI
                textBox.Focus(FocusState.Programmatic);
                textBox.SelectionStart = textBox.Text?.Length ?? 0;
                textBox.SelectionLength = 0;
#endif
#if SMARTMAUI
                //textBox.Focus();
                //textBox.CaretIndex = textBox.Text?.Length ?? 0;
#endif
#if WPF || WINUI
            }
#endif
        }
#endif

            //internal void FocusParameter(SmartFinance.Connections connection,
            //                    string textBoxName)
            //{
            //    Dispatcher.BeginInvoke(new Action(() =>
            //    {
            //        if (connection == null || ConnectionsListView == null) return;
            //        // Ensure the row is realized (virtualization-safe)
            //        ConnectionsListView.ScrollIntoView(connection);
            //        ConnectionsListView.UpdateLayout();
            //        // Get the ListViewItem container
            //        if (ConnectionsListView.ItemContainerGenerator
            //            .ContainerFromItem(connection) is not ListViewItem container)
            //            return;
            //        // Find the TextBox inside the row
            //        TextBox textBox = FindVisualChild<TextBox>(

            //                        container,
            //    tb => tb.Name == textBoxName && tb.Visibility == Visibility.Visible);
            //        if (textBox != null)
            //        {
            //            textBox.Focus();
            //            textBox.CaretIndex = textBox.Text?.Length ?? 0;
            //        }
            //    }), DispatcherPriority.Loaded);
            //}

        private async void ConnectionProvider_DropDownClosed(object sender, System.EventArgs e)
        {
            if (!IsLoaded || !IsReady) return;
#if WPF  || WINUI
            ComboBox comboBox = sender as ComboBox;
            //if (sender != comboBox as ComboBox) return;
            if (comboBox.DataContext != SmartFinance.Connections connection) return;
            
#endif
#if SMARTMAUI
            if (sender is not Picker comboBox) return;
            if (comboBox.BindingContext is not SmartFinance.Connections connection) return;
#endif
            if (comboBox.SelectedItem is not BrandItem brand) return;
            
            // 👇 This IS your ViewModel
            if (!ResolveProviderSelection(connection, brand))
            {
#if WPF
                new ToastWindow(this, SignIn.BesetByChimps(signinviewmodel, "NoFreeLoginMethods"), ToastType.Warning).Show();
#endif
#if WINUI
                ToastWindow tw = new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "NoFreeLoginMethods"), ToastType.Warning);
                tw.Show();
#endif
#if SMARTMAUI
                new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "NoFreeLoginMethods"), ToastType.Warning);
#endif
                return;
            }
            
            // 🔥 If login method auto-resolved → finalize via SAME path
            if (connection.IsLoginMethodLocked)
            {
                await FinalizeConnectionAsync(connection);
            }
            else
            {
                RefreshConnections(financeviewmodel); // show LoginMethod dropdown
            }
        }


#if WPF  || WINUI
        private void Parameter_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded || !IsReady) return;
            TextBox textBox = sender as TextBox;
            financeviewmodel.SelectedConnection = textBox?.DataContext as SmartFinance.Connections;
            if (financeviewmodel.SelectedConnection != null)
            {
                financeviewmodel.PARAMETER1 = financeviewmodel.SelectedConnection.Parameter1;
            }
            return;
        }
#endif

#if SMARTMAUI
        private void Parameter_GotFocus(object sender, EventArgs e)
        {
            if (!IsLoaded || !IsReady) return;
            Label textBox = sender as Label;
            financeviewmodel.SelectedConnection = textBox?.BindingContext as SmartFinance.Connections;
            if (financeviewmodel.SelectedConnection != null)
            {
                financeviewmodel.PARAMETER1 = financeviewmodel.SelectedConnection.Parameter1;
            }
            return;
        }
#endif

#if WPF || WINUI
        private static T FindVisualChild<T>(DependencyObject parent,
                                            Func<T, bool> predicate) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typed && predicate(typed)) return typed;
                var result = FindVisualChild(child, predicate);
                if (result != null) return result;
            }
            return null;
        }
#endif

        private void RefreshLoginsView()
        {
#if WPF
            var view = CollectionViewSource.GetDefaultView(
                financeviewmodel.LoginsViewList);
            view?.Refresh();
#endif
#if SMARTMAUI
            var list = financeviewmodel.LoginsViewList;
            financeviewmodel.LoginsViewList = new List<SmartFinance.Logins>(list);
#endif
        }
        private void Parameter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsLoaded || !IsReady) return;
#if WPF  || WINUI
            if (sender is not TextBox textBox) return;
            if (textBox.DataContext is not SmartFinance.Connections connection) return;
#endif
#if SMARTMAUI
            if (sender is not Entry textBox) return;
            if (textBox.BindingContext is not SmartFinance.Connections connection) return;
#endif
            financeviewmodel.SelectedConnection = connection;
            string value = textBox.Text?.Trim() ?? "";
            // Assign based on which Parameter
#if WPF  || WINUI

            switch (textBox.Name)
#endif
#if SMARTMAUI

            switch (textBox.StyleId)
#endif
            {
                case "Parameter1TextBox":
                    connection.Parameter1 = value;
                    break;
                case "Parameter2TextBox":
                    connection.Parameter2 = value;
                    break;
                case "Parameter3TextBox":
                    connection.Parameter3 = value;
                    break;
            }
            // Valid → mark row as no longer a draft
            connection.IsPendingCompletion = false;
            return;
        }

#if WPF
        private async void Parameter_KeyDown(object sender, KeyEventArgs e)
#endif
#if WINUI
        private async void Parameter_KeyDown(object sender, KeyRoutedEventArgs e)
#endif
#if SMARTMAUI
        private async void Parameter_Completed(object sender, EventArgs e)
#endif
        {
            // 🛡 Guard against early execution
            if (!IsLoaded || !IsReady) return;
#if WPF || WINUI
#if WPF
            if (e.Key is not (Key.Tab or Key.Enter))
#endif
#if WINUI
            if (e.Key is not (VirtualKey.Tab or VirtualKey.Enter))
#endif

            {
                return;
            }
            e.Handled = true;
#endif
            if (_keyHandlerBusy) return;
            
            _keyHandlerBusy = true;

            try
            {
#if WPF || WINUI
                if (sender is not TextBox textBox) return;
                if (textBox.DataContext is not SmartFinance.Connections connection) return;
                if (!int.TryParse(textBox.Tag?.ToString(), out int index)) return;
                // Commit binding
                textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
                
#endif
#if SMARTMAUI
                if (sender is not Entry textBox)
                    return;

                if (textBox.BindingContext is not SmartFinance.Connections connection)
                    return;

                if (!int.TryParse(textBox.Text, out int index))
                    return;
#endif
                financeviewmodel.SelectedConnection = connection;

                // 🔒 Parameter1 must exist before others
                if (index > 1 && string.IsNullOrWhiteSpace(connection.Parameter1))
                {
#if WPF
                    new ToastWindow(this, SignIn.BesetByChimps(signinviewmodel, "Parameter1Empty"), ToastType.Error).Show();
#endif
#if WINUI
                    ToastWindow tw = new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "Parameter1Empty"), ToastType.Error);
                    tw.Show();
#endif
#if SMARTMAUI
                    new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "Parameter1Empty"), ToastType.Error);
#endif
                    FocusParameter(connection, "Parameter1TextBox");
                    return;
                }

                // 🔐 Validate Parameter1 specifically
                if (index == 1)
                {
                    if (string.IsNullOrWhiteSpace(connection.Parameter1))
                    {
#if WPF
                        new ToastWindow(this, SignIn.BesetByChimps(signinviewmodel, "Parameter1Empty"), ToastType.Error).Show();
#endif
#if WINUI
                        ToastWindow tw = new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "Parameter1Empty"), ToastType.Error);
                        tw.Show();
#endif
#if SMARTMAUI
                        new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "Parameter1Empty"), ToastType.Error);
#endif
                        return;
                    }

                    if (!SmartSpikeFinanceV2017.FinanceLookupParameterValidity(
                            financeviewmodel.PLO.institutionsList,
                            financeviewmodel.PLO.brandsList,
                            financeviewmodel.PLO.brand_connectionList,
                            connection.INSTITUTION_CODE,
                            connection.BRAND_CODE,
                            connection.LOGIN_METHOD,
                            1,
                            connection.Parameter1))
                    {
#if WPF
                        new ToastWindow(this, SignIn.BesetByChimps(signinviewmodel, "Parameter1Failed"), ToastType.Error).Show();
#endif
#if WINUI
                        ToastWindow tw = new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "Parameter1Failed"), ToastType.Error);
                        tw.Show();
#endif
#if SMARTMAUI
                        new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "Parameter1Failed"), ToastType.Error);
#endif
                        connection.Parameter1 = "";
                        textBox.Text = "";
                        return;
                    }
                }

                // Persist
                if (!await SmartFinanceV2025.CommitConnectionAsync(signinviewmodel, ourviewmodel, financeviewmodel, connection, this, update: true, delete: false))
                    return;
                // 🔥 Auto-focus next parameter
                if (index < 3)
                {
                    FocusParameter(connection, $"Parameter{index + 1}TextBox");
                }
            }
            finally
            {
                _keyHandlerBusy = false;
            }
        }

#if WPF || WINUI
        private void Parameter_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!IsLoaded || !IsReady) return;
            if (sender is not TextBox textBox) return;
            // Push TextBox value into the bound property
            textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            // Track the selected connection (safe)
            if (textBox.DataContext is SmartFinance.Connections connection)
            {
                financeviewmodel.SelectedConnection = connection;
            }
            return;
        }
#endif
#if SMARTMAUI
        private void Parameter_LostFocus(object sender, EventArgs e)
        {
            if (!IsLoaded || !IsReady) return;
            if (sender is not Entry textBox) return;
            // Push TextBox value into the bound property
            // MAUI Bindings update automatically. So I'm told ...
            //textBox.GetBindingExpression(Entry.TextProperty)?.UpdateSource();

            // Track the selected connection (safe)
            if (textBox.BindingContext is SmartFinance.Connections connection)
            {
                financeviewmodel.SelectedConnection = connection;
            }
            return;
        }
#endif

        private void RemoveAbandonedDrafts()
        {
            List<SmartFinance.Connections> abandoned = financeviewmodel.ConnectionsViewList
                .Where(c => c.IsPendingCompletion && string.IsNullOrWhiteSpace(c.Parameter1))
                .ToList();
            foreach (SmartFinance.Connections draft in abandoned)
            {
                financeviewmodel.ConnectionsViewList.Remove(draft);
            }
            // Force refresh
            RefreshConnections(financeviewmodel);
            return;
        }

        private async void ConnectionLoginMethod_DropDownClosed(object sender, EventArgs e)
        {
            if (!IsLoaded || !IsReady) return;
#if WPF || WINUI
            if (sender is not ComboBox comboBox) return;
            if (comboBox.DataContext is not SmartFinance.Connections connection) return;
#endif
#if SMARTMAUI
            if (sender is not Picker comboBox) return;
            if (comboBox.BindingContext is not SmartFinance.Connections connection) return;

#endif
            // 🔒 Already finalized — ignore re-entry
            if (connection.IsLoginMethodLocked)
            {
                return;
            }
            if (comboBox.SelectedItem is not int selectedMethod)
            {
                return;
            }
            connection.LOGIN_METHOD = selectedMethod;
            connection.IsLoginMethodLocked = true;
            
            await FinalizeConnectionAsync(connection);
        }

        private async Task FinalizeConnectionAsync(SmartFinance.Connections connection)
        {
            if (connection == null) return;

            // 🔒 HARD GUARD — FINAL LINE OF DEFENCE
            if (connection.LOGIN_METHOD == 0)
            {
#if WPF
                new ToastWindow(this, SignIn.BesetByChimps(signinviewmodel, "LoginMethodNotSelected"), ToastType.Error).Show();
#endif
#if WINUI
                ToastWindow tw = new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "LoginMethodNotSelected"), ToastType.Error);
                tw.Show();
#endif
#if SMARTMAUI
                new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "LoginMethodNotSelected"), ToastType.Error);
#endif
                RefreshConnections(financeviewmodel);
                return;
            }
            // Enable parameters
#if WPF || WINUI || SMARTMAUI
            connection.Parameter1Visibility = Visibility.Visible;
            connection.Parameter2Visibility = Visibility.Visible;
            connection.Parameter3Visibility = Visibility.Visible;
#endif

            connection.IsPendingCompletion = false;

            if (!await SmartFinanceV2025.CommitConnectionAsync(signinviewmodel, ourviewmodel, financeviewmodel, connection, this, update: false, delete: false))
            {
                return;
            }
            RefreshConnections(financeviewmodel, connection, "Parameter1TextBox");
        }

        #region Delete / Checkbox

#if WPF || WINUI
        private async void ConnectionsDelete_Click(object sender, RoutedEventArgs e)
#endif
#if SMARTMAUI
        private async void ConnectionsDelete_Click(object sender, EventArgs e)
#endif
        {
            if (!IsLoaded || !IsReady) return;
            if (sender is not RadioButton rb ||
#if WPF || WINUI
                rb.DataContext is not SmartFinance.Connections connection)
#endif
#if SMARTMAUI
                rb.BindingContext is not SmartFinance.Connections connection)
#endif
            {
                return;
            }
            financeviewmodel.suppressClose = true;
#if WPF
            MessageBoxResult result = MessageBox.Show(this,
                $"Delete connection {connection.BrandName} {connection.Parameter1}?\n\n" +
                "All associated Logins will also be deleted.",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
#endif
#if WINUI
            ContentDialog dialog = new ContentDialog
            {
                Title = "Confirm Delete",
                Content = $"Delete connection {connection.BrandName} {connection.Parameter1}?\n\n" +
                "All associated Logins will also be deleted.",                
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                XamlRoot = this.XamlRoot
            };

            ContentDialogResult result = await dialog.ShowAsync();

#endif
#if SMARTMAUI
            //MessageBoxResult result = MessageBox.Show(this,
            //    $"Delete connection {connection.BrandName} {connection.Parameter1}?\n\n" +
            //    "All associated Logins will also be deleted.",
            //    "Confirm Delete",
            //    MessageBoxButton.YesNo,
            //    MessageBoxImage.Warning);

            //MessageBox.Show(this, "Connections saved successfully.", "Finish", MessageBoxButton.OK, MessageBoxImage.Information);
            bool result = await DisplayAlert(
                "Confirm",
                $"Delete connection {connection.BrandName} {connection.Parameter1}?\n\n" +
                "All associated Logins will also be deleted",
                "Yes",
                "No");
#endif
            financeviewmodel.suppressClose = false;
#if WPF
            if (result != MessageBoxResult.Yes)
#endif
#if WINUI
            if (result != ContentDialogResult.Primary)
#endif
#if SMARTMAUI
            if (result) // True is "Yes"
#endif
            {
                rb.IsChecked = false;
                return;
            }

            // 🔥 FIRST: delete associated logins
            var loginsToDelete = financeviewmodel.LoginsViewList
                .Where(l =>
                    l.INSTITUTION_CODE == connection.INSTITUTION_CODE &&
                    l.BRAND_CODE == connection.BRAND_CODE &&
                    l.LOGIN_METHOD == connection.LOGIN_METHOD)
                .ToList(); // 🔥 materialize before modifying collection

            foreach (var login in loginsToDelete)
            {
                financeviewmodel.SelectedLogin = login;
                financeviewmodel.SelectedLogin.USERNAME = ourviewmodel.UserName;
                financeviewmodel.SelectedLogin.CUBEFACE_CODE = SmartParametersV2016.Finance;
                financeviewmodel.SelectedLogin.Updated = true;
                financeviewmodel.SelectedLogin.Delete = true;
                financeviewmodel.PLO.finance_logins_changesList.Add(financeviewmodel.SelectedLogin);
            }

            // 🔥 THEN: delete the connection itself
            if (!await SmartFinanceV2025.CommitConnectionAsync(signinviewmodel, ourviewmodel, financeviewmodel, connection, this, update: true, delete: true))
            {
                return;
            }
            // Persist delete
            foreach (var login in loginsToDelete)
            {                
                financeviewmodel.LoginsViewList.Remove(login);
            }

            financeviewmodel.ConnectionsViewList.Remove(connection);

            // Lock remaining logins & refresh
            LockAllExistingLogins();
            RefreshConnections(financeviewmodel);
            RefreshLoginsView();
        }
#endregion

        private void SaveConnection(SmartFinance.Connections connection)
        {
            // 🔒 sanity check
            if (connection == null)
            {
                return;
            }
            // TODO: replace this with your real persistence logic
            // e.g. database update / file write / API call
            Debug.WriteLine(
                $"SAVED: {connection.BrandName}, LoginMethod={connection.LOGIN_METHOD}, " +
                $"Parameter1={connection.Parameter1}, Parameter2={connection.Parameter2}, " +
                $"Parameter3={connection.Parameter3}");
            return;
        }
        //<=====================  Logins ================>
#if WPF || WINUI
        private async Task<bool> AddLogin_Click(object sender, RoutedEventArgs e,
#endif
#if SMARTMAUI
        private async Task<bool> AddLogin_Click(object sender, EventArgs e,
#endif
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
#if WPF
                                        Window myWindow)
#endif
#if WINUI
                                        ContentDialog myWindow)
#endif
#if SMARTMAUI
                                        ContentPage myWindow)
#endif
        {
            if (!await SmartFinanceV2025.AddLoginCoreAsync(signinviewmodel, 
                                ourviewmodel,
                              financeviewmodel,
                              DefaultInstitutionCode,
                              DefaultInstitutionName
#if WPF
                                             , myWindow
#endif
#if WINUI
                                             , myWindow
#endif
#if SMARTMAUI
                                              , myWindow
#endif
                                              ))
            {
                return false;
            }
            RefreshLoginsView();
            return true;
        }

        private void LockAllExistingLogins()
        {
            foreach (var login in financeviewmodel.LoginsViewList)
            {
                login.IsNew = false;
                if (login.LOGIN_METHOD != 0 && login.UserFinalizedLoginMethod)
                {
                    login.IsLoginMethodLocked = true;
                }
            }
            return;
        }
        #region Login Changes
        private void LoginProvider_DropdownClosed(object sender, EventArgs e)
        {
            if (!IsLoaded || !IsReady) return;
#if WPF  || WINUI
            if (sender is ComboBox combobox &&
                combobox.DataContext is SmartFinance.Logins login &&
#endif
#if SMARTMAUI
            if (sender is Picker combobox &&
                combobox.BindingContext is SmartFinance.Logins login &&
#endif
            login.SelectedBrand != null)
            {
                // 🔥 THIS removes the blue highlight
                combobox.SelectedItem = null;
            }
            return;
        }
#endregion
        #region Assign Methods

        private void LoginMethod_DropdownClosed(object sender, EventArgs e)
        {
            if (!IsLoaded || !IsReady) return;
#if WPF  || WINUI
            if (sender is not ComboBox comboBox) return;
            if (comboBox.DataContext is not SmartFinance.Logins login) return;
#endif
#if SMARTMAUI
            if (sender is not Picker comboBox) return;
            if (comboBox.BindingContext is not SmartFinance.Logins login) return;
#endif
            // 🔒 SAFETY NET — nothing selected
            if (comboBox.SelectedItem == null) return;
            login.LOGIN_METHOD = (int)comboBox.SelectedItem;
            
            login.Parameter1 = SmartFinanceV2025.FindParameter1(
                financeviewmodel,
                login.INSTITUTION_CODE,
                login.BRAND_CODE,
                login.LOGIN_METHOD);
            SaveLogin(login);
            login.UserFinalizedLoginMethod = true;
            login.IsLoginMethodLocked = true;
            // Force UI refresh NO!!
            // RefreshLoginsView();            
            return;
        }
#endregion
        #region Logins Delete / Checkbox
#if WPF  || WINUI
        private async void LoginsDelete_Click(object sender, RoutedEventArgs e)
#endif
#if SMARTMAUI
        private async void LoginsDelete_Click(object sender, EventArgs e)
#endif
        {
            if (!IsLoaded || !IsReady) return;
            if (sender is RadioButton radiobutton &&
#if WPF  || WINUI
                radiobutton.DataContext is SmartFinance.Logins login)
#endif
#if SMARTMAUI
                radiobutton.BindingContext is SmartFinance.Logins login)
#endif
            {
                financeviewmodel.suppressClose = true;
#if WPF
                MessageBoxResult result = MessageBox.Show(this,
                    $"Are you sure you want to delete login Provider: {login.BrandName} {login.Parameter1} ?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
#endif
#if WINUI
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Confirm Delete",
                    Content = $"Are you sure you want to delete login Provider: {login.BrandName} {login.Parameter1} ?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No",
                    XamlRoot = this.XamlRoot
                };

                ContentDialogResult result = await dialog.ShowAsync();

#endif
#if SMARTMAUI
                //MessageBoxResult result = MessageBox.Show(this,
                //    $"Are you sure you want to delete login Provider: {login.BrandName} {login.Parameter1} ?",
                //    "Confirm Delete",
                //    MessageBoxButton.YesNo,
                //    MessageBoxImage.Warning);

                //MessageBox.Show(this, "Connections saved successfully.", "Finish", MessageBoxButton.OK, MessageBoxImage.Information);
                bool result = await DisplayAlert(
                    "Confirm",
                    $"Are you sure you want to delete login Provider: {login.BrandName} {login.Parameter1} ?",
                    "Yes",
                    "No");
#endif
                financeviewmodel.suppressClose = false;
#if WPF
                if (result == MessageBoxResult.Yes)
#endif
#if WINUI
                if (result == ContentDialogResult.Primary)
#endif
#if SMARTMAUI
                if (result) // true is "Yes"
#endif
                {
                    login.Delete = true;                    
                    // 🔥 THEN: delete the login itself
                    if (!await SmartFinanceV2025.CommitLoginAsync(signinviewmodel, ourviewmodel, financeviewmodel, login, this, update: true, delete: true))
                    {
                        return;
                    }
                    // Persist delete
                    financeviewmodel.LoginsViewList.Remove(login);
                    // 🔒 IMPORTANT: explicitly lock remaining rows
                    LockAllExistingLogins();
                    // 🔥 ONE controlled rebuild
                    RefreshLoginsView();                    
                }
                else
                {
                    radiobutton.IsChecked = false;
                }
            }
            return;
        }

#if WPF  || WINUI
        private void CheckBox_Changed(object sender, RoutedEventArgs e)
#endif
#if SMARTMAUI
        private void CheckBox_Changed(object sender, EventArgs e)
#endif
        {
            if (!IsLoaded || !IsReady) return;
            if (sender is CheckBox combobox &&
#if WPF  || WINUI
                combobox.DataContext is SmartFinance.Logins login)
#endif
#if SMARTMAUI
                combobox.BindingContext is SmartFinance.Logins login)
#endif
            {
                login.Updated = true;
                SaveLogin(login);
            }
            return;
        }
#endregion
        
        #region Save Login
        private void SaveLogin(SmartFinance.Logins login)
        {
            if (login == null) return;
            Debug.WriteLine($"SAVED: {login.BrandName}, LoginMethod={login.LOGIN_METHOD}, Banks={login.BANKSCHECKED}, Savings={login.SAVINGSCHECKED}, Invest={login.INVESTMENTSCHECKED}, Crypto={login.CRYPTOSCHECKED}");
        }
        #endregion

#if WPF
        private void LoginComboBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
#endif
#if WINUI
        private void LoginComboBox_PreviewMouseDown(object sender, PointerRoutedEventArgs e)
#endif
#if SMARTMAUI
        private void LoginComboBox_PreviewMouseDown(object sender, PointerEventArgs e)
#endif
        {
            if (!IsLoaded || !IsReady) return;
#if WPF  || WINUI
            if (sender is not ComboBox comboBox) return;
            if (comboBox.DataContext is not SmartFinance.Logins login) return;
#endif
#if SMARTMAUI
            if (sender is not Picker comboBox) return;
            if (comboBox.BindingContext is not SmartFinance.Logins login) return;
#endif
            // Ensure the ListView row is selected FIRST
            financeviewmodel.SelectedLogin = login;
            // Force dropdown to open on first click
#if WPF  || WINUI
            if (!comboBox.IsDropDownOpen)
            {
                comboBox.IsDropDownOpen = true;

                e.Handled = true; // prevent ListView from stealing the click

            }
#endif
        }

#if SMARTMAUI
        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
#endif

#if SMARTMAUI
        private void OnConnectionAddClicked(object sender, EventArgs e)
        {
        }

        private void OnLoginAddClicked(object sender, EventArgs e)
        {
        }

        private async void OnFinishClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
#endif
    }
#endif
        }