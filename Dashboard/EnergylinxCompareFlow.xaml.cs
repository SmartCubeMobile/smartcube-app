using SmartCubeMobile.MockData;
using System.Globalization;
#if WINDOWS
using Microsoft.Web.WebView2.Core;
#endif

namespace SmartCubeMobile.Dashboard
{
    public partial class EnergylinxCompareFlow : ContentPage
    {
        public event Action TariffsUpdated;

        private readonly CultureInfo culture = new("en-GB");
        private readonly List<string> _capturedResponses = new();
        private readonly List<ScrapedTariff> _tariffs = new();
        private bool _resultsFound;
        private bool _autoFillAttempted;
        private int _navCount;

        private string _postcode;
        private string _houseNumber;
        private string _email;
        private string _gasSupplier;
        private string _elecSupplier;
        private decimal _elecMonthlyKwh;
        private decimal _gasMonthlyKwh;
        private bool _hasSameSupplier;
        private bool _hasSmartMeter;

        private const string EnergylinxUrl = "https://switch.energylinx.com/#widget-data=eyJmdWVsVHlwZSI6ImR1YWwifQ==";

        public EnergylinxCompareFlow()
        {
            InitializeComponent();
            LoadUserData();
            ConfigureWebView();
            CompareWebView.Source = new UrlWebViewSource { Url = EnergylinxUrl };
        }

        private void LoadUserData()
        {
            var profile = MockDataService.GetUserProfile();
            _postcode = profile.Postcode ?? "";
            _houseNumber = profile.HouseNumber ?? "";
            _email = profile.Email ?? "";

            var suppliers = MockDataService.GetSuppliers();
            var elecSupplier = suppliers.FirstOrDefault(s => s.Type == "Electricity");
            var gasSupplier = suppliers.FirstOrDefault(s => s.Type == "Gas");
            _elecSupplier = elecSupplier?.Name ?? "";
            _gasSupplier = gasSupplier?.Name ?? "";
            _hasSameSupplier = !string.IsNullOrEmpty(_elecSupplier) && _elecSupplier == _gasSupplier;

            var bills = MockDataService.GetUtilityBills();
            var recentElec = bills.Where(b => b.FuelType == "Electricity").OrderByDescending(b => b.BillDate).Take(3).ToList();
            var recentGas = bills.Where(b => b.FuelType == "Gas").OrderByDescending(b => b.BillDate).Take(3).ToList();
            _elecMonthlyKwh = recentElec.Count > 0 ? Math.Round(recentElec.Average(b => b.UnitsUsed), 1) : 0;
            _gasMonthlyKwh = recentGas.Count > 0 ? Math.Round(recentGas.Average(b => b.UnitsUsed), 1) : 0;

            var readings = MockDataService.GetMeterReadings();
            _hasSmartMeter = readings.Any(r => r.Source == "Smart Meter");

            if (!string.IsNullOrEmpty(_postcode))
                PageSubtitle.Text = $"Auto-filling for {_postcode}...";
        }

        private void ConfigureWebView()
        {
#if WINDOWS
            CompareWebView.HandlerChanged += (s, e) =>
            {
                if (CompareWebView.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.WebView2 wv2)
                {
                    wv2.CoreWebView2Initialized += (sender, args) =>
                    {
                        if (wv2.CoreWebView2 == null) return;

                        wv2.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true;
                        wv2.CoreWebView2.Settings.IsWebMessageEnabled = true;
                        wv2.CoreWebView2.Settings.AreDevToolsEnabled = false;
                        wv2.CoreWebView2.Settings.UserAgent =
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

                        wv2.CoreWebView2.ServerCertificateErrorDetected += (s2, certArgs) =>
                        {
                            certArgs.Action = CoreWebView2ServerCertificateErrorAction.AlwaysAllow;
                        };

                        wv2.CoreWebView2.WebResourceResponseReceived += async (s2, resArgs) =>
                        {
                            try
                            {
                                var uri = resArgs.Request.Uri?.ToLower() ?? "";
                                var ct = resArgs.Response.Headers?.GetHeader("Content-Type") ?? "";
                                if (ct.Contains("json") && (
                                    uri.Contains("_next/data") ||
                                    uri.Contains("tariff") || uri.Contains("quote") ||
                                    uri.Contains("result") || uri.Contains("compare") ||
                                    uri.Contains("switch") || uri.Contains("energy") ||
                                    uri.Contains("supplier") || uri.Contains("api")))
                                {
                                    var stream = await resArgs.Response.GetContentAsync();
                                    if (stream != null)
                                    {
                                        using var netStream = stream.AsStreamForRead();
                                        using var reader = new System.IO.StreamReader(netStream);
                                        var body = await reader.ReadToEndAsync();
                                        if (!string.IsNullOrEmpty(body) && body.Length < 1000000)
                                        {
                                            _capturedResponses.Add(body);
                                            TryParseFromApi(body);
                                        }
                                    }
                                }
                            }
                            catch { }
                        };
                    };
                }
            };
#endif
        }

        private void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
        {
            LoadingSpinner.IsRunning = true;
            UrlLabel.Text = e.Url;
        }

        private async void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
        {
            LoadingSpinner.IsRunning = false;
            UrlLabel.Text = e.Url;

            _navCount++;
            ScrapeBtn.IsVisible = true;

            if (!_autoFillAttempted && !string.IsNullOrEmpty(_postcode))
            {
                PageSubtitle.Text = "Waiting for page to load...";
                await Task.Delay(3500);
                await AutoFillForm();
            }

            var url = e.Url?.ToLower() ?? "";
            if (url.Contains("result") || url.Contains("quote") || url.Contains("compare"))
            {
                await Task.Delay(3000);
                await TryScrapeResults();
            }
        }

        private async Task AutoFillForm()
        {
            _autoFillAttempted = true;
            PageSubtitle.Text = "Filling in your details...";

            // Stage 1: Dismiss dialogs and fill postcode
            var stage1 = BuildStage1();
            await CompareWebView.EvaluateJavaScriptAsync(stage1);

            // Wait for address lookup
            await Task.Delay(4000);

            // Stage 2: Select fuel type, supplier, and fill remaining fields
            var stage2 = BuildStage2();
            await CompareWebView.EvaluateJavaScriptAsync(stage2);

            await Task.Delay(3000);

            // Stage 3: Fill usage, email, and final options
            var stage3 = BuildStage3();
            await CompareWebView.EvaluateJavaScriptAsync(stage3);

            PageSubtitle.Text = "Details pre-filled — select address and compare";
        }

        private static string ReactSetValue()
        {
            return
                "function rSet(el,v){" +
                "var s=Object.getOwnPropertyDescriptor(Object.getPrototypeOf(el),'value');" +
                "if(s&&s.set){s.set.call(el,v);}else{el.value=v;}" +
                "el.dispatchEvent(new Event('input',{bubbles:true}));" +
                "el.dispatchEvent(new Event('change',{bubbles:true}));" +
                "el.dispatchEvent(new Event('blur',{bubbles:true}));" +
                "}";
        }

        private static string ReactClickFn()
        {
            return
                "function rClick(el){" +
                "if(!el)return false;" +
                "el.scrollIntoView({block:'center'});" +
                "el.dispatchEvent(new PointerEvent('pointerdown',{bubbles:true}));" +
                "el.dispatchEvent(new MouseEvent('mousedown',{bubbles:true}));" +
                "el.dispatchEvent(new PointerEvent('pointerup',{bubbles:true}));" +
                "el.dispatchEvent(new MouseEvent('mouseup',{bubbles:true}));" +
                "el.dispatchEvent(new MouseEvent('click',{bubbles:true,cancelable:true}));" +
                "return true;}" +
                "function findBtn(text){" +
                "var btns=Array.from(document.querySelectorAll('button'));" +
                "return btns.find(function(b){return b.textContent.trim()===text;})||null;" +
                "}" +
                "function clickBtn(text){return rClick(findBtn(text));}" +
                "function findBtnContains(text){" +
                "var btns=Array.from(document.querySelectorAll('button'));" +
                "return btns.find(function(b){return b.textContent.trim().indexOf(text)>-1;})||null;" +
                "}" +
                "function clickBtnContains(text){return rClick(findBtnContains(text));}";
        }

        private string BuildStage1()
        {
            return
                "try{" +
                ReactSetValue() +
                ReactClickFn() +
                // Remove ALL consent overlays aggressively
                "document.querySelectorAll('dialog').forEach(function(d){d.remove();});" +
                "document.querySelectorAll('[class*=consent],[class*=Consent],[id*=consent],[id*=sp_message]').forEach(function(d){d.remove();});" +
                "document.querySelectorAll('div').forEach(function(d){" +
                "var z=parseInt(window.getComputedStyle(d).zIndex||0);" +
                "if(z>999){d.remove();}});" +
                "document.querySelectorAll('iframe').forEach(function(f){" +
                "var s=f.src||'';" +
                "if(s.indexOf('privacy')>-1||s.indexOf('consent')>-1||s.indexOf('sp_')>-1){" +
                "if(f.parentElement)f.parentElement.remove();else f.remove();}});" +
                // Also click Accept/Agree if any consent buttons remain
                "var cBtns=document.querySelectorAll('button');" +
                "for(var ci=0;ci<cBtns.length;ci++){" +
                "var ct=cBtns[ci].textContent.toLowerCase();" +
                "if(ct.indexOf('accept')>-1||ct.indexOf('agree')>-1||ct.indexOf('consent')>-1){" +
                "rClick(cBtns[ci]);break;}}" +
                // Dismiss "Start a new journey" if present
                "clickBtn('Start a new journey');" +
                "clickBtnContains('Start');" +
                // Wait a tick for dialog to close, then fill postcode
                "setTimeout(function(){" +
                // Find the postcode input - try placeholder match first, then fall back to first text input
                "var pcInput=document.querySelector('input[placeholder*=postcode]')" +
                "||document.querySelector('input[placeholder*=Postcode]')" +
                "||document.querySelector('input[name*=postcode]')" +
                "||document.querySelector('input[type=text]');" +
                "if(pcInput){rSet(pcInput,'" + EscapeJs(_postcode) + "');}" +
                // House number input
                "var inputs=document.querySelectorAll('input[type=text]');" +
                "if(inputs.length>=2){rSet(inputs[1],'" + EscapeJs(_houseNumber) + "');}" +
                // Submit the form
                "clickBtn('Find Address');" +
                "clickBtnContains('Find');" +
                "},1500);" +
                "'stage1-done';" +
                "}catch(ex){'stage1-error:'+ex.message;}";
        }

        private string BuildStage2()
        {
            return
                "try{" +
                ReactClickFn() +
                // Click "Both gas and electricity"
                "clickBtn('Both gas and electricity');" +
                "setTimeout(function(){" +
                // Same supplier
                "clickBtn('" + (_hasSameSupplier ? "Yes" : "No") + "');" +
                // Select supplier by button text (contains supplier name)
                "clickBtnContains('" + EscapeJs(_elecSupplier) + "');" +
                "setTimeout(function(){" +
                // Payment method
                "clickBtn('Monthly Direct Debit');" +
                // Economy 7 - first No button after Economy 7 heading
                "var h=document.querySelectorAll('h1,h2,h3,h4,h5,h6');" +
                "for(var i=0;i<h.length;i++){" +
                "if(h[i].textContent.indexOf('Economy 7')>-1){" +
                "var sib=h[i].parentElement;" +
                "if(sib){var nbtns=sib.querySelectorAll('button');" +
                "for(var j=0;j<nbtns.length;j++){" +
                "if(nbtns[j].textContent.trim()==='No'){rClick(nbtns[j]);break;}" +
                "}}break;}}" +
                // Smart meters
                "var smartText='" + (_hasSmartMeter ? "Yes" : "No") + "';" +
                "for(var i2=0;i2<h.length;i2++){" +
                "if(h[i2].textContent.indexOf('smart meter')>-1){" +
                "var sib2=h[i2].parentElement;" +
                "if(sib2){var sbtns=sib2.querySelectorAll('button');" +
                "for(var j2=0;j2<sbtns.length;j2++){" +
                "if(sbtns[j2].textContent.trim()===smartText){rClick(sbtns[j2]);break;}" +
                "}}}" +
                "}" +
                // Billing - Online
                "clickBtn('Online');" +
                // Tariff
                "clickBtnContains('know my tariff');" +
                "},1500);" +
                "},1500);" +
                "'stage2-done';" +
                "}catch(ex){'stage2-error:'+ex.message;}";
        }

        private string BuildStage3()
        {
            return
                "try{" +
                ReactSetValue() +
                ReactClickFn() +
                // Gas usage - I don't know (let Energylinx use industry data)
                "var gasUsageBtn=null;" +
                "var h3=document.querySelectorAll('h1,h2,h3,h4,h5,h6');" +
                "for(var i=0;i<h3.length;i++){" +
                "if(h3[i].textContent.indexOf('Gas')>-1&&h3[i].textContent.indexOf('use')>-1){" +
                "var p=h3[i].parentElement;" +
                "if(p){var gb=p.querySelectorAll('button');" +
                "for(var j=0;j<gb.length;j++){" +
                "if(gb[j].textContent.trim().indexOf('know')>-1){rClick(gb[j]);break;}" +
                "}}break;}}" +
                // Electricity usage - I know kWh
                "for(var i2=0;i2<h3.length;i2++){" +
                "if(h3[i2].textContent.indexOf('Electricity')>-1&&h3[i2].textContent.indexOf('use')>-1){" +
                "var p2=h3[i2].parentElement;" +
                "if(p2){var eb=p2.querySelectorAll('button');" +
                "for(var j2=0;j2<eb.length;j2++){" +
                "if(eb[j2].textContent.indexOf('kWh')>-1){rClick(eb[j2]);break;}" +
                "}}break;}}" +
                // Set electricity usage value
                "setTimeout(function(){" +
                "var numInputs=document.querySelectorAll('input[type=number]');" +
                "for(var n=0;n<numInputs.length;n++){" +
                "var ni=numInputs[n];" +
                "if(ni.offsetParent!==null){" +
                "rSet(ni,'" + _elecMonthlyKwh.ToString(CultureInfo.InvariantCulture) + "');" +
                "}}" +
                // Email
                "var emailInput=document.querySelector('input[type=email]');" +
                "if(emailInput){rSet(emailInput,'" + EscapeJs(_email) + "');}" +
                // Updates - No (last No button on the page)
                "var allBtns=document.querySelectorAll('button');" +
                "for(var b=allBtns.length-1;b>=0;b--){" +
                "if(allBtns[b].textContent.trim()==='No'&&allBtns[b].offsetParent!==null){" +
                "rClick(allBtns[b]);break;}}" +
                "},1000);" +
                "'stage3-done';" +
                "}catch(ex){'stage3-error:'+ex.message;}";
        }

        private static string EscapeJs(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"");
        }

        private async void OnScrapeClicked(object sender, EventArgs e)
        {
            ScrapeBtn.IsEnabled = false;
            ScrapeBtn.Text = "Scraping...";
            PageSubtitle.Text = "Extracting tariff data...";

            await TryScrapeResults();

            if (_tariffs.Count > 0)
            {
                ShowResults();
            }
            else
            {
                TryParseAllCaptured();
                if (_tariffs.Count > 0)
                {
                    ShowResults();
                }
                else
                {
                    await DumpAndAlert();
                }
            }

            ScrapeBtn.IsEnabled = true;
            ScrapeBtn.Text = "Scrape Results";
        }

        private async Task TryScrapeResults()
        {
            var js = BuildResultsScraper();
            var result = await CompareWebView.EvaluateJavaScriptAsync(js);

            if (string.IsNullOrEmpty(result)) return;

            result = result.Trim('"').Replace("\\\"", "\"").Replace("\\\\", "\\");

            try
            {
                var data = Newtonsoft.Json.Linq.JObject.Parse(result);
                var tariffs = data["tariffs"] as Newtonsoft.Json.Linq.JArray;
                if (tariffs != null && tariffs.Count > 0)
                {
                    _tariffs.Clear();
                    foreach (var t in tariffs)
                    {
                        var name = t["supplier"]?.ToString() ?? "";
                        var tariff = t["tariff"]?.ToString() ?? "";
                        var cost = t["annualCost"]?.ToString() ?? "";
                        var saving = t["saving"]?.ToString() ?? "";
                        var features = t["features"]?.ToString() ?? "";

                        if (string.IsNullOrEmpty(name)) continue;

                        decimal.TryParse(cost.Replace("£", "").Replace(",", "").Trim(),
                            NumberStyles.Any, CultureInfo.InvariantCulture, out var annualCost);
                        decimal.TryParse(saving.Replace("£", "").Replace(",", "").Replace("+", "").Trim(),
                            NumberStyles.Any, CultureInfo.InvariantCulture, out var annualSaving);

                        _tariffs.Add(new ScrapedTariff
                        {
                            SupplierName = name,
                            TariffName = tariff,
                            AnnualCost = annualCost,
                            AnnualSaving = annualSaving,
                            Features = features,
                        });
                    }
                    if (_tariffs.Count > 0)
                    {
                        _resultsFound = true;
                        ShowResults();
                    }
                }
            }
            catch { }
        }

        private static string BuildResultsScraper()
        {
            return
                "try{" +
                "var r={tariffs:[],pageTitle:document.title||'',url:location.href,textLen:(document.body.innerText||'').length};" +
                "var cards=document.querySelectorAll('article,section,div,li,tr');" +
                "for(var i=0;i<cards.length&&r.tariffs.length<30;i++){" +
                "var c=cards[i];if(c.children.length>20||c.children.length<1)continue;" +
                "var txt=c.innerText||'';if(txt.length>1000||txt.length<20)continue;" +
                "var hasPrice=txt.match(/£\\s*(\\d[\\d,]*\\.?\\d{0,2})/);" +
                "if(!hasPrice)continue;" +
                "var lines=txt.split('\\n').map(function(l){return l.trim();}).filter(function(l){return l.length>0;});" +
                "if(lines.length<2)continue;" +
                "var supplier='';var tariff='';var annual='';var saving='';" +
                "var knownSuppliers=['British Gas','EDF','Octopus','OVO','E.ON','Scottish','Shell','Bulb','Utility Warehouse','So Energy','Outfox','Rebel','Ecotricity','Good Energy','Green Energy','Affect','Opus'];" +
                "for(var j=0;j<lines.length;j++){" +
                "var ln=lines[j];" +
                "for(var k=0;k<knownSuppliers.length;k++){" +
                "if(ln.indexOf(knownSuppliers[k])>-1&&!supplier){supplier=ln;break;}" +
                "}" +
                "if(ln.match(/tariff|plan|fix|variable|tracker|standard|flexi|green/i)&&!tariff)tariff=ln;" +
                "if(ln.match(/per\\s*year|annual|p\\.a\\.|yearly|estimated/i)){" +
                "var pm=ln.match(/£\\s*(\\d[\\d,]*\\.?\\d{0,2})/);" +
                "if(pm)annual=pm[1].replace(/,/g,'');" +
                "}" +
                "if(ln.match(/sav|cheaper|less|more/i)){" +
                "var sm=ln.match(/£\\s*(\\d[\\d,]*\\.?\\d{0,2})/);" +
                "if(sm)saving=sm[1].replace(/,/g,'');" +
                "}" +
                "}" +
                "if(!annual&&hasPrice)annual=hasPrice[1].replace(/,/g,'');" +
                "if(supplier||tariff){" +
                "var dup=r.tariffs.some(function(t){return t.supplier===supplier&&t.annualCost===annual;});" +
                "if(!dup)r.tariffs.push({supplier:supplier||'Unknown',tariff:tariff||'',annualCost:annual,saving:saving,features:''});" +
                "}}" +
                "if(r.tariffs.length===0){" +
                "var allText=document.body.innerText||'';" +
                "var prices=allText.match(/£\\d[\\d,]*\\.\\d{2}\\s*(?:per year|a year|annual|p\\.a)/gi)||[];" +
                "for(var p=0;p<prices.length&&p<10;p++){" +
                "var pm2=prices[p].match(/£(\\d[\\d,]*\\.\\d{2})/);" +
                "if(pm2)r.tariffs.push({supplier:'Tariff '+(p+1),tariff:'',annualCost:pm2[1].replace(/,/g,''),saving:'',features:prices[p]});" +
                "}}" +
                "JSON.stringify(r);" +
                "}catch(ex){JSON.stringify({error:ex.message,tariffs:[]});}";
        }

        private void TryParseFromApi(string json)
        {
            try
            {
                var token = Newtonsoft.Json.Linq.JToken.Parse(json);
                SearchForTariffs(token);
            }
            catch { }
        }

        private void TryParseAllCaptured()
        {
            foreach (var json in _capturedResponses)
                TryParseFromApi(json);
        }

        private void SearchForTariffs(Newtonsoft.Json.Linq.JToken token)
        {
            if (_tariffs.Count >= 30) return;

            if (token is Newtonsoft.Json.Linq.JArray arr)
            {
                var found = new List<ScrapedTariff>();
                foreach (var item in arr)
                {
                    if (item is Newtonsoft.Json.Linq.JObject obj)
                    {
                        var t = TryExtractTariff(obj);
                        if (t != null) found.Add(t);
                    }
                }
                if (found.Count >= 2)
                {
                    _tariffs.Clear();
                    _tariffs.AddRange(found);
                    _resultsFound = true;
                    return;
                }
                foreach (var item in arr)
                    SearchForTariffs(item);
            }
            else if (token is Newtonsoft.Json.Linq.JObject jObj)
            {
                foreach (var prop in jObj.Properties())
                {
                    var name = prop.Name.ToLower();
                    if (prop.Value is Newtonsoft.Json.Linq.JArray propArr &&
                        (name.Contains("tariff") || name.Contains("result") || name.Contains("quote") ||
                         name.Contains("plan") || name.Contains("deal") || name.Contains("offer") ||
                         name.Contains("supplier") || name.Contains("product")))
                    {
                        var found = new List<ScrapedTariff>();
                        foreach (var item in propArr)
                        {
                            if (item is Newtonsoft.Json.Linq.JObject itemObj)
                            {
                                var t = TryExtractTariff(itemObj);
                                if (t != null) found.Add(t);
                            }
                        }
                        if (found.Count >= 1)
                        {
                            _tariffs.Clear();
                            _tariffs.AddRange(found);
                            _resultsFound = true;
                            return;
                        }
                    }

                    if (prop.Value is Newtonsoft.Json.Linq.JObject || prop.Value is Newtonsoft.Json.Linq.JArray)
                        SearchForTariffs(prop.Value);

                    if (_tariffs.Count >= 30) return;
                }
            }
        }

        private ScrapedTariff TryExtractTariff(Newtonsoft.Json.Linq.JObject obj)
        {
            string supplier = null, tariffName = null, features = null;
            decimal annualCost = 0, saving = 0;

            var supplierNames = new[] { "supplierName", "supplier", "providerName", "provider", "brandName", "brand", "name", "company" };
            var tariffNames = new[] { "tariffName", "tariff", "planName", "plan", "productName", "product", "dealName", "deal", "displayName" };
            var costNames = new[] { "annualCost", "estimatedAnnualCost", "totalAnnualCost", "yearlyTotal", "totalCost",
                "estimatedCost", "annual", "projectedAnnualCost", "cost", "annualSpend", "projectedCost" };
            var savingNames = new[] { "saving", "savings", "annualSaving", "annualSavings", "estimatedSaving",
                "potentialSaving", "projectedSaving" };
            var featureNames = new[] { "features", "benefits", "details", "description", "tariffType", "planType", "type" };

            foreach (var n in supplierNames)
            {
                var val = FindVal(obj, n);
                if (val != null) { supplier = val; break; }
            }
            foreach (var n in tariffNames)
            {
                var val = FindVal(obj, n);
                if (val != null) { tariffName = val; break; }
            }
            foreach (var n in costNames)
            {
                var val = FindVal(obj, n);
                if (val != null)
                {
                    var s = val.Replace("£", "").Replace(",", "").Trim();
                    if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var c))
                    {
                        annualCost = c > 5000 ? Math.Round(c / 100m, 2) : c;
                        break;
                    }
                }
            }
            foreach (var n in savingNames)
            {
                var val = FindVal(obj, n);
                if (val != null)
                {
                    var s = val.Replace("£", "").Replace(",", "").Replace("+", "").Replace("-", "").Trim();
                    if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var sv))
                    {
                        saving = sv > 5000 ? Math.Round(sv / 100m, 2) : sv;
                        break;
                    }
                }
            }
            foreach (var n in featureNames)
            {
                var val = FindVal(obj, n);
                if (val != null) { features = val; break; }
            }

            if ((supplier != null || tariffName != null) && annualCost > 0)
            {
                return new ScrapedTariff
                {
                    SupplierName = supplier ?? "Unknown",
                    TariffName = tariffName ?? "",
                    AnnualCost = annualCost,
                    AnnualSaving = saving,
                    Features = features ?? "",
                };
            }
            return null;
        }

        private static string FindVal(Newtonsoft.Json.Linq.JObject obj, string name)
        {
            var val = obj[name];
            if (val != null && val.Type != Newtonsoft.Json.Linq.JTokenType.Null &&
                val.Type != Newtonsoft.Json.Linq.JTokenType.Object &&
                val.Type != Newtonsoft.Json.Linq.JTokenType.Array)
            {
                var s = val.ToString().Trim();
                return string.IsNullOrEmpty(s) ? null : s;
            }

            foreach (var prop in obj.Properties())
            {
                if (prop.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    prop.Value.Type != Newtonsoft.Json.Linq.JTokenType.Null &&
                    prop.Value.Type != Newtonsoft.Json.Linq.JTokenType.Object &&
                    prop.Value.Type != Newtonsoft.Json.Linq.JTokenType.Array)
                {
                    var s = prop.Value.ToString().Trim();
                    return string.IsNullOrEmpty(s) ? null : s;
                }
            }
            return null;
        }

        private void ShowResults()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TariffResults.Children.Clear();
                ResultsTitle.Text = $"Available Tariffs ({_tariffs.Count})";

                var ordered = _tariffs.OrderBy(t => t.AnnualCost).ToList();
                var cheapest = ordered.FirstOrDefault()?.AnnualCost ?? 0;

                foreach (var t in ordered)
                {
                    bool isCheapest = t.AnnualCost == cheapest && cheapest > 0;
                    var monthlyCost = t.AnnualCost > 0 ? Math.Round(t.AnnualCost / 12m, 2) : 0;

                    var card = new Border
                    {
                        BackgroundColor = Color.FromArgb("#131B2E"),
                        Stroke = Color.FromArgb(isCheapest ? "#22C55E40" : "#1E2D4A"),
                        StrokeThickness = isCheapest ? 2 : 1,
                        StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
                        Padding = new Thickness(18, 14),
                    };

                    var grid = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition(GridLength.Star),
                            new ColumnDefinition(GridLength.Auto),
                        },
                        ColumnSpacing = 16,
                    };

                    var info = new VerticalStackLayout { Spacing = 3, VerticalOptions = LayoutOptions.Center };

                    var nameRow = new HorizontalStackLayout { Spacing = 8 };
                    nameRow.Children.Add(new Label
                    {
                        Text = t.SupplierName,
                        TextColor = Color.FromArgb("#F1F5F9"),
                        FontSize = 15,
                        FontAttributes = FontAttributes.Bold,
                    });
                    if (isCheapest)
                    {
                        nameRow.Children.Add(new Border
                        {
                            BackgroundColor = Color.FromArgb("#22C55E20"),
                            Stroke = Colors.Transparent,
                            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 },
                            Padding = new Thickness(6, 2),
                            Content = new Label
                            {
                                Text = "Cheapest",
                                TextColor = Color.FromArgb("#22C55E"),
                                FontSize = 9,
                                FontAttributes = FontAttributes.Bold,
                            }
                        });
                    }
                    info.Children.Add(nameRow);

                    if (!string.IsNullOrEmpty(t.TariffName))
                    {
                        info.Children.Add(new Label
                        {
                            Text = t.TariffName,
                            TextColor = Color.FromArgb("#3B82F6"),
                            FontSize = 12,
                        });
                    }

                    if (!string.IsNullOrEmpty(t.Features))
                    {
                        info.Children.Add(new Label
                        {
                            Text = t.Features,
                            TextColor = Color.FromArgb("#64748B"),
                            FontSize = 11,
                            LineBreakMode = LineBreakMode.TailTruncation,
                            MaxLines = 2,
                        });
                    }

                    if (t.AnnualSaving > 0)
                    {
                        info.Children.Add(new Label
                        {
                            Text = $"Save {t.AnnualSaving.ToString("C0", culture)}/yr",
                            TextColor = Color.FromArgb("#22C55E"),
                            FontSize = 11,
                            FontAttributes = FontAttributes.Bold,
                        });
                    }

                    Grid.SetColumn(info, 0);

                    var costCol = new VerticalStackLayout
                    {
                        Spacing = 2,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.End,
                    };

                    if (monthlyCost > 0)
                    {
                        costCol.Children.Add(new Label
                        {
                            Text = $"{monthlyCost.ToString("C", culture)}/mo",
                            TextColor = Color.FromArgb(isCheapest ? "#22C55E" : "#F59E0B"),
                            FontSize = 16,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalTextAlignment = TextAlignment.End,
                        });
                    }

                    if (t.AnnualCost > 0)
                    {
                        costCol.Children.Add(new Label
                        {
                            Text = $"{t.AnnualCost.ToString("C0", culture)}/yr",
                            TextColor = Color.FromArgb("#94A3B8"),
                            FontSize = 11,
                            HorizontalTextAlignment = TextAlignment.End,
                        });
                    }

                    Grid.SetColumn(costCol, 1);

                    grid.Children.Add(info);
                    grid.Children.Add(costCol);
                    card.Content = grid;

                    card.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () =>
                        {
                            await card.ScaleTo(0.98, 60, Easing.CubicOut);
                            await card.ScaleTo(1.0, 60, Easing.CubicOut);
                        })
                    });

                    TariffResults.Children.Add(card);
                }

                ResultsPanel.IsVisible = true;
                PageSubtitle.Text = $"{_tariffs.Count} tariff(s) found";
            });
        }

        private async Task DumpAndAlert()
        {
            try
            {
                var dumpPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "energylinx_dump.txt");
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"URL: {UrlLabel.Text}");
                sb.AppendLine($"APIs captured: {_capturedResponses.Count}");
                sb.AppendLine($"Tariffs found: {_tariffs.Count}");
                sb.AppendLine();
                for (int i = 0; i < _capturedResponses.Count; i++)
                {
                    sb.AppendLine($"=== API {i + 1} ===");
                    var r = _capturedResponses[i];
                    sb.AppendLine(r.Length > 3000 ? r.Substring(0, 3000) + "..." : r);
                    sb.AppendLine();
                }
                System.IO.File.WriteAllText(dumpPath, sb.ToString());

                await DisplayAlert("No Tariffs Found",
                    $"APIs captured: {_capturedResponses.Count}\n" +
                    $"Complete the comparison on Energylinx first, then tap Scrape Results.\n\n" +
                    $"API dump saved to Desktop:\nenergylinx_dump.txt",
                    "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("No Tariffs Found",
                    $"APIs: {_capturedResponses.Count}. Complete the Energylinx comparison, then tap Scrape Results.\nError: {ex.Message}", "OK");
            }
        }

        private void OnBackToSearch(object sender, EventArgs e)
        {
            ResultsPanel.IsVisible = false;
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private class ScrapedTariff
        {
            public string SupplierName { get; set; }
            public string TariffName { get; set; }
            public decimal AnnualCost { get; set; }
            public decimal AnnualSaving { get; set; }
            public string Features { get; set; }
        }
    }
}
