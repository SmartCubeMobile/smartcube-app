//using System.Net;           // <== THIS IS DIFFERENT FOR LIGHSILVER than for Windows FORMS!!!
// What a complete pile of absolute fucking bollocks this shit is

//using System.Net.Http;        // No more of this absolute bollocks shit
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Xml;

#if WINFORMS
using System.Windows;
using SmartDashboard;
using OxyPlot.WindowsForms;
using OxyPlot;
using MoreLinq;
using System.Net.Http;
using System.Security.Cryptography;
using Jose;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
#endif

#if WPF
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using OxyPlot;
using OxyPlot.Wpf;
using MoreLinq;
// Crypto shit
using Microsoft.Web.WebView2.Wpf;
using System.Collections.Concurrent;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.UI.Input;
using iText.Layout.Element;
//using Microsoft.Web.WebView2.Core.DevToolsProtocolExtension;
#endif

#if WINUI
using OxyPlot.SkiaSharp;
using System.Collections.Generic;
using System;
using System.Threading;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Markup;
using System.Linq;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI;
using Windows.Storage.Streams;
using OxyPlot;
using iText.StyledXmlParser.Jsoup.Nodes;
using Microsoft.UI.Input;
#endif

#if ANDROIDX
using Android.Graphics;
using OxyPlot.Xamarin.Android;
using OxyPlot;
using Android.Views;
using AndroidX.Lifecycle;
using AndroidX.ViewPager2.Adapter;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Tabs;
using AndroidX.RecyclerView.Widget;
using AndroidX.AppCompat.App;
using Android.Content;
using System.Diagnostics.CodeAnalysis;
using Android.Webkit;
#endif

#if SMARTMAUI
using CommunityToolkit.Maui.Views;
//using OxyPlot;
//using OxyPlot.Maui.Skia;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;

#endif

namespace SmartCubeMobile
{
#if ANDROIDX
    public static class FinanceViewModelStore
    {
        private static readonly Dictionary<string, (MainViewModel main, FinanceViewModel finance)> store = new();
        public static void Add(string key, MainViewModel main, FinanceViewModel finance)
        {
            store[key] = (main, finance);
        }
        public static void Add(string key, FinanceViewModel finance)
        {
            store[key] = (null, finance);
        }
        public static (MainViewModel main, FinanceViewModel finance)? Get(string key)
        {
            return store.TryGetValue(key, out var value) ? value : null;
        }
        public static void Remove(string key)
        {
            store.Remove(key);
        }
    }
    
    public class FinanceStrategy : Java.Lang.Object, Google.Android.Material.Tabs.TabLayoutMediator.ITabConfigurationStrategy//TabLayoutMediator.ITabConfigurationStrategy
    {
        internal static readonly List<string> financefragmentTitles = new()
        { 
            "Transactions", 
            "Charts", 
            "WebView" 
        };
        private readonly ViewPager2 _viewPager;

        public FinanceStrategy(ViewPager2 viewPager)
        {
            _viewPager = viewPager;
        }
        public void OnConfigureTab(TabLayout.Tab tab, int position)
        {
            tab.SetText(financefragmentTitles[position]);
        }

        // Add GetCurrentItem
        public int GetCurrentItem()
        {
            return _viewPager.CurrentItem;
        }

        // Add SetCurrentItem
        public void SetCurrentItem(int index, bool animated = false)
        {
            _viewPager.SetCurrentItem(index, animated);
        }
    }
    public class FinanceViewPager2Adapter : FragmentStateAdapter
    {
        private readonly int itemCount;
        public FinanceWebViewFragment FinanceWebViewFragmentInstance { get; private set; }
        public string PreloadedUrl { get; set; }

        private MainViewModel ourviewmodel;
        private FinanceViewModel financeviewmodel;
        
        public FinanceViewPager2Adapter(AndroidX.Fragment.App.FragmentManager fragmentManager, Lifecycle lifecycle, int itemCount, MainViewModel ourvm, FinanceViewModel financevm) : base(fragmentManager, lifecycle)
        {
            this.itemCount = itemCount;
            this.ourviewmodel = ourvm;
            this.financeviewmodel = financevm;            
        }
        public override int ItemCount => itemCount;
        public override AndroidX.Fragment.App.Fragment CreateFragment(int position)
        {
            AndroidX.Fragment.App.Fragment financefragment = new AndroidX.Fragment.App.Fragment();
            if (position == 2 && PreloadedUrl == null)
            {
                PreloadedUrl = "http://localhost";
            }
            return position switch
            {
                
                2 => FinanceWebViewFragment.NewInstance(PreloadedUrl, ourviewmodel, financeviewmodel),
                0 => FinanceTransactionsFragment.NewInstance(financeviewmodel),
                1 => FinanceChartsFragment.NewInstance(financeviewmodel),
                _ => new AndroidX.Fragment.App.Fragment(),
            };
        }
    }
    public class FinanceCurrencyStrategy : Java.Lang.Object, TabLayoutMediator.ITabConfigurationStrategy
    {
        private readonly FinanceViewModel financeviewmodel;

        public FinanceCurrencyStrategy(FinanceViewModel fvm)
        {
            financeviewmodel = fvm;
        }
        public void OnConfigureTab(TabLayout.Tab tab, int position)
        {
            tab.SetText(financeviewmodel.TabTitles[position]);
        }
    }
    
    public class FinanceCurrencyPager2Adapter : FragmentStateAdapter
    {
        private readonly int itemCount;
        private readonly FinanceViewModel financeviewmodel;
        public FinanceCurrencyPager2Adapter(
            AndroidX.Fragment.App.FragmentManager fragmentManager,
            Lifecycle lifecycle,
            int itemCount,
            FinanceViewModel financevm) : base(fragmentManager, lifecycle)
        {
            this.itemCount = itemCount;
            financeviewmodel = financevm;
        }

        public override int ItemCount => itemCount;

        public override AndroidX.Fragment.App.Fragment CreateFragment(int position)
        {
            return new FinanceCurrenciesFragment(financeviewmodel.FinanceTotals, position);
        }

        public string GetPageTitle(int position)
        {
            return financeviewmodel.TabTitles[position];
        }
    }

    public class FinanceCurrencyTabAdapter : RecyclerView.Adapter
    {
        private List<string> _tabs;
        private int _selectedPosition = 0;
        private readonly Action<int> _onTabClick;
        public FinanceCurrencyTabAdapter(List<string> tabs, Action<int> onTabClick)
        {
            _tabs = tabs;
            _onTabClick = onTabClick;
        }
        public int SelectedPosition
        {
            get => _selectedPosition;
            set
            {
                // Notify the adapter of the old and new selected positions to refresh
                NotifyItemChanged(_selectedPosition);
                _selectedPosition = value;
                NotifyItemChanged(_selectedPosition);
            }
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ItemTab, parent, false);
            return new TabViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            TabViewHolder tabHolder = holder as TabViewHolder;
            tabHolder.Bind(_tabs[position], position == _selectedPosition);

            tabHolder.ItemView.Click += (s, e) =>
            {
                int previousPosition = _selectedPosition;
                _selectedPosition = position;
                NotifyItemChanged(previousPosition);
                NotifyItemChanged(_selectedPosition);
                _onTabClick(position);
            };
        }

        public override int ItemCount => _tabs.Count;
        private class TabViewHolder : RecyclerView.ViewHolder
        {
            private TextView _tabText;
            public TabViewHolder(View itemView) : base(itemView)
            {
                _tabText = itemView.FindViewById<TextView>(Resource.Id.tabText);
            }
            public void Bind(string title, bool isSelected)
            {
                _tabText.Text = title;
                _tabText.SetTextColor(isSelected ? Color.Black : Color.Gray);
                _tabText.SetTypeface(null, isSelected ? TypefaceStyle.Bold : TypefaceStyle.Normal);
            }
        }
    }

    public class CustomOnPageChangeCallback : ViewPager2.OnPageChangeCallback
    {
        private readonly FinanceCurrencyTabAdapter _tabAdapter;
        private readonly RecyclerView _tabRecyclerView;

        public CustomOnPageChangeCallback(FinanceCurrencyTabAdapter tabAdapter, RecyclerView tabRecyclerView)
        {
            _tabAdapter = tabAdapter;
            _tabRecyclerView = tabRecyclerView;
        }

        public override void OnPageSelected(int position)
        {
            _tabAdapter.SelectedPosition = position;
            _tabAdapter.NotifyDataSetChanged();
            _tabRecyclerView.SmoothScrollToPosition(position);
        }
    }

    public class ItemAdapter : RecyclerView.Adapter
    {
        private readonly List<SmartFinance.BrandsView> _items;
        private readonly Action<int> _onItemClick;
        
        public ItemAdapter(List<SmartFinance.BrandsView> items, Action<int> onItemClick)
        {
            _items = items;
            _onItemClick = onItemClick;
        }

        public override int ItemCount => _items.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is ItemViewHolder viewHolder)
            {
                var item = _items[position];
                viewHolder.BrandNameTextView.Text = item.BRAND_NAME;
                viewHolder.DescriptionTextView.Text = item.DESCRIPTION;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FinanceInstitutionProvidersItem, parent, false);            
            return new ItemViewHolder(itemView, _onItemClick);
        }

        private class ItemViewHolder : RecyclerView.ViewHolder
        {
            public TextView BrandNameTextView { get; }
            public TextView DescriptionTextView { get; }
            public ItemViewHolder(View itemView,Action<int> clickListener) : base(itemView)
            {
                BrandNameTextView = itemView.FindViewById<TextView>(Resource.Id.brandName);
                DescriptionTextView = itemView.FindViewById<TextView>(Resource.Id.description);
                // Set up item click event
                itemView.Click += (sender, e) =>
                {
                    // Use BindingAdapterPosition instead of AdapterPosition
                    int position = BindingAdapterPosition;
                    // Ensure the position is valid
                    if (position != RecyclerView.NoPosition)
                    {
                        clickListener(position);
                    }
                };
            }
        }
    }
#endif
    public class SmartFinanceV2025
    {
        internal static void LoadConnections(FinanceViewModel financeviewmodel, short InstitutionCode, string InstitutionName)
        {
            financeviewmodel.ConnectionsViewList = new List<SmartFinance.Connections>();
            foreach (SmartFinance.Connections connection in financeviewmodel.PLO.finance_connectionsList)
            {
                if (connection.INSTITUTION_CODE == InstitutionCode)
                {
                    var brand = financeviewmodel.CreatedBrands.FirstOrDefault(b => b.BRAND_CODE == connection.BRAND_CODE);
                    if (brand != null)
                    {
                        connection.InstitutionName = InstitutionName;
                        connection.BrandName = brand.BrandName;
                        connection.IsNew = false;
                        connection.IsLoginMethodLocked = true;
#if WPF || SMARTMAUI
                        connection.Parameter1Visibility = Visibility.Visible;
                        connection.Parameter2Visibility = Visibility.Visible;
                        connection.Parameter3Visibility = Visibility.Visible;
#endif
#if WINUI
                        connection.Parameter1Visibility = Visibility.Visible;
                        connection.Parameter2Visibility = Visibility.Visible;
                        connection.Parameter3Visibility = Visibility.Visible;
#endif
#if ANDROIDX
                        connection.Parameter1Visibility = ViewStates.Visible;
                        connection.Parameter2Visibility = ViewStates.Visible;
                        connection.Parameter3Visibility = ViewStates.Visible;
#endif
                        financeviewmodel.ConnectionsViewList.Add(connection);
                    }
                }
            }
            return;
        }

        internal static void LoadLogins(FinanceViewModel financeviewmodel, short InstitutionCode, string InstitutionName)
        {
            financeviewmodel.LoginsViewList = new List<SmartFinance.Logins>();
            foreach (SmartFinance.Logins login in financeviewmodel.PLO.finance_loginsList)
            {
                if (login.INSTITUTION_CODE == InstitutionCode)
                {
                    var brand = financeviewmodel.CreatedBrands.FirstOrDefault(b => b.BRAND_CODE == login.BRAND_CODE);
                    if (brand != null)
                    {
                        login.InstitutionName = InstitutionName;
                        login.BrandName = brand.BrandName;
                        login.IsNew = false;
                        login.IsLoginMethodLocked = true;
                        login.Parameter1 = FindParameter1(financeviewmodel,
                                            login.INSTITUTION_CODE,
                                            login.BRAND_CODE,
                                            login.LOGIN_METHOD);
                        financeviewmodel.LoginsViewList.Add(login);
                    }
                }
            }
            return;
        }

        internal static string FindParameter1(FinanceViewModel financeviewmodel,
                                        short InstitutionCode,
                                        short BrandCode,
                                        int LoginMethod)
        {
            return financeviewmodel.ConnectionsViewList
                .FirstOrDefault(c =>
                    c.INSTITUTION_CODE == InstitutionCode &&
                    c.BRAND_CODE == BrandCode &&
                    c.LOGIN_METHOD == LoginMethod)
                ?.Parameter1;
        }
        internal static async Task<SmartFinance.Connections> AddConnectionCoreAsync(
                                                                SignInViewModel signinviewmodel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                short DefaultInstitutionCode,
                                                                string DefaultInstitutionName
#if WPF
                                                                , Window myWindow
#endif
#if WINUI
                                                                , ContentDialog myWindow
#endif
#if ANDROIDX
                                                                , Context context
#endif
#if SMARTMAUI
                                                                , ContentPage myWindow
#endif
                                                            )
        {
            short institutionCode = DefaultInstitutionCode;

            // 🔍 Only brands with free login methods
            List<BrandItem> availableBrands =
                GetBrandsWithAvailableMethods(financeviewmodel, institutionCode);
            if (!availableBrands.Any())
            {
#if WPF
                new ToastWindow(myWindow,
                    SignIn.BesetByChimps(signinviewmodel, "NoProvidersLoginMethods"),ToastType.Warning).Show();
#endif
#if WINUI
                new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "NoProvidersLoginMethods"),ToastType.Warning).Show();
#endif
#if ANDROIDX
                ToastService.Warning(context,
                    SignIn.BesetByChimps(signinviewmodel, "NoProvidersLoginMethods"));
#endif
#if SMARTMAUI
                new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "NoProvidersLoginMethods"), ToastType.Warning);
#endif
                return null;
            }

            bool singleBrand = availableBrands.Count == 1;

            if (singleBrand)
            {
                BrandItem onlyBrand = availableBrands[0];

                SmartFinance.Connections newConnection =
                    new SmartFinance.Connections
                    {
                        IsPendingCompletion = true,
                        IsNew = false,
                        INSTITUTION_CODE = institutionCode,
                        BRAND_CODE = onlyBrand.BRAND_CODE,
                        LOGIN_METHOD = 0,
                        ACTIVE_FLAG = SmartParametersV2016.activeFlag,
                        BrandName = onlyBrand.BrandName,
                        AvailableBrands = availableBrands,
                        SelectedBrand = onlyBrand,
                        Parameter1 = "",
                        Parameter2 = "",
                        Parameter3 = "",
                        Updated = false,
                        Delete = false
                    };

                // 🔐 Auto-assign provider + login method if only ONE option
                List<int> methods =
                    ConnectionsGetAvailableMethods(
                        financeviewmodel,
                        institutionCode,
                        onlyBrand.BRAND_CODE);

                if (methods.Count == 0)
                {
#if WPF
                    new ToastWindow(myWindow,
                        SignIn.BesetByChimps(signinviewmodel, "NoFreeLoginMethods"), ToastType.Warning).Show();
#endif
#if WINUI
                    new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "NoFreeLoginMethods"), ToastType.Warning).Show();
#endif
#if ANDROIDX
                    ToastService.Warning(context,
                    SignIn.BesetByChimps(signinviewmodel, "NoFreeLoginMethods"));
#endif
#if SMARTMAUI
                    new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "NoFreeLoginMethods"), ToastType.Warning);
#endif
                    return null;
                }

                if (methods.Count == 1)
                {
                    newConnection.LOGIN_METHOD = methods[0];
                    newConnection.IsLoginMethodLocked = true;
                    newConnection.IsPendingCompletion = false;

                    financeviewmodel.ConnectionsViewList.Add(newConnection);
                    financeviewmodel.SelectedConnection = newConnection;

                    if (!await CommitConnectionAsync(
                            signinviewmodel,
                            ourviewmodel,
                            financeviewmodel,

                            newConnection,
#if WPF
                            myWindow,
#endif
#if WINUI
                            myWindow,
#endif
#if ANDROIDX
                            context,
#endif
#if SMARTMAUI
                            myWindow,
#endif
                            update: false,
                            delete: false))
                    {
                        return null;
                    }

                    return newConnection;
                }

                // If multiple methods exist, return connection for UI to complete
                financeviewmodel.ConnectionsViewList.Add(newConnection);
                financeviewmodel.SelectedConnection = newConnection;

                return newConnection;
            }
            else
            {
                // More than one brand
                SmartFinance.Connections newConnection =
                    new SmartFinance.Connections
                    {
                        IsPendingCompletion = true,
                        IsNew = true,
                        INSTITUTION_CODE = institutionCode,
                        BRAND_CODE = 0,
                        LOGIN_METHOD = 0,
                        ACTIVE_FLAG = SmartParametersV2016.activeFlag,
                        BrandName = "",
                        AvailableBrands = availableBrands,
                        SelectedBrand = null,
                        Parameter1 = "",
                        Parameter2 = "",
                        Parameter3 = "",
                        Updated = false,
                        Delete = false
                    };

                financeviewmodel.ConnectionsViewList.Add(newConnection);
                financeviewmodel.SelectedConnection = newConnection;

                return newConnection;
            }
        }

        internal static async Task<bool> AddLoginCoreAsync(SignInViewModel signinviewmodel,
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            short DefaultInstitutionCode,
                                                            string DefaultInstitutionName
#if WPF
                                                            , Window myWindow
#endif
#if WINUI
                                                            , ContentDialog myWindow
#endif
#if ANDROIDX
                                                            , Context context
#endif
#if SMARTMAUI
                                                            , ContentPage myWindow
#endif
                                                            )
        {
            short institutionCode = DefaultInstitutionCode;

            var validConnections = financeviewmodel.ConnectionsViewList
                .Where(c =>
                    c.INSTITUTION_CODE == institutionCode &&
                    !c.IsPendingCompletion &&
                    c.BRAND_CODE != 0 &&
                    c.LOGIN_METHOD != 0 &&
                    !string.IsNullOrWhiteSpace(c.Parameter1))
                .ToList();
            if (!validConnections.Any())
            {
#if WPF
                new ToastWindow(myWindow, SignIn.BesetByChimps(signinviewmodel, "NoValidConnections"), ToastType.Warning).Show();
#endif
#if WINUI
                new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "NoValidConnections"), ToastType.Warning).Show();
#endif
#if ANDROIDX
                ToastService.Warning(context,
                SignIn.BesetByChimps(signinviewmodel, "NoValidConnections"));
#endif
#if SMARTMAUI
                new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "NoValidConnections"), ToastType.Warning);
#endif
                return false;

            }

            // 2️⃣ Find first connection that does NOT already exist as a login
            var sourceConnection = validConnections.FirstOrDefault(conn =>
                !financeviewmodel.LoginsViewList.Any(l =>
                    l.BRAND_CODE == conn.BRAND_CODE &&
                    l.LOGIN_METHOD == conn.LOGIN_METHOD));
            if (sourceConnection == null)
            {
#if WPF
                new ToastWindow(myWindow, SignIn.BesetByChimps(signinviewmodel, "LoginExists"), ToastType.Warning).Show();
#endif
#if WINUI
                new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "LoginExists"), ToastType.Warning).Show();
#endif
#if ANDROIDX
                ToastService.Warning(context,
                SignIn.BesetByChimps(signinviewmodel, "LoginExists"));
#endif
#if SMARTMAUI
                new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "LoginExists"), ToastType.Warning);
#endif
                return false;
            }

            // 3️⃣ Get ALL possible login methods for this brand
            var methods = LoginsGetAvailableMethods(financeviewmodel,
                                                    sourceConnection.INSTITUTION_CODE,
                                                    sourceConnection.BRAND_CODE);
            if (!methods.Any())
            {
#if WPF
                new ToastWindow(myWindow, SignIn.BesetByChimps(signinviewmodel, "ProviderNoLoginMethods"), ToastType.Warning).Show();
#endif
#if WINUI
                new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "ProviderNoLoginMethods"), ToastType.Warning).Show();
#endif
#if ANDROIDX
                ToastService.Warning(context,
                SignIn.BesetByChimps(signinviewmodel, "ProviderNoLoginMethods"));
#endif
#if SMARTMAUI
                new ToastWindow(SignIn.BesetByChimps(signinviewmodel, "ProviderNoLoginMethods"), ToastType.Warning);
#endif
                return false;
            }

            // 4️⃣ Build new login
            SmartFinance.Logins newLogin = new SmartFinance.Logins
            {
                INSTITUTION_CODE = sourceConnection.INSTITUTION_CODE,
                BRAND_CODE = sourceConnection.BRAND_CODE,
                BrandName = sourceConnection.BrandName,
                Parameter1 = sourceConnection.Parameter1
            };

            if (methods.Count == 1)
            {
                // ✅ ONE possible login method → TEXT ONLY
                newLogin.LOGIN_METHOD = methods[0];
                newLogin.IsLoginMethodLocked = true;
                newLogin.UserFinalizedLoginMethod = true;
            }
            else
            {
                // Multiple methods → user must choose
                newLogin.LOGIN_METHOD = methods.First();
                newLogin.IsLoginMethodLocked = false;
                newLogin.UserFinalizedLoginMethod = false;
            }
            newLogin.ACTIVE_FLAG = SmartParametersV2016.activeFlag;

            financeviewmodel.SelectedLogin = newLogin;

            if (!await UpdateLoginCoreAsync(signinviewmodel, ourviewmodel, financeviewmodel, false, false
#if WPF
                                                                        , myWindow
#endif
#if WINUI
                                                                        , myWindow
#endif
#if ANDROIDX
                                                                        , context
#endif
#if SMARTMAUI
                                                                        , myWindow
#endif
                                                                        ))
            {
                return false;
            }
            financeviewmodel.LoginsViewList.Add(newLogin);
            return true; // newLogin;
        }

        internal static List<int> LoginsGetAvailableMethods(FinanceViewModel financeviewmodel,
                                                    short institutionCode,
                                                    short brandCode)
        {
            var allMethods = financeviewmodel.ConnectionsViewList
                .Where(c =>
                    c.INSTITUTION_CODE == institutionCode &&
                    c.BRAND_CODE == brandCode &&
                    !c.IsPendingCompletion &&              // 🔥 ONLY COMMITTED
                    c.LOGIN_METHOD != 0)
                .Select(c => c.LOGIN_METHOD)
                .Distinct();

            return allMethods
                .OrderBy(m => m)
                .ToList();
        }
        internal static List<BrandItem> GetBrandsWithAvailableMethods(FinanceViewModel financeviewmodel,
                                                                short institutionCode)
        {
            return financeviewmodel.CreatedBrands
                .Where(b =>
                    b.InstitutionCode == institutionCode &&
                    ConnectionsGetAvailableMethods(financeviewmodel, institutionCode, b.BRAND_CODE).Any())
                .Select(b => new BrandItem
                {
                    InstitutionCode = b.InstitutionCode,
                    BRAND_CODE = b.BRAND_CODE,
                    BrandName = b.BrandName
                })
                .OrderBy(b => b.BrandName)
                .ToList();
        }

        internal static List<int> ConnectionsGetAvailableMethods(FinanceViewModel financeviewmodel,
                                                        short institutionCode,
                                                        short brandCode)
        {
            List<int> allMethods = financeviewmodel.PLO.brand_connectionList
                .Where(c =>
                    c.INSTITUTION_CODE == institutionCode &&
                    c.BRAND_CODE == brandCode)
                .Select(c => c.LOGIN_METHOD)
                .Distinct().ToList();
            List<int> usedMethods = financeviewmodel.ConnectionsViewList
                .Where(c =>
                    c.INSTITUTION_CODE == institutionCode &&
                    c.BRAND_CODE == brandCode &&
                    !c.IsPendingCompletion &&              // 🔥 ONLY COMMITTED
                    c.LOGIN_METHOD != 0)
                .Select(c => c.LOGIN_METHOD)
                .Distinct().ToList();
            List<int> all = allMethods
                .Except(usedMethods)
                .OrderBy(m => m)
                .ToList();
            return all;
        }

        internal static async Task<bool> CommitConnectionAsync(
                                            SignInViewModel signinviewmodel,
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            SmartFinance.Connections connection,
#if WPF
                                            Window myWindow,
#endif
#if WINUI
                                            ContentDialog myWindow,
#endif
#if ANDROIDX
                                            Context context,
#endif
#if SMARTMAUI
                                            ContentPage myWindow,
#endif
                                            bool update,
                                            bool delete)
        {
            if (connection == null) return false;
            financeviewmodel.SelectedConnection = connection;
            return await UpdateConnectionCoreAsync(signinviewmodel,
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    update,
                                                    delete
#if WPF
                                                   ,myWindow
#endif
#if WINUI
                                                   ,myWindow
#endif
#if ANDROIDX
                                                   , context
#endif
#if SMARTMAUI
                                                   ,myWindow
#endif
                                                   );
        }

        internal static async Task<bool> CommitLoginAsync(
                                            SignInViewModel signinviewmodel,
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            SmartFinance.Logins login,
#if WPF
                                            Window myWindow,
#endif
#if WINUI
                                            ContentDialog myWindow,
#endif
#if ANDROIDX
                                            Context context,
#endif
#if SMARTMAUI
                                            ContentPage myWindow,
#endif

                                            bool update,
                                            bool delete)
        {
            if (login == null) return false;
            financeviewmodel.SelectedLogin = login;
            return await UpdateLoginCoreAsync(signinviewmodel,
                                                ourviewmodel,
                                                    financeviewmodel,
                                                    update,
                                                    delete
#if WPF
                                                   , myWindow
#endif
#if WINUI
                                                   , myWindow
#endif
#if ANDROIDX
                                                   , context
#endif
#if SMARTMAUI
                                                   , myWindow
#endif
                                                    );
        }
        internal static async Task<bool> UpdateConnectionCoreAsync(
                                        SignInViewModel signinviewmodel,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        bool update,
                                        bool delete
#if WPF
                                        ,Window myWindow
#endif
#if WINUI
                                        ,ContentDialog myWindow
#endif
#if ANDROIDX
                                        , Context context
#endif
#if SMARTMAUI
                                        ,ContentPage myWindow
#endif
                                        )
        {
            if (financeviewmodel.SelectedConnection == null)
            {
                return false;
            }
            financeviewmodel.SelectedConnection.USERNAME = ourviewmodel.UserName;
            financeviewmodel.SelectedConnection.CUBEFACE_CODE = SmartParametersV2016.Finance;
            financeviewmodel.SelectedConnection.DETAILS = "";
            financeviewmodel.SelectedConnection.RANDOMKEY = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            financeviewmodel.SelectedConnection.Updated = update;
            financeviewmodel.SelectedConnection.Delete = delete;
            financeviewmodel.PLO.finance_connections_changesList.Add(financeviewmodel.SelectedConnection);
            string message = "";
            // NOTE: DoAllSmartFinance executes entirely on the UI thread.
            // Safe to update UI-bound collections here.
            if (!await SmartFinanceScrapeV2023.DoAllSmartFinance(
                    ourviewmodel,
                    financeviewmodel,
                    false,
                    SmartParametersV2016.sqliteformat,
                    delete))
            {
                if (delete) message = SignIn.BesetByChimps(signinviewmodel, "ConnectionDeleteFail");
                else if (update) message = SignIn.BesetByChimps(signinviewmodel, "ConnectionUpdateFail");
                else message = SignIn.BesetByChimps(signinviewmodel, "ConnectionAddFail");
#if WPF
                new ToastWindow(myWindow, message, ToastType.Error).Show();
#endif
#if WINUI
                new ToastWindow(message, ToastType.Error).Show();
#endif
#if ANDROIDX
                ToastService.Error(context,
                message);
#endif
#if SMARTMAUI
                new ToastWindow(message, ToastType.Error);
#endif
                return false;
            }
            else
            {
                if (delete) message = SignIn.BesetByChimps(signinviewmodel, "ConnectionDeleteSuccess");
                else if (update) message = SignIn.BesetByChimps(signinviewmodel, "ConnectionUpdateSuccess");
                else message = SignIn.BesetByChimps(signinviewmodel, "ConnectionAddSuccess");
#if WPF
                new ToastWindow(myWindow, message, ToastType.Success).Show();
#endif
#if WINUI
                new ToastWindow(message, ToastType.Success).Show();
#endif
#if ANDROIDX
                ToastService.Error(context,
                message);
#endif
#if SMARTMAUI
                new ToastWindow(message, ToastType.Success);
#endif
            }
            return true;
        }

        internal static async Task<bool> UpdateLoginCoreAsync(
                                        SignInViewModel signinviewmodel,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        bool update,
                                        bool delete
#if WPF
                                        , Window myWindow
#endif
#if WINUI
                                        , ContentDialog myWindow
#endif
#if ANDROIDX
                                        , Context context
#endif
#if SMARTMAUI
                                        , ContentPage myWindow
#endif
                                        )
        {
            if (financeviewmodel.SelectedLogin == null)
            {
                return false;
            }
            financeviewmodel.SelectedLogin.USERNAME = ourviewmodel.UserName;
            financeviewmodel.SelectedLogin.CUBEFACE_CODE = SmartParametersV2016.Finance;
            financeviewmodel.SelectedLogin.DETAILS = "";
            financeviewmodel.SelectedLogin.RANDOMKEY = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR);
            financeviewmodel.SelectedLogin.Updated = update;
            financeviewmodel.SelectedLogin.Delete = delete;
            financeviewmodel.PLO.finance_logins_changesList.Add(financeviewmodel.SelectedLogin);
            string message = "";
            // NOTE: DoAllSmartFinance executes entirely on the UI thread.
            // Safe to update UI-bound collections here.
            if (!await SmartFinanceScrapeV2023.DoAllSmartFinance(
                    ourviewmodel,
                    financeviewmodel,
                    false,
                    SmartParametersV2016.sqliteformat,
                    delete))
            {
                if (delete) message = SignIn.BesetByChimps(signinviewmodel, "LoginDeleteFail");
                else if (update) message = SignIn.BesetByChimps(signinviewmodel, "LoginUpdateFail");
                else message = SignIn.BesetByChimps(signinviewmodel, "LoginAddFail");
#if WPF
                financeviewmodel.suppressClose = true;                
                new ToastWindow(myWindow, message, ToastType.Error).Show();
                financeviewmodel.suppressClose = false;
#endif
#if WINUI
                financeviewmodel.suppressClose = true;
#if WINUI
                new ToastWindow(message, ToastType.Error).Show();
#endif
                financeviewmodel.suppressClose = false;
#endif
#if ANDROIDX
                ToastService.Error(context,
                message);
#endif
#if SMARTMAUI
                financeviewmodel.suppressClose = true;
                new ToastWindow(message, ToastType.Error);
                financeviewmodel.suppressClose = false;
#endif
                return false;
            }
            else
            {
                if (delete) message = SignIn.BesetByChimps(signinviewmodel, "LoginDeleteSuccess");
                else if (update) message = SignIn.BesetByChimps(signinviewmodel, "LoginUpdateSuccess");
                else message = SignIn.BesetByChimps(signinviewmodel, "LoginAddSuccess");
#if WPF
                financeviewmodel.suppressClose = true;
                new ToastWindow(myWindow, message, ToastType.Success).Show();
                financeviewmodel.suppressClose = false;
#endif
#if WINUI
                financeviewmodel.suppressClose = true;
#if WINUI
                new ToastWindow(message, ToastType.Success).Show();
#endif
                financeviewmodel.suppressClose = false;
#endif
#if ANDROIDX
                ToastService.Success(context,
                message);
#endif
#if SMARTMAUI
                financeviewmodel.suppressClose = true;
                new ToastWindow(message, ToastType.Success);
                financeviewmodel.suppressClose = false;
#endif
            }
            return true;
        }

#if ANDROIDX
        internal static FinanceViewPager2Adapter financeAdapter { get; set; }

        public static void SetUpFinanceViewPager(View thisView, AndroidX.Fragment.App.FragmentManager fm, AndroidX.Lifecycle.Lifecycle lfc, MainViewModel ourviewmodel, FinanceViewModel financeviewmodel)
        {
            TabLayout financetabLayout = thisView.FindViewById<TabLayout>(Resource.Id.financetabLayout);
            ViewPager2 financePager2 = thisView.FindViewById<ViewPager2>(Resource.Id.financeviewPager);
            financePager2.OffscreenPageLimit = 3;   // < New
            financeviewmodel.financePager2 = financePager2;

            financeAdapter = new FinanceViewPager2Adapter(fm, lfc, FinanceStrategy.financefragmentTitles.Count, ourviewmodel, financeviewmodel);
            financePager2.Adapter = financeAdapter;
            financePager2.UserInputEnabled = false;
            financeAdapter.NotifyDataSetChanged();
            //strategy = new FinanceStrategy(financePager2);
            new TabLayoutMediator(financetabLayout, financePager2, new FinanceStrategy(financePager2)).Attach();
            return;
        }

        public static void SetUpFinanceCurrencyPager(View thisView, AndroidX.Fragment.App.FragmentManager fm, AndroidX.Lifecycle.Lifecycle lfc, FinanceViewModel financeviewmodel)
        {
            ViewPager2 currencyPager2 = thisView.FindViewById<ViewPager2>(Resource.Id.viewPager);
            RecyclerView currencytabRecyclerView = thisView.FindViewById<RecyclerView>(Resource.Id.verticalTabRecyclerView);

            // Set up ViewPager2 adapter
            FinanceCurrencyPager2Adapter currencyAdapter = new FinanceCurrencyPager2Adapter(fm, lfc, 3, financeviewmodel);
            currencyPager2.Adapter = currencyAdapter;
            currencyPager2.UserInputEnabled = false;
            currencyAdapter.NotifyDataSetChanged();

            // Set up tabs for RecyclerView
            FinanceCurrencyTabAdapter tabAdapter = new FinanceCurrencyTabAdapter(financeviewmodel.TabTitles, position => currencyPager2.SetCurrentItem(position, true));
            currencytabRecyclerView.SetAdapter(tabAdapter);
            currencytabRecyclerView.SetLayoutManager(new LinearLayoutManager(thisView.Context));

            // Sync RecyclerView with ViewPager2
            currencyPager2.RegisterOnPageChangeCallback(new CustomOnPageChangeCallback(tabAdapter, currencytabRecyclerView));
            return;
        }
#endif

#if WINFORMS
        internal static async void InstitutionSelectionChanged(object sender, EventArgs e,
#endif
#if WPF  || WINUI || SMARTMAUI
        internal static async void InstitutionSelectionChanged(object sender, SelectionChangedEventArgs e,
#endif
#if ANDROIDX
        // I know the events are wrong
        internal static async void InstitutionSelectionChanged(object sender, EventArgs e,
#endif

                                                SignInViewModel signinviewmodel,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {
#if WINFORMS
            ComboBox combo = sender as ComboBox;
#endif
#if WPF
            System.Windows.Controls.ComboBox combo = sender as System.Windows.Controls.ComboBox;
#endif
#if WINUI
            ComboBox combo = sender as ComboBox;
#endif
#if ANDROIDX
            Spinner combo = sender as Spinner;
#endif
#if SMARTMAUI
            Picker combo = sender as Picker;
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            if (combo.SelectedItem == null)
            {
                return;
            }
             FinanceViewModel.InstitutionItem item = combo.SelectedItem as FinanceViewModel.InstitutionItem;
            short DefaultInstitutionCode = Convert.ToInt16(item.Value);
            FinanceViewModel.InstitutionItem institutionItem = combo.SelectedItem as FinanceViewModel.InstitutionItem;
            financeviewmodel.FinanceInstitutionsText = institutionItem.Content;
            List<SmartFinance.InstitutionInfo> infos_found = SmartSpikeFinanceV2017.Finance_Lookup_INSTITUTION_INFOS(financeviewmodel.PLO.institutionsList,

            financeviewmodel.PLO.institution_infoList);
#endif
            financeviewmodel.PLO.finance_connectionsList.Clear();
            await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                        financeviewmodel,
                                                        true,
                                                        "SmartFinance",
                                                        "Connections",
                                                        ourviewmodel.UserName,
                                                        ourviewmodel.UserName,
                                                        ourviewmodel.UserName,
                                                        ourviewmodel.FDEK,
                                                        false);
            foreach (SmartFinance.Connections conn in financeviewmodel.PLO.finance_connectionsList)
            {
                conn.InstitutionName = SmartSpikeFinanceV2017.Finance_Lookup_InstitutionName(ourviewmodel,
                            financeviewmodel,
                            conn.INSTITUTION_CODE);
                conn.BrandName = SmartSpikeFinanceV2017.Finance_Lookup_SingleBrandName(ourviewmodel,
                            financeviewmodel,
                            conn.INSTITUTION_CODE,
                            conn.BRAND_CODE);
            }
            financeviewmodel.PLO.finance_loginsList = new List<SmartFinance.Logins>();
            await SmartPhyllV2020.LoadCommonFinance(ourviewmodel,
                                                        financeviewmodel,
                                                        true,
                                                        "SmartFinance",
                                                        "Logins",
                                                        ourviewmodel.UserName,
                                                        ourviewmodel.UserName,
                                                        ourviewmodel.UserName,
                                                        ourviewmodel.FDEK,
                                                        false);
            financeviewmodel.ConnectionsViewList = new List<SmartFinance.Connections>();
            foreach (SmartFinance.Connections con in financeviewmodel.PLO.finance_connectionsList)
            {
                con.InstitutionName = SmartSpikeFinanceV2017.Finance_Lookup_InstitutionName(ourviewmodel,
                            financeviewmodel,
                            con.INSTITUTION_CODE);
                con.BrandName = SmartSpikeFinanceV2017.Finance_Lookup_SingleBrandName(ourviewmodel,
                            financeviewmodel,
                            con.INSTITUTION_CODE,
                            con.BRAND_CODE);
                var connection = new SmartFinance.Connections
                {
                    USERNAME = con.USERNAME,
                    CUBEFACE_CODE = con.CUBEFACE_CODE,
                    INSTITUTION_CODE = con.INSTITUTION_CODE,
                    BRAND_CODE = con.BRAND_CODE,
                    LOGIN_METHOD = con.LOGIN_METHOD,
                    ACTIVE_FLAG = con.ACTIVE_FLAG,
                    InstitutionName = con.InstitutionName,
                    BrandName = con.BrandName,
                    Parameter1 = con.Parameter1,
                    Parameter2 = con.Parameter2,
                    Parameter3 = con.Parameter3,
                    IsLoginMethodLocked = true         // lock to show text
                };
                if (connection.INSTITUTION_CODE == financeviewmodel.DefaultInstitutionCode)
                {
                    //var brand = financeviewmodel.CreatedBrands.FirstOrDefault(b => b.BRAND_CODE == connection.BRAND_CODE);
                    //if (brand != null)
                    //{
                    financeviewmodel.ConnectionsViewList.Add(connection);
                    //}
                }
            }
#if WPF
            var view = CollectionViewSource.GetDefaultView(
                financeviewmodel.ConnectionsViewList);
            view?.Refresh();
#endif
#if WINUI
            financeviewmodel.ConnectionsViewList = financeviewmodel.ConnectionsViewList.ToList();
#endif
            financeviewmodel.LoginsViewList = new List<SmartFinance.Logins>();
            foreach (SmartFinance.Logins log in financeviewmodel.PLO.finance_loginsList)
            {
                log.InstitutionName = SmartSpikeFinanceV2017.Finance_Lookup_InstitutionName(ourviewmodel,
                            financeviewmodel,
                            log.INSTITUTION_CODE);
                log.BrandName = SmartSpikeFinanceV2017.Finance_Lookup_SingleBrandName(ourviewmodel,
                            financeviewmodel,
                            log.INSTITUTION_CODE,
                            log.BRAND_CODE);
                SmartFinance.Logins login = new SmartFinance.Logins
                {
                    USERNAME = log.USERNAME,
                    CUBEFACE_CODE = log.CUBEFACE_CODE,
                    INSTITUTION_CODE = log.INSTITUTION_CODE,
                    InstitutionName = log.InstitutionName,
                    BRAND_CODE = log.BRAND_CODE,
                    BrandName = log.BrandName,
                    LOGIN_METHOD = log.LOGIN_METHOD,
                    ACTIVE_FLAG = log.ACTIVE_FLAG,
                    BANKSCHECKED = log.BANKSCHECKED,
                    SAVINGSCHECKED = log.SAVINGSCHECKED,
                    INVESTMENTSCHECKED = log.INVESTMENTSCHECKED,
                    CRYPTOSCHECKED = log.CRYPTOSCHECKED
                };
                // 🔒 Existing logins are ALWAYS finalized
                login.UserFinalizedLoginMethod = true;
                login.IsLoginMethodLocked = true;
                //login.IsLoginMethodEnabled = false;
                // 🔥 THIS WAS MISSING
                if (string.IsNullOrEmpty(login.Parameter1))
                {
                    login.Parameter1 = SmartSpikeFinanceV2017.FinanceFindParameter1(ourviewmodel,
                                                                                    financeviewmodel,
                                                                                    log.INSTITUTION_CODE,
                                                                                    log.BRAND_CODE,
                                                                                    log.LOGIN_METHOD);
                }
                if (login.INSTITUTION_CODE == financeviewmodel.DefaultInstitutionCode)
                {
                    financeviewmodel.LoginsViewList.Add(login);
                }
            }
            financeviewmodel.LoginsViewList = financeviewmodel.LoginsViewList.ToList();
            financeviewmodel.CreatedBrands.Clear();
            foreach (SmartFinance.Brands brand in financeviewmodel.PLO.brandsList)
            {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
                if (brand.INSTITUTION_CODE == Convert.ToInt16(item.Value))
#endif
#if ANDROIDX
                if (brand.INSTITUTION_CODE > 0)// Convert.ToInt16(item.Value)) // I know this is wrong
#endif
                {
                    BrandItem brandItem = new BrandItem()
                    {
                        InstitutionCode = brand.INSTITUTION_CODE,
                        BRAND_CODE = brand.BRAND_CODE,
                        BrandName = brand.BRAND_NAME
                    };
                    financeviewmodel.CreatedBrands.Add(brandItem);
                }
            }
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            financeviewmodel.DefaultInstitutionCode = Convert.ToInt16(item.Value);
#endif
#if ANDROIDX
            financeviewmodel.DefaultInstitutionCode = financeviewmodel.DefaultInstitutionCode; // I know this is wrong
#endif
            //financeviewmodel.DefaultInstitutionName = Convert.ToInt16(item.Value);
#if WPF
            FinanceConnectionsLogins abc = new FinanceConnectionsLogins(signinviewmodel, ourviewmodel,
                                            financeviewmodel,
                                            financeviewmodel.DefaultInstitutionCode,
                                            "Rays");
            abc.ShowDialog();
#endif
#if WINUI
            FinanceConnectionsLogins abc = new FinanceConnectionsLogins(signinviewmodel, ourviewmodel,
                                            financeviewmodel,
                                            financeviewmodel.DefaultInstitutionCode,
                                            "Rays");
            await abc.ShowAsync();
#endif
            return;
        }

        internal static bool Connections_Build_Institutions_Dropdown(
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            char activeFlag)
        {
            // There is no resource_type passed into this routine ... Yes there is
            // We are doing Institutions within Areas and we need to do Areas within Suppliers
            // (because we may have a Supplier with no Areas, but we will never have an Area with no Suppliers)
            List<FinanceViewModel.InstitutionItem> temp_institutions = new List<FinanceViewModel.InstitutionItem>();
            if (financeviewmodel.PLO.institutionsList.Count > 0)
            {
                int institution_id = 0;
                foreach (SmartFinance.Institutions info in financeviewmodel.PLO.institutionsList)
                {
                    string header = "";
                    string institution_name = "";
                    if (financeviewmodel.PLO.institution_infoList.Count > 0)
                    {
                        institution_name = SmartSpikeFinanceV2017.Finance_Lookup_InstitutionName(
                                ourviewmodel,
                                financeviewmodel,
                                info.INSTITUTION_CODE);
                        if (!string.IsNullOrEmpty(institution_name))
                        {
#if WINFORMS
                            System.Drawing.Color  tempColour = ourviewmodel.blackColour;
#endif
#if WPF
                            Brush tempColour = ourviewmodel.blackColour;
#endif
#if WINUI
                            Brush tempColour = ourviewmodel.blackColour;
#endif
#if ANDROIDX
                            Color tempColour = ourviewmodel.blackColour;
#endif
#if SMARTMAUI
                            Color tempColour = ourviewmodel.blackColour;
#endif
                            FinanceViewModel.InstitutionItem institution_item = new FinanceViewModel.InstitutionItem()
                            {
                                InstitutionID = Convert.ToInt16(institution_id + 1),
                                Colour = tempColour,
                                Content = header + institution_name,
                                Value = info.INSTITUTION_CODE.ToString()
                            };
                            // She is a MINE of mangled distortions, distractions, muddled facts, mis-represntations
                            // and mis-information regarding names
                            // dates, times, places people and events AND all of them put together!
                            temp_institutions.Add(institution_item);
                            institution_id++;
                        }
                    }
                }
                // There is a selected Institution which is the first
                // You can choose any of them by clicking to set up
                // or change the connection info 
                financeviewmodel.FinanceInstitutionsList = temp_institutions;
            }
            return true;
        }
        internal static async void ButtonAutoSwitchFinanceClick(object sender, object e,
                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel)
        {
            if (sender != null &&
#if WINFORMS || WPF
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
#if WINUI
                (FrontEndGUI.DecodeRoutedEventFlags(e) != null))
#endif
#if ANDROIDX
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif
#if SMARTMAUI
                (FrontEndGUI.DecodeEventFlags(e) != null))
#endif

            {
                if (!await SmartFinanceV2025.ButtonAutoSwitchFinanceClickActual(signinviewmodel,
                                                                                ourviewmodel,
                                                                                financeviewmodel))
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel,
                        ourviewmodel.quitCts.Token, 0, 0,
                        "Problem 23: AutoSwitch Finance failed"))
                    {
                        return;
                    }
                }
            }
            return;
        }
#if ANDROIDX
        //        internal async void ButtonAutoSwitchFinanceClick(object sender, EventArgs e)
        //        {
        //            if (sender != null && e != null)
        //            {
        //                if (!await SmartUtilityV2022.ButtonAutoSwitchUtilityClickActual(SignIn.signinviewmodel,
        //                                                                        ourviewmodel,
        //                                                                        utilityviewmodel))
        //                {
        //                    await SmartRoutinesV2018.CheckTrace(ourviewmodel, ourviewmodel.userToken, 0, 0, "AutoSwitch Finance failed");
        //                }
        //            }
        //            return;
        //        }
#endif

        internal static void CancelAllTimers(FinanceViewModel financeviewmodel)
        {
            if (financeviewmodel.timers.Count == 0)
            {
                return;
            }
            financeviewmodel.timers.Clear();  // Don't call OnComplete for each
            return;
        }

        internal static long UnixSequenceNo()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();    // UTC time
        }

        internal static SmartFinance.Accounts Account_Template(MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                short institution_code,
                                                                short brand_code,
                                                                string sortcode,
                                                                string account_no,
                                                                string udprn,
                                                                DateTime accountCreated,
                                                                char category_code,
                                                                short accountOrdinal,
                                                                string account_title,
                                                                int balance,
                                                                char status)
        {
            SmartFinance.Accounts account_template = new SmartFinance.Accounts()
            {
                USERNAME = ourviewmodel.UserName,
                CUBEFACE_CODE = financeviewmodel.cubeface_code,
                INSTITUTION_CODE = institution_code,
                BRAND_CODE = brand_code,
                SORTCODE = sortcode,
                ACCOUNT_NO = account_no,
                UDPRN = udprn,
                ACCOUNT_CREATED = accountCreated,  // UTC time
                RANDOMKEY1 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
                CATEGORY_CODE = category_code,
                CURRENCY_ORDINAL = accountOrdinal,
                ACCOUNT_TITLE = account_title,
                //ACCOUNT_SUBTYPE = "",
                //DESCRIPTION = "",
                //NICKNAME = "",
                ACCOUNT_BALANCE = balance,
                //OTHER = "",
                STATUS = status,
                RANDOMKEY2 = SmartRoutinesV2018.GetNextRandomNew(ourviewmodel.randomR),
                Updated = false     // Its an I(nsert)
            };
            return account_template;
        }


        // All the Arse Breaking stuff starts here
        // and its all been shifted out to SmartFinanceConnectionsV2026
        // Ends here

        // This is CUMULATIVE!
        internal static async Task<bool> Start_Finance_Display(
#if WINFORMS
                                                        MainProcess process_components,
#endif
#if ANDROIDX
                                                        AppCompatActivity meterActivity,
#endif
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        DateTime next_connection,
                                                        char cubeface_code,
                                                        string face_last_display)
        {
            //
            // In Finance we have four Categories - Banks, Savings, Investments and ... Crypto
            //
            try
            {
                bool[] autoswitch_button_status = new bool[2] { false, false };
                // Find out which one was set ('B' is the default if none set <= No!)
                FinanceDisplaySwitch(
#if ANDROIDX
                                        meterActivity,
#endif
#if WINFORMS
                                        process_components,
#endif

                                        ourviewmodel,
                                        financeviewmodel);
                //if (financeviewmodel.category_code != SmartParametersV2016.defaultChar)
                //{
                if (Finance_Get_Pending(financeviewmodel.category_code, autoswitch_button_status))
                {
                    financeviewmodel.AutoSwitchFinanceColour = ourviewmodel.redColour;
#if ANDROIDX
                    ourviewmodel.AutoSwitch0 = ourviewmodel.openred0;
                    ourviewmodel.AutoSwitch25 = ourviewmodel.openred25;
                    ourviewmodel.AutoSwitch50 = ourviewmodel.openred50;
                    ourviewmodel.AutoSwitch75 = ourviewmodel.openred75;
#endif
                }
                else
                {
                    financeviewmodel.AutoSwitchFinanceColour = ourviewmodel.magentaColour;
#if ANDROIDX
                    ourviewmodel.AutoSwitch0 = ourviewmodel.autoswitch0;
                    ourviewmodel.AutoSwitch25 = ourviewmodel.autoswitch25;
                    ourviewmodel.AutoSwitch50 = ourviewmodel.autoswitch50;
                    ourviewmodel.AutoSwitch75 = ourviewmodel.autoswitch75;
#endif
                }
                FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.orangeColour);
                // Before you wonder why this isn't an 'async void' routine, check out
                // https://msdn.microsoft.com/en-us/magazine/jj991977.aspx
                if (!await DisplayFinanceMeterAsync(
#if WINFORMS
                                                process_components,
#endif
#if ANDROIDX
                                                meterActivity,
#endif
                                                ourviewmodel,
                                                financeviewmodel,
                                                SmartParametersV2016.activeFlag,
                                                next_connection))
                {
                    // We failed possibly (?) because of a Cancellation ... but which one Quit or Cancel?
                    // We do nothing if there is a failure .. !  SmartSwitch DOES NOT fail!!
                    return false;
                }
                // Turn on the buttons

                // Ive cut all this out because I dont know why were doing it?
                // Surely, when I make the FinanceView (whichever element) I create
                // the RadioButton events (press/check/why) inside FinanceView!
                // So ... why amI doing them again here???

#if WINFORMS
                // Lets not make life difficult for ourselves
                FinanceView.RadioButtonTappedEvents(process_components, true); // MouseDoubleClick
                FinanceView.RadioButtonCheckedEvents(process_components, ourviewmodel, financeviewmodel, true); // MouseClick
#endif
#if WPF
                FinanceView.RadioButtonTappedEvents((FinanceView)SmartRoutinesV2018.CubefaceView(ourviewmodel, SmartParametersV2016.Finance), ourviewmodel, financeviewmodel, true);
                FinanceView.RadioButtonCheckedEvents((FinanceView)SmartRoutinesV2018.CubefaceView(ourviewmodel, SmartParametersV2016.Finance), ourviewmodel, financeviewmodel, true);
#endif
#if WINUI
                FinanceView.RadioButtonTappedEvents((FinanceView)SmartRoutinesV2018.CubefaceView(ourviewmodel, SmartParametersV2016.Finance), ourviewmodel, financeviewmodel, true);
                FinanceView.RadioButtonCheckedEvents((FinanceView)SmartRoutinesV2018.CubefaceView(ourviewmodel, SmartParametersV2016.Finance), ourviewmodel, financeviewmodel, true);
#endif
#if ANDROIDX
                FinanceView.RadioButtonTappedEvents(SmartRoutinesV2018.CubefaceView(ourviewmodel, SmartParametersV2016.Finance), meterActivity, ourviewmodel, financeviewmodel, true);
                FinanceView.RadioButtonCheckedEvents(SmartRoutinesV2018.CubefaceView(ourviewmodel, SmartParametersV2016.Finance), meterActivity, ourviewmodel, financeviewmodel, true);
#endif

                SmartRoutinesV2018.FocusOpenClose(ourviewmodel);

                // Even enabling the drop-downs HERE ... the SelectedIndex event
                // still gets called (what a pile of bollocks)
                TurnOnFinanceStatus(
#if WINFORMS
                                                    process_components,
#endif
                                                    financeviewmodel);
                await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel,
                                            "Cubeface: " + cubeface_code + " display");
#if WINFORMS
                // Never got this shit working
                //process_components.comboBoxGUIFinanceCultures.SelectedIndex = financeviewmodel.selectedIndex;
#endif
            }
            catch (ArgumentNullException ex)
            {
                // Nothing ever turns this back from Red
                financeviewmodel.errorMessage = ex.Message;
            }
            catch (NullReferenceException ex)
            {
                // Nothing ever turns this back from Red
                financeviewmodel.errorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
            }
            if (!string.IsNullOrEmpty(financeviewmodel.errorMessage))
            {
                // Nothing ever turns this back from Red
                // Try and tell HQ
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem " + " " + financeviewmodel.errorMessage))
                {
                    return false;
                }
                return false;
            }
            return true;
        }

        internal static void FinanceDisplaySwitch(
#if WINFORMS
                                                    MainProcess components,
#endif
#if ANDROIDX
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
            bool[] autoswitch = new bool[3] { false, false, false };

            foreach (SmartFinance.Categories category_row in financeviewmodel.PLO.finance_categoriesList)
            {
                // Might be more than ONE CHECKED now!
                if (category_row.CHECKED == SmartParametersV2016.lastChecked)
                {
                    //if (financeviewmodel.category_codex == SmartParametersV2016.defaultChar)
                    //{
                    //    financeviewmodel.category_codex = category_row.CATEGORY_CODE;
                    //}
                    switch (category_row.CATEGORY_CODE) //financeviewmodel.category_code)
                    {
                        case SmartParametersV2016.Banks:
#if WINFORMS
                            components.RBBanks.Checked = true;
#endif
#if WPF || SMARTMAUI
                            financeviewmodel.BanksChecked = true;
#endif
#if WINUI
                            financeviewmodel.BanksChecked = true;
#endif
#if ANDROIDX
                            financeviewmodel.BanksChecked = true;
                            financeviewmodel.RBBanks.Checked = true;
#endif
                            financeviewmodel.meterFormat = SmartParametersV2016.format2Places;
                            FixCategoryTypes(financeviewmodel, category_row);
                            financeviewmodel.categoryCodes += category_row.CATEGORY_CODE.ToString();
                            break;
                        case SmartParametersV2016.Savings:
#if WINFORMS
                            components.RBSavings.Checked = true;
#endif
#if WPF || SMARTMAUI
                            financeviewmodel.SavingsChecked = true;
#endif
#if WINUI
                            financeviewmodel.SavingsChecked = true;
#endif
#if ANDROIDX
                            financeviewmodel.SavingsChecked = true;
                            financeviewmodel.RBSavings.Checked = true;
#endif
                            financeviewmodel.meterFormat = SmartParametersV2016.format2Places;
                            FixCategoryTypes(financeviewmodel, category_row);
                            financeviewmodel.categoryCodes += category_row.CATEGORY_CODE.ToString();

                            break;
                        case SmartParametersV2016.Investments:
#if WINFORMS
                            components.RBInvestments.Checked = true;
#endif
#if WPF || SMARTMAUI
                            financeviewmodel.InvestmentsChecked = true;
#endif
#if WINUI
                            financeviewmodel.InvestmentsChecked = true;
#endif
#if ANDROIDX
                            financeviewmodel.InvestmentsChecked = true;
                            financeviewmodel.RBInvestments.Checked = true;
#endif
                            financeviewmodel.meterFormat = SmartParametersV2016.format2Places;
                            FixCategoryTypes(financeviewmodel, category_row);
                            financeviewmodel.categoryCodes += category_row.CATEGORY_CODE.ToString();
                            break;
                        case SmartParametersV2016.Cryptos:
#if WINFORMS
                            components.RBCryptos.Checked = true;
#endif
#if WPF || SMARTMAUI
                            financeviewmodel.CryptosChecked = true;
#endif
#if WINUI
                            financeviewmodel.CryptosChecked = true;
#endif
#if ANDROIDX
                            financeviewmodel.CryptosChecked = true;
                            financeviewmodel.RBCryptos.Checked = true;
#endif
                            financeviewmodel.meterFormat = SmartParametersV2016.format2Places;
                            FixCategoryTypes(financeviewmodel, category_row);
                            financeviewmodel.categoryCodes += category_row.CATEGORY_CODE.ToString();
                            break;
                        default:
                            break;
                    }
                }
            }

            //if (consumers_view_row.AUTOSWITCH == Convert.ToChar(SmartParametersV2016.yesFlag))
            //{
            //    Finance_Update_Pending(financeviewmodel, consumers_view_row.CATEGORY_CODE, true);
            //}

#if ANDROIDX
            financeviewmodel.FinanceSpinnerAddressesList = new();
#endif

            financeviewmodel.FinanceAddressesList.Clear();
            List<MainViewModel.AddressItem> tempList = new List<MainViewModel.AddressItem>();

            List<SmartProfile.AddressesView> addressList = SmartSpikeFinanceV2017.Finance_Configure_Addresses(ourviewmodel, financeviewmodel);
            foreach (SmartProfile.AddressesView address_row in addressList)
            {
                // What are we saying here is that IF we have an AccountNo then we MUST have an address
                // to hook it to, because Banks work like that!  We can say 'if you have an
                // BANK ACCOUNT then you must have an address to hook it to 'cos a bank won't open an
                // account for you without an address
                // We won't have ANY Finance Accounts without a UDPRN which
                // can be joined back to Addresses.  Likewise we will never
                // have ANY Finance Transactions which don't can't be joined
                // back to a Finance Account and hence to an Address.

                MainViewModel.AddressItem addressItem = new MainViewModel.AddressItem()
                {
                    Content = address_row.BASIC,
                    Value = address_row.UDPRN,
                    Colour = ourviewmodel.blackColour
                };
                tempList.Add(addressItem);
#if ANDROIDX
                financeviewmodel.FinanceSpinnerAddressesList.Add(addressItem.Content);
#endif
            }

            if (tempList.Count > 0)
            {
                financeviewmodel.FinanceAddressesList = tempList;
#if ANDROIDX
                ArrayAdapter<string> addressAdapter = new ArrayAdapter<string>(meterActivity, Android.Resource.Layout.SimpleSpinnerItem, financeviewmodel.FinanceSpinnerAddressesList);
                addressAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                financeviewmodel.Addresses.Adapter = addressAdapter;
                financeviewmodel.Addresses.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>((s, e) => FinanceView.Finance_AddressSelectionChanged(s, e, ourviewmodel, financeviewmodel));
#endif
                //financeviewmodel.AddressSelectedIndex = 0;
            }

            List<MainViewModel.CultureItem> temp =
                new List<MainViewModel.CultureItem>();
            int index = -1;
            int selectedIndex = 0;
            foreach (SmartData.CultureView cv in ourviewmodel.cultureviewList)
            {
                index++;
                MainViewModel.CultureItem cultureItem = new MainViewModel.CultureItem()
                {
                    CultureID = index,
                    Content = cv.CULTURE_CODE,
                    Source = cv.DESCRIPTION
                };
                if (cv.CULTURE_CODE == financeviewmodel.FinanceCultureCode)
                {
                    // So the first default item appears 'green' in the dropdown
                    cultureItem.Colour = ourviewmodel.greenColour;
                    selectedIndex = index;
                }
                else
                {
                    cultureItem.Colour = ourviewmodel.blackColour;
                }
                temp.Add(cultureItem);
            }
            financeviewmodel.FinanceCulturesList = temp;
            financeviewmodel.LastCultureSelectedIndex = selectedIndex;
#if WINFORMS
            components.comboBoxGUIFinanceCultures.DataSource = financeviewmodel.FinanceCulturesList;
            components.financeCulturesListBindingSource.DataMember = "FinanceCulturesList";
            components.financeCulturesListBindingSource.DataSource = components.FinanceBindingSource;
            components.comboBoxGUIFinanceCultures.DataBindings.Add(new("SelectedIndex", components.financeCulturesListBindingSource, "Content", true, DataSourceUpdateMode.OnPropertyChanged));
            if (components.comboBoxGUIFinanceCultures.Items.Count > 0)
            {
                components.comboBoxGUIFinanceCultures.SelectedIndex = selectedIndex;
            }
            else
            {
                components.comboBoxGUIFinanceCultures.SelectedIndex = -1;
            }
            components.comboBoxGUIFinanceCultures.SelectedIndexChanged += (s, e) => components.FinanceCultures_SelectedIndexChanged(s, e, ourviewmodel, financeviewmodel);
#endif
#if ANDROIDX
            SmartFinanceV2025.FinanceCulturesAdapter adapter = new FinanceCulturesAdapter(meterActivity, financeviewmodel.FinanceCulturesList);
            financeviewmodel.FinanceCultures.Adapter = adapter;
            // Otherwise 'CAN' appears in the first slot
            financeviewmodel.FinanceCultures.SetSelection(financeviewmodel.LastCultureSelectedIndex);
#endif
            financeviewmodel.WorkingCurrency = financeviewmodel.ConvertToSymbol;
            if (financeviewmodel.WorkingCurrency == "")
            {
                financeviewmodel.WorkingCurrency = "GBP";
            }
            switch (financeviewmodel.ConvertToSymbol)
            {
                case "GBP":
#if WINFORMS
                    components.FinanceRBGBP.Checked = true;
#endif
#if WPF || SMARTMAUI
                    financeviewmodel.FinanceGBPChecked = true;

#endif
#if WINUI
                    financeviewmodel.FinanceGBPChecked = true;

#endif
#if ANDROIDX
                    financeviewmodel.FinanceRBGBP.Checked = true;
                    financeviewmodel.FinanceRBGBP.SetTextColor(ourviewmodel.greenColour);
#endif
                    break;
                case "EUR":
#if WINFORMS
                    components.FinanceRBEUR.Checked = true;
#endif
#if WPF || SMARTMAUI
                    financeviewmodel.FinanceEURChecked = true;
#endif
#if WINUI
                    financeviewmodel.FinanceEURChecked = true;
#endif
#if ANDROIDX
                    financeviewmodel.FinanceRBEUR.Checked = true;
                    financeviewmodel.FinanceRBEUR.SetTextColor(ourviewmodel.greenColour);
#endif
                    break;
                case "USD":
#if WINFORMS
                    components.FinanceRBUSD.Checked = true;
#endif
#if WPF || SMARTMAUI
                    financeviewmodel.FinanceUSDChecked = true;
#endif
#if WINUI
                    financeviewmodel.FinanceUSDChecked = true;
#endif
#if ANDROIDX
                    financeviewmodel.FinanceRBUSD.Checked = true;
                    financeviewmodel.FinanceRBUSD.SetTextColor(ourviewmodel.greenColour);
#endif
                    break;
                case "JPY":
#if WINFORMS
                    components.FinanceRBJPY.Checked = true;
#endif
#if WPF || SMARTMAUI
                    financeviewmodel.FinanceJPYChecked = true;
#endif
#if WINUI
                    financeviewmodel.FinanceJPYChecked = true;
#endif
#if ANDROIDX
                    financeviewmodel.FinanceRBJPY.Checked = true;
                    financeviewmodel.FinanceRBJPY.SetTextColor(ourviewmodel.greenColour);
#endif
                    break;
                default:
#if WINFORMS
                    components.FinanceRBNON.Checked = true;
#endif
#if WPF || SMARTMAUI
                    financeviewmodel.FinanceNONChecked = true;
#endif
#if WINUI
                    financeviewmodel.FinanceNONChecked = true;
#endif
#if ANDROIDX
                    financeviewmodel.FinanceRBNON.Checked = true;
                    financeviewmodel.FinanceRBNON.SetTextColor(ourviewmodel.greenColour);
#endif
                    break;
            }
            // Again, not sure why this isn't in a common place like MainMeter?
            ourviewmodel.exchangeRate = SmartSpikeV2017.Lookup_Todays_Exchange_Rate(ourviewmodel.Blanche.exchangeRatesList);
            // Fixup GB in case Exchange Rates is out-of-synch
            if (ourviewmodel.exchangeRate[0] == 0)
            {
                ourviewmodel.exchangeRate[0] = 1;
            }
            return;
        }
        internal static void FixCategoryTypes(FinanceViewModel financeviewmodel,
                                SmartFinance.Categories category_row)
        {
            foreach (SmartFinance.CategoryTypes cattype in financeviewmodel.PLO.finance_categorytypesList)
            {
                if (category_row.USERNAME == cattype.USERNAME &&
                    category_row.CUBEFACE_CODE == cattype.CUBEFACE_CODE &&
                    category_row.CATEGORY_CODE == cattype.CATEGORY_CODE)
                {
                    // Do nothing! Maybe 'Active' at some point?
                    // cattype.Include = category_row.Include;
                }
            }
            return;
        }
        internal static void Finance_Update_Pending(FinanceViewModel financeviewmodel,
                                                    char resource_code,
                                                    bool value)
        {
            switch (resource_code)
            {
                case SmartParametersV2016.Banks:
                    financeviewmodel.autoswitchToggle[0] = value;
                    break;
                case SmartParametersV2016.Savings:
                    financeviewmodel.autoswitchToggle[1] = value;
                    break;
                case SmartParametersV2016.Investments:
                    financeviewmodel.autoswitchToggle[2] = value;
                    break;
                case SmartParametersV2016.Cryptos:
                    financeviewmodel.autoswitchToggle[3] = value;
                    break;
                default:
                    break;
            }
            return;
        }
        internal static bool Finance_Get_Pending(char category_code, bool[] autoswitch)
        {
            switch (category_code)
            {
                case SmartParametersV2016.Banks:
                    return autoswitch[0];
                case SmartParametersV2016.Savings:
                    return autoswitch[1];
                case SmartParametersV2016.Investments:
                    return autoswitch[0] && autoswitch[1];
                case SmartParametersV2016.Cryptos:
                    // No Autoswitching Cryptos yet ha ha!
                    break;
                default:
                    break;
            }
            return false;
        }

        //
        // Before you wonder why this isn't an 'async void' routine, check out
        // https://msdn.microsoft.com/en-us/magazine/jj991977.aspx
        //
        internal static async Task<bool> DisplayFinanceMeterAsync(
#if WINFORMS
                                                                    MainProcess process_components,
#endif
#if ANDROIDX
                                                                    AppCompatActivity meterActivity,
#endif
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    char activeFlag,
                                                                    DateTime next_connection)
        {
            short area_code = 0;            // Never shown though
                                            //string displayed_resource_type = "";
                                            //string displayed_account_no = "";  // The one showing in the ComboBox
            short institution_code = 0;
            short brand_code = 0;


            // The variable 'age' is assigned but its value is never used
            //int age = 0;

            // Look up accounts based on previous Category.CHECKED!
            financeviewmodel.accountsFound =
                    SmartSpikeFinanceV2017.Finance_Lookup_Accounts(ourviewmodel,
                                                                    financeviewmodel);
            if (financeviewmodel.accountsFound.Count == -1)
            {
                ClearDownEverythingFinance(ourviewmodel, financeviewmodel);
                // This IS a show-stopper because we should ALWAYS have an Account Type/Category even if we have no Transactions whatsoever
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, SmartParametersV2016.Finance.ToString() + " " + financeviewmodel.categoryCodes + " Has no Accounts"))
                {
                    return false;
                }
                // Think this is the only place where we return a false - No it isn't
                goto dropdowns;
            }
            
            if (financeviewmodel.accountsFound.Count > 0)
            {
                institution_code = financeviewmodel.accountsFound.First().INSTITUTION_CODE;
                brand_code = financeviewmodel.accountsFound.First().BRAND_CODE;
            }
            // Look up the Finance Addresses JUST to get the Area Code and the
            // Post Code to put in the display - that's all
            // (The addresses dropdown is built later)
            string postcode = "";
            if (financeviewmodel.accountsFound.Count > 0)
            {
#if WINFORMS || WPF || SMARTMAUI
                financeviewmodel.SortCode = financeviewmodel.accountsFound.First().SORTCODE;
#endif
#if WINUI
                financeviewmodel.SortCode = financeviewmodel.accountsFound.First().SORTCODE;
#endif
#if ANDROIDX
                financeviewmodel.SortCode.Text = financeviewmodel.accountsFound.First().SORTCODE;
#endif
                
#if WINFORMS || WPF || SMARTMAUI
                financeviewmodel.AccountNo = financeviewmodel.accountsFound.First().ACCOUNT_NO;
#endif
#if WINUI
                financeviewmodel.AccountNo = financeviewmodel.accountsFound.First().ACCOUNT_NO;
#endif
#if ANDROIDX
                financeviewmodel.AccountNo.Text = financeviewmodel.accountsFound.First().ACCOUNT_NO;
#endif

                

                // We no longer find the Account ordinal,
                // as its copied into each account transaction                
                string udprn = financeviewmodel.accountsFound.First().UDPRN;
                List<SmartProfile.AddressesView> addresses_found =
                        SmartSpikeV2017.Users_Lookup_AddressesUDPRN(ourviewmodel,
                                                    ourviewmodel.UserName,
                                                    udprn);
                if (addresses_found.Count > 0)
                {
                    area_code = addresses_found.First().AREA_CODE; // Might be 0?
                }
                postcode = addresses_found.First().POSTCODE;

                //if (financeviewmodel.area_code == 0)
                //{
                //    // This IS a show-stopper because the Area code should never be 0
                //    // If it is ever ZERO, then its probably because there
                //    // Is NO ADDRESS assigned to an account, and that's fatal
                //    // No Financial institution is going to set up an account for
                //    // an entity (person or business) at a location for which it doesn't have an address...
                //    //
                //    // But then there comes Crypto which doesn't know anything about
                //    // Area Codes ... but it DOES know Country Codes e.g. 44 for UK
                //    // But its cuppa tea time!!

                //    await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institution_code, brand_code, SmartParametersV2016.Finance.ToString() + " " +
                //                            financeviewmodel.categoryCodes +
                //                            " Account " +
                //                            financeviewmodel.accountsFound.First().ACCOUNT_NO +
                //                            " has Area code: " + financeviewmodel.area_code);

                //    // Think this is the only place where we return a false - No it isn't
                //    return false;
                //}
#if WINFORMS || WPF || SMARTMAUI
                financeviewmodel.AreaId = area_code.ToString();
#endif
#if WINUI
                financeviewmodel.AreaId = area_code.ToString();
#endif
#if ANDROIDX
                financeviewmodel.AreaId.Text = area_code.ToString();
#endif                
            }

            if (!string.IsNullOrEmpty(postcode))
            {
                // Sort of 'center' this - there are approximately 2 spaces for each char
#if WINFORMS || WPF || SMARTMAUI
                // Pad the postcode out to 12 (sp it's 'central'??)
                financeviewmodel.PostCode = SmartRoutinesV2018.Check_Postcodes(postcode);
#endif
#if WINUI
                // Pad the postcode out to 12 (sp it's 'central'??)
                financeviewmodel.PostCode = SmartRoutinesV2018.Check_Postcodes(postcode);
#endif
#if ANDROIDX
                // Don't bother for Android, the view should have it central
                financeviewmodel.PostCode.Text = postcode;
#endif
            }
#if WINFORMS || WPF || SMARTMAUI
            financeviewmodel.NextConnectionMessage = SmartRoutinesV2018.Check_Next_Connection(financeviewmodel.CultureINF,
                                                                                            SmartParametersV2016.none,
                                                                                            next_connection,
                                                                                            SmartParametersV2016.defaultDate);
#endif
#if WINUI
            financeviewmodel.NextConnectionMessage = SmartRoutinesV2018.Check_Next_Connection(financeviewmodel.CultureINF,
                                                                                            SmartParametersV2016.none,
                                                                                            next_connection,
                                                                                            SmartParametersV2016.defaultDate);
#endif
#if ANDROIDX
            financeviewmodel.NextConnectionMessage.Text = SmartRoutinesV2018.Check_Next_Connection(financeviewmodel.CultureINF,
                                                                                                SmartParametersV2016.none,
                                                                                                next_connection,
                                                                                                SmartParametersV2016.defaultDate);
#endif
            //            // INSTITUTIONS IS STATIC
            //            // Don't forget => Institutions is a SPINNER
            //            //              => Providers is a MULTIPICKER
            //            //              => Accounts is a MULTIPICKER
            //            //              => Transactions is a MULTIPICKER
            //            //              => Addresses is a SPINNER but should be a MULTIPICKER
            //            if (!Finance_Build_Institutions_Dropdown(ourviewmodel,
            //                                                            financeviewmodel,
            //                                                            activeFlag))
            //            {
            //                financeviewmodel.errorMessage = "Finance Build Institutions failed";
            //                return false;
            //            }
            //            else
            //            {
            //                if (financeviewmodel.FinanceInstitutionsList.Count > 0)
            //                {
            //#if WINUI
            //                financeviewmodel.FinanceInstitutionsText = financeviewmodel.FinanceInstitutionsList.First().Content;
            //                financeviewmodel.FinanceInstitutionsColour = financeviewmodel.FinanceInstitutionsList.First().Colour;
            //#endif
            //                }
            //            }

            financeviewmodel.contrastColour = true;
            if (financeviewmodel.PLO.transactionscategoriesList.Count == 0)
            {
#if WINFORMS
                if (financeviewmodel.StartDate == SmartParametersV2016.defaultDate &&
                    financeviewmodel.EndDate == SmartParametersV2016.defaultMaxdate)
#endif
#if WPF || SMARTMAUI
                if (financeviewmodel.StartDate == SmartParametersV2016.defaultDate &&
                        financeviewmodel.EndDate == SmartParametersV2016.defaultMaxdate)
#endif
#if WINUI
                    if (financeviewmodel.StartDate == SmartParametersV2016.defaultDate &&
                            financeviewmodel.EndDate == SmartParametersV2016.defaultMaxdate)
#endif
#if ANDROIDX
                if (financeviewmodel.StartDate.DateTime == SmartParametersV2016.defaultDate &&
                    financeviewmodel.EndDate.DateTime == SmartParametersV2016.defaultMaxdate)
#endif
                    {
                        financeviewmodel.StartDateEnabled =
                    financeviewmodel.EndDateEnabled = false;
                    // Set the start and end dates to be TODAY
#if WINFORMS
                    financeviewmodel.StartDate =
                    financeviewmodel.EndDate = DateTime.Today;
#endif
#if WPF || SMARTMAUI
                    financeviewmodel.StartDate =
                    financeviewmodel.EndDate = DateTime.Today;
#endif
#if WINUI
                    financeviewmodel.StartDate =
                    financeviewmodel.EndDate = DateTime.Today;
#endif
#if ANDROIDX
                    financeviewmodel.StartDate.DateTime =
                    financeviewmodel.EndDate.DateTime = DateTime.Today;
#endif
                    financeviewmodel.StartDateEnabled =
                    financeviewmodel.EndDateEnabled = true;
                }
            }

            financeviewmodel.transaction_groupsfound = SmartSpikeFinanceV2017.Finance_Lookup_Transaction_Groups(ourviewmodel,
                                                                financeviewmodel,
                                                                institution_code,
                                                                brand_code);


            FinanceTransactionsComplete(
#if WINFORMS
                                            process_components,
                                            process_components.comboBoxGUIFinanceProviders,
                                            process_components.comboBoxGUIFinanceAccounts,
                                            process_components.comboBoxGUIFinanceTransactionGroups,
                                            process_components.TransactionsDataGrid,
#endif
                                            ourviewmodel,
                                            financeviewmodel,
                                            //Convert.ToChar(financeviewmodel.categoryCodes), // Assume, initially its only ONE just to get compiled!
#if WINFORMS
                                            financeviewmodel.StartDate,
                                            financeviewmodel.EndDate);
#endif
#if WPF || SMARTMAUI
                                            financeviewmodel.StartDate,
                                            financeviewmodel.EndDate);
#endif
#if WINUI
            financeviewmodel.StartDate,
                                            financeviewmodel.EndDate);
#endif
#if ANDROIDX
                                            meterActivity,
                                            financeviewmodel.StartDate.DateTime,
                                            financeviewmodel.EndDate.DateTime);
#endif
            if (financeviewmodel.FinanceTransactions.Count > 0)
            {
                // See if we want the Username column to be displayed or not
#if WPF || SMARTMAUI
                CheckActiveProfiles(ourviewmodel, financeviewmodel);

#endif
#if WINUI
                CheckActiveProfiles(ourviewmodel, financeviewmodel);

#endif
#if WINFORMS || WPF || SMARTMAUI
                financeviewmodel.SortCode = financeviewmodel.FinanceTransactions.First().SORTCODE;
                financeviewmodel.AccountNo = financeviewmodel.FinanceTransactions.First().ACCOUNT_NO;

#endif
#if WINUI
                financeviewmodel.SortCode = financeviewmodel.FinanceTransactions.First().SORTCODE;
                financeviewmodel.AccountNo = financeviewmodel.FinanceTransactions.First().ACCOUNT_NO;

#endif
#if ANDROIDX
                financeviewmodel.SortCode.Text = financeviewmodel.FinanceTransactions.First().SORTCODE;
                financeviewmodel.AccountNo.Text = financeviewmodel.FinanceTransactions.First().ACCOUNT_NO;
#endif

                
                financeviewmodel.dno_stream = System.IO.Stream.Null;

                // Go and find the image
                // Look in the table first
                // I'm not 1000% sure this 'caching' is needed ...
                // but I'm going to leave it in for the time being
                // in case I need it for the Transaction items .. which I'm doing now
                string brand_name = "";
                List<SmartFinance.Brands> brands_found = SmartSpikeFinanceV2017.Finance_Find_Brands(ourviewmodel,
                                                                                            financeviewmodel,
                                                                                            financeviewmodel.accountsFound.First().INSTITUTION_CODE,
                                                                                            financeviewmodel.accountsFound.First().BRAND_CODE);
                if (brands_found.Count > 0)
                {
                    brand_name = brands_found.First().BRAND_NAME;
                    financeviewmodel.PictureBoxLOGO = Lookup_LOGO(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                       financeviewmodel,
                                                       SmartParametersV2016.Finance,
                                                       brand_name);
                    if (financeviewmodel.PictureBoxLOGO == null)
                    {
                        financeviewmodel.PictureBoxLOGO = await Download_LOGOAsync(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                       financeviewmodel,
                                                       SmartParametersV2016.Finance,
                                                       brand_name);
                        // It may still be blank here ...! But it shouldn't be!!
                    }
                }
            }

            List<SmartFinance.Switches> switches_found =
                SmartSpikeFinanceV2017.Finance_Lookup_Switches(ourviewmodel,
                                                                financeviewmodel,
                                                                institution_code,
                                                                brand_code);
            if (switches_found.Count > 0)
            {
                if (switches_found.First().AUTOSWITCH_EMAIL_SENT != SmartParametersV2016.defaultDate)
                {
#if WINFORMS
                    financeviewmodel.LedSwitched = true;
#endif
#if WPF || SMARTMAUI
                    financeviewmodel.LedSwitched = Visibility.Visible;
#endif
#if WINUI
                    financeviewmodel.LedSwitched = Visibility.Visible;
#endif
#if ANDROIDX
                    financeviewmodel.LedSwitched.Text = "0";
#endif
                }
                else
                {
#if WINFORMS
                    financeviewmodel.LedSwitched = false;
#endif
#if WPF || SMARTMAUI
                    financeviewmodel.LedSwitched = Visibility.Hidden;
#endif
#if WINUI
                    financeviewmodel.LedSwitched = Visibility.Collapsed;
#endif
#if ANDROIDX

                    financeviewmodel.LedSwitched.Text = "0";
#endif
                }
            }

        // Exchange Rates are 'done' by 'binding' to ourviewmodel...
        // (fingers fucking well crossed of course)

        dropdowns:
            // Build dropdowns
            // This now ONLY Institutions
            if (!Finance_Build_Institutions_Dropdown(
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    activeFlag))
            {
                financeviewmodel.errorMessage = "Finance Build Institutions failed";
                return false;
            }
            else
            {
                if (financeviewmodel.FinanceInstitutionsList.Count > 0)
                {
#if WPF || SMARTMAUI
                    financeviewmodel.FinanceInstitutionsText = financeviewmodel.FinanceInstitutionsList.First().Content;
                    financeviewmodel.FinanceInstitutionsColour = financeviewmodel.FinanceInstitutionsList.First().Colour;
#endif
#if WINUI
                    financeviewmodel.FinanceInstitutionsText = financeviewmodel.FinanceInstitutionsList.First().Content;
                    financeviewmodel.FinanceInstitutionsColour = financeviewmodel.FinanceInstitutionsList.First().Colour;
#endif
                }
            }

            // Data for building dropdowns
            //            financeviewmodel.area_code = area_code;
            //            financeviewmodel.displayed_resource_type = displayed_resource_type;
            //            financeviewmodel.displayed_institution_code = displayed_institution_code;
            //            financeviewmodel.displayed_brand_code = displayed_brand_code;

            //            urgent_message = Build_Finance_Dropdowns(financeviewmodel);
            //            if (!string.IsNullOrEmpty(urgent_message))
            //            {
            //                await SmartBobV2017.ListenerAsync(ourviewmodel,  financeviewmodel.displayed_brand_code, financeviewmodel.displayed_institution_code, SmartParametersV2016.space + urgent_message);
            //                return false;
            //            }
            //            // We have NO Providers, Accounts or Transactions dropdowns

            //            // At this point, we haven't scraped anything - these are just loaded in from before
            //            if (!SmartSpikeFinanceV2017.Find_Bank_Transactions(ourviewmodel,
            //                                                        financeviewmodel,
            //                                                        SmartParametersV2016.defaultDate,
            //                                                        ourviewmodel.time_now))

            //            Can't select any by choosing them UNLESS they have a Connection i.e they are in
            //            Upper Case (WINUI) or they are a green colour (Android)
            //            
            //            All of what follows below is bollocks:
            //            You can't select an Institution.  If you set SelectedIndex - and the Item is neither
            //            uppercase or green, then the event handler will happen and the popup will appear from
            //            the getgo.
            //            If there is a connection, and the Item is either uppercase or green, and you set
            //            SelectedIndex, then the popup will also appear from the start.  So either you, you
            //            will see the popup simply because of the SelectedIndex ... and you can't stop the
            //            SelectedIndex appearing because a) you want it to happen but b) ONLY when you are
            //            creating a Connection or changing a connection.
            //            The difference between the Institutions and Providers is that the information we
            //            need for Institutions is EXTERNAL (hence the need for a popup)

            //            The logic for Institutions DOESN'T apply to the Providers because a) we can have
            //            more than one and b) there is no popup to appear i.e. all the info we need is INTERNAL
            //            i.e. from the Bank Transactions.              
            return true;
        }

        internal static void CheckActiveProfiles(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {
            // We have updated one (or more) of the Groups checkboxes
            // so now we need to display the Username column in
            // the FinanceTransactions page. If none of the Groups
            // checkboxes are ticked then we can remove the Username column
            // What an absolute fag!!
            List<SmartProfile.Groups> profilesActive = new List<SmartProfile.Groups>
            (from Profile in ourviewmodel.SmartProfile.profilegroupsList
             where Profile.ACTIVEFLAG == SmartParametersV2016.active
             select Profile
            );
            if (profilesActive.Count > 0)
            {
#if WPF || SMARTMAUI
                FrontEndGUI.DecodeListViewUsername(financeviewmodel, true);
#endif
#if WINUI
                FrontEndGUI.DecodeListViewUsername(financeviewmodel, true);
#endif
            }
            else
            {
#if WPF || SMARTMAUI
                FrontEndGUI.DecodeListViewUsername(financeviewmodel, false);
#endif
#if WINUI
                FrontEndGUI.DecodeListViewUsername(financeviewmodel, false);
#endif
            }
            return;
        }

        internal static void ClearDownEverythingFinance(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel)//,
                                                                              //string none)
        {
            if (ourviewmodel != null &&
                financeviewmodel != null)
            {
                //financeviewmodel.NextConnectionMessage = none; Leave this alone
#if WINFORMS || WPF || SMARTMAUI
                financeviewmodel.PostCode = "";
#endif
#if WINUI
                financeviewmodel.PostCode = "";
#endif
#if ANDROIDX
                financeviewmodel.PostCode.Text = "";
#endif
#if WPF || SMARTMAUI
                financeviewmodel.PictureBoxLOGO = null;
#endif
#if WINUI
                financeviewmodel.PictureBoxLOGO = null;
#endif
#if ANDROIDX
                Bitmap bitMapImageKeasdon = BitmapFactory.DecodeFile(@"Images/Images/keasdon_energy_small.jpg");
                financeviewmodel.PictureBoxLOGO.SetImageBitmap(bitMapImageKeasdon);
#endif
#if WINFORMS
                financeviewmodel.LedSwitched = false;
#endif
#if WPF || SMARTMAUI
                financeviewmodel.LedSwitched = Visibility.Hidden;
#endif
#if WINUI
                financeviewmodel.LedSwitched = Visibility.Collapsed;
#endif
#if ANDROIDX
                financeviewmodel.LedSwitched.Text = "0";
#endif
#if WINFORMS || WPF || SMARTMAUI
                financeviewmodel.SortCode = "<none>";
                financeviewmodel.AccountNo = "<none>";
                financeviewmodel.TotalTransactions = "0";
                financeviewmodel.TotalValue = "0";
#endif
#if WINUI
                financeviewmodel.SortCode = "<none>";
                financeviewmodel.AccountNo = "<none>";
                financeviewmodel.TotalTransactions = "0";
                financeviewmodel.TotalValue = "0";
#endif
#if ANDROIDX
                financeviewmodel.SortCode.Text = "<none>";
                financeviewmodel.AccountNo.Text = "<none>";
                financeviewmodel.TotalTransactions.Text = "0";
                financeviewmodel.TotalValue.Text = "0";
#endif
            }
            return;
        }

        // Its ok to use async void on Event Handlers
        internal static void FinanceUpdateTransactions(FinanceViewModel financeviewmodel)

        {
            List<SmartFinance.CommonTransactionsView>
                temp_found = new List<SmartFinance.CommonTransactionsView>();
            if (financeviewmodel.bookingsort)
            {
                // True means Descending 
                temp_found = new List<SmartFinance.CommonTransactionsView>
                    (from Transactions in financeviewmodel.FinanceTransactions
                     orderby Transactions.TRANSACTION_DATE descending,
                             Transactions.SEQUENCE_NO descending
                     select Transactions);
            }
            else
            {
                // False means Ascending
                temp_found = new List<SmartFinance.CommonTransactionsView>
                    (from Transactions in financeviewmodel.FinanceTransactions
                     orderby Transactions.TRANSACTION_DATE ascending,
                             Transactions.SEQUENCE_NO ascending
                     select Transactions);
            }
            financeviewmodel.FinanceTransactions = temp_found;
            return;
        }

        internal static decimal ReturnValue(FinanceViewModel financeviewmodel,
                                            string ConvertToSymbol,      // The one we want
                                            string ConvertFromSymbol,              // The one we've got
                                            int ConvertFromOrdinal,
                                            decimal[] exchangeRate,
                                            decimal value)
        {
            if (financeviewmodel.CurrencyOrdinal > 0)
            {
                // See if they're the same
                if (ConvertToSymbol != ConvertFromSymbol)
                {
                    if (ConvertFromOrdinal > 1) // i.e. non-GBP
                    {
                        value = value / exchangeRate[ConvertFromOrdinal - 1];
                    }
                    // They're not - do a conversion
                    if (exchangeRate[financeviewmodel.CurrencyOrdinal - 1] != 1.0m)
                    {
                        value = exchangeRate[financeviewmodel.CurrencyOrdinal - 1] * value;
                    }
                }
            }
            return value;
        }

        internal static string DisplayValue(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            string ConvertToSymbol,      // The one we want
                                            string ConvertFromSymbol,              // The one we've got
                                            int ConvertFromOrdinal,
                                            decimal[] exchangeRate,
                                            decimal value)
        {
            if (financeviewmodel.CurrencyOrdinal > 0)
            {
                // See if they're the same
                if (ConvertToSymbol != ConvertFromSymbol)
                {
                    if (ConvertFromOrdinal > 1) // i.e. non-GBP
                    {
                        value = value / exchangeRate[ConvertFromOrdinal - 1];
                    }
                    // They're not - do a conversion
                    if (exchangeRate[financeviewmodel.CurrencyOrdinal - 1] != 1.0m)
                    {
                        value = exchangeRate[financeviewmodel.CurrencyOrdinal - 1] * value;
                    }
                }
            }
            return SmartSpikeFinanceV2017.GetCultureView(ourviewmodel,
                                                    financeviewmodel,
                                                    ConvertFromSymbol,
                                                    ConvertToSymbol,
                                                    value);
        }
        internal static void UndisplayFinanceMeter(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            char local_category_code,
                                            string meterFormat,
                                            string currencyFormat)
        {
#if WINFORMS
            //decimal total_reading = 0.0M;
            //int billed_amount = 0,
            //    projectedCost = 0;
#endif
            string none = "          <none>           ";

            List<string> resource_codeList = new List<string>() { };

            
            // Do we need to clear the colours as well?
            List<SmartFinance.Switches> switches_found = SmartSpikeFinanceV2017.Finance_Find_CategoryLASTDISPLAYANY(ourviewmodel,
                                                                                                                financeviewmodel);
            if (switches_found.Count == 0)
            {
#if WINFORMS || WPF || SMARTMAUI
                financeviewmodel.NextConnectionMessage = none;
                financeviewmodel.SortCode = "";
                financeviewmodel.AccountNo = "";
                financeviewmodel.PostCode = "";
#endif
#if WINUI
                financeviewmodel.NextConnectionMessage = none;
                financeviewmodel.SortCode = "";
                financeviewmodel.AccountNo = "";
                financeviewmodel.PostCode = "";
#endif
#if ANDROIDX
                financeviewmodel.NextConnectionMessage.Text = none;
                financeviewmodel.SortCode.Text = "";
                financeviewmodel.AccountNo.Text = "";
                financeviewmodel.PostCode.Text = "";
#endif
                ClearDownEverythingFinance(ourviewmodel, financeviewmodel);
            }
            return;
        }

        internal static void ClearDownTransactions(
#if WINFORMS
                                                MainProcess components,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {
#if WINFORMS
            financeviewmodel.TabControlPanel.TabPages.Clear();
#endif
#if WPF
            financeviewmodel.TabControlPanel.Items.Clear();
            financeviewmodel.grids.Clear();
#endif
#if WINUI
            financeviewmodel.TabControlPanel.TabItems.Clear();
            financeviewmodel.grids.Clear();
#endif
#if ANDROIDX
            financeviewmodel.TabTitles.Clear();
#endif
#if SMARTMAUI
            //financeviewmodel.TabControlPanel.Children.Clear();
            financeviewmodel.grids.Clear();
#endif
            financeviewmodel.FinanceTotals = new List<SmartFinance.TotalsView>[ourviewmodel.currenciesList.Count, 3];
            short tabIndex = 0;
            foreach (SmartData.Currencies abc in ourviewmodel.currenciesList)
            {
                financeviewmodel.FinanceTotals[tabIndex, 0] = new List<SmartFinance.TotalsView>();
                financeviewmodel.FinanceTotals[tabIndex, 1] = new List<SmartFinance.TotalsView>();
                financeviewmodel.FinanceTotals[tabIndex, 2] = new List<SmartFinance.TotalsView>();
                tabIndex++;
            }
            financeviewmodel.PaidInFontSize =
                        financeviewmodel.PaidOutFontSize =
                        financeviewmodel.DifferenceFontSize = 12;
            return;
        }

        internal static string Check_Brand_Area(FinanceViewModel financeviewmodel,
                                                short local_institution_code,
                                                short local_brand_code,
                                                FieldInfo[] myFields)
        {
            string brand_name = "";
            // Is it really necessary to return a brand_name list when the brand code
            // only ever appears once for a Brand or Supplier in Brand Area Matrix??
            int field_index = SmartNibbyV2016.Area_Matrix_Index(financeviewmodel.area_code, myFields);
            if (field_index >= 0)
            {
                List<SmartFinance.BrandMatrix> brand_matrix_found = SmartSpikeFinanceV2017.Finance_Find_BrandMatrix(financeviewmodel,
                                                                                                local_institution_code,
                                                                                                local_brand_code);
                int row_count = 0;
                foreach (SmartFinance.BrandMatrix brand_matrix_row in brand_matrix_found)
                {
                    if (Convert.ToChar(myFields[field_index].GetValue(brand_matrix_row).ToString()) == SmartParametersV2016.brandMatrixValid)
                    {
                        if (string.IsNullOrEmpty(brand_name))
                        {
                            //brand_name = brand_matrix_row.;
                        }
                        row_count++;
                    }
                }
                if (row_count != brand_matrix_found.Count)
                {
                    brand_name = "";
                }
            }
            return brand_name;
        }

        internal static bool Lookup_Finance_Consumer_Info(FinanceViewModel financeviewmodel)
        {
            // If we are simulating, then stop ...
            char local_category_code = Finance_Find_Account_Type(financeviewmodel);
            if (financeviewmodel.FinanceProvidersList.Count > 0) // I have no idea why this is here ...
            {
                // Find the UDPRN
                switch (local_category_code)
                {
                    // 
                    // THESE LOOK ALL THE SAME TO ME????
                    //
                    case SmartParametersV2016.Banks:
                        if (financeviewmodel.FinanceAddressesList.Count > 0)
                        {
                            foreach (MainViewModel.AddressItem address_item in financeviewmodel.FinanceAddressesList)
                            {
                                financeviewmodel.udprn = address_item.Value.ToString();
                                break;
                            }
                        }
                        break;
                    case SmartParametersV2016.Investments:
                        if (financeviewmodel.FinanceAddressesList.Count > 0)
                        {
                            foreach (MainViewModel.AddressItem address_item in financeviewmodel.FinanceAddressesList)
                            {
                                financeviewmodel.udprn = address_item.Value;
                                break;
                            }
                        }
                        break;
                    case SmartParametersV2016.Savings:
                        if (financeviewmodel.FinanceAddressesList.Count > 0)
                        {
                            foreach (MainViewModel.AddressItem address_item in financeviewmodel.FinanceAddressesList)
                            {
                                financeviewmodel.udprn = address_item.Value;
                                break;
                            }
                        }
                        break;
                    case SmartParametersV2016.Cryptos:
                        if (financeviewmodel.FinanceAddressesList.Count > 0)
                        {
                            foreach (MainViewModel.AddressItem address_item in financeviewmodel.FinanceAddressesList)
                            {
                                financeviewmodel.udprn = address_item.Value;
                                break;
                            }
                        }
                        break;
                    default:
                        if (financeviewmodel.FinanceAddressesList.Count > 0)
                        {
                            foreach (MainViewModel.AddressItem address_item in financeviewmodel.FinanceAddressesList)
                            {
                                financeviewmodel.udprn = address_item.Value;
                                break;
                            }
                        }
                        break;
                }
            }
            return false;
        }

        // This is a REAL fucking kludge!
        internal static async Task<bool> Cancel_Finance_Meter_Async(MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)
        {
            // So - write this assuming you have two scenarios:
            // 1. Different Suppliers / User Ids and Passwords
            //    which implies different LAST_DATETIMEs and NEXT_CONNECTIONs
            //
            //    For each Supplier pull out the User Id, Password and
            //    Next Connection.
            //
            // 2. Same Suppliers which implies Same User Ids and Passwords
            //    which also implies same NEXT_CONNECTIONs (the LAST_DATETIMEs might
            //    still indeed be different for Electricity and Gas
            //
            // How are you going to code this?
            // For all the Consumer Energy records, check the Next Connections
            //
            //  If you find none that needs SUBMITting then exit
            //  If you find one that needs SUBMITting, then check its Supplier with the
            //  other one (which doesn't need SUBMITting).
            //      If the Suppliers are the same adjust the non-SUBMIT down to the SUBMIT
            //      so that both will get processed and updated
            //      If the Suppliers aren't the same, then leave the non-SUBMIT alone
            //  If you find both need SUBMITting then check the Suppliers.
            //      If the are equal, then its a classic dual-tariff and do both in one
            //      read of the Supplier
            //      If they are different, then do them in order of Consumer Energy record
            //      sequence
            int returned_codes;// = financeviewmodel.returned_codes;

            short displayed_area_code = 0,
                    displayed_brand_code = 0;
            // We never come in from the Clock we always come in from a Cancel
            // From the Cancel - we can do this with ZERO Consumer Energy Records

            List<SmartFinance.Accounts> accounts_found = SmartSpikeFinanceV2017.Finance_Lookup_Accounts(ourviewmodel,
                                                                                                                    financeviewmodel);
            //SmartParametersV2016.defaultDate);
            //List<SmartFinance.Banks> first_mpan_mprnList = new List<SmartFinance.Banks>();
            //// Examine all we can find
            //// Always a CANCEL?
            //switch (first_resource_code)
            //{
            //    case SmartParametersV2016.Banks:
            //        first_mpan_mprnList = SmartSpikeFinanceV2017.Lookup_Banks_By_Resource(army_code,
            //                                                            first_resource_code,
            //                                                            Hamas.banksList);
            //        break;
            //    case SmartParametersV2016.Investments:
            //        first_mpan_mprnList = SmartSpikeFinanceV2017.Lookup_Banks_By_Resource(army_code,
            //                                                            first_resource_code,
            //                                                            Hamas.banksList);
            //        break;
            //    case SmartParametersV2016.Savings:
            //        first_mpan_mprnList = SmartSpikeFinanceV2017.Lookup_Banks_By_Resource(army_code,
            //                                                            first_resource_code,
            //                                                            Hamas.banksList);
            //        break;
            //    default:
            //        break;
            //}

            if (accounts_found.Count > 0)
            {
                short institution_code = accounts_found.First().INSTITUTION_CODE;
                short brand_code = accounts_found.First().BRAND_CODE;

                //financeviewmodel.category_code = accounts_found.First().CATEGORY_CODE;
                List<SmartFinance.Switches> switches_found = SmartSpikeFinanceV2017.Finance_Lookup_Switches(ourviewmodel,
                                                                                                            financeviewmodel,
                                                                                                            institution_code,
                                                                                                            brand_code);
                // Make sure we do Institutions and their Finance.Accounts one after the other
                if (switches_found.Count > 0)
                {
                    // The difference between Finance and Utility is that the Banks (as of yet!)
                    // don't have one of their devices
                    // installed in your home; hence there is no requirement for an MPAN_MPRN fields


                    //DateTime next_conn;// For when we fix next connections
                    foreach (SmartFinance.Switches switches_row in switches_found)
                    {
                        // Should be doing the latest here ...
                        //displayed_area_code = consumers_view_found.First().AREA_CODE;
                        displayed_brand_code = switches_found.First().BRAND_CODE;
                        // Despite everything and all the pain
                        // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
                        //switches_row.NEXT_CONNECTION = next_conn;
                        if (!string.IsNullOrEmpty(switches_row.ToString()))
                        {
                            switches_row.Updated = true;
                            financeviewmodel.PLO.finance_switches_changesList.Add(switches_row);
                            if (!await SmartFinanceScrapeV2023.DoAllSmartFinance(ourviewmodel,
                                                            financeviewmodel,
                                                            false,
                                                            SmartParametersV2016.sqliteformat))
                            {
#if WPF || SMARTMAUI
                                financeviewmodel.errorMessage = "Cannot update switches(1) - cannot continue";
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                // This is a bit fatal, because if we can't do this, there's
                                // every chance we can't do lots of things we need to further on
#endif
#if WINUI
                                financeviewmodel.errorMessage = "Cannot update switches(1) - cannot continue";
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                // This is a bit fatal, because if we can't do this, there's
                                // every chance we can't do lots of things we need to further on
#endif
                                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, ourviewmodel.errorMessage))
                                {
                                    return false;
                                }
                                //  Led8 turned to Red on return
                                return false;
                            }
                            //financeviewmodel.NextConnectionMessage = financeviewmodel.next_connection.ToString(financeviewmodel.financeDisplayCulture);
                            // Try and find the same User Id for this Supplier
                            // on the opposite Energy Code.  Even if the Supplier is
                            // used on more than one record, its the User Id access
                            // we want to Cancel
                            //List<SmartFinance.Banks> second_mpan_mprnList = new List<SmartFinance.Banks>();
                            //switch (first_resource_code)
                            //{
                            //    case SmartParametersV2016.Banks:
                            //        second_resource_code = SmartParametersV2016.Gas;
                            //        second_mpan_mprnList = first_mpan_mprnList = SmartSpikeFinanceV2017.Lookup_Banks_By_Resource(arym_code,
                            //                                                    second_resource_code,
                            //                                                    Hamas.banksList);
                            //        break;
                            //    case SmartParametersV2016.Investments:
                            //        second_resource_code = SmartParametersV2016.Electricity;
                            //        second_mpan_mprnList = first_mpan_mprnList = SmartSpikeFinanceV2017.Lookup_Banks_By_Resource(army_code,
                            //                                                    second_resource_code,
                            //                                                    Hamas.banksList);
                            //        break;
                            //    case SmartParametersV2016.Savings:
                            //        second_resource_code = SmartParametersV2016.Electricity;
                            //        second_mpan_mprnList = first_mpan_mprnList = SmartSpikeFinanceV2017.Lookup_Banks_By_Resource(army_code,
                            //                                                    second_resource_code,
                            //                                                    Hamas.banksList);
                            //        break;
                            //    default:
                            //        break;
                            //}

                            //foreach (SmartFinance.Banks m_row in second_mpan_mprnList)
                            //{
                            //    second_mpan_mprn = m_row.ACCOUNT_NO;
                            //    break;
                            //}
                            List<SmartFinance.Switches> finance2_found = SmartSpikeFinanceV2017.Finance_Consumer_CancelList(ourviewmodel, financeviewmodel);


                            if (finance2_found.Count > 0)
                            {
                                foreach (SmartFinance.Switches finance2_row in finance2_found)
                                {
                                    //finance2_row.NEXT_CONNECTION = next_conn;
                                    if (!string.IsNullOrEmpty(switches_row.ToString()))
                                    {
                                        finance2_row.Updated = true;
                                        financeviewmodel.PLO.finance_switches_changesList.Add(finance2_row);
                                        if (!await SmartFinanceScrapeV2023.DoAllSmartFinance(ourviewmodel,
                                                                        financeviewmodel,
                                                                        false,
                                                                        SmartParametersV2016.sqliteformat))
                                        {
#if WPF || SMARTMAUI
                                            financeviewmodel.errorMessage = "Cannot update switches(2) - cannot continue";
                                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                            // This is a bit fatal, because if we can't do this, there's
                                            // every chance we can't do lots of things we need to further on
#endif
#if WINUI
                                            financeviewmodel.errorMessage = "Cannot update switches(2) - cannot continue";
                                            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                            // This is a bit fatal, because if we can't do this, there's
                                            // every chance we can't do lots of things we need to further on
#endif
                                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, ourviewmodel.errorMessage))
                                            {
                                                return false;
                                            }
                                            // Led8 turned to Red on return
                                            return false;
                                        }
                                    }
                                    break;  // Only the latest
                                }
                            }
                        }
                        break; // Only the latest
                    }
                }
            }
            // Led6 turned to Orange on return
            returned_codes = displayed_brand_code + (displayed_area_code * 256);
            financeviewmodel.returned_codes = returned_codes;
            return true;
        }

        
#if WINFORMS
        internal static System.Drawing.Image Lookup_LOGO(MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    char cubefaceCode,
                                                                    string brand_name)

#endif
#if WPF
        internal static BitmapImage Lookup_LOGO(MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    char cubefaceCode,
                                                                    string brand_name)
#endif
#if WINUI
        internal static BitmapImage Lookup_LOGO(MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    char cubefaceCode,
                                                                    string brand_name)
#endif
#if ANDROIDX
        internal static ImageView Lookup_LOGO(
                                                Context context,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                char cubefaceCode,
                                                string brand_name)
#endif
#if SMARTMAUI
        internal static ImageSource Lookup_LOGO(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                char cubefaceCode,
                                                string brand_name)
#endif
         {
#if WINFORMS
            // This is needed to initialize it properly
            System.Drawing.Image logo = new Bitmap(1, 1);
#endif
#if WPF
            BitmapImage logo = new BitmapImage();
#endif
#if WINUI
            BitmapImage logo = new BitmapImage();
#endif
#if ANDROIDX
            ImageView logo = new ImageView(context);
#endif
#if SMARTMAUI
            ImageSource logo = null;
#endif
            if (!string.IsNullOrEmpty(brand_name))
            {
                foreach (MainViewModel.ImageItem itemView in ourviewmodel.ImageList)
                {
                    if (itemView.Cubeface == cubefaceCode &&
                        itemView.ImageName == brand_name)
                    {
                        //
                        // My God in Heaven, this stuff is SHIT
                        //                                 ====
                        // For reasons best know to the Chimps, when you
                        // re-read a Stream the position is at the END!!
#if WINFORMS
                        logo = System.Drawing.Image.FromStream(itemView.StreamBits);                        
#endif
#if WPF
                        itemView.StreamBits.Position = 0;
                        logo.BeginInit();
                        logo.StreamSource = itemView.StreamBits;//   new Uri("your-image.jpg", UriKind.Relative);
                        logo.DecodePixelWidth = 100; // match display size
                        logo.CacheOption = BitmapCacheOption.OnLoad;
                        logo.EndInit();
#endif
#if WINUI
                        // Conversion from one stream to another (yawn)
                        InMemoryRandomAccessStream randomaccessStream = new InMemoryRandomAccessStream();
                        using (System.IO.Stream outputStream = randomaccessStream.AsStreamForWrite())
                        {
                            itemView.StreamBits.CopyTo(outputStream);
                            outputStream.FlushAsync();
                        }
                        randomaccessStream.Seek(0); // Reset position
                        logo.SetSource(randomaccessStream);
#endif
#if ANDROIDX
                        Android.Graphics.Bitmap bmap = BitmapFactory.DecodeStream(itemView.StreamBits);
                        logo.SetImageBitmap(bmap);
#endif
#if SMARTMAUI
                        logo = ImageSource.FromStream(() =>
                        {
                            var ms = new MemoryStream();
                            itemView.StreamBits.Position = 0;
                            itemView.StreamBits.CopyTo(ms);
                            ms.Position = 0;
                            return ms;
                        });
#endif
                        return logo;
                    }
                }
            }
            return null;
        }

#if WINFORMS
        internal static async Task<System.Drawing.Image> Download_LOGOAsync(MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    char cubefaceCode,
                                                                    string brand_name)

#endif
#if WPF || SMARTMAUI
        internal static async Task<ImageSource> Download_LOGOAsync(MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    char cubefaceCode,
                                                                    string brand_name)
#endif
#if WINUI
        internal static async Task<ImageSource> Download_LOGOAsync(MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    char cubefaceCode,
                                                                    string brand_name)
#endif
#if ANDROIDX
        internal static async Task<ImageView> Download_LOGOAsync(
                                                                    Context context,
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    char cubefaceCode,
                                                                    string brand_name)
#endif
        {
#if WINFORMS
            // This is needed to initialize it properly
            System.Drawing.Image logo = new Bitmap(1, 1);            
#endif
#if WPF
            BitmapImage logo = new BitmapImage();
#endif
#if WINUI
            BitmapImage logo = new BitmapImage();
#endif
#if ANDROIDX
            ImageView logo = new ImageView(context);
#endif
#if SMARTMAUI
            ImageSource logo = null;
#endif
            // This SHOULD never fail ...
            if (!string.IsNullOrEmpty(brand_name))
            {
                if (await SmartBobV2017.Load_LOGO_Async(ourviewmodel,
                                                            financeviewmodel,
                                                            financeviewmodel.financeToken,
                                                            cubefaceCode,
                                                            brand_name))
                {
                    if (financeviewmodel.dno_stream != null)
                    {
                        MainViewModel.ImageItem itemView = new MainViewModel.ImageItem()
                        {
                            Cubeface = cubefaceCode,
                            ImageName = brand_name,
                            StreamBits = financeviewmodel.dno_stream
                        };
                    
                        ourviewmodel.ImageList.Add(itemView);
                        itemView.StreamBits.Position = 0;
#if WINFORMS
                        logo = System.Drawing.Image.FromStream(financeviewmodel.dno_stream);

#endif
#if WPF

                        logo.BeginInit();
                        logo.StreamSource = itemView.StreamBits;//   new Uri("your-image.jpg", UriKind.Relative);
                        logo.DecodePixelWidth = 100; // match display size
                        logo.CacheOption = BitmapCacheOption.OnLoad;
                        logo.EndInit();
#endif
#if WINUI
                        // Conversion from one stream to another (yawn)
                        InMemoryRandomAccessStream randomaccessStream = new InMemoryRandomAccessStream();
                        using (System.IO.Stream outputStream = randomaccessStream.AsStreamForWrite())
                        {
                            await itemView.StreamBits.CopyToAsync(outputStream);
                            await outputStream.FlushAsync();
                        }
                        randomaccessStream.Seek(0); // Reset position
                        await logo.SetSourceAsync(randomaccessStream);
#endif
#if ANDROIDX
                        Android.Graphics.Bitmap bmap = BitmapFactory.DecodeStream(itemView.StreamBits);
                        logo.SetImageBitmap(bmap);
#endif
#if SMARTMAUI
                        logo = ImageSource.FromStream(() =>
                        {
                            var ms = new MemoryStream();
                            itemView.StreamBits.Position = 0;
                            itemView.StreamBits.CopyTo(ms);
                            ms.Position = 0;
                            return ms;
                        });
#endif
                    }
                }
            }
            return logo;
        }

        internal static char Finance_Find_Account_Type(FinanceViewModel financeviewmodel)
        {
            if (financeviewmodel != null)
            {
                if (financeviewmodel.BanksChecked)
                {
                    return Convert.ToChar(financeviewmodel.ButtonBanks);
                }
                else
                {
                    if (financeviewmodel.InvestmentsChecked)
                    {
                        return Convert.ToChar(financeviewmodel.ButtonInvestments);
                    }
                    else
                    {
                        if (financeviewmodel.SavingsChecked)
                        {
                            return Convert.ToChar(financeviewmodel.ButtonSavings);
                        }
                        else
                        {
                            if (financeviewmodel.CryptosChecked)
                            {
                                return Convert.ToChar(financeviewmodel.ButtonCryptos);
                            }
                        }
                    }
                }
            }
            return SmartParametersV2016.defaultFinanceAccountTypeCode;
        }


        // She is STILL dragging up her poor Dad and slagging him off re his buying habits
        // Even after THIRTY YEARS she still rakes it up.  Unbelievable ...
        // Now the SIlly Cow is trying to meddle in buying PCs ...
        internal static async Task<bool> FinanceButtonSubmitClickActual(
#if WINFORMS
                                                    WebView2 FinanceWebView,
                                                    RichTextBox textBoxConsole,
                                                    MainProcess components,
#endif
#if SMARTMAUI
                                                    WebView FinanceWebView,
#endif
#if WPF || WINUI
                                                    WebView2 FinanceWebView,
#endif
#if ANDROIDX
                                                    WebView FinanceWebView,
                                                    AppCompatActivity meterActivity,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel)
        {

            // Led6 comes in here as GREEN



            // Have we come from a Normal which has set these Buttons thus?
            if (financeviewmodel.SubmitStatus && !financeviewmodel.CancelStatus)
            {
                // Yes, turn OFF the Submit but turn ON the Cancel
                SmartFinanceV2025.TurnOffFinanceStatus(
#if WINFORMS
                                                    components,
#endif
                                                    financeviewmodel);
                // Here now we have Submit = false and Cancel = true;
                if (financeviewmodel.CancelStatus)
                {
                    // Here we are saying "Yes, it can be cancelled"
                    //QuitMessageBox.FixFinanceToken(financeviewmodel, true);
                    if (financeviewmodel.financeCts != null)
                    {
                        financeviewmodel.financeCts = new CancellationTokenSource();                        
                    }


                    if (!await SmartFinanceScrapeV2023.FinanceSubmitScrape(

#if WINFORMS
                                                            FinanceWebView,
                                                            textBoxConsole,
                                                            components,
#endif
#if SMARTMAUI
                                                            FinanceWebView,
#endif
#if WPF  || WINUI
                                                            FinanceWebView,
#endif
#if ANDROIDX
                                                            FinanceWebView,
                                                            meterActivity,
#endif
                                                            ourviewmodel,
                                                            financeviewmodel))
                    {
                        // Turn us to a Normal. This allows Connects
                        FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.orangeColour);

                        await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "Submit button failed");

                        if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Submit button failed"))
                        {
                            return false;
                        }
                        // Now, turn ON the Submit but turn OFF the Cancel
                        SmartFinanceV2025.TurnOnFinanceStatus(
#if WINFORMS
                                                            components,
#endif
                                                            financeviewmodel);

                        return false;
                    }
                    else
                    {
                        try
                        {
                            await Task.WhenAll(financeviewmodel.taskList);
                            //Console.WriteLine("Both finance tasks completed.");
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "All Finance tasks completed");

                            //// Now you can inspect individual results
                            //foreach (var t in financeviewmodel.taskList)
                            //{
                            //    if (t.IsFaulted)
                            //    {
                            //        Console.Error.WriteLine($"Task error: {t.Exception?.InnerException?.Message}");
                            //    }
                            //}


                            // Turn us to a Normal. This allows Connects
                            FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.orangeColour);

                            // Now, turn ON the Submit but turn OFF the Cancel
                            SmartFinanceV2025.TurnOnFinanceStatus(
#if WINFORMS
                                                                components,
#endif
                                                                financeviewmodel);
                            await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "Submit button succeeded");

                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Submit button succeeded"))
                            {
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Console.Error.WriteLine($"An error occurred: {ex.Message}");
                            // Optional: inspect each task for faulted state
                            
                        }
                    }
                }                
            }
            return true;
        }
        internal static async Task<bool> FinanceButtonCancelClickActual(
#if ANDROIDX
                                                                            AppCompatActivity meterActivity,
#endif
                                                                            MainViewModel ourviewmodel,
                                                                            FinanceViewModel financeviewmodel)
        {
            // Led6 comes in here as WHITE

            // Have we gone into a Submit which has set these Buttons thus?
            if (financeviewmodel.CancelStatus && !financeviewmodel.SubmitStatus)
            {
                // Yes
                CancelAllTimers(financeviewmodel);
                financeviewmodel.CancelStatus = false;
                financeviewmodel.SubmitStatus = true;
            }
            else
            {
                if (!FrontEndGUI.CompareLedColour(ourviewmodel, 6, ourviewmodel.whiteColour))
                {
                    financeviewmodel.returned_codes = 0;
                    if (!await Cancel_Finance_Meter_Async(ourviewmodel,
                                                            financeviewmodel))
                    {
                        if (!ourviewmodel.quitCts.IsCancellationRequested)
                        {
                            FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "CancelMeter"))
                            {
                                return false;
                            }
                        }
                        return false;
                    }
                    else
                    {
                        // Set this
                        FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.whiteColour);
                        // Re-enable this
                        financeviewmodel.SubmitStatus = true;

                        // This is a real kludge ...
                        int area_code = financeviewmodel.returned_codes / 256;
                        //int displayed_brand_code;// = financeviewmodel.returned_codes - (area_code * 256);
                        //                        short displayed_supplier_code = 0;
                        //                        Lookup_Finance_Supplier_Code(arm_ycode,

                        //                                                            financeviewmodel.category_code,
                        //                                                            (short)displayed_brand_code,
                        //                                                                ourviewmodel.Fatah.categoriesList,
                        //                                                                financeviewmodel.PLO.institutionsList,
                        //                                                                financeviewmodel.PLO.institution_typesList,
                        //                                                                financeviewmodel.PLO.brandsList,
                        //                                                                rf displayed_supplier_code);

                        //financeviewmodel.activeFlag = (financeviewmodel.category_codex == SmartParametersV2016.defaultChar ? SmartParametersV2016.activeFlag : SmartParametersV2016.activeDefault);
                        financeviewmodel.area_code = (short)area_code;
                        //short brand_code;// = (short)displayed_brand_code;
                        if (!Finance_Build_Institutions_Dropdown(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel,
                                                                financeviewmodel,
                                                                SmartParametersV2016.activeFlag))
                        {
                            if (!ourviewmodel.quitCts.IsCancellationRequested)
                            {
                                FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                            }
                            return false;
                        }
                    }
                }
                if (!FrontEndGUI.CompareLedColour(ourviewmodel, 7, ourviewmodel.redColour))
                {
                    // Reset this
                    FrontEndGUI.SetLedColour(ourviewmodel, 7, ourviewmodel.orangeColour);
                }
            }
            // Last but not least let the popup happen
            financeviewmodel.autoswitch_pending = false;
            return true;
        }

#if WINFORMS || WPF  || WINUI || SMARTMAUI
#if WINFORMS
        internal static bool Finance_Check_Transaction(SmartFinance.TransactionsCategories transaction_row)
#endif
#if WPF || SMARTMAUI
        internal static bool Finance_Check_Transaction(SmartFinance.CommonTransactionsView transaction_row)
#endif
#if WINUI
        internal static bool Finance_Check_Transaction(SmartFinance.CommonTransactionsView transaction_row)
#endif

        {
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            if (!string.IsNullOrEmpty(transaction_row.SORTCODE) &&
                !string.IsNullOrEmpty(transaction_row.ACCOUNT_NO) &&
                (transaction_row.TRANSACTION_DATE != SmartParametersV2016.defaultDate) &&
                !string.IsNullOrEmpty(transaction_row.DESCRIPTION) &&
                (transaction_row.AMOUNT > 0)) // The Bank would never process a transaction of zero! 
                //(transaction_row.BALANCE >= 0)) No! Its perfectly acceptable for the Balance to be zero!!
            {
                return true;
            }
            return false;
        }
#endif
#endif
//#if WINUI
//            if (!string.IsNullOrEmpty(transaction_row.SORTCODE) &&
//                !string.IsNullOrEmpty(transaction_row.ACCOUNT_NO) &&
//                (transaction_row.TRANSACTION_DATE != SmartParametersV2016.defaultDate) &&
//                !string.IsNullOrEmpty(transaction_row.DESCRIPTION) &&
//                (transaction_row.AMOUNT > 0)) // The Bank would never process a transaction of zero! 
//                //(transaction_row.BALANCE >= 0)) No! Its perfectly acceptable for the Balance to be zero!!
//            {
//                return true;
//            }
//            return false;
//        }
//#endif

#if ANDROIDX
        internal static bool Finance_Check_Transaction(SmartFinance.CommonTransactionsView transaction_row)
        {
            if (!string.IsNullOrEmpty(transaction_row.SORTCODE) &&
                !string.IsNullOrEmpty(transaction_row.ACCOUNT_NO) &&
                (transaction_row.TRANSACTION_DATE != SmartParametersV2016.defaultDate) &&
                !string.IsNullOrEmpty(transaction_row.DESCRIPTION) &&
                (transaction_row.AMOUNT > 0)) // The Bank would never process a transaction of zero! 
                                              //(transaction_row.BALANCE >= 0)) No! Its perfectly acceptable for the Balance to be zero!!
            {
                return true;
            }
            return false;
        }
#endif
        internal static string Finance_Transaction_Report(char local_resource_code,
                                            string comparison_institution_name,
                                            string sortcode,
                                            string accountno,
                                            string transaction_date,
                                            string description,
                                            int credit_debit,  // Credit = true, Debit = false
                                            string amount,
                                            //string currency,
                                            string balance)
        {
            string transaction_report = "";
            switch (local_resource_code)
            {
                case SmartParametersV2016.Banks:
                    transaction_report = SmartReportV2016.Fill_Transaction_Report(comparison_institution_name,
                                                                        sortcode,
                                                                        accountno,
                                                                        transaction_date,
                                                                        description,
                                                                        credit_debit,
                                                                        amount,
                                                                        //currency,
                                                                        balance);
                    break;
                case SmartParametersV2016.Savings:
                    break;
                case SmartParametersV2016.Investments:
                    break;
                case SmartParametersV2016.Cryptos:
                    break;
                default:
                    break;
            }
            return transaction_report;
        }

#if WINFORMS
        internal static void Finance_PopupMouseLeave(object sender, System.Windows.Forms.MouseEventArgs e, FinanceViewModel financeviewmodel)
        {
#endif
#if WPF
        internal static void Finance_PopupMouseLeave(object sender, MouseEventArgs e, FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal static void Finance_PopupMouseLeave(object sender, PointerRoutedEventArgs e, FinanceViewModel financeviewmodel)
        {
#endif
#if SMARTMAUI
        internal static void Finance_PopupMouseLeave(object sender, EventArgs e, FinanceViewModel financeviewmodel)
        {
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            if (sender != null && e != null)
            {
#if WPF  || WINUI || SMARTMAUI
                if (financeviewmodel.childWindow != null)
                {
#if WPF
                    if (financeviewmodel.childWindow.IsVisible)
                    {
                        financeviewmodel.childWindow.Close();
                    }
#endif
#if WINUI
                    if (financeviewmodel.childWindow.IsOpen)
                    {
                        financeviewmodel.childWindow.IsOpen = false;
                    }
#endif
#if SMARTMAUI
                    financeviewmodel.childWindow.Close();

#endif
                }
#endif
            }
#endif
#if WINFORMS || WPF  || WINUI || SMARTMAUI
            return;
        }
#endif

#if WINFORMS
        internal static void Finance_PopupMouseLeftButtonDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
#endif
#if WPF
        internal static void Finance_PopupMouseLeftButtonDown(object sender, MouseButtonEventArgs e, FinanceViewModel financeviewmodel)
        {
#endif
#if WINUI
        internal static void Finance_PopupMouseLeftButtonDown(object sender, PointerRoutedEventArgs e, FinanceViewModel financeviewmodel)
        {
#endif
#if ANDROIDX
        internal static void Finance_PopupMouseLeftButtonDown(object sender, EventArgs e)
        {
#endif
#if SMARTMAUI
        internal static void Finance_PopupMouseLeftButtonDown(object sender, EventArgs e, FinanceViewModel financeviewmodel)
        {
#endif
            if (sender != null)
            {
                // This absolute fucking useless bollcks #1
                // THANK YOU Doguhan Uluca - you are a star ...
#if WINFORMS
                //int count = 1;
                //switch (ClickCount)
#endif
#if WPF || SMARTMAUI
#if WPF
                int count = e.ClickCount;
#endif
#if SMARTMAUI
                int count = 0;
#endif
                switch (count)
                {
                    case 0:
                    case 1:
                        // Cancel this handler because this appears to get called as well
                        // when the window is removed ... and we only want to close it ONCE, DON'T WE???
#if WPF
                        financeviewmodel.childWindow.MouseLeave += new MouseEventHandler((s, e1) => Finance_PopupMouseLeave(s, e1, financeviewmodel));
#endif
                        // Remove the popup grid from the screen
                        financeviewmodel.childWindow.Close();
                        break;
                    default:
                        break;
                }
#endif
#if WINUI
                // TapCount unreliable in WINUI so I'm told!
                // Close popup on click
                if (financeviewmodel.childWindow != null)
                {
                    // Attach ONCE (optional)
                    financeviewmodel.childWindow.PointerExited -= (s, e) => Finance_PopupMouseLeave(s, e, financeviewmodel);
                    financeviewmodel.childWindow.IsOpen = false;
                }                
#endif
                }
            return;
        }

        // Its ok to use async void on Event Handlers
#if WINFORMS
        internal static void Finance_TransactionsMouseDoubleClick_Actual(DataGridView datagrid,
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)
#endif
#if WPF
        internal static bool Finance_TransactionsMouseDoubleClick_Actual(ListView viewlist,
                                                                        MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel,
                                                                        FinanceView components)
#endif
#if WINUI
    internal static bool Finance_TransactionsMouseDoubleClick_Actual(ListView viewlist,
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel,
                                                                    FinanceView components)
#endif
#if ANDROIDX
        internal static bool Finance_TransactionsMouseDoubleClick_Actual(Android.Views.View contentpage,
                                                                                    Android.Widget.ListView viewlist,
                                                                                    MainViewModel ourviewmodel,
                                                                                    FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        internal static bool Finance_TransactionsMouseDoubleClick_Actual(ListView viewlist,
                                                                        MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel,
                                                                        FinanceView components)
#endif
        {
            char displayed_category_code = financeviewmodel.category_code;
            //string displayed_resource_type = "";
            // Sometimes? this shit is null ... hence the test

#if WINFORMS
            //DataGridViewRow transactions_row = datagrid.CurrentRow;
            SmartFinance.TransactionsCategories transactions_row = new SmartFinance.TransactionsCategories();
            //SmartFinance.TransactionsView 
            //var transactions_row = (SmartFinance.TransactionsView)rowindex;
#endif
#if WPF || SMARTMAUI
            SmartFinance.CommonTransactionsView transactions_row = (SmartFinance.CommonTransactionsView)viewlist.SelectedItem;
#endif
#if WINUI
            SmartFinance.CommonTransactionsView transactions_row = (SmartFinance.CommonTransactionsView)viewlist.SelectedItem;
#endif
#if ANDROIDX
            // This needs fixing - can't do it at the moment!
            SmartFinance.CommonTransactionsView transactions_row = new();// (SmartFinance.CommonTransactionsView)viewlist.SelectedItem;
            //SmartFinance.TransactionsView transactions_row = new();
            // Was =>    SmartFinance.TransactionsView(SmartFinance.TransactionsView)viewlist.SelectedItem;
            //transactions_row.ShowAtLocation(contentpage, gflags, 0, 0);
#endif
            if (transactions_row == null)
            {
#if WINFORMS
                return;
#endif
#if WPF || SMARTMAUI
                return false;
#endif
#if WINUI
                return false;
#endif
#if ANDROIDX
                return false;
#endif
            }
            else
            {
                if (Finance_Check_Transaction(transactions_row))
                {
#if WPF || SMARTMAUI
                    //financeviewmodel.childWindow = new Window
                    //{
                    //    SizeToContent = SizeToContent.WidthAndHeight,
                    //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    //    Topmost = true
                    //};
#endif
#if WINUI
                    //financeviewmodel.childWindow = new Window
                    //{
                    //    SizeToContent = SizeToContent.WidthAndHeight,
                    //    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    //    Topmost = true
                    //};
#endif
#if WINFORMS
                    // string column_header = datagrid.CurrentCell.ColumnNumber.ToString();

#endif
                    short institution_code = transactions_row.INSTITUTION_CODE;
                    short brand_code = transactions_row.BRAND_CODE;
                    List<SmartFinance.Brands> brand_name = SmartSpikeFinanceV2017.Finance_Lookup_BrandName(ourviewmodel,
                                                                                financeviewmodel,
                                                                                //displayed_category_code, // all displayable categories have Include=true
                                                                                "",
                                                                                institution_code,
                                                                                brand_code);
                    string InstitutionName = "<Unknown>";
                    if (brand_name.Count > 0)
                    {
                        InstitutionName = brand_name.First().BRAND_NAME;
                    }

                    //string accountCurrency = SmartSpikeFinanceV2017.LookupCurrency(ourviewmodel,
                    //                                                                financeviewmodel,
                    //                                                                transactions_row.SORTCODE,
                    //                                                                transactions_row.ACCOUNT_NO);
                    string transaction_report = Finance_Transaction_Report(displayed_category_code,
                                            InstitutionName,
                                            transactions_row.SORTCODE,
                                            transactions_row.ACCOUNT_NO,
                                            transactions_row.TRANSACTION_DATE.ToString(financeviewmodel.CultureINF),
                                            transactions_row.DESCRIPTION,
                                            transactions_row.CREDITDEBIT_INDICATOR,
                                            //accountCurrency,
#if WINFORMS
                                            "",
                                            "0"
#endif
#if !WINFORMS
                                            transactions_row.BALANCE_DISPLAY,
                                            transactions_row.BALANCE.ToString()
#endif
                                            );
                    // What an absolute fucking bitch all this DataGridView stuff is ..
                    // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!

                    // List allows items to be added after ItemsSource
                    // is set and the UI will react to changes

                    string[] lines = transaction_report.Split(SmartParametersV2016.newline);
                    financeviewmodel.Transaction.Clear();
                    foreach (string line in lines)
                    {
                        string trimmed = line.Trim();
                        if (!string.IsNullOrEmpty(trimmed))
                        {
                            string[] bits = trimmed.Split(SmartParametersV2016.colon);
                            FinanceViewModel.TransactionDescription abc = new FinanceViewModel.TransactionDescription()
                            {
                                Field = bits[0].Trim(),
                                Value = SmartParametersV2016.colon + SmartParametersV2016.space + bits[1].Trim()
                            };
                            financeviewmodel.Transaction.Add(abc);
                        }
                        else
                        {
                            // Break on first 'blank' line
                            break;
                        }
                    }
                    financeviewmodel.TransactionDescriptionTitle = "Report - " + SmartReportV2016.Report_Finance_Resource(displayed_category_code);
#if WPF  || WINUI || SMARTMAUI
                    //TransactionDescription transactiondescription_popup = new TransactionDescription();

                    //DockPanel target_dockpanel = new DockPanel();
                    //target_dockpanel.Children.Add(SmartRoutinesV2018.CreateDataGrid(SmartRoutinesV2018.TextColumn("Transaction Description", "description"), transaction_report.ToString().Trim()));
                    //Finance_ApplyWindowMouseButtons(financeviewmodel.childWindow);
                    //financeviewmodel.childWindow.Content = target_dockpanel;
                    //financeviewmodel.childWindow.Show();
                    try
                    {
#if WPF
                        TransactionDescription transactiondescription_popup = new TransactionDescription(financeviewmodel)
                        {

                            // Took me ALL DAY to get this fucker working ...
                            PlacementTarget = components,
                            Placement = PlacementMode.Center,
                            IsOpen = true
#endif
#if WINUI
                        Popup transactiondescription_popup = new Popup();
                        transactiondescription_popup.Child = new TransactionDescription(financeviewmodel);
                        // Centred in the XAML code
                        transactiondescription_popup.IsOpen = true;
                            
                        //TransactionDescription = new TransactionDescription(financeviewmodel);
                        //transactiondescription_popup.IsOpen = true;
                        {
                            // Took me ALL DAY to get this fucker working ...
                            //PlacementTarget = components,
                            //Placement = PlacementMode.Center,
                            //IsOpen = true
#endif
#if SMARTMAUI
                        Popup transactiondescription_popup = new TransactionDescription(financeviewmodel);
                        transactiondescription_popup.Content = new Label()
                        {
                            Text = transaction_report
                        };
                        Application.Current.MainPage.ShowPopupAsync(transactiondescription_popup);

                        //Popup transactiondescription_popup = new Popup();
                        //transactiondescription_popup.Child = new TransactionDescription(financeviewmodel);
                        // Centred in the XAML code
                        //transactiondescription_popup.IsOpen = true;

                        //TransactionDescription = new TransactionDescription(financeviewmodel);
                        //transactiondescription_popup.IsOpen = true;
                        {
                            // Took me ALL DAY to get this fucker working ...
                            //PlacementTarget = components,
                            //Placement = PlacementMode.Center,
                            //IsOpen = true
#endif
                            };

                        // What an absolute fucking bitch all this DataGridView stuff is ..
                        // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                    }
                    catch (Exception ex)
                    {
                        financeviewmodel.errorMessage = ex.Message;
                        return false;
                    }
#endif
#if ANDROIDX
                    TransactionDescription transactiondescription_popup = new(financeviewmodel);
                    GravityFlags gf = new GravityFlags();
                    transactiondescription_popup.ShowAtLocation(contentpage, gf, 0, 0);
#endif
                }
            }
#if WINFORMS
            return;
#endif
#if WPF  || WINUI || SMARTMAUI
            return true;
#endif
#if ANDROIDX
            return true;
#endif
        }


#if WPF
        internal static void Finance_ApplyWindowMouseButtons(UIElement glaze, FinanceViewModel financeviewmodel)
        {
            if (glaze != null)
            {
                glaze.MouseLeftButtonDown += new MouseButtonEventHandler((s, e1) => Finance_PopupMouseLeftButtonDown(s, e1, financeviewmodel));
                glaze.MouseLeave += new MouseEventHandler((s, e2) => Finance_PopupMouseLeave(s, e2, financeviewmodel));
                //glaze.MouseLeftButtonDown += new MouseButtonEventHandler(Finance_PopupMouseLeftButtonDown);
                //glaze.MouseLeave += new System.Windows.Input.MouseEventHandler(Finance_PopupMouseLeave);
            }
            return;
        }
#endif

#if WPF
        internal static void Finance_ApplyGridMouseButtons(UIElement square, FinanceViewModel financeviewmodel)
        {
            if (square != null)
            {
                square.MouseLeftButtonDown += new MouseButtonEventHandler((s, e1) => Finance_PopupMouseLeftButtonDown(s, e1, financeviewmodel));
                //square.MouseLeftButtonDown += new MouseButtonEventHandler(Finance_PopupMouseLeftButtonDown);
            }
            return;
        }
#endif

#if ANDROIDX
        internal static void Finance_ApplyWindowMouseButtons(PopupWindow glaze)
        {
            if (glaze != null)
            {
                //glaze. += new PointerEventHandler(SmartRoutinesV2018.Finance_PopupMouseLeftButtonDown);
                //glaze.PointerExited += new PointerEventHandler(SmartRoutinesV2018.Finance_PopupMouseLeave);
            }
            return;
        }
#endif

        internal static async Task<bool> Finance_RadioButtonChecked_Actual(
#if WINFORMS
                                                                        MainProcess process_components,
#endif
#if ANDROIDX
                                                                        AppCompatActivity meterActivity,
#endif
                                                                        MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel)
        {
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOffFinanceStatus(
#if WINFORMS
                                process_components,
#endif
                                    financeviewmodel);
            bool status = false;
            // Resetting these causes the DataSet dates to be calculated
#if WINFORMS
            financeviewmodel.StartDate = SmartParametersV2016.defaultDate;
            financeviewmodel.EndDate = SmartParametersV2016.defaultMaxdate;
#endif
#if WPF || SMARTMAUI
            financeviewmodel.StartDate = SmartParametersV2016.defaultDate;
            financeviewmodel.EndDate = SmartParametersV2016.defaultMaxdate;
#endif
#if WINUI
            financeviewmodel.StartDate = SmartParametersV2016.defaultDate;
            financeviewmodel.EndDate = SmartParametersV2016.defaultMaxdate;
#endif
#if ANDROIDX
            financeviewmodel.StartDate.DateTime = SmartParametersV2016.defaultDate;
            financeviewmodel.EndDate.DateTime = SmartParametersV2016.defaultMaxdate;
#endif
            if (!await DisplayFinanceMeterAsync(
#if WINFORMS
                                                process_components,
#endif
#if ANDROIDX
                                                    meterActivity,
#endif
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    SmartParametersV2016.activeFlag,
                                                    SmartParametersV2016.defaultDate))
            {
                // We failed because of a Cancellation ... but which one? Check
                if (!ourviewmodel.quitCts.IsCancellationRequested)
                {
                    // If we DIDN'T request a cancellation
                    // We do nothing if there is a failure .. !  SmartSwitch DOESN'T fail!!
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                }
            }
            //else
            //{
            //if (autoswitch_on)
            //{
            //    ourviewmodel.AutoSwitch = ourviewmodel.redColour;
            //}
            //else
            //{
            //    ourviewmodel.AutoSwitch = ourviewmodel.magentaColour;
            //}
            else
            {
                // Despite everything and all the pain
                // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
                if (await Finance_Switchover(
#if ANDROIDX
                                                meterActivity,
#endif
                                                ourviewmodel,
                                                financeviewmodel,
                                                false))              // Reset all


                {
                    status = true;
                }
                else
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 42: Finance Switchover"))
                    {
                        return false;
                    }
                }
                // She is such a pompous sanctimonious bitch ...
                // Conservatory <= Observatory    You couldn't make it up!!!
                // excrement housing <= excreable housing
            }
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOnFinanceStatus(
#if WINFORMS
                                    process_components,
#endif
                                    financeviewmodel);
            return status;
        }


#if WINFORMS
        internal static async Task<bool> Finance_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                int width,
                                                                int height)
#endif
#if WPF || SMARTMAUI
        internal static async Task<bool> Finance_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                SmartCubeMobile.FinanceView components)
#endif
#if WINUI
    internal static async Task<bool> Finance_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                SmartCubeMobile.FinanceView components)
#endif
#if ANDROIDX
        internal static async Task<bool> Finance_TabMouseDoubleClick_Actual(Android.Views.View contentpage,
                                                                    string tabLabel,
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)
#endif
        {
            if (!string.IsNullOrEmpty(tabLabel))
            {
                switch (tabLabel)
                {
                    case "Charts":
                        break;
                    case "Transactions":
                        // What an absolute fucking bitch all this DataGridView stuff is ..
                        // More tea!!
                        // What an absolute bitch this stuff is.  Its a fucking minefield ..
                        financeviewmodel.TransactionsTitle = "Banking" + " " + (DateTime.Now + ourviewmodel.utcOffset).ToString();  // Local time
#if WINFORMS
                        DataGridView transactions = new DataGridView()
                        {
                            DataSource = financeviewmodel.FinanceTransactions,
                            AutoSize = true,
                            ScrollBars = ScrollBars.Vertical,
                            Dock = DockStyle.Fill
                        };
                        Form frm = new Form()
                        {
                            AutoSize = true
                        };
                        frm.Controls.Add(transactions);
                        frm.Height = (height * 2) / 3;
                        frm.Show();
#endif
                        try
                        {
#if WPF
                            FinanceTransactionsPopup transactions_popup = new FinanceTransactionsPopup()
                            {

                                // Took me ALL DAY to get this fucker working ...
                                PlacementTarget = components,
                                Placement = PlacementMode.Center,
                                IsOpen = true


                            };
#endif
#if WINUI
                            Popup transactions_popup = new Popup();
                            transactions_popup.Child = new FinanceTransactionsPopup(ourviewmodel, financeviewmodel);
                            // Centred in the XAML code
                            transactions_popup.IsOpen = true;

                            // Took me ALL DAY to get this fucker working ...
                            //PlacementTarget = components,
                            //Placement = PlacementMode.Center,
                            //IsOpen = true,
                            //CloseButtonText = "Close"

                            //Window.Current.CoreWindow.KeyDown += (sender, args) =>
                            //{
                            //    if (args.VirtualKey == VirtualKey.Escape)
                            //    {
                            //        transactions_popup.Hide();
                            //    }
                            //};
                            //transactions_popup.XamlRoot = transactions_popup.stk.XamlRoot;

                            //await transactions_popup.ShowAsync(); // (components as FrameworkElement);// true ;//  ShowAsync();
#endif
                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 43: Transactions Popup " + ex.Message))
                            {
                                return false;
                            }
                            return false;
                        }
#if ANDROIDX
                        try
                        {
                            financeviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                            financeviewmodel.PopupWidth = ourviewmodel.currentWidth - ourviewmodel.currentWidth / 10;
                            FinanceTransactionsPopup financetransactions_popup = new FinanceTransactionsPopup();
                            GravityFlags gflags = new GravityFlags();
                            financetransactions_popup.ShowAtLocation(contentpage, gflags, 0, 0);
                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            return false;
                        }
#endif
#if SMARTMAUI
                        Popup transactions_popup = new Transactions()
                        {
                            // Took me ALL DAY to get this fucker working ...
                            //PlacementTarget = components,
                            //Placement = PlacementMode.Center,
                            //IsOpen = true
                        };
#endif
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

#if WINFORMS
        internal static async Task<bool> FinanceCrypto_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                int width,
                                                                int height)
#endif
#if WPF || SMARTMAUI
        internal static async Task<bool> FinanceCrypto_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                SmartCubeMobile.FinanceView components)
#endif
#if WINUI
        internal static async Task<bool> FinanceCrypto_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                SmartCubeMobile.FinanceView components)
#endif
#if ANDROIDX
        internal static async Task <bool> FinanceCrypto_TabMouseDoubleClick_Actual(Android.Views.View contentpage,
                                                                    string tabLabel,
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)
#endif
        {
            if (!string.IsNullOrEmpty(tabLabel))
            {
                switch (tabLabel)
                {
                    case "Transactions":
                        // What an absolute fucking bitch all this DataGridView stuff is ..
                        // More tea!!
                        // What an absolute bitch this stuff is.  Its a fucking minefield ..
                        financeviewmodel.TransactionsTitle = "Crypto" + " " + (DateTime.Now + ourviewmodel.utcOffset).ToString();  // Local time
#if WINFORMS
                        DataGridView transactions = new DataGridView()
                        {
                            DataSource = financeviewmodel.FinanceTransactions,
                            AutoSize = true,
                            ScrollBars = ScrollBars.Vertical,
                            Dock = DockStyle.Fill
                        };
                        Form frm = new Form()
                        {
                            AutoSize = true
                        };
                        frm.Controls.Add(transactions);
                        frm.Height = (height * 2) / 3;
                        frm.Show();
#endif
                        try
                        {
#if WPF
                            FinanceCryptoTransactionsPopup transactions_popup = new FinanceCryptoTransactionsPopup(ourviewmodel, financeviewmodel)
                            {

                                // Took me ALL DAY to get this fucker working ...
                                PlacementTarget = components,
                                Placement = PlacementMode.Center,
                                IsOpen = true


                            };
#endif
#if WINUI
                            Popup transactions_popup = new Popup();
                            transactions_popup.Child = new FinanceTransactionsPopup(ourviewmodel, financeviewmodel);
                            // Centred in the XAML code
                            transactions_popup.IsOpen = true;
                            //Window.Current.CoreWindow.KeyDown += (sender, args) =>
                            //{
                            //    if (args.VirtualKey == VirtualKey.Escape)
                            //    {
                            //        transactions_popup.Hide();
                            //    }
                            //};
                            //transactions_popup.XamlRoot = transactions_popup.stk.XamlRoot;

                            //await transactions_popup.ShowAsync(); // (components as FrameworkElement);// true ;//  ShowAsync();
#endif
                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 43: Transactions Popup " + ex.Message))
                            {
                                return false;
                            }
                            return false;
                        }
#if ANDROIDX
                        try
                        {
                            financeviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                            financeviewmodel.PopupWidth = ourviewmodel.currentWidth - ourviewmodel.currentWidth / 10;
                            FinanceTransactionsPopup financetransactions_popup = new FinanceTransactionsPopup();
                            GravityFlags gflags = new GravityFlags();
                            financetransactions_popup.ShowAtLocation(contentpage, gflags, 0, 0);
                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            return false;
                        }
#endif
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

#if WINFORMS
        internal static async Task<bool> FinanceCryptoTabItems_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                int width,
                                                                int height)
#endif
#if WPF || SMARTMAUI
        internal static async Task<bool> FinanceCryptoTabItems_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                SmartCubeMobile.FinanceView components)
#endif
#if WINUI
    internal static async Task<bool> FinanceCryptoTabItems_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                SmartCubeMobile.FinanceView components)
#endif
#if ANDROIDX
        internal static async Task<bool> FinanceCryptoTabItems_TabMouseDoubleClick_Actual(Android.Views.View contentpage,
                                                                    string tabLabel,
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)
#endif
        {
            if (!string.IsNullOrEmpty(tabLabel))
            {
                switch (tabLabel)
                {
                    case "Transactions":
                        // What an absolute fucking bitch all this DataGridView stuff is ..
                        // More tea!!
                        // What an absolute bitch this stuff is.  Its a fucking minefield ..
                        financeviewmodel.TransactionsTitle = "Crypto" + " " + (DateTime.Now + ourviewmodel.utcOffset).ToString();  // Local time
#if WINFORMS
                        DataGridView transactions = new DataGridView()
                        {
                            DataSource = financeviewmodel.FinanceTransactions,
                            AutoSize = true,
                            ScrollBars = ScrollBars.Vertical,
                            Dock = DockStyle.Fill
                        };
                        Form frm = new Form()
                        {
                            AutoSize = true
                        };
                        frm.Controls.Add(transactions);
                        frm.Height = (height * 2) / 3;
                        frm.Show();
#endif
                        try
                        {
#if WPF
                            FinanceCryptoTransactionsPopup cryptotransactions_popup = new FinanceCryptoTransactionsPopup(ourviewmodel, financeviewmodel)
                            {

                                // Took me ALL DAY to get this fucker working ...
                                PlacementTarget = components,
                                Placement = PlacementMode.Center,
                                IsOpen = true
                            };  
#endif
#if WINUI
                            Popup cryptotransactions_popup = new Popup();
                            cryptotransactions_popup.Child = new FinanceCryptoTransactionsPopup(ourviewmodel, financeviewmodel);
                            // Centred in the XAML code
                            cryptotransactions_popup.IsOpen = true;

                            //Window.Current.CoreWindow.KeyDown += (sender, args) =>
                            //{
                            //    if (args.VirtualKey == VirtualKey.Escape)
                            //    {
                            //        addresses_popup.Hide();
                            //    }
                            //};
                            //transactions_popup.XamlRoot = transactions_popup.stk.XamlRoot;

                            //await transactions_popup.ShowAsync(); // (components as FrameworkElement);// true ;//  ShowAsync();

                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
#endif
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 43: Transactions Popup " + ex.Message))
                            {
                                return false;
                            }
                            return false;
                        }
#if ANDROIDX
                        try
                        {
                            financeviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                            financeviewmodel.PopupWidth = ourviewmodel.currentWidth - ourviewmodel.currentWidth / 10;
                            FinanceCryptoTransactionsPopup financetransactions_popup = new FinanceCryptoTransactionsPopup(ourviewmodel, financeviewmodel);
                            GravityFlags gflags = new GravityFlags();
                            financetransactions_popup.ShowAtLocation(contentpage, gflags, 0, 0);
                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            return false;
                        }
#endif
                        break;
                    case "Accounts":
                        // What an absolute fucking bitch all this DataGridView stuff is ..
                        // More tea!!
                        // What an absolute bitch this stuff is.  Its a fucking minefield ..
                        financeviewmodel.TransactionsTitle = "Crypto" + " " + (DateTime.Now + ourviewmodel.utcOffset).ToString();  // Local time
#if WINFORMS
                        DataGridView accounts = new DataGridView()
                        {
                            DataSource = financeviewmodel.FinanceCryptoAccounts,
                            AutoSize = true,
                            ScrollBars = ScrollBars.Vertical,
                            Dock = DockStyle.Fill
                        };
                        Form form = new Form()
                        {
                            AutoSize = true
                        };
                        form.Controls.Add(accounts);
                        form.Height = (height * 2) / 3;
                        form.Show();
#endif
                        try
                        {
#if WPF
                            FinanceCryptoAccountsPopup accounts_popup = new FinanceCryptoAccountsPopup(ourviewmodel, financeviewmodel)
                            {

                                // Took me ALL DAY to get this fucker working ...
                                PlacementTarget = components,
                                Placement = PlacementMode.Center,
                                IsOpen = true
                             };
#endif
#if WINUI
                            // Took me ALL DAY to get this fucker working ...
                            //PlacementTarget = components,
                            //Placement = PlacementMode.Center,
                            //IsOpen = true,
                            //CloseButtonText = "Close"

                            Popup cryptoaccounts_popup = new Popup();
                            cryptoaccounts_popup.Child = new FinanceCryptoAccountsPopup(ourviewmodel, financeviewmodel);
                            // Centred in the XAML code
                            cryptoaccounts_popup.IsOpen = true;

                            //Window.Current.CoreWindow.KeyDown += (sender, args) =>
                            //{
                            //    if (args.VirtualKey == VirtualKey.Escape)
                            //    {
                            //        accounts_popup.Hide();
                            //    }
                            //};
                            //accounts_popup.XamlRoot = accounts_popup.stk.XamlRoot;

                            //await accounts_popup.ShowAsync(); // (components as FrameworkElement);// true ;//  ShowAsync();
#endif
                        }

                        // What an absolute fucking bitch all this DataGridView stuff is ..
                        // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!
                        // 
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 43: Accounts Popup " + ex.Message))
                            {
                                return false;
                            }
                            return false;
                        }
#if ANDROIDX
                        try
                        {
                            financeviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                            financeviewmodel.PopupWidth = ourviewmodel.currentWidth - ourviewmodel.currentWidth / 10;
                            FinanceCryptoAccountsPopup financeaccounts_popup = new FinanceCryptoAccountsPopup(ourviewmodel, financeviewmodel);
                            GravityFlags gflags = new GravityFlags();
                            financeaccounts_popup.ShowAtLocation(contentpage, gflags, 0, 0);
                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            return false;
                        }
#endif
                        break;

                    case "Addresses":
                        // What an absolute fucking bitch all this DataGridView stuff is ..
                        // More tea!!
                        // What an absolute bitch this stuff is.  Its a fucking minefield ..
                        financeviewmodel.TransactionsTitle = "Crypto" + " " + (DateTime.Now + ourviewmodel.utcOffset).ToString();  // Local time
#if WINFORMS
                        DataGridView addresses = new DataGridView()
                        {
                            DataSource = financeviewmodel.FinanceCryptoAddresses,
                            AutoSize = true,
                            ScrollBars = ScrollBars.Vertical,
                            Dock = DockStyle.Fill
                        };
                        Form form1 = new Form()
                        {
                            AutoSize = true
                        };
                        form1.Controls.Add(addresses);
                        form1.Height = (height * 2) / 3;
                        form1.Show();
#endif
#if WPF  || WINUI || SMARTMAUI
                        try
                        {
#if WPF
                            FinanceCryptoAddressesPopup addresses_popup = new FinanceCryptoAddressesPopup()
                            {

                                // Took me ALL DAY to get this fucker working ...
                                PlacementTarget = components,
                                Placement = PlacementMode.Center,
                                IsOpen = true
                            };
#endif
#if WINUI
                            // Took me ALL DAY to get this fucker working ...
                            //PlacementTarget = components,
                            //Placement = PlacementMode.Center,
                            //IsOpen = true,
                            //CloseButtonText = "Close"
                            
                            Popup cryptoaddresses_popup = new Popup();
                            cryptoaddresses_popup.Child = new FinanceCryptoAddressesPopup(ourviewmodel, financeviewmodel);
                            // Centred in the XAML code
                            cryptoaddresses_popup.IsOpen = true;

                            //Window.Current.CoreWindow.KeyDown += (sender, args) =>
                            //{
                            //  if (args.VirtualKey == VirtualKey.Escape)
                            //{
                            //        addresses_popup.Hide();
                            //    }
                            //};
                            //addresses_popup.XamlRoot = addresses_popup.stk.XamlRoot;

                            //await addresses_popup.ShowAsync(); // (components as FrameworkElement);// true ;//  ShowAsync();

                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
#endif
                        }

                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 43: Addresses Popup " + ex.Message))
                            {
                                return false;
                            }

                            return false;
                        }
#endif
#if ANDROIDX
                        try
                        {
                            financeviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                            financeviewmodel.PopupWidth = ourviewmodel.currentWidth - ourviewmodel.currentWidth / 10;
                            FinanceCryptoAddressesPopup financeaddresses_popup = new FinanceCryptoAddressesPopup();
                            GravityFlags gflags = new GravityFlags();
                            financeaddresses_popup.ShowAtLocation(contentpage, gflags, 0, 0);
                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            return false;
                        }
#endif
                        break;

                    case "Exchange Rates":
                        // What an absolute fucking bitch all this DataGridView stuff is ..
                        // More tea!!
                        // What an absolute bitch this stuff is.  Its a fucking minefield ..
                        financeviewmodel.TransactionsTitle = "Crypto" + " " + (DateTime.Now + ourviewmodel.utcOffset).ToString();  // Local time
#if WINFORMS
                        DataGridView exchangeRates = new DataGridView()
                        {
                            DataSource = financeviewmodel.FinanceCryptoCurrencyRates,
                            AutoSize = true,
                            ScrollBars = ScrollBars.Vertical,
                            Dock = DockStyle.Fill
                        };
                        Form form2 = new Form()
                        {
                            AutoSize = true
                        };
                        form2.Controls.Add(exchangeRates);
                        form2.Height = (height * 2) / 3;
                        form2.Show();
#endif
#if WPF  || WINUI || SMARTMAUI
                        try
                        {
#if WPF
                            FinanceCryptoExchangeRatesPopup exchangerates_popup = new FinanceCryptoExchangeRatesPopup(ourviewmodel, financeviewmodel)
                            {
                                // Took me ALL DAY to get this fucker working ...
                                PlacementTarget = components,
                                Placement = PlacementMode.Center,
                                IsOpen = true
                            };
#endif

#if WINUI
                            // Took me ALL DAY to get this fucker working ...
                            //PlacementTarget = components,
                            //    Placement = PlacementMode.Center,
                            //    IsOpen = true,
                            //    CloseButtonText = "Close"

                            Popup exchangerates_popup = new Popup();
                            exchangerates_popup.Child = new FinanceExchangeRatesPopup(ourviewmodel, financeviewmodel);
                            // Centred in the XAML code
                            exchangerates_popup.IsOpen = true;

                            //Window.Current.CoreWindow.KeyDown += (sender, args) =>
                            //{
                            //if (args.VirtualKey == VirtualKey.Escape)
                            //{
                            //    addresses_popup.Hide();
                            //}
                            //};
                            //exchangerates_popup.XamlRoot = exchangerates_popup.stk.XamlRoot;

                            //await exchangerates_popup.ShowAsync(); // (components as FrameworkElement);// true ;//  ShowAsync();

                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
#endif
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 43: ExchangeRates Popup " + ex.Message))
                            {
                                return false;
                            }
                            return false;
                        }
#endif
#if ANDROIDX
                        try
                        {
                            financeviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                            financeviewmodel.PopupWidth = ourviewmodel.currentWidth - ourviewmodel.currentWidth / 10;
                            FinanceCryptoExchangeRatesPopup financeexchangerates_popup = new FinanceCryptoExchangeRatesPopup(ourviewmodel, financeviewmodel);
                            GravityFlags gflags = new GravityFlags();
                            financeexchangerates_popup.ShowAtLocation(contentpage, gflags, 0, 0);
                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            return false;
                        }
#endif
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

#if WINFORMS
        internal static bool FinanceCryptoAddresses_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                int width,
                                                                int height)
#endif
#if WPF || SMARTMAUI
        internal static async Task<bool> FinanceCryptoAddresses_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                SmartCubeMobile.FinanceView components)
#endif
#if WINUI
    internal static async Task<bool> FinanceCryptoAddresses_TabMouseDoubleClick_Actual(string tabLabel,
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel,
                                                                SmartCubeMobile.FinanceView components)
#endif
#if ANDROIDX
        internal static bool FinanceCryptoAddresses_TabMouseDoubleClick_Actual(Android.Views.View contentpage,
                                                                    string tabLabel,
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)
#endif
        {
            if (!string.IsNullOrEmpty(tabLabel))
            {
                switch (tabLabel)
                {
                    case "Addresses":
                        // What an absolute fucking bitch all this DataGridView stuff is ..
                        // More tea!!
                        // What an absolute bitch this stuff is.  Its a fucking minefield ..
                        financeviewmodel.TransactionsTitle = "Crypto" + " " + (DateTime.Now + ourviewmodel.utcOffset).ToString();  // Local time
#if WINFORMS
                        DataGridView transactions = new DataGridView()
                        {
                            DataSource = financeviewmodel.FinanceTransactions,
                            AutoSize = true,
                            ScrollBars = ScrollBars.Vertical,
                            Dock = DockStyle.Fill
                        };
                        Form frm = new Form()
                        {
                            AutoSize = true
                        };
                        frm.Controls.Add(transactions);
                        frm.Height = (height * 2) / 3;
                        frm.Show();
#endif
#if WPF  || WINUI || SMARTMAUI
                        try
                        {
#if WPF
                            FinanceCryptoAddressesPopup transactions_popup = new FinanceCryptoAddressesPopup()
                            {

                                // Took me ALL DAY to get this fucker working ...
                                PlacementTarget = components,
                                Placement = PlacementMode.Center,
                                IsOpen = true

                            };
#endif
#if WINUI
                            Popup cryptoaddresses_popup = new Popup();
                            cryptoaddresses_popup.Child = new FinanceCryptoAddressesPopup(ourviewmodel, financeviewmodel);
                            // Centred in the XAML code
                            cryptoaddresses_popup.IsOpen = true;
                            //Window.Current.CoreWindow.KeyDown += (sender, args) =>
                            //{
                            //    if (args.VirtualKey == VirtualKey.Escape)
                            //    {
                            //        transactions_popup.Hide();
                            //    }
                            //};
                            //transactions_popup.XamlRoot = transactions_popup.stk.XamlRoot;

                            //await transactions_popup.ShowAsync(); // (components as FrameworkElement);// true ;//  ShowAsync();

                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            

#endif
                        }

                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 43: Transactions Popup " + ex.Message))
                            {
                                return false;
                            }
                            return false;
                        }
#endif
#if ANDROIDX
                        try
                        {
                            financeviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                            financeviewmodel.PopupWidth = ourviewmodel.currentWidth - ourviewmodel.currentWidth / 10;
                            FinanceTransactionsPopup financetransactions_popup = new FinanceTransactionsPopup();
                            GravityFlags gflags = new GravityFlags();
                            financetransactions_popup.ShowAtLocation(contentpage, gflags, 0, 0);
                            // What an absolute fucking bitch all this DataGridView stuff is ..
                            // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                        }
                        catch (Exception ex)
                        {
                            financeviewmodel.errorMessage = ex.Message;
                            return false;
                        }
#endif
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

#if ANDROIDX
        internal static void PopupClosed(object sender, EventArgs e)
        {
            return;
        }
#endif

#if WINFORMS
        internal static void MouseEnterInstitutionsFinance_Actual(MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel)

#endif
#if WPF || SMARTMAUI
        internal static void MouseEnterInstitutionsFinance_Actual(MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel)

#endif
#if WINUI
        internal static void MouseEnterInstitutionsFinance_Actual(MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel)

#endif
#if ANDROIDX
        internal static void MouseEnterInstitutionsFinance_Actual(MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel)

#endif
        {
#if WINFORMS
            financeviewmodel.InstitutionsBorderBrush = ourviewmodel.cyanColour;
            //financeviewmodel.InstitutionsBorderThickness = new Thickness(1);
#endif
#if WPF || SMARTMAUI
            financeviewmodel.InstitutionsBorderBrush = ourviewmodel.cyanColour;
            financeviewmodel.InstitutionsBorderThickness = new Thickness(1);
#endif
#if WINUI
            financeviewmodel.InstitutionsBorderBrush = ourviewmodel.cyanColour;
            financeviewmodel.InstitutionsBorderThickness = new Thickness(1);
#endif
#if ANDROIDX
            financeviewmodel.InstitutionsBorderBrush = ourviewmodel.cyanColour;
            //financeviewmodel.InstitutionsBorderThickness = new Thickness(1);
#endif
            return;
        }

#if WINFORMS
        internal static void MouseLeaveInstitutionsFinance_Actual(MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel)

#endif
#if WPF || SMARTMAUI
        internal static void MouseLeaveInstitutionsFinance_Actual(MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)

#endif
#if WINUI
        internal static void MouseLeaveInstitutionsFinance_Actual(MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)

#endif
#if ANDROIDX
        internal static void MouseLeaveInstitutionsFinance_Actual(MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)

#endif
        {
#if WINFORMS
            financeviewmodel.InstitutionsBorderBrush = ourviewmodel.transparentColour;
            //financeviewmodel.InstitutionsBorderThickness = new Thickness(0);
#endif
#if WPF || SMARTMAUI
            financeviewmodel.InstitutionsBorderBrush = ourviewmodel.transparentColour;
            financeviewmodel.InstitutionsBorderThickness = new Thickness(0);
#endif
#if WINUI
            financeviewmodel.InstitutionsBorderBrush = ourviewmodel.transparentColour;
            financeviewmodel.InstitutionsBorderThickness = new Thickness(0);
#endif
#if ANDROIDX
            financeviewmodel.InstitutionsBorderBrush = ourviewmodel.transparentColour;
            //financeviewmodel.InstitutionsBorderThickness = new Thickness(0);
#endif
            return;
        }

        internal static async Task<bool> Finance_Switchover(
#if ANDROIDX
                                                            AppCompatActivity meterActivity,
#endif
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            bool reset_all)
        {
            // Only do switches between Banks, Savings and Investments            
            if (reset_all)
            {
                foreach (SmartFinance.Categories category_row in financeviewmodel.PLO.finance_categoriesList)
                {
                    category_row.CHECKED = "";
                    category_row.Updated = true;
                    financeviewmodel.PLO.finance_categories_changesList.Add(category_row);
                }
            }
            else
            {
                foreach (SmartFinance.Categories category_row in financeviewmodel.PLO.finance_categoriesList)
                {
                    if (category_row.Updated)
                    {
                        financeviewmodel.PLO.finance_categories_changesList.Add(category_row);
                    }
                }
            }

            if (financeviewmodel.PLO.finance_categories_changesList.Count > 0)
            {
                // Record the switch ...and ..
                // Update the internal DB
                if (!await SmartFinanceScrapeV2023.DoAllSmartFinance(ourviewmodel,
                                                financeviewmodel,
                                                false,
                                                SmartParametersV2016.sqliteformat))
                {
#if WPF || SMARTMAUI
                    financeviewmodel.errorMessage = "Cannot update categories(1) - cannot continue";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                    // This is a bit fatal, because if we can't do this, there's
                    // every chance we can't do lots of things we need to further on
                    return false;
#endif
#if WINUI
                    financeviewmodel.errorMessage = "Cannot update categories(1) - cannot continue";
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                    // This is a bit fatal, because if we can't do this, there's
                    // every chance we can't do lots of things we need to further on
                    return false;
#endif
                }
                else
                {
                    // Tell the console we have switched
                    await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                            meterActivity,
#endif
                                            ourviewmodel, "Finance Type switched to: " + financeviewmodel.categoryCodes);
                }
            }
            return true;
        }

#if WINFORMS
        internal static void FinanceLoginParams(FinanceViewModel financeviewmodel,
                                    System.Windows.Forms.TextBox ConnectionId,
                                    System.Windows.Forms.TextBox CustomerNumber,
                                    System.Windows.Forms.TextBox DateOfBirth,
                                    System.Windows.Forms.TextBox Passcode,
                                    SmartFinance.Logins oldlogin_info,
                                    List<SmartFinance.BrandConnection> params_found)
#endif
#if WPF
        internal static void FinanceLoginParams(FinanceViewModel financeviewmodel,
                                    TextBox ConnectionId,
                                    TextBox CustomerNumber,
                                    TextBox DateOfBirth,
                                    TextBox Passcode,
                                    SmartFinance.Logins oldlogin_info,
                                    List<SmartFinance.BrandConnection> params_found)
#endif
#if WINUI
        internal static void FinanceLoginParams(FinanceViewModel financeviewmodel,
                                    TextBox ConnectionId,
                                    TextBox CustomerNumber,
                                    TextBox DateOfBirth,
                                    TextBox Passcode,
                                    SmartFinance.Logins oldlogin_info,
                                    List<SmartFinance.BrandConnection> params_found)
#endif
#if ANDROIDX
        internal static void FinanceLoginParams(FinanceViewModel financeviewmodel,
                                EditText ConnectionId,
                                EditText CustomerNumber,
                                EditText DateOfBirth,
                                EditText Passcode,
                                SmartFinance.Logins oldlogin_info,
                                List<SmartFinance.BrandConnection> params_found)
#endif
#if SMARTMAUI
        internal static void FinanceLoginParams(FinanceViewModel financeviewmodel,
                                    Entry ConnectionId,
                                    Entry CustomerNumber,
                                    Entry DateOfBirth,
                                    Entry Passcode,
                                    SmartFinance.Logins oldlogin_info,
                                    List<SmartFinance.BrandConnection> params_found)
#endif
        {
            SmartFinance.Connections temp = new SmartFinance.Connections();
            // Do the ConnectionId
            ConnectionId.Text = oldlogin_info.LOGIN_METHOD.ToString();
            if (string.IsNullOrEmpty(ConnectionId.Text))
            {
                ConnectionId.Text = "Connection Id";
            }
            // Do the Params
            CustomerNumber.Text = temp.Parameter1;
            if (string.IsNullOrEmpty(CustomerNumber.Text) && params_found.Count >= 1)
            {
                CustomerNumber.Text = params_found[0].PARAMETER_PROMPT;
            }

            DateOfBirth.Text = temp.Parameter2;
            if (string.IsNullOrEmpty(DateOfBirth.Text) && params_found.Count >= 2)
            {
                DateOfBirth.Text = params_found[1].PARAMETER_PROMPT;
            }
            Passcode.Text = temp.Parameter3;
            if (string.IsNullOrEmpty(Passcode.Text) && params_found.Count >= 3)
            {
                Passcode.Text = params_found[2].PARAMETER_PROMPT;
                financeviewmodel.passcodeMaxlength = params_found[2].PARAMETER_LENGTH;
            }
            else
            {
                financeviewmodel.passcodeMaxlength = 6;
            }
            return;
        }

#if WINFORMS
        internal static bool Finance_Institution_Selected_Actual(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                string institution_text)
#endif
#if WPF
        internal static bool Finance_Institution_Selected_Actual(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                FinanceView financeview,
                                                string institution_text)
#endif
#if WINUI
        internal static bool Finance_Institution_Selected_Actual(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                string institution_text)
#endif
#if ANDROIDX
        internal static bool Finance_Institution_Selected_Actual(//Android.Views.View contentpage,
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                string institution_text)
#endif
#if SMARTMAUI
        internal static async Task<bool> Finance_Institution_Selected_Actual(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                FinanceView financeview,
                                                string institution_text)
#endif
        {
#if WPF
            financeviewmodel.childWindow = new Window
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Topmost = true
            };
#endif
#if SMARTMAUI
            financeviewmodel.childWindow = new Popup
            {
                //SizeToContent = SizeToContent.WidthAndHeight,
                //WindowStartupLocation = WindowStartupLocation.CenterScreen,
                //Topmost = true
            };
#endif
#if WINUI
            financeviewmodel.childWindow = SmartRoutinesV2018.PopupTitle("Report - " + institution_text);            
#endif
#if WPF
            DockPanel target_dockpanel = new DockPanel();
#endif
#if WINUI
            StackPanel target_dockpanel = new StackPanel();
#endif
#if SMARTMAUI
            VerticalStackLayout target_dockpanel = new VerticalStackLayout();
#endif
            // Was this fucking painful, or what?  Couldn't get the fucking
            // text to clean up its lines before sending ....
#if WPF || SMARTMAUI
            string title = "Report - " + institution_text;
#if WPF
            financeviewmodel.childWindow.Title = title;
#endif
#endif
            // God - was THIS fucking painful or what???  ALl this just to get the
            // title fully shown at the top of the box. Fucking , fucking Microshit
#if WPF  || WINUI
            FontFamily FontFamily = new FontFamily("Arial");
#endif
#if ANDROIDX
            Typeface font = Typeface.Create("Arial", TypefaceStyle.Normal);
#endif
#if SMARTMAUI
            //PortableDocumentFontFamily FontFamily = new PortableDocumentFontFamily();
#endif
#if WPF
            FontWeight FontWeight = System.Windows.FontWeights.Bold;
            FontStyle FontStyle = new FontStyle();
            int FontSize = 12;
            Typeface Typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretches.Normal);

            FormattedText ft = new FormattedText(financeviewmodel.childWindow.Title,
                                            CultureInfo.GetCultureInfo("en-GB"),
                                            FlowDirection.LeftToRight,
                                            Typeface,
                                            FontSize,
                                            Brushes.Black,
                                            VisualTreeHelper.GetDpi(financeview).PixelsPerDip);
            financeviewmodel.childWindow.Width = ft.Width;
#endif
#if SMARTMAUI
            var label = new Label
            {
                Text = title,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold
            };
            Microsoft.Maui.SizeRequest size = label.Measure(double.PositiveInfinity, double.PositiveInfinity);
            financeviewmodel.childWindow.Content = new Grid
            {
                WidthRequest = size.Request.Width,   // 👈 this controls popup width
                Children = { label }
            };
#endif
            string institution_report = SmartReportV2016.Fill_Institution_Report(financeviewmodel,
                                                                                institution_text);
#if WPF  || WINUI || SMARTMAUI
            //Label abc = SmartRoutinesV2018.TextColumn("Description", "description");
#if WPF 
            target_dockpanel.Children.Add(SmartRoutinesV2018.CreateDataGrid(SmartRoutinesV2018.TextColumn("Description", "description"), financeviewmodel.childWindow.Title.ToString().Trim()));
#endif
#if SMARTMAUI
            target_dockpanel.Children.Add(SmartRoutinesV2018.CreateDataGrid("Description", institution_report.Trim()));
#endif
#endif
#if WPF
            Finance_ApplyWindowMouseButtons(financeviewmodel.childWindow, financeviewmodel);
#endif
#if WPF  || SMARTMAUI
            financeviewmodel.childWindow.Content = target_dockpanel;
#endif
#if WPF
            financeviewmodel.childWindow.Show();
#endif

#if WINUI
            //Finance_ApplyWindowMouseButtons(financeviewmodel.childWindow);

            financeviewmodel.childWindow.Child = target_dockpanel;
            financeviewmodel.childWindow.IsOpen = true;
#endif
#if ANDROIDX
            InstitutionDetails institution_popup = new InstitutionDetails();
            GravityFlags gflags = new();
            //institution_popup.ShowAtLocation(contentpage, gflags, 0, 0);
#endif
#if SMARTMAUI
            await Shell.Current.ShowPopupAsync(financeviewmodel.childWindow);
#endif
            return true;
        }

        // HERE
        internal static SmartFinance.Logins CreateEmptyLogin(FinanceViewModel financeviewmodel,
                                                string Username,
                                                short institution_code,
                                                short brand_code,
                                                string maxCategories)
        {
            // Can change Parameter1
            SmartFinance.Logins newLogin = new SmartFinance.Logins()
            {
                USERNAME = Username,
                CUBEFACE_CODE = SmartParametersV2016.Finance,
                INSTITUTION_CODE = institution_code,
                BRAND_CODE = brand_code,
                ACTIVE_FLAG = SmartParametersV2016.activeFlag
            };
            // Note: no RANDOMKEY yet!!
            financeviewmodel.ConnectionIdReadOnly = false;
            return newLogin;
        }
        //#if ANDROIDX
        //        internal static View BuildLoginDetails(MainViewModel ourviewmodel,
        //                                                FinanceViewModel financeviewmodel,
        //                                                View financeLoginsView,
        //                                                short institution_code,
        //                                                short brand_code,
        //                                                char category_code,
        //                                                string brandName,
        //                                                string contactEmail,
        //                                                string[] loginText)
        //        {

        //            TextView providerText = financeLoginsView.FindViewById<TextView>(Resource.Id.ProviderText);
        //            string text = brandName;
        //            providerText.SetText(text.ToCharArray(), 0, text.Length);

        //            TextView connectionInfo = financeLoginsView.FindViewById<TextView>(Resource.Id.ConnectInfo);
        //            text = "required entry of your " +
        //                    contactEmail +
        //                    "connection details";
        //            connectionInfo.SetText(text.ToCharArray(), 0, text.Length);

        //            List<SmartFinance.BrandConnection> bc_found = SmartSpikeFinanceV2017.Finance_Find_BrandConnections(ourviewmodel,
        //                                                            financeviewmodel,
        //                                                            institution_code,
        //                                                            brand_code,
        //                                                            category_code);

        //            if (bc_found.Count > 0)
        //            {
        //                // Find the Layout for the EditTexts
        //                // Find the LinearLayout by its ID
        //                LinearLayout rootLayout = financeLoginsView.FindViewById<LinearLayout>(Resource.Id.linearLayout);
        //                // Optional: Set layout parameters
        //                //LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(
        //                //    LinearLayout.LayoutParams.WrapContent,
        //                //    LinearLayout.LayoutParams.WrapContent);
        //                //layoutParams.SetMargins(0, 16, 0, 16); // Add some margin

        //                //LinearLayout detail = new LinearLayout(financeviewmodel.context);
        //                //detail.Orientation = Orientation.Horizontal;

        //                // Create a new EditText(s) programmatically
        //                foreach (SmartFinance.BrandConnection bc in bc_found)
        //                {
        //                    int index = bc.ORDINAL - 1;
        //                    EditText editText = new EditText(financeviewmodel.context)
        //                    {
        //                        Gravity = GravityFlags.Center,
        //                        InputType = Android.Text.InputTypes.Null,
        //                        Focusable = true,
        //                        TextSize = 14,
        //                        TextAlignment = TextAlignment.Center,
        //                        Hint = bc.PARAMETER_HINT,

        //                    };
        //                    editText.LayoutParameters = new LinearLayout.LayoutParams(125, 35);// LinearLayout.LayoutParams.WrapContent);
        //                    if (loginText[index] != "")
        //                    {
        //                        editText.SetText(loginText[index].ToCharArray(), 0, text.Length);
        //                    }
        //                    // Add the EditText to the details LinearLayout
        //                    rootLayout.AddView(editText);
        //                }                
        //            }

        //            return financeLoginsView;
        //        }

        //        private static void SubmitButton_Click(object sender, EventArgs e)
        //        {
        //            throw new NotImplementedException();
        //        }
        //#endif
        internal static async Task<bool> Finance_InstitutionsPopup_Actual(
#if ANDROIDX
                                                    //Android.Views.View contentPage,
                                                    AppCompatActivity activity,
#endif
#if WINFORMS
                                                    MainProcess process_components,
#endif
#if WPF || SMARTMAUI
                                                    FinanceView components,
#endif
#if WINUI
                                                    FinanceView components,
#endif
                                                    SignInViewModel signinviewmodel,MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    FinanceViewModel.InstitutionItem institution_item)
        {
            bool status = true;
            if (institution_item != null)
            {
                string institution_name = institution_item.Content;
                short institutionCode = 0;
                short brand_code = 0;
                //char twoFactor = SmartParametersV2016.defaultChar;
                //bool twoFactor = false; Its in Providers now
#if WINFORMS
                institution_name = institution_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif
                Decode_Tag_Institution(institution_item.Value.ToString(),
                                        ref institutionCode);
                // We are going to get info? Better turn off some buttons first!
                //
                // These can all be turned off now, except the Cancel button
                TurnOffFinanceStatus(
#if WINFORMS
                                    process_components,
#endif
                                    financeviewmodel);
                // We are GOOD TO GO!!! 11:00am on 3rd July 2017!!! 
                // and again 24th Feb 2025 at 20:21 !!!
                // and AGAIN on 29th Jan 2026!!!!!
                // This is where we store the data
                // Open the SmartCube file for RAY
                // Create it if it doesn't exist
                try
                {
                    // Given the InstitutionCode, lookup its name in Institutions
                    //List<SmartFinance.InstitutionInfo> institutionsFound =
                    string InstitutionName = SmartSpikeFinanceV2017.Finance_Lookup_InstitutionName(ourviewmodel,
                                                                                              financeviewmodel,
                                                                                              //SmartParametersV2016.activeFlag,
                                                                                              institutionCode);
                    if (InstitutionName != "")
                    {
                        financeviewmodel.FinanceInstitutionsText = InstitutionName;
                    }
                    else
                    {
                        financeviewmodel.FinanceInstitutionsText = institution_item.Content;                        
                    }

#if WPF || SMARTMAUI
                    financeviewmodel.DefaultInstitutionCode = institutionCode;
                    //financeviewmodel.FCLWindow = new MainWindow();
                    // Come on, Ray!!!!
                    financeviewmodel.FCLWindow = new FinanceConnectionsLogins(signinviewmodel,
                                                                            ourviewmodel,
                                                                            financeviewmodel,
                                                                            institutionCode,
                                                                            institution_name);
#if WPF
                    financeviewmodel.FCLWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    //financeviewmodel.FCLWindow.VerticalOffset = financeviewmodel.VerticalOffset;
#endif
#endif
#if WINUI
                    financeviewmodel.DefaultInstitutionCode = institutionCode;
                    //financeviewmodel.FCLWindow = new MainWindow();
                    // Come on, Ray!!!!
                    financeviewmodel.FCLWindow = new Popup
                    {
                        HorizontalOffset = 100,
                        VerticalOffset = 100
                    };
                    FinanceConnectionsLogins popupContent = new FinanceConnectionsLogins(signinviewmodel,
                                                                    ourviewmodel,
                                                                    financeviewmodel,
                                                                    institutionCode,
                                                                    institution_name);
                    popupContent.DataContext = financeviewmodel;
                    popupContent.PointerExited += (s, e) =>
                    {
                        financeviewmodel.FCLWindow.IsOpen = false;
                    };
#endif
#if WINFORMS
                    //financeviewmodel.FCLPopup.Show();
#endif
#if WPF
                    financeviewmodel.FCLWindow.ShowDialog();
#endif
#if WINUI
                    financeviewmodel.FCLWindow.XamlRoot = financeviewmodel.xamlRoot;
                    financeviewmodel.FCLWindow.Child = popupContent;
                    financeviewmodel.FCLWindow.IsOpen = true;
#endif
#if ANDROIDX
                    financeviewmodel.CreatedBrands = new List<BrandItem>();
                    foreach (SmartFinance.Brands brand in financeviewmodel.PLO.brandsList)
                    {
                        if (brand.INSTITUTION_CODE == institutionCode)
                        {
                            financeviewmodel.CreatedBrands.Add(new BrandItem
                            {
                                InstitutionCode = brand.INSTITUTION_CODE,
                                BRAND_CODE = brand.BRAND_CODE,
                                BrandName = brand.BRAND_NAME
                            });
                        }
                    }
                    LoadConnections(financeviewmodel, institutionCode, InstitutionName);
                    LoadLogins(financeviewmodel, institutionCode, InstitutionName);

                    var dialog = new FinanceConnectionsFragment
                    {
                        financeviewmodel = financeviewmodel,
                        ourviewmodel = ourviewmodel,
                        //signinviewmodel = signinviewmodel,
                        InstitutionCode = institutionCode,
                        InstitutionName = InstitutionName

                    }; 
                    dialog.Show(activity.SupportFragmentManager, "FinancePopup");
#endif
#if SMARTMAUI
                    await Shell.Current.Navigation.PushModalAsync(financeviewmodel.FCLWindow);
#endif
                }
                catch (Exception ex)
                {
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, institutionCode, brand_code, SmartParametersV2016.Finance.ToString() + " Login details failed from " + institution_name + " " + ex.Message))
                    {
                        return false;
                    }
                    status = false;

                }
                // These can all be turned on now, except the Cancel button
                TurnOnFinanceStatus(
#if WINFORMS
                                        process_components,
#endif
                                        financeviewmodel);
            }
            else
            {
                status = false;
            }
            return status;
        }
        internal static void Finance_InstitutionsChanged_Actual(
#if ANDROIDX
                                                    //Android.Views.View contentPage,
                                                    Spinner mySpinner,
#endif
#if WINFORMS
                                                    MainProcess process_components,
#endif
#if WPF || SMARTMAUI
                                                    FinanceView components,
#endif
#if WINUI 
                                                    FinanceView components,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    FinanceViewModel.InstitutionItem institution_item)
        {
            // Here is where we either:
            // Check all accounts if the provider is checked
            // Uncheck all accounts if the provider is unchecked

            short institution_code = 0;
            //short temp_brand_code = 0;
            string sortcode = "";
            string account_no = "";
            short ordinal = 0;

            short pid = 0;
            // You have to do this bollcks this way, otherwise FinanceProvidersList
            // DOESN'T get updated ... what a pile of shit this garbage is ...
            //List<FinanceViewModel.InstitutionItem> temp_institutions = financeviewmodel.FinanceInstitutionsList;
            List<FinanceViewModel.ProviderItem> temp_providers = financeviewmodel.FinanceProvidersList;
            List<FinanceViewModel.AccountItem> temp_accounts = financeviewmodel.FinanceAccountsList;

            Decode_Tag_Institution(institution_item.Value.ToString(),
                                        ref institution_code);

            // Now find all Providers for this Institution code
            List<SmartFinance.Brands> brands_found = SmartSpikeFinanceV2017.Finance_Find_Brands(ourviewmodel,
                                                       financeviewmodel,
                                                       institution_code);
            foreach (SmartFinance.Brands brand in brands_found)
            {
                pid++;
                string itemValue = Build_Tag_Provider(brand.INSTITUTION_CODE,
                                                    brand.BRAND_CODE,
                                                    brand.TWOFACTOR_FLAG);
                FinanceViewModel.ProviderItem provider_item = new FinanceViewModel.ProviderItem()
                {
                    Colour = ourviewmodel.greenColour,
                    Content = brand.BRAND_NAME,
                    ProviderID = pid,
                    IsChecked = true,
                    Value = itemValue
                };                
                temp_providers.Add(provider_item);
            }

    
#if WINFORMS
            for (int provider_index = 0; provider_index < process_components.comboBoxGUIFinanceProviders.Items.Count; provider_index++)
#endif
#if WPF || SMARTMAUI
            for (int provider_index = 0; provider_index < temp_providers.Count; provider_index++)
#endif
#if WINUI
            for (int provider_index = 0; provider_index < temp_providers.Count; provider_index++)
#endif
#if ANDROIDX
            for (int provider_index = 0; provider_index < temp_providers.Count; provider_index++)
#endif
            {
#if WINFORMS
                FinanceViewModel.ProviderItem provider_item = process_components.comboBoxGUIFinanceProviders.Items[provider_index] as FinanceViewModel.ProviderItem;
#endif
#if WPF || SMARTMAUI
                    FinanceViewModel.ProviderItem provider_item = temp_providers[provider_index] as FinanceViewModel.ProviderItem;
#endif
#if WINUI
                FinanceViewModel.ProviderItem provider_item = temp_providers[provider_index] as FinanceViewModel.ProviderItem;
#endif
#if ANDROIDX
                FinanceViewModel.ProviderItem provider_item = temp_providers[provider_index] as FinanceViewModel.ProviderItem;
#endif
                short pinstitution_code = 0;
                short pbrand_code = 0;
                char twoFactor = SmartParametersV2016.defaultChar;
                Decode_Tag_Provider(provider_item.Value.ToString(),
                                            ref pinstitution_code,
                                            ref pbrand_code,
                                            ref twoFactor);
                    if (pinstitution_code == institution_code)
                    {
#if WINFORMS
                        if (process_components.comboBoxGUIFinanceProviders.GetItemChecked(provider_index))
#endif
                        if (temp_providers[provider_index].IsChecked)
                        {
                            // Make sure all the accounts are checked THAT MATCH you CLUNKHEAD
#if WINFORMS
                            for (int account_index = 0; account_index < process_components.comboBoxGUIFinanceAccounts.Items.Count; account_index++)
#endif
#if WPF || SMARTMAUI
                            for (int account_index = 0; account_index < temp_accounts.Count; account_index++)
#endif
#if WINUI
                            for (int account_index = 0; account_index < temp_accounts.Count; account_index++)
#endif
#if ANDROIDX
                            for (int account_index = 0; account_index < temp_accounts.Count; account_index++)
#endif
                            {
                                short a1institution_code = 0;
                                short a1brand_code = 0;
                                financeviewmodel.udprn = "";
#if WINFORMS
                                FinanceViewModel.AccountItem account_item = process_components.comboBoxGUIFinanceAccounts.Items[account_index] as FinanceViewModel.AccountItem;
#endif
#if WPF || SMARTMAUI
                                FinanceViewModel.AccountItem account_item = temp_accounts[account_index] as FinanceViewModel.AccountItem;
#endif
#if WINUI
                                FinanceViewModel.AccountItem account_item = temp_accounts[account_index] as FinanceViewModel.AccountItem;
#endif
#if ANDROIDX
                                FinanceViewModel.AccountItem account_item = temp_accounts[account_index] as FinanceViewModel.AccountItem;
#endif
                                Decode_AccountTag_Extra(account_item.Value.ToString(),
                                                ref a1institution_code,
                                                ref a1brand_code,
                                                ref sortcode,
                                                ref account_no,
                                                ref financeviewmodel.udprn,
                                                ref ordinal);
                                if (a1institution_code == pinstitution_code &&
                                    a1brand_code == pbrand_code)
                                {
#if WINFORMS
                                    process_components.comboBoxGUIFinanceAccounts.SetItemChecked(account_index, true);
#endif
#if WPF || SMARTMAUI
                                    temp_accounts[account_index].IsChecked = true;
#endif
#if WINUI
                                    temp_accounts[account_index].IsChecked = true;
#endif
#if ANDROIDX
                                    temp_accounts[account_index].IsChecked = true;
#endif
                                }
                            }
                        }
                        else
                        {
                            // Make sure all the accounts are unchecked
                            // Make sure all the accounts are checked THAT MATCH you CLUNKHEAD
#if WINFORMS
                            for (int account_index = 0; account_index < process_components.comboBoxGUIFinanceAccounts.Items.Count; account_index++)
#endif
#if WPF || SMARTMAUI
                            for (int account_index = 0; account_index < temp_accounts.Count; account_index++)
#endif
#if WINUI
                            for (int account_index = 0; account_index < temp_accounts.Count; account_index++)
#endif
#if ANDROIDX
                            for (int account_index = 0; account_index < temp_accounts.Count; account_index++)
#endif
                            {
                                short a2institution_code = 0;
                                short a2brand_code = 0;
                                financeviewmodel.udprn = "";
#if WINFORMS
                                FinanceViewModel.AccountItem account_item = process_components.comboBoxGUIFinanceAccounts.Items[account_index] as FinanceViewModel.AccountItem;
#endif
#if WPF || SMARTMAUI
                                FinanceViewModel.AccountItem account_item = temp_accounts[account_index] as FinanceViewModel.AccountItem;
#endif
#if WINUI
                                FinanceViewModel.AccountItem account_item = temp_accounts[account_index] as FinanceViewModel.AccountItem;
#endif
#if ANDROIDX
                                FinanceViewModel.AccountItem account_item = temp_accounts[account_index] as FinanceViewModel.AccountItem;
#endif
                                Decode_AccountTag_Extra(account_item.Value.ToString(),
                                                    ref a2institution_code,
                                                    ref a2brand_code,
                                                    ref sortcode,
                                                    ref account_no,
                                                    ref financeviewmodel.udprn,
                                                    ref ordinal);
                                if (a2institution_code == pinstitution_code &&
                                    a2brand_code == pbrand_code)
                                {
#if WINFORMS
                                    process_components.comboBoxGUIFinanceAccounts.SetItemChecked(account_index, false);
#endif
#if WPF || SMARTMAUI
                                    temp_accounts[account_index].IsChecked = false;
#endif
#if WINUI
                                    temp_accounts[account_index].IsChecked = false;
#endif
#if ANDROIDX
                                    temp_accounts[account_index].IsChecked = false;
#endif
                                }
                            }
                        }
                    }
                }
            
            // You have to do this bollcks this way, otherwise FinanceProvidersList
            // DOESN'T get updated ... what a pile of shit this garbage is ...
            //financeviewmodel.FinanceInstitutionsList = new List<FinanceViewModel.InstitutionItem>();
            //financeviewmodel.FinanceInstitutionsList = temp_institutions;

            financeviewmodel.FinanceProvidersList = new List<FinanceViewModel.ProviderItem>();
            financeviewmodel.FinanceProvidersList = temp_providers;

            financeviewmodel.FinanceAccountsList = new List<FinanceViewModel.AccountItem>();
            financeviewmodel.FinanceAccountsList = temp_accounts;

#if WPF || SMARTMAUI
            FinanceProviders_UpdateText(financeviewmodel);
            //FinanceAccounts_UpdateText(financeviewmodel);
#endif
#if WINUI
            FinanceProviders_UpdateText(financeviewmodel);
            //FinanceAccounts_UpdateText(financeviewmodel);
#endif
            Finance_SubsetTransactions(
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
                                            financeviewmodel.StartDate,
                                            financeviewmodel.EndDate);
#endif
#if WPF || SMARTMAUI
                                            financeviewmodel.StartDate,
                                            financeviewmodel.EndDate);
#endif
#if WINUI
                                            financeviewmodel.StartDate,
                                            financeviewmodel.EndDate);
#endif
#if ANDROIDX
                                            financeviewmodel.StartDate.DateTime,
                                            financeviewmodel.EndDate.DateTime);
#endif
            return;
        }

        internal static async Task<bool> Finance_ProvidersPopup_Actual(
#if ANDROIDX
                                                    //Android.Views.View contentPage,
                                                    AppCompatActivity activity,
#endif
#if WINFORMS
                                                    MainProcess process_components,
#endif
#if WPF || SMARTMAUI
                                                    FinanceView components,
#endif
#if WINUI 
                                                    FinanceView components,
#endif
                                                    SignInViewModel signinviewmodel, MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    FinanceViewModel.ProviderItem provider_item)
        {
            bool status = true;
            try
            {
                if (provider_item != null)
                {
                    string provider_name = provider_item.Content;
                    //short institutionCode = 0;
                    //short brand_code = 0;
                    //char twoFactor = SmartParametersV2016.defaultChar;
                    //bool twoFactor = false; Its in Providers now
#if WINFORMS
                    provider_name = provider_name.TrimStart(SmartParametersV2016.defaultgreen).TrimEnd(SmartParametersV2016.defaultred);
#endif
                }
            }
            catch (Exception ex)
            {
                if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, SmartParametersV2016.Finance.ToString() + " Providers popup failed " + ex.Message))
                {
                    return false;
                }
                status = false;
            }
            return status;
        }

        // This is never called in an Android program. Not true called in MultiSpinner
        // All Provider changes are done in the MultiSpinner routine
        internal static void Finance_ProvidersChanged_Actual(
#if WINFORMS
                                                            MainProcess process_components,
#endif
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel)
        {
            // Here is where we either:
            // Check all accounts if the provider is checked
            // Uncheck all accounts if the provider is unchecked

            short temp_institution_code = 0;
            short temp_brand_code = 0;
            string sortcode = "";
            string account_no = "";
            //short transactiontype = 0;
            //bool paid_in = false;
            //string udprn = "";
            short ordinal = 0;


            // You have to do this bollcks this way, otherwise FinanceProvidersList
            // DOESN'T get updated ... what a pile of shit this garbage is ...
            List<FinanceViewModel.ProviderItem> temp_providers = financeviewmodel.FinanceProvidersList;
            List<FinanceViewModel.AccountItem> temp_accounts = financeviewmodel.FinanceAccountsList;
#if WINFORMS
            for (int provider_index = 0; provider_index < process_components.comboBoxGUIFinanceProviders.Items.Count; provider_index++)
#endif
#if WPF || SMARTMAUI
            for (int provider_index = 0; provider_index < temp_providers.Count; provider_index++)
#endif
#if WINUI
            for (int provider_index = 0; provider_index < temp_providers.Count; provider_index++)
#endif
#if ANDROIDX
            for (int provider_index = 0; provider_index < temp_providers.Count; provider_index++)
#endif
                {
#if WINFORMS
                FinanceViewModel.ProviderItem provider_item = process_components.comboBoxGUIFinanceProviders.Items[provider_index] as FinanceViewModel.ProviderItem;
#endif
#if WPF || SMARTMAUI
                FinanceViewModel.ProviderItem provider_item = temp_providers[provider_index] as FinanceViewModel.ProviderItem;
#endif
#if WINUI
                FinanceViewModel.ProviderItem provider_item = temp_providers[provider_index] as FinanceViewModel.ProviderItem;
#endif
#if ANDROIDX
                FinanceViewModel.ProviderItem provider_item = temp_providers[provider_index] as FinanceViewModel.ProviderItem;
#endif
                short institution_code = 0;
                short brand_code = 0;
                char twoFactor = SmartParametersV2016.defaultChar;
                Decode_Tag_Provider(provider_item.Value.ToString(),
                                        ref institution_code,
                                        ref brand_code,
                                        ref twoFactor);

#if WINFORMS
                if (process_components.comboBoxGUIFinanceProviders.GetItemChecked(provider_index))
#endif
                if (temp_providers[provider_index].IsChecked)
                {
                    // Make sure all the accounts are checked THAT MATCH you CLUNKHEAD
#if WINFORMS
                    for (int account_index = 0; account_index < process_components.comboBoxGUIFinanceAccounts.Items.Count; account_index++)
#endif
#if WPF || SMARTMAUI
                    for (int account_index = 0; account_index < temp_accounts.Count; account_index++)
#endif
#if WINUI
                    for (int account_index = 0; account_index < temp_accounts.Count; account_index++)
#endif
#if ANDROIDX
                    for (int account_index = 0; account_index < temp_accounts.Count; account_index++)
#endif
                        {
                            temp_institution_code = 0;
                        temp_brand_code = 0;
                        //sortcode = "";
                        //account_no = "";
                        financeviewmodel.udprn = "";
                        //ordinal = 0;
#if WINFORMS
                        FinanceViewModel.AccountItem account_item = process_components.comboBoxGUIFinanceAccounts.Items[account_index] as FinanceViewModel.AccountItem;
#endif
#if WPF || SMARTMAUI
                        FinanceViewModel.AccountItem account_item = temp_accounts[account_index] as FinanceViewModel.AccountItem;
#endif
#if WINUI
                        FinanceViewModel.AccountItem account_item = temp_accounts[account_index] as FinanceViewModel.AccountItem;
#endif
#if ANDROIDX
                        FinanceViewModel.AccountItem account_item = temp_accounts[account_index] as FinanceViewModel.AccountItem;
#endif
                        Decode_AccountTag_Extra(account_item.Value.ToString(),
                                        ref temp_institution_code,
                                        ref temp_brand_code,
                                        ref sortcode,
                                        ref account_no,
                                        ref financeviewmodel.udprn,
                                        ref ordinal);

                        if (institution_code == temp_institution_code &&
                            brand_code == temp_brand_code)
                        {
#if WINFORMS
                                process_components.comboBoxGUIFinanceAccounts.SetItemChecked(account_index, true);
#endif
#if WPF || SMARTMAUI
                            temp_accounts[account_index].IsChecked = true;
#endif
#if WINUI
                            temp_accounts[account_index].IsChecked = true;
#endif
#if ANDROIDX
                            temp_accounts[account_index].IsChecked = true;
#endif
                        }
                    }
                }
                else
                {
                    // Make sure all the accounts are unchecked
                    // Make sure all the accounts are checked THAT MATCH you CLUNKHEAD
#if WINFORMS
                    for (int account_index = 0; account_index < process_components.comboBoxGUIFinanceAccounts.Items.Count; account_index++)
#endif
#if WPF || SMARTMAUI
                    for (int account_index = 0; account_index < temp_accounts.Count; account_index++)
#endif
#if WINUI
                    for (int account_index = 0; account_index < temp_accounts.Count; account_index++)
#endif
#if ANDROIDX
                    for (int account_index = 0; account_index < temp_accounts.Count; account_index++)
#endif
                    {
                            temp_institution_code = 0;
                        temp_brand_code = 0;
                        //sortcode = "";
                        //account_no = "";
                        financeviewmodel.udprn = "";
                        //ordinal = 0;

#if WINFORMS
                        FinanceViewModel.AccountItem account_item = process_components.comboBoxGUIFinanceAccounts.Items[account_index] as FinanceViewModel.AccountItem;
#endif
#if WPF || SMARTMAUI
                        FinanceViewModel.AccountItem account_item = temp_accounts[account_index] as FinanceViewModel.AccountItem;
#endif
#if WINUI
                        FinanceViewModel.AccountItem account_item = temp_accounts[account_index] as FinanceViewModel.AccountItem;
#endif
#if ANDROIDX
                        FinanceViewModel.AccountItem account_item = temp_accounts[account_index] as FinanceViewModel.AccountItem;
#endif
                        Decode_AccountTag_Extra(account_item.Value.ToString(),
                                            ref temp_institution_code,
                                            ref temp_brand_code,
                                            ref sortcode,
                                            ref account_no,
                                            ref financeviewmodel.udprn,
                                            ref ordinal);

                        if (institution_code == temp_institution_code &&
                            brand_code == temp_brand_code)
                        {
#if WINFORMS
                            process_components.comboBoxGUIFinanceAccounts.SetItemChecked(account_index, false);
#endif
#if WPF || SMARTMAUI
                            temp_accounts[account_index].IsChecked = false;
#endif
#if WINUI
                            temp_accounts[account_index].IsChecked = false;
#endif
#if ANDROIDX
                            temp_accounts[account_index].IsChecked = false;
#endif
                        }
                    }
                }
            }

            // You have to do this bollcks this way, otherwise FinanceProvidersList
            // DOESN'T get updated ... what a pile of shit this garbage is ...
            financeviewmodel.FinanceProvidersList = new List<FinanceViewModel.ProviderItem>();
            financeviewmodel.FinanceProvidersList = temp_providers;

            financeviewmodel.FinanceAccountsList = new List<FinanceViewModel.AccountItem>();
            financeviewmodel.FinanceAccountsList = temp_accounts;

#if WPF || SMARTMAUI
            FinanceProviders_UpdateText(financeviewmodel);
            //FinanceAccounts_UpdateText(financeviewmodel);
#endif
#if WINUI
            FinanceProviders_UpdateText(financeviewmodel);
            //FinanceAccounts_UpdateText(financeviewmodel);
#endif
            Finance_SubsetTransactions(
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
                                            financeviewmodel.StartDate,
                                            financeviewmodel.EndDate);
#endif
#if WPF || SMARTMAUI
                                            financeviewmodel.StartDate,
                                            financeviewmodel.EndDate);
#endif
#if WINUI
                                            financeviewmodel.StartDate,
                                            financeviewmodel.EndDate);
#endif
#if ANDROIDX
                                            financeviewmodel.StartDate.DateTime,
                                            financeviewmodel.EndDate.DateTime);
#endif

            return;
        }



#if WPF  || WINUI || SMARTMAUI
        //internal static void FinanceInstitutions_UpdateText(FinanceViewModel financeviewmodel)
        //{
        //    financeviewmodel.FinanceInstitutions_Text = "";

        //    foreach (FinanceViewModel.InstitutionItem institution_item in financeviewmodel.FinanceProvidersList)
        //    {
        //        if (provider_item.IsChecked)
        //        {
        //            if (financeviewmodel.FinanceProviders_Text != "")
        //            {
        //                financeviewmodel.FinanceProviders_Text += ",";
        //            }
        //            financeviewmodel.FinanceProviders_Text += provider_item.Content;
        //        }
        //    }
        //    return;
        //}
        internal static void FinanceProviders_UpdateText(FinanceViewModel financeviewmodel)
        {
            financeviewmodel.FinanceProviders_Text = "";

            foreach (FinanceViewModel.ProviderItem provider_item in financeviewmodel.FinanceProvidersList)
            {
                if (provider_item.IsChecked)
                {
                    if (financeviewmodel.FinanceProviders_Text != "")
                    {
                        financeviewmodel.FinanceProviders_Text += ",";
                    }
                    financeviewmodel.FinanceProviders_Text += provider_item.Content;
                }
            }
            return;
        }
        internal static void FinanceAccounts_UpdateText(FinanceViewModel financeviewmodel)
        {
            financeviewmodel.FinanceAccounts_Text = "";

            foreach (FinanceViewModel.AccountItem account_item in financeviewmodel.FinanceAccountsList)
            {
                if (account_item.IsChecked)
                {
                    if (financeviewmodel.FinanceAccounts_Text != "")
                    {
                        financeviewmodel.FinanceAccounts_Text += ",";
                    }
                    financeviewmodel.FinanceAccounts_Text += account_item.Content;
                }
            }
            return;
        }
#endif

        // This routine is never called in an Android program Yes it is
        // in MultiSpinner
        // All changes to the accounts is done in a MultiSpinner routine
        internal static void Finance_AccountsChanged_Actual(
#if WINFORMS
                                                            MainProcess process_components,
#endif
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel)
        {
            // Here is where we either:
            // Check a provider if ONE of the provider's accounts is checked
            // Uncheck a provider is ALL of the provider's accounts are unchecked


            // You have to do this bollcks this way, otherwise FinanceProvidersList
            // DOESN'T get updated ... what a pile of shit this garbage is ...
            List<FinanceViewModel.ProviderItem> temp_providers = financeviewmodel.FinanceProvidersList;
            financeviewmodel.selectedAccountsList.Clear();

            short temp_institution_code = 0;
            short temp_brand_code = 0;
            string sortcode = "";
            string account_no = "";
            string udprn = "";
            short ordinal = 0;

#if WINFORMS
            for (int provider_index = 0; provider_index < process_components.comboBoxGUIFinanceProviders.Items.Count; provider_index++)
#endif
#if WPF || SMARTMAUI
            for (int provider_index = 0; provider_index < temp_providers.Count; provider_index++)
#endif
#if WINUI
            for (int provider_index = 0; provider_index < temp_providers.Count; provider_index++)
#endif
#if ANDROIDX
            for (int provider_index = 0; provider_index < temp_providers.Count; provider_index++)
#endif
            {
#if WINFORMS
                FinanceViewModel.ProviderItem provider_item = process_components.comboBoxGUIFinanceProviders.Items[provider_index] as FinanceViewModel.ProviderItem;
#endif
#if WPF || SMARTMAUI
                FinanceViewModel.ProviderItem provider_item = temp_providers[provider_index] as FinanceViewModel.ProviderItem;
#endif
#if WINUI
                FinanceViewModel.ProviderItem provider_item = temp_providers[provider_index] as FinanceViewModel.ProviderItem;
#endif
#if ANDROIDX
                FinanceViewModel.ProviderItem provider_item = temp_providers[provider_index] as FinanceViewModel.ProviderItem;
#endif
                short institution_code = 0;
                short brand_code = 0;
                char twoFactor = SmartParametersV2016.defaultChar;

                Decode_Tag_Provider(provider_item.Value.ToString(),
                                        ref institution_code,
                                        ref brand_code,
                                        ref twoFactor);

                // Clear down the Provider IN CASE all accounts are unchecked
#if WINFORMS
                process_components.comboBoxGUIFinanceProviders.SetItemChecked(provider_index, false);
#endif
                temp_providers[provider_index].IsChecked = false;
                // Go through the accounts looking for just ONE that is checked
#if WINFORMS
                for (int account_index = 0; account_index < process_components.comboBoxGUIFinanceAccounts.Items.Count; account_index++)
#endif
#if WPF || SMARTMAUI
                for (int account_index = 0; account_index < financeviewmodel.FinanceAccountsList.Count; account_index++)
#endif
#if WINUI
                for (int account_index = 0; account_index < financeviewmodel.FinanceAccountsList.Count; account_index++)
#endif
#if ANDROIDX
                for (int account_index = 0; account_index < financeviewmodel.FinanceAccountsList.Count; account_index++)
#endif
                {
                    temp_institution_code = 0;
                    temp_brand_code = 0;
                    sortcode = "";
                    account_no = "";
                    udprn = "";
                    ordinal = 0;
#if WINFORMS
                    FinanceViewModel.AccountItem account_item = process_components.comboBoxGUIFinanceAccounts.Items[account_index] as FinanceViewModel.AccountItem;
#endif
#if WPF || SMARTMAUI
                    FinanceViewModel.AccountItem account_item = financeviewmodel.FinanceAccountsList[account_index] as FinanceViewModel.AccountItem;
#endif
#if WINUI
                    FinanceViewModel.AccountItem account_item = financeviewmodel.FinanceAccountsList[account_index] as FinanceViewModel.AccountItem;
#endif
#if ANDROIDX
                    FinanceViewModel.AccountItem account_item = financeviewmodel.FinanceAccountsList[account_index] as FinanceViewModel.AccountItem;
#endif
                    Decode_AccountTag_Extra(account_item.Value.ToString(),
                                        ref temp_institution_code,
                                        ref temp_brand_code,
                                        ref sortcode,
                                        ref account_no,
                                        ref udprn,
                                        ref ordinal);

                    if (institution_code == temp_institution_code &&
                        brand_code == temp_brand_code)
                    {
                        // It matches but is it checked??
#if WINFORMS
                        if (process_components.comboBoxGUIFinanceAccounts.GetItemChecked(account_index))
#endif
#if WPF || SMARTMAUI
                        //if (financeviewmodel.FinanceAccountsList[account_index].IsChecked)
                        if (account_item.IsChecked)
#endif
#if WINUI
                        //if (financeviewmodel.FinanceAccountsList[account_index].IsChecked)
                        if (account_item.IsChecked)
#endif
#if ANDROIDX
                        //if (financeviewmodel.FinanceAccountsList[account_index].IsChecked)
                        if (account_item.IsChecked)
#endif
                            {
                                // Yes, set the Provider and escape
#if WINFORMS
                            process_components.comboBoxGUIFinanceProviders.SetItemChecked(provider_index, true);
#endif
                            temp_providers[provider_index].IsChecked = true;
                            SmartFinance.SelectedAccounts subset = new SmartFinance.SelectedAccounts()
                            {
                                INSTITUTION_CODE = temp_institution_code,
                                BRAND_CODE = temp_brand_code,
                                SORTCODE = sortcode,
                                ACCOUNT_NO = account_no,
                                UDPRN = udprn,
                                CURRENCY_ORDINAL = ordinal
                            };
                            financeviewmodel.selectedAccountsList.Add(subset);
                        }
                    }
                }
            }

            // You have to do this bollcks this way, otherwise FinanceProvidersList
            // DOESN'T get updated ... what a pile of shit this garbage is ...
            financeviewmodel.FinanceProvidersList = new List<FinanceViewModel.ProviderItem>();
            financeviewmodel.FinanceProvidersList = temp_providers;

#if WPF || SMARTMAUI
            FinanceProviders_UpdateText(financeviewmodel);
#endif
#if WINUI
            FinanceProviders_UpdateText(financeviewmodel);
#endif
            Finance_SubsetTransactions(
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
                                            financeviewmodel.StartDate,
                                            financeviewmodel.EndDate);
#endif
#if WPF || SMARTMAUI
                                            financeviewmodel.StartDate,
                                            financeviewmodel.EndDate);
#endif
#if WINUI
                                            financeviewmodel.StartDate,
                                            financeviewmodel.EndDate);
#endif
#if ANDROIDX
                                            financeviewmodel.StartDate.DateTime,
                                            financeviewmodel.EndDate.DateTime);
#endif
            return;
        }

#if WPF  || WINUI || SMARTMAUI
        //internal static void FinanceAccounts_UpdateText(FinanceViewModel financeviewmodel)
        //{
        //    financeviewmodel.FinanceAccounts_Text = "";

        //    foreach (SmartFinance.SelectedAccounts account_item in financeviewmodel.selectedAccountsList)
        //    {
        //        // All the accounts in selectedAccountsList have been put in
        //        // there becuase they ARE checked
        //        //if (account_item.IsChecked)
        //        //{
        //            if (financeviewmodel.FinanceAccounts_Text != "")
        //            {
        //                financeviewmodel.FinanceAccounts_Text += ", ";
        //            }
        //            financeviewmodel.FinanceAccounts_Text += account_item.;
        //        //}
        //    }
        //    return;
        //}
#endif
        internal static void Finance_TransactionGroupsChanged_Actual(
#if WINFORMS
                                                                MainProcess process_components,
#endif
                                                                MainViewModel ourviewmodel,
                                                                FinanceViewModel financeviewmodel)
        {
#if WPF || SMARTMAUI
            FinanceTransactionGroups_UpdateText(financeviewmodel);
#endif
#if WINUI
            FinanceTransactionGroups_UpdateText(financeviewmodel);
#endif
            Finance_SubsetTransactions(
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
                                                        financeviewmodel.StartDate,
                                                        financeviewmodel.EndDate);
#endif
#if WPF || SMARTMAUI
                                                        financeviewmodel.StartDate,
                                                        financeviewmodel.EndDate);
#endif
#if WINUI
                                                        financeviewmodel.StartDate,
                                                        financeviewmodel.EndDate);
#endif
#if ANDROIDX
                                                        financeviewmodel.StartDate.DateTime,
                                                        financeviewmodel.EndDate.DateTime);
#endif
            return;
        }

#if WPF  || WINUI || SMARTMAUI
        internal static void FinanceTransactionGroups_UpdateText(FinanceViewModel financeviewmodel)
        {
            financeviewmodel.FinanceTransactionsText = "";

            foreach (FinanceViewModel.TransactionGroupItem transactiongroup_item in financeviewmodel.temp_groups)
            {
                if (transactiongroup_item.IsChecked)
                {
                    if (financeviewmodel.FinanceTransactionsText != "")
                    {
                        financeviewmodel.FinanceTransactionsText += ", ";
                    }
                    financeviewmodel.FinanceTransactionsText += transactiongroup_item.Content;
                }
            }
            return;
        }
#endif

#if WINFORMS
        internal static void Finance_AddressesChanged_Actual(MainProcess process_components,
                                                             MainViewModel ourviewmodel,
                                                             FinanceViewModel financeviewmodel)
#endif
#if WPF || SMARTMAUI
        internal static void Finance_AddressesChanged_Actual(MainViewModel ourviewmodel,
                                                             FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal static void Finance_AddressesChanged_Actual(MainViewModel ourviewmodel,
                                                             FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        // I still don't know what to do with this fucker?  What do I do
        // when the address changes???
        internal static void Finance_AddressesChanged_Actual(MainViewModel ourviewmodel,
                                                             FinanceViewModel financeviewmodel)
#endif
        {
            // Sanity Check
            if (financeviewmodel.AddressSelectedIndex != -1)
            {
                MainViewModel.AddressItem addresses_item = (MainViewModel.AddressItem)financeviewmodel.FinanceAddressesList[financeviewmodel.AddressSelectedIndex];
                string udprn = addresses_item.Value.ToString();

                //List<SmartFinance.Categories> categories_found = SmartSpikeFinanceV2017.Finance_Find_CategoriesList(ourviewmodel,
                //                                                                        financeviewmodel,
                //                                                                        SmartParametersV2016.defaultChar);


                Finance_SubsetTransactions(
#if WINFORMS
                                                    process_components,
                                                    process_components.comboBoxGUIFinanceProviders,
                                                    process_components.comboBoxGUIFinanceAccounts,
                                                    process_components.comboBoxGUIFinanceTransactionGroups,
                                                    process_components.TransactionsDataGrid,
#endif
                                                    ourviewmodel,
                                                    financeviewmodel,
                                                    udprn,
                                                    true, // // DO respect Addresses
#if WINFORMS
                                                    financeviewmodel.StartDate,
                                                    financeviewmodel.EndDate);
#endif
#if WPF || SMARTMAUI
                                                    financeviewmodel.StartDate,
                                                    financeviewmodel.EndDate);
#endif
#if WINUI
                                                    financeviewmodel.StartDate,
                                                    financeviewmodel.EndDate);
#endif
#if ANDROIDX
                                                    financeviewmodel.StartDate.DateTime,
                                                    financeviewmodel.EndDate.DateTime);
#endif
            }
            return;
        }

#if WINFORMS
        internal static void Finance_ResetDates_Actual(MainProcess process_components,
                                                             MainViewModel ourviewmodel,
                                                             FinanceViewModel financeviewmodel)
#endif
#if WPF
        internal static void Finance_ResetDates_Actual(MainViewModel ourviewmodel,
                                                             FinanceViewModel financeviewmodel)
#endif
#if WINUI
        internal static void Finance_ResetDates_Actual(MainViewModel ourviewmodel,
                                                             FinanceViewModel financeviewmodel)
#endif
#if ANDROIDX
        // I still don't know what to do with this fucker?  What do I do
        // when the address changes???
        internal static void Finance_ResetDates_Actual(MainViewModel ourviewmodel,
                                                             FinanceViewModel financeviewmodel)
#endif
#if SMARTMAUI
        // I still don't know what to do with this fucker?  What do I do
        // when the address changes???
        internal static void Finance_ResetDates_Actual(MainViewModel ourviewmodel,
                                                       FinanceViewModel financeviewmodel)
#endif
        {
            // No Sanity Check
            financeviewmodel.StartDateEnabled =
            financeviewmodel.EndDateEnabled = false;
#if WINFORMS
            financeviewmodel.StartDate = SmartParametersV2016.defaultDate;
            financeviewmodel.EndDate = SmartParametersV2016.defaultMaxdate;
#endif
#if WPF || SMARTMAUI
            financeviewmodel.StartDate = SmartParametersV2016.defaultDate;
            financeviewmodel.EndDate = SmartParametersV2016.defaultMaxdate;
#endif
#if WINUI
            financeviewmodel.StartDate = SmartParametersV2016.defaultDate;
            financeviewmodel.EndDate = SmartParametersV2016.defaultMaxdate;
#endif
#if ANDROIDX
            financeviewmodel.StartDate.DateTime = SmartParametersV2016.defaultDate;
            financeviewmodel.EndDate.DateTime = SmartParametersV2016.defaultMaxdate;
#endif
            Finance_SubsetTransactions(
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
                                                        financeviewmodel.StartDate,
                                                        financeviewmodel.EndDate);
#endif
#if WPF || SMARTMAUI
                                                        financeviewmodel.StartDate,
                                                        financeviewmodel.EndDate);
#endif
#if WINUI
                                                        financeviewmodel.StartDate,
                                                        financeviewmodel.EndDate);
#endif
#if ANDROIDX
                                                        financeviewmodel.StartDate.DateTime,
                                                        financeviewmodel.EndDate.DateTime);
#endif
            financeviewmodel.StartDateEnabled =
            financeviewmodel.EndDateEnabled = true;
            return;
        }

        internal static void TurnOffFinanceStatus(
#if WINFORMS
                                                    MainProcess process_components,
#endif
                                                    FinanceViewModel financeviewmodel)
        {
            if (financeviewmodel != null)
            {
                financeviewmodel.InstitutionsEnabled =
                financeviewmodel.ProvidersEnabled =
                financeviewmodel.AccountsEnabled =
                financeviewmodel.TransactionGroupsEnabled =
                financeviewmodel.AddressesEnabled =
                financeviewmodel.FinanceCulturesEnabled = false;

                financeviewmodel.CategoriesEnabled =
                financeviewmodel.FinanceConversionsEnabled = false;

#if WINFORMS
                process_components.comboBoxGUIFinanceProviders.DropDownClosed -=
                                new EventHandler((s, e) => MainProcess.Finance_Providers_DropDownClosed(s, e, process_components));
                process_components.comboBoxGUIFinanceAccounts.DropDownClosed -=
                                                                new EventHandler((s, e) => MainProcess.Finance_Accounts_DropDownClosed(s, e, process_components));
                process_components.comboBoxGUIFinanceTransactionGroups.DropDownClosed -=
                                                                new EventHandler((s, e) => MainProcess.Finance_TransactionGroups_DropDownClosed(s, e, process_components));

                //process_components.RBBanks.CheckedChanged -= new EventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, process_components));
                //process_components.RBSavings.CheckedChanged -= new EventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, process_components));
                //process_components.RBInvestments.CheckedChanged -= new EventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, process_components));


                //process_components.radioButtonFinanceUK.CheckedChanged -=
                //    new EventHandler((s, e) => MainProcess.radioButtonFinance_CultureCheckedChanged(s, e, process_components));
                //process_components.radioButtonFinanceUS.CheckedChanged -=
                //    new EventHandler((s, e) => MainProcess.radioButtonFinance_CultureCheckedChanged(s, e, process_components));
                //process_components.radioButtonFinanceFR.CheckedChanged -=
                //    new EventHandler((s, e) => MainProcess.radioButtonFinance_CultureCheckedChanged(s, e, process_components));
                //process_components.radioButtonFinanceCA.CheckedChanged -=
                //    new EventHandler((s, e) => MainProcess.radioButtonFinance_CultureCheckedChanged(s, e, process_components));
#endif
                financeviewmodel.StartDateEnabled =
                financeviewmodel.ResetDatesEnabled =
                financeviewmodel.EndDateEnabled = false;

                financeviewmodel.FinanceCultureStatus = false;

                // These specials should always be opposite one another
                financeviewmodel.SubmitStatus = false;
                // Cancel button is always opposite of Submit
                financeviewmodel.CancelStatus = true;   // So we can Cancel
            }
            return;
        }

        internal static void TurnOnFinanceStatus(
#if WINFORMS
                                                    MainProcess process_components,
#endif
                                                    FinanceViewModel financeviewmodel)
        {
            if (financeviewmodel != null)
            {
                financeviewmodel.InstitutionsEnabled =
                financeviewmodel.ProvidersEnabled =
                financeviewmodel.AccountsEnabled =
                financeviewmodel.TransactionGroupsEnabled =
                financeviewmodel.AddressesEnabled =
                financeviewmodel.FinanceCulturesEnabled = true;

                financeviewmodel.CategoriesEnabled = true;
                financeviewmodel.FinanceConversionsEnabled = true;

                financeviewmodel.StartDateEnabled =
                financeviewmodel.ResetDatesEnabled =
                financeviewmodel.EndDateEnabled = true;

                financeviewmodel.FinanceCultureStatus = true;

                // These specials should always be opposite one another
                financeviewmodel.SubmitStatus = true;
                // Cancel button is always opposite of Submit
                financeviewmodel.CancelStatus = false;
#if WINFORMS
                // Manually add handler for when an item check state has been modified.
                process_components.comboBoxGUIFinanceProviders.DropDownClosed +=
                                new EventHandler((s, e) => MainProcess.Finance_Providers_DropDownClosed(s, e, process_components));
                process_components.comboBoxGUIFinanceAccounts.DropDownClosed +=
                                                new EventHandler((s, e) => MainProcess.Finance_Accounts_DropDownClosed(s, e, process_components));
                process_components.comboBoxGUIFinanceTransactionGroups.DropDownClosed +=
                                                new EventHandler((s, e) => MainProcess.Finance_TransactionGroups_DropDownClosed(s, e, process_components));

                //process_components.RBBanks.CheckedChanged += new EventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, process_components));
                //process_components.RBSavings.CheckedChanged += new EventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, process_components));
                //process_components.RBInvestments.CheckedChanged += new EventHandler((s, e) => MainProcess.radioButtonFinance_CheckedChanged(s, e, process_components));


                //process_components.radioButtonFinanceUK.CheckedChanged +=
                //                    new EventHandler((s, e) => MainProcess.radioButtonFinance_CultureCheckedChanged(s, e, process_components));
                //process_components.radioButtonFinanceUS.CheckedChanged +=
                //    new EventHandler((s, e) => MainProcess.radioButtonFinance_CultureCheckedChanged(s, e, process_components));
                //process_components.radioButtonFinanceFR.CheckedChanged +=
                //    new EventHandler((s, e) => MainProcess.radioButtonFinance_CultureCheckedChanged(s, e, process_components));
                //process_components.radioButtonFinanceCA.CheckedChanged +=
                //    new EventHandler((s, e) => MainProcess.radioButtonFinance_CultureCheckedChanged(s, e, process_components));

#endif
            }
            return;
        }

        internal static bool Finance_Do_We_Do_Something(SmartFinance.Categories categories_row,
                                           char category_code,
                                           string displayed_udprn)
        {
            // Fix this Ray

            //  Rules:
            //  If it doesn't match and X turn it to blank
            //  If it doesn't match and blank leave it alone
            //  If it matches and X leave it alone
            //  If it matches and blank turn it to X

            string udprn = "";

            char local_category_code = category_code;
            if (udprn == displayed_udprn)
            {
                if (local_category_code != category_code)
                {
                    if (categories_row.CHECKED == SmartParametersV2016.lastChecked)
                    {
                        categories_row.CHECKED = "";
                        return true;
                    }
                }
                else
                {
                    if (categories_row.CHECKED != SmartParametersV2016.lastChecked)
                    {
                        categories_row.CHECKED = "";
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool Finance_Build_Institutions_Dropdown(
#if ANDROIDX
                                                            AppCompatActivity meterActivity,
#endif
                                                            MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            char activeFlag)
        {
            // There is no resource_type passed into this routine ... Yes there is

            // We are doing Institutions within Areas and we need to do Areas within Suppliers
            // (because we may have a Supplier with no Areas, but we will never have an Area with no Suppliers)
            List<FinanceViewModel.InstitutionItem> temp_institutions = new List<FinanceViewModel.InstitutionItem>();
            //#if ANDROIDX
            //            financeviewmodel.FinanceInstitutionsList = new List<FinanceViewModel.InstitutionItem>();
            //#endif
            if (financeviewmodel.PLO.institutionsList.Count > 0)
            {
                int institution_id = 0;
                foreach (SmartFinance.Institutions info in financeviewmodel.PLO.institutionsList)
                {
                    string header = "";
                    string institution_name = "";
                   string infos_found = SmartSpikeFinanceV2017.Finance_Lookup_InstitutionName(ourviewmodel,
                                                                            financeviewmodel,
                                                                            //SmartParametersV2016.activeFlag,       // Either 'Y' for scrapeables or ALL of them
                                                                            info.INSTITUTION_CODE);
                    if (infos_found != "")
                    {
                        institution_name = infos_found;
#if WINFORMS
                        System.Drawing.Color tempColour = ourviewmodel.blackColour;
#endif
#if WPF || SMARTMAUI
                        Brush tempColour = ourviewmodel.blackColour;
#endif
#if WINUI
                        Brush tempColour = ourviewmodel.blackColour;
#endif
#if ANDROIDX
                        Color tempColour = ourviewmodel.blackColour;
#endif
                        List<SmartFinance.Logins> logins_found = SmartSpikeFinanceV2017.Finance_Find_Logins(ourviewmodel,
                                                                                                    financeviewmodel,
                                                                                                    activeFlag,       // Either 'Y' for scrapeables or ALL of them
                                                                                                    info.INSTITUTION_CODE,
                                                                                                    true); // Only those with CONNECTION_ID > 0
                        if (logins_found.Count > 0)
                        {
                            tempColour = ourviewmodel.greenColour;

                            //#if WINFORMS
                            //                            if (!string.IsNullOrEmpty(logins_found.First().CUSTOMER_NO) &&
                            //                                !string.IsNullOrEmpty(logins_found.First().DOB) &&
                            //                                !string.IsNullOrEmpty(logins_found.First().PASSCODE))
                            //                            {
                            //                                // Because in Windows WINUI our chimp developed picker cannot show Colours, we have to resort
                            //                                // to this crude indication

                            //                                header = SmartParametersV2016.defaultgreen.ToString();
                            //                            }                         
                            //#endif
                        }
                        FinanceViewModel.InstitutionItem institution_item = new FinanceViewModel.InstitutionItem()
                        {
                            InstitutionID = institution_id + 1,
                            Colour = tempColour,
                            Content = header + institution_name,
                            Value = Build_Tag_InstitutionNew(info.INSTITUTION_CODE)
                        };
                        // She is a MINE of mangled distortions, distractions, muddled facts, mis-represntations
                        // and mis-information regarding names
                        // dates, times, places people and events AND all of them put together!

                        temp_institutions.Add(institution_item);
                        //#if ANDROIDX
                        //                        financeviewmodel.FinanceSpinnerInstitutionsList.Add(institution_item);
                        //#endif
                        institution_id++;
                    }
                }

                // There is a selected Institution which is the first
                // You can choose any of them by clicking to set up
                // or change the connection info 
                financeviewmodel.FinanceInstitutionsList = temp_institutions;
#if ANDROIDX
                // This DOES deserver a fucking Scotch!  Worked first time!!!!
                // Unfuckingbelieveable!!!!
                InstitutionsAdapter adapter = new(meterActivity, financeviewmodel.FinanceInstitutionsList);
                financeviewmodel.FinanceInstitutions.Adapter = adapter;
#endif

                // Temporarily ignore the SelectionChanged event during initialization
                financeviewmodel.IDDisInitializing = true;
#if WPF || SMARTMAUI
                // Set the first item programmatically
                financeviewmodel.InstitutionSelectedIndex = 0;
                // Re-enable SelectionChanged logic
                financeviewmodel.IDDisInitializing = false;
#endif
#if WINUI
                // Set the first item programmatically
                financeviewmodel.InstitutionSelectedIndex = 0;
                // Re-enable SelectionChanged logic
                financeviewmodel.IDDisInitializing = false;
#endif
            }
            return true;
        }

#if ANDROIDX
        internal class InstitutionsAdapter : BaseAdapter<string>
        {
            private readonly Activity activity;
            private List<FinanceViewModel.InstitutionItem> institutions;
            public InstitutionsAdapter(Activity myactivity, List<FinanceViewModel.InstitutionItem> institutions)
            {
                this.activity = myactivity;
                this.institutions = institutions;
            }
            public override string this[int position] => institutions[position].Content;
            public override int Count => institutions.Count;
            public override Java.Lang.Object GetItem(int position)
            {
                FinanceViewModel.InstitutionItem xyz = institutions[position];
                //Java.Lang.Object abc = (Java.Lang.Object)xyz;
                return null;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                FinanceViewModel.InstitutionItem item = institutions[position];
                View view = activity.LayoutInflater.Inflate(Resource.Layout.SpinnerLayout, null);
                TextView textView = view.FindViewById<TextView>(Resource.Id.textView);
                // Change the item's color in popup view  
                textView.SetTextColor(item.Colour);
                textView.Text = item.Content;
                return view;
            }
        }

        internal class FinanceCulturesAdapter : BaseAdapter<string>
        {
            private readonly Activity activity;
            private List<MainViewModel.CultureItem> cultures;
            public FinanceCulturesAdapter(Activity myactivity, List<MainViewModel.CultureItem> cultures)
            {
                this.activity = myactivity;
                this.cultures = cultures;
            }
            public override string this[int position] => cultures[position].Content;
            public override int Count => cultures.Count;
            public override Java.Lang.Object GetItem(int position)
            {
                //.Lang.Object abc = institutions[position].ToString() as Java.Lang.Object;
                return null;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                MainViewModel.CultureItem item = cultures[position];
                View view = activity.LayoutInflater.Inflate(Resource.Layout.SpinnerLayout, null);
                //ImageView icon = view.FindViewById<ImageView>(Resource.Id.imageView);
                TextView textView = view.FindViewById<TextView>(Resource.Id.textView);
                textView.SetTextColor(item.Colour);
                textView.Text = item.Content;
                // Oh and make sure the FUCKING IMAGE exists, of course ...
                //icon.SetImageResource(SetImageId(item.Source));
                return view;
            }

            internal static int SetImageId(string source)
            {
                // Make sure all the Images are 'AndroidResource' !!!!
                return (int)typeof(Resource.Drawable).GetField(source).GetValue(null);
            }
        }
#endif

        internal static async Task<bool> ButtonAutoSwitchFinanceClickActual(SignInViewModel signinviewmodel,
                                                                        MainViewModel ourviewmodel,
                                                                        FinanceViewModel financeviewmodel)
        {
            // One day .... 
            //         ... well THAT DAY is 28-May-2017!!!
            if (ourviewmodel.SmartProfile.profilecubefacesList.Count > 0)
            {
                char toggle = Convert.ToChar(SmartParametersV2016.noFlag);
                // Set the toggle switch to 'On'

#if WINFORMS || WPF || SMARTMAUI
                if (financeviewmodel.AutoSwitchFinanceColour == ourviewmodel.darkorangeColour)
#endif
#if WINUI
                 if (financeviewmodel.AutoSwitchFinanceColour == ourviewmodel.darkorangeColour)
#endif
#if ANDROIDX
                if (financeviewmodel.AutoSwitchFinanceColour == ourviewmodel.darkorangeColour)
#endif
                {
                    toggle = Convert.ToChar(SmartParametersV2016.yesFlag);
                }

                foreach (SmartProfile.Cubefaces cubeface_row in ourviewmodel.SmartProfile.profilecubefacesList)
                {
                    bool do_something = Toggle_AutoSwitch(cubeface_row,
                                        toggle);
                    if (do_something)
                    {
                        SmartFinance.Switches switches_row = new SmartFinance.Switches();
                        if (!string.IsNullOrEmpty(switches_row.ToString()))
                        {
                            switches_row.AUTOSWITCH = cubeface_row.FACE_AUTOSWITCH;
                            switches_row.Updated = true;
                            financeviewmodel.PLO.finance_switches_changesList.Add(switches_row);
                            if (!await SmartFinanceScrapeV2023.DoAllSmartFinance(ourviewmodel,
                                                            financeviewmodel,
                                                            false,
                                                            SmartParametersV2016.sqliteformat))
                            {
#if WPF || SMARTMAUI
                                financeviewmodel.errorMessage = "Cannot update switches(4) - cannot continue";
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                // This is a bit fatal, because if we can't do this, there's
                                // every chance we can't do lots of things we need to further on

#endif
#if WINUI
                                financeviewmodel.errorMessage = "Cannot update switches(4) - cannot continue";
                                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, financeviewmodel.errorMessage);
                                // This is a bit fatal, because if we can't do this, there's
                                // every chance we can't do lots of things we need to further on

#endif
                                if (!ourviewmodel.quitCts.IsCancellationRequested)
                                {
                                    // Nothing ever turns this back from Red
                                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 44: Updating Resource"))
                                    {
                                        return false;
                                    }
                                }
                                return false;
                            }
                        }
                    }
                }
                // DarkRed turned to darkorange
                if (toggle == Convert.ToChar(SmartParametersV2016.yesFlag))
                {
                    financeviewmodel.AutoSwitchFinanceColour = ourviewmodel.darkorangeColour;
#if ANDROIDX
                    ourviewmodel.AutoSwitch0 = ourviewmodel.openred0;
                    ourviewmodel.AutoSwitch25 = ourviewmodel.openred25;
                    ourviewmodel.AutoSwitch50 = ourviewmodel.openred50;
                    ourviewmodel.AutoSwitch75 = ourviewmodel.openred75;
#endif
                }
                else
                {
                    financeviewmodel.AutoSwitchFinanceColour = ourviewmodel.darkgreenColour;
#if ANDROIDX
                    ourviewmodel.AutoSwitch0 = ourviewmodel.autoswitch0;
                    ourviewmodel.AutoSwitch25 = ourviewmodel.autoswitch25;
                    ourviewmodel.AutoSwitch50 = ourviewmodel.autoswitch50;
                    ourviewmodel.AutoSwitch75 = ourviewmodel.autoswitch75;
#endif
                }
            }
            return true;
        }

        internal static bool Toggle_AutoSwitch(SmartProfile.Cubefaces cubeface_row,
                                                char toggle)
        {
            cubeface_row.FACE_AUTOSWITCH = toggle;
            // Its ok to use async void on Event Handlers
            return true;
        }

        internal static async Task<bool> Finance_CultureChanged_Actual(
#if WINFORMS
                                                              MainProcess process_components,
#endif
#if ANDROIDX
                                                              AppCompatActivity meterActivity,
#endif
                                                              MainViewModel ourviewmodel,
                                                              FinanceViewModel financeviewmodel)
        {
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOffFinanceStatus(
#if WINFORMS
                                 process_components,
#endif
                                 financeviewmodel);
            bool status = false;
            // Despite everything and all the pain
            // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
            if (!await DisplayFinanceMeterAsync(
#if WINFORMS
                                                    process_components,
#endif
#if ANDROIDX
                                                     meterActivity,
#endif
                                                     ourviewmodel,
                                                     financeviewmodel,
                                                     SmartParametersV2016.activeFlag,
                                                     SmartParametersV2016.defaultDate))
            {
                // We failed because of a Cancellation ... but which one? Check
                if (!ourviewmodel.quitCts.IsCancellationRequested)
                {
                    // If we DIDN'T request a cancellation
                    // We do nothing if there is a failure .. !  SmartSwitch DOESN'T fail!!
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 123: DisplayFinanceMeterAsync"))
                    {
                        return false;
                    }
                }
            }
            else
            {
                foreach (SmartProfile.Cubefaces cubeface_row in ourviewmodel.SmartProfile.profilecubefacesList)
                {
                    if (cubeface_row.CUBEFACE_CODE == SmartParametersV2016.Finance)
                    {
                        cubeface_row.FACE_CULTURE_CODE = financeviewmodel.FinanceCultureCode;
                        cubeface_row.Updated = true;
                        ourviewmodel.SmartProfile.profilecubefacesChangesList.Add(cubeface_row);
                        break;
                    }
                }

                if (!await SmartRoutinesV2018.DoAllSmartProfile(ourviewmodel,
                                                                SmartParametersV2016.sqliteformat))
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 45: Finance Culture Switch"))
                    {
                        return false;
                    }
                }
                else
                {
                    // Tell the console we have switched
                    if (await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel, "Finance Culture switched to: " + financeviewmodel.FinanceCultureCode))
                    {
                        status = true;
                    }
                }
            }
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOnFinanceStatus(
#if WINFORMS
                                 process_components,
#endif
                                 financeviewmodel);
            return status;    // Either nothing was done OR something was done successfully
        }

        internal static async Task<bool> Finance_RateChanged_Actual(
#if WINFORMS
                                                              MainProcess process_components,
#endif
#if ANDROIDX
                                                              AppCompatActivity meterActivity,
#endif
                                                              MainViewModel ourviewmodel,
                                                              FinanceViewModel financeviewmodel,
                                                              string targetCurrency)
        {
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOffFinanceStatus(
#if WINFORMS
                                 process_components,
#endif
                                 financeviewmodel);
            bool status = false;
            // Despite everything and all the pain
            // your program is ABSOLUTELY beautiful Ray, and its ALL YOURS!
            //FinanceUpdateRate(financeviewmodel);

            if (!await DisplayFinanceMeterAsync(
#if WINFORMS
                                                    process_components,
#endif
#if ANDROIDX
                                                     meterActivity,
#endif
                                                     ourviewmodel,
                                                     financeviewmodel,
                                                     SmartParametersV2016.activeFlag,
                                                     SmartParametersV2016.defaultDate))
            {
                // We failed because of a Cancellation ... but which one? Check
                if (!ourviewmodel.quitCts.IsCancellationRequested)
                {
                    // If we DIDN'T request a cancellation
                    // We do nothing if there is a failure .. !  SmartSwitch DOESN'T fail!!
                    FrontEndGUI.SetLedColour(ourviewmodel, 8, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 446: DisplayFinanceMeter"))
                    {
                        return false;
                    }
                }
            }
            else
            {
                foreach (SmartProfile.Cubefaces cubeface_row in ourviewmodel.SmartProfile.profilecubefacesList)
                {
                    if (cubeface_row.CUBEFACE_CODE == SmartParametersV2016.Finance)
                    {
                        cubeface_row.FACE_CURRENCY = financeviewmodel.CurrencyOrdinal;
                        cubeface_row.Updated = true;
                        ourviewmodel.SmartProfile.profilecubefacesChangesList.Add(cubeface_row);
                        break;
                    }
                }

                if (!await SmartRoutinesV2018.DoAllSmartProfile(ourviewmodel,
                                                                SmartParametersV2016.sqliteformat))
                {
                    FrontEndGUI.SetLedColour(ourviewmodel, 6, ourviewmodel.redColour);
                    if (!await SmartRoutinesV2018.CheckTrace(ourviewmodel, financeviewmodel.financeToken, 0, 0, "Problem 46: Finance Currency Switch"))
                    {
                        return false;
                    }
                }
                else
                {
                    // Tell the console we have switched
                    if (await SmartRoutinesV2018.TextBlockUpdate(
#if ANDROIDX
                                                                meterActivity,
#endif
                                                                ourviewmodel, "Finance Currency switched to: " + targetCurrency))
                    {
                        status = true;
                    }
                }
            }
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOnFinanceStatus(
#if WINFORMS
                                 process_components,
#endif
                                 financeviewmodel);
            return status;    // Either nothing was done OR something was done successfully
        }

#if WINFORMS
        internal static bool Finance_ButtonExchangeRatesClick_Actual(
#endif
#if WPF
        internal static bool Finance_ButtonExchangeRatesClick_Actual(
#endif
#if WINUI
        internal static async Task<bool> Finance_ButtonExchangeRatesClick_Actual(
#endif
#if ANDROIDX
        internal static bool Finance_ButtonExchangeRatesClick_Actual(
#endif
#if SMARTMAUI
        internal static async Task<bool> Finance_ButtonExchangeRatesClick_Actual(
#endif
#if WPF || SMARTMAUI
                                                                    Popup contentpage,
#endif
#if WINUI
                                                                    ContentDialog contentpage,
#endif
#if ANDROIDX
                                                                    Android.Views.View contentpage,
#endif
                                                                    MainViewModel ourviewmodel,
                                                                    FinanceViewModel financeviewmodel)
        {

            try
            {
#if WPF
                financeviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                FinanceExchangeRatesPopup exchangerates_popup = new FinanceExchangeRatesPopup();
                contentpage.IsOpen = true;
#endif
#if WINUI
                financeviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                FinanceExchangeRatesPopup exchangerates_popup = new FinanceExchangeRatesPopup();
                await contentpage.ShowAsync();
#endif
#if ANDROIDX
                FinanceExchangeRatesPopup exchangerates_popup = new FinanceExchangeRatesPopup();
                GravityFlags gflags = new GravityFlags();
                exchangerates_popup.ShowAtLocation(contentpage, gflags, 0, 0);
#endif
#if SMARTMAUI
                financeviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                Popup exchangerates_popup = contentpage;
                await Shell.Current.ShowPopupAsync(exchangerates_popup);
#endif

                // What an absolute fucking bitch all this DataGridView stuff is ..
                // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
            }
            catch (Exception ex)
            {
                financeviewmodel.errorMessage = ex.Message;
                return false;
            }
            return true;
        }

#if WINFORMS || SMARTMAUI
        // Don't really do double-click
        internal static bool ChartsMouseDoubleClick_Actual(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                            string[] tabinfo)
        {

            // Did we get here on a Double Click?
            if (tabinfo.Length != 2)   // <= This needs fixing
            {
                return true;
            }
            return false;
        }
#endif

        
#if WINFORMS
        internal static bool ChartsMouseDoubleClick_ActualX(MainProcess process_components,
#endif
#if WPF || SMARTMAUI
        internal static bool ChartsMouseDoubleClick_Actual(
#endif
#if WINUI
        internal static bool ChartsMouseDoubleClick_Actual(
#endif
#if ANDROIDX
        internal static bool ChartsMouseDoubleClick_Actual(
#endif

                                                        SignInViewModel signinviewmodel,
                                                        MainViewModel ourviewmodel,
                                                        FinanceViewModel financeviewmodel,
                                                        string[] tabinfo)
        {
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOffFinanceStatus(
#if WINFORMS
                                     process_components,
#endif
                                    financeviewmodel);
            bool status = false;
            if (tabinfo[0] != "")
            {
                // Pretty sure the Label doesn't have a parent?
                // However we can find out which tab item it is by working forward
                // from a TabControl statement which is underneath the Charts tab
                // and then down through all the TabItems below the TabControl ...
                // A kludge but .. can YOU work out the TabItem from the label??
                // No? Then fuck off, smart arse

                try
                {
#if WPF
                    financeviewmodel.PopupHeight = Application.Current.MainWindow.Height;
                    financeviewmodel.PopupWidth = Application.Current.MainWindow.Width;
                    financeviewmodel.childWindow = new Window
                    {
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Topmost = true,
                        Title = tabinfo[1]
                    };
#endif
#if WINUI
                    financeviewmodel.PopupHeight = ourviewmodel.currentHeight - ourviewmodel.currentHeight / 10;
                    financeviewmodel.PopupWidth = ourviewmodel.currentWidth;
                    financeviewmodel.childWindow = new Popup
                    {

                        Height = financeviewmodel.PopupHeight,
                        Width = financeviewmodel.PopupWidth//,
                        //DesiredPlacement = PopupPlacementMode.Auto
                        //Title = tablabel
                    };
#endif
#if WINFORMS || WPF
                    PlotView popup_plotview = new PlotView();
#endif
#if WINUI
#if OXYPLOT
                    PlotView popup_plotview = new PlotView();
#else
                    PlotModel popup_plotview = null;
#endif
#endif
#if WPF
                    financeviewmodel.childWindow.Content = popup_plotview;
                    // Create some events so we can escape from the popup
                    financeviewmodel.childWindow.MouseDoubleClick += new MouseButtonEventHandler((s, e) => FinanceView.Chart_MouseDoubleClick(s, e, financeviewmodel));
                    financeviewmodel.childWindow.MouseLeave += new MouseEventHandler((s, e) => FinanceView.Chart_MouseLeave(s, e, ourviewmodel, financeviewmodel));
#endif
#if WINUI
                    // No idea how to do this ??
                    // financeviewmodel.childWindow.Child = popup_plotview;

                    // Create some events so we can escape from the popup
                    //financeviewmodel.childWindow.MouseDoubleClick += new MouseButtonEventHandler(FinanceView.Chart_MouseDoubleClick);
                    financeviewmodel.childWindow.Tapped +=
                        new TappedEventHandler((s, e) => ChildWindowTapped(s, e, financeviewmodel)); // += new MouseEventHandler(FinanceView.Chart_MouseLeave); 
                    financeviewmodel.childWindow.PointerExited += new PointerEventHandler((s, e) => FinanceView.Chart_MouseLeave(s, e, ourviewmodel, financeviewmodel));
#endif
#if ANDROIDX
                    //financeviewmodel.PopupHeight = Application.Current.MainPage.Height;
                    //financeviewmodel.PopupWidth = Application.Current.MainPage.Width;
                    //FinanceChartsPopup charts_popup = new FinanceChartsPopup();
#endif
#if SMARTMAUI
                    var popup_plotview = financeviewmodel.ChartSeries;
                    //popup_plotview.Model = new PlotModel(); // <= Need to fill this in!!!
                    //VerticalStackLayout target_dockpanel = new VerticalStackLayout();
                    //target_dockpanel.Children.Add(popup_plotview);
                    //financeviewmodel.childWindow.Content = target_dockpanel;
                    // Create some events so we can escape from the popup
                    //financeviewmodel.childWindow.MouseDoubleClick += new MouseButtonEventHandler((s, e) => FinanceView.Chart_MouseDoubleClick(s, e, financeviewmodel));
                    //financeviewmodel.childWindow.MouseLeave += new MouseEventHandler((s, e) => FinanceView.Chart_MouseLeave(s, e, ourviewmodel, financeviewmodel));
#endif
#if ANDROIDX
                    // This needs fixing
                    PlotView popup_plotview = null; // (PlotView)charts_popup.FindByName("PopupPlotView");
#endif
#if SMARTMAUI
                    var xaxes = new Axis[1];
                    var yaxes = new Axis[1];
                    if (popup_plotview != null)
                    {
                        //PlotModel pmodel = new PlotModel();
                        ISeries[] pmodel = financeviewmodel.ChartSeries;
                        // So we can change the header title in the XAML file
                        switch (tabinfo[0])
                        {
                            case "1":
                                pmodel = SmartLiveCharts2V2026.CreateDebitsCreditsSeries("title",
                                                                                    ourviewmodel,
                                                                                    financeviewmodel,
                                                                                    out xaxes,
                                                                                    out yaxes);
                                break;
                            default:
                                break;
                        }

                        if (pmodel != null)
                        {
#if OXYPLOT
                            popup_plotview.Model = pmodel;
#endif
                            string desc = SmartSpikeV2017.Lookup_Button_Description(signinviewmodel.Fatah.buttonsList,
                                                         SmartParametersV2016.Finance,
                                                         financeviewmodel.category_code); // <= BUTTON_CODE
                            //financeviewmodel.FinanceChartsPlotTitle = desc + " - " + (DateTime.Now + ourviewmodel.utcOffset).ToString(SmartParametersV2016.militaryFormat); // Local time
#if WPF
                            // Show the pop-up window
                            financeviewmodel.childWindow.Show();
#endif
#if WINUI
                            // Show the pop-up window
                            financeviewmodel.childWindow.IsOpen = true;
#endif
#if ANDROIDX
                            // This needs fixing
                            //GravityFlags gflagsx = new GravityFlags();
                            //charts_popup.ShowAtLocation(popup_plotview, gflagsx, 0, 0);
#endif
#if SMARTMAUI
                            // Show the pop-up window
                            Shell.Current.ShowPopupAsync(financeviewmodel.childWindow);
#endif
                        }
                    }
#endif
                    // What an absolute fucking bitch all this PlotView/PlotModel stuff is ..
                    // When you copy the ItemSource, the Rows are copied ... but the Columns aren't!!                            
                    status = true;
                }
                catch (Exception ex)
                {
                    financeviewmodel.errorMessage = ex.Message;
                    status = false;
                }
            }
            // We are going to get info? Better turn off some buttons first!
            // These can all be turned off now, except the Cancel button
            TurnOnFinanceStatus(
#if WINFORMS
                                     process_components,
#endif
                                 financeviewmodel);
            return status;
        }

#if WINUI
        private static void ChildWindowTapped(object sender, TappedRoutedEventArgs e, FinanceViewModel financeviewmodel)
        {
            if (sender != null && e != null)
            {
                financeviewmodel.childWindow.IsOpen = false;
            }
            //throw new NotImplementedException();
            return;
        }
#endif
        internal static void Decode_Tag_Institution(string tag,
                                                    ref short institution_code)
        {
            string[] Bouncer = tag.Split(SmartParametersV2016.fieldSeparator);
            if (Bouncer.Length > 0)
            {
                institution_code = Convert.ToInt16(Bouncer[0].ToString());
            }
            return;
        }

        internal static string Build_Tag_InstitutionNew(short institution_code)
        {
            return institution_code.ToString();
        }
        internal static string Build_Tag_InstitutionX(short institution_code, short brand_code, bool twoFactor)
        {
            return institution_code.ToString() + SmartParametersV2016.fieldSeparator +
                    brand_code.ToString() + SmartParametersV2016.fieldSeparator +
                    twoFactor.ToString();
        }

        internal static string Build_Tag_Provider(short institution_code, 
                                                    short brand_code,
                                                    char twoFactor)
        {
            return institution_code.ToString() + SmartParametersV2016.fieldSeparator +
                    brand_code.ToString() + SmartParametersV2016.fieldSeparator +
                    twoFactor.ToString();
        }

        internal static void Decode_Tag_Provider(string tag,
                                                ref short institution_code,
                                                ref short brand_code,
                                                ref char twoFactor)
        {
            string[] Bouncer = tag.Split(SmartParametersV2016.fieldSeparator);
            if (Bouncer.Length > 2) // was 3
            {
                institution_code = Convert.ToInt16(Bouncer[0].ToString());
                brand_code = Convert.ToInt16(Bouncer[1].ToString());
                twoFactor = Convert.ToChar(Bouncer[2].ToString());
            }
            return;
        }

        internal static void Decode_AccountTag_Extra(string tag,
                                                ref short temp_institution_code,
                                                ref short temp_brand_code,
                                                ref string sortcode,
                                                ref string account_no,
                                                ref string udprn,
                                                ref short ordinal)
        {
            string[] Bouncer = tag.Split(SmartParametersV2016.fieldSeparator);
            if (Bouncer.Length >= 5)
            {
                temp_institution_code = Convert.ToInt16(Bouncer[0].ToString());
                temp_brand_code = Convert.ToInt16(Bouncer[1].ToString());
                sortcode = Bouncer[2];
                account_no = Bouncer[3];
                udprn = Bouncer[4];
                ordinal = Convert.ToInt16(Bouncer[5]);
            }
            return;
        }

        internal static string Build_AccountTag_Extra(short institution_code,
                                                short brand_code,
                                                string sortcode,
                                                string account_no,
                                                string udprn,
                                                short ordinal)
        {
            return institution_code.ToString() + SmartParametersV2016.fieldSeparator +
                    brand_code.ToString() + SmartParametersV2016.fieldSeparator +
                    sortcode + SmartParametersV2016.fieldSeparator +
                    account_no + SmartParametersV2016.fieldSeparator +
                    udprn + SmartParametersV2016.fieldSeparator +
                    ordinal.ToString();
        }

        internal static string Build_Tag_Extra_TransactionTypes(short institution_code,
                                                                short brand_code,
                                                                string sortcode,
                                                                string account_no,
                                                                short transactiontype,
                                                                bool paid_in)
        {
            return institution_code.ToString() + SmartParametersV2016.fieldSeparator +
                    brand_code.ToString() + SmartParametersV2016.fieldSeparator +
                    sortcode + SmartParametersV2016.fieldSeparator +
                    account_no + SmartParametersV2016.fieldSeparator +
                    transactiontype.ToString() + SmartParametersV2016.fieldSeparator +
                    paid_in.ToString();

        }

        internal static void Decode_Tag_Extra_TransactionTypes(string tag,
                                                                ref short institution_code,
                                                                ref short brand_code,
                                                                ref string sortcode,
                                                                ref string account_no,
                                                                ref short transactionType,
                                                                ref bool paid_in)
        {
            string[] Bouncer = tag.Split(SmartParametersV2016.fieldSeparator);
            if (Bouncer.Length > 5)
            {
                institution_code = Convert.ToInt16(Bouncer[0].ToString());
                brand_code = Convert.ToInt16(Bouncer[1].ToString());
                sortcode = Bouncer[2];
                account_no = Bouncer[3];
                transactionType = Convert.ToInt16(Bouncer[4].ToString());
                paid_in = Convert.ToBoolean(Bouncer[5].ToString());
            }
            return;
        }

        internal static string Build_Tag_Transaction(short institution_code,
                                                    short brand_code,
                                                    bool transaction_paid_in,
                                                    short transgroup_code,
                                                    short transaction_code)
        {
            return institution_code.ToString() + SmartParametersV2016.fieldSeparator +
                    brand_code.ToString() + SmartParametersV2016.fieldSeparator +
                    transaction_paid_in + SmartParametersV2016.fieldSeparator +
                    transgroup_code.ToString() + SmartParametersV2016.fieldSeparator +
                    transaction_code;
        }

        internal static void BuildDebitsCreditsChart(
#if WINFORMS
                                                MainProcess components,
#endif
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel)
        {
#if SMARTMAUI
            financeviewmodel.ChartSeries =
                SmartLiveCharts2V2026.CreateDebitsCreditsSeries(
                    "Debits/Credits",
                    ourviewmodel,
                    financeviewmodel,
                    out Axis[] xaxes,
                    out Axis[] yaxes);

            financeviewmodel.XAxes = xaxes;
            financeviewmodel.YAxes = yaxes;
#else

            financeviewmodel.PlotModel2 = SmartChartsV2024.CreateDebitsCreditsSeries("", ourviewmodel, financeviewmodel);
#endif
#if WINFORMS
            components.FinancePlotView2.Model = financeviewmodel.PlotModel2;
#endif
            return;
        }
        
#if WINFORMS
        internal static TabPage CreateTabItem(MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        int tabIndex,
                                        string symbol)
        {
            // Create TabPage
            TabPage tabPage = new TabPage(symbol);
            // Create Outer TableLayoutPanel (3 rows)
            TableLayoutPanel outerTableLayout = new TableLayoutPanel
            {
                RowCount = 3,
                ColumnCount = 2, // Only 2 column in the outer table
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = ourviewmodel.orangeColour,
                Padding = new Padding(0, 0, 0, 0),
                Margin = new Padding(0, 0, 0, 0)
            };
            // Loop to add 3 rows, each containing a 2-column inner TableLayoutPanel
            for (int row = 0; row < 3; row++)
            {
                List<SmartFinance.TotalsView> money = financeviewmodel.FinanceTotals[tabIndex, row];
                if (money.Count() > 0)
                {
                    // Create BindingSource
                    BindingSource bindingSource = new BindingSource { DataSource = money };
                    // Create TextBox for Column 1 and bind it
                    System.Windows.Forms.TextBox textBox1 = new System.Windows.Forms.TextBox
                    {
                        Dock = DockStyle.Left,
                        TextAlign = System.Windows.Forms.HorizontalAlignment.Left,

                        Font = new Font("Microsoft Sans Serif", 13F),
                        BackColor = ourviewmodel.orangeColour,
                        ForeColor = ourviewmodel.blackColour,
                        Padding = new Padding(0, 0, 0, 0),
                        Margin = new Padding(0, 0, 0, 0)
                    };
                    textBox1.DataBindings.Add("Text", bindingSource, "DESCRIPTION");
                    // Create TextBox for Column 2 and bind it
                    System.Windows.Forms.TextBox textBox2 = new System.Windows.Forms.TextBox
                    {
                        Dock = DockStyle.Right,
                        TextAlign = System.Windows.Forms.HorizontalAlignment.Right,
                        Font = new Font("Microsoft Sans Serif", 13F),
                        BackColor = ourviewmodel.orangeColour,
                        ForeColor = ourviewmodel.blackColour,
                        Padding = new Padding(0, 0, 0, 0),
                        Margin = new Padding(0, 0, 0, 0)
                    };
                    textBox2.DataBindings.Add("Text", bindingSource, "AMOUNT");
                    // Add the TextBoxes to the inner TableLayoutPanel (2 columns)
                    outerTableLayout.Controls.Add(textBox1, 0, row); // Column 1
                    outerTableLayout.Controls.Add(textBox2, 1, row); // Column 2
                }
            }

            // Add the outer TableLayoutPanel to the TabPage
            tabPage.Controls.Add(outerTableLayout);
            return tabPage;
        }
#endif

#if WPF
        internal static TabItem CreateTabItem(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                int tabIndex,
                                                string symbol)
        {
            TabItem tabItem = new TabItem()
            {
                Padding = new System.Windows.Thickness(0, 0, 0, 0),
                Margin = new System.Windows.Thickness(0, 0, 0, 0),
                BorderThickness = new System.Windows.Thickness(0, 0, 0, 0),
                Header = symbol,
                FontSize = 7,
                Height = 20,
                Background = ourviewmodel.lightgreenColour,
                Width = 20,
                //HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                //VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch,
                Content = CreateGrid(ourviewmodel, financeviewmodel, tabIndex)
            };
            return tabItem;
        }
#endif
#if WINUI
        internal static void BuildTabContent(MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        int tabIndex,
                                        string symbol)
        {

            financeviewmodel.TabTitles.Add(new TabTitle() { Name = symbol });
            financeviewmodel.TabContentColour = new SolidColorBrush(Colors.Orange);
            financeviewmodel.grids.Add(CreateGrid(ourviewmodel, financeviewmodel, tabIndex));
        }
#endif
#if ANDROIDX
        internal static void BuildTabContent(FinanceViewModel financeviewmodel,
                                        string symbol)
        {
            financeviewmodel.TabTitles.Add(symbol);
            return;
        }
#endif
#if SMARTMAUI
        internal static TabbedPage CreateTabItem(MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                int tabIndex,
                                                string symbol)
        {
            TabbedPage tabItem = new TabbedPage()
            {
                //Padding = new System.Windows.Thickness(0, 0, 0, 0),
                //Margin = new System.Windows.Thickness(0, 0, 0, 0),
                //BorderThickness = new System.Windows.Thickness(0, 0, 0, 0),
                //Header = symbol,
                //FontSize = 7,
                //Height = 20,
                //Background = ourviewmodel.lightgreenColour,
                //Width = 20,
                //HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                //VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch,
                //Content = CreateGrid(ourviewmodel, financeviewmodel, tabIndex)
            };
            return tabItem;
        }
#endif

#if WPF  || WINUI || SMARTMAUI
        internal static Grid CreateGrid(MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel,
                                        int tabIndex)
        {
            Grid grid = new Grid();

#if WPF
            grid.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            grid.ShowGridLines = false;
            grid.Margin = new System.Windows.Thickness(0, 0, 0, 0);

#endif
#if SMARTMAUI
            grid.VerticalOptions = LayoutOptions.Fill;
            grid.HorizontalOptions = LayoutOptions.Fill;
            // grid.ShowGridLines = false; No grid lines in MAUI
            grid.Margin = new Thickness(0, 0, 0, 0);
#endif
#if WINUI
            // Nothing I seem to be able to do, can stop or change the FontSize
            // from being 18? and the grid rows being way too high.
            // I wasted a day of my life trying to fix this, all to no avail ...
            //grid.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch;
            //grid.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
            grid.VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Stretch;
            grid.HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch;

            //grid.ShowGridLines = false;
            // All this to keep the rows 'tight'
            grid.BorderThickness = new Thickness(0, 0, 0, 0);
            grid.Margin = new Thickness(0, 0, 0, 0);
            grid.Padding = new Thickness(0, 0, 0, 0);
            grid.RowSpacing = 0;
            grid.ColumnSpacing = 0;
#endif
            RowDefinition row0 = new RowDefinition()
            {
                Height = new GridLength(33.0, GridUnitType.Star)
            };
            grid.RowDefinitions.Add(row0);
            RowDefinition row1 = new RowDefinition()
            {
                Height = new GridLength(33.0, GridUnitType.Star)
            };
            grid.RowDefinitions.Add(row1);
            RowDefinition row2 = new RowDefinition()
            {
                Height = new GridLength(34.0, GridUnitType.Star)
            };
            grid.RowDefinitions.Add(row2);

            grid.Children.Add(CreateDataGrid(ourviewmodel, financeviewmodel, "Paidin", 0, 0, tabIndex));
            grid.Children.Add(CreateDataGrid(ourviewmodel, financeviewmodel, "Paidout", 1, 0, tabIndex));
            grid.Children.Add(CreateDataGrid(ourviewmodel, financeviewmodel, "Difference", 2, 0, tabIndex));
            return grid;
        }
#endif

#if WPF  || WINUI
        internal static DataGrid CreateDataGrid(MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel,
                                            string name,
                                            int row,
                                            int column,
                                            int tabIndex)
        {
#if WPF  || WINUI
            DataGrid abc = new DataGrid();
#endif

            abc.Name = name;
#if WPF
            abc.BorderThickness = new Thickness(0, 0, 0, 0);
#endif
#if WINUI
            // Nothing I seemed to be able to do could change the FontSize
            // OR stop the grid rows from being too high
            abc.BorderThickness = new Thickness(0, 0, 0, 0);
            abc.Padding = new Thickness(0, 0, 0, 0);
#endif
            abc.Background = ourviewmodel.orangeColour;
            abc.AutoGenerateColumns = false;
            abc.HeadersVisibility = DataGridHeadersVisibility.None;
            abc.GridLinesVisibility = DataGridGridLinesVisibility.None;
            abc.IsReadOnly = true;
            Grid.SetRow(abc, row);
            Grid.SetColumn(abc, column);

            switch (name)
            {
                case "Paidin":
#if WPF
                    abc.SetBinding(DataGrid.FontSizeProperty, new Binding("PaidInFontSize"));
#endif
#if WINUI
                    abc.SetBinding(DataGrid.FontSizeProperty, new Binding()
                    {
                        Path = new PropertyPath("PaidInFontSize"),
                        Source = financeviewmodel
                    });
                    abc.FontSize = financeviewmodel.PaidInFontSize;
#endif
                    abc.ItemsSource = financeviewmodel.FinanceTotals[tabIndex, 0];
                    break;
                case "Paidout":
#if WPF
                    abc.SetBinding(DataGrid.FontSizeProperty, new Binding("PaidOutFontSize"));
#endif
#if WINUI
                    abc.SetBinding(DataGrid.FontSizeProperty, new Binding()
                    {
                        Path = new PropertyPath("PaidOutFontSize"),
                        Source = financeviewmodel
                    });
                    abc.FontSize = financeviewmodel.PaidOutFontSize;
#endif
                    abc.ItemsSource = financeviewmodel.FinanceTotals[tabIndex, 1];
                    break;
                case "Difference":
#if WPF
                    abc.SetBinding(DataGrid.FontSizeProperty, new Binding("DifferenceFontSize"));
#endif
#if WINUI
                    abc.SetBinding(DataGrid.FontSizeProperty, new Binding()
                    {
                        Path = new PropertyPath("DifferenceFontSize"),
                        Source = financeviewmodel
                    });
                    abc.FontSize = financeviewmodel.DifferenceFontSize;
#endif
                    abc.ItemsSource = financeviewmodel.FinanceTotals[tabIndex, 2];
                    break;
            }
#if WPF  || WINUI
            abc.Columns.Add(CreateColumn(ourviewmodel, financeviewmodel, "DESCRIPTION", 1.0, true));
            abc.Columns.Add(CreateColumn(ourviewmodel, financeviewmodel, "AMOUNT", 3.0, false));
#endif
            return abc;
        }
#endif

#if SMARTMAUI
        internal static View CreateDataGrid(
            MainViewModel ourviewmodel,
            FinanceViewModel financeviewmodel,
            string name,
            int row,
            int column,
            int tabIndex)
        {
            var collectionView = new CollectionView
            {
                BackgroundColor = ourviewmodel.orangeColour,
                SelectionMode = Microsoft.Maui.Controls.SelectionMode.None
            };

            // Choose ItemsSource + FontSize
            double fontSize = 12;
            //object itemsSource = null;

            switch (name)
            {
                case "Paidin":
                    fontSize = financeviewmodel.PaidInFontSize;
                    //itemsSource = financeviewmodel.FinanceTotals[tabIndex, 0];
                    collectionView.ItemsSource = financeviewmodel.FinanceTotals[tabIndex, 0];
                    break;

                case "Paidout":
                    fontSize = financeviewmodel.PaidOutFontSize;
                    //itemsSource = financeviewmodel.FinanceTotals[tabIndex, 1];
                    collectionView.ItemsSource = financeviewmodel.FinanceTotals[tabIndex, 1];
                    break;

                case "Difference":
                    fontSize = financeviewmodel.DifferenceFontSize;
                    //itemsSource = financeviewmodel.FinanceTotals[tabIndex, 2];
                    collectionView.ItemsSource = financeviewmodel.FinanceTotals[tabIndex, 2];
                    break;
            }

            //collectionView.ItemsSource = itemsSource as ;

            // Template replaces DataGrid columns
            collectionView.ItemTemplate = new DataTemplate(() =>
            {
                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) }
                    }
                };

                var description = new Label
                {
                    FontSize = fontSize,
                    VerticalOptions = LayoutOptions.Center
                };
                description.SetBinding(Label.TextProperty, "Description");

                var amount = new Label
                {
                    FontSize = fontSize,
                    HorizontalTextAlignment = TextAlignment.End,
                    VerticalOptions = LayoutOptions.Center
                };
                amount.SetBinding(Label.TextProperty, "Amount");

                grid.Add(description, 0, 0);
                grid.Add(amount, 1, 0);

                return grid;
            });

            Grid.SetRow(collectionView, row);
            Grid.SetColumn(collectionView, column);

            return collectionView;
        }

#endif

#if WPF  || WINUI
        internal static DataGridTemplateColumn CreateColumn(MainViewModel ourviewmodel,
                                                            FinanceViewModel financeviewmodel,
                                                            string bindingName,
                                                            double columnWidth,
                                                            bool align)     // True = Left, false = Right
        {
            // Create a new DataGridTemplateColumn
            DataGridTemplateColumn myColumn = new DataGridTemplateColumn
            {
                Header = "",
                Width = new DataGridLength(columnWidth, DataGridLengthUnitType.Star)
            };

#if WPF
            // Fuck me, this worked almost first time!!!
            // We have to do it this way, because this absolute bollocks
            // system doesn't let us code-up a  DataTemplate any other way
            // This ENTIRE IDE is a COMPLETE PILE OF SHIT
            string alignment = align == true ? "Left" : "Right";
            string text = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <TextBlock HorizontalAlignment=""parameter1"" Text=""{Binding parameter2}""/>
            </DataTemplate>".Replace("parameter1", alignment).Replace("parameter2", bindingName);
            // You couldn't make this bollocks up, you really couldn't ...
            StringReader stringReader = new StringReader(text);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            DataTemplate template = System.Windows.Markup.XamlReader.Load(xmlReader) as DataTemplate;
#endif
#if WINUI
            // Fuck me, this worked almost first time!!!
            // We have to do it this way, because this absolute bollocks
            // system doesn't let us code-up a  DataTemplate any other way
            // This ENTIRE IDE is a COMPLETE PILE OF SHIT
            string alignment = align == true ? "Left" : "Right";
            string text = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            <TextBlock HorizontalAlignment=""parameter1"" Text=""{Binding parameter2}""/>
            </DataTemplate>".Replace("parameter1", alignment).Replace("parameter2", bindingName);
            // You couldn't make this bollocks up, you really couldn't ...
            DataTemplate template = XamlReader.Load(new StringReader(text).ReadToEnd()) as DataTemplate;
#endif
            // CANT REMEMBER WHAT I DID THIS FOR!!!
            // Fuck me, this worked almost first time!!!
            // We have to do it this way, because this absolute bollocks
            // system doesn't let us code-up a  DataTemplate any other way
            // This ENTIRE IDE is a COMPLETE PILE OF SHIT
            //string alignment = align == true ? "Left" : "Right";
            //string text = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
            //<TextBlock HorizontalAlignment=""parameter1"" Text=""{Binding parameter2}""/>
            //</DataTemplate>".Replace("parameter1", alignment).Replace("parameter2", bindingName);
            //// You couldn't make this bollocks up, you really couldn't ...
            //StringReader stringReader = new StringReader(text);
            //XmlReader xmlReader = XmlReader.Create(stringReader);
            //DataTemplate template = System.Windows.Markup.XamlReader.Load(xmlReader) as DataTemplate;

            myColumn.CellTemplate = template;

            // And set the CellStyle
            myColumn.CellStyle = financeviewmodel.CellStyle;
            return myColumn;
        }
#endif

        internal static async void FinanceTransactionsComplete(
#if WINFORMS
                                                MainProcess process_components,
                                                CheckedComboBox providers_checked_combobox,
                                                CheckedComboBox accounts_checked_combobox,
                                                CheckedComboBox transactiontypes_checked_combobox,
                                                DataGridView TransactionsDataGrid,
#endif
                                                MainViewModel ourviewmodel,
                                                FinanceViewModel financeviewmodel,
                                                //char categoryCode,
#if WINFORMS || WPF || SMARTMAUI
                                                DateTime startDate,
                                                DateTime endDate
#endif
#if WINUI
                                                DateTimeOffset startDate,
                                                DateTimeOffset endDate
#endif
#if ANDROIDX
                                                AppCompatActivity meterActivity,
                                                DateTime startDate,
                                                DateTime endDate
#endif
                                                )
        {

#if WINFORMS
            providers_checked_combobox.Items.Clear();
            accounts_checked_combobox.Items.Clear();
            // And transactions eventually!
            transactiontypes_checked_combobox.Items.Clear();
#endif
            // You have to do this bollocks this way, otherwise FinanceProvidersList
            // DOESN'T get updated ... what a pile of shit this garbage is ...

            List<FinanceViewModel.ProviderItem> temp_providers = new List<FinanceViewModel.ProviderItem>();
            List<FinanceViewModel.AccountItem> temp_accounts = new List<FinanceViewModel.AccountItem>();
            financeviewmodel.temp_groups = new List<FinanceViewModel.TransactionGroupItem>();

            // You could condense this all down into ONE 'bank_providers_found' but its a lot
            // clearer to leave it like this after all the pain ...

            // Get all the TRANSACTIONS first
            financeviewmodel.transactionsViewFound =
                new List<SmartFinance.TransactionsView>(
                    from Accounts in financeviewmodel.accountsFound // These have been reduced by CATEGORY_CODE?
                    join Transactions in financeviewmodel.PLO.transactionsList
                    on new { Accounts.SORTCODE, Accounts.ACCOUNT_NO, Accounts.UDPRN }
                    equals new { Transactions.SORTCODE, Transactions.ACCOUNT_NO, Transactions.UDPRN }
                    select new SmartFinance.TransactionsView
                    {
                        USERNAME = Transactions.USERNAME,                            // KEY
                        CUBEFACE_CODE = Transactions.CUBEFACE_CODE,
                        INSTITUTION_CODE = Transactions.INSTITUTION_CODE,
                        BRAND_CODE = Transactions.BRAND_CODE,
                        SORTCODE = Transactions.SORTCODE,
                        ACCOUNT_NO = Transactions.ACCOUNT_NO,
                        ACCOUNT_NAME = Transactions.ACCOUNT_NO,
                        UDPRN = Transactions.UDPRN,
                        STATEMENT_DATE = Transactions.STATEMENT_DATE,
                        STATEMENT_NO = Transactions.STATEMENT_NO
                    });

            //if (financeviewmodel.PLO.crypto_accountsList.Count > 0)
            //{
            //    financeviewmodel.transactionsViewFound =
            //    new List<SmartFinance.TransactionsView>(
            //        from Accounts in financeviewmodel.PLO.crypto_accountsList // These have been reduce by CATEGORY_CODE?
            //        join Transactions in financeviewmodel.PLO.transactionsList
            //        on new { Accounts.SORTCODE, Accounts.ACCOUNT_NO, Accounts.UDPRN }
            //        equals new { Transactions.SORTCODE, Transactions.ACCOUNT_NO, Transactions.UDPRN }

            //        select new SmartFinance.TransactionsView
            //        {
            //            USERNAME = Transactions.USERNAME,                            // KEY
            //            CUBEFACE_CODE = Transactions.CUBEFACE_CODE,
            //            INSTITUTION_CODE = Transactions.INSTITUTION_CODE,
            //            BRAND_CODE = Transactions.BRAND_CODE,
            //            SORTCODE = Transactions.SORTCODE,
            //            ACCOUNT_NO = Transactions.ACCOUNT_NO,
            //            ACCOUNT_NAME = Accounts.ACCOUNT_NO == Transactions.ACCOUNT_NO ? Accounts.NAME : Accounts.ACCOUNT_NO,
            //            UDPRN = Transactions.UDPRN,
            //            //CURRENCY_ORDINAL = Transactions.CURRENCY_ORDINAL,
            //            STATEMENT_DATE = Transactions.STATEMENT_DATE,
            //            STATEMENT_NO = Transactions.STATEMENT_NO
            //        });

            //}

            // Reduce em down the Accounts by Category - Bank, Savings or Investments
            List<SmartFinance.TransactionsCategories> transactionscategories_found =
                new List<SmartFinance.TransactionsCategories>(
                    from Transactions in financeviewmodel.transactionsViewFound
                    join TransactionsCategories in financeviewmodel.PLO.transactionscategoriesList
                    on new { Transactions.INSTITUTION_CODE, Transactions.BRAND_CODE, Transactions.SORTCODE, Transactions.ACCOUNT_NO, Transactions.UDPRN, Transactions.STATEMENT_DATE, Transactions.STATEMENT_NO }
                    equals new { TransactionsCategories.INSTITUTION_CODE, TransactionsCategories.BRAND_CODE, TransactionsCategories.SORTCODE, TransactionsCategories.ACCOUNT_NO, TransactionsCategories.UDPRN, TransactionsCategories.STATEMENT_DATE, TransactionsCategories.STATEMENT_NO }
                    select TransactionsCategories);


            // Find all unique Accounts
            List<SmartFinance.TransactionsView> transactions_accounts_found =
                new List<SmartFinance.TransactionsView>(financeviewmodel.transactionsViewFound.DistinctBy(key => new
                {
                    key.USERNAME,
                    key.CUBEFACE_CODE,
                    key.INSTITUTION_CODE,
                    key.BRAND_CODE,
                    key.SORTCODE,
                    key.ACCOUNT_NO,
                    key.UDPRN //<= NO! because you want the Account Nos to be UNIQUE
                    // key.STATEMENT_DATE,  Ignore these two
                    // key.STATEMENT_NO,    We are not intested in Statements here

                }));

            // Reduce down the Transactions by Category - Bank, Savings or Investments
            //financeviewmodel.transgroups_found =
            //    new List<SmartFinance.TransactionsCategories>(financeviewmodel.PLO.transactionscategoriesList.DistinctBy(key => new
            //    {
            //        key.USERNAME,
            //        key.CUBEFACE_CODE,
            //        key.INSTITUTION_CODE,
            //        key.BRAND_CODE,
            //        //key.STATEMENT_DATE,   Ignore these two
            //        //key.STATEMENT_NO,     We are not intested in statements here
            //        //key.SORTCODE,
            //        //key.ACCOUNT_NO,
            //        //key.UDPRN,
            //        key.TRANSGROUP_CODE     // Have to use TransCategories because of this
            //    }));

            //// Reduce down the Transactions by Category - Bank, Savings or Investments
            //financeviewmodel.transCodes_found =
            //	new List<SmartFinance.TransactionsCategories>(financeviewmodel.PLO.transactionscategoriesList.DistinctBy(key => new
            //	{
            //		key.USERNAME,
            //		key.CUBEFACE_CODE,
            //		key.INSTITUTION_CODE,
            //		key.BRAND_CODE,
            //		//key.STATEMENT_DATE,   Ignore these two
            //		//key.STATEMENT_NO,     We are not intested in statements here
            //		//key.SORTCODE,
            //		//key.ACCOUNT_NO,
            //		//key.UDPRN,
            //		key.TRANSACTION_CODE    // Have to use TransCategories because of this
            //	}));

            // Reduce down the Providers by Category - Bank, Savings or Investments
            List<SmartFinance.TransactionsView> providers_found = new List<SmartFinance.TransactionsView>(transactions_accounts_found.DistinctBy(key => new
            {
                key.USERNAME,
                key.CUBEFACE_CODE,
                key.INSTITUTION_CODE,
                key.BRAND_CODE
            }));


            // Before you do this next step - you have to build a list of Providers, Accounts
            // and Transactions which have been explicity or implicitly checked (because
            // they are new).  Don't include any that have been explicity UNCHECKED.

            int provider_id = 1;
            int account_id = 1;
            int transaction_id = 1;
            financeviewmodel.selectedProvidersList.Clear();
            financeviewmodel.selectedAccountsList.Clear();
            // Look, TransactionTypes is different from the other two
            // a) because it is set up directly i.e. we don't need elements
            //    from Providers or Accounts and
            // b) it doesn't need an intermediary like "selectedTransactiontypesList"
#if ANDROIDX
            // Initialize first setup for the dripdown <= many a true word spoken in jest, Ray
            List<string> itemsProviders = new();
            string initialProviders = "";
            List<string> itemsAccounts = new();
            string initialAccounts = "";
            List<string> itemsTransactionGroups = new();
            string initialTransactionGroups = "";
#endif
            foreach (SmartFinance.TransactionsView provider_item in providers_found)
            {
                short institution_code = provider_item.INSTITUTION_CODE;
                short brand_code = provider_item.BRAND_CODE;
                List<SmartFinance.Brands> brands_found =
                    SmartSpikeFinanceV2017.Finance_Lookup_BrandName(ourviewmodel,
                                            financeviewmodel,
                                            //SmartParametersV2016.activeFlag,
                                            "",
                                            institution_code,
                                            brand_code);

                if (brands_found.Count > 0)
                {
                    char twoFactor = SmartParametersV2016.defaultChar;
                    twoFactor = brands_found.First().TWOFACTOR_FLAG;
                    //foreach (SmartFinance.Institutions insti in financeviewmodel.PLO.institutionsList)
                    //{
                    //    if (insti.INSTITUTION_CODE == provider_item.INSTITUTION_CODE)
                    //    {
                    //        twoFactor = insti.TWOFACTOR;
                    //        break;
                    //    }
                    //}
                    FinanceViewModel.ProviderItem pitem = new FinanceViewModel.ProviderItem()
                    {
                        ProviderID = provider_id,
                        Content = brands_found.First().BRAND_NAME,
                        Value = Build_Tag_Provider(provider_item.INSTITUTION_CODE, provider_item.BRAND_CODE, twoFactor),
                        IsChecked = true,
                        Colour = ourviewmodel.blackColour
                    };

#if WINFORMS
                    providers_checked_combobox.Items.Add(pitem);
                    providers_checked_combobox.SetItemChecked(provider_id - 1, true);
#endif
                    temp_providers.Add(pitem);

                    provider_id++;
                    
                    SmartFinance.SelectedProviders sitem = new SmartFinance.SelectedProviders()
                    {
                        USERNAME = provider_item.USERNAME,
                        CUBEFACE_CODE = provider_item.CUBEFACE_CODE,
                        INSTITUTION_CODE = provider_item.INSTITUTION_CODE,
                        BRAND_CODE = provider_item.BRAND_CODE
                    };
                    financeviewmodel.selectedProvidersList.Add(sitem);
#if ANDROIDX
                    itemsProviders.Add(pitem.Content);
                    if (initialProviders.Length > 0)
                    {
                        initialProviders += SmartParametersV2016.comma;
                    }
                    initialProviders += pitem.Content;
#endif
                    List<string> xxx = new List<string>();

                    foreach (SmartFinance.TransactionsView transaction_item in transactions_accounts_found)
                    {
                        if (transaction_item.INSTITUTION_CODE == provider_item.INSTITUTION_CODE &&
                            transaction_item.BRAND_CODE == provider_item.BRAND_CODE)
                        {
                            bool found = false;
                            foreach (string zzz in xxx)
                            {
                                if (zzz == transaction_item.ACCOUNT_NAME)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            short currOrd = 0;
                            if (!found)
                            {
                                xxx.Add(transaction_item.ACCOUNT_NAME);
                                // Build the account dropdown item
                                // Lookup the Account details!!!
                                currOrd = SmartSpikeFinanceV2017.LookupCurrencyOrdinal(ourviewmodel,
                                                financeviewmodel,
                                                transaction_item.SORTCODE,
                                                transaction_item.ACCOUNT_NO);

                                FinanceViewModel.AccountItem aitem = new FinanceViewModel.AccountItem()
                                {
                                    AccountID = account_id,
                                    Content = pitem.Content + " - " + transaction_item.ACCOUNT_NAME,
                                    Value = Build_AccountTag_Extra(transaction_item.INSTITUTION_CODE,
                                                                            transaction_item.BRAND_CODE,
                                                                            transaction_item.SORTCODE,
                                                                            transaction_item.ACCOUNT_NO,
                                                                            transaction_item.UDPRN,
                                                                            currOrd),
                                    IsChecked = true,
                                    Colour = ourviewmodel.blackColour
                                };
#if WINFORMS
                                accounts_checked_combobox.Items.Add(aitem);
                                accounts_checked_combobox.SetItemChecked(account_id - 1, true);
#endif
                                temp_accounts.Add(aitem);
#if ANDROIDX
                                if (initialAccounts.Length > 0)
                                {
                                    initialAccounts += SmartParametersV2016.comma;
                                }
                                initialAccounts += aitem.Content;
                                itemsAccounts.Add(aitem.Content);   // <= Not sure about this?
#endif
                                account_id++;
                            }
                            SmartFinance.SelectedAccounts bitem = new SmartFinance.SelectedAccounts()
                            {
                                USERNAME = transaction_item.USERNAME,
                                CUBEFACE_CODE = transaction_item.CUBEFACE_CODE,
                                INSTITUTION_CODE = transaction_item.INSTITUTION_CODE,
                                BRAND_CODE = transaction_item.BRAND_CODE,
                                SORTCODE = transaction_item.SORTCODE,
                                ACCOUNT_NO = transaction_item.ACCOUNT_NO,
                                UDPRN = transaction_item.UDPRN,
                                CURRENCY_ORDINAL = currOrd
                            };
                            financeviewmodel.selectedAccountsList.Add(bitem);
                        }
                    }

                    foreach (SmartFinance.Transaction_Groups transactionGroup in financeviewmodel.transaction_groupsfound)
                    {
                        FinanceViewModel.TransactionGroupItem tgitem = new FinanceViewModel.TransactionGroupItem()
                        {
                            TransactionID = transaction_id,
                            Content = transactionGroup.AMOUNT_TYPE,
                            CREDITDEBIT_INDICATOR = transactionGroup.CREDITDEBIT_INDICATOR,
                            IsChecked = true,
                            Colour = ourviewmodel.blackColour
                        };
                        financeviewmodel.temp_groups.Add(tgitem);
#if WINFORMS
                        transactiontypes_checked_combobox.Items.Add(tgitem);
                        transactiontypes_checked_combobox.SetItemChecked(transaction_id - 1, true);
#endif
#if ANDROIDX
                        itemsTransactionGroups.Add(tgitem.Content);
                        if (initialTransactionGroups.Length > 0)
                        {
                            initialTransactionGroups += SmartParametersV2016.comma;
                        }
                        initialTransactionGroups += tgitem.Content;
#endif
                        transaction_id++;
                    }
                }
            }
            // You have to do this bollocks this way, otherwise FinanceProvidersList
            // DOESN'T get updated ... what a pile of shit this garbage is ...
            financeviewmodel.FinanceProvidersList = new List<FinanceViewModel.ProviderItem>();
            financeviewmodel.FinanceProvidersList = temp_providers;
#if ANDROIDX
            financeviewmodel.FinanceProviders.SetItems(itemsProviders, initialProviders, null, 'P');
#endif
            financeviewmodel.FinanceAccountsList = new List<FinanceViewModel.AccountItem>();
            financeviewmodel.FinanceAccountsList = temp_accounts;
#if ANDROIDX
            financeviewmodel.FinanceAccounts.SetItems(itemsAccounts, initialAccounts, null, 'A');
#endif
            financeviewmodel.FinanceTransactionGroupsList = new List<FinanceViewModel.TransactionGroupItem>();
            financeviewmodel.FinanceTransactionGroupsList = financeviewmodel.temp_groups;
#if ANDROIDX
            financeviewmodel.FinanceTransactionGroups.SetItems(itemsTransactionGroups, initialTransactionGroups, null, 'T');
#endif
#if WPF || SMARTMAUI
            FinanceProviders_UpdateText(financeviewmodel);
            FinanceAccounts_UpdateText(financeviewmodel);
            FinanceTransactionGroups_UpdateText(financeviewmodel);
#endif
#if WINUI
            FinanceProviders_UpdateText(financeviewmodel);
            FinanceAccounts_UpdateText(financeviewmodel);
            FinanceTransactionGroups_UpdateText(financeviewmodel);
#endif
            
            // Now, cant we find all the logos we are going to need by
            // trawling through the ACCOUNTS and looking for Logos we haven't got?
            // And downloading them? Then won't we have everything we are going
            // to need??

            foreach (SmartFinance.SelectedAccounts accItem in financeviewmodel.selectedAccountsList)
            {
                // Here Ray!
#if WINFORMS || WPF  || WINUI || ANDROIDX || SMARTMAUI
                string brand_name = "";
                List<SmartFinance.Brands> brands_found = SmartSpikeFinanceV2017.Finance_Find_Brands(ourviewmodel,
                                                                                            financeviewmodel,
                                                                                            financeviewmodel.accountsFound.First().INSTITUTION_CODE,
                                                                                            financeviewmodel.accountsFound.First().BRAND_CODE);
                if (brands_found.Count > 0)
                {
                    brand_name = brands_found.First().BRAND_NAME;
                    financeviewmodel.PictureBoxLOGO = Lookup_LOGO(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        SmartParametersV2016.Finance,
                                                        brand_name);
                    if (financeviewmodel.PictureBoxLOGO == null)
                    {
                        financeviewmodel.PictureBoxLOGO = await Download_LOGOAsync(
#if ANDROIDX
                                                        meterActivity,
#endif
                                                        ourviewmodel,
                                                        financeviewmodel,
                                                        SmartParametersV2016.Finance,
                                                        brand_name);
                        // It may still be blank here ...! But it shouldn't be!!
                    }
                }
#endif
            }

            Finance_SubsetTransactions(
#if WINFORMS
                                            process_components,
                                            providers_checked_combobox,
                                            accounts_checked_combobox,
                                            transactiontypes_checked_combobox,
                                            TransactionsDataGrid,
#endif
                                            ourviewmodel,
                                            financeviewmodel,
                                            "",
                                            false,          // DO/DON'T respect Addresses
                                            startDate,
                                            endDate);
            return;
        }
        
        internal static void Finance_SubsetTransactions(
#if WINFORMS
                                                    MainProcess components,
                                                    CheckedComboBox providers_checked_combobox,
                                                    CheckedComboBox accounts_checked_combobox,
                                                    CheckedComboBox transactiontypes_checked_combobox,
                                                    DataGridView TransactionsDataGrid,
#endif
                                                    MainViewModel ourviewmodel,
                                                    FinanceViewModel financeviewmodel,
                                                    string udprn,
                                                    bool respect_addresses,
#if WINFORMS || WPF || SMARTMAUI
                                                    DateTime startDate,
                                                    DateTime endDate
#endif
#if WINUI
                                                    DateTimeOffset startDate,
                                                    DateTimeOffset endDate
#endif
#if ANDROIDX
                                                    DateTime startDate,
                                                    DateTime endDate
#endif
                                                    )
        {
            financeviewmodel.QuantityWidth = financeviewmodel.CryptosChecked ? 70 : 0;

            ClearDownTransactions(
#if WINFORMS
                                    components,
#endif
                                    ourviewmodel, financeviewmodel);
            // ALWAYS RESPECT DATES!!

            // You have to do this bollocks this way, otherwise FinanceProvidersList
            // DOESN'T get updated ... what a pile of shit this garbage is ...

            // You could condense this all down into ONE 'bank_providers_found' but its a lot
            // clearer to leave it like this after all the pain ...
            List<SmartFinance.TransactionsCategories> finance_transactions_found = financeviewmodel.PLO.transactionscategoriesList;
            if (respect_addresses)
            {
                finance_transactions_found = SmartSpikeFinanceV2017.InitialTransactionsAddresses(financeviewmodel,
                                                                                                finance_transactions_found,
                                                                                                udprn);
            }
            finance_transactions_found = SmartSpikeFinanceV2017.InitialTransactionsDates(financeviewmodel,
                                                                                        finance_transactions_found,
                                                                                        startDate,
                                                                                        endDate);



            finance_transactions_found = SmartSpikeFinanceV2017.InitialTransactionsAccounts(financeviewmodel,
                                                                                    finance_transactions_found);

            finance_transactions_found = SmartSpikeFinanceV2017.InitialTransactionsGroups(financeviewmodel,
                                                                                    finance_transactions_found);

            if (financeviewmodel.PLO.crypto_ledgersList.Count > 0)
            {
                financeviewmodel.PLO.crypto_ledgersList = new List<SmartFinance.CryptoLedgers>
                (from Transactions in financeviewmodel.PLO.crypto_ledgersList
                 orderby Transactions.TRANSACTION_DATE ascending
                 select Transactions).ToList();
                // All because you can't clone crypto_ledgersList
                UpdateCryptoLedgers(financeviewmodel);
            }

            if (financeviewmodel.PLO.crypto_wallettotalsList.Count > 0)
            {
                List<SmartFinance.CryptoWalletTotals> tempTotals = new List<SmartFinance.CryptoWalletTotals>
                (from Totals in financeviewmodel.PLO.crypto_wallettotalsList
                 select Totals).ToList();
                financeviewmodel.FinanceWalletTotals = tempTotals;
            }

            //
            // RE-DO THIS BADLY RAY!!
            //
            financeviewmodel.FinanceCryptoTransactions =
            new List<SmartFinance.CryptoTransactionsView>();
            //from Provider in financeviewmodel.selectedProvidersList
            //join Account in financeviewmodel.selectedAccountsList
            //on new { Provider.INSTITUTION_CODE, Provider.BRAND_CODE }
            //equals new { Account.INSTITUTION_CODE, Account.BRAND_CODE }
            //join generic in financeviewmodel.PLO.transactionsList // was crypto_
            //on new { Account.INSTITUTION_CODE, Account.BRAND_CODE, Account.SORTCODE, Account.ACCOUNT_NO }
            //equals new { generic.INSTITUTION_CODE, generic.BRAND_CODE, generic.SORTCODE, generic.ACCOUNT_NO }//SORTCODE = generic.EXCHANGE, ACCOUNT_NO = generic.UUID }
            //select generic);

            financeviewmodel.FinanceCryptoAccounts =
            new List<SmartFinance.CryptoAccountsView>
            (
                from Account in financeviewmodel.selectedAccountsList
                join generic in financeviewmodel.PLO.crypto_accountsList
                on new { Account.INSTITUTION_CODE, Account.BRAND_CODE, Account.SORTCODE, Account.ACCOUNT_NO }
                equals new { generic.INSTITUTION_CODE, generic.BRAND_CODE, generic.SORTCODE, generic.ACCOUNT_NO }
                select new SmartFinance.CryptoAccountsView
                {
                    USERNAME = generic.USERNAME,                            // KEY
                    EXCHANGE = generic.SORTCODE,
                    UUID = generic.ACCOUNT_NO,
                    // = generic.ACTIVE,
                    UDPRN = generic.UDPRN,
                    AVAILABLE_BALANCE_CURRENCY = generic.AVAILABLE_BALANCE_CURRENCY,
                    AVAILABLE_BALANCE_VALUE = generic.AVAILABLE_BALANCE_VALUE,
                    CREATED_AT = generic.CREATED_AT,
                    CURRENCY = "GBP", //generic.CURRENCY,   // Sort this Ray
                    DEFAULT = generic.DEFAULT,
                    DELETED_AT = generic.DELETED_AT,
                    HOLD_CURRENCY = generic.HOLD_CURRENCY,
                    HOLD_VALUE = generic.HOLD_VALUE,
                    NAME = generic.NAME,
                    //PLATFORM = generic.PLATFORM,
                    READY = generic.READY,
                    RETAIL_PORTFOLIO_ID = generic.RETAIL_PORTFOLIO_ID,
                    TYPE = generic.TYPE,
                    UPDATED_AT = generic.UPDATED_AT

                });

            List<SmartFinance.CryptoAddressesView> tempAddresses =
                new List<SmartFinance.CryptoAddressesView>
                (
                    from Account in financeviewmodel.selectedAccountsList
                    join Address in financeviewmodel.PLO.crypto_addressesList
                    on new { Account.INSTITUTION_CODE, Account.BRAND_CODE, Account.SORTCODE, Account.ACCOUNT_NO }
                    equals new { Address.INSTITUTION_CODE, Address.BRAND_CODE, Address.SORTCODE, Address.ACCOUNT_NO }
                    select new SmartFinance.CryptoAddressesView
                    {
                        USERNAME = Address.USERNAME,
                        EXCHANGE = Address.SORTCODE,
                        UUID = Address.ACCOUNT_NO,
                        ADDRESS = Address.ADDRESS,
                        ADDRESS_INFO_ADDRESS = Address.ADDRESS_INFO_ADDRESS,
                        ADDRESS_LABEL = Address.ADDRESS_LABEL,
                        CALLBACK_URL = Address.CALLBACK_URL,
                        CREATED_AT = Address.CREATED_AT,
                        DEFAULT_RECEIVE = Address.DEFAULT_RECEIVE,
                        DEPOSIT_URI = Address.DEPOSIT_URI,
                        DESTINATION_TAG = Address.DESTINATION_TAG,
                        ID = Address.ID,
                        INLINE_WARNING_TEXT = Address.INLINE_WARNING_TEXT,
                        INLINE_WARNING_TOOLTIP = Address.INLINE_WARNING_TOOLTIP,
                        NAME = Address.NAME,
                        NETWORK = Address.NETWORK,
                        QR_CODE_IMAGE_URL = Address.QR_CODE_IMAGE_URL,
                        RECEIVE_SUBTITLE = Address.RECEIVE_SUBTITLE,
                        RESOURCE = Address.RESOURCE,
                        RESOURCE_PATH = Address.RESOURCE_PATH,
                        SHARE_ADDRESS_COPY_LINE1 = Address.SHARE_ADDRESS_COPY_LINE1,
                        SHARE_ADDRESS_COPY_LINE2 = Address.SHARE_ADDRESS_COPY_LINE2,
                        UPDATED_AT = Address.UPDATED_AT,
                        URI_SCHEME = Address.URI_SCHEME
                    });

            financeviewmodel.FinanceCryptoAddresses = tempAddresses;

            List<SmartCrypto.V2CryptoTotals> tempCryptoTotals = new List<SmartCrypto.V2CryptoTotals>();

            //foreach (SmartFinance.CryptoTransactionsView cryptoView in financeviewmodel.FinanceCryptoTransactions)
            //{
            //    List<SmartFinance.CryptoCurrencyRates> currencyRate = new List<SmartFinance.CryptoCurrencyRates>(
            //        from Currency in financeviewmodel.PLO.crypto_currenciesList
            //        where Currency.CRYPTO_ORDINAL == cryptoView.CRYPTO_CURRENCY_ORDINAL
            //        select Currency);
            //    if (currencyRate.Count > 0)
            //    {
            //        cryptoView.CRYPTO_RATE = Convert.ToDouble(currencyRate.First().CRYPTO_RATE);

//        //cryptoView.CRYPTO_RATE = (cryptoView.CRYPTO_AMOUNT / cryptoView.NATIVE_AMOUNT).ToString("F3");
//        cryptoView.TOTAL_COST = (cryptoView.NATIVE_AMOUNT + cryptoView.FEE_AMOUNT).ToString("F2");
//        bool found_it = false;
//        foreach (SmartCrypto.V2CryptoTotals totalsX in tempCryptoTotals)
//        {
//            if (totalsX.CRYPTO_CURRENCY_ORDINAL == cryptoView.CRYPTO_CURRENCY_ORDINAL)
//            {
//                totalsX.CRYPTO_AMOUNT += cryptoView.CRYPTO_AMOUNT;
//                totalsX.VALUE += cryptoView.NATIVE_AMOUNT + cryptoView.FEE_AMOUNT;
//                totalsX.RATE = cryptoView.CRYPTO_RATE;
//                totalsX.CRYPTO_CURRENCY = cryptoView.CRYPTO_CURRENCY;
//                totalsX.CRYPTO_CURRENCY_ORDINAL = cryptoView.CRYPTO_CURRENCY_ORDINAL;
//                found_it = true;
//                break;
//            }
//        }
//        if (!found_it)
//        {
//            SmartCrypto.V2CryptoTotals totalsY = new SmartCrypto.V2CryptoTotals()
//            {
//                CRYPTO_AMOUNT = cryptoView.CRYPTO_AMOUNT,
//                VALUE = cryptoView.NATIVE_AMOUNT + cryptoView.FEE_AMOUNT,
//                RATE = cryptoView.CRYPTO_RATE,
//                CRYPTO_CURRENCY = cryptoView.CRYPTO_CURRENCY,
//                CRYPTO_CURRENCY_ORDINAL = cryptoView.CRYPTO_CURRENCY_ORDINAL
//            };
//            tempCryptoTotals.Add(totalsY);
//        }
//    }
//}
#if CRYPTOS
            tempCryptoTotals =
                        new List<SmartCrypto.V2CryptoTotals>
                        (from Currencies in tempCryptoTotals
                         orderby Currencies.CRYPTO_CURRENCY
                         select Currencies);
            financeviewmodel.FinanceCryptoCurrencyRates = tempCryptoTotals;
#endif

            //// Reduce down the Transactions Codes
            //List< SmartFinance.TransactionsCategories> transactioncodes_found =
            //    new List<SmartFinance.TransactionsCategories>(finance_transactions_found.DistinctBy(key => new
            //    {
            //        key.USERNAME,
            //        key.CUBEFACE_CODE,
            //        key.INSTITUTION_CODE,
            //        key.BRAND_CODE,
            //        //key.SORTCODE,
            //        //key.ACCOUNT_NO,
            //        //key.STATEMENT_DATE,
            //        //key.STATEMENT_NO,
            //        key.TRANSACTION_CODE
            //    }));

            // Before you do this next step - you have to build a list of
            // Providers, Accounts and Transactions which have been explicity
            // or implicitly checked (because they are new).
            // Don't include any that have been explicity UNCHECKED.

            // RAY!!  Here you are relying on selectedAccountsList
            // to contain the SortCodes and AccountNos which related
            // to categories 'B' and 'S'
            //
            // ** They do. At this point I can trust them! **
            // (If selectedAccountsList is WRONG then the next select
            // will be WRONG as well!!!!)
            if (finance_transactions_found.Count > 0)
            //if (transactioncodes_found.Count > 0)
            {
                finance_transactions_found =
                    new List<SmartFinance.TransactionsCategories>
                    (from Provider in financeviewmodel.selectedProvidersList
                     join Account in financeviewmodel.selectedAccountsList
                     on new { Provider.INSTITUTION_CODE, Provider.BRAND_CODE }
                     equals new { Account.INSTITUTION_CODE, Account.BRAND_CODE }
                     join Transactions in financeviewmodel.transactionsViewFound
                     on new { Account.INSTITUTION_CODE, Account.BRAND_CODE, Account.SORTCODE, Account.ACCOUNT_NO, Account.UDPRN }
                     equals new { Transactions.INSTITUTION_CODE, Transactions.BRAND_CODE, Transactions.SORTCODE, Transactions.ACCOUNT_NO, Transactions.UDPRN }
                     join TransactionsCategories in finance_transactions_found
                     on new { Transactions.INSTITUTION_CODE, Transactions.BRAND_CODE, Transactions.STATEMENT_DATE, Transactions.STATEMENT_NO, Transactions.SORTCODE, Transactions.ACCOUNT_NO, Transactions.UDPRN }
                     equals new { TransactionsCategories.INSTITUTION_CODE, TransactionsCategories.BRAND_CODE, TransactionsCategories.STATEMENT_DATE, TransactionsCategories.STATEMENT_NO, TransactionsCategories.SORTCODE, TransactionsCategories.ACCOUNT_NO, TransactionsCategories.UDPRN }
                     // This isn't going to work because some of the records in Transactions_Flows
                     // are neither Credit nor Debit
                     //join TransactionFlow in financeviewmodel.PLO.transaction_flowsList
                     //on new { TransactionsCategories.CREDITDEBIT_INDICATOR }
                     //equals new { TransactionFlow.CREDITDEBIT_INDICATOR }
                     //orderby BankTransaction.TRANSACTION_DATE descending, 
                     //        BankTransaction.SEQUENCE_NO descending // Same thing as Login.CREATED desc as we have matched on this field ..
                     //select TransactionsCategories);// Categories);                   // Can use 'new' because we specify the fields below
                     select new SmartFinance.TransactionsCategories
                     {
                         USERNAME = TransactionsCategories.USERNAME,                            // KEY
                         CUBEFACE_CODE = TransactionsCategories.CUBEFACE_CODE,
                         INSTITUTION_CODE = TransactionsCategories.INSTITUTION_CODE,
                         BRAND_CODE = TransactionsCategories.BRAND_CODE,
                         SORTCODE = TransactionsCategories.SORTCODE,
                         ACCOUNT_NO = Transactions.ACCOUNT_NAME,
                         UDPRN = TransactionsCategories.UDPRN,
                         //CURRENCY_ORDINAL = TransactionsCategories.CURRENCY_ORDINAL,
                         STATEMENT_DATE = TransactionsCategories.STATEMENT_DATE,
                         STATEMENT_NO = TransactionsCategories.STATEMENT_NO,
                         SEQUENCE_NO = TransactionsCategories.SEQUENCE_NO,
                         RANDOMKEY1 = TransactionsCategories.RANDOMKEY1,
                         TRANSACTION_DATE = TransactionsCategories.TRANSACTION_DATE,
                         CREDITDEBIT_INDICATOR = TransactionsCategories.CREDITDEBIT_INDICATOR,
                         PTC_CODE = TransactionsCategories.PTC_CODE,
                         TRANSGROUP_CODE = TransactionsCategories.TRANSGROUP_CODE,
                         TRANSACTION_CODE = TransactionsCategories.TRANSACTION_CODE,
                         DESCRIPTION = TransactionsCategories.DESCRIPTION,
                         TYPE = TransactionsCategories.TYPE,                                     // Crypto
                         CRYPTO_AMOUNT = TransactionsCategories.CRYPTO_AMOUNT,
                         CRYPTO_CURRENCY_ORDINAL = TransactionsCategories.CRYPTO_CURRENCY_ORDINAL,
                         AMOUNT = TransactionsCategories.AMOUNT,
                         AMOUNT_CURRENCY_ORDINAL = TransactionsCategories.AMOUNT_CURRENCY_ORDINAL,
                         BALANCE_AMOUNT = TransactionsCategories.BALANCE_AMOUNT,
                         BALANCE_CURRENCY_ORDINAL = TransactionsCategories.BALANCE_CURRENCY_ORDINAL,
                         BALANCE_CREDITDEBIT_INDICATOR = TransactionsCategories.BALANCE_CREDITDEBIT_INDICATOR,
                         RANDOMKEY2 = TransactionsCategories.RANDOMKEY2,
                         EXCHANGE_RATES = TransactionsCategories.EXCHANGE_RATES
                     });
            }
            else
            {
                if (financeviewmodel.selectedAccountsList.Count > 0)
                {
                    finance_transactions_found =
                        new List<SmartFinance.TransactionsCategories>
                        (from Provider in financeviewmodel.selectedProvidersList
                         join Account in financeviewmodel.selectedAccountsList
                         on new { Provider.INSTITUTION_CODE, Provider.BRAND_CODE }
                         equals new { Account.INSTITUTION_CODE, Account.BRAND_CODE }
                         join Transactions in financeviewmodel.transactionsViewFound
                         on new { Account.INSTITUTION_CODE, Account.BRAND_CODE, Account.SORTCODE, Account.ACCOUNT_NO }
                         equals new { Transactions.INSTITUTION_CODE, Transactions.BRAND_CODE, Transactions.SORTCODE, Transactions.ACCOUNT_NO }
                         join TransactionsCategories in finance_transactions_found
                         on new { Transactions.INSTITUTION_CODE, Transactions.BRAND_CODE, Transactions.STATEMENT_DATE, Transactions.STATEMENT_NO, Transactions.SORTCODE, Transactions.ACCOUNT_NO, Transactions.UDPRN }
                         equals new { TransactionsCategories.INSTITUTION_CODE, TransactionsCategories.BRAND_CODE, TransactionsCategories.STATEMENT_DATE, TransactionsCategories.STATEMENT_NO, TransactionsCategories.SORTCODE, TransactionsCategories.ACCOUNT_NO, TransactionsCategories.UDPRN }
                         //orderby BankTransaction.TRANSACTION_DATE descending,
                         //         BankTransaction.SEQUENCE_NO descending // Same thing as Login.CREATED desc as we have matched on this field ..
                         select TransactionsCategories);                 // Can use 'new' because we specify the fields below                    
                }
                else
                {
                    if (financeviewmodel.selectedProvidersList.Count > 0)
                    {
                        finance_transactions_found =
                            new List<SmartFinance.TransactionsCategories>
                            (from Provider in financeviewmodel.selectedProvidersList
                             join Transactions in financeviewmodel.transactionsViewFound //bank_transactions_found
                             on new { Provider.INSTITUTION_CODE, Provider.BRAND_CODE }
                             equals new { Transactions.INSTITUTION_CODE, Transactions.BRAND_CODE }
                             join TransactionsCategories in finance_transactions_found
                             on new { Transactions.INSTITUTION_CODE, Transactions.BRAND_CODE, Transactions.STATEMENT_DATE, Transactions.STATEMENT_NO, Transactions.SORTCODE, Transactions.ACCOUNT_NO, Transactions.UDPRN }
                             equals new { TransactionsCategories.INSTITUTION_CODE, TransactionsCategories.BRAND_CODE, TransactionsCategories.STATEMENT_DATE, TransactionsCategories.STATEMENT_NO, TransactionsCategories.SORTCODE, TransactionsCategories.ACCOUNT_NO, TransactionsCategories.UDPRN }
                             //orderby BankTransaction.TRANSACTION_DATE descending,
                             //           BankTransaction.SEQUENCE_NO descending // Same thing as Login.CREATED desc as we have matched on this field ..
                             select TransactionsCategories);
                        // Can use 'new' because we specify the fields below
                    }
                }
            }

            // If we're doing Crypto, then work out a running balance
            if (finance_transactions_found.Count > 0)
            {
                finance_transactions_found = new List<SmartFinance.TransactionsCategories>
                (from TransactionsCategories in finance_transactions_found
                 orderby TransactionsCategories.TRANSACTION_DATE ascending,
                         TransactionsCategories.SEQUENCE_NO ascending
                 select TransactionsCategories);
                double rbalance = 0;
                foreach (SmartFinance.TransactionsCategories trans in finance_transactions_found)
                {
                    if (!string.IsNullOrEmpty(trans.TYPE))
                    {
                        rbalance = rbalance + trans.AMOUNT;

                        trans.BALANCE_AMOUNT = rbalance;
                        trans.BALANCE_CURRENCY_ORDINAL = trans.AMOUNT_CURRENCY_ORDINAL;
                        trans.BALANCE_CREDITDEBIT_INDICATOR = trans.BALANCE_AMOUNT < 0 ? 0 : 1;
                    }
                }
            }

            finance_transactions_found = new List<SmartFinance.TransactionsCategories>
                (from TransactionsCategories in finance_transactions_found
                 join ExchangeRates in ourviewmodel.Blanche.exchangeRatesList
                 on new { TransactionsCategories.TRANSACTION_DATE.Date }            // Ensures we are comparing Date with Date
                 equals new { ExchangeRates.TRANSACTION_DATE.Date }
                 orderby TransactionsCategories.TRANSACTION_DATE descending,
                         TransactionsCategories.SEQUENCE_NO descending // Same thing as Login.CREATED desc as we have matched on this field ..
                 select new SmartFinance.TransactionsCategories
                 {
                     USERNAME = TransactionsCategories.USERNAME,                            // KEY
                     CUBEFACE_CODE = TransactionsCategories.CUBEFACE_CODE,
                     INSTITUTION_CODE = TransactionsCategories.INSTITUTION_CODE,
                     BRAND_CODE = TransactionsCategories.BRAND_CODE,
                     SORTCODE = TransactionsCategories.SORTCODE,
                     ACCOUNT_NO = TransactionsCategories.ACCOUNT_NO,
                     UDPRN = TransactionsCategories.UDPRN,
                     STATEMENT_DATE = TransactionsCategories.STATEMENT_DATE,
                     STATEMENT_NO = TransactionsCategories.STATEMENT_NO,
                     SEQUENCE_NO = TransactionsCategories.SEQUENCE_NO,
                     RANDOMKEY1 = TransactionsCategories.RANDOMKEY1,
                     TRANSACTION_DATE = TransactionsCategories.TRANSACTION_DATE,
                     CREDITDEBIT_INDICATOR = TransactionsCategories.CREDITDEBIT_INDICATOR,
                     PTC_CODE = TransactionsCategories.PTC_CODE,
                     TRANSGROUP_CODE = TransactionsCategories.TRANSGROUP_CODE,
                     TRANSACTION_CODE = TransactionsCategories.TRANSACTION_CODE,
                     DESCRIPTION = TransactionsCategories.DESCRIPTION,
                     TYPE = TransactionsCategories.TYPE,
                     CRYPTO_AMOUNT = TransactionsCategories.CRYPTO_AMOUNT,
                     CRYPTO_CURRENCY_ORDINAL = TransactionsCategories.CRYPTO_CURRENCY_ORDINAL,
                     AMOUNT = TransactionsCategories.AMOUNT,
                     AMOUNT_CURRENCY_ORDINAL = TransactionsCategories.AMOUNT_CURRENCY_ORDINAL,
                     BALANCE_AMOUNT = TransactionsCategories.BALANCE_AMOUNT,
                     BALANCE_CURRENCY_ORDINAL = TransactionsCategories.BALANCE_CURRENCY_ORDINAL,
                     BALANCE_CREDITDEBIT_INDICATOR = TransactionsCategories.BALANCE_CREDITDEBIT_INDICATOR,
                     RANDOMKEY2 = TransactionsCategories.RANDOMKEY2,
                     EXCHANGE_RATES = new[]
                    {
                        ExchangeRates.CURRENCY_RATE_01,
                        ExchangeRates.CURRENCY_RATE_02,
                        ExchangeRates.CURRENCY_RATE_03,
                        ExchangeRates.CURRENCY_RATE_04
                    }
                 });


            
            financeviewmodel.FinanceTransactions = new List<SmartFinance.CommonTransactionsView>();


#if SMARTMAUI
            // Well, showing the UserName boils down to this extremely
            // crude and Chimp-like 
            // 'solution' which is to make the Width of the column 0 or 60!
            // What complete and utter BOLLOCKS all this shit is!!
            // These next two lines assume its only 'us'
            // What we need to do is find out how many different unique
            // Usernames we have.
            financeviewmodel.UsernameWidth = new GridLength(0);
            financeviewmodel.UsernameVisible = false;
            // How many Users are we dealing with here?
            string displayName = "";
            List<SmartFinance.SelectedAccounts> xxxx = new List<SmartFinance.SelectedAccounts>(
                from selected in financeviewmodel.selectedAccountsList
                select selected).DistinctBy(key => new
                {
                    key.USERNAME
                }).ToList();
            if (xxxx.Count() == 1)
            {
                // we've only got one Username but is it ours?
                if (xxxx.First().USERNAME != ourviewmodel.UserName)
                {
                    // No
                    financeviewmodel.UsernameWidth = GridLength.Auto;
                    financeviewmodel.UsernameVisible = true;
                }
            }
            else
            { 
                if (xxxx.Count() > 1)
                {
                    // we have more than one Username so always display them
                    financeviewmodel.UsernameWidth = GridLength.Auto;
                    financeviewmodel.UsernameVisible = true;
                }
            }
            
            financeviewmodel.currencytotals = new List<CurrencyValues>();
            // The currencies we deal with and are prepared to display
            // are based on the Accounts we have in our list i.e. those
            // Accounts which have been checked and they may or may not
            // have produced transactions. If they have, then we total up
            // the transactions for that currency otherwise we display zero.
            // Simples, eh?
            // Ok build the currencies tab
            List<CurrencyTab> currencyTemp = new List<CurrencyTab>();
            List<SmartFinance.SelectedAccounts> xyz = new List<SmartFinance.SelectedAccounts>(
                from sa in financeviewmodel.selectedAccountsList
                select sa).DistinctBy(key => new
                {
                    key.CURRENCY_ORDINAL
                }).ToList();
            foreach (SmartFinance.SelectedAccounts chosen in xyz)
            {
                string currencySymbol = "";
                List<SmartData.Currencies> abcd = new List<SmartData.Currencies>(
                        from Curr in ourviewmodel.currenciesList
                        where Curr.ORDINAL == chosen.CURRENCY_ORDINAL
                        select Curr);
                if (abcd.Count > 0)
                {
                    //if (financeviewmodel.CurrencyOrdinal == 0)
                    //{
                    //    indexpos = abcd.First().ORDINAL;
                    //}
                    //else
                    //{
                    //    indexpos = financeviewmodel.CurrencyOrdinal;
                    //}
                    //indexpos--;
                    currencySymbol = abcd.First().ISOCURRENCYSYMBOL;
                    CurrencyTab tab = new CurrencyTab();
                    tab.Code = currencySymbol;
                    tab.Ordinal = chosen.CURRENCY_ORDINAL;
                    tab.Label = currencySymbol;
                    tab.IsVisible = true;
                    currencyTemp.Add(tab);
                }
            }
            financeviewmodel.Currencies = currencyTemp;

            // Now loop round the Selected Transactions and work out the totals
            if (finance_transactions_found.Count > 0)
            {
                int count = finance_transactions_found.Count - 1;
                //short last_institution_code = 0;

                foreach (SmartFinance.TransactionsCategories trans in finance_transactions_found)
                {
                    string ConvertFromSymbol = "";
                    short ConvertFromOrdinal = 0;

                    List<CurrencyTab> tab_found =
                        new List<CurrencyTab>
                        (from tabs in financeviewmodel.Currencies
                         where tabs.Ordinal == trans.AMOUNT_CURRENCY_ORDINAL
                         select tabs);
                    if (tab_found.Count() > 0)
                    {
                        ConvertFromSymbol = tab_found.First().Code;
                        ConvertFromOrdinal = trans.AMOUNT_CURRENCY_ORDINAL;

                    }
                    else
                    {
                        Console.WriteLine("Bug!");
                    }

                    if (!string.IsNullOrEmpty(ConvertFromSymbol))
                    {
                        string itsacredit = "",
                                itsadebit = "";
                        decimal value = 0m;
                        if (trans.CREDITDEBIT_INDICATOR == 1) // <= ALL BECAUSE OF SQLITE
                        {
                            // Its in here we check currencyOrdinal
                            // If its 0 (zero) we do nothing
                            // If its > 0 we do a rate conversion
                            value = ReturnValue(financeviewmodel,
                                                        financeviewmodel.ConvertToSymbol, // The one we want
                                                        ConvertFromSymbol,                // The one we've got!
                                                        ConvertFromOrdinal,               // The one we've got
                                                        ourviewmodel.exchangeRate,
                                                        (decimal)(trans.AMOUNT));
                            itsacredit = SmartSpikeFinanceV2017.GetCultureView(ourviewmodel,
                                                      financeviewmodel,
                                                      ConvertFromSymbol,
                                                      financeviewmodel.ConvertToSymbol,
                                                      (decimal)(value / 100m));
                        }
                        else
                        {
                            // Its in here we check currencyOrdinal
                            // If its 0 (zero) we do nothing
                            // If its > 0 we do a rate conversion
                            // We want DEBITS to display WITHOUT a '-' sign <= WRONG!
                            // NO! For Crypto NOT TRUE Sends are MINUSES!!!!
                            value = ReturnValue(financeviewmodel,
                                                        financeviewmodel.ConvertToSymbol, // The one we want
                                                        ConvertFromSymbol,                // The one we've got!
                                                        ConvertFromOrdinal,               // The one we've got
                                                        ourviewmodel.exchangeRate,
                                                        (decimal)(trans.AMOUNT));
                            itsadebit = SmartSpikeFinanceV2017.GetCultureView(ourviewmodel,
                                                      financeviewmodel,
                                                      ConvertFromSymbol,
                                                      financeviewmodel.ConvertToSymbol,
                                                      (decimal)(value / 100m));
                        }

                        List<CurrencyValues> totals_found =
                            new List<CurrencyValues>
                            (from totals in financeviewmodel.currencytotals
                             where totals.Code == ConvertFromSymbol
                             select totals);
                        if (totals_found.Count() > 0)
                        {
                            if (trans.CREDITDEBIT_INDICATOR == 1)
                            {
                                totals_found.First().PaidIn += value;
                            }
                            else
                            {
                                totals_found.First().PaidOut += value;
                            }
                        }
                        else
                        {
                            CurrencyValues cv = new CurrencyValues();
                            cv.Code = ConvertFromSymbol;
                            cv.Difference = 0;
                            if (trans.CREDITDEBIT_INDICATOR == 1)
                            {
                                cv.PaidIn = value;
                                cv.PaidOut = 0;
                            }
                            else
                            {
                                cv.PaidIn = 0;
                                cv.PaidOut += value;
                            }
                            financeviewmodel.currencytotals.Add(cv);
                        }
                        SmartFinance.CommonTransactionsView abc = new SmartFinance.CommonTransactionsView()
                        {
                            // Notice ... no TRANSACTION_CREATED or SEQUENCE_NO in here
                            // these two are ONLY USED to get the transactions sorted correctly
                            USERNAME = displayName,
                            INSTITUTION_CODE = trans.INSTITUTION_CODE,
                            BRAND_CODE = trans.BRAND_CODE,
                            PTC_CODE = trans.PTC_CODE,
                            TRANSACTION_DATE = trans.TRANSACTION_DATE,
                            SEQUENCE_NO = trans.SEQUENCE_NO,
                            SORTCODE = trans.SORTCODE,
                            ACCOUNT_NO = trans.ACCOUNT_NO,
                            DESCRIPTION = trans.DESCRIPTION,
                            //CURRENCY = trans.CURRENCY_ORDINAL,
                            CURRENCY_DISPLAY = ConvertFromSymbol,
                            TRANSGROUP_CODE = trans.TRANSGROUP_CODE,
                            TRANSACTION_CODE = trans.TRANSACTION_CODE,
                            CREDITDEBIT_INDICATOR = trans.CREDITDEBIT_INDICATOR,
                            CRYPTO_AMOUNT = trans.CRYPTO_AMOUNT,  // Holds +ve and -ve values
                            CRYPTO_AMOUNT_DISPLAY = Convert.ToString(trans.CRYPTO_AMOUNT),
                            AMOUNT = trans.AMOUNT,  // Holds +ve and -ve values

                            CREDITS = itsacredit,   // Always displays as +ve
                            DEBITS = itsadebit,     // Might display as +ve OR -ve
                            BALANCE = trans.BALANCE_AMOUNT, // Holds +ve and -ve values
                            BALANCE_DISPLAY = DisplayValue(ourviewmodel,
                                                    financeviewmodel,
                                                    financeviewmodel.ConvertToSymbol,
                                                    ConvertFromSymbol,
                                                    ConvertFromOrdinal,
                                                    ourviewmodel.exchangeRate,
                                                    (decimal)(trans.BALANCE_AMOUNT / 100.0))
                        };

                        string brand_name = "";
                        List<SmartFinance.Brands> brands_found = SmartSpikeFinanceV2017.Finance_Find_Brands(ourviewmodel,
                                                                                                    financeviewmodel,
                                                                                                    trans.INSTITUTION_CODE,
                                                                                                    trans.BRAND_CODE);
                        if (brands_found.Count > 0)
                        {
                            brand_name = brands_found.First().BRAND_NAME;

                            abc.BANKLOGO = Lookup_LOGO(ourviewmodel,
                                                           financeviewmodel,
                                                           SmartParametersV2016.Finance,
                                                           brand_name);

                        }

                        financeviewmodel.FinanceTransactions.Add(abc);
                    }
                }
            }
#else

        double last_balance = 0;

            string last_username = "";
            string displayName = "";

            // totals is 5 because FinanceTotals is based on currenciesList which is 5
            decimal[,] totals = new decimal[financeviewmodel.FinanceTotals.GetLength(0), 3];


            if (finance_transactions_found.Count > 0)
            {
                int count = finance_transactions_found.Count - 1;
                short last_institution_code = 0;

                foreach (SmartFinance.TransactionsCategories trans in finance_transactions_found)
                {
                    // Look you thick cunt.  The AMOUNT (Credit or Debit)
                    // doesn't depend on the Culture.
                    // Yes, you can convert the AMOUNT to a
                    // differenct exchange rate, but the original CURRENCY
                    // of the AMOUNT doesn't change! The **CURRENCY** defines
                    // how it's value is treated, it's the **CULTURE** that determines
                    // how the AMOUNT is DISPLAYED. So you need the CultureInfo for
                    // the Cubeface.  This also defines how the TRANSACTION_DATE is displayed!!!
                    // This is the way the currency displays are going to work
                    // The FACE_CURRENCY is 0 (zero) by default which means that
                    // no conversions of any values (or totals) is performed.
                    // So the totals might end up as follows:
                    // totals[0, 0] GBP paid in
                    // totals[0, 1] GBP paid out
                    // totals[0, 2] GBP difference
                    // totals[1, 0] EUR paid in
                    // totals[1, 1] EUR paid out
                    // totals[1, 2] EUR difference
                    // totals[2, 0] USD paid in
                    // totals[2, 1] USD paid out
                    // totals[2, 2] USD difference
                    // totals[3, 0] CAD paid in    <= chosen currency
                    // totals[3, 1] CAD paid out   <= chosen currency
                    // totals[3, 2] CAD difference <= chosen currency
                    // If the FACE_CURRENCY is > 0 (i.e. 1 for GBP) then that means that
                    // ALL value amounts are converted to GBP. This meand we will only
                    // have ONE set of totals:
                    // totals[0, 0] GBP paid in
                    // totals[0, 1] GBP paid out
                    // totals[0, 2] GBP difference
                    // If the FACE_CURRENCY is 2 (for EUR) we will still only get ONE
                    // set of totals
                    // totals[1, 0] EUR paid in
                    // totals[1, 1] EUR paid out
                    // totals[1, 2] EUR difference
                    // If the FACE_CURRENCY is 3 (for USD) we will still only get ONE
                    // set of totals
                    // totals[2, 0] USD paid in
                    // totals[2, 1] USD paid out
                    // totals[2, 2] USD difference
                    // If the FACE_CURRENCY is 4 (for CAD) we will still only get ONE
                    // set of totals
                    // totals[3, 0] CAD paid in     <= chosen currency
                    // totals[3, 1] CAD paid out    <= chosen currency
                    // totals[3, 2] CAD difference  <= chosen currency
                    //
                    // Now FACE_CURRENCY comes through as financeviewmodel.currencyOrdinalX
                    // so that's what we use to test with
                    short indexpos = 0;
                    string ConvertFromSymbol = "";
                    int ConvertFromOrdinal = 0;

                    // What do we want to do here? For None, we just want to
                    // find the symbol for the ordinal and set the indexpos to the
                    // derived ordinal
                    // For GBP, EUR or USED we want to derive the trans stuff
                    // BUT we want to set the indexpos to the pre-defined
                    // finance ordinal
                    // currenciesList is usually very small (< 10)
                    List<SmartData.Currencies> abcd = new List<SmartData.Currencies>(
                        from Curr in ourviewmodel.currenciesList
                        where Curr.ORDINAL == trans.AMOUNT_CURRENCY_ORDINAL
                        select Curr);
                    if (abcd.Count > 0)
                    {
                        if (financeviewmodel.CurrencyOrdinal == 0)
                        {
                            indexpos = abcd.First().ORDINAL;
                        }
                        else
                        {
                            indexpos = financeviewmodel.CurrencyOrdinal;
                        }
                        indexpos--;
                        ConvertFromSymbol = abcd.First().ISOCURRENCYSYMBOL;
                        ConvertFromOrdinal = abcd.First().ORDINAL;
                    }

                    if (last_username != trans.USERNAME)
                    {
                        last_username = trans.USERNAME;
                        displayName = SmartRoutinesV2018.GetDisplayName(ourviewmodel, trans.USERNAME);
                    }

                    string itsacredit = "",
                            itsadebit = "";
                    decimal value = 0m;
                    if (trans.CREDITDEBIT_INDICATOR == 1) // <= ALL BECAUSE OF SQLITE
                    {
                        // Its in here we check currencyOrdinal
                        // If its 0 (zero) we do nothing
                        // If its > 0 we do a rate conversion
                        value = ReturnValue(financeviewmodel,
                                                    financeviewmodel.ConvertToSymbol, // The one we want
                                                    ConvertFromSymbol,                // The one we've got!
                                                    ConvertFromOrdinal,               // The one we've got
                                                    ourviewmodel.exchangeRate,
                                                    (decimal)(trans.AMOUNT));
                        itsacredit = SmartSpikeFinanceV2017.GetCultureView(ourviewmodel,
                                                  financeviewmodel,
                                                  ConvertFromSymbol,
                                                  financeviewmodel.ConvertToSymbol,
                                                  (decimal)(value / 100m));
                    }
                    else
                    {
                        // Its in here we check currencyOrdinal
                        // If its 0 (zero) we do nothing
                        // If its > 0 we do a rate conversion
                        // We want DEBITS to display WITHOUT a '-' sign <= WRONG!
                        // NO! For Crypto NOT TRUE Sends are MINUSES!!!!
                        value = ReturnValue(financeviewmodel,
                                                    financeviewmodel.ConvertToSymbol, // The one we want
                                                    ConvertFromSymbol,                // The one we've got!
                                                    ConvertFromOrdinal,               // The one we've got
                                                    ourviewmodel.exchangeRate,
                                                    (decimal)(trans.AMOUNT));
                        itsadebit = SmartSpikeFinanceV2017.GetCultureView(ourviewmodel,
                                                  financeviewmodel,
                                                  ConvertFromSymbol,
                                                  financeviewmodel.ConvertToSymbol,
                                                  (decimal)(value / 100m));
                    }

                    SmartFinance.CommonTransactionsView abc = new SmartFinance.CommonTransactionsView()
                    {
                        // Notice ... no TRANSACTION_CREATED or SEQUENCE_NO in here
                        // these two are ONLY USED to get the transactions sorted correctly
                        USERNAME = displayName,
                        INSTITUTION_CODE = trans.INSTITUTION_CODE,
                        BRAND_CODE = trans.BRAND_CODE,
                        PTC_CODE = trans.PTC_CODE,
                        TRANSACTION_DATE = trans.TRANSACTION_DATE,
                        SEQUENCE_NO = trans.SEQUENCE_NO,
                        SORTCODE = trans.SORTCODE,
                        ACCOUNT_NO = trans.ACCOUNT_NO,
                        DESCRIPTION = trans.DESCRIPTION,
                        //CURRENCY = trans.CURRENCY_ORDINAL,
                        CURRENCY_DISPLAY = ConvertFromSymbol,
                        TRANSGROUP_CODE = trans.TRANSGROUP_CODE,
                        TRANSACTION_CODE = trans.TRANSACTION_CODE,
                        CREDITDEBIT_INDICATOR = trans.CREDITDEBIT_INDICATOR,
                        CRYPTO_AMOUNT = trans.CRYPTO_AMOUNT,  // Holds +ve and -ve values
                        CRYPTO_AMOUNT_DISPLAY = Convert.ToString(trans.CRYPTO_AMOUNT),
                        AMOUNT = trans.AMOUNT,  // Holds +ve and -ve values

                        CREDITS = itsacredit,   // Always displays as +ve
                        DEBITS = itsadebit,     // Might display as +ve OR -ve
                        BALANCE = trans.BALANCE_AMOUNT, // Holds +ve and -ve values
                        BALANCE_DISPLAY = DisplayValue(ourviewmodel,
                                                    financeviewmodel,
                                                    financeviewmodel.ConvertToSymbol,
                                                    ConvertFromSymbol,
                                                    ConvertFromOrdinal,
                                                    ourviewmodel.exchangeRate,
                                                    (decimal)(trans.BALANCE_AMOUNT / 100.0))
                    };

#if WINFORMS || WPF  || WINUI || SMARTMAUI
                    string brand_name = "";
                    List<SmartFinance.Brands> brands_found = SmartSpikeFinanceV2017.Finance_Find_Brands(ourviewmodel,
                                                                                                financeviewmodel,
                                                                                                trans.INSTITUTION_CODE,
                                                                                                trans.BRAND_CODE);
                    if (brands_found.Count > 0)
                    {
                        brand_name = brands_found.First().BRAND_NAME;

                        abc.BANKLOGO = Lookup_LOGO(ourviewmodel,
                                                       financeviewmodel,
                                                       SmartParametersV2016.Finance,
                                                       brand_name);

                    }
#endif
                    financeviewmodel.FinanceTransactions.Add(abc);

                    // Now ... we are going to try and split out the Euro and US dollar
                    // transactions.  Each entry in a Sterling account will be in GBP
                    // (which is currency 1 and has an exchange rate of 1 (no surprise there)
                    // So look for transactions which have a CONVERSION e.g. "EUR at" or
                    // "USD at" and isolate them into the EUR and USD lines on our main
                    // balance sheet.  We ONLY do these two currencies at the moment purely
                    // because of space limitations, nothing else

                    //if (trans.DESCRIPTION.Contains("EUR at") ||
                    //    trans.DESCRIPTION.Contains("USD at"))
                    //{
                    //    if (trans.DESCRIPTION.Contains("USD at"))
                    //    {
                    //        // Assume its impure GBP
                    //        if (trans.CREDITDEBIT_INDICATOR)
                    //        {
                    //            // This is an impure GBP transaction
                    //            total_paidinusd += trans.AMOUNT;
                    //        }
                    //        else
                    //        {
                    //            // This is an impure GBP transaction
                    //            total_paidoutusd += trans.AMOUNT;
                    //        }
                    //    }
                    //    else
                    //    {
                    //        // Assume its impure GBP
                    //        if (trans.CREDITDEBIT_INDICATOR)
                    //        {
                    //            // This is an impure GBP transaction
                    //            total_paidineur += trans.AMOUNT;
                    //        }
                    //        else
                    //        {
                    //            // This is an impure GBP transaction
                    //            total_paidouteur += trans.AMOUNT;
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    if (trans.CREDITDEBIT_INDICATOR == 1)
                    {
                        // This is a GBP, EUR or USD transaction
                        totals[indexpos, 0] += value;
                    }
                    else
                    {
                        // This is a GBP, EUR or USD transaction
                        totals[indexpos, 1] += value;
                    }
                    last_balance = trans.BALANCE_AMOUNT;
                    last_institution_code = trans.INSTITUTION_CODE;
                }
            }
            // These are a mix of GBP, EUR and USD transactions
            for (int indexpos = 0; indexpos < totals.GetLength(0); indexpos++)
            {
                // Difference  = Credits        - Debits
                totals[indexpos, 2] = totals[indexpos, 0] - totals[indexpos, 1];
            }
            // DON'T Test for Exchange_Rate of ZERO!  It should *never* be zero, but if these
            // values appear as 0 then it means something has gone drastically wrong!
            int tabIndex = 0;
#if WPF
            financeviewmodel.TabControlPanel.Items.Clear();
#endif
#if WINUI
            financeviewmodel.TabControlPanel.TabItems.Clear();
#endif
#if ANDROIDX
            financeviewmodel.TabTitles.Clear();
#endif
#if SMARTMAUI
            financeviewmodel.TabControlPanel.Children.Clear();
#endif
            for (int indexpos = 0; indexpos < totals.GetLength(0); indexpos++)
            {
                if (!(totals[indexpos, 0] == 0 &&
                        totals[indexpos, 1] == 0 &&
                        totals[indexpos, 2] == 0))
                {
                    // Now, find the currency in currenciesList from indexpos
                    string ConvertFromSymbol = "";
                    int ConvertFromOrdinal = 0;
                    foreach (SmartData.Currencies abcd in ourviewmodel.currenciesList)
                    {
                        if (abcd.ORDINAL == indexpos + 1)
                        {
                            ConvertFromSymbol = abcd.ISOCURRENCYSYMBOL;
                            ConvertFromOrdinal = abcd.ORDINAL;
                            break;
                        }
                    }
                    // Check we have a symbol
                    if (!string.IsNullOrEmpty(ConvertFromSymbol))
                    {
                        // Try and do the Debits/Credits here
                        // Big mistake
                        // We don't ALWAYS do GBP even if its zeros everywhere
                        SmartFinance.TotalsView paidin = new SmartFinance.TotalsView
                        {
                            DESCRIPTION = ConvertFromSymbol,
                            AMOUNT = DisplayValue(ourviewmodel,
                                                    financeviewmodel,
                                                    financeviewmodel.ConvertToSymbol,
                                                    ConvertFromSymbol,
                                                    ConvertFromOrdinal,
                                                    ourviewmodel.exchangeRate,
                                                    (decimal)(totals[indexpos, 0] / 100.0m))
                        };
                        financeviewmodel.FinanceTotals[tabIndex, 0].Add(paidin);
                        SmartFinance.TotalsView paidout = new SmartFinance.TotalsView
                        {
                            DESCRIPTION = ConvertFromSymbol,
                            AMOUNT = DisplayValue(ourviewmodel,
                                                    financeviewmodel,
                                                    financeviewmodel.ConvertToSymbol,
                                                    ConvertFromSymbol,
                                                    ConvertFromOrdinal,
                                                    ourviewmodel.exchangeRate,
                                                    (decimal)(totals[indexpos, 1] / 100.0m))
                        };
                        financeviewmodel.FinanceTotals[tabIndex, 1].Add(paidout);
                        SmartFinance.TotalsView difference = new SmartFinance.TotalsView
                        {
                            DESCRIPTION = ConvertFromSymbol,
                            AMOUNT = DisplayValue(ourviewmodel,
                                                    financeviewmodel,
                                                    financeviewmodel.ConvertToSymbol,
                                                    ConvertFromSymbol,
                                                    ConvertFromOrdinal,
                                                    ourviewmodel.exchangeRate,
                                                    (decimal)(totals[indexpos, 2] / 100.0m))
                        };
                        financeviewmodel.FinanceTotals[tabIndex, 2].Add(difference);

                        // try and do the Debits and Credits
#if WINFORMS
                        financeviewmodel.TabControlPanel.TabPages.Add(CreateTabItem(ourviewmodel, financeviewmodel, tabIndex, ConvertFromSymbol));
                        financeviewmodel.TabControlPanel.SizeMode = TabSizeMode.Fixed;
                        financeviewmodel.TabControlPanel.DrawMode = TabDrawMode.OwnerDrawFixed;
#endif
#if WPF
                        financeviewmodel.TabControlPanel.Items.Add(CreateTabItem(ourviewmodel, financeviewmodel, tabIndex, ConvertFromSymbol));
#endif
#if WINUI
                        BuildTabContent(ourviewmodel, financeviewmodel, tabIndex, ConvertFromSymbol);
#endif
#if ANDROIDX
                        BuildTabContent(financeviewmodel, ConvertFromSymbol);
#endif
#if SMARTMAUI
                        //financeviewmodel.TabControlPanel.Children.Add(CreateTabItem(ourviewmodel, financeviewmodel, tabIndex, ConvertFromSymbol));
#endif
                    }
                    tabIndex++;
                }
            }
#if WINFORMS
            if (financeviewmodel.TabControlPanel.TabPages.Count > 0)
            {
                financeviewmodel.TabControlPanel.SelectedIndex = 0;
            }
#endif
#if WPF
            if (financeviewmodel.TabControlPanel.Items.Count > 0)
            {
                financeviewmodel.TabControlPanel.SelectedIndex = 0;
            }
#endif
#if WINUI
            // Default to the first if there is one
            if (financeviewmodel.grids.Count > 0)
            {
                financeviewmodel.TabControlPanel.TabItems.Add(financeviewmodel.grids[0]);
            }
#endif
            switch (financeviewmodel.FinanceTotals[0, 2].Count)
            {
                case 2:      // Two currencies
                    financeviewmodel.PaidInFontSize =
                        financeviewmodel.PaidOutFontSize =
                        financeviewmodel.DifferenceFontSize = 7;
                    break;
                case 3:     // Three currencies
                    financeviewmodel.PaidInFontSize =
                        financeviewmodel.PaidOutFontSize =
                        financeviewmodel.DifferenceFontSize = 4;
                    break;
                default:    // One currency
                    financeviewmodel.PaidInFontSize =
                        financeviewmodel.PaidOutFontSize =
                        financeviewmodel.DifferenceFontSize = 12;
                    break;
            }
            // Well, showing the UserName boils down to this extremely
            // crude and Chimp-like 
            // 'solution' which is to make the Width of the column 0 or 60!
            // What complete and utter BOLLOCKS all this shit is!!

            //financeviewmodel.UsernameWidth = 0;
            //financeviewmodel.UsernameVisible = false;

            // Should always have ONE entry even if its 0
            if (financeviewmodel.FinanceTotals[0, 2].Count > 0)
            {
#if WINFORMS || WPF || SMARTMAUI
                financeviewmodel.TotalValue = financeviewmodel.FinanceTotals[0, 2][0].AMOUNT;
#endif
#if WINUI
                financeviewmodel.TotalValue = financeviewmodel.FinanceTotals[0, 2][0].AMOUNT;
#endif
#if ANDROIDX
                financeviewmodel.TotalValue.Text = financeviewmodel.FinanceTotals[0, 2][0].AMOUNT;
#endif
            }

            if (financeviewmodel.FinanceTransactions.Count == 0)
            {
#if WINFORMS || WPF || SMARTMAUI
                financeviewmodel.TotalTransactions = "0";
#endif
#if WINUI
                financeviewmodel.TotalTransactions = "0";
#endif
#if ANDROIDX
                financeviewmodel.TotalTransactions.Text = "0";
#endif
                // You haven't found anything OR the Dataset is empty
                // so there is nothing to find!  Leave the Start and End dates
                // alone!! But clear the Results down
#if WINFORMS || WPF || SMARTMAUI
                financeviewmodel.ResultStartDate = "";
                financeviewmodel.ResultEndDate = "";
#endif
#if WINUI
                financeviewmodel.ResultStartDate = "";
                financeviewmodel.ResultEndDate = "";
#endif
#if ANDROIDX
                financeviewmodel.ResultStartDate.Text = "";
                financeviewmodel.ResultEndDate.Text = "";
#endif
            }
            else
            {
#if WINFORMS || WPF || SMARTMAUI
                financeviewmodel.TotalTransactions = financeviewmodel.FinanceTransactions.Count.ToString();
#endif
#if WINUI
                financeviewmodel.TotalTransactions = financeviewmodel.FinanceTransactions.Count.ToString();
#endif
#if ANDROIDX
                financeviewmodel.TotalTransactions.Text = financeviewmodel.FinanceTransactions.Count.ToString();
#endif
#if WINFORMS
                if (financeviewmodel.StartDate == SmartParametersV2016.defaultDate &&
                    financeviewmodel.EndDate == SmartParametersV2016.defaultMaxdate)
                {
                    financeviewmodel.StartDateEnabled =
                    financeviewmodel.EndDateEnabled = false;
                    financeviewmodel.StartDate = financeviewmodel.FinanceTransactions.Last().TRANSACTION_DATE;
                    financeviewmodel.EndDate = financeviewmodel.FinanceTransactions.First().TRANSACTION_DATE;
                    financeviewmodel.StartDateEnabled =
                    financeviewmodel.EndDateEnabled = true;
                }
#endif
#if WPF || SMARTMAUI
                if (financeviewmodel.StartDate == SmartParametersV2016.defaultDate &&
                    financeviewmodel.EndDate == SmartParametersV2016.defaultMaxdate)
                {
                    financeviewmodel.StartDateEnabled =
                    financeviewmodel.EndDateEnabled = false;
                    financeviewmodel.StartDate = financeviewmodel.FinanceTransactions.Last().TRANSACTION_DATE;
                    financeviewmodel.EndDate = financeviewmodel.FinanceTransactions.First().TRANSACTION_DATE;
                    financeviewmodel.StartDateEnabled =
                    financeviewmodel.EndDateEnabled = true;
                }
#endif
#if WINUI
                if (financeviewmodel.StartDate == SmartParametersV2016.defaultDate &&
                    financeviewmodel.EndDate == SmartParametersV2016.defaultMaxdate)
                {
                    financeviewmodel.StartDateEnabled =
                    financeviewmodel.EndDateEnabled = false;
                    financeviewmodel.StartDate = financeviewmodel.FinanceTransactions.Last().TRANSACTION_DATE;
                    financeviewmodel.EndDate = financeviewmodel.FinanceTransactions.First().TRANSACTION_DATE;
                    financeviewmodel.StartDateEnabled =
                    financeviewmodel.EndDateEnabled = true;
                }
#endif
#if ANDROIDX
                if (financeviewmodel.StartDate.DateTime == SmartParametersV2016.defaultDate &&
                    financeviewmodel.EndDate.DateTime == SmartParametersV2016.defaultMaxdate)
                {
                    financeviewmodel.StartDateEnabled =
                    financeviewmodel.EndDateEnabled = false;
                    financeviewmodel.StartDate.DateTime = financeviewmodel.FinanceTransactions.First().TRANSACTION_DATE;
                    financeviewmodel.EndDate.DateTime = financeviewmodel.FinanceTransactions.Last().TRANSACTION_DATE;
                    financeviewmodel.StartDateEnabled =
                    financeviewmodel.EndDateEnabled = true;
                }
#endif
#if WINFORMS || WPF || SMARTMAUI
                financeviewmodel.ResultStartDate = financeviewmodel.FinanceTransactions.Last().TRANSACTION_DATE.ToString("d", financeviewmodel.CultureINF);
                financeviewmodel.ResultEndDate = financeviewmodel.FinanceTransactions.First().TRANSACTION_DATE.ToString("d", financeviewmodel.CultureINF);
#endif
#if WINUI
                financeviewmodel.ResultStartDate = financeviewmodel.FinanceTransactions.Last().TRANSACTION_DATE.ToString("d", financeviewmodel.CultureINF);
                financeviewmodel.ResultEndDate = financeviewmodel.FinanceTransactions.First().TRANSACTION_DATE.ToString("d", financeviewmodel.CultureINF);
#endif
#if ANDROIDX
                financeviewmodel.ResultStartDate.Text = financeviewmodel.FinanceTransactions.Last().TRANSACTION_DATE.ToString("d", financeviewmodel.CultureINF);
                financeviewmodel.ResultEndDate.Text = financeviewmodel.FinanceTransactions.First().TRANSACTION_DATE.ToString("d", financeviewmodel.CultureINF);
#endif
            }

            FinanceUpdateTransactions(financeviewmodel);

#if (WPF || SMARTMAUI) && CRYPTO
            // THIS NEEDS FIXING RAY!!!

            if (financeviewmodel.v2accountsList.Count > 1)
            {
                // Turn on the Sort Code and Account No in the grid
                //FrontEndGUI.DecodeDataGridViewSortAccount(TransactionsDataGrid, true);
            }
            else
            {
                //FrontEndGUI.DecodeDataGridViewSortAccount(TransactionsDataGrid, false);
            }
#endif
#if (WINUI) && CRYPTO
            // THIS NEEDS FIXING RAY!!!

            if (financeviewmodel.v2accountsList.Count > 1)
            {
                // Turn on the Sort Code and Account No in the grid
                //FrontEndGUI.DecodeDataGridViewSortAccount(TransactionsDataGrid, true);
            }
            else
            {
                //FrontEndGUI.DecodeDataGridViewSortAccount(TransactionsDataGrid, false);
            }
#endif
#if WPF || SMARTMAUI
            if (financeviewmodel.selectedAccountsList.Count == 0)
            {
                // Turn on the Sort Code and Account No in the grid
                FrontEndGUI.DecodeListViewSortAccount(financeviewmodel, true);
            }
            else
            {
                FrontEndGUI.DecodeListViewSortAccount(financeviewmodel, false);
            }
#endif
#if WINUI
            if (financeviewmodel.selectedAccountsList.Count > 1)
            {
                // Turn on the Sort Code and Account No in the grid
                FrontEndGUI.DecodeDataGridViewSortAccount(financeviewmodel, true);
            }
            else
            {
                FrontEndGUI.DecodeDataGridViewSortAccount(financeviewmodel, false);
            }
#endif
            // Because of ANDROID!! we no longer have 'PlotView' here (it shows later, the Android
            // chart shows in FinanceCharts as a ViewPager2 something or fucking other)
            // So this is *only* for WINFORMS or WPF or WINUI

            BuildDebitsCreditsChart(
#if WINFORMS
                                    components,
#endif
                                    ourviewmodel,
                                    financeviewmodel);

#endif
            return;
        }

//        internal static async Task<ImageSource> Lookup_TransactionLogo(MainViewModel ourviewmodel,
//                                                                FinanceViewModel financeviewmodel,
//                                                                short institutionCode,
//                                                                short brandCode)
//        {
//            financeviewmodel.dno_stream = Stream.Null;

//            // Go and find the image
//            // Look in the table first
//            // I'm not 1000% sure this 'caching' is needed ...
//            // but I'm going to leave it in for the time being
//            // in case I need it for the Transaction items .. which I'm doing now
//            if (!await Lookup_LOGO_Async(ourviewmodel,
//                                                financeviewmodel,
//                                                SmartParametersV2016.Finance,
//                                                institutionCode,
//                                                brandCode))
//            {
//                if (!string.IsNullOrEmpty(ourviewmodel.errorMessage))
//                {
//                    return false;
//                }
//            }
//            else
//            {
//                if (financeviewmodel.dno_stream != null)
//                {
//                    //
//                    // My God in Heaven, this stuff is SHIT
//                    //                                 ====
//                    // For reasons best know to the Chimps, when you
//                    // re-read a Stream the position is at the END!!
//                    financeviewmodel.dno_stream.Position = 0;
//#if WINFORMS
//                        System.Drawing.Image pbox = System.Drawing.Image.FromStream(financeviewmodel.dno_stream);
//                        financeviewmodel.PictureBoxLOGO = pbox;
//#endif
//#if WPF  || WINUI
//                    BitmapImage pbox = new BitmapImage();
//#if WPF
//                    pbox.BeginInit();
//                    pbox.StreamSource = financeviewmodel.dno_stream;
//                    pbox.EndInit();
//#endif
//                    //return pbox;
//#endif
//#if ANDROIDX
//                    Bitmap pbox = BitmapFactory.DecodeStream(financeviewmodel.dno_stream);
//                    financeviewmodel.PictureBoxLOGO.SetImageBitmap(pbox);
//#endif
//                    if (financeviewmodel.sucked)
//                    {
//                        MainViewModel.ImageItem itemView = new MainViewModel.ImageItem()
//                        {
//                            Cubeface = financeviewmodel.cubeface_code,
//                            ImageName = financeviewmodel.brand_name,
//                            StreamBits = financeviewmodel.dno_stream
//                        };
//                        ourviewmodel.ImageList.Add(itemView);
//                    }
//                }
//            }
//            return 
//        }

        internal static void UpdateCryptoLedgers(FinanceViewModel financeviewmodel)
        {
            // Make a Deep copy
            List<SmartFinance.CryptoLedgers> tempLedger = financeviewmodel.PLO.crypto_ledgersList?.Select(p => p.Clone()).ToList();

            foreach (SmartFinance.CryptoLedgers temp in tempLedger)//financeviewmodel.PLO.crypto_ledgersList)
            {
                foreach (SmartFinance.CryptoWalletTotals cwt in financeviewmodel.PLO.crypto_wallettotalsList)
                {
                    if (cwt.CRYPTO_WALLET_NAME != "")
                    {
                        if (temp.SOURCE == cwt.ADDRESS)
                        {
                            temp.SOURCE = cwt.CRYPTO_WALLET_NAME;

                        }
                        if (temp.DESTINATION == cwt.ADDRESS)
                        {
                            temp.DESTINATION = cwt.CRYPTO_WALLET_NAME;
                        }
                    }
                }
            }
            financeviewmodel.FinanceCryptoLedger = tempLedger;
            return;
        }

#if WPF
        internal static string FindFieldName(object sender, GridViewColumnHeader columnHeader)
        {
            string fieldName = "";
            if (columnHeader != null)
            {

                GridViewColumn clickedColumn = columnHeader.Column;


                // Retrieve the CellTemplate associated with the clicked column
                DataTemplate cellTemplate = clickedColumn.CellTemplate;

                // If the cell template has a Binding (e.g., TextBlock's Text)
                if (cellTemplate != null)
                {
                    // Retrieve the Binding from the TextBlock inside the CellTemplate
                    FrameworkElement cellContent = cellTemplate.LoadContent() as FrameworkElement;
                    if (cellContent != null)
                    {
#if !WINFORMS
                        Binding binding = cellContent.GetBindingExpression(TextBlock.TextProperty)?.ParentBinding;
                        // If a Binding exists, retrieve the field name from it
                        if (binding != null)
                        {
                            fieldName = binding.Path.Path;
                        }
#endif
                    }
                }
            }
            return fieldName;
        }
#endif

#if SMARTMAUI
        private void FindFieldName(object sender, TappedEventArgs e)
        {
            var gesture = sender as Label;

            string fieldName =
                gesture?.GestureRecognizers
                    .OfType<TapGestureRecognizer>()
                    .FirstOrDefault()?
                    .CommandParameter?
                    .ToString();

            // "BALANCE"
        }
#endif

#if WINUI
        //internal static string FindFieldName(object sender, object column)
        //{
        //    if (column is DataGridColumn dgColumn)
        //    {
        //        return (dgColumn.Binding as Binding)?.Path?.Path ?? "";
        //    }

        //    return "";
        //}
        internal static string FindFieldName(object column)
        {
            if (column is DataGridColumn dgColumn)
            {
                return dgColumn.Tag?.ToString() ?? "";
            }

            return "";
        }
#endif

        public static void ReorderTransactions(FinanceViewModel financeviewmodel, string fieldName, int index)
        {
            // Get the PropertyInfo once
            PropertyInfo propertyInfo = typeof(SmartFinance.Transactions)
                       .GetProperty(fieldName, SmartParametersV2016.bindingFlags);
            if ((financeviewmodel.SortDirection & (1 << index)) == 0)
            {
                // Sort Ascending (default)
                financeviewmodel.FinanceTransactions =
                    financeviewmodel.FinanceTransactions
                    .OrderBy(p => propertyInfo.GetValue(p, null))
                    .ToList();
                // Set the bit ON for the next column click
                financeviewmodel.SortDirection |= (1 << index);
            }
            else
            {
                // Sort Descending
                financeviewmodel.FinanceTransactions =
                    financeviewmodel.FinanceTransactions
                    .OrderByDescending(p => propertyInfo.GetValue(p, null))
                    .ToList();
                // Set the bit OFF for the next column click
                financeviewmodel.SortDirection &= ~(1 << index);
            }
            return;
        }
    }
}