namespace SmartCubeMobile.Services
{
    public class VehicleLookupResult
    {
        public string Registration { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Colour { get; set; }
        public string FuelType { get; set; }
        public int Year { get; set; }
        public int EngineCC { get; set; }
        public string FirstRegistered { get; set; }
        public string MotExpiry { get; set; }
        public int Mileage { get; set; }
    }

    public static class VehicleLookupService
    {
        public static async Task<(VehicleLookupResult Data, string Error)> LookupRegistration(string registration, WebView webView)
        {
            var reg = registration.Replace(" ", "").ToUpper().Trim();
            if (reg.Length < 2) return (null, "Invalid registration");

            try
            {
                var url = $"https://www.check-mot.service.gov.uk/results?registration={reg}&checkRecalls=true";

                var navTcs = new TaskCompletionSource<bool>();
                void onNav(object s, WebNavigatedEventArgs e) => navTcs.TrySetResult(true);
                webView.Navigated += onNav;
                webView.Source = new UrlWebViewSource { Url = url };

                using var cts = new CancellationTokenSource(20000);
                try { await navTcs.Task.WaitAsync(cts.Token); }
                catch (OperationCanceledException) { }
                webView.Navigated -= onNav;

                await Task.Delay(2000);

                var extractJs = @"(function(){
    var t = document.title || '';
    if (t.indexOf('Check the MOT') === -1) return 'WAITING';
    var lines = document.body.innerText.split('\n');
    var clean = [];
    for (var i = 0; i < lines.length; i++) {
        var s = lines[i].trim();
        if (s.length > 0) clean.push(s);
    }
    function after(label) {
        for (var i = 0; i < clean.length; i++) {
            if (clean[i] === label && i + 1 < clean.length) return clean[i + 1];
        }
        return '';
    }
    var title = t.split(' - ')[0].trim();
    var sp = title.indexOf(' ');
    var make = sp > 0 ? title.substring(0, sp) : title;
    var model = sp > 0 ? title.substring(sp + 1) : '';
    var colour = after('Colour');
    var fuel = after('Fuel type');
    var dateReg = after('Date registered');
    var motUntil = after('MOT valid until');
    var mileage = '0';
    var body = document.body.innerText;
    var mm = body.match(/([\d,]+)\s*miles/);
    if (mm) mileage = mm[1].replace(/,/g, '');
    return make + '|||' + model + '|||' + colour + '|||' + fuel + '|||' + dateReg + '|||' + motUntil + '|||' + mileage;
})()";

                string lastError = "";
                string lastResult = "";
                string debugInfo = "";

#if WINDOWS
                var nativeWebView = webView.Handler?.PlatformView as Microsoft.UI.Xaml.Controls.WebView2;
                if (nativeWebView == null)
                {
                    debugInfo = "Handler.PlatformView is null";
                }
                else if (nativeWebView.CoreWebView2 == null)
                {
                    try
                    {
                        await nativeWebView.EnsureCoreWebView2Async();
                    }
                    catch (Exception initEx)
                    {
                        debugInfo = $"EnsureCoreWebView2 failed: {initEx.Message}";
                    }
                }

                if (nativeWebView?.CoreWebView2 != null)
                {
                    debugInfo = "Using native CoreWebView2";

                    for (int i = 0; i < 25; i++)
                    {
                        await Task.Delay(1000);

                        string result;
                        try
                        {
                            result = await nativeWebView.CoreWebView2.ExecuteScriptAsync(extractJs);
                        }
                        catch (Exception jsEx)
                        {
                            lastError = jsEx.Message;
                            continue;
                        }

                        if (string.IsNullOrEmpty(result) || result == "null") continue;

                        var val = result.Trim('"').Replace("\\u0027", "'");
                        lastResult = val;

                        if (val == "WAITING") continue;
                        if (!val.Contains("|||")) continue;

                        var parts = val.Split("|||");
                        if (parts.Length < 7 || string.IsNullOrEmpty(parts[0]))
                            continue;

                        try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }

                        var lookupResult = new VehicleLookupResult
                        {
                            Registration = reg,
                            Make = TitleCase(parts[0]),
                            Model = TitleCase(parts[1]),
                            Colour = parts[2],
                            FuelType = parts[3],
                            FirstRegistered = parts[4],
                            MotExpiry = parts[5],
                        };

                        if (int.TryParse(parts[6], out var miles))
                            lookupResult.Mileage = miles;

                        if (DateTime.TryParse(lookupResult.FirstRegistered, out var regDate))
                            lookupResult.Year = regDate.Year;

                        return (lookupResult, null);
                    }
                }
                else
                {
                    for (int i = 0; i < 25; i++)
                    {
                        await Task.Delay(1000);

                        string result;
                        try { result = await webView.EvaluateJavaScriptAsync(extractJs); }
                        catch (Exception jsEx) { lastError = jsEx.Message; continue; }

                        if (string.IsNullOrEmpty(result)) continue;

                        var val = result.Trim('"').Replace("\\n", "\n");
                        lastResult = val;

                        if (val == "WAITING" || val == "null") continue;
                        if (!val.Contains("|||")) continue;

                        var parts = val.Split("|||");
                        if (parts.Length < 7 || string.IsNullOrEmpty(parts[0]))
                            continue;

                        try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }

                        var lookupResult = new VehicleLookupResult
                        {
                            Registration = reg,
                            Make = TitleCase(parts[0]),
                            Model = TitleCase(parts[1]),
                            Colour = parts[2],
                            FuelType = parts[3],
                            FirstRegistered = parts[4],
                            MotExpiry = parts[5],
                        };

                        if (int.TryParse(parts[6], out var miles))
                            lookupResult.Mileage = miles;

                        if (DateTime.TryParse(lookupResult.FirstRegistered, out var regDate))
                            lookupResult.Year = regDate.Year;

                        return (lookupResult, null);
                    }
                }
#else
                for (int i = 0; i < 25; i++)
                {
                    await Task.Delay(1000);
                    string result;
                    try { result = await webView.EvaluateJavaScriptAsync(extractJs); }
                    catch (Exception jsEx) { lastError = jsEx.Message; continue; }
                    if (string.IsNullOrEmpty(result)) continue;
                    var val = result.Trim('"');
                    lastResult = val;
                    if (val == "WAITING" || val == "null") continue;
                    if (!val.Contains("|||")) continue;
                    var parts = val.Split("|||");
                    if (parts.Length < 7 || string.IsNullOrEmpty(parts[0])) continue;
                    try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }
                    var lookupResult = new VehicleLookupResult
                    {
                        Registration = reg,
                        Make = TitleCase(parts[0]),
                        Model = TitleCase(parts[1]),
                        Colour = parts[2],
                        FuelType = parts[3],
                        FirstRegistered = parts[4],
                        MotExpiry = parts[5],
                    };
                    if (int.TryParse(parts[6], out var miles))
                        lookupResult.Mileage = miles;
                    if (DateTime.TryParse(lookupResult.FirstRegistered, out var regDate))
                        lookupResult.Year = regDate.Year;
                    return (lookupResult, null);
                }
#endif

                try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }
                return (null, $"Failed. Debug=[{debugInfo}] LastResult=[{lastResult}] Error=[{lastError}]");
            }
            catch (Exception ex)
            {
                return (null, $"Lookup failed: {ex.Message}");
            }
        }

        public static async Task<(string TestDate, string Result, string MotExpiry, int Mileage, List<string> Advisories, string Error)> FetchMotDetails(string registration, WebView webView)
        {
            var reg = registration.Replace(" ", "").ToUpper().Trim();
            try
            {
                var url = $"https://www.check-mot-history.co.uk/vehicle/{reg.ToLower()}";
                var navTcs = new TaskCompletionSource<bool>();
                void onNav(object s, WebNavigatedEventArgs e) => navTcs.TrySetResult(true);
                webView.Navigated += onNav;
                webView.Source = new UrlWebViewSource { Url = url };
                using var cts = new CancellationTokenSource(20000);
                try { await navTcs.Task.WaitAsync(cts.Token); }
                catch (OperationCanceledException) { }
                webView.Navigated -= onNav;
                await Task.Delay(3000);

                var extractJs = @"(function(){
    var body = document.body.innerText;
    if (!body || body.length < 50) return 'WAITING';
    var em = body.match(/MOT Expires?:\s*(\d{2}\/\d{2}\/\d{4})/i);
    var motExpiry = em ? em[1] : '';
    if (!motExpiry) {
        em = body.match(/MOT Expiry Date:\s*(\d{2}\/\d{2}\/\d{4})/i);
        if (em) motExpiry = em[1];
    }
    var lines = body.split('\n');
    var tests = [];
    var cur = null;
    for (var i = 0; i < lines.length; i++) {
        var line = lines[i].trim();
        if (!line) continue;
        var dm = line.match(/^Date Tested:\s*(\d{2}\/\d{2}\/\d{4})/i);
        if (dm) {
            if (cur) tests.push(cur);
            cur = {date:dm[1], result:'', mileage:'', expiry:'', advisories:[]};
            continue;
        }
        if (!cur) continue;
        var rm = line.match(/Test Result:.*?(Passed|Failed)/i);
        if (rm) { cur.result = rm[1]; continue; }
        var mm = line.match(/^Mileage:\s*([\d,]+)/i);
        if (mm) { cur.mileage = mm[1].replace(/,/g,''); continue; }
        var xm = line.match(/^MOT Expiry Date:\s*(\d{2}\/\d{2}\/\d{4})/i);
        if (xm) { cur.expiry = xm[1]; continue; }
        if (line.match(/^(Advisory|Dangerous|Major|Minor)\s*-\s*/i)) {
            cur.advisories.push(line);
        }
    }
    if (cur) tests.push(cur);
    if (tests.length === 0 && !motExpiry) return 'WAITING';
    var testDate = tests.length > 0 ? tests[0].date : '';
    var testResult = tests.length > 0 ? tests[0].result : '';
    var mileage = tests.length > 0 ? tests[0].mileage : '';
    if (!motExpiry && tests.length > 0) motExpiry = tests[0].expiry;
    var advisories = [];
    for (var t = 0; t < tests.length && t < 4; t++) {
        if (tests[t].advisories.length > 0) {
            advisories = tests[t].advisories;
            break;
        }
    }
    return testDate + '|||' + testResult + '|||' + mileage + '|||' + motExpiry + '|||' + advisories.join('###');
})()";

#if WINDOWS
                var nativeWebView = webView.Handler?.PlatformView as Microsoft.UI.Xaml.Controls.WebView2;
                if (nativeWebView?.CoreWebView2 == null)
                {
                    try { await nativeWebView?.EnsureCoreWebView2Async(); }
                    catch { }
                }
                if (nativeWebView?.CoreWebView2 != null)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        await Task.Delay(1000);
                        string result;
                        try { result = await nativeWebView.CoreWebView2.ExecuteScriptAsync(extractJs); }
                        catch { continue; }
                        if (string.IsNullOrEmpty(result) || result == "null") continue;
                        var val = result.Trim('"').Replace("\\u0027", "'");
                        if (val == "WAITING") continue;
                        if (!val.Contains("|||")) continue;
                        var parts = val.Split("|||");
                        if (parts.Length < 5) continue;
                        if (string.IsNullOrWhiteSpace(parts[0]) && string.IsNullOrWhiteSpace(parts[3])) continue;
                        try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }
                        var advisories = parts[4].Split("###", StringSplitOptions.RemoveEmptyEntries).ToList();
                        int.TryParse(parts[2], out var miles);
                        return (parts[0], parts[1], parts[3], miles, advisories, null);
                    }
                }
#endif
                try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }
                return (null, null, null, 0, new List<string>(), "Could not fetch MOT data");
            }
            catch (Exception ex)
            {
                return (null, null, null, 0, new List<string>(), ex.Message);
            }
        }

        private static string TitleCase(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return string.Join(" ", s.Split(new[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Length > 0 ? char.ToUpper(w[0]) + w[1..].ToLower() : w));
        }
    }
}
