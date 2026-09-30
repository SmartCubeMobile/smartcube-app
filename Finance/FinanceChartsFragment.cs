#if ANDROIDX
using Android.Views;
using OxyPlot;
using OxyPlot.Xamarin.Android;  // Make sure OxyPlot.Core is version 2.0 <= !!!!!!

namespace SmartCubeMobile
{
    public class FinanceChartsFragment : AndroidX.Fragment.App.Fragment
    {
        private FinanceViewModel financeviewmodel;
        public FinanceChartsFragment() { }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            String key = Arguments?.GetString("vm_key");

            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            var result = FinanceViewModelStore.Get(key);
            if (result == null)
            {
                Console.WriteLine("ViewModels lost (process recreation)");
                return;
            }
            financeviewmodel = result.Value.finance;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.FinanceCharts, container, false);
            PlotView plotView2 = view.FindViewById<PlotView>(Resource.Id.financeTransactionsByTime);
            if (plotView2 != null && 
                financeviewmodel != null)
            {
                plotView2.Model = financeviewmodel.PlotModel2;
            }
            return view;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            String key = Arguments?.GetString("vm_key");
            if (!string.IsNullOrEmpty(key))
            {
                FinanceViewModelStore.Remove(key);
            }
        }

        public static FinanceChartsFragment NewInstance(FinanceViewModel financevm)
        {
            String key = Guid.NewGuid().ToString();
            FinanceViewModelStore.Add(key, financevm);
            FinanceChartsFragment fragment = new FinanceChartsFragment();
            Bundle args = new Bundle();
            args.PutString("vm_key", key);
            fragment.Arguments = args;
            return fragment;
        }
    }
}
#endif