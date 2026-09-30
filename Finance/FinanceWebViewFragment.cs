#if ANDROIDX
using Android.Views;
using Android.Webkit;
using Java.Net;
using static Google.Android.Material.Tabs.TabLayout;

namespace SmartCubeMobile
{
    public class FinanceWebViewFragment : AndroidX.Fragment.App.Fragment
    {
        private WebView mywebView;
        // Left in - we may need these two!!
        private MainViewModel ourviewmodel;
        private FinanceViewModel financeviewmodel;
        public FinanceWebViewFragment() { }
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
            ourviewmodel = result.Value.main;
            financeviewmodel = result.Value.finance;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.FinanceWebView, container, false);
            mywebView = view.FindViewById<WebView>(Resource.Id.financeWebView);

            mywebView.Settings.JavaScriptEnabled = true;
            mywebView.Settings.DomStorageEnabled = true;
            mywebView.Settings.UserAgentString =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
            "(KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36";
            mywebView.SetWebViewClient(new WebViewClient()); // Ensures loading inside WebView

            string UrlToLoad = Arguments?.GetString("url_to_load");
            if (!string.IsNullOrEmpty(UrlToLoad))
            {
                mywebView.LoadUrl(UrlToLoad);
            }
            return view;
        }

        // Public method to allow Activity or other code to use the WebView
        public void LoadUrl(string url)
        {
            if (mywebView != null)
            {
                mywebView.LoadUrl(url);
            }
        }
        public void SetWebViewClient(WebViewClient client)
        {
            mywebView.SetWebViewClient(client);
            return;
        }
        public WebView GetWebView()
        {
            return mywebView;
        }
        public override void OnResume()
        {
            base.OnResume();
            mywebView.OnResume();
        }

        public override void OnPause()
        {
            mywebView?.OnPause();
            base.OnPause();
        }
        public override void OnDestroyView()
        {
            mywebView?.Destroy();
            mywebView = null;
            base.OnDestroyView();
        }
        public static FinanceWebViewFragment NewInstance(string Url, MainViewModel mainvm, FinanceViewModel financevm)
        {
            String key = Guid.NewGuid().ToString();
            FinanceViewModelStore.Add(key, mainvm, financevm);
            FinanceWebViewFragment fragment = new FinanceWebViewFragment();
            Bundle args = new Bundle();
            args.PutString("vm_key", key);
            args.PutString("url_to_load", Url); // pass URL here
            fragment.Arguments = args;
            return fragment;
        }
    }
}
#endif