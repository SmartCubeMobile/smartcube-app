#if ANDROIDX
using Android.Content;
using Android.Util;

namespace SmartCubeMobile
{
    public interface MultiSpinnerListener
    {
        void onItemsSelected(bool[] selected);
    }

    public class MultiSpinner : Spinner, IDialogInterfaceOnMultiChoiceClickListener, IDialogInterfaceOnCancelListener
    {
        
        Context _context;

        private MainViewModel ourviewmodel;
        private FinanceViewModel financeviewmodel;

        private List<string> items;
        private bool[] selected;
        internal string defaultText;
        private MultiSpinnerListener listener;
        internal char which;


        public MultiSpinner(Context context, MainViewModel mainvm, FinanceViewModel fvm) : base(context)
        {
            _context = context;
            this.ourviewmodel = mainvm;
            this.financeviewmodel = fvm;
            return;
        }

        public MultiSpinner(Context context, IAttributeSet arg1) : base(context, arg1)
        {
            _context = context;
            return;
        }

        public MultiSpinner(Context context, IAttributeSet arg1, int arg2) : base(context, arg1, arg2)
        {
            _context = context;
            return;
        }

        public void OnClick(IDialogInterface dialog, int which, bool isChecked)
        {
            if (isChecked)
            {
                selected[which] = true;
            }
            else
            {
                selected[which] = false;
            }
            return;
        }

        public override void OnClick(IDialogInterface dialog, int which)
        {
            dialog.Cancel();
            return;
        }

        public override bool PerformClick()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(_context);
            //builder.SetView();
            builder.SetMultiChoiceItems(items.ToArray(), selected, this);
            builder.SetPositiveButton("OK", this);
            builder.SetOnCancelListener(this);
            builder.Show();
            return true;
        }

        public bool[] SetItems(List<string> items, string initialText, MultiSpinnerListener listener, char which)
        {
            this.which = which;
            this.items = items;
            this.defaultText = initialText;
            this.listener = listener;

            selected = new bool[items.Count];

            if (initialText == "")
            {
                initialText = "<empty>";
            }
            else
            {
                string[] initialItems = initialText.Split(',');
                // all selected by default
                foreach (string initialItem in initialItems)
                {
                    for (int i = 0; i < selected.Length; i++)
                    {
                        if (items[i] == initialItem)
                        {
                            selected[i] = true;
                            break;
                        }
                    }
                }
            }

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(_context, Resource.Layout.simple_spinner_item, Resource.Id.tv_item, new string[] { initialText });
            // all text on the spinner
            //ArrayAdapter<String> adapter = new ArrayAdapter<String>(_context,Resource.Layout.simple_spinner_item, new String[] { allText });
            Adapter = adapter;
            return selected;
        }

        public void OnCancel(IDialogInterface dialog)
        {
            Java.Lang.StringBuffer spinnerBuffer = new Java.Lang.StringBuffer();
            bool someUnselected = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (selected[i] == true)
                {
                    if (spinnerBuffer.Count() > 0)
                    {
                        spinnerBuffer.Append(",");
                    }
                    spinnerBuffer.Append(items[i]);
                }
                else
                {
                    someUnselected = true;
                }
            }
            string spinnerText = spinnerBuffer.ToString();
            this.defaultText = spinnerText;
            if (someUnselected)
            {
                if (spinnerText.Length == 0)
                {
                    spinnerText = "<empty>";
                }
            }

            ArrayAdapter<string> adapter = new ArrayAdapter<string>(_context, Resource.Layout.simple_spinner_item, Resource.Id.tv_item, new string[] { spinnerText });
            Adapter = adapter;
            if (listener != null)
            {
                listener.onItemsSelected(selected);
            }
            dialog.Dismiss();

            // This is where you UPDATE all the Transactions
            // Here!!!
            string[] what = this.defaultText.Split(SmartParametersV2016.comma);
            switch (this.which)
            {
                case 'A':
                    foreach (FinanceViewModel.AccountItem pitem in financeviewmodel.FinanceAccountsList)
                    {
                        pitem.IsChecked = false;
                        foreach (string item in what)
                        {
                            if (pitem.Content == item)
                            {
                                pitem.IsChecked = true;
                                break;
                            }
                        }
                    }
                    SmartFinanceV2025.Finance_AccountsChanged_Actual(ourviewmodel,
                                                                    financeviewmodel);
                    break;
                case 'P':
                    foreach (FinanceViewModel.ProviderItem pitem in financeviewmodel.FinanceProvidersList)
                    {
                        pitem.IsChecked = false;
                        foreach (string item in what)
                        {
                            if (pitem.Content == item)
                            {
                                pitem.IsChecked = true;
                                break;
                            }
                        }
                    }
                    SmartFinanceV2025.Finance_ProvidersChanged_Actual(ourviewmodel,
                                                                    financeviewmodel);
                    break;
                case 'T':
                    foreach (FinanceViewModel.TransactionGroupItem titem in financeviewmodel.FinanceTransactionGroupsList)
                    {
                        titem.IsChecked = false;
                        foreach (string item in what)
                        {
                            if (titem.Content == item)
                            {
                                titem.IsChecked = true;
                                break;
                            }
                        }
                    }
                    SmartFinanceV2025.Finance_TransactionGroupsChanged_Actual(ourviewmodel,
                                                                            financeviewmodel);
                    
                    break;
                default:
                    break;
            }
            return;
        }
    }
}
#endif