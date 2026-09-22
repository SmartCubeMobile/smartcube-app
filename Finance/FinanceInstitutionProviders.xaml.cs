#if WINFORMS
using System.Windows.Forms;
using static SmartCubeMobile.FinanceViewModel;
#endif

#if WPF
//using OpenQA.Selenium.DevTools.V124.Emulation;
using System.Windows;
using System.Windows.Controls.Primitives;
#endif

#if UWP
using Windows.UI.Xaml;
using System.Collections.Generic;
using System;
using Windows.UI.Xaml.Controls;
using System.Linq;
#endif

#if WINUI
using System.Collections.Generic;
using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Linq;
#endif

#if ANDROIDX
using Android.Content;
using Android.Views;
using static SmartCubeMobile.SmartUsers;
#endif

namespace SmartCubeMobile
{
    // <summary>
    // Interaction logic for FinanceInstitutionProviders.xaml
    // </summary>
#if WINFORMS
    public partial class FinanceInstitutionProviders : Form
#endif
#if WPF
    public partial class FinanceInstitutionProviders : Popup
#endif
#if UWP || WINUI
    public partial class FinanceInstitutionProviders : ContentDialog
#endif
#if ANDROIDX
    public partial class FinanceInstitutionProviders : PopupWindow
#endif
#if SMARTMAUI
    public partial class FinanceInstitutionProviders : ContentPage
#endif
    {
        //internal short institution_code = 0;
        //internal string institution_name = "";
        internal FinanceInstitutionProviders(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            FinanceView components,
                                            short institution_code,
                                            short brand_code,
                                            short loginMethod,
                                            string institution_name)
        //List<SmartFinance.BrandLoginsView> brandLoginsList)

        {
#if WPF || UWP || WINUI || SMARTMAUI
            InitializeComponent();
#endif
#if WPF || SMARTMAUI
            FrontEndGUI.SetFinanceDataContext(this, financeviewmodel);
            //institution_code = INSTITUTION_CODE;
            //institution_name = institution_namex;
#endif

#if WPF || UWP || WINUI
            //InstitutionProvidersListView.SelectionChanged += new SelectionChangedEventHandler((s, e) => ListView_SelectionChanged(s, e, ourviewmodel, financeviewmodel, components));
#endif

#if ANDROIDX
            View view = ourviewmodel.activity.LayoutInflater.Inflate(Resource.Layout.FinanceInstitutionProviders, null);
            Button AddButton = view.FindViewById<Button>(Resource.Id.AddButton);
            Button PrintButton = view.FindViewById<Button>(Resource.Id.PrintButton);

            //TextView BrandName = view.FindViewById<TextView>(Resource.Id.brandName);
            //TextView Description = view.FindViewById<TextView>(Resource.Id.description);            
#endif
            //financeviewmodel.FinanceBrandLoginsViewList = brandLoginsList;

#if WPF || UWP || WINUI

            AddButton.Click += new RoutedEventHandler((s, e) => OnAddButtonClick(s, e, ourviewmodel, financeviewmodel, components, institution_name, institution_code, brand_code, loginMethod));

            PrintButton.Click += new RoutedEventHandler((s, e) => OnPrintButtonClick(s, e, ourviewmodel, financeviewmodel));

            //CustomerNumber.GotFocus += OnFocus_CustomerNumber;
            //DateOfBirth.GotFocus += OnFocus_DateOfBirth;
            //Passcode.GotFocus += OnFocus_Passcode;
#endif
#if ANDROIDX
            AddButton.Click += new EventHandler((s, e) => OnAddButtonClick(s, e, ourviewmodel, financeviewmodel, components, institution_name, institution_code, brand_code, loginMethod));

            PrintButton.Click += new EventHandler((s, e) => OnPrintButtonClick(s, e, ourviewmodel, financeviewmodel));

            //CustomerNumber.FocusChange += OnFocus_CustomerNumber;
            //DateOfBirth.FocusChange += OnFocus_DateOfBirth;
            //Passcode.FocusChange += OnFocus_Passcode;
#endif
            return;
        }

#if WINFORMS
        internal void OnAddButtonClick(object sender, EventArgs e)
#endif
#if WPF || UWP || WINUI
        internal void OnAddButtonClick(object sender, RoutedEventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        FinanceView components,
                                        string institutionName,
                                        short institutionCode,
                                        short brandCode,
                                        short loginMethod)
#endif
#if ANDROIDX || SMARTMAUI
        internal void OnAddButtonClick(object sender, EventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        FinanceView components,
                                        string institutionName,
                                        short institutionCode,
                                        short brandCode,
                                        short loginMethod)
#endif
        {
            // In memory of Joy - 8th July 1926 - 25th June 2021 - kind, warm-hearted, generous to a fault                
            // You bring me luck, Joy ...  You certainly do, dear..
            if (sender != null)
            {
                string maxCategories = "";
                //foreach (SmartFinance.BrandsView bview in financeviewmodel.brandsview_found)
                //{
                //    maxCategories += bview.CATEGORY_CODE.ToString();
                //};

                // Check to see if there are any categories 'free'
                // Display the ones 'we already have'
                //Setup(financeviewmodel, financeviewmodel.loginInfo.CATEGORIES, true);

                // Find all logins with the same Institution and Brand code
                //string maxCategories = SmartSpikeFinanceV2017.Finance_Find_BrandCategories(ourviewmodel,
                //                                                                            financeviewmodel,
                //                                                                            financeviewmodel.loginInfo.INSTITUTION_CODE);
                foreach (SmartFinance.Logins login in financeviewmodel.PLO.finance_loginsList)//FinanceBrandLoginsViewList)
                {
                    if (institutionCode == login.INSTITUTION_CODE &&
                        brandCode == login.BRAND_CODE)
                    {
                        //maxCategories = maxCategories.Replace(login.CATEGORIES, "");
                        if (maxCategories == "")
                        {
                            break;
                        }
                    }
                }
                if (maxCategories == "")
                {
#if WINFORMS || WPF || UWP || WINUI
                    ToastBox toast = new ToastBox(components,
                                                    "No spare categories",
                                                    SmartParametersV2016.toastDuration,
                                                    ourviewmodel.currentWidth);
#endif
#if ANDROIDX
                Toast.MakeText(MainMeter.ourviewmodel.activity,
                                                        "No spare categories", 
                                                        ToastLength.Short).Show();
#endif

                    return;
                }

                SmartFinance.Logins newLogin = SmartFinanceV2025.CreateEmptyLogin(financeviewmodel,
                                                        ourviewmodel.UserName,
                                                        institutionCode,
                                                        brandCode,
                                                        maxCategories);// brand_code);
                //  THIS NEEDS TESTING AGAIN!!!

                // Might be able to pass the BLANK LoginInfo CATEGORIES into here??
                List<SmartFinance.BrandConnection> params_found =
                    SmartSpikeFinanceV2017.Finance_Find_BrandConnections(ourviewmodel,
                                                                        financeviewmodel,
                                                                        institutionCode,
                                                                        brandCode,
                                                                        "");
#if WINFORMS || WPF || UWP
                //MainMeter.financeviewmodel.FCLPopup = new FinanceConnectionLogins(ourviewmodel,
                //                                                financeviewmodel,
                //                                                institutionCode,
                //                                                brandCode,
                //                                                loginMethod);
#endif
            }
#if WPF
            ProviderTitle.Focus();
#endif
            return;
        }

        //#if WINFORMS || WPF || UWP || WINUI
        //        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e,
        //                                                MainViewModel ourviewmodel,
        //                                                FinanceViewModel financeviewmodel,
        //                                                FinanceView components)
        //        {
        //            ListView myListView = sender as ListView;
        //            SmartFinance.BrandLoginsView lvitem = myListView.SelectedItem as SmartFinance.BrandLoginsView;
        //            if (lvitem != null)
        //            {
        //                //bool status = false;
        //                string brand_name = lvitem.BRAND_NAME;
        //                short institution_code = Convert.ToInt16(lvitem.INSTITUTION_CODE);
        //                short brand_code = Convert.ToInt16(lvitem.BRAND_CODE);
        //                string connectionId = Convert.ToString(lvitem.LOGIN_METHOD);
        //                List<SmartFinance.Logins> logins_found = new List<SmartFinance.Logins>();
        //                //    = SmartSpikeFinanceV2017.Finance_Find_BrandLoginsX(ourviewmodel,
        //                //                                                                financeviewmodel,
        //                //                                                                SmartParametersV2016.activeFlag,
        //                //                                                                institution_code,
        //                //                                                                brand_code,
        //                //                                                                connectionId);
        //                if (logins_found.Count == 0)
        //                {
        //                    return;
        //                    //string categories = "";
        //                    //foreach (SmartFinance.BrandsView bview in financeviewmodel.brandsview_found)
        //                    //{
        //                    //    categories += bview.CATEGORY_CODE.ToString();
        //                    //};
        //                    //SmartFinanceV2025.CreateEmptyLogin(financeviewmodel,
        //                    //                                    ourviewmodel.UserName,
        //                    //                                    institution_code,
        //                    //                                    brand_code,
        //                    //                                    categories);                    
        //                }
        //                else
        //                {
        //                    // Cannot change Parameter1
        //                    financeviewmodel.ConnectionIdReadOnly = true;

        //                    financeviewmodel.loginInfo = logins_found.First();                    
        //                    // Note: no UDPRN or RANDOMKEYs !!
        //                }

        //                // Previous FindInstitutionConnection COULD have returned false
        //                // If it returned TRUE then prompt for the Username password
        //                // If it returned FALSE then there is no data and no need for a Username password prompt

        //                // Pop-up the Finance Login Details window ... whether the info
        //                // was found - or not.  If it wasn't found you need to collect it
        //                // if it was found, you will be interested in changing it.

        //                List<SmartFinance.BrandConnection> params_found = SmartSpikeFinanceV2017.Finance_Find_BrandConnections(ourviewmodel,
        //                                                                                                                    financeviewmodel,
        //                                                                                                                    lvitem.INSTITUTION_CODE,
        //                                                                                                                    lvitem.BRAND_CODE,
        //                                                                                                                    lvitem.CATEGORIES);

        //#if WINFORMS
        //                financeviewmodel.newlogin_info = new SmartFinance.Logins();

        //                financeviewmodel.FLDConnectionPage = new FinanceLoginDetails(financeviewmodel,
        //                                                                params_found,
        //                                                                (logins_found.Count == 0 ? false : true),
        //                                                                //rf login_info, // Does this work?? It does now!
        //                                                                ourviewmodel.PassWord);
        //                financeviewmodel.FLDConnectionPage.Text = institution_name;
        //                DialogResult dialogresult = financeviewmodel.FLDConnectionPage.ShowDialog();
        //                if (dialogresult == DialogResult.Yes)
        //                {
        //                    string errorMessage;
        //                    await Finance_LoginSetup(ourviewmodel,
        //                                                financeviewmodel,
        //                                                financeviewmodel.newlogin_info,
        //                                                financeviewmodel.loginInfo,
        //                                                // One day we will need this to show we are the same person who started the porgram
        //                                                //ourviewmodel.PassWord, 
        //                                                (logins_found.Count == 0 ? false : true),
        //                                                em => errorMessage = em);
        //                }
        //                else
        //                {
        //                    if (dialogresult == DialogResult.Cancel)
        //                    {
        //                        // Do nothing
        //                    }
        //                }
        //                // Popup is already closed here
        //                financeviewmodel.FLDConnectionPage.Dispose();
        //#endif

        //#if WPF || UWP || WINUI
        //                //  THIS NEEDS TESTING AGAIN!!!
        //                financeviewmodel.FLDConnectionPage = new FinanceLoginDetails(ourviewmodel,
        //                                                                financeviewmodel,
        //                                                                components,
        //                                                                params_found,
        //                                                                //ourviewmodel.PassWord,
        //                                                                brand_name,
        //                                                                institution_code,
        //                                                                brand_code);
        //#endif
        //#if ANDROIDX
        //                //  THIS NEEDS TESTING AGAIN!!!
        //                List<SmartFinance.Brands> brand_name = SmartSpikeFinanceV2017.Finance_Lookup_BrandName(ourviewmodel,
        //                                                                                financeviewmodel,
        //                                                                                //financeviewmodel.category_code,
        //                                                                                "",
        //                                                                                institution_code,
        //                                                                                brand_code);

        //                if (brand_name.Count > 0)
        //                {
        //                    financeviewmodel.FLDConnectionPage = new FinanceLoginDetails(financeviewmodel,
        //                                                                params_found,
        //                                                                logins_found.Count, // == 0 ? false : true),
        //                                                                                    //rf login_info, // Does this work?? It does now!
        //                                                                ourviewmodel.PassWord,
        //                                                                brand_name.First().BRAND_NAME);
        //#endif
        //#if ANDROIDX
        //                GravityFlags gflags = new();
        //                financeviewmodel.ConnectionPage.ShowAtLocation(contentpage, gflags, 0, 0);
        //#endif
        //#if WPF || UWP || WINUI

        //            }
        //#endif
        //#if ANDROIDX
        //            }
        //#endif
        //            return;
        //        }
        //#endif

        internal void PopupClosing(object sender, EventArgs e)
        {
            //    if (e.Key == Key.Return ||
            //        e.Key == Key.Enter)
            //    {
            //        e.Handled = true;
            //    //    //await SubmitClick(sender, e);
            //    }
            //
            // this.IsOpen = false; ????
            return;
        }

#if WPF
        internal void MouseLeaving(object sender, EventArgs e)
        {
            // For the time being so we can fucking TEST!!!
            this.IsOpen = false;
            return;
        }
#endif
#if WINFORMS
        internal void OnPrintButtonClick(object sender, EventArgs e)
#endif
#if WPF || WINUI || UWP
        internal static void OnPrintButtonClick(object sender, RoutedEventArgs e, MainViewModel ourviewmodel, FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX || SMARTMAUI
        internal static void OnPrintButtonClick(object sender, EventArgs e, MainViewModel ourviewmodel, FinanceViewModel financeviewmodel)
#endif
        {
            // In memory of Joy - 8th July 1926 - 25th June 2021 - kind, warm-hearted, generous to a fault                
            // You bring me luck, Joy ...  You certainly do, dear..
            if (sender != null)
            {
                financeviewmodel.PrintButtonEnabled = false;
                string title = financeviewmodel.TransactionsTitle;
                if (!ourviewmodel.subscriber)
                {
                    title += " - " + SmartParametersV2016.KEASDONENERGYLTD;
                }
                financeviewmodel.TransactionsListTitle = title;
#if WPF || UWP || WINUI
                // https://www.c-sharpcorner.com/uploadfile/f2e803/basic-pdf-creation-using-itextsharp-part-ii/
                // With eternal thanks to Micke Blomquist

                string filePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                //filePath = System.IO.Path.Combine(filePath, "SmartCube");
                filePath = System.IO.Path.Combine(filePath, "Providers.pdf");
#endif
#if ANDROIDX
                string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Transactions.pdf");
#endif
#if SMARTMAUI
                string filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Providers.pdf");
#endif
                SmartRoutinesV2018.CreateFinanceList(ourviewmodel,
                                                    financeviewmodel,
                                                    "Providers",
                                                    ourviewmodel.subscriber,
                                                    filePath,
                                                    title);
#if WPF || UWP || WINUI
                // Cannot use DependencyService in WPF ... or at least I
                // can't figure out how the fucking thing works
                // Declare an interface instance.
                IPrint print_object = new PrintService();
                // Call the member.
                print_object.Print(filePath, financeviewmodel);
#endif
#if ANDROIDX
                // This is where the shit USED to happen ... !!!
                //PdfSharpCore.Pdf.PdfDocument printDocument = PDFManager.GeneratePDFFromView(mainGrid);                
                try
                {
                    Stream printStream = new MemoryStream(File.ReadAllBytes(filePath));
                    // Miss out all the interface shit ...
                    PrintService.Print(printStream, filePath);
                }
                catch (Exception exc)
                {
                    financeviewmodel.errorMessage = exc.Message;
                }
#endif
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
        private void OnAddClicked(object sender, EventArgs e)
        {
        }
#endif

#if SMARTMAUI
        private void OnPrintClicked(object sender, EventArgs e)
        {
        }
#endif
    }
}