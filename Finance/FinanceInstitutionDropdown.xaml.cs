using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#if UWP
using Windows.UI.Xaml.Controls;
#endif

#if WINUI
using Microsoft.UI.Xaml.Controls;
#endif

namespace SmartCubeMobile
{
#if UWP || WINUI
    public partial class FinanceInstitutionDropdown : ContentDialog
    {
#endif
#if SMARTMAUI
    public partial class FinanceInstitutionDropdown : ContentPage
    {
#endif
        private FinanceViewModel financeviewmodel;
        internal FinanceInstitutionDropdown(FinanceViewModel financevm)
        {
#if WINFORMS || WPF || SMARTMAUI
            InitializeComponent();
#endif
            financeviewmodel = financevm;
#if UWP || WINUI
            DataContext = financeviewmodel;
#endif
        }

        internal void OnInstitutionItemTapped(object sender, EventArgs e)
        {
            // If 'e' has been set to null, do not 'process' tapped event
            if (sender != null && e != null)
            {
#if UWP || WINUI
                StackPanel stack_layout = sender as StackPanel;
                FinanceViewModel.InstitutionItem selected_item = stack_layout.DataContext as FinanceViewModel.InstitutionItem;
#endif
#if SMARTMAUI
                VerticalStackLayout stack_layout = sender as VerticalStackLayout;
                FinanceViewModel.InstitutionItem selected_item = stack_layout.BindingContext as FinanceViewModel.InstitutionItem;
#endif
                List<FinanceViewModel.InstitutionItem> tempInstitutions = financeviewmodel.FinanceInstitutionsList;

                int index = 0;
                dynamic[] selections = new dynamic[tempInstitutions.Count];
                foreach (FinanceViewModel.InstitutionItem item in tempInstitutions)
                {
                    if (item.InstitutionID == selected_item.InstitutionID)
                    {
                        item.IsChecked = true;
                    }
                    selections[index] = item;
                    index++;
                }
                financeviewmodel.FinanceInstitutionsList = new List<FinanceViewModel.InstitutionItem>(tempInstitutions);
                financeviewmodel.FinanceInstitution_Selections = (dynamic[])selections.Clone();
                financeviewmodel.InstitutionPopupVisible = false;
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
        private void OnInstitutionSelected(object sender, SelectionChangedEventArgs e)
        {
        }
#endif
    }
}