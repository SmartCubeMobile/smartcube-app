#if ANDROID
using Android.Views;
using AndroidX.RecyclerView.Widget;
using static SmartCubeMobile.SmartFinance;

namespace SmartCubeMobile
{
    public class FinanceTransactionsFragment : AndroidX.Fragment.App.Fragment
    {
        private FinanceViewModel financeviewmodel;
        public FinanceTransactionsFragment() { }

        public static FinanceTransactionsFragment NewInstance(FinanceViewModel financevm)
        {
            String key = Guid.NewGuid().ToString();
            FinanceViewModelStore.Add(key, financevm);
            FinanceTransactionsFragment fragment = new FinanceTransactionsFragment();
            Bundle args = new Bundle();
            args.PutString("vm_key", key);
            fragment.Arguments = args;
            return fragment;
        }

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
            // Use this to return your custom view for this Fragment            
            View financeTransactionsView = inflater.Inflate(Resource.Layout.FinanceTransactions, container, false);

            RecyclerView transactionsRecyclerView = financeTransactionsView.FindViewById<AndroidX.RecyclerView.Widget.RecyclerView>(Resource.Id.financeTransactions);

            // Layout manager that lays out each card in the RecyclerView:
            // Layout Manager Setup:
            // Use the built-in linear layout manager:
            
            // Plug the layout manager into the RecyclerView:
            transactionsRecyclerView.SetLayoutManager(new AndroidX.RecyclerView.Widget.LinearLayoutManager(financeTransactionsView.Context));
            transactionsRecyclerView.AddItemDecoration(new DividerItemDecoration(financeTransactionsView.Context, LinearLayoutManager.Vertical));

            // Will this work? Yes
            transactionsRecyclerView.HasFixedSize = true;

            // Create an adapter for the RecyclerView, and pass it the
            // data set (the FinanceTransactions) to manage:
            // Plug the adapter into the RecyclerView:
            List<CommonTransactionsView> transactions = financeviewmodel?.FinanceTransactions ?? new List<CommonTransactionsView>();
            transactionsRecyclerView.SetAdapter(new TransactionsAdapter(transactions));
            return financeTransactionsView;
        }
       
        // Adapter to connect the data set (FinanceTransactions) to the RecyclerView: 
        public class TransactionsAdapter : RecyclerView.Adapter
        {
            // Event handler for item clicks:
            internal event EventHandler<int> ItemClick;

            // Underlying data set (a photo album):
            internal List<SmartFinance.CommonTransactionsView> financeTransactions;

            // Load the adapter with the data set (photo album) at construction time:
            internal TransactionsAdapter(List<SmartFinance.CommonTransactionsView> transactions)
            {
                financeTransactions = transactions;
            }

            // Create a new photo CardView (invoked by the layout manager): 
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int position)
            {
                // Inflate the CardView for the transactions:
                View itemview = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FinanceTransactionItem, parent, false);
                // Create a ViewHolder to find and hold these view references, and 
                // register OnClick with the view holder:
                TransactionViewHolder tvh = new TransactionViewHolder(itemview, OnClick);
                return tvh;
            }

            // Fill in the contents of the photo card (invoked by the layout manager):
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                TransactionViewHolder tvh = holder as TransactionViewHolder;

                // Set the ImageView and TextView in this ViewHolder's CardView 
                // from this position in the Transactions View:
                //tvh.Image.SetImageResource(mTransactions[position1].PhotoID);
                tvh.ItemDate.Text = financeTransactions[position].TRANSACTION_DATE.ToString(SmartParametersV2016.ddmmmyyyyFormat);
                tvh.ItemSortCode.Text = financeTransactions[position].SORTCODE;
                tvh.ItemAccountNo.Text = financeTransactions[position].ACCOUNT_NO;
                tvh.ItemDescription.Text = financeTransactions[position].DESCRIPTION;
                tvh.ItemDebits.Text = financeTransactions[position].DEBITS;
                tvh.ItemCredits.Text = financeTransactions[position].CREDITS;
                tvh.ItemBalance.Text = financeTransactions[position].BALANCE_DISPLAY;
                return;
            }

            // Return the number of Transactions available in the Transactions View:
            public override int ItemCount
            {
                get { return financeTransactions.Count; }   // Because of the header
            }

            //public override int GetItemViewType(int position)
            //{
            //    //if (position == 0)
            //    //{
            //    //    return TYPE_HEAD;
            //    //}
            //    //return TYPE_ITEM;
            //    return base.GetItemViewType(position);
            //}

            // Raise an event when the item-click takes place:
            void OnClick(int position)
            {
                if (ItemClick != null)
                    ItemClick(this, position);
            }

            // Implement the ViewHolder pattern: each ViewHolder holds references
            // to the UI components (ImageView and TextView) within the CardView 
            // that is displayed in a row of the RecyclerView:
            public class TransactionViewHolder : RecyclerView.ViewHolder
            {
                //internal ImageView Image;
                internal TextView ItemDate;
                internal TextView ItemSortCode;
                internal TextView ItemAccountNo;
                internal TextView ItemDescription;
                internal TextView ItemDebits;
                internal TextView ItemCredits;
                internal TextView ItemBalance;

                // Get references to the views defined in the CardView layout.
                public TransactionViewHolder(View itemView, Action<int> listener) : base(itemView)
                {
                    // Locate and cache view references:
                    //Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
                    ItemDate = itemView.FindViewById<TextView>(Resource.Id.transactionDate);
                    ItemSortCode = itemView.FindViewById<TextView>(Resource.Id.transactionSortCode);
                    ItemAccountNo = itemView.FindViewById<TextView>(Resource.Id.transactionAccountNo);
                    ItemDescription = itemView.FindViewById<TextView>(Resource.Id.transactionDescription);
                    ItemDebits = itemView.FindViewById<TextView>(Resource.Id.transactionDebits);
                    ItemCredits = itemView.FindViewById<TextView>(Resource.Id.transactionCredits);
                    ItemBalance = itemView.FindViewById<TextView>(Resource.Id.transactionBalance);

                    // Detect user clicks on the item view and report which item
                    // was clicked (by layout position) to the listener:
                    itemView.Click += (s, e) => listener(base.LayoutPosition);
                }
            }
        }
    }
}
#endif