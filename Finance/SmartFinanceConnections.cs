using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace SmartCubeMobile
{
    //[Activity(
    //    Label = "SmartCubeMobile",
    //    Exported = true
    //)]
    //public class FinanceConnectionsLoginsActivity : Activity
    //{
        //    RecyclerView recycler;
        //    Button btnAdd;

        //    List<SmartFinance.Connections> connections;
        //    ConnectionsAdapter adapter;

        //    protected override void OnCreate(Bundle savedInstanceState)
        //    {
        //        base.OnCreate(savedInstanceState);

        //        SetContentView(Resource.Layout.FinanceConnectionsLogins);

        //        SetFinishOnTouchOutside(false);

        //        var metrics = Resources.DisplayMetrics;
        //        int width = (int)(metrics.WidthPixels * 0.75);
        //        int height = (int)(metrics.HeightPixels * 0.75);
        //        Window.SetLayout(width, height);
        //        Window.SetGravity(GravityFlags.Center);

        //        recycler = FindViewById<RecyclerView>(Resource.Id.financeConnections);
        //        btnAdd = FindViewById<Button>(Resource.Id.connectionsAddButton);

        //        connections = AppState.financeviewmodel.ConnectionsViewList
        //            .Select(c =>
        //            {
        //                c.IsNew = false;
        //                c.AvailableLoginMethods = null;
        //                return c;
        //            })
        //            .ToList();

        //        var providers = connections
        //            .Where(c => !string.IsNullOrWhiteSpace(c.BrandName))
        //            .Select(c => c.BrandName)
        //            .Distinct()
        //            .OrderBy(x => x)
        //            .ToArray();

        //        adapter = new ConnectionsAdapter(connections, providers);

        //        recycler.SetLayoutManager(new LinearLayoutManager(this));
        //        recycler.SetAdapter(adapter);

        //        btnAdd.Click += (s, e) =>
        //        {
        //            if (connections.Any(c => c.IsNew))
        //                return;

        //            var freeProviders = AppState.financeviewmodel.ConnectionsViewList
        //                .GroupBy(c => c.BrandName)
        //                .Where(g => g.Any(x => x.LOGIN_METHOD == 0))
        //                .Select(g => g.Key)
        //                .ToList();

        //            var newRec = new Connections
        //            {
        //                IsNew = true,
        //                ProviderIndex = -1,
        //                LoginMethodIndex = -1,
        //                HasSingleProvider = freeProviders.Count == 1,
        //                SingleProviderName = freeProviders.Count == 1 ? freeProviders[0] : null
        //            };

        //            connections.Add(newRec);
        //            adapter.NotifyItemInserted(connections.Count - 1);
        //            recycler.ScrollToPosition(connections.Count - 1);
        //        };
        //    }
        //}

        //// ======================== ADAPTER ========================
        //public class ConnectionsAdapter : RecyclerView.Adapter
        //{
        //    public List<Connections> Items { get; }
        //    private readonly string[] AllProviders;

        //    public ConnectionsAdapter(List<Connections> items, string[] allProviders)
        //    {
        //        Items = items;
        //        AllProviders = allProviders;
        //    }

        //    public override int ItemCount => Items.Count;

        //    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        //    {
        //        var view = LayoutInflater.From(parent.Context)
        //            .Inflate(Resource.Layout.FinanceConnectionsItem, parent, false);

        //        return new ConnectionsViewHolder(view);
        //    }

        //    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        //    {
        //        var vh = (ConnectionsViewHolder)holder;
        //        var rec = Items[position];

        //        // Hide everything initially
        //        vh.ProviderAuto.Visibility = ViewStates.Gone;
        //        vh.ProviderText.Visibility = ViewStates.Gone;
        //        vh.LoginMethodSpinner.Visibility = ViewStates.Gone;

        //        // ---------- EXISTING ----------
        //        if (!rec.IsNew)
        //        {
        //            vh.ProviderText.Visibility = ViewStates.Visible;
        //            vh.ProviderText.Text = rec.BrandName;
        //            return;
        //        }

        //        // ---------- NEW ----------
        //        if (rec.HasSingleProvider)
        //        {
        //            vh.ProviderText.Visibility = ViewStates.Visible;
        //            vh.ProviderText.Text = rec.SingleProviderName;

        //            rec.AvailableLoginMethods = GetLoginMethodsForProvider(rec.SingleProviderName);
        //        }
        //        else
        //        {
        //            vh.ProviderAuto.Visibility = ViewStates.Visible;

        //            var adapter = new ArrayAdapter<string>(
        //                vh.ItemView.Context,
        //                Android.Resource.Layout.SimpleDropDownItem1Line,
        //                AllProviders);

        //            vh.ProviderAuto.Adapter = adapter;

        //            if (rec.ProviderIndex >= 0)
        //                vh.ProviderAuto.Text = AllProviders[rec.ProviderIndex];

        //            vh.ProviderAuto.ItemClick -= vh.ProviderSelected;
        //            vh.ProviderSelected = (s, e) =>
        //            {
        //                var selectedProvider = vh.ProviderAuto.Text;
        //                rec.ProviderIndex = Array.IndexOf(AllProviders, selectedProvider);
        //                rec.AvailableLoginMethods = GetLoginMethodsForProvider(selectedProvider);
        //                rec.LoginMethodIndex = -1;

        //                NotifyItemChanged(position);
        //            };
        //            vh.ProviderAuto.ItemClick += vh.ProviderSelected;
        //        }

        //        if (rec.AvailableLoginMethods != null && rec.AvailableLoginMethods.Count > 0)
        //        {
        //            vh.LoginMethodSpinner.Visibility = ViewStates.Visible;

        //            var loginAdapter = new ArrayAdapter<int>(
        //                vh.ItemView.Context,
        //                Android.Resource.Layout.SimpleSpinnerItem,
        //                rec.AvailableLoginMethods);

        //            loginAdapter.SetDropDownViewResource(
        //                Android.Resource.Layout.SimpleSpinnerDropDownItem);

        //            vh.LoginMethodSpinner.Adapter = loginAdapter;

        //            if (rec.LoginMethodIndex >= 0)
        //                vh.LoginMethodSpinner.SetSelection(rec.LoginMethodIndex, false);

        //            vh.LoginMethodSpinner.ItemSelected -= vh.LoginMethodSelected;
        //            vh.LoginMethodSelected = (s, e) =>
        //            {
        //                rec.LoginMethodIndex = e.Position;
        //                rec.LOGIN_METHOD = rec.AvailableLoginMethods[e.Position];
        //            };
        //            vh.LoginMethodSpinner.ItemSelected += vh.LoginMethodSelected;
        //        }
        //    }

        //    private List<int> GetLoginMethodsForProvider(string providerName)
        //    {
        //        return Items
        //            .Where(c => c.BrandName == providerName)
        //            .Select(c => c.LOGIN_METHOD)
        //            .Distinct()
        //            .OrderBy(x => x)
        //            .ToList();
        //    }
        //}

        //// ======================== VIEWHOLDER ========================
        //public class ConnectionsViewHolder : RecyclerView.ViewHolder
        //{
        //    public AutoCompleteTextView ProviderAuto { get; }
        //    public TextView ProviderText { get; }
        //    public Spinner LoginMethodSpinner { get; }

        //    public EventHandler<AdapterView.ItemClickEventArgs> ProviderSelected;
        //    public EventHandler<AdapterView.ItemSelectedEventArgs> LoginMethodSelected;

        //    public ConnectionsViewHolder(View itemView) : base(itemView)
        //    {
        //        ProviderAuto = itemView.FindViewById<AutoCompleteTextView>(Resource.Id.connectionsProviderAuto);
        //        ProviderText = itemView.FindViewById<TextView>(Resource.Id.connectionsProviderText);
        //        LoginMethodSpinner = itemView.FindViewById<Spinner>(Resource.Id.connectionsLoginMethod);
        //    }
        //
    //}
}
