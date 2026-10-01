using SmartCubeMobile.MockData;
using SmartCubeMobile.Services;
using System.Globalization;
using System.Text.RegularExpressions;
#if WINDOWS
using Microsoft.Web.WebView2.Core;
#endif

namespace SmartCubeMobile.Dashboard
{
    public partial class SupplierLoginFlow : ContentPage
    {
        public event Action SupplierAdded;

        private readonly CultureInfo culture = new("en-GB");
        private SupplierInfo _selected;
        private bool _loggedIn;
        private List<ExtractedBill> _extractedBills = new();
        private readonly List<string> _capturedApiResponses = new();

        private static readonly List<SupplierInfo> Suppliers = new()
        {
            // Energy
            new("British Gas", "⚡", "#0072CE", "Energy", "https://www.britishgas.co.uk/identity/login"),
            new("EDF Energy", "⚡", "#FF6B00", "Energy", "https://www.edfenergy.com/myaccount/login"),
            new("Octopus Energy", "⚡", "#E51A5E", "Energy", "https://octopus.energy/dashboard/"),
            new("OVO Energy", "⚡", "#30BF54", "Energy", "https://my.ovoenergy.com/login"),
            new("E.ON Next", "⚡", "#EA1B2D", "Energy", "https://www.eonnext.com/login"),
            new("Scottish Power", "⚡", "#003DA5", "Energy", "https://www.scottishpower.co.uk/login/"),
            new("Shell Energy", "⚡", "#FFD500", "Energy", "https://shellenergy.co.uk/my-account/login"),
            new("Bulb", "⚡", "#79D17C", "Energy", "https://account.bulb.co.uk/login"),
            new("Utility Warehouse", "⚡", "#5C2D91", "Energy", "https://www.utilitywarehouse.co.uk/login"),
            new("So Energy", "⚡", "#00B4D8", "Energy", "https://www.so.energy/account/login"),

            // Water
            new("Thames Water", "💧", "#0072CE", "Water", "https://myaccount.thameswater.co.uk/"),
            new("Severn Trent", "💧", "#00A6D6", "Water", "https://myaccount.stwater.co.uk/login"),
            new("United Utilities", "💧", "#003865", "Water", "https://www.unitedutilities.com/my-account/sign-in/"),
            new("Yorkshire Water", "💧", "#0077C8", "Water", "https://www.yorkshirewater.com/my-account/"),
            new("Anglian Water", "💧", "#00447C", "Water", "https://myaccount.anglianwater.co.uk/"),
            new("South West Water", "💧", "#006B77", "Water", "https://www.southwestwater.co.uk/my-account/"),
            new("Welsh Water", "💧", "#003DA5", "Water", "https://www.dwrcymru.com/en/my-account"),
            new("Southern Water", "💧", "#005A9C", "Water", "https://myaccount.southernwater.co.uk/"),
            new("Northumbrian Water", "💧", "#0077B6", "Water", "https://www.nwl.co.uk/your-account/"),

            // Telecom
            new("BT", "📡", "#5514B4", "Telecom", "https://www.bt.com/mybt/login"),
            new("Sky", "📡", "#0072C9", "Telecom", "https://www.sky.com/signin"),
            new("Virgin Media", "📡", "#ED1C24", "Telecom", "https://www.virginmedia.com/my-virgin-media/sign-in"),
            new("EE", "📱", "#007B85", "Telecom", "https://id.ee.co.uk/id/login"),
            new("Three", "📱", "#FF7B7B", "Telecom", "https://www.three.co.uk/my3account"),
            new("Vodafone", "📱", "#E60000", "Telecom", "https://www.vodafone.co.uk/myvodafone/"),
            new("O2", "📱", "#0019A5", "Telecom", "https://accounts.o2.co.uk/signin"),
            new("Plusnet", "📡", "#FA6632", "Telecom", "https://www.plus.net/member-centre/login/"),
            new("TalkTalk", "📡", "#6C2D82", "Telecom", "https://www.talktalk.co.uk/myaccount"),

            // Other
            new("Council Tax", "🏛", "#4A5568", "Other", ""),
            new("TV Licence", "📺", "#2D3748", "Other", "https://www.tvlicensing.co.uk/cs/account/index.app"),
        };

        public SupplierLoginFlow()
        {
            InitializeComponent();
            ConfigureWebView();
            BuildSupplierList();
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Connect Supplier",
                "Pick your energy, water, broadband or mobile supplier to log in on their own website inside SmartCube. After your first login SmartCube offers to remember the username and password, encrypted on this PC only, so next time it signs you in when you click the supplier (any text-message code still needs typing). Once you're logged in, opening or downloading a bill there adds it to your bill history automatically, or use Scrape Bills to read the current page. Forget login removes a saved login; Log shows a technical log for troubleshooting. Everything is stored locally on this PC.",
                "OK");
        }

        private void ConfigureWebView()
        {
#if WINDOWS
            SupplierWebView.HandlerChanged += (s, e) =>
            {
                if (SupplierWebView.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.WebView2 wv2Ctrl)
                {
                    wv2Ctrl.CoreWebView2Initialized += async (sender, args) =>
                    {
                        if (wv2Ctrl.CoreWebView2 != null)
                        {
                            _core = wv2Ctrl.CoreWebView2;
                            wv2Ctrl.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
                            wv2Ctrl.CoreWebView2.Settings.IsWebMessageEnabled = true;
                            wv2Ctrl.CoreWebView2.Settings.AreDevToolsEnabled = false;
                            wv2Ctrl.CoreWebView2.Settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

                            wv2Ctrl.CoreWebView2.ServerCertificateErrorDetected += (s2, certArgs) =>
                            {
                                certArgs.Action = CoreWebView2ServerCertificateErrorAction.AlwaysAllow;
                            };

                            // Saved login: capture what the user types into a login form (offered to be remembered
                            // after a successful login) and fill it in automatically on later visits.
                            try { await wv2Ctrl.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(LoginCaptureScript); }
                            catch (Exception ex) { SupplierLog.Error("Install login capture", ex); }

                            wv2Ctrl.CoreWebView2.WebMessageReceived += (s2, msgArgs) =>
                            {
                                try
                                {
                                    var msg = Newtonsoft.Json.Linq.JObject.Parse(msgArgs.TryGetWebMessageAsString());
                                    if (msg["type"]?.ToString() == "creds")
                                    {
                                        var pass = msg["pass"]?.ToString();
                                        if (string.IsNullOrEmpty(pass)) return;
                                        _typedUser = msg["user"]?.ToString() ?? _typedUser;
                                        _typedPass = pass;
                                        SupplierLog.Write("Login form submitted by user (credentials held in memory pending login check).");
                                    }
                                }
                                catch { }
                            };

                            wv2Ctrl.CoreWebView2.NavigationCompleted += (s2, navDone) =>
                            {
                                if (navDone.IsSuccess) _ = TryAutoLogin();
                            };

                            // "View bill" links: don't open the PDF at all. Fetch it in the background and stay on
                            // the page, so the user can go straight on to the next bill.
                            wv2Ctrl.CoreWebView2.NavigationStarting += (s2, navArgs) =>
                            {
                                try
                                {
                                    if (_selected != null && LooksLikePdfUrl(navArgs.Uri ?? ""))
                                    {
                                        navArgs.Cancel = true;
                                        SupplierLog.Write($"Intercepted bill link (kept page): {navArgs.Uri.Split('?')[0]}");
                                        MainThread.BeginInvokeOnMainThread(() => ShowBillStrip("Reading bill…", busy: true));
                                        _ = CapturePdfFromUrl(navArgs.Uri);
                                    }
                                }
                                catch (Exception ex) { SupplierLog.Error("NavigationStarting", ex); }
                            };

                            // Bill PDFs that open in a new tab stay inside SmartCube's browser.
                            wv2Ctrl.CoreWebView2.NewWindowRequested += (s2, nwArgs) =>
                            {
                                try
                                {
                                    nwArgs.Handled = true;
                                    if (LooksLikePdfUrl(nwArgs.Uri ?? ""))
                                    {
                                        SupplierLog.Write($"Intercepted bill link in new window (kept page): {nwArgs.Uri.Split('?')[0]}");
                                        MainThread.BeginInvokeOnMainThread(() => ShowBillStrip("Reading bill…", busy: true));
                                        _ = CapturePdfFromUrl(nwArgs.Uri);
                                        return;
                                    }
                                    wv2Ctrl.CoreWebView2.Navigate(nwArgs.Uri);
                                }
                                catch (Exception ex) { SupplierLog.Error("NewWindowRequested", ex); }
                            };

                            // "Download bill" on the supplier site lands straight in SmartCube, no Explorer step.
                            wv2Ctrl.CoreWebView2.DownloadStarting += (s2, dl) =>
                            {
                                try
                                {
                                    var name = System.IO.Path.GetFileName(dl.ResultFilePath ?? "");
                                    if (string.IsNullOrWhiteSpace(name)) name = "bill.pdf";
                                    var dlKey = (dl.DownloadOperation?.Uri ?? "").Split('?')[0];
                                    bool alreadyHave;
                                    lock (_seenPdfUrls) { alreadyHave = _seenPdfUrls.Contains(dlKey); }
                                    if (alreadyHave)
                                    {
                                        // The viewer copy was already imported; don't download it twice.
                                        dl.Cancel = true;
                                        dl.Handled = true;
                                        SupplierLog.Write($"Download skipped (already imported): {name}");
                                        MainThread.BeginInvokeOnMainThread(() => ShowBillStrip("That bill is already imported.", busy: false));
                                        return;
                                    }
                                    var path = NewDownloadPath(name);
                                    SupplierLog.Write($"Download started: {name} from {dl.DownloadOperation?.Uri} -> {path}");
                                    dl.ResultFilePath = path;
                                    dl.Handled = true;
                                    var op = dl.DownloadOperation;
                                    MainThread.BeginInvokeOnMainThread(() => PageSubtitle.Text = $"Downloading {name}…");
                                    op.StateChanged += (s3, _) =>
                                    {
                                        if (op.State == CoreWebView2DownloadState.Completed)
                                        {
                                            SupplierLog.Write($"Download completed: {name}");
                                            MainThread.BeginInvokeOnMainThread(() => ImportDownloadedFile(path));
                                        }
                                        else if (op.State == CoreWebView2DownloadState.Interrupted)
                                        {
                                            SupplierLog.Write($"Download interrupted: {name} ({op.InterruptReason})");
                                            MainThread.BeginInvokeOnMainThread(() => PageSubtitle.Text = "Download failed. Try again on the supplier's site.");
                                        }
                                    };
                                }
                                catch (Exception ex) { SupplierLog.Error("DownloadStarting", ex); }
                            };

                            wv2Ctrl.CoreWebView2.WebResourceResponseReceived += async (s2, resArgs) =>
                            {
                                try
                                {
                                    var uri = resArgs.Request.Uri?.ToLower() ?? "";
                                    var headers = resArgs.Response.Headers;
                                    // GetHeader throws when the header is absent, so check first.
                                    var ct = headers != null && headers.Contains("Content-Type") ? headers.GetHeader("Content-Type") ?? "" : "";
                                    var disposition = headers != null && headers.Contains("Content-Disposition") ? headers.GetHeader("Content-Disposition") : null;

                                    // A PDF viewed in the browser (not downloaded) is captured too.
                                    if (ct.Contains("application/pdf") && resArgs.Response.StatusCode == 200)
                                    {
                                        var key = (resArgs.Request.Uri ?? "").Split('?')[0];
                                        lock (_seenPdfUrls) { if (_seenPdfUrls.Contains(key)) return; }

                                        // Only claim the URL once we actually have the bytes; a PDF shown in the
                                        // viewer can't be read here and is fetched by CapturePdfFromUrl instead.
                                        Windows.Storage.Streams.IRandomAccessStream pdfStream = null;
                                        try { pdfStream = await resArgs.Response.GetContentAsync(); }
                                        catch (Exception ex) { SupplierLog.Write($"PDF response not readable here ({ex.Message}); will fetch by URL. {key}"); }
                                        if (pdfStream == null)
                                        {
                                            if (LooksLikePdfUrl(resArgs.Request.Uri ?? "")) _ = CapturePdfFromUrl(resArgs.Request.Uri);
                                            return;
                                        }
                                        lock (_seenPdfUrls) { if (!_seenPdfUrls.Add(key)) return; }
                                        try
                                        {
                                            var fileName = FileNameFromResponse(disposition, key);
                                            var path = NewDownloadPath(fileName);
                                            SupplierLog.Write($"PDF response captured: {key} -> {path}");
                                            using (var net = pdfStream.AsStreamForRead())
                                            using (var fs = System.IO.File.Create(path))
                                                await net.CopyToAsync(fs);
                                            MainThread.BeginInvokeOnMainThread(() => ImportDownloadedFile(path));
                                        }
                                        catch (Exception ex)
                                        {
                                            // Give the URL back so the by-URL fetch can still get it.
                                            lock (_seenPdfUrls) { _seenPdfUrls.Remove(key); }
                                            SupplierLog.Write($"PDF response save failed ({ex.Message}); fetching by URL instead.");
                                            if (LooksLikePdfUrl(resArgs.Request.Uri ?? "")) _ = CapturePdfFromUrl(resArgs.Request.Uri);
                                        }
                                        return;
                                    }

                                    if (ct.Contains("json") && (
                                        uri.Contains("bill") || uri.Contains("payment") ||
                                        uri.Contains("account") || uri.Contains("balance") ||
                                        uri.Contains("usage") || uri.Contains("statement") ||
                                        uri.Contains("graphql") || uri.Contains("api") ||
                                        uri.Contains("energy") || uri.Contains("tariff") ||
                                        uri.Contains("meter") || uri.Contains("charge") ||
                                        uri.Contains("consumption") || uri.Contains("myaccount") ||
                                        uri.Contains("customer")))
                                    {
                                        var stream = await resArgs.Response.GetContentAsync();
                                        if (stream != null)
                                        {
                                            using var netStream = stream.AsStreamForRead();
                                            using var reader = new System.IO.StreamReader(netStream);
                                            var body = await reader.ReadToEndAsync();
                                            if (!string.IsNullOrEmpty(body) && body.Length < 500000)
                                            {
                                                _capturedApiResponses.Add(body);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Header lookups on ordinary page assets throw routinely; only log PDFs.
                                    if ((resArgs.Request.Uri ?? "").Contains(".pdf", StringComparison.OrdinalIgnoreCase))
                                        SupplierLog.Error("Response hook", ex);
                                }
                            };
                        }
                    };
                }
            };
#endif
        }

        private int _stripToken;

        // Green strip above the browser. busy=true keeps it up with a spinner; otherwise it hides after a moment.
        private void ShowBillStrip(string text, bool busy, bool ok = true)
        {
            var token = ++_stripToken;
            BillStripLabel.Text = text;
            BillStripSpinner.IsVisible = busy;
            BillStripSpinner.IsRunning = busy;
            BillStrip.BackgroundColor = Color.FromArgb(ok ? "#0A3D2E" : "#3B1A1A");
            BillStrip.Stroke = Color.FromArgb(ok ? "#10B981" : "#EF4444");
            BillStripLabel.TextColor = Color.FromArgb(ok ? "#D1FAE5" : "#FECACA");
            BillStrip.IsVisible = true;
            if (!busy)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(ok ? 3500 : 6000);
                    MainThread.BeginInvokeOnMainThread(() => { if (_stripToken == token) BillStrip.IsVisible = false; });
                });
            }
        }

        private void OnBrowserBackClicked(object sender, EventArgs e)
        {
            try { if (SupplierWebView.CanGoBack) SupplierWebView.GoBack(); }
            catch (Exception ex) { SupplierLog.Error("Browser back", ex); }
        }

        private void OnBrowserForwardClicked(object sender, EventArgs e)
        {
            try { if (SupplierWebView.CanGoForward) SupplierWebView.GoForward(); }
            catch (Exception ex) { SupplierLog.Error("Browser forward", ex); }
        }

        private readonly HashSet<string> _seenPdfUrls = new();
        private int _importedCount;

        // ---- Saved login (auto sign-in) ----
        private string _typedUser, _typedPass;   // what the user typed this session, until we know login worked
        private int _autoLoginAttempts;
        private bool _autoLoginUsed;

        // Runs in every page: when a login form is submitted, sends the username/password to the app.
        private const string LoginCaptureScript = @"
(function(){
  if (window.__scLoginHook) return; window.__scLoginHook = true;
  function visible(i){ return i.offsetParent !== null; }
  function grab(){
    var p = Array.from(document.querySelectorAll('input[type=password]')).filter(visible).find(function(x){ return x.value; });
    if (!p) return null;
    var inputs = Array.from(document.querySelectorAll('input')).filter(visible);
    var idx = inputs.indexOf(p), u = null;
    for (var i = idx - 1; i >= 0; i--) { var t = (inputs[i].type || 'text').toLowerCase(); if ((t === 'text' || t === 'email' || t === 'tel') && inputs[i].value) { u = inputs[i]; break; } }
    if (!u) u = inputs.find(function(x){ var t = (x.type || 'text').toLowerCase(); return (t === 'text' || t === 'email' || t === 'tel') && x.value; });
    return { user: u ? u.value : (window.__scUser || ''), pass: p.value };
  }
  function remember(){ var inputs = Array.from(document.querySelectorAll('input')).filter(visible); var u = inputs.find(function(x){ var t=(x.type||'text').toLowerCase(); return (t==='text'||t==='email'||t==='tel') && x.value; }); if (u && !document.querySelector('input[type=password]')) window.__scUser = u.value; }
  function send(){ remember(); var c = grab(); if (c && c.pass && window.chrome && window.chrome.webview) window.chrome.webview.postMessage(JSON.stringify({ type: 'creds', user: c.user, pass: c.pass })); }
  document.addEventListener('submit', send, true);
  document.addEventListener('click', function(e){ var b = e.target && e.target.closest ? e.target.closest('button, input[type=submit], [role=button]') : null; if (b) send(); }, true);
  document.addEventListener('keydown', function(e){ if (e.key === 'Enter') send(); }, true);
})();";

        // Fills the visible login fields from the saved login and submits. Handles two-step (username, then password) sites.
        private static string BuildAutoLoginScript(string user, string pass)
        {
            static string Js(string s) => Newtonsoft.Json.JsonConvert.SerializeObject(s ?? "");
            return @"
(function(){
  var U = " + Js(user) + @", P = " + Js(pass) + @";
  function visible(i){ return i.offsetParent !== null && !i.disabled && !i.readOnly; }
  function set(el, v){ if (!el) return false; var d = Object.getOwnPropertyDescriptor(HTMLInputElement.prototype, 'value'); if (d && d.set) d.set.call(el, v); else el.value = v; ['input','change'].forEach(function(t){ el.dispatchEvent(new Event(t, {bubbles:true})); }); return true; }
  var inputs = Array.from(document.querySelectorAll('input')).filter(visible);
  var p = inputs.find(function(x){ return x.type === 'password'; });
  var u = inputs.find(function(x){ var t = (x.type || 'text').toLowerCase(); var d = ((x.name||'') + ' ' + (x.id||'') + ' ' + (x.placeholder||'') + ' ' + (x.getAttribute('autocomplete')||'')).toLowerCase(); return (t === 'email' || t === 'text' || t === 'tel') && !/search|postcode|code|otp|verification/.test(d); });
  var did = 0;
  if (u && U) did += set(u, U) ? 1 : 0;
  if (p && P) did += set(p, P) ? 1 : 0;
  if (!did) return 'none';
  var form = (p || u).closest('form');
  var btn = form ? (form.querySelector('button[type=submit], input[type=submit]') || form.querySelector('button')) : null;
  if (!btn) { var all = Array.from(document.querySelectorAll('button, input[type=submit]')).filter(visible); btn = all.find(function(b){ return /log ?in|sign ?in|continue|next|submit/i.test(b.innerText || b.value || ''); }); }
  setTimeout(function(){ if (btn) btn.click(); else if (form && form.requestSubmit) form.requestSubmit(); }, 250);
  return (u && U ? 'user ' : '') + (p && P ? 'pass ' : '') + (btn ? 'submit' : 'nosubmit');
})();";
        }

        private async Task TryAutoLogin()
        {
            try
            {
                if (_selected == null || _loggedIn || _autoLoginAttempts >= 3) return;
                var saved = SupplierCredentialStore.Get(_selected.Name);
                if (saved == null) return;
                await Task.Delay(800);   // let the page finish rendering its form
                var result = await SupplierWebView.EvaluateJavaScriptAsync(BuildAutoLoginScript(saved.User, saved.Pass));
                result = (result ?? "").Trim('"');
                if (result == "none" || string.IsNullOrEmpty(result)) return;
                _autoLoginAttempts++;
                _autoLoginUsed = true;
                SupplierLog.Write($"Auto-login attempt {_autoLoginAttempts} for {_selected.Name}: {result}");
                MainThread.BeginInvokeOnMainThread(() => PageSubtitle.Text = "Signing you in with your saved login…");
            }
            catch (Exception ex) { SupplierLog.Error("Auto-login", ex); }
        }

        // Called once a post-login page is detected. Offers to remember what the user typed.
        private async Task OfferToSaveLogin()
        {
            if (_selected == null || string.IsNullOrEmpty(_typedPass)) return;
            var existing = SupplierCredentialStore.Get(_selected.Name);
            if (existing != null && existing.User == _typedUser && existing.Pass == _typedPass) { _typedPass = null; return; }
            var user = _typedUser; var pass = _typedPass;
            _typedPass = null;

            var save = await DisplayAlert("Remember this login?",
                $"Save your {_selected.Name} username and password so SmartCube signs you in automatically next time?\n\n" +
                "It is stored encrypted on this PC only and is never sent to SmartCube or anyone else. " +
                "You can forget it later with the Forget login button.",
                "Save", "Not now");
            if (!save) return;
            SupplierCredentialStore.Set(_selected.Name, user, pass);
            SupplierLog.Write($"Saved login for {_selected.Name} (user {user}).");
            UpdateForgetButton();
        }

        private void UpdateForgetButton()
        {
            ForgetLoginBtn.IsVisible = _selected != null && SupplierCredentialStore.Has(_selected.Name);
        }

        private async void OnForgetLoginClicked(object sender, EventArgs e)
        {
            if (_selected == null) return;
            var ok = await DisplayAlert("Forget saved login",
                $"Remove the saved username and password for {_selected.Name} from this PC?", "Forget", "Cancel");
            if (!ok) return;
            SupplierCredentialStore.Forget(_selected.Name);
            SupplierLog.Write($"Forgot saved login for {_selected.Name}.");
            UpdateForgetButton();
            PageSubtitle.Text = "Saved login removed. You'll be asked to log in next time.";
        }
#if WINDOWS
        private CoreWebView2 _core;
#endif
        private const string BrowserUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

        private static bool LooksLikePdfUrl(string url)
        {
            try { return new Uri(url).AbsolutePath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase); }
            catch { return false; }
        }

        // "View bill" usually opens the PDF in the browser's own viewer, which we can't read from.
        // So we fetch a second copy ourselves, using the browser's cookies, and import that.
        private async Task CapturePdfFromUrl(string url)
        {
            var key = url.Split('?')[0];
            lock (_seenPdfUrls) { if (!_seenPdfUrls.Add(key)) return; }
            SupplierLog.Write($"PDF opened in viewer, fetching a copy: {key}");
            try
            {
                var handler = new HttpClientHandler { UseCookies = true, CookieContainer = new System.Net.CookieContainer() };
#if WINDOWS
                if (_core != null)
                {
                    try
                    {
                        var cookies = await _core.CookieManager.GetCookiesAsync(url);
                        foreach (var c in cookies)
                        {
                            try { handler.CookieContainer.Add(new System.Net.Cookie(c.Name, c.Value, c.Path, c.Domain) { Secure = c.IsSecure }); }
                            catch { }
                        }
                    }
                    catch (Exception ex) { SupplierLog.Error("Copy cookies", ex); }
                }
#endif
                using var http = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(60) };
                http.DefaultRequestHeaders.UserAgent.ParseAdd(BrowserUserAgent);
                if (_selected?.LoginUrl != null) http.DefaultRequestHeaders.Referrer = new Uri(_selected.LoginUrl);

                using var resp = await http.GetAsync(url);
                var bytes = await resp.Content.ReadAsByteArrayAsync();
                SupplierLog.Write($"Fetched {(int)resp.StatusCode} {resp.Content.Headers.ContentType} {bytes.Length:N0} bytes");
                if (!resp.IsSuccessStatusCode || bytes.Length < 5 || bytes[0] != (byte)'%' || bytes[1] != (byte)'P')
                {
                    SupplierLog.Write("Fetched content is not a PDF.");
                    lock (_seenPdfUrls) { _seenPdfUrls.Remove(key); }
                    MainThread.BeginInvokeOnMainThread(() => ShowBillStrip("Couldn't read that bill. Try Save on the supplier's site.", busy: false, ok: false));
                    return;
                }

                var name = FileNameFromResponse(resp.Content.Headers.ContentDisposition?.ToString(), key);
                var path = NewDownloadPath(name);
                await System.IO.File.WriteAllBytesAsync(path, bytes);
                MainThread.BeginInvokeOnMainThread(() => ImportDownloadedFile(path));
            }
            catch (Exception ex)
            {
                SupplierLog.Error("Fetch PDF", ex);
                lock (_seenPdfUrls) { _seenPdfUrls.Remove(key); }
                MainThread.BeginInvokeOnMainThread(() => ShowBillStrip("Couldn't fetch that bill. Details are in the log.", busy: false, ok: false));
            }
        }

        // The download is only a staging copy: once the bill is read and an encrypted copy is in
        // Documents, the plain download is removed so no unencrypted bill stays on disk.
        private static void DeleteDownload(string path)
        {
            try { if (System.IO.File.Exists(path)) System.IO.File.Delete(path); }
            catch (Exception ex) { SupplierLog.Error("Remove staging download", ex); }
        }

        private static string NewDownloadPath(string name)
        {
            var dir = System.IO.Path.Combine(SmartCubeMobile.Services.AppPaths.AppData, "SupplierDownloads");
            System.IO.Directory.CreateDirectory(dir);
            foreach (var c in System.IO.Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
            if (!name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)) name += ".pdf";
            return System.IO.Path.Combine(dir, $"{DateTime.Now:yyyyMMdd-HHmmss}-{name}");
        }

        private static string FileNameFromResponse(string contentDisposition, string url)
        {
            var m = Regex.Match(contentDisposition ?? "", @"filename\*?=(?:UTF-8'')?""?([^"";]+)", RegexOptions.IgnoreCase);
            if (m.Success) return Uri.UnescapeDataString(m.Groups[1].Value.Trim());
            try
            {
                var last = System.IO.Path.GetFileName(new Uri(url).AbsolutePath);
                if (!string.IsNullOrWhiteSpace(last)) return last;
            }
            catch { }
            return "bill.pdf";
        }

        // Parses a PDF that arrived from the supplier's site and adds it to the user's bills.
        private async void ImportDownloadedFile(string path)
        {
            var name = System.IO.Path.GetFileName(path);
            PageSubtitle.Text = $"Reading {name}…";
            ShowBillStrip("Reading bill…", busy: true);
            long size = 0;
            try { size = new System.IO.FileInfo(path).Length; } catch { }
            SupplierLog.Write($"PDF received: {name} ({size:N0} bytes)");

            ParsedBill parsed = null;
            try { parsed = await Task.Run(() => BillPdfParser.ParseFromFile(path)); }
            catch (Exception ex) { SupplierLog.Error($"Parse {name}", ex); }

            if (parsed == null)
            {
                SupplierLog.Write($"Parse {name}: no bill data recognised. Saved to Documents.");
                try { DocumentStorageService.AddDocument(path, "Bill", name); DeleteDownload(path); } catch (Exception ex) { SupplierLog.Error("Save document", ex); }
                PageSubtitle.Text = "Downloaded, but it didn't look like a bill. Saved to your Documents instead.";
                ShowBillStrip("That file didn't look like a bill. Saved to your Documents instead.", busy: false, ok: false);
                return;
            }

            if (string.IsNullOrWhiteSpace(parsed.Supplier) && _selected != null)
                parsed.Supplier = _selected.Name;

            SupplierLog.Write($"Parse {name}: supplier={parsed.Supplier} fuel={parsed.FuelType} amount={parsed.Amount} date={parsed.BillDate:yyyy-MM-dd} period={parsed.Period} units={parsed.UnitsUsed} {parsed.UoM} account={parsed.AccountNumber}");

            int added = 0, dupes = 0;
            var split = BillPdfParser.SplitDualFuel(parsed);
            foreach (var bill in split)
            {
                var ok = MockDataService.AddBill(bill);
                SupplierLog.Write($"  {(ok ? "added" : "duplicate")}: {bill.Supplier} {bill.FuelType} £{bill.Amount} {bill.Period}");
                if (ok) added++; else dupes++;
            }
            EnsureSupplierRecords(split, parsed.AccountNumber);
            try { DocumentStorageService.AddDocument(path, "Bill", $"{parsed.Supplier} {parsed.BillDate:MMM yyyy}.pdf"); DeleteDownload(path); }
            catch (Exception ex) { SupplierLog.Error("Save document", ex); }

            if (added > 0)
            {
                _importedCount += added;
                SupplierAdded?.Invoke();
                PageSubtitle.Text = $"{_importedCount} bill{(_importedCount == 1 ? "" : "s")} imported this session";
                ShowBillStrip($"✓ Bill added: {parsed.Supplier} £{parsed.Amount:N2} · {parsed.BillDate:dd MMM yyyy}{(added > 1 ? $" ({added} fuels)" : "")}", busy: false);
            }
            else
            {
                PageSubtitle.Text = $"{_importedCount} bill{(_importedCount == 1 ? "" : "s")} imported this session";
                ShowBillStrip($"{parsed.Supplier} £{parsed.Amount:N2} · {parsed.BillDate:dd MMM yyyy} is already in your bills.", busy: false);
            }
        }

        // Makes sure the supplier card(s) exist for each fuel we've imported bills for, and keeps
        // their monthly cost in step with the bills (average of the last 12 months).
        private void EnsureSupplierRecords(List<MockUtilityBill> imported, string accountNumber)
        {
            if (_selected == null) return;
            try
            {
                var suppliers = MockDataService.GetSuppliers();
                var allBills = MockDataService.GetUtilityBills();
                var since = DateTime.Now.AddMonths(-12);
                var changed = false;

                foreach (var fuel in imported.Where(b => b.Amount > 0).Select(b => b.FuelType).Distinct())
                {
                    var rec = suppliers.FirstOrDefault(s =>
                        MockDataService.SupplierNamesMatch(s.Name, _selected.Name) &&
                        string.Equals(s.Type, fuel, StringComparison.OrdinalIgnoreCase));
                    if (rec == null)
                    {
                        rec = new MockSupplier
                        {
                            Name = _selected.Name,
                            Type = fuel,
                            Icon = _selected.Icon,
                            Tariff = "Connected Account",
                            TariffDetail = $"Linked {DateTime.Now:dd MMM yyyy}",
                            MonthlyCost = 0m,
                            AccountRef = accountNumber ?? "",
                        };
                        MockDataService.AddSupplier(rec);
                        SupplierLog.Write($"Supplier record added: {rec.Name} ({fuel})");
                    }

                    var fuelBills = allBills.Where(b =>
                        MockDataService.SupplierNamesMatch(b.Supplier, _selected.Name) &&
                        string.Equals(b.FuelType, fuel, StringComparison.OrdinalIgnoreCase) &&
                        b.Amount > 0 && b.BillDate >= since).ToList();
                    if (fuelBills.Count > 0)
                        rec.MonthlyCost = Math.Round(fuelBills.Average(b => b.Amount), 2);
                    if (!string.IsNullOrWhiteSpace(accountNumber)) rec.AccountRef = accountNumber;
                    rec.TariffDetail = $"{fuelBills.Count} bill{(fuelBills.Count == 1 ? "" : "s")} in the last year · updated {DateTime.Now:dd MMM yyyy}";
                    changed = true;
                }

                if (changed) MockDataService.SaveSuppliers();
                _loggedIn = true;
                SupplierAdded?.Invoke();
                _ = OfferToSaveLogin();   // a bill arriving proves the login worked, even if no page change was seen
            }
            catch (Exception ex) { SupplierLog.Error("EnsureSupplierRecords", ex); }
        }

        private void BuildSupplierList()
        {
            foreach (var s in Suppliers)
            {
                var list = s.Category switch
                {
                    "Energy" => EnergyList,
                    "Water" => WaterList,
                    "Telecom" => TelecomList,
                    _ => OtherList,
                };

                var card = new Border
                {
                    BackgroundColor = Color.FromArgb("#1C2744"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 10 },
                    Stroke = Color.FromArgb("#1E2D4A"),
                    Padding = new Thickness(14, 12),
                };

                var grid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition(new GridLength(36)),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto),
                    },
                    ColumnSpacing = 12,
                };

                var iconBorder = new Border
                {
                    BackgroundColor = Color.FromArgb(s.Colour + "30"),
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                    Stroke = Colors.Transparent,
                    WidthRequest = 36, HeightRequest = 36,
                    Content = new Label
                    {
                        Text = s.Icon, FontSize = 18,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    }
                };
                Grid.SetColumn(iconBorder, 0);

                var name = new Label
                {
                    Text = s.Name, TextColor = Color.FromArgb("#F1F5F9"),
                    FontSize = 14, FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.Center,
                };
                Grid.SetColumn(name, 1);

                var arrow = new Label
                {
                    Text = "→", TextColor = Color.FromArgb("#64748B"),
                    FontSize = 16, VerticalOptions = LayoutOptions.Center,
                };
                Grid.SetColumn(arrow, 2);

                grid.Children.Add(iconBorder);
                grid.Children.Add(name);
                grid.Children.Add(arrow);
                card.Content = grid;

                var supplier = s;
                ToolTipProperties.SetText(card, string.IsNullOrEmpty(supplier.LoginUrl)
                    ? $"Add {supplier.Name} manually."
                    : $"Log in to {supplier.Name}'s website; bills you open there are added to SmartCube automatically.");
                card.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await card.ScaleTo(0.97, 80, Easing.CubicOut);
                        await card.ScaleTo(1.0, 80, Easing.CubicOut);
                        SelectSupplier(supplier);
                    })
                });

                list.Children.Add(card);
            }
        }

        private async void SelectSupplier(SupplierInfo supplier)
        {
            _selected = supplier;
            _loggedIn = false;
            _capturedApiResponses.Clear();

            if (string.IsNullOrEmpty(supplier.LoginUrl))
            {
                var flow = new AddSupplierFlow();
                flow.SupplierAdded += () => SupplierAdded?.Invoke();
                await Navigation.PushAsync(flow);
                return;
            }

            PageTitle.Text = supplier.Name;
            PageSubtitle.Text = SupplierCredentialStore.Has(supplier.Name) ? "Signing you in with your saved login…" : "Log in to your account";
            ExtractBtn.IsVisible = false;
            _typedUser = _typedPass = null;
            _autoLoginAttempts = 0;
            _autoLoginUsed = false;
            UpdateForgetButton();
            SupplierLog.Write($"--- Connect supplier: {supplier.Name} ({supplier.Category}) {supplier.LoginUrl}{(SupplierCredentialStore.Has(supplier.Name) ? " [saved login]" : "")}");

            SupplierWebView.Source = new UrlWebViewSource { Url = supplier.LoginUrl };

            Step1.IsVisible = false;
            Step2.IsVisible = true;
        }

        private void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            LoadingIndicator.IsRunning = true;
            UrlLabel.Text = e.Url;
        }

        private async void OnLogClicked(object sender, EventArgs e) => await SupplierLog.Open();

        private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            LoadingIndicator.IsRunning = false;
            UrlLabel.Text = e.Url;
            SupplierLog.Write($"Navigated ({e.Result}): {e.Url}");

            if (e.Result == WebNavigationResult.Success && _selected != null)
            {
                var url = e.Url?.ToLower() ?? "";

                if (LooksLikePdfUrl(e.Url ?? ""))
                {
                    _ = CapturePdfFromUrl(e.Url);
                    return;
                }

                if (!_loggedIn)
                {
                    var hasLoginInUrl = url.Contains("/login") || url.Contains("/signin") || url.Contains("/sign-in")
                                    || url.Contains("/authenticate");
                    var isPostLogin = url.Contains("my-account") || url.Contains("myaccount") || url.Contains("dashboard")
                                    || url.Contains("overview") || url.Contains("account-summary")
                                    || url.Contains("/home") || url.Contains("/account");

                    if (!hasLoginInUrl && isPostLogin)
                    {
                        // Logged in: add the supplier and stay here. Nothing is extracted automatically;
                        // bills are added only when the user opens or downloads a PDF, or taps Scrape Bills.
                        _loggedIn = true;
                        SupplierLog.Write($"Login detected for {_selected.Name} at {e.Url}{(_autoLoginUsed ? " (auto-login)" : "")}");
                        AddSupplierFromLogin();
                        _ = OfferToSaveLogin();
                        PageSubtitle.Text = "Supplier added. Open or download a bill on the supplier's site and it will be added here automatically.";
                        ExtractBtn.Text = "Scrape Bills";
                        ExtractBtn.IsVisible = true;
                        ExtractBtn.IsEnabled = true;
                    }
                }
                else
                {
                    ExtractBtn.Text = "Scrape Bills";
                    ExtractBtn.IsVisible = true;
                    ExtractBtn.IsEnabled = true;
                }
            }
        }

        private void AddSupplierFromLogin()
        {
            var type = _selected.Category switch
            {
                "Energy" => "Electricity",
                "Water" => "Water",
                "Telecom" => _selected.Icon == "\U0001F4F1" ? "Mobile" : "Broadband",
                _ => "Other",
            };

            var existing = MockDataService.GetSuppliers()
                .Any(s => s.Name == _selected.Name);

            if (!existing)
            {
                SupplierLog.Write($"Supplier record added: {_selected.Name} ({type})");
                MockDataService.AddSupplier(new MockSupplier
                {
                    Name = _selected.Name,
                    Type = type,
                    Icon = _selected.Icon,
                    Tariff = "Connected Account",
                    TariffDetail = $"Linked {DateTime.Now:dd MMM yyyy}",
                    MonthlyCost = 0m,
                    AccountRef = "",
                });
                SupplierAdded?.Invoke();
            }
        }

        private async Task AutoExtractBills()
        {
            PageSubtitle.Text = "Extracting bill data...";

            try
            {
                if (await TryExtractAndSave())
                    return;

                PageSubtitle.Text = "Waiting for page data...";
                await Task.Delay(3000);

                if (await TryExtractAndSave())
                    return;

                PageSubtitle.Text = "Supplier added. Open or download a bill on the supplier's site and it will be added here automatically.";
            }
            catch
            {
                PageSubtitle.Text = "Supplier added. Open or download a bill on the supplier's site and it will be added here automatically.";
            }
        }

        private async Task<bool> TryExtractAndSave()
        {
            if (TryParseFromCapturedApi() && _extractedBills.Count > 0)
            {
                SaveExtractedBills();
                PageSubtitle.Text = $"Done — {_extractedBills.Count} bill(s) added";
                await Task.Delay(1500);
                SupplierAdded?.Invoke();
                await Navigation.PopAsync();
                return true;
            }

            var js = GetSupplierScraper(_selected?.Name ?? "");
            var result = await SupplierWebView.EvaluateJavaScriptAsync(js);

            if (!string.IsNullOrEmpty(result))
            {
                result = result.Trim('"').Replace("\\\"", "\"").Replace("\\\\", "\\");
                ParseExtractedData(result);

                if (_extractedBills.Count > 0)
                {
                    SaveExtractedBills();
                    PageSubtitle.Text = $"Done — {_extractedBills.Count} bill(s) added";
                    await Task.Delay(1500);
                    SupplierAdded?.Invoke();
                    await Navigation.PopAsync();
                    return true;
                }
            }

            return false;
        }

        private void SaveExtractedBills()
        {
            var type = _selected.Category switch
            {
                "Energy" => "Electricity",
                "Water" => "Water",
                "Telecom" => _selected.Icon == "\U0001F4F1" ? "Mobile" : "Broadband",
                _ => "Other",
            };

            foreach (var bill in _extractedBills)
            {
                DateTime billDate;
                if (!DateTime.TryParse(bill.DateStr, culture, DateTimeStyles.None, out billDate))
                    DateTime.TryParse(bill.DateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out billDate);
                if (billDate == default) billDate = DateTime.Now;

                MockDataService.AddBill(new MockUtilityBill
                {
                    BillDate = billDate,
                    Supplier = _selected.Name,
                    FuelType = type,
                    Amount = bill.Amount,
                    UnitsUsed = 0,
                    Period = billDate.ToString("MMM yyyy"),
                    Address = "",
                });
            }

            var suppliers = MockDataService.GetSuppliers();
            var supplier = suppliers.FirstOrDefault(s => s.Name == _selected.Name);
            if (supplier != null && _extractedBills.Count > 0)
            {
                supplier.MonthlyCost = Math.Round(_extractedBills.Average(b => b.Amount), 2);
            }
        }

        // Manual page scrape. Shows what it found and asks before saving; never leaves this page.
        private async void OnExtractClicked(object sender, EventArgs e)
        {
            ExtractBtn.IsEnabled = false;
            ExtractBtn.Text = "Scraping...";
            SupplierLog.Write($"Scrape Bills tapped on {UrlLabel.Text} ({_capturedApiResponses.Count} API responses captured)");

            try
            {
                var source = "";
                if (TryParseFromCapturedApi() && _extractedBills.Count > 0)
                    source = "API data";
                else
                {
                    var js = GetSupplierScraper(_selected?.Name ?? "");
                    var result = await SupplierWebView.EvaluateJavaScriptAsync(js);
                    SupplierLog.Write($"Page scrape returned: {Truncate(result ?? "(null)", 500)}");
                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.Trim('"').Replace("\\\"", "\"").Replace("\\\\", "\\");
                        ParseExtractedData(result);
                        if (_extractedBills.Count > 0) source = "page text";
                    }
                }

                if (_extractedBills.Count == 0)
                {
                    SupplierLog.Write("Scrape found no bills.");
                    await DisplayAlert("No bills found",
                        "Nothing on this page looked like a bill. Open a bill PDF or click Download on the supplier's site instead; it will be added automatically.",
                        "OK");
                    return;
                }

                var preview = string.Join("\n", _extractedBills.Take(8).Select(b => $"• {b.DateStr}  £{b.Amount:N2}"));
                if (_extractedBills.Count > 8) preview += $"\n… and {_extractedBills.Count - 8} more";
                SupplierLog.Write($"Scrape found {_extractedBills.Count} candidate bill(s) from {source}: {preview.Replace("\n", " | ")}");

                var save = await DisplayAlert($"{_extractedBills.Count} possible bill(s) from {source}",
                    preview + "\n\nAdd these to your bill history? Scraped figures can be wrong; PDF bills are more reliable.",
                    "Add", "Don't add");
                if (!save)
                {
                    SupplierLog.Write("User declined scraped bills.");
                    return;
                }

                SaveExtractedBills();
                SupplierAdded?.Invoke();
                SupplierLog.Write($"Saved {_extractedBills.Count} scraped bill(s) for {_selected?.Name}.");
                PageSubtitle.Text = $"{_extractedBills.Count} bill(s) added from {source}. You can keep browsing.";
            }
            catch (Exception ex)
            {
                SupplierLog.Error("Scrape", ex);
                await DisplayAlert("Scrape Error", ex.Message + "\n\nDetails are in the log (Log button).", "OK");
            }
            finally
            {
                ExtractBtn.IsEnabled = true;
                ExtractBtn.Text = "Scrape Bills";
            }
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return s.Length <= max ? s : s.Substring(0, max) + "...";
        }

        private bool TryParseFromCapturedApi()
        {
            _extractedBills.Clear();

            foreach (var json in _capturedApiResponses)
            {
                try
                {
                    var token = Newtonsoft.Json.Linq.JToken.Parse(json);
                    SearchForBills(token);
                    if (_extractedBills.Count > 0)
                        return true;
                }
                catch { }
            }
            return false;
        }

        private void SearchForBills(Newtonsoft.Json.Linq.JToken token)
        {
            if (_extractedBills.Count >= 24) return;

            if (token is Newtonsoft.Json.Linq.JArray arr)
            {
                // Check if this array contains bill-like objects
                var bills = new List<ExtractedBill>();
                foreach (var item in arr)
                {
                    if (item is Newtonsoft.Json.Linq.JObject itemObj)
                    {
                        var b = TryExtractBillFromObject(itemObj);
                        if (b != null) bills.Add(b);
                    }
                }
                if (bills.Count >= 2)
                {
                    _extractedBills.AddRange(bills);
                    return;
                }

                // Recurse into array items
                foreach (var item in arr)
                    SearchForBills(item);
            }
            else if (token is Newtonsoft.Json.Linq.JObject obj)
            {
                foreach (var prop in obj.Properties())
                {
                    var name = prop.Name.ToLower();
                    // Prioritise properties that sound like bill collections
                    if (prop.Value is Newtonsoft.Json.Linq.JArray propArr &&
                        (name.Contains("bill") || name.Contains("statement") || name.Contains("invoice") ||
                         name.Contains("payment") || name.Contains("transaction") || name.Contains("charge") ||
                         name.Contains("history") || name.Contains("ledger")))
                    {
                        var bills = new List<ExtractedBill>();
                        foreach (var item in propArr)
                        {
                            if (item is Newtonsoft.Json.Linq.JObject itemObj)
                            {
                                var b = TryExtractBillFromObject(itemObj);
                                if (b != null) bills.Add(b);
                            }
                        }
                        if (bills.Count >= 1)
                        {
                            _extractedBills.AddRange(bills);
                            return;
                        }
                    }

                    // Recurse into child objects/arrays
                    if (prop.Value is Newtonsoft.Json.Linq.JObject || prop.Value is Newtonsoft.Json.Linq.JArray)
                        SearchForBills(prop.Value);

                    if (_extractedBills.Count >= 24) return;
                }
            }
        }

        private ExtractedBill TryExtractBillFromObject(Newtonsoft.Json.Linq.JObject obj)
        {
            var amountNames = new[] {
                "amount", "total", "value", "billAmount", "totalAmount", "netAmount",
                "grossAmount", "cost", "charge", "totalGrossAmount", "totalNetAmount",
                "amountOwed", "chargesTotal", "balance", "net", "gross",
                "totalCharges", "totalCost"
            };
            var dateNames = new[] {
                "date", "billDate", "invoiceDate", "statementDate", "createdDate",
                "issueDate", "dueDate", "periodEnd", "billingDate", "issuedDate",
                "fromDate", "toDate", "startDate", "endDate", "periodFrom", "periodTo",
                "billedFrom", "billedTo", "createdAt", "updatedAt"
            };

            decimal? amount = null;
            string dateStr = null;

            foreach (var n in amountNames)
            {
                var val = FindTokenDeep(obj, n);
                if (val == null) continue;
                var s = val.ToString().Replace("£", "").Replace(",", "").Trim();
                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var a))
                {
                    if (a == 0) continue;
                    // EDF Kraken API uses pence for some values — amounts over 500 are likely pence
                    amount = Math.Abs(a) > 500 ? Math.Round(Math.Abs(a) / 100m, 2) : Math.Abs(a);
                    break;
                }
            }

            foreach (var n in dateNames)
            {
                var val = FindTokenDeep(obj, n);
                if (val == null) continue;
                var s = val.ToString().Trim();
                if (string.IsNullOrEmpty(s) || s == "null") continue;
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                {
                    dateStr = d.ToString("dd MMM yyyy");
                    break;
                }
                if (DateTime.TryParse(s, culture, DateTimeStyles.None, out var d2))
                {
                    dateStr = d2.ToString("dd MMM yyyy");
                    break;
                }
                if (s.Length >= 6)
                {
                    dateStr = s;
                    break;
                }
            }

            if (amount.HasValue && amount.Value > 0)
                return new ExtractedBill { Amount = amount.Value, DateStr = dateStr ?? "" };

            return null;
        }

        private static Newtonsoft.Json.Linq.JToken FindTokenDeep(Newtonsoft.Json.Linq.JObject obj, string name)
        {
            // Direct property first
            var val = obj[name];
            if (val != null && val.Type != Newtonsoft.Json.Linq.JTokenType.Null &&
                val.Type != Newtonsoft.Json.Linq.JTokenType.Object &&
                val.Type != Newtonsoft.Json.Linq.JTokenType.Array)
                return val;

            // Case-insensitive search
            foreach (var prop in obj.Properties())
            {
                if (prop.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    prop.Value.Type != Newtonsoft.Json.Linq.JTokenType.Null &&
                    prop.Value.Type != Newtonsoft.Json.Linq.JTokenType.Object &&
                    prop.Value.Type != Newtonsoft.Json.Linq.JTokenType.Array)
                    return prop.Value;
            }

            return null;
        }

        private string FindJsonValue(Newtonsoft.Json.Linq.JObject obj, params string[] names)
        {
            foreach (var n in names)
            {
                var val = obj.SelectToken(n) ?? obj.SelectToken($"data.{n}");
                if (val != null)
                {
                    var s = val.ToString().Trim();
                    if (!string.IsNullOrEmpty(s) && s != "0" && s != "null") return s;
                }
            }
            return null;
        }

        private static string GetSupplierScraper(string supplierName)
        {
            return BuildScraper();
        }

        private static string BuildScraper()
        {
            return
                "try{" +
                "var d={bills:[],account:'',balance:'',usage:'',title:document.title||'',len:(document.body.innerText||'').length};" +
                "var t=document.body.innerText||'';" +
                "var am=t.match(/(?:balance|amount\\s*due|you\\s*owe|please\\s*pay)[^£]*£([\\d,]+\\.?\\d*)/i);" +
                "if(am)d.balance=am[1].replace(/,/g,'');" +
                "var um=t.match(/(\\d[\\d,]*\\.?\\d*)\\s*kWh/i);" +
                "if(um)d.usage=um[1].replace(/,/g,'')+' kWh';" +
                "var els=document.querySelectorAll('tr,li,div,article,section,a,span,p');" +
                "for(var i=0;i<els.length&&d.bills.length<24;i++){" +
                "var e=els[i];if(e.children.length>10)continue;" +
                "var x=e.innerText||'';if(x.length>500||x.length<5)continue;" +
                "var a=x.match(/£(\\d[\\d,]*\\.?\\d{0,2})/);" +
                "var dt=x.match(/(\\d{1,2})\\s*(Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:tember)?|Oct(?:ober)?|Nov(?:ember)?|Dec(?:ember)?)\\s*(\\d{2,4})/i);" +
                "if(!dt)dt=x.match(/(\\d{1,2})[\\/\\-](\\d{1,2})[\\/\\-](\\d{2,4})/);" +
                "if(a&&dt){" +
                "var v=a[1].replace(/,/g,'');" +
                "var dd=dt[0];" +
                "var dup=d.bills.some(function(b){return b.amount===v&&b.date===dd;});" +
                "if(!dup&&parseFloat(v)>0)d.bills.push({amount:v,date:dd});" +
                "}}" +
                "if(d.bills.length===0){" +
                "var amts=t.match(/£\\d[\\d,]*\\.\\d{2}/g)||[];" +
                "var dts=t.match(/\\d{1,2}\\s+(?:Jan(?:uary)?|Feb(?:ruary)?|Mar(?:ch)?|Apr(?:il)?|May|Jun(?:e)?|Jul(?:y)?|Aug(?:ust)?|Sep(?:tember)?|Oct(?:ober)?|Nov(?:ember)?|Dec(?:ember)?)\\s+\\d{2,4}/gi)||[];" +
                "var c=Math.min(amts.length,dts.length,24);" +
                "for(var j=0;j<c;j++){d.bills.push({amount:amts[j].replace(/[£,]/g,''),date:dts[j]});}}" +
                "JSON.stringify(d);" +
                "}catch(ex){JSON.stringify({error:ex.message,bills:[]});}";
        }

        private void ParseExtractedData(string json)
        {
            try
            {
                var data = Newtonsoft.Json.Linq.JObject.Parse(json);
                var bills = data["bills"] as Newtonsoft.Json.Linq.JArray;

                _extractedBills.Clear();

                if (bills != null && bills.Count > 0)
                {
                    foreach (var b in bills)
                    {
                        if (decimal.TryParse(b["amount"]?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
                        {
                            _extractedBills.Add(new ExtractedBill
                            {
                                Amount = amount,
                                DateStr = b["date"]?.ToString() ?? "",
                            });
                        }
                    }
                }
            }
            catch { }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            if (Step2.IsVisible)
            {
                Step2.IsVisible = false;
                Step1.IsVisible = true;
                ExtractBtn.IsVisible = false;
                ForgetLoginBtn.IsVisible = false;
                PageTitle.Text = "Connect Supplier";
                PageSubtitle.Text = "Choose your supplier";
                _loggedIn = false;
                return;
            }
            await Navigation.PopAsync();
        }

        private record SupplierInfo(string Name, string Icon, string Colour, string Category, string LoginUrl);
        private class ExtractedBill
        {
            public decimal Amount { get; set; }
            public string DateStr { get; set; }
        }
    }
}
