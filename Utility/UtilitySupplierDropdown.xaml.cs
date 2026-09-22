#if UWP
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
#endif

#if WINUI
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
#endif

#if ANDROIDX
using Android.Views;
#endif

namespace SmartCubeMobile
{
#if UWP
    public partial class UtilitySupplierDropdown : Page
#endif
#if WINUI
    public partial class UtilitySupplierDropdown : Page
#endif
#if ANDROIDX
    public partial class UtilitySupplierDropdown : PopupWindow
#endif
#if SMARTMAUI
    public partial class UtilitySupplierDropdown : ContentPage
#endif
    {
        private MainViewModel ourviewmodel; 
        private UtilityViewModel utilityviewmodel;
        internal UtilitySupplierDropdown(MainViewModel mainvm, UtilityViewModel utilityvm)
        {
#if WINFORMS || WPF || UWP || WINUI || SMARTMAUI
            InitializeComponent();
            ourviewmodel = mainvm;
            utilityviewmodel = utilityvm;
#endif
#if UWP || WINUI
            DataContext = utilityviewmodel;
            //this.IsOpen = true;
#endif
            return;
        }

        internal void OnSupplierItemTapped(object sender, EventArgs e)
        {
            // If 'e' has been set to null, do not 'process' tapped event
            if (sender != null && e != null)
            {
                UtilityViewModel.SupplierItem selected_item = new UtilityViewModel.SupplierItem();
                if (utilityviewmodel.SupplierSelectedIndex != -1)
                {
                    selected_item.Supplier_ID = utilityviewmodel.SupplierSelectedIndex;
                }
                else
                {
#if UWP || WINUI
                    StackPanel stack_layout = sender as StackPanel;
                    selected_item = stack_layout.DataContext as UtilityViewModel.SupplierItem;
#endif
#if ANDROIDX
                    TextView stack_layout = (TextView)sender;
                    UtilityViewModel.SupplierItem abc = new UtilityViewModel.SupplierItem()
                    {
                        Content = stack_layout.Text,
                        IsChecked = true,
                        Value = "",
                        Supplier_ID = 0
                    };
#endif
                }
                List<UtilityViewModel.SupplierItem> tempSuppliers = utilityviewmodel.UtilitySuppliersList;

                int index = 0;
                dynamic[] selections = new dynamic[tempSuppliers.Count];
                foreach (UtilityViewModel.SupplierItem item in tempSuppliers)
                {
                    if (item.Supplier_ID == selected_item.Supplier_ID)
                    {
                        item.IsChecked = true;
                    }
                    selections[index] = item;
                    index++;
                }
                utilityviewmodel.UtilitySuppliersList = new List<UtilityViewModel.SupplierItem>(tempSuppliers);
                utilityviewmodel.UtilitySupplier_Selections = (dynamic[])selections.Clone();
            }
#if ANDROIDX
            this.Dismiss();
#endif
            utilityviewmodel.SupplierPopupVisible = false;
            return;
        }

#if ANDROIDX
        protected async void OnDisappearing()
        {
            if (utilityviewmodel.SupplierPopupVisible)
            {
                this.Dismiss();
                utilityviewmodel.SupplierPopupVisible = false;
            }
            utilityviewmodel.ArrowButton.SetImageResource(Resource.Drawable.ExpanderOpen);
            // Here is where you want to call 'Providers Changed' ?? 
            await SmartUtilityV2022.UtilitySuppliersChanged_Actual(ourviewmodel,
                                                                    utilityviewmodel);

            return;
        }
#endif

#if ANDROIDX
        internal static void OpenUtilitySuppliers(
                                            UtilityViewModel utilityviewmodel,
                                            string prompt,
                                            Action<string> set_text,
                                            Action<Android.Graphics.Color> set_colour)
        {

            SuppliersMultiSelect(
                                    utilityviewmodel,
                                    utilityviewmodel.UtilitySuppliersList.ToArray(),
                                    new UtilitySupplierDropdown(),
                                    (val) =>
                                    {
                                        string contentChosen = string.Empty;
#if UWP || WINUI
                                        Brush colourChosen = new SolidColorBrush(Colors.Black);
#endif
#if ANDROIDX
                                        Android.Graphics.Color colourChosen = Android.Graphics.Color.Black;
#endif
                                        UtilityViewModel.SupplierItem supplier_item = new UtilityViewModel.SupplierItem();
                                        int index = 0;
                                        foreach (dynamic value_item in val)
                                        {
                                            //UtilityViewModel.SupplierItem item = value_item as UtilityViewModel.SupplierItem;
                                            if (value_item.IsChecked)
                                            {
                                                contentChosen = value_item.Content;
                                                colourChosen = value_item.Colour;
                                                utilityviewmodel.UtilitySuppliersList[index].IsChecked = false;
                                                utilityviewmodel.SupplierSelectedIndex = index;
                                                //supplier_item = utilityviewmodel.UtilitySuppliersList[index];
                                                break;
                                            };
                                            index++;
                                        }
                                        if (string.IsNullOrEmpty(contentChosen))
                                        {
                                            contentChosen = prompt;
                                        }
                                        else
                                        {
                                            set_text(contentChosen);
                                            set_colour(colourChosen);

                                            // Here is where you want to call 'Suppliers Changed' ?? 
                                            //await SmartRoutinesV2018.Utility_SuppliersChanged_Actual(ourviewmodel,
                                            //                                                    utilityviewmodel,
                                            //                                                    supplier_item);

                                        }
                                    });
            return;
        }
#endif

#if ANDROIDX
        internal static void SuppliersMultiSelect(

                UtilityViewModel utilityviewmodel,
                dynamic[] selections,
                PopupWindow multiSelectPage,
                Action<dynamic[]> afterHideCallback)
        {
            utilityviewmodel.UtilitySupplier_Selections = selections.Clone() as dynamic[];

            Android.Widget.ListView abc = null;
            foreach (dynamic xyz in selections)
            {
                Android.Widget.ListView ray = xyz as Android.Widget.ListView;
                abc.AddChildrenForAccessibility((IList<Android.Views.View>)ray);
            }
            GravityFlags gflags = new GravityFlags();
            multiSelectPage.ShowAtLocation(abc, gflags, 0, 0);

            //if (utilityviewmodel.SupplierSelectedIndex != -1)
            //{
            //    object sender = new object();
            //    EventArgs e = new EventArgs();
            //    OnSupplierItemTapped(sender, e);
            //}
            //else
            //{
            utilityviewmodel.SupplierPopupVisible = true;
            //}
            return;
        }
#endif
    }
}