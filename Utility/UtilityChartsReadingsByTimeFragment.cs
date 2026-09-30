#if ANDROIDX
using Android.Views;
using OxyPlot;
using OxyPlot.Xamarin.Android;  // Make sure OxyPlot.Core is version 2.0 <= !!!!!!

namespace SmartCubeMobile
{
    public class UtilityChartsReadingsByTimeFragment : AndroidX.Fragment.App.Fragment
    {
        private MainViewModel ourviewmodel;
        private UtilityViewModel utilityviewmodel;
        public UtilityChartsReadingsByTimeFragment() { }
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
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.UtilityChartsReadingsByTime, container, false);
            PlotView plotView2 = view.FindViewById<PlotView>(Resource.Id.utilityChartsReadingsByTime);
            if (plotView2 != null &&
                ourviewmodel != null &&
                utilityviewmodel != null)
            {
                plotView2.Model = utilityviewmodel.PlotModel2;
            }
            return view;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            String key = Arguments?.GetString("vm_key");
            if (!string.IsNullOrEmpty(key))
            {
                UtilityViewModelStore.Remove(key);
            }
        }

        public static UtilityChartsReadingsByTimeFragment NewInstance(MainViewModel mainvm, UtilityViewModel utilityvm)
        {
            String key = Guid.NewGuid().ToString();
            UtilityViewModelStore.Add(key, mainvm, utilityvm);
            UtilityChartsReadingsByTimeFragment fragment = new UtilityChartsReadingsByTimeFragment();
            Bundle args = new Bundle();
            args.PutString("vm_key", key);
            fragment.Arguments = args;
            return fragment;
        }
    }
}
#endif