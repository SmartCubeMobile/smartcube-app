#if ANDROIDX
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace SmartCubeMobile
{
    public class UtilityCostsFragment : AndroidX.Fragment.App.Fragment
    {
        private UtilityViewModel utilityviewmodel;        
        public UtilityCostsFragment() { }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment            
            View view = inflater.Inflate(Resource.Layout.UtilityCosts, container, false);
            if (utilityviewmodel == null)
            {
                return view;
            }
            RecyclerView costsRecyclerView = view.FindViewById<AndroidX.RecyclerView.Widget.RecyclerView>(Resource.Id.utilityCosts);
            if (costsRecyclerView == null)
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
            costsRecyclerView.SetLayoutManager(new LinearLayoutManager(view.Context));
            costsRecyclerView.AddItemDecoration(new DividerItemDecoration(view.Context, LinearLayoutManager.Vertical));
            
            // Will this work? Yes but taken out as list size might change
            //costsRecyclerView.HasFixedSize = true;
            
            // Create an adapter for the RecyclerView, and pass it the
            // data set (the FinanceTransactions) to manage:
            // Plug the adapter into the RecyclerView:
            // But only if there is something to plug!! viewmodel may be null ..
            List<SmartUtility.AnalysisCostsView> utilityCosts = utilityviewmodel.UtilityCosts ?? new List<SmartUtility.AnalysisCostsView>();
            costsRecyclerView.SetAdapter(new CostsAdapter(utilityCosts));

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
        // Adapter to connect the data set (CostsTransactions) to the RecyclerView: 

        // With acknowledgement to some ginger ...
        // ... whose piece of shit didn't fucking scroll!!!
        // Adapter to connect the data set (UtilityCosts) to the RecyclerView: 
        internal class CostsAdapter : RecyclerView.Adapter
        {
            // Event handler for item clicks:
            internal event EventHandler<int> ItemClick;

            // Underlying data set (a photo album):
            internal List<SmartUtility.AnalysisCostsView> utilityCosts;

            // Load the adapter with the data set (UtilityCosts) at construction time:
            internal CostsAdapter(List<SmartUtility.AnalysisCostsView> costs)
            {
                utilityCosts = costs;
            }

            // Create a new costs CardView (invoked by the layout manager): 
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                // Inflate the CardView for the transactions:
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.UtilityCostItem, parent, false);
                // Create a ViewHolder to find and hold these view references, and 
                // register OnClick with the view holder:
                CostsViewHolder cvh = new CostsViewHolder(itemView, OnClick);
                return cvh;
            }

            // Fill in the contents of the Costs card (invoked by the layout manager):
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                CostsViewHolder cvh = holder as CostsViewHolder;
                if (cvh != null && position < utilityCosts.Count)
                {
                    // Set the TextViews in this ViewHolder's CardView 
                    // from this position in the Utility Costs:

                    SmartUtility.AnalysisCostsView item = utilityCosts[position];

                    cvh.SupplierName.Text = item.SUPPLIER_NAME;
                    cvh.TariffName.Text = item.TARIFF_NAME;
                    cvh.MeterType.Text = item.METER_TYPE;
                    cvh.PaymentName.Text = item.PAYMENT_NAME;
                    cvh.TotalAmount.Text = item.TOTAL_AMOUNT;
                }
                return;
            }

            // Return the number of Costs available in Utility Costs:
            public override int ItemCount
            {
                get { return utilityCosts.Count; } // NumTransactions; }
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
        internal class CostsViewHolder : RecyclerView.ViewHolder
        {
            //public ImageView Image { get; private set; }
            internal TextView SupplierName;
            internal TextView TariffName;
            internal TextView MeterType;
            internal TextView PaymentName;
            internal TextView TotalAmount;

            // Get references to the views defined in the CardView layout.
            public CostsViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                // Locate and cache view references:
                SupplierName = itemView.FindViewById<TextView>(Resource.Id.costsSupplierName);
                TariffName = itemView.FindViewById<TextView>(Resource.Id.costsTariffName);
                MeterType = itemView.FindViewById<TextView>(Resource.Id.costsMeterType);
                PaymentName = itemView.FindViewById<TextView>(Resource.Id.costsPaymentName);
                TotalAmount = itemView.FindViewById<TextView>(Resource.Id.costsTotalAmount);
                // Detect user clicks on the item view and report which item
                // was clicked (by layout position) to the listener:
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
        
        public static UtilityCostsFragment NewInstance(UtilityViewModel utilityvm)
        {
            String key = Guid.NewGuid().ToString();
            UtilityViewModelStore.Add(key, utilityvm);
            UtilityCostsFragment fragment = new UtilityCostsFragment();
            Bundle args = new Bundle();
            args.PutString("vm_key", key);
            fragment.Arguments = args;
            return fragment;
        }
    }
}
#endif