
using System;
using System.Collections.Generic;
using System.Linq;

#if UWP
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endif

#if WINUI
using Microsoft.UI.Xaml.Controls;
#endif

namespace SmartCubeMobile
{
#if UWP || WINUI
    public partial class FinanceProviderDropdown : ContentDialog
    {
#endif
#if SMARTMAUI
    public partial class FinanceProviderDropdown : ContentPage
    {
#endif
        private FinanceViewModel financeviewmodel;
        internal FinanceProviderDropdown(FinanceViewModel financevm)
        {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
            InitializeComponent();
#endif
            financeviewmodel = financevm;
#if UWP || WINUI
            DataContext = financeviewmodel;
#endif
        }

        internal void OnProviderItemTapped(object sender, EventArgs e)
        {
            // If 'e' has been set to null, do not 'process' tapped event
            if (sender != null && e != null)
            {
#if UWP || WINUI
                StackPanel ours = sender as StackPanel;
                FinanceViewModel.ProviderItem abc = ours.DataContext as FinanceViewModel.ProviderItem;
#endif
#if SMARTMAUI
                VerticalStackLayout ours = sender as VerticalStackLayout;
                FinanceViewModel.ProviderItem abc = ours.BindingContext as FinanceViewModel.ProviderItem;
#endif
                List<FinanceViewModel.ProviderItem> tempBrands = financeviewmodel.FinanceProvidersList;

                int index = 0;
                dynamic[] selections = new dynamic[tempBrands.Count()];
                foreach (FinanceViewModel.ProviderItem brand in tempBrands)
                {
                    if (brand.ProviderID == abc.ProviderID)
                    {
                        brand.IsChecked = !brand.IsChecked;
                    }
                    selections[index] = brand;
                    index++;
                }
                financeviewmodel.FinanceProvidersList = new List<FinanceViewModel.ProviderItem>(tempBrands);
                financeviewmodel.FinanceProvider_Selections = (dynamic[])selections.Clone();
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
        private void OnProviderSelected(object sender, SelectionChangedEventArgs e)
        {
        }
#endif
    }
}