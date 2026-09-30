using System.Text.RegularExpressions;

namespace SmartCubeMobile.Services
{
    public class PropertyValueResult
    {
        public decimal EstimatedValue { get; set; }
        public decimal LastSalePrice { get; set; }
        public string LastSaleDate { get; set; }
        public string PropertyType { get; set; }
        public string Bedrooms { get; set; }
        public string Tenure { get; set; }
        public string CouncilTaxBand { get; set; }
        public string EpcRating { get; set; }
        public string FloorArea { get; set; }
        public List<(string Date, decimal Price)> PriceHistory { get; set; } = new();
    }

    public class PropertySearchResult
    {
        public string Address { get; set; }
        public string EpcRating { get; set; }
        public string CertificateUrl { get; set; }
        public string PropertyType { get; set; }
    }

    public class PropertyDetailResult
    {
        public string EpcRating { get; set; }
        public string PropertyType { get; set; }
        public string FloorArea { get; set; }
        public string WallsType { get; set; }
        public string RoofType { get; set; }
        public string HeatingType { get; set; }
        public string WindowsType { get; set; }
        public string HotWater { get; set; }
        public string Lighting { get; set; }
        public string TotalFloorArea { get; set; }
        public string EpcExpiry { get; set; }
    }

    public static class PropertyLookupService
    {
        public static async Task<(PropertyDetailResult Details, string Error)> FetchCertificateDetails(string certPath, WebView webView)
        {
            try
            {
                var url = certPath.StartsWith("http")
                    ? certPath
                    : $"https://find-energy-certificate.service.gov.uk{certPath}";

                var navTcs = new TaskCompletionSource<bool>();
                void onNav(object s, WebNavigatedEventArgs e) => navTcs.TrySetResult(true);
                webView.Navigated += onNav;
                webView.Source = new UrlWebViewSource { Url = url };
                using var cts = new CancellationTokenSource(20000);
                try { await navTcs.Task.WaitAsync(cts.Token); }
                catch (OperationCanceledException) { }
                webView.Navigated -= onNav;
                await Task.Delay(2500);

                var extractJs = @"(function(){
    var body = document.body.innerText;
    if (!body || body.length < 100) return 'WAITING';
    function find(label) {
        var lines = body.split('\n');
        var lab = label.toLowerCase();
        for (var i = 0; i < lines.length; i++) {
            var line = lines[i].trim();
            var lower = line.toLowerCase();
            if (lower.indexOf(lab) === 0) {
                var tabs = line.split('\t');
                if (tabs.length >= 2) {
                    for (var t = 1; t < tabs.length; t++) {
                        var v = tabs[t].trim();
                        if (v && !v.match(/^(Good|Average|Poor|Very good|Very poor)$/i)) return v;
                    }
                    return tabs[1].trim();
                }
                for (var j = i+1; j < lines.length && j < i+4; j++) {
                    var v = lines[j].trim();
                    if (v.length > 0 && v.length < 200 && v.toLowerCase() !== lab) return v;
                }
            }
        }
        return '';
    }
    var rating = '';
    var rm = body.match(/Energy rating\s*([A-G])/i);
    if (rm) rating = rm[1].toUpperCase();
    if (!rating) { rm = body.match(/\b([A-G])\s*\d{1,3}\s*\|\s*\d/); if (rm) rating = rm[1].toUpperCase(); }
    var propType = find('Property type');
    var floorArea = find('Total floor area');
    if (!floorArea) floorArea = find('Floor area');
    var walls = find('Wall');
    var roof = find('Roof');
    var heating = find('Main heating');
    if (!heating) heating = find('Heating');
    var windows = find('Window');
    var hotWater = find('Hot water');
    var lighting = find('Lighting');
    var expiry = '';
    var em = body.match(/Valid until\s+(\d{1,2}\s+\w+\s+\d{4})/i);
    if (em) expiry = em[1];
    return [rating,propType,floorArea,walls,roof,heating,windows,hotWater,lighting,expiry].join('|||');
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
                    for (int i = 0; i < 20; i++)
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
                        if (parts.Length < 10) continue;
                        try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }
                        return (new PropertyDetailResult
                        {
                            EpcRating = parts[0],
                            PropertyType = parts[1],
                            TotalFloorArea = parts[2],
                            WallsType = parts[3],
                            RoofType = parts[4],
                            HeatingType = parts[5],
                            WindowsType = parts[6],
                            HotWater = parts[7],
                            Lighting = parts[8],
                            EpcExpiry = parts[9],
                        }, null);
                    }
                }
#endif
                try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }
                return (null, "Could not fetch certificate details");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public static async Task<(string Band, string Error)> FetchCouncilTaxBand(string postcode, string address, WebView webView)
        {
            try
            {
                var url = "https://www.tax.service.gov.uk/check-council-tax-band/search";

                var navTcs = new TaskCompletionSource<bool>();
                void onNav(object s, WebNavigatedEventArgs e) => navTcs.TrySetResult(true);
                webView.Navigated += onNav;
                webView.Source = new UrlWebViewSource { Url = url };
                using var cts = new CancellationTokenSource(20000);
                try { await navTcs.Task.WaitAsync(cts.Token); }
                catch (OperationCanceledException) { }
                webView.Navigated -= onNav;
                await Task.Delay(2000);

                var pc = postcode.Trim().ToUpper();
                var addrLower = address.ToLower().Trim();
                var houseNum = "";
                var hm = Regex.Match(addrLower, @"^(\d+)");
                if (hm.Success) houseNum = hm.Groups[1].Value;

                var fillAndSubmitJs = @"(function(){
    var input = document.querySelector('input[name=""postcode""]') || document.querySelector('input[type=""text""]');
    if (!input) return 'NOINPUT';
    input.value = '" + pc.Replace("'", "\\'") + @"';
    var btn = document.querySelector('button[type=""submit""]') || document.querySelector('input[type=""submit""]');
    if (!btn) { var form = input.closest('form'); if (form) { form.submit(); return 'SUBMITTED'; } return 'NOBTN'; }
    btn.click();
    return 'SUBMITTED';
})()";

                var streetWord = "";
                var addrParts = address.Split(',')[0].Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (addrParts.Length >= 2) streetWord = addrParts[1].ToLower();

                var extractJs = @"(function(){
    var body = document.body.innerText;
    if (!body || body.length < 50) return 'WAITING';
    if (body.indexOf('Enter a postcode') >= 0 && body.indexOf('Band') < 0) return 'WAITING';
    var houseNum = '" + houseNum.Replace("'", "\\'") + @"';
    var streetWord = '" + streetWord.Replace("'", "\\'") + @"';
    var lines = body.split('\n');
    for (var i = 0; i < lines.length; i++) {
        var line = lines[i].trim().toLowerCase();
        var hasNum = houseNum && line.match(new RegExp('\\b' + houseNum + '\\b'));
        var hasStreet = streetWord && line.indexOf(streetWord) >= 0;
        if (hasNum && hasStreet) {
            for (var j = Math.max(0,i-2); j < i+6 && j < lines.length; j++) {
                var m = lines[j].trim().match(/\bBand\s+([A-H])\b/i);
                if (m) return m[1];
            }
        }
    }
    for (var i = 0; i < lines.length; i++) {
        var m = lines[i].trim().match(/\bBand\s+([A-H])\b/i);
        if (m) return m[1];
    }
    if (body.indexOf('Band') < 0) return 'NOBANDS';
    return 'NOTFOUND';
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
                    try { await nativeWebView.CoreWebView2.ExecuteScriptAsync(fillAndSubmitJs); }
                    catch { }
                    await Task.Delay(4000);

                    for (int i = 0; i < 20; i++)
                    {
                        await Task.Delay(1500);
                        string result;
                        try { result = await nativeWebView.CoreWebView2.ExecuteScriptAsync(extractJs); }
                        catch { continue; }
                        if (string.IsNullOrEmpty(result) || result == "null") continue;
                        var val = result.Trim('"');
                        if (val == "WAITING") continue;
                        if (val == "NOBANDS") continue;
                        try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }
                        if (val == "NOTFOUND") return (null, "Council tax band not found");
                        return (val, null);
                    }
                }
#endif
                try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }
                return (null, "Could not fetch council tax band");
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
        }

        public static async Task<(PropertyValueResult Result, string Error)> FetchPropertyValue(string address, string postcode, WebView webView, Action<bool> setCaptchaVisible = null)
        {
            try
            {
                var slug = Regex.Replace(address.Replace(",", "").Trim(), @"\s+", "-").ToLower();
                var url = $"https://themovemarket.com/tools/propertyprices/{slug}";

                var navTcs = new TaskCompletionSource<bool>();
                void onNav(object s, WebNavigatedEventArgs e) => navTcs.TrySetResult(true);
                webView.Navigated += onNav;
                webView.Source = new UrlWebViewSource { Url = url };
                using var cts = new CancellationTokenSource(20000);
                try { await navTcs.Task.WaitAsync(cts.Token); }
                catch (OperationCanceledException) { }
                webView.Navigated -= onNav;
                await Task.Delay(2000);

                var captchaCheckJs = @"(function(){
    var t = document.title || '';
    if (t.indexOf('Verification') >= 0 || t.indexOf('Human') >= 0) return 'CAPTCHA';
    var body = document.body.innerText || '';
    if (body.indexOf('confirm you are human') >= 0) return 'CAPTCHA';
    return 'OK';
})()";

                var extractJs = @"(function(){
    var body = document.body.innerText;
    if (!body || body.length < 100) return 'WAITING';
    if (body.indexOf('confirm you are human') >= 0) return 'CAPTCHA';
    if (body.indexOf('Estimated Value') < 0 && body.indexOf('valued at') < 0) return 'WAITING';
    var estimated = '';
    var m = body.match(/Estimated\s+Value\s*£([\d,]+)/i);
    if (m) estimated = m[1].replace(/,/g,'');
    if (!estimated) { m = body.match(/valued\s+at\s*£([\d,]+)/i); if (m) estimated = m[1].replace(/,/g,''); }
    var propType = '';
    m = body.match(/\b(Detached\s+House|Semi-Detached\s+House|Semi\s+Detached|Terraced|Flat|End.?Terrace|Maisonette|Bungalow|Apartment)\b/i);
    if (m) propType = m[1];
    var tenure = '';
    m = body.match(/\b(Freehold|Leasehold)\b/i);
    if (m) tenure = m[1];
    var bedrooms = '';
    m = body.match(/Bedrooms\s+(\d+)/i);
    if (!m) m = body.match(/(\d+)\s*bed(?:room)?/i);
    if (m) bedrooms = m[1];
    var councilTax = '';
    m = body.match(/Council\s+Tax\s+Band\s+([A-H])/i);
    if (m) councilTax = m[1];
    var epc = '';
    m = body.match(/EPC\s+Rating\s+\d+\s*\(([A-G])\)/i);
    if (!m) m = body.match(/EPC\s+Rating\s+([A-G])\b/i);
    if (m) epc = m[1].toUpperCase();
    var floorArea = '';
    m = body.match(/Size\s+([\d,]+)\s*sq\s*ft/i);
    if (m) floorArea = m[1] + ' sq ft';
    if (!floorArea) { m = body.match(/([\d,]+)\s*sq(?:uare)?\s*(?:feet|ft)/i); if (m) floorArea = m[1] + ' sq ft'; }
    var lastPrice = ''; var lastDate = '';
    m = body.match(/last\s+sold\s+.*?(\w+\s+\d{4}).*?£([\d,]+)/i);
    if (m) { lastDate = m[1]; lastPrice = m[2].replace(/,/g,''); }
    if (!lastPrice) { m = body.match(/sold\s+.*?(\w+\s+\d{4}).*?£([\d,]+)/i); if (m) { lastDate = m[1]; lastPrice = m[2].replace(/,/g,''); } }
    var sales = [];
    var lines = body.split('\n');
    for (var i = 0; i < lines.length; i++) {
        var sm = lines[i].match(/(\w+\s+\d{4})\s+(?:.*?)£([\d,]+)/);
        if (sm) { sales.push(sm[1] + ':' + sm[2].replace(/,/g,'')); if (sales.length >= 10) break; }
    }
    if (!estimated && !lastPrice && sales.length === 0) return 'NODATA';
    return [estimated,lastPrice,lastDate,propType,bedrooms,tenure,councilTax,epc,floorArea,sales.join('###')].join('|||');
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
                    string captchaCheck;
                    try { captchaCheck = await nativeWebView.CoreWebView2.ExecuteScriptAsync(captchaCheckJs); }
                    catch { captchaCheck = ""; }
                    var isCaptcha = captchaCheck?.Trim('"') == "CAPTCHA";
                    if (isCaptcha) setCaptchaVisible?.Invoke(true);

                    for (int i = 0; i < 120; i++)
                    {
                        await Task.Delay(isCaptcha ? 2000 : 1500);
                        string result;
                        try { result = await nativeWebView.CoreWebView2.ExecuteScriptAsync(extractJs); }
                        catch { continue; }
                        if (string.IsNullOrEmpty(result) || result == "null") continue;
                        var val = result.Trim('"').Replace("\\u0027", "'");
                        if (val == "WAITING") continue;
                        if (val == "CAPTCHA") { isCaptcha = true; setCaptchaVisible?.Invoke(true); continue; }
                        if (val == "NODATA") { i = Math.Max(i, 115); continue; }
                        if (!val.Contains("|||")) continue;

                        setCaptchaVisible?.Invoke(false);
                        var parts = val.Split("|||");
                        if (parts.Length < 10) continue;
                        try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }

                        var pvr = new PropertyValueResult
                        {
                            PropertyType = parts[3],
                            Bedrooms = parts[4],
                            Tenure = parts[5],
                            CouncilTaxBand = parts[6],
                            EpcRating = parts[7],
                            FloorArea = parts[8],
                        };
                        if (decimal.TryParse(parts[0], out var est)) pvr.EstimatedValue = est;
                        if (decimal.TryParse(parts[1], out var lsp)) pvr.LastSalePrice = lsp;
                        pvr.LastSaleDate = parts[2];

                        if (!string.IsNullOrEmpty(parts[9]))
                        {
                            foreach (var sale in parts[9].Split("###", StringSplitOptions.RemoveEmptyEntries))
                            {
                                var sp = sale.Split(':');
                                if (sp.Length == 2 && decimal.TryParse(sp[1], out var price))
                                    pvr.PriceHistory.Add((sp[0], price));
                            }
                        }
                        if (pvr.LastSalePrice == 0 && pvr.PriceHistory.Count > 0)
                        {
                            pvr.LastSalePrice = pvr.PriceHistory[0].Price;
                            pvr.LastSaleDate = pvr.PriceHistory[0].Date;
                        }
                        return (pvr, null);
                    }
                }
#endif
                setCaptchaVisible?.Invoke(false);
                try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }
                return (null, "Could not fetch property value");
            }
            catch (Exception ex)
            {
                setCaptchaVisible?.Invoke(false);
                return (null, ex.Message);
            }
        }

        public static async Task<(List<PropertySearchResult> Results, string Error)> SearchByPostcode(string postcode, WebView webView)
        {
            var pc = postcode.Replace(" ", "").ToUpper().Trim();
            if (pc.Length < 5) return (null, "Invalid postcode");

            try
            {
                var url = $"https://find-energy-certificate.service.gov.uk/find-a-certificate/search-by-postcode?postcode={Uri.EscapeDataString(pc)}";

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
    if (t.indexOf('Find') === -1 && t.indexOf('energy') === -1 && t.indexOf('certificate') === -1) return 'WAITING';

    var links = document.querySelectorAll('a[href*=""/energy-certificate/""]');
    if (links.length === 0) {
        var err = document.body.innerText.substring(0, 200);
        return 'NOLINKS:' + err;
    }

    var results = [];
    for (var i = 0; i < links.length; i++) {
        var a = links[i];
        var addr = a.innerText.trim();
        var href = a.getAttribute('href') || '';
        var row = a.closest('tr') || a.closest('li') || a.parentElement;
        var rating = '';
        if (row) {
            var cells = row.querySelectorAll('td, span, dd');
            for (var j = 0; j < cells.length; j++) {
                var ct = cells[j].innerText.trim();
                if (ct.match(/^[A-G]$/)) { rating = ct; break; }
            }
        }
        if (addr.length > 5) {
            results.push(addr + '|||' + rating + '|||' + href);
        }
    }
    if (results.length === 0) {
        var body = document.body.innerText.substring(0, 500);
        return 'EMPTY:' + body;
    }
    return results.join('###');
})()";

                string lastResult = "";
                string lastError = "";

#if WINDOWS
                var nativeWebView = webView.Handler?.PlatformView as Microsoft.UI.Xaml.Controls.WebView2;
                if (nativeWebView?.CoreWebView2 == null)
                {
                    try { await nativeWebView?.EnsureCoreWebView2Async(); }
                    catch { }
                }

                if (nativeWebView?.CoreWebView2 != null)
                {
                    for (int i = 0; i < 20; i++)
                    {
                        await Task.Delay(1000);

                        string result;
                        try { result = await nativeWebView.CoreWebView2.ExecuteScriptAsync(extractJs); }
                        catch (Exception jsEx) { lastError = jsEx.Message; continue; }

                        if (string.IsNullOrEmpty(result) || result == "null") continue;

                        var val = result.Trim('"');
                        lastResult = val;

                        if (val == "WAITING") continue;

                        if (val.StartsWith("NOLINKS:") || val.StartsWith("EMPTY:"))
                        {
                            try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }
                            return (null, $"No properties found for {pc}");
                        }

                        if (!val.Contains("###") && !val.Contains("|||")) continue;

                        var entries = val.Split("###");
                        var results = new List<PropertySearchResult>();

                        foreach (var entry in entries)
                        {
                            var parts = entry.Split("|||");
                            if (parts.Length >= 1 && !string.IsNullOrEmpty(parts[0]))
                            {
                                results.Add(new PropertySearchResult
                                {
                                    Address = parts[0].Trim(),
                                    EpcRating = parts.Length > 1 ? parts[1].Trim() : "",
                                    CertificateUrl = parts.Length > 2 ? parts[2].Trim() : "",
                                });
                            }
                        }

                        try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }

                        if (results.Count > 0)
                            return (results, null);

                        return (null, "No properties found");
                    }
                }
#endif

                try { webView.Source = new UrlWebViewSource { Url = "about:blank" }; } catch { }
                return (null, $"Lookup timed out. Last=[{lastResult}] Err=[{lastError}]");
            }
            catch (Exception ex)
            {
                return (null, $"Lookup failed: {ex.Message}");
            }
        }
    }
}
