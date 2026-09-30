#if ANDROIDX
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace SmartCubeMobile
{
    public class CubefacesAdapter : RecyclerView.Adapter
    {
        public List<CubefaceRecord> Items { get; }

        public event Action<CubefaceRecord> CheckboxChanged;

        public CubefacesAdapter(List<CubefaceRecord> items)
        {
            Items = items ?? new List<CubefaceRecord>();
        }

        public override int ItemCount => Items.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.ProfileCubefacesItem, parent, false);

            return new CubefacesViewHolder(view, OnCheckboxChanged);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as CubefacesViewHolder;
            vh?.Bind(Items[position]);
        }

        void OnCheckboxChanged(CubefaceRecord rec)
        {
            CheckboxChanged?.Invoke(rec);
        }
    }

    public class CubefacesViewHolder : RecyclerView.ViewHolder
    {
        readonly TextView etCubefaceName;
        readonly CheckBox cbActive;

        readonly Action<CubefaceRecord> checkboxCallback;

        CubefaceRecord record;
        bool isBinding;

        public CubefacesViewHolder(View itemView, Action<CubefaceRecord> callback)
            : base(itemView)
        {
            checkboxCallback = callback;

            etCubefaceName = itemView.FindViewById<TextView>(Resource.Id.cubefaceName);
            cbActive = itemView.FindViewById<CheckBox>(Resource.Id.cubefaceActiveFlag);
        }

        public void Bind(CubefaceRecord rec)
        {
            record = rec;

            isBinding = true;

            etCubefaceName.Text = rec.CUBEFACENAME;

            cbActive.CheckedChange -= OnCheckedChanged;
            cbActive.Checked = rec.FACE_ACTIVE;
            cbActive.CheckedChange += OnCheckedChanged;

            isBinding = false;
        }

        void OnCheckedChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (isBinding || record == null)
                return;

            record.FACE_ACTIVE = e.IsChecked;

            checkboxCallback?.Invoke(record);
        }
    }
}
#endif