// 08-Mar-2026 - Everything works!! PS => MES
using System.Linq;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using SmartCubeMobile;

namespace SmartCubeMobile
{
    public class FinanceConnectionsFragment : AndroidX.Fragment.App.DialogFragment
    {
        public SignInViewModel signinviewmodel;
        public MainViewModel ourviewmodel;
        public FinanceViewModel financeviewmodel;

        public short InstitutionCode;
        public string InstitutionName;
        public TextView institutionName;

        private RecyclerView connectionRecyclerView;
        private ConnectionBrandAdapter connectionAdapter;
        private Button ConnectionAdd;
        private RecyclerView loginRecyclerView;
        private LoginBrandAdapter loginAdapter;
        private Button LoginAdd;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.FinanceConnectionsLogins, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            institutionName = view.FindViewById<TextView>(Resource.Id.institutionName);
            institutionName.Text = InstitutionName;

            connectionRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.connectionsRecyclerView);
            ConnectionAdd = view.FindViewById<Button>(Resource.Id.connectionsAddButton);
            loginRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.loginsRecyclerView);
            LoginAdd = view.FindViewById<Button>(Resource.Id.loginsAddButton);

            // LOGINS RECYCLER
            loginRecyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            loginRecyclerView.SetItemAnimator(null);

            loginAdapter = new LoginBrandAdapter(Context, financeviewmodel.CreatedBrands, financeviewmodel.LoginsViewList, InstitutionCode, financeviewmodel.ConnectionsViewList, signinviewmodel, ourviewmodel, financeviewmodel);
            loginRecyclerView.SetAdapter(loginAdapter);

            LoginAdd.Click += async (s, e) => await AddLogin_Click(signinviewmodel, ourviewmodel, financeviewmodel);

            // CONNECTIONS RECYCLER
            connectionRecyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            connectionRecyclerView.SetItemAnimator(null);

            connectionAdapter = new ConnectionBrandAdapter(Context, financeviewmodel.CreatedBrands, financeviewmodel.ConnectionsViewList, financeviewmodel.LoginsViewList, InstitutionCode, loginAdapter, signinviewmodel, ourviewmodel, financeviewmodel);
            connectionRecyclerView.SetAdapter(connectionAdapter);

            ConnectionAdd.Click += async (s, e) => await AddConnectionCoreAsyncWrapper(signinviewmodel, ourviewmodel, financeviewmodel, InstitutionCode, InstitutionName, Context);
        }

        public override void OnStart()
        {
            base.OnStart();
            if (Dialog?.Window != null)
            {
                var metrics = Resources.DisplayMetrics;
                int width = (int)(metrics.WidthPixels * 0.75);
                int height = (int)(metrics.HeightPixels * 0.75);
                Dialog.Window.SetLayout(width, height);
                Dialog.Window.SetGravity(GravityFlags.Center);
            }
        }

        private async Task AddConnectionCoreAsyncWrapper(SignInViewModel signinviewmodel, MainViewModel ourviewmodel, FinanceViewModel financeviewmodel, short DefaultInstitutionCode, string DefaultInstitutionName, Context context)
        {
            SmartFinance.Connections connection = await SmartFinanceV2025.AddConnectionCoreAsync(
                signinviewmodel, ourviewmodel, financeviewmodel, DefaultInstitutionCode, DefaultInstitutionName, context);

            if (connection != null)
            {
                connectionAdapter.NotifyItemInserted(financeviewmodel.ConnectionsViewList.Count - 1);
                connectionRecyclerView.ScrollToPosition(connectionAdapter.ItemCount - 1);
            }
        }

        private async Task<bool> AddLogin_Click(SignInViewModel signinviewmodel, MainViewModel ourviewmodel, FinanceViewModel financeviewmodel)
        {
            return await SmartFinanceV2025.AddLoginCoreAsync(signinviewmodel, ourviewmodel, financeviewmodel, InstitutionCode, InstitutionName, Context);
                
        }
    }

    // ===================== CONNECTIONS =====================
    public class ConnectionBrandAdapter : RecyclerView.Adapter
    {
        private Context context;
        private List<BrandItem> brandData;
        private List<SmartFinance.Connections> ConnectionsViewList;
        private List<SmartFinance.Logins> LoginsViewList;
        private short defaultInstitutionCode;
        private LoginBrandAdapter loginAdapter;
        private SignInViewModel signinviewmodel;
        private MainViewModel ourviewmodel;
        private FinanceViewModel financeviewmodel;

        public ConnectionBrandAdapter(Context ctx, List<BrandItem> data, List<SmartFinance.Connections> connectionsViewList, List<SmartFinance.Logins> loginsViewList, short institutionCode, LoginBrandAdapter loginAdapterRef, SignInViewModel signinvm, MainViewModel ourviewmodel, FinanceViewModel financeviewmodel)
        {
            context = ctx;
            brandData = data;
            ConnectionsViewList = connectionsViewList;
            LoginsViewList = loginsViewList;
            defaultInstitutionCode = institutionCode;
            loginAdapter = loginAdapterRef;
            this.signinviewmodel = signinvm;
            this.ourviewmodel = ourviewmodel;
            this.financeviewmodel = financeviewmodel;
        }

        public override int ItemCount => ConnectionsViewList.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(context).Inflate(Resource.Layout.FinanceConnectionsRow, parent, false);
            return new ConnectionBrandViewHolder(view, context, brandData, ConnectionsViewList, LoginsViewList, this, defaultInstitutionCode, loginAdapter, signinviewmodel, ourviewmodel, financeviewmodel);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is ConnectionBrandViewHolder vh)
                vh.Bind(position);
        }
    }

    public class ConnectionBrandViewHolder : RecyclerView.ViewHolder
    {
        private AutoCompleteTextView connectionCompleteBrands;
        private AutoCompleteTextView connectionCompleteLogin;
        private TextView textBrand;
        private TextView textLogin;
        private RadioButton radioDeleteConnection;
        private EditText editParameter1, editParameter2, editParameter3;

        private List<BrandItem> brandData;
        private List<SmartFinance.Connections> ConnectionsViewList;
        private List<SmartFinance.Logins> LoginsViewList;
        private ConnectionBrandAdapter adapter;
        private LoginBrandAdapter loginAdapter;
        private Context context;
        private short defaultInstitutionCode;
        private SignInViewModel signinviewmodel;
        private MainViewModel mvm;
        private FinanceViewModel fvm;

        public ConnectionBrandViewHolder(View itemView, Context ctx, List<BrandItem> data, List<SmartFinance.Connections> connectionsViewList, List<SmartFinance.Logins> loginsViewList, ConnectionBrandAdapter adapterRef, short institutionCode, LoginBrandAdapter loginAdapterRef, SignInViewModel signinvm, MainViewModel ourviewmodel, FinanceViewModel financeviewmodel) : base(itemView)
        {
            context = ctx;
            brandData = data;
            ConnectionsViewList = connectionsViewList;
            LoginsViewList = loginsViewList;
            adapter = adapterRef;
            loginAdapter = loginAdapterRef;
            defaultInstitutionCode = institutionCode;
            signinviewmodel = signinvm;
            mvm = ourviewmodel;
            fvm = financeviewmodel;

            connectionCompleteBrands = itemView.FindViewById<AutoCompleteTextView>(Resource.Id.connectionCompleteBrands);
            connectionCompleteLogin = itemView.FindViewById<AutoCompleteTextView>(Resource.Id.connectionCompleteLogin);
            textBrand = itemView.FindViewById<TextView>(Resource.Id.textBrand);
            textLogin = itemView.FindViewById<TextView>(Resource.Id.textLogin);
            editParameter1 = itemView.FindViewById<EditText>(Resource.Id.editParameter1);
            editParameter2 = itemView.FindViewById<EditText>(Resource.Id.editParameter2);
            editParameter3 = itemView.FindViewById<EditText>(Resource.Id.editParameter3);
            radioDeleteConnection = itemView.FindViewById<RadioButton>(Resource.Id.radioDeleteConnection);

            editParameter1.ImeOptions = Android.Views.InputMethods.ImeAction.Next;
            editParameter2.ImeOptions = Android.Views.InputMethods.ImeAction.Next;
            editParameter3.ImeOptions = Android.Views.InputMethods.ImeAction.Done;

            editParameter1.EditorAction += Parameter_EditorAction;
            editParameter2.EditorAction += Parameter_EditorAction;
            editParameter3.EditorAction += Parameter_EditorAction;

            SetupBrandDropdown();
        }

        private void Parameter_EditorAction(object sender, TextView.EditorActionEventArgs e)
        {
            if (e.ActionId != Android.Views.InputMethods.ImeAction.Next && e.ActionId != Android.Views.InputMethods.ImeAction.Done) return;

            int pos = BindingAdapterPosition;
            if (pos == RecyclerView.NoPosition) return;

            var connection = ConnectionsViewList[pos];
            connection.Parameter1 = editParameter1.Text;
            connection.Parameter2 = editParameter2.Text;
            connection.Parameter3 = editParameter3.Text;

            Toast.MakeText(context, "Connection updated", ToastLength.Short).Show();

            if (sender == editParameter1)
                editParameter2.RequestFocus();
            else if (sender == editParameter2)
                editParameter3.RequestFocus();
            else
                ((EditText)sender).ClearFocus();

            e.Handled = true;
        }

        public void Bind(int position)
        {
            radioDeleteConnection.CheckedChange -= (s, e) =>
                                Delete_Connection(s, e, signinviewmodel, mvm, fvm);

            if (position >= ConnectionsViewList.Count)
            {
                connectionCompleteBrands.Visibility = ViewStates.Visible;
                connectionCompleteBrands.Text = "";
                connectionCompleteBrands.Hint = GetAvailableBrands().FirstOrDefault();
                connectionCompleteLogin.Visibility = ViewStates.Gone;
                textBrand.Visibility = ViewStates.Gone;
                textLogin.Visibility = ViewStates.Gone;
                editParameter1.Visibility = ViewStates.Visible;
                editParameter2.Visibility = ViewStates.Visible;
                editParameter3.Visibility = ViewStates.Visible;
                editParameter1.Text = editParameter2.Text = editParameter3.Text = "";
                radioDeleteConnection.Visibility = ViewStates.Gone;
                RefreshBrandDropdown();
                return;
            }

            var connection = ConnectionsViewList[position];
            connectionCompleteBrands.Visibility = ViewStates.Gone;
            textBrand.Visibility = ViewStates.Visible;
            textBrand.Text = connection.BrandName;

            if (connection.LOGIN_METHOD > 0)
            {
                connectionCompleteLogin.Visibility = ViewStates.Gone;
                textLogin.Visibility = ViewStates.Visible;
                textLogin.Text = connection.LOGIN_METHOD.ToString();
            }
            else
            {
                connectionCompleteLogin.Visibility = ViewStates.Visible;
                textLogin.Visibility = ViewStates.Gone;
                SetupLoginDropdown(connection);
            }

            editParameter1.Text = connection.Parameter1 ?? "";
            editParameter2.Text = connection.Parameter2 ?? "";
            editParameter3.Text = connection.Parameter3 ?? "";

            radioDeleteConnection.Visibility = ViewStates.Visible;
            radioDeleteConnection.Checked = false;
            radioDeleteConnection.CheckedChange += (s, e) =>
                                Delete_Connection(s, e, signinviewmodel, mvm, fvm);
        }

        private List<string> GetAvailableBrands()
        {
            return brandData.Where(b => !ConnectionsViewList.Any(c => c.BrandName == b.BrandName)).Select(b => b.BrandName).ToList();
        }

        private void RefreshBrandDropdown()
        {
            var available = GetAvailableBrands();
            connectionCompleteBrands.Adapter = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleListItem1, available);

            int pos = BindingAdapterPosition;
            if (pos == RecyclerView.NoPosition) return;
            var connection = ConnectionsViewList[pos];

            if (available.Count == 1)
            {
                var brandItem = brandData.First(b => b.BrandName == available[0]);
                connection.BRAND_CODE = brandItem.BRAND_CODE;
                connection.BrandName = brandItem.BrandName;

                connectionCompleteBrands.Visibility = ViewStates.Gone;
                textBrand.Visibility = ViewStates.Visible;
                textBrand.Text = brandItem.BrandName;

                radioDeleteConnection.Visibility = ViewStates.Visible;
                radioDeleteConnection.Checked = false;
                radioDeleteConnection.CheckedChange -= (s, e) =>
                                    Delete_Connection(s, e, signinviewmodel, mvm, fvm);

                radioDeleteConnection.CheckedChange += (s, e) =>
                                    Delete_Connection(s, e, signinviewmodel, mvm, fvm);

                connectionCompleteLogin.Visibility = ViewStates.Visible;
                textLogin.Visibility = ViewStates.Gone;

                SetupLoginDropdown(connection);
                adapter.NotifyItemChanged(pos);
            }
            else if (available.Count > 1)
            {
                connectionCompleteBrands.Hint = available[0];
            }
        }

        private void SetupBrandDropdown()
        {
            connectionCompleteBrands.InputType = Android.Text.InputTypes.Null;
            connectionCompleteBrands.KeyListener = null;
            connectionCompleteBrands.Threshold = 0;
            connectionCompleteBrands.Touch -= Brands_Touch;
            connectionCompleteBrands.Touch += Brands_Touch;
            connectionCompleteBrands.ItemClick -= Brands_ItemClick;
            connectionCompleteBrands.ItemClick += Brands_ItemClick;
            RefreshBrandDropdown();
        }

        private void Brands_Touch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Up)
                connectionCompleteBrands.ShowDropDown();
        }

        private void Brands_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string selectedBrand = connectionCompleteBrands.Adapter.GetItem(e.Position).ToString();
            SelectBrand(selectedBrand);
        }

        private void SelectBrand(string selectedBrand)
        {
            var brandItem = brandData.FirstOrDefault(x => x.BrandName == selectedBrand);
            if (brandItem == null) return;
            int pos = BindingAdapterPosition;
            if (pos == RecyclerView.NoPosition) return;
            var connection = ConnectionsViewList[pos];
            connection.BRAND_CODE = brandItem.BRAND_CODE;
            connection.BrandName = selectedBrand;
            adapter.NotifyItemChanged(pos);
        }

        private void SetupLoginDropdown(SmartFinance.Connections connection)
        {
            connectionCompleteLogin.InputType = Android.Text.InputTypes.Null;
            connectionCompleteLogin.KeyListener = null;
            connectionCompleteLogin.Threshold = 0;
            var loginOptions = new List<string> { "1" };
            connectionCompleteLogin.Adapter = new ArrayAdapter<string>(context, Android.Resource.Layout.SimpleDropDownItem1Line, loginOptions);

            if (loginOptions.Count == 1)
            {
                connectionCompleteLogin.Visibility = ViewStates.Gone;
                textLogin.Visibility = ViewStates.Visible;
                connection.LOGIN_METHOD = 1;
                textLogin.Text = "1";
            }
            else
            {
                connectionCompleteLogin.Visibility = ViewStates.Visible;
                textLogin.Visibility = ViewStates.Gone;
                connectionCompleteLogin.Touch -= Login_Touch;
                connectionCompleteLogin.Touch += Login_Touch;
                connectionCompleteLogin.ItemClick -= Login_ItemClick;
                connectionCompleteLogin.ItemClick += Login_ItemClick;
            }
        }

        private void Login_Touch(object sender, View.TouchEventArgs e)
        {
            if (e.Event.Action == MotionEventActions.Up)
                connectionCompleteLogin.ShowDropDown();
        }

        private void Login_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int pos = BindingAdapterPosition;
            if (pos == RecyclerView.NoPosition) return;
            var connection = ConnectionsViewList[pos];
            string selected = connectionCompleteLogin.Adapter.GetItem(e.Position).ToString();
            connection.LOGIN_METHOD = int.Parse(selected);
            connectionCompleteLogin.Visibility = ViewStates.Gone;
            textLogin.Visibility = ViewStates.Visible;
            textLogin.Text = selected;
        }

        private void Delete_Connection(object sender, CompoundButton.CheckedChangeEventArgs e,
                                            SignInViewModel signinviewmodel,
                                            MainViewModel ourviewmodel,
                                            FinanceViewModel financeviewmodel)
        {
            if (!e.IsChecked) return;
            int pos = BindingAdapterPosition;
            if (pos == RecyclerView.NoPosition) return;

            SmartFinance.Connections connection = ConnectionsViewList[pos];
            new AndroidX.AppCompat.App.AlertDialog.Builder(context)
                .SetTitle(SignIn.BesetByChimps(signinviewmodel, "ConfirmDelete"))
                .SetMessage(SignIn.BesetByChimps(signinviewmodel, "DeleteConnection") +
                            " " +
                            connection.BrandName + " " +
                            connection.Parameter1 + "?\n\n" +
                            SignIn.BesetByChimps(signinviewmodel, "AllAssociatedLogins"))
                .SetPositiveButton(SignIn.BesetByChimps(signinviewmodel, "Yes"), async (s, args) =>
                {
                    // YES action
                    List<SmartFinance.Logins> loginsToDelete = LoginsViewList
                       .Where(l => l.INSTITUTION_CODE == connection.INSTITUTION_CODE &&
                                   l.BRAND_CODE == connection.BRAND_CODE &&
                                   l.LOGIN_METHOD == connection.LOGIN_METHOD)
                       .ToList();

                    foreach (SmartFinance.Logins login in loginsToDelete)
                    {
                        SmartFinance.Logins selectedLogin = new SmartFinance.Logins();
                        selectedLogin = login;
                        selectedLogin.USERNAME = ourviewmodel.UserName;
                        selectedLogin.CUBEFACE_CODE = SmartParametersV2016.Finance;
                        selectedLogin.Updated = true;
                        selectedLogin.Delete = true;
                        financeviewmodel.PLO.finance_logins_changesList.Add(selectedLogin);
                    }
                    // 🔥 THEN: delete the connection itself
                    if (await SmartFinanceV2025.CommitConnectionAsync(signinviewmodel, ourviewmodel, financeviewmodel, connection, context,update: true, delete: true))
                    {
                        foreach (SmartFinance.Logins login in loginsToDelete)
                        {
                            int loginIndex = LoginsViewList.IndexOf(login);
                            if (loginIndex >= 0)
                            {
                                LoginsViewList.RemoveAt(loginIndex);
                                loginAdapter.NotifyItemRemoved(loginIndex); // properly notify login adapter
                            }
                        }
                        // Remove the Connection from the screen list
                        ConnectionsViewList.RemoveAt(pos);
                        adapter.NotifyItemRemoved(pos);                        
                    }
                })
                .SetNegativeButton(SignIn.BesetByChimps(signinviewmodel, "No"), (s, args) =>
                {
                    // NO action
                })
                .Show();
        
                return;
        
            }

        }
    

    // ===================== LOGINS =====================
    public class LoginBrandAdapter : RecyclerView.Adapter
    {
        private Context context;
        private List<BrandItem> brandData;
        private List<SmartFinance.Logins> LoginsViewList;
        private short defaultInstitutionCode;
        private List<SmartFinance.Connections> ConnectionsViewList;
        private SignInViewModel signinviewmodel;
        private MainViewModel mvm;
        private FinanceViewModel fvm;
        public LoginBrandAdapter(Context ctx, List<BrandItem> brands, List<SmartFinance.Logins> loginsViewList, short institutionCode, List<SmartFinance.Connections> connectionsViewList, SignInViewModel signinvm, MainViewModel ourviewmodel, FinanceViewModel financeviewmodel)
        {
            context = ctx;
            brandData = brands;
            LoginsViewList = loginsViewList;
            defaultInstitutionCode = institutionCode;
            ConnectionsViewList = connectionsViewList;
            signinviewmodel = signinvm;
            mvm = ourviewmodel;
            fvm = financeviewmodel;
        }

        public override int ItemCount => LoginsViewList.Count;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(context).Inflate(Resource.Layout.FinanceLoginsRow, parent, false);
            return new LoginBrandViewHolder(view, context, brandData, LoginsViewList, this, ConnectionsViewList, signinviewmodel, mvm, fvm);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ((LoginBrandViewHolder)holder).Bind(position);
        }
    }

    public class LoginBrandViewHolder : RecyclerView.ViewHolder
    {
        private TextView textBrand, textLogin, textParameter1;
        private CheckBox checkBanks, checkSavings, checkInvestments, checkCryptos;
        private RadioButton radioDeleteLogin;
        private List<BrandItem> brandData;
        private List<SmartFinance.Logins> LoginsViewList;
        private List<SmartFinance.Connections> ConnectionsViewList;
        private LoginBrandAdapter adapter;
        private Context context;
        private SignInViewModel signinviewmodel;
        private MainViewModel mvm;
        private FinanceViewModel fvm;
        public LoginBrandViewHolder(View itemView, Context ctx, List<BrandItem> brands, List<SmartFinance.Logins> loginsViewList, LoginBrandAdapter adapterRef, List<SmartFinance.Connections> connectionsViewList, SignInViewModel signinvm, MainViewModel ourviewmodel, FinanceViewModel financeviewmodel) : base(itemView)
        {
            context = ctx;
            brandData = brands;
            LoginsViewList = loginsViewList;
            ConnectionsViewList = connectionsViewList;
            adapter = adapterRef;
            signinviewmodel = signinvm;
            mvm = ourviewmodel;
            fvm = financeviewmodel;

            textBrand = itemView.FindViewById<TextView>(Resource.Id.loginTextBrand);
            textLogin = itemView.FindViewById<TextView>(Resource.Id.loginTextLogin);
            textParameter1 = itemView.FindViewById<TextView>(Resource.Id.loginTextParameter1);
            checkBanks = itemView.FindViewById<CheckBox>(Resource.Id.loginCheckBanks);
            checkSavings = itemView.FindViewById<CheckBox>(Resource.Id.loginCheckSavings);
            checkInvestments = itemView.FindViewById<CheckBox>(Resource.Id.loginCheckInvestments);
            checkCryptos = itemView.FindViewById<CheckBox>(Resource.Id.loginCheckCrypto);
            radioDeleteLogin = itemView.FindViewById<RadioButton>(Resource.Id.loginRadioDelete);

            checkBanks.CheckedChange += CheckBoxChanged;
            checkSavings.CheckedChange += CheckBoxChanged;
            checkInvestments.CheckedChange += CheckBoxChanged;
            checkCryptos.CheckedChange += CheckBoxChanged;
            radioDeleteLogin.CheckedChange += (s, e) =>
                                Delete_Login(s, e, mvm, fvm);
        }

        public void Bind(int position)
        {
            var login = LoginsViewList[position];

            checkBanks.CheckedChange -= CheckBoxChanged;
            checkSavings.CheckedChange -= CheckBoxChanged;
            checkInvestments.CheckedChange -= CheckBoxChanged;
            checkCryptos.CheckedChange -= CheckBoxChanged;

            checkBanks.Checked = login.BANKSCHECKED;
            checkSavings.Checked = login.SAVINGSCHECKED;
            checkInvestments.Checked = login.INVESTMENTSCHECKED;
            checkCryptos.Checked = login.CRYPTOSCHECKED;

            checkBanks.CheckedChange += CheckBoxChanged;
            checkSavings.CheckedChange += CheckBoxChanged;
            checkInvestments.CheckedChange += CheckBoxChanged;
            checkCryptos.CheckedChange += CheckBoxChanged;

            textParameter1.Text = login.Parameter1;

            var brand = brandData.FirstOrDefault(b => b.BRAND_CODE == login.BRAND_CODE);
            textBrand.Text = brand?.BrandName ?? "";
            textBrand.Visibility = ViewStates.Visible;

            textLogin.Text = login.LOGIN_METHOD.ToString();
            textLogin.Visibility = ViewStates.Visible;
            radioDeleteLogin.Checked = false;
        }

        private void CheckBoxChanged(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            int pos = BindingAdapterPosition;
            if (pos == RecyclerView.NoPosition) return;

            var login = LoginsViewList[pos];
            if (sender == checkBanks) login.BANKSCHECKED = e.IsChecked;
            if (sender == checkSavings) login.SAVINGSCHECKED = e.IsChecked;
            if (sender == checkInvestments) login.INVESTMENTSCHECKED = e.IsChecked;
            if (sender == checkCryptos) login.CRYPTOSCHECKED = e.IsChecked;

            Toast.MakeText(context, SignIn.BesetByChimps(signinviewmodel, "LoginUpdateSuccess"), ToastLength.Short).Show();
        }

        private void Delete_Login(object sender, CompoundButton.CheckedChangeEventArgs e,
                                        MainViewModel ourviewmodel,
                                        FinanceViewModel financeviewmodel)
        {
            if (!e.IsChecked) return;
            int pos = BindingAdapterPosition;
            if (pos == RecyclerView.NoPosition) return;

            SmartFinance.Logins login = LoginsViewList[pos];
            new AndroidX.AppCompat.App.AlertDialog.Builder(context)
                .SetTitle(SignIn.BesetByChimps(signinviewmodel, "ConfirmDelete"))
                .SetMessage(SignIn.BesetByChimps(signinviewmodel, "DeleteLogin") + " " + login.BrandName + " " + login.Parameter1 + "?")
                .SetPositiveButton(SignIn.BesetByChimps(signinviewmodel, "Yes"), async (s, e) =>
                {
                    // YES action
                    login.Delete = true;
                    // 🔥 THEN: delete the login itself
                    if (!await SmartFinanceV2025.CommitLoginAsync(signinviewmodel, ourviewmodel, financeviewmodel, login, context, update: true, delete: true))
                    {
                        return;
                    }
                    // Persist delete
                    LoginsViewList.RemoveAt(pos);
                    adapter.NotifyItemRemoved(pos);
                })
                .SetNegativeButton(SignIn.BesetByChimps(signinviewmodel, "No"), (s, e) =>
                {
                    // NO action

                })
                .Show();

            return;
        }
    }
}