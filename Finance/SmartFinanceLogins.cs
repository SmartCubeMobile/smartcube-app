using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;

namespace SmartCubeMobile
{
    public class LoginsAdapter : RecyclerView.Adapter
    {
        public List<SmartFinance.Logins> Items { get; }
        public event Action<SmartFinance.Logins> DeleteRequested;
        public event Action<SmartFinance.Logins> Changed;

        private readonly string[] LoginMethods;

        public LoginsAdapter(List<SmartFinance.Logins> items,
            string[] loginMethods)
        {
            Items = items;
            LoginMethods = loginMethods;
        }
        
        public override int ItemCount => Items.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.FinanceLoginsItem, parent, false);

            return new LoginsViewHolder(view, LoginMethods);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is not LoginsViewHolder vh)
                return;

            var rec = Items[position];
            vh.LoginProvider.Text = rec.BrandName ?? string.Empty;

            // -------- LOGIN METHOD SPINNER --------
            vh.LoginMethodSpinner.ItemSelected -= vh.LoginMethodSelected;
            vh.LoginMethodSelected = (s, e) =>
            {
                rec.LoginMethodIndex = e.Position;
                rec.LOGIN_METHOD = e.Position;
                Changed?.Invoke(rec);
            };
            vh.LoginMethodSpinner.SetSelection(rec.LoginMethodIndex, false);
            vh.LoginMethodSpinner.ItemSelected += vh.LoginMethodSelected;

            // -------- DELETE RADIO --------
            vh.DeleteRadiobutton.CheckedChange -= vh.DeleteCheckedChange;
            vh.DeleteRadiobutton.Checked = rec.Delete;
            vh.DeleteCheckedChange = (s, e) =>
            {
                rec.Delete = e.IsChecked;
                DeleteRequested?.Invoke(rec);
            };
            vh.DeleteRadiobutton.CheckedChange += vh.DeleteCheckedChange;
        }

        public void Remove(SmartFinance.Logins rec)
        {
            int index = Items.IndexOf(rec);
            if (index >= 0)
            {
                Items.RemoveAt(index);
                NotifyItemRemoved(index);
            }
        }
    }

    public class LoginsViewHolder : RecyclerView.ViewHolder
    {
        public TextView LoginProvider { get; }
        public Spinner LoginMethodSpinner { get; }
        public RadioButton DeleteRadiobutton { get; }

        public EventHandler<AdapterView.ItemSelectedEventArgs> LoginMethodSelected;
        public EventHandler<CompoundButton.CheckedChangeEventArgs> DeleteCheckedChange;

        public LoginsViewHolder(View itemView, string[] loginMethods)
            : base(itemView)
        {
            LoginProvider =
                itemView.FindViewById<TextView>(Resource.Id.LoginLed1);

            LoginMethodSpinner =
                itemView.FindViewById<Spinner>(Resource.Id.LoginLed2);
            DeleteRadiobutton =
                itemView.FindViewById<RadioButton>(Resource.Id.LoginLed3);

            // Login method adapter (created ONCE)
            var loginAdapter = new ArrayAdapter<string>(
                itemView.Context,
                Android.Resource.Layout.SimpleSpinnerItem,
                loginMethods);
            loginAdapter.SetDropDownViewResource(
                Android.Resource.Layout.SimpleSpinnerDropDownItem);

            LoginMethodSpinner.Adapter = loginAdapter;
        }
    }
}
