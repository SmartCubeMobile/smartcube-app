using System.Globalization;
using Newtonsoft.Json;
using SmartCubeMobile.MockData;

namespace SmartCubeMobile.Services
{
    // Fills whatever fields a web form has that we can recognise (registration, postcode, name,
    // email, date of birth, cover start date...) from the user's own SmartCube data.
    // Generic by label/name matching, so it works on most quote sites without per-site scripts.
    public static class WebFormAutoFill
    {
        private static readonly CultureInfo Uk = new("en-GB");

        public static Dictionary<string, string> ValuesFor(InsurancePolicy policy)
        {
            var v = new Dictionary<string, string>();
            var profile = UserProfileDataService.GetProfile();

            void Put(string key, string value) { if (!string.IsNullOrWhiteSpace(value)) v[key] = value.Trim(); }
            void PutDate(string key, DateTime? d)
            {
                if (d == null) return;
                v[key] = d.Value.ToString("dd/MM/yyyy");
                v[key + "Iso"] = d.Value.ToString("yyyy-MM-dd");
                v[key + "D"] = d.Value.Day.ToString("00");
                v[key + "M"] = d.Value.Month.ToString("00");
                v[key + "Y"] = d.Value.Year.ToString();
            }
            DateTime? Parse(string s) => DateTime.TryParse(s, Uk, DateTimeStyles.None, out var d) ? d : null;

            // Person
            var name = !string.IsNullOrWhiteSpace(policy?.NamedInsured) ? policy.NamedInsured : profile.FullName;
            Put("fullName", name);
            if (!string.IsNullOrWhiteSpace(name))
            {
                var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(p => !p.EndsWith('.') && p.Length > 2 || p.Length > 3).ToArray();
                if (parts.Length >= 2) { Put("firstName", parts[0]); Put("lastName", parts[^1]); }
                else if (parts.Length == 1) Put("lastName", parts[0]);
            }
            Put("email", profile.Email);
            Put("phone", profile.Phone);
            PutDate("dob", Parse(profile.DateOfBirth));
            Put("marital", profile.MaritalStatus);
            Put("employment", profile.EmploymentStatus);
            Put("title", profile.Gender?.ToLower() switch { "male" => "Mr", "female" => "Ms", _ => null });
            PutDate("licenceDate", Parse(profile.LicenceIssueDate));

            // Address
            var prop = profile.Properties?.FirstOrDefault();
            var address = prop?.Address ?? MockDataService.GetProperty()?.Address;
            var postcode = prop?.Postcode ?? MockDataService.GetProperty()?.Postcode;
            Put("address", address);
            Put("postcode", postcode);
            if (!string.IsNullOrWhiteSpace(address))
            {
                // "5 Baron Green, ..." -> house number 5; "Flat 2, ..." -> leave for the user.
                var firstToken = address.Trim().Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (firstToken.Any(char.IsDigit)) Put("house", firstToken);
            }

            // Policy
            if (policy != null)
            {
                Put("reg", policy.VehicleReg?.Replace(" ", "").ToUpperInvariant());
                PutDate("start", policy.RenewalDate);
                Put("cover", policy.PolicyType);
                if (policy.Excess > 0) Put("excess", policy.Excess.ToString("0"));
                if (!string.IsNullOrWhiteSpace(policy.PetName)) Put("petName", policy.PetName);
                if (!string.IsNullOrWhiteSpace(policy.PetBreed)) Put("petBreed", policy.PetBreed);

                var vehicle = profile.Vehicles?.FirstOrDefault(x =>
                    string.Equals(x.Registration?.Replace(" ", ""), policy.VehicleReg?.Replace(" ", ""), StringComparison.OrdinalIgnoreCase));
                if (vehicle != null)
                {
                    Put("make", vehicle.Make);
                    Put("model", vehicle.Model);
                    if (vehicle.Year > 0) Put("year", vehicle.Year.ToString());
                    if (vehicle.Mileage > 0) Put("mileage", ((int)vehicle.Mileage).ToString());
                }
            }
            return v;
        }

        // Returns the JavaScript to run in the page. It evaluates to a JSON string {filled:[...], inputs:n}.
        public static string BuildScript(Dictionary<string, string> values)
        {
            var json = JsonConvert.SerializeObject(values);
            return Script.Replace("__VALUES__", json);
        }

        private const string Script = @"
(function(){
  var V = __VALUES__;
  var filled = [];
  function desc(el){
    var s = [el.name, el.id, el.placeholder, el.getAttribute('aria-label'), el.getAttribute('autocomplete'), el.getAttribute('data-testid'), el.getAttribute('data-qa')];
    var lbl = '';
    try { if (el.labels && el.labels.length) lbl = Array.from(el.labels).map(function(l){ return l.innerText; }).join(' '); } catch(e) {}
    var al = el.getAttribute('aria-labelledby');
    if (al) { al.split(' ').forEach(function(id){ var n = document.getElementById(id); if (n) lbl += ' ' + n.innerText; }); }
    if (!lbl) { var p = el.closest('label, fieldset, .form-group, .field, [class*=field], [class*=question]'); if (p) lbl = (p.innerText || '').slice(0, 160); }
    return (s.join(' ') + ' ' + lbl).toLowerCase();
  }
  function fire(el){ ['input','change','blur'].forEach(function(t){ el.dispatchEvent(new Event(t, {bubbles:true})); }); }
  function setVal(el, val){
    if (el.value === val) return false;
    var proto = el.tagName === 'TEXTAREA' ? HTMLTextAreaElement.prototype : HTMLInputElement.prototype;
    var d = Object.getOwnPropertyDescriptor(proto, 'value');
    if (d && d.set) d.set.call(el, val); else el.value = val;
    el.focus(); fire(el); return true;
  }
  function setSel(el, val){
    var v = String(val || '').toLowerCase(); if (!v) return false;
    var opts = Array.from(el.options);
    var o = opts.find(function(x){ return x.text.trim().toLowerCase() === v || x.value.toLowerCase() === v; })
         || opts.find(function(x){ return v.length > 1 && x.text.toLowerCase().indexOf(v) >= 0; })
         || opts.find(function(x){ return v.length > 1 && v.indexOf(x.text.trim().toLowerCase()) >= 0 && x.text.trim().length > 1; });
    if (!o || el.value === o.value) return false;
    el.value = o.value; fire(el); return true;
  }
  var rules = [
    {re:/(registration|\breg\b|reg\.|vrm|number ?plate|licence ?plate|license ?plate)/, key:'reg', not:/email|region|regard|register/},
    {re:/post ?code/, key:'postcode'},
    {re:/(first|given).{0,6}name|forename/, key:'firstName'},
    {re:/(last|sur|family).{0,6}name/, key:'lastName'},
    {re:/full ?name|your name|\bname\b/, key:'fullName', not:/user ?name|pet|company|make|model|street|road/},
    {re:/e-?mail/, key:'email'},
    {re:/(phone|mobile|telephone|contact number)/, key:'phone', not:/email/},
    {re:/(\bdob\b|date.?of.?birth|birth)/, key:'dob', date:true},
    {re:/(cover|policy|insurance).{0,25}(start|begin|from)|start.{0,12}date|when.{0,25}(start|begin)/, key:'start', date:true},
    {re:/(licence|license).{0,25}(date|held|since|obtained|passed|issue)/, key:'licenceDate', date:true},
    {re:/(house|building|flat).{0,10}(number|name|no\b)/, key:'house'},
    {re:/address.{0,8}(1\b|line ?1|first)/, key:'address'},
    {re:/marital/, key:'marital'},
    {re:/employment|occupation status|work status/, key:'employment'},
    {re:/\btitle\b/, key:'title', not:/job|policy|page/},
    {re:/annual.{0,10}mileage|mileage/, key:'mileage'},
    {re:/\bmake\b|manufacturer/, key:'make', not:/model/},
    {re:/\bmodel\b/, key:'model'},
    {re:/year.{0,15}(manufacture|registration|made)|\byear\b/, key:'year', not:/birth|dob|licence|license|start|expiry|claim/},
    {re:/pet.{0,10}name|name.{0,10}pet/, key:'petName'},
    {re:/breed/, key:'petBreed'},
  ];
  var els = Array.from(document.querySelectorAll('input:not([type=hidden]):not([type=submit]):not([type=button]):not([type=checkbox]):not([type=radio]):not([type=file]), select, textarea'))
    .filter(function(e){ return e.offsetParent !== null && !e.disabled && !e.readOnly; });
  els.forEach(function(el){
    var d = desc(el);
    for (var i = 0; i < rules.length; i++){
      var r = rules[i];
      if (!r.re.test(d) || (r.not && r.not.test(d))) continue;
      var val;
      if (r.date){
        if (/\bday\b|\bdd\b/.test(d)) val = V[r.key + 'D'];
        else if (/\bmonth\b|\bmm\b/.test(d)) val = V[r.key + 'M'];
        else if (/\byear\b|yyyy/.test(d)) val = V[r.key + 'Y'];
        else val = el.type === 'date' ? V[r.key + 'Iso'] : V[r.key];
      } else val = V[r.key];
      if (!val) break;
      var ok = el.tagName === 'SELECT' ? setSel(el, val) : setVal(el, String(val));
      if (ok && filled.indexOf(r.key) < 0) filled.push(r.key);
      break;
    }
  });
  return JSON.stringify({filled: filled, inputs: els.length});
})();";
    }
}
