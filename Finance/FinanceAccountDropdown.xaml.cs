using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;


#if UWP
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
#endif

#if WINUI
using Microsoft.UI.Xaml.Controls;
#endif

namespace SmartCubeMobile
{
#if UWP || WINUI
    public partial class FinanceAccountDropdown : ContentDialog
    {
#endif
#if SMARTMAUI
    public partial class FinanceAccountDropdown : ContentPage
    {
#endif
        private FinanceViewModel financeviewmodel;
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
        public FinanceAccountDropdown(FinanceViewModel financevm)
#endif

        {

#if WINFORMS || WPF || UWP || SMARTMAUI
            InitializeComponent();

#endif
            financeviewmodel = financevm;
#if UWP
            DataContext = MainMeter.financeviewmodel;
#endif
        }

        internal void OnAccountItemTapped(object sender, EventArgs e)
        {
            // If 'e' has been set to null, do not 'process' tapped event
            if (sender != null && e != null)
            {
#if UWP || WINUI
                StackPanel ours = sender as StackPanel;
                FinanceViewModel.AccountItem abc = ours.DataContext as FinanceViewModel.AccountItem;
#endif
#if SMARTMAUI
                VerticalStackLayout ours = sender as VerticalStackLayout;
                FinanceViewModel.AccountItem abc = ours.BindingContext as FinanceViewModel.AccountItem;
#endif
                List<FinanceViewModel.AccountItem> tempAccounts = financeviewmodel.FinanceAccountsList;

                int index = 0;
                dynamic[] selections = new dynamic[tempAccounts.Count()];
                foreach (FinanceViewModel.AccountItem account_item in tempAccounts)
                {
                    if (account_item.AccountID == abc.AccountID)
                    {
                        account_item.IsChecked = !account_item.IsChecked;
                    }
                    selections[index] = account_item;
                    index++;
                }
                financeviewmodel.FinanceAccountsList = new List<FinanceViewModel.AccountItem>(tempAccounts);
                financeviewmodel.FinanceAccount_Selections = (dynamic[])selections.Clone();
            }
            return;
        }

#if SMARTMAUI
        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
#endif

#if SMARTMAUI
        private void OnAccountSelected(object sender, SelectionChangedEventArgs e)
        {
        }
#endif
    }
}