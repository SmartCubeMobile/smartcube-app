#if ANDROIDX
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Org.Apache.Http.Impl.Cookie;
using static SmartCubeMobile.UtilityCostsFragment;

namespace SmartCubeMobile
{
    public class UtilityBillsFragment : AndroidX.Fragment.App.Fragment
    {
        private UtilityViewModel utilityviewmodel;
        public UtilityBillsFragment() { }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment            
            View view = inflater.Inflate(Resource.Layout.UtilityBills, container, false);
            if (utilityviewmodel == null)
            {
                return view;
            }
            RecyclerView billsRecyclerView = view.FindViewById<AndroidX.RecyclerView.Widget.RecyclerView>(Resource.Id.utilityBills);
            if (billsRecyclerView == null)
            {
                return view;
            }
            // Layout manager that lays out each card in the RecyclerView:
            // Layout Manager Setup:
            // Use the built-in linear layout manager:

            //billsLayoutManager = new GridLayoutManager(MainActivity.mmviewmodel.OurActivity, 1, GridLayoutManager.Vertical, false);

            // Or use the built-in grid layout manager (two horizontal rows):
            // mLayoutManager = new GridLayoutManager
            //        (this, 2, GridLayoutManager.Horizontal, false);

            // Plug the layout manager into the RecyclerView:
            billsRecyclerView.SetLayoutManager(new AndroidX.RecyclerView.Widget.LinearLayoutManager(view.Context));
            billsRecyclerView.AddItemDecoration(new DividerItemDecoration(view.Context, LinearLayoutManager.Vertical));

            // Will this work? Yes but taken out as list size might change
            //billsRecyclerView.HasFixedSize = true;

            // Create an adapter for the RecyclerView, and pass it the
            // data set (the FinanceTransactions) to manage:
            // Plug the adapter into the RecyclerView:
            // But only if there is something to plug!
            List<SmartUtility.AnalysisBillsView> utilityBills = utilityviewmodel.UtilityBills ?? new List<SmartUtility.AnalysisBillsView>();
            billsRecyclerView.SetAdapter(new BillsAdapter(utilityBills));
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

        // Adapter to connect the data set (UtilityBills) to the RecyclerView: 

        // With acknowledgement to some ginger ...
        // ... whose piece of shit didn't fucking scroll!!!
        // Adapter to connect the data set (UtilityBills) to the RecyclerView: 
        internal class BillsAdapter : RecyclerView.Adapter
        {
            // Event handler for item clicks:
            internal event EventHandler<int> ItemClick;

            // Underlying data set (a photo album):
            internal List<SmartUtility.AnalysisBillsView> utilityBills;

            // Load the adapter with the data set (UtilityBills) at construction time:
            internal BillsAdapter(List<SmartUtility.AnalysisBillsView> bills)
            {
                utilityBills = bills;
            }

            // Create a new bills CardView (invoked by the layout manager): 
            public override RecyclerView.ViewHolder
                OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                // Inflate the CardView for the bills:
                View itemView = LayoutInflater.From(parent.Context).
                            Inflate(Resource.Layout.UtilityBillItem, parent, false);

                // Create a ViewHolder to find and hold these view references, and 
                // register OnClick with the view holder:
                BillsViewHolder vh = new BillsViewHolder(itemView, OnClick);
                return vh;
            }

            // Fill in the contents of the Utility Bills (invoked by the layout manager):
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                BillsViewHolder bvh = holder as BillsViewHolder;
                if (bvh != null && position < utilityBills.Count)
                {
                    // Set the TextViews in this ViewHolder's CardView 
                    // from this position in the Utility Bills:

                    SmartUtility.AnalysisBillsView item = utilityBills[position];

                    bvh.AccountNo.Text = item.ACCOUNT_NO;
                    bvh.BillDate.Text = item.BILL_DATE.ToString(SmartParametersV2016.ddmmmyyyyFormat);
                    bvh.StatementId.Text = item.STATEMENT_ID;
                    bvh.Date.Text = item.DATE.ToString(SmartParametersV2016.ddmmmyyyyFormat);
                    bvh.Code.Text = item.CODE;
                    bvh.Description.Text = item.DESCRIPTION;
                    bvh.Amount.Text = item.AMOUNT;
                    bvh.Balance.Text = item.BALANCE;
                }
                return;
            }

            // Return the number of Bills available in the Utility Bills:
            public override int ItemCount
            {
                get { return utilityBills.Count; } // NumTransactions; }
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
        internal class BillsViewHolder : RecyclerView.ViewHolder
        {
            internal TextView AccountNo;
            internal TextView BillDate;
            internal TextView StatementId;
            internal TextView Date;
            internal TextView Code;
            internal TextView Description;
            internal TextView Amount;
            internal TextView Balance;

            // Get references to the views defined in the CardView layout.
            public BillsViewHolder(View itemView, Action<int> listener)
                : base(itemView)
            {
                // Locate and cache view references:
                AccountNo = itemView.FindViewById<TextView>(Resource.Id.billsAccountNo);
                BillDate = itemView.FindViewById<TextView>(Resource.Id.billsBillDate);
                StatementId = itemView.FindViewById<TextView>(Resource.Id.billsStatementId);
                Date = itemView.FindViewById<TextView>(Resource.Id.billsDate);
                Code = itemView.FindViewById<TextView>(Resource.Id.billsCode);
                Description = itemView.FindViewById<TextView>(Resource.Id.billsDescription);
                Amount = itemView.FindViewById<TextView>(Resource.Id.billsAmount);
                Balance = itemView.FindViewById<TextView>(Resource.Id.billsBalance);
                // Detect user clicks on the item view and report which item
                // was clicked (by layout position) to the listener:
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
        public static UtilityBillsFragment NewInstance(UtilityViewModel utilityvm)
        {
            String key = Guid.NewGuid().ToString();
            UtilityViewModelStore.Add(key, utilityvm);
            UtilityBillsFragment fragment = new UtilityBillsFragment();
            Bundle args = new Bundle();
            args.PutString("vm_key", key);
            fragment.Arguments = args;
            return fragment;
        }
    }
}
#endif