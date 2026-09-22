#if ANDROIDX
using Android.Views;
using AndroidX.RecyclerView.Widget;
using static SmartCubeMobile.UtilityReadingsFragment;

namespace SmartCubeMobile
{
    public class UtilityBreakdownFragment : AndroidX.Fragment.App.Fragment
    {
        private UtilityViewModel utilityviewmodel;
        public UtilityBreakdownFragment() { }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment            
            View view = inflater.Inflate(Resource.Layout.UtilityBreakdown, container, false);
            if (utilityviewmodel == null)
            {
                return view;
            }
            RecyclerView breakdownRecyclerView = view.FindViewById<AndroidX.RecyclerView.Widget.RecyclerView>(Resource.Id.utilityBreakdown);
            if (breakdownRecyclerView == null)
            {
                return view;
            }
            // Layout manager that lays out each card in the RecyclerView:
            // Layout Manager Setup:
            // Use the built-in linear layout manager:

            //costsLayoutManager = new GridLayoutManager(MainActivity.mmviewmodel.OurActivity, 1, GridLayoutManager.Vertical, false);//  //(this);

            // Or use the built-in grid layout manager (two horizontal rows):
            // mLayoutManager = new GridLayoutManager
            //        (this, 2, GridLayoutManager.Horizontal, false);

            // Plug the layout manager into the RecyclerView:
            breakdownRecyclerView.SetLayoutManager(new AndroidX.RecyclerView.Widget.LinearLayoutManager(view.Context));
            breakdownRecyclerView.AddItemDecoration(new DividerItemDecoration(view.Context, LinearLayoutManager.Vertical));

            // Will this work? Yes but taken out as list size might change
            //breakdownRecyclerView.HasFixedSize = true;

            // Create an adapter for the RecyclerView, and pass it the
            // data set (the FinanceTransactions) to manage:
            // Plug the adapter into the RecyclerView:
            // But only if there is something to plug!
            List<SmartUtility.AnalysisBreakdownView> utilityBreakdown = utilityviewmodel.UtilityBreakdown ?? new List<SmartUtility.AnalysisBreakdownView>();
            breakdownRecyclerView.SetAdapter(new BreakdownAdapter(utilityBreakdown));
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
            utilityviewmodel = result.Value.utility;
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
        // With acknowledgement to some ginger ...
        // ... whose piece of shit didn't fucking scroll!!!
        // Adapter to connect the data set (UtilityBreakdown) to the RecyclerView: 
        internal class BreakdownAdapter : RecyclerView.Adapter
        {
            // Event handler for item clicks:
            internal event EventHandler<int> ItemClick;

            // Underlying data set (Utility Breakdown):
            internal List<SmartUtility.AnalysisBreakdownView> utilityBreakdown;

            // Load the adapter with the data set (UtilityBreakdown) at construction time:
            internal BreakdownAdapter(List<SmartUtility.AnalysisBreakdownView> breakdown)
            {
                utilityBreakdown = breakdown;
            }

            // Create a new Breakdown CardView (invoked by the layout manager): 
            public override RecyclerView.ViewHolder
                OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                // Inflate the CardView for the Breakdown:
                View itemView = LayoutInflater.From(parent.Context).
                            Inflate(Resource.Layout.UtilityBreakdownItem, parent, false);

                // Create a ViewHolder to find and hold these view references, and 
                // register OnClick with the view holder:
                BreakdownViewHolder vh = new BreakdownViewHolder(itemView, OnClick);
                return vh;
            }

            // Fill in the contents of the Utility Breakdown (invoked by the layout manager):
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                BreakdownViewHolder bdvh = holder as BreakdownViewHolder;
                if (bdvh != null && position < utilityBreakdown.Count)
                {
                    // Set the TextViews in this ViewHolder's CardView 
                    // from this position in the Utility Bills:
                    SmartUtility.AnalysisBreakdownView item = utilityBreakdown[position];

                    bdvh.FromDate.Text = item.FROM_DATE.ToString(SmartParametersV2016.ddmmmyyyyFormat);
                    bdvh.ToDate.Text = item.TO_DATE.ToString(SmartParametersV2016.ddmmmyyyyFormat);
                    bdvh.Code.Text = item.CODE;
                    bdvh.Description.Text = item.DESCRIPTION;
                    bdvh.Items.Text = item.ITEMS;
                    bdvh.ChargesDiscounts.Text = item.CHARGES_DISCOUNTS;
                    bdvh.DayUnits.Text = item.DAY_UNITS;
                    bdvh.DayRate.Text = item.DAY_RATE;
                    bdvh.NightUnits.Text = item.NIGHT_UNITS;
                    bdvh.NightRate.Text = item.NIGHT_RATE;
                    bdvh.TotalUnits.Text = item.TOTAL_UNITS;
                    bdvh.Total.Text = item.TOTAL;
                }
                return;                
            }

            // Return the number of Breakdowns available in the Utility Breakdown:
            public override int ItemCount
            {
                get { return utilityBreakdown.Count; } // NumBreakdown; }
            }

            // Raise an event when the item-click takes place:
            void OnClick(int position)
            {
                if (ItemClick != null)
                    ItemClick(this, position);
            }
        }

        // Implement the ViewHolder pattern: each ViewHolder holds references
        // to the UI components (ImageView and TextView) within the CardView 
        // that is displayed in a row of the RecyclerView:
        internal class BreakdownViewHolder : RecyclerView.ViewHolder
        {
            internal TextView FromDate;
            internal TextView ToDate;
            internal TextView Code;
            internal TextView Description;
            internal TextView Items;
            internal TextView ChargesDiscounts;
            internal TextView DayUnits;
            internal TextView DayRate;
            internal TextView NightUnits;
            internal TextView NightRate;
            internal TextView TotalUnits;
            internal TextView Total;
            // Get references to the views defined in the CardView layout.
            public BreakdownViewHolder(View itemView, Action<int> listener)
                : base(itemView)
            {
                // Locate and cache view references:
                FromDate = itemView.FindViewById<TextView>(Resource.Id.breakdownFromDate);
                ToDate = itemView.FindViewById<TextView>(Resource.Id.breakdownToDate);
                Code = itemView.FindViewById<TextView>(Resource.Id.breakdownCode);
                Description = itemView.FindViewById<TextView>(Resource.Id.breakdownDescription);
                Items = itemView.FindViewById<TextView>(Resource.Id.breakdownItems);
                ChargesDiscounts = itemView.FindViewById<TextView>(Resource.Id.breakdownChargesDiscounts);
                DayUnits = itemView.FindViewById<TextView>(Resource.Id.breakdownDayUnits);
                DayRate = itemView.FindViewById<TextView>(Resource.Id.breakdownDayRate);
                NightUnits = itemView.FindViewById<TextView>(Resource.Id.breakdownNightUnits);
                NightRate = itemView.FindViewById<TextView>(Resource.Id.breakdownNightRate);
                TotalUnits = itemView.FindViewById<TextView>(Resource.Id.breakdownTotalUnits);
                Total = itemView.FindViewById<TextView>(Resource.Id.breakdownTotal);

                // Detect user clicks on the item view and report which item
                // was clicked (by layout position) to the listener:
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }

        public static UtilityBreakdownFragment NewInstance(UtilityViewModel utilityvm)
        {
            String key = Guid.NewGuid().ToString();
            UtilityViewModelStore.Add(key, utilityvm);
            UtilityBreakdownFragment fragment = new UtilityBreakdownFragment();
            Bundle args = new Bundle();
            args.PutString("vm_key", key);
            fragment.Arguments = args;
            return fragment;
        }
    }
}
#endif