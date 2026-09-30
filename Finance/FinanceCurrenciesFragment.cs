#if ANDROIDX
using Android.Views;
using AndroidX.RecyclerView.Widget;
namespace SmartCubeMobile
{
    // Fragments for each tab? No, they all use the same CurrencyPanel
    public class FinanceCurrenciesFragment(List<SmartFinance.TotalsView>[,] FinanceTotals, int position) : AndroidX.Fragment.App.Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,Bundle savedInstanceState)
        {
            View financeCurrenciesView = inflater.Inflate(Resource.Layout.FinanceCurrencies, container, false);
            RecyclerView currenciesRecyclerView = financeCurrenciesView.FindViewById<RecyclerView>(Resource.Id.financeCurrencies);
            // Layout manager that lays out each card in the RecyclerView:
            // Layout Manager Setup:
            // Use the built-in linear layout manager:
            //currenciesLayoutManager = new GridLayoutManager(mmviewmodel.OurActivity, 1, GridLayoutManager.Vertical, false);//  //(this);
            // Or use the built-in grid layout manager (two horizontal rows):
            // mLayoutManager = new GridLayoutManager
            //        (this, 2, GridLayoutManager.Horizontal, false);
            // Plug the layout manager into the RecyclerView:
            currenciesRecyclerView.SetLayoutManager(new AndroidX.RecyclerView.Widget.LinearLayoutManager(financeCurrenciesView.Context));
            currenciesRecyclerView.AddItemDecoration(new DividerItemDecoration(financeCurrenciesView.Context, LinearLayoutManager.Vertical));
            // Will this work? Yes
            currenciesRecyclerView.HasFixedSize = true;
            // Create an adapter for the RecyclerView, and pass it the
            // data set (the FinanceCurrencies) to manage:
            // Plug the adapter into the RecyclerView:
            List<SmartFinance.TotalsView> targetCurrencies = new List<SmartFinance.TotalsView>();
            // I'm sure there is a much more efficient and subtle way of
            // setting targetCurrencies up, but as of right now,
            // I'm SO TIRED of all this bollocks I just can't be arsed ..
            for (int i = 0; i < 3; i++) // PaidIn + PaidOut + Difference = 3
            {
                List<SmartFinance.TotalsView> xyz = FinanceTotals[position, i];
                foreach (SmartFinance.TotalsView tview in xyz)
                {
                    targetCurrencies.Add(tview);
                }
            }
            currenciesRecyclerView.SetAdapter(new CurrenciesAdapter(targetCurrencies));
            return financeCurrenciesView;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        // Adapter to connect the data set (FinanceCurrencies) to the RecyclerView:
        // With acknowledgement to some ginger ...
        // ... whose piece of shit didn't fucking scroll!!!
        public class CurrenciesAdapter : RecyclerView.Adapter
        {
            // Event handler for item clicks:
            internal event EventHandler<int> ItemClick;
            // Underlying data set (a photo album):
            internal List<SmartFinance.TotalsView> financeCurrencies;
            // Load the adapter with the data set (photo album) at construction time:
            internal CurrenciesAdapter(List<SmartFinance.TotalsView> currencies)
            {
                financeCurrencies = currencies;
            }
            // Create a new photo CardView (invoked by the layout manager):
            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int position)
            {
                // Inflate the CardView for the currencies:
                View itemview = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.FinanceCurrencyItem, parent, false);
                // Create a ViewHolder to find and hold these view references, and
                // register OnClick with the view holder:
                CurrencyViewHolder tvh = new CurrencyViewHolder(itemview, OnClick);
                return tvh;
            }
            // Fill in the contents of the photo card (invoked by the layout manager):
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                CurrencyViewHolder tvh = holder as CurrencyViewHolder;
                // Set the ImageView and TextView in this ViewHolder's CardView
                // from this position in the Currencies View:
                //tvh.Image.SetImageResource(mCurrenciess[position1].PhotoID);
                if (financeCurrencies.Count == 3)
                {
                    SmartFinance.TotalsView Money = financeCurrencies[0];
                    tvh.PaidInDescription.Text = Money.DESCRIPTION;
                    tvh.PaidInAmount.Text = Money.AMOUNT;
                    Money = financeCurrencies[1];
                    tvh.PaidOutDescription.Text = Money.DESCRIPTION;
                    tvh.PaidOutAmount.Text = Money.AMOUNT;
                    Money = financeCurrencies[2];
                    tvh.DifferenceDescription.Text = Money.DESCRIPTION;
                    tvh.DifferenceAmount.Text = Money.AMOUNT;
                }
                return;
            }
            // Return the number of Currencies available in the Currencies View:
            public override int ItemCount
            {
                // Return the number of times we want to loop through the data! ONCE!!
                get { return 1; }   // Because of the header
            }

            // Raise an event when the item-click takes place:
            void OnClick(int position)
            {
                if (ItemClick != null)
                    ItemClick(this, position);
            }
            // Implement the ViewHolder pattern: each ViewHolder holds references
            // to the UI components (ImageView and TextView) within the CardView
            // that is displayed in a row of the RecyclerView:
            public class CurrencyViewHolder : RecyclerView.ViewHolder
            {
                internal TextView PaidInDescription;
                internal TextView PaidInAmount;
                internal TextView PaidOutDescription;
                internal TextView PaidOutAmount;
                internal TextView DifferenceDescription;
                internal TextView DifferenceAmount;
                // Get references to the views defined in the CardView layout.
                public CurrencyViewHolder(View itemView, Action<int> listener) : base(itemView)
                {
                    // Locate and cache view references:
                    //Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
                    PaidInDescription = itemView.FindViewById<TextView>(Resource.Id.PaidInDescription);
                    PaidInAmount = itemView.FindViewById<TextView>(Resource.Id.PaidInAmount);
                    PaidOutDescription = itemView.FindViewById<TextView>(Resource.Id.PaidOutDescription);
                    PaidOutAmount = itemView.FindViewById<TextView>(Resource.Id.PaidOutAmount);
                    DifferenceDescription = itemView.FindViewById<TextView>(Resource.Id.DifferenceDescription);
                    DifferenceAmount = itemView.FindViewById<TextView>(Resource.Id.DifferenceAmount);

                    // Detect user clicks on the item view and report which item
                    // was clicked (by layout position) to the listener:
                    //itemView.Click += (sender, e) => listener(base.LayoutPosition);
                }
            }
        }
    }
}
#endif