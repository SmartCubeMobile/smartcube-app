using System;
using System.Collections.Generic;
using System.Linq;
#if ANDROIDX
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;

namespace SmartCubeMobile
{
    // ---------------- DIFF CALLBACK ----------------
    public class GroupDiffCallback : DiffUtil.Callback
    {
        readonly List<GroupRecord> oldList;
        readonly List<GroupRecord> newList;

        public GroupDiffCallback(List<GroupRecord> oldList, List<GroupRecord> newList)
        {
            this.oldList = oldList;
            this.newList = newList;
        }

        public override int OldListSize => oldList.Count;
        public override int NewListSize => newList.Count;

        public override bool AreItemsTheSame(int oldItemPosition, int newItemPosition)
            => oldList[oldItemPosition].Id == newList[newItemPosition].Id;

        public override bool AreContentsTheSame(int oldItemPosition, int newItemPosition)
        {
            var oldGroup = oldList[oldItemPosition];
            var newGroup = newList[newItemPosition];

            return oldGroup.GROUPNAME == newGroup.GROUPNAME
                && oldGroup.ACTIVEFLAG == newGroup.ACTIVEFLAG
                && oldGroup.SENDF == newGroup.SENDF
                && oldGroup.SENDU == newGroup.SENDU
                && oldGroup.RECEIVEALL == newGroup.RECEIVEALL
                && oldGroup.DISPLAYNAME == newGroup.DISPLAYNAME;
        }
    }

    // ---------------- ADAPTER ----------------
    //public class GroupsAdapter : RecyclerView.Adapter
    //{
    //    public List<GroupRecord> Items { get; }
    //    private List<GroupRecord> Snapshot { get; }

    //    public event Action<GroupRecord, GroupRecord> Committed;
    //    public event Action<GroupRecord> DeleteRequested;
    //    public event Action<GroupRecord> CheckboxChanged;

    //    public GroupsAdapter(List<GroupRecord> items)
    //    {
    //        Items = items ?? new List<GroupRecord>();

    //        Snapshot = Items.Select(r => r.ShallowCopy()).ToList();

    //        Committed += Commit;
    //    }

    //    public override int ItemCount => Items.Count;

    //    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    //    {
    //        var view = LayoutInflater.From(parent.Context)
    //            .Inflate(Resource.Layout.ProfileGroupsItem, parent, false);

    //        return new GroupViewHolder(
    //            view,
    //            RaiseCommitted,
    //            RaiseDeleteRequested,
    //            RaiseCheckboxChanged
    //        );
    //    }

    //    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    //    {
    //        ((GroupViewHolder)holder).Bind(Items[position]);
    //    }

    //    // ---------------- SNAPSHOT COMMIT ----------------
    //    private void Commit(GroupRecord oldRecord, GroupRecord newRecord)
    //    {
    //        int index = Items.IndexOf(newRecord);
    //        if (index < 0) return;

    //        if (index < Snapshot.Count)
    //            Snapshot[index] = oldRecord.ShallowCopy();
    //        else
    //            Snapshot.Add(oldRecord.ShallowCopy());

    //        var diff = DiffUtil.CalculateDiff(
    //            new GroupDiffCallback(Snapshot, Items)
    //        );

    //        diff.DispatchUpdatesTo(this);
    //    }

    //    public void RemoveItem(GroupRecord rec)
    //    {
    //        int index = Items.IndexOf(rec);

    //        if (index >= 0)
    //        {
    //            Items.RemoveAt(index);
    //            Snapshot.RemoveAt(index);

    //            NotifyItemRemoved(index);
    //        }
    //    }

    //    public void CleanupEmptyRows(GroupRecord except = null)
    //    {
    //        var emptyRows = Items
    //            .Where(r => r.IsNew && string.IsNullOrWhiteSpace(r.GROUPNAME) && r != except)
    //            .ToList();

    //        foreach (var row in emptyRows)
    //            RemoveItem(row);
    //    }

    //    public void RaiseDeleteRequested(GroupRecord rec)
    //        => DeleteRequested?.Invoke(rec);

    //    public void RaiseCommitted(GroupRecord oldRecord, GroupRecord newRecord)
    //        => Committed?.Invoke(oldRecord, newRecord);

    //    public void RaiseCheckboxChanged(GroupRecord rec)
    //        => CheckboxChanged?.Invoke(rec);
    //}

    //// ---------------- VIEW HOLDER ----------------
    //public class GroupViewHolder : RecyclerView.ViewHolder
    //{
    //    readonly EditText GroupName;
    //    readonly EditText Display;
    //    readonly CheckBox Active, Finance, Utility, Receive;
    //    readonly RadioButton btnDelete;

    //    readonly Action<GroupRecord, GroupRecord> committedCallback;
    //    readonly Action<GroupRecord> deleteCallback;
    //    readonly Action<GroupRecord> checkboxCallback;

    //    GroupRecord record;
    //    bool isBinding;

    //    public GroupViewHolder(
    //        View itemView,
    //        Action<GroupRecord, GroupRecord> committed,
    //        Action<GroupRecord> delete,
    //        Action<GroupRecord> checkbox) : base(itemView)
    //    {
    //        committedCallback = committed;
    //        deleteCallback = delete;
    //        checkboxCallback = checkbox;

    //        GroupName = itemView.FindViewById<EditText>(Resource.Id.groupsGroupName);
    //        Display = itemView.FindViewById<EditText>(Resource.Id.groupsDisplayName);
    //        Active = itemView.FindViewById<CheckBox>(Resource.Id.groupsActiveFlag);
    //        Finance = itemView.FindViewById<CheckBox>(Resource.Id.groupsFinance);
    //        Utility = itemView.FindViewById<CheckBox>(Resource.Id.groupsUtility);
    //        Receive = itemView.FindViewById<CheckBox>(Resource.Id.groupsReceiveAll);
    //        btnDelete = itemView.FindViewById<RadioButton>(Resource.Id.groupsDelete);

    //        // ---- TEXT CHANGES ----
    //        GroupName.AfterTextChanged += (s, e) =>
    //        {
    //            if (isBinding) return;

    //            if (string.IsNullOrWhiteSpace(GroupName.Text))
    //            {
    //                deleteCallback?.Invoke(record);
    //                return;
    //            }

    //            bool wasEmpty = string.IsNullOrWhiteSpace(record.GROUPNAME);

    //            var oldRecord = record.ShallowCopy();

    //            record.GROUPNAME = GroupName.Text;

    //            if (record.IsNew && wasEmpty)
    //            {
    //                record.IsNew = false;
    //                record.ChangeType = ChangeTypeEnum.Added;
    //                SetFieldsEnabled(true);
    //            }
    //            else
    //            {
    //                record.ChangeType = ChangeTypeEnum.Modified;
    //            }

    //            committedCallback?.Invoke(oldRecord, record);
    //        };

    //        Display.AfterTextChanged += (s, e) =>
    //        {
    //            if (isBinding || record.IsNew) return;

    //            var oldRecord = record.ShallowCopy();

    //            record.DISPLAYNAME = Display.Text;
    //            record.ChangeType = ChangeTypeEnum.Modified;

    //            committedCallback?.Invoke(oldRecord, record);
    //        };

    //        Active.CheckedChange += (s, e) => OnCheckboxChanged();
    //        Finance.CheckedChange += (s, e) => OnCheckboxChanged();
    //        Utility.CheckedChange += (s, e) => OnCheckboxChanged();
    //        Receive.CheckedChange += (s, e) => OnCheckboxChanged();

    //        btnDelete.Click += (s, e) => deleteCallback?.Invoke(record);
    //    }

    //    public void Bind(GroupRecord rec)
    //    {
    //        record = rec;

    //        isBinding = true;

    //        GroupName.Text = rec.GROUPNAME;
    //        Display.Text = rec.DISPLAYNAME;

    //        Active.Checked = rec.ACTIVEFLAG;
    //        Finance.Checked = rec.SENDF;
    //        Utility.Checked = rec.SENDU;
    //        Receive.Checked = rec.RECEIVEALL;

    //        btnDelete.Checked = false;

    //        if (rec.IsNew && string.IsNullOrWhiteSpace(rec.GROUPNAME))
    //        {
    //            SetFieldsEnabled(false);

    //            GroupName.Enabled = true;
    //            btnDelete.Enabled = true;

    //            GroupName.RequestFocus();
    //            GroupName.Post(() => GroupName.SetSelection(GroupName.Text.Length));
    //        }
    //        else
    //        {
    //            SetFieldsEnabled(true);

    //            GroupName.Enabled = true;
    //            btnDelete.Enabled = true;
    //        }

    //        isBinding = false;
    //    }

    //    void OnCheckboxChanged()
    //    {
    //        if (isBinding || record == null || record.IsNew)
    //            return;

    //        var oldRecord = record.ShallowCopy();

    //        record.ACTIVEFLAG = Active.Checked;
    //        record.SENDF = Finance.Checked;
    //        record.SENDU = Utility.Checked;
    //        record.RECEIVEALL = Receive.Checked;
    //        record.ChangeType = ChangeTypeEnum.Modified;

    //        committedCallback?.Invoke(oldRecord, record);

    //        checkboxCallback?.Invoke(record);
    //    }

    //    void SetFieldsEnabled(bool enabled)
    //    {
    //        Display.Enabled = enabled;
    //        Active.Enabled = enabled;
    //        Finance.Enabled = enabled;
    //        Utility.Enabled = enabled;
    //        Receive.Enabled = enabled;
    //    }
    //}

    public class GroupsAdapter : RecyclerView.Adapter
    {
        public List<GroupRecord> Items { get; private set; }

        public event Action<GroupRecord, GroupRecord> Committed;
        public event Action<GroupRecord> DeleteRequested;

        public GroupsAdapter(List<GroupRecord> items)
        {
            Items = items ?? new List<GroupRecord>();
        }

        public override int ItemCount => Items.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context)
                .Inflate(Resource.Layout.ProfileGroupsItem, parent, false);
            return new GroupViewHolder(view, this);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((GroupViewHolder)holder).Bind(Items[position]);
        }

        public void AddItem(GroupRecord rec)
        {
            Items.Add(rec);
            NotifyItemInserted(Items.Count - 1);
        }

        public void RemoveItem(GroupRecord rec)
        {
            int index = Items.IndexOf(rec);
            if (index >= 0)
            {
                Items.RemoveAt(index);
                NotifyItemRemoved(index);
            }
        }

        public void RaiseCommitted(GroupRecord oldRecord, GroupRecord newRecord)
            => Committed?.Invoke(oldRecord, newRecord);

        public void RaiseDeleteRequested(GroupRecord rec)
            => DeleteRequested?.Invoke(rec);
    }

    public class GroupViewHolder : RecyclerView.ViewHolder
    {
        EditText GroupName, Display;
        CheckBox Active, Finance, Utility, Receive;
        RadioButton DeleteButton;
        GroupsAdapter Adapter;

        GroupRecord record;
        bool isBinding;

        public GroupViewHolder(View itemView, GroupsAdapter adapter) : base(itemView)
        {
            Adapter = adapter;

            GroupName = itemView.FindViewById<EditText>(Resource.Id.groupsGroupName);
            Display = itemView.FindViewById<EditText>(Resource.Id.groupsDisplayName);
            Active = itemView.FindViewById<CheckBox>(Resource.Id.groupsActiveFlag);
            Finance = itemView.FindViewById<CheckBox>(Resource.Id.groupsFinance);
            Utility = itemView.FindViewById<CheckBox>(Resource.Id.groupsUtility);
            Receive = itemView.FindViewById<CheckBox>(Resource.Id.groupsReceiveAll);
            DeleteButton = itemView.FindViewById<RadioButton>(Resource.Id.groupsDelete);

            GroupName.AfterTextChanged += (s, e) => OnTextChanged();
            Display.AfterTextChanged += (s, e) => OnTextChanged();
            Active.CheckedChange += (s, e) => OnCheckboxChanged();
            Finance.CheckedChange += (s, e) => OnCheckboxChanged();
            Utility.CheckedChange += (s, e) => OnCheckboxChanged();
            Receive.CheckedChange += (s, e) => OnCheckboxChanged();
            DeleteButton.Click += (s, e) => Adapter.RaiseDeleteRequested(record);
        }

        public void Bind(GroupRecord rec)
        {
            record = rec;
            isBinding = true;

            GroupName.Text = rec.GROUPNAME;
            Display.Text = rec.DISPLAYNAME;
            Active.Checked = rec.ACTIVEFLAG;
            Finance.Checked = rec.SENDF;
            Utility.Checked = rec.SENDU;
            Receive.Checked = rec.RECEIVEALL;

            isBinding = false;
        }

        void OnTextChanged()
        {
            if (isBinding) return;
            var oldRecord = record.ShallowCopy();
            record.GROUPNAME = GroupName.Text;
            record.DISPLAYNAME = Display.Text;
            record.ChangeType = ChangeTypeEnum.Modified;
            Adapter.RaiseCommitted(oldRecord, record);
        }

        void OnCheckboxChanged()
        {
            if (isBinding) return;
            var oldRecord = record.ShallowCopy();
            record.ACTIVEFLAG = Active.Checked;
            record.SENDF = Finance.Checked;
            record.SENDU = Utility.Checked;
            record.RECEIVEALL = Receive.Checked;
            record.ChangeType = ChangeTypeEnum.Modified;
            Adapter.RaiseCommitted(oldRecord, record);
        }
    }
}
#endif