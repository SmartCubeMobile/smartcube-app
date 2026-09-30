#if ANDROIDX
using Android.Views;
using AndroidX.RecyclerView.Widget;
using static SmartCubeMobile.UtilityBillsFragment;

namespace SmartCubeMobile
{
    public class UtilityReadingsFragment : AndroidX.Fragment.App.Fragment
    {
        private UtilityViewModel utilityviewmodel;
        public UtilityReadingsFragment() { }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment            
            View view = inflater.Inflate(Resource.Layout.UtilityReadings, container, false);
            if (utilityviewmodel == null)
            {
                return view;
            }
            RecyclerView readingsRecyclerView = view.FindViewById<AndroidX.RecyclerView.Widget.RecyclerView>(Resource.Id.utilityReadings);
            if (readingsRecyclerView == null)
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
            readingsRecyclerView.SetLayoutManager(new AndroidX.RecyclerView.Widget.LinearLayoutManager(view.Context));
            readingsRecyclerView.AddItemDecoration(new DividerItemDecoration(view.Context, LinearLayoutManager.Vertical));

            // Will this work? Yes but taken out as list size might change
            //readingsRecyclerView.HasFixedSize = true;

            // Create an adapter for the RecyclerView, and pass it the
            // data set (the FinanceTransactions) to manage:
            // Plug the adapter into the RecyclerView:
            // But only if there is something to plug!
            List<SmartUtility.AnalysisReadingsView> utilityReadings = utilityviewmodel.UtilityReadings ?? new List<SmartUtility.AnalysisReadingsView>();
            readingsRecyclerView.SetAdapter(new ReadingsAdapter(utilityReadings));
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
        // Adapter to connect the data set (UtilityReadings) to the RecyclerView: 
        internal class ReadingsAdapter : RecyclerView.Adapter
        {
            // Event handler for item clicks:
            internal event EventHandler<int> ItemClick;

            // Underlying data set (a photo album):
            internal List<SmartUtility.AnalysisReadingsView> utilityReadings;

            // Load the adapter with the data set (UtilityReadings) at construction time:
            internal ReadingsAdapter(List<SmartUtility.AnalysisReadingsView> readings)
            {
                utilityReadings = readings;
            }

            // Create a new Readings CardView (invoked by the layout manager): 
            public override RecyclerView.ViewHolder
                OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                // Inflate the CardView for the readings:
                View itemView = LayoutInflater.From(parent.Context).
                            Inflate(Resource.Layout.UtilityReadingItem, parent, false);

                // Create a ViewHolder to find and hold these view references, and 
                // register OnClick with the view holder:
                ReadingsViewHolder vh = new ReadingsViewHolder(itemView, OnClick);
                return vh;
            }

            // Fill in the contents of the Utility Readings (invoked by the layout manager):
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                ReadingsViewHolder rvh = holder as ReadingsViewHolder;
                if (rvh != null && position < utilityReadings.Count)
                {
                    // Set the TextViews in this ViewHolder's CardView 
                    // from this position in the Utility Bills:
                    SmartUtility.AnalysisReadingsView item = utilityReadings[position];

                    rvh.AccountNo.Text = item.ACCOUNT_NO;
                    rvh.StatementId.Text = item.STATEMENT_ID;
                    rvh.ReadingsPeriodEnd.Text = item.READINGS_PERIOD_END.ToString(SmartParametersV2016.ddmmmyyyyFormat);
                    rvh.MeterSerialNo.Text = item.METER_SERIAL_NO;
                    rvh.ReadType.Text = item.READ_TYPE;
                    rvh.ThisRead.Text = item.THIS_READ;
                    rvh.LastRead.Text = item.LAST_READ;
                    rvh.UnitsUsed.Text = item.UNITS_USED;
                    rvh.UnitOfMeasure.Text = item.UNIT_OF_MEASURE;
                }
                return;
            }

            // Return the number of Readings available in the Utility Readings:
            public override int ItemCount
            {
                get { return utilityReadings.Count; } // NumReadings; }
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
        internal class ReadingsViewHolder : RecyclerView.ViewHolder
        {
            internal TextView AccountNo;
            internal TextView StatementId;
            internal TextView ReadingsPeriodEnd;
            internal TextView MeterSerialNo;
            internal TextView ReadType;
            internal TextView ThisRead;
            internal TextView LastRead;
            internal TextView UnitsUsed;
            internal TextView UnitOfMeasure;

            // Get references to the views defined in the CardView layout.
            public ReadingsViewHolder(View itemView, Action<int> listener)
                : base(itemView)
            {
                // Locate and cache view references:
                AccountNo = itemView.FindViewById<TextView>(Resource.Id.readingsAccountNo);
                StatementId = itemView.FindViewById<TextView>(Resource.Id.readingsStatementId);
                ReadingsPeriodEnd = itemView.FindViewById<TextView>(Resource.Id.readingsReadingsPeriodEnd);
                MeterSerialNo = itemView.FindViewById<TextView>(Resource.Id.readingsMeterSerialNo);
                ReadType = itemView.FindViewById<TextView>(Resource.Id.readingsReadType);
                ThisRead = itemView.FindViewById<TextView>(Resource.Id.readingsThisRead);
                LastRead = itemView.FindViewById<TextView>(Resource.Id.readingsLastRead);
                UnitsUsed = itemView.FindViewById<TextView>(Resource.Id.readingsUnitsUsed);
                UnitOfMeasure = itemView.FindViewById<TextView>(Resource.Id.readingsUnitOfMeasure);
                // Detect user clicks on the item view and report which item
                // was clicked (by layout position) to the listener:
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
        public static UtilityReadingsFragment NewInstance(UtilityViewModel utilityvm)
        {
            String key = Guid.NewGuid().ToString();
            UtilityViewModelStore.Add(key, utilityvm);
            UtilityReadingsFragment fragment = new UtilityReadingsFragment();
            Bundle args = new Bundle();
            args.PutString("vm_key", key);
            fragment.Arguments = args;
            return fragment;
        }
    }
}
#endif