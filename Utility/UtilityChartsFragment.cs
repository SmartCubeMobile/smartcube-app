#if ANDROIDX
using Android.Views;
using AndroidX.ViewPager2.Adapter;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.Tabs;
using static Google.Android.Material.Tabs.TabLayoutMediator;

namespace SmartCubeMobile
{
    public class UtilityChartsFragment : AndroidX.Fragment.App.Fragment
    {
        private MainViewModel ourviewmodel;
        private UtilityViewModel utilityviewmodel;
        public UtilityChartsFragment() { }
        
        public static readonly List<string> utilitychartsfragmentTitles = new List<string>()
        {   "Costs by Time", 
            "Readings by Time", 
            "Usage by Time"
        };

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.UtilityCharts, container, false);
            if (ourviewmodel == null || utilityviewmodel == null)
            {
                return view;
            }
            TabLayout tabLayout = view.FindViewById<TabLayout>(Resource.Id.utilitychartstabLayout);
            ViewPager2 pager = view.FindViewById<ViewPager2>(Resource.Id.utilitychartsviewPager);
            if (tabLayout == null || pager == null)
            {
                return view;
            }
            UtilityChartsViewPager2Adapter adapter = new UtilityChartsViewPager2Adapter(
                                                        ChildFragmentManager,
                                                        Lifecycle,
                                                        utilitychartsfragmentTitles.Count,
                                                        ourviewmodel,
                                                        utilityviewmodel
                                                    );
            pager.Adapter = adapter;            
            // False means disable swipe between tabs? I think I want this!
            pager.UserInputEnabled = false;
            // Unnecessary
            //utilitychartsAdapter.NotifyDataSetChanged();            
            new TabLayoutMediator(tabLayout, pager, new UtilityChartsStrategy()).Attach();
            return view;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            String key = Arguments?.GetString("vm_key");

            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            var result = UtilityViewModelStore.Get(key);
            if (result == null)
            {
                Console.WriteLine("ViewModels lost (process recreation)");
                return;
            }
            ourviewmodel = result.Value.main;
            utilityviewmodel = result.Value.utility;
        }
        public override void OnDestroy()
        {
            base.OnDestroy();

            var key = Arguments?.GetString("vm_key");

            if (!string.IsNullOrEmpty(key))
            {
                UtilityViewModelStore.Remove(key);
            }
        }
        public class UtilityChartsStrategy : Java.Lang.Object, ITabConfigurationStrategy
        {
            public void OnConfigureTab(TabLayout.Tab p0, int p1)
            {
                p0.SetText(utilitychartsfragmentTitles[p1]);
            }
        }

        public class UtilityChartsViewPager2Adapter : FragmentStateAdapter
        {
            private readonly int itemCount;
            private MainViewModel ourviewmodel;
            private UtilityViewModel utilityviewmodel;

            public UtilityChartsViewPager2Adapter(AndroidX.Fragment.App.FragmentManager fragmentManager, AndroidX.Lifecycle.Lifecycle lifecycle, int itemCount, MainViewModel mainvm, UtilityViewModel utilityvm) : base(fragmentManager, lifecycle)
            {
                this.itemCount = itemCount;
                this.ourviewmodel = mainvm;
                this.utilityviewmodel = utilityvm;
            }
            public override int ItemCount => itemCount;

            public override AndroidX.Fragment.App.Fragment CreateFragment(int position)
            {
                return position switch
                {
                    0 => UtilityChartsCostsByTimeFragment.NewInstance(ourviewmodel, utilityviewmodel),
                    1 => UtilityChartsReadingsByTimeFragment.NewInstance(ourviewmodel, utilityviewmodel),
                    2 => UtilityChartsPastUsageByTimeFragment.NewInstance(ourviewmodel, utilityviewmodel),
                    _ => new AndroidX.Fragment.App.Fragment()
                };                
            }
        }

        public static UtilityChartsFragment NewInstance(MainViewModel mainvm, UtilityViewModel utilityvm)
        {
            String key = Guid.NewGuid().ToString();
            UtilityViewModelStore.Add(key, mainvm, utilityvm);
            UtilityChartsFragment fragment = new UtilityChartsFragment();
            Bundle args = new Bundle();
            args.PutString("vm_key", key);
            fragment.Arguments = args;
            return fragment;
        }
    }
}
#endif