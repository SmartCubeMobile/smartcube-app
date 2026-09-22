using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using SmartCubeMobile.MockData;
using System.Text.Json;

namespace SmartCubeMobile.Services
{
    public class EmailSettings
    {
        public string EmailAddress { get; set; }
        public string AppPassword { get; set; }
        public string ImapServer { get; set; }
        public int ImapPort { get; set; } = 993;
        public DateTime? LastScanDate { get; set; }
    }

    public static class EmailBillScanner
    {
        private static readonly Dictionary<string, (string Server, int Port)> KnownProviders = new(StringComparer.OrdinalIgnoreCase)
        {
            ["gmail.com"] = ("imap.gmail.com", 993),
            ["googlemail.com"] = ("imap.gmail.com", 993),
            ["outlook.com"] = ("outlook.office365.com", 993),
            ["hotmail.com"] = ("outlook.office365.com", 993),
            ["hotmail.co.uk"] = ("outlook.office365.com", 993),
            ["live.com"] = ("outlook.office365.com", 993),
            ["live.co.uk"] = ("outlook.office365.com", 993),
            ["yahoo.com"] = ("imap.mail.yahoo.com", 993),
            ["yahoo.co.uk"] = ("imap.mail.yahoo.com", 993),
            ["icloud.com"] = ("imap.mail.me.com", 993),
            ["me.com"] = ("imap.mail.me.com", 993),
            ["aol.com"] = ("imap.aol.com", 993),
            ["btinternet.com"] = ("mail.btinternet.com", 993),
            ["sky.com"] = ("imap.tools.sky.com", 993),
            ["virginmedia.com"] = ("imap.virginmedia.com", 993),
            ["talktalk.net"] = ("imap.talktalk.net", 993),
            ["plusnet.com"] = ("imap.plusnet.com", 993),
        };

        private static readonly string[] SupplierSenders = new[]
        {
            "edf", "edfenergy", "britishgas", "octopus.energy", "ovoenergy",
            "eonenergy", "e.on", "scottishpower", "shellenergy", "sse.co",
            "bulb.co", "soenergy", "goodenergy", "utilitywarehouse",
            "unitedutilities", "severntrent", "thameswater", "anglianwater",
            "yorkshirewater", "southwestwater", "southernwater", "wessexwater",
            "nwater.com", "dwrcymru", "southeastwater", "bristolwater",
            "affinitywater",
        };

        private static string SettingsPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SmartCube", "email_settings.json");

        public static EmailSettings LoadSettings()
        {
            if (!File.Exists(SettingsPath)) return null;
            try { return JsonSerializer.Deserialize<EmailSettings>(File.ReadAllText(SettingsPath)); }
            catch { return null; }
        }

        public static void SaveSettings(EmailSettings settings)
        {
            var dir = Path.GetDirectoryName(SettingsPath);
            Directory.CreateDirectory(dir);
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings));
        }

        public static void ClearSettings()
        {
            if (File.Exists(SettingsPath))
                File.Delete(SettingsPath);
        }

        public static (string Server, int Port) DetectImapServer(string email)
        {
            var domain = email.Split('@').LastOrDefault()?.ToLower();
            if (domain != null && KnownProviders.TryGetValue(domain, out var provider))
                return provider;
            return ("", 993);
        }

        public static async Task<(bool Success, string Error)> TestConnection(EmailSettings settings)
        {
            try
            {
                using var client = new ImapClient();
                await client.ConnectAsync(settings.ImapServer, settings.ImapPort, MailKit.Security.SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(settings.EmailAddress, settings.AppPassword);
                await client.DisconnectAsync(true);
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public static async Task<(int Found, int Added, string Error)> ScanForBills(EmailSettings settings, IProgress<string> progress = null)
        {
            int found = 0, added = 0;
            try
            {
                using var client = new ImapClient();
                progress?.Report("Connecting...");
                await client.ConnectAsync(settings.ImapServer, settings.ImapPort, MailKit.Security.SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(settings.EmailAddress, settings.AppPassword);

                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);

                var since = settings.LastScanDate ?? DateTime.Now.AddYears(-1);
                progress?.Report($"Searching since {since:dd MMM yyyy}...");

                var subjectFilter = SearchQuery.Or(
                    SearchQuery.Or(
                        SearchQuery.SubjectContains("bill"),
                        SearchQuery.SubjectContains("statement")),
                    SearchQuery.Or(
                        SearchQuery.SubjectContains("invoice"),
                        SearchQuery.Or(
                            SearchQuery.SubjectContains("your account"),
                            SearchQuery.SubjectContains("your charges"))));
                var query = SearchQuery.DeliveredAfter(since).And(subjectFilter);

                var uids = await inbox.SearchAsync(query);
                progress?.Report($"Found {uids.Count} potential emails...");

                var tempDir = Path.Combine(Path.GetTempPath(), "SmartCubeBills");
                Directory.CreateDirectory(tempDir);

                foreach (var uid in uids)
                {
                    var message = await inbox.GetMessageAsync(uid);
                    var from = message.From.Mailboxes.FirstOrDefault()?.Address?.ToLower() ?? "";

                    if (!SupplierSenders.Any(s => from.Contains(s)))
                        continue;

                    foreach (var attachment in message.Attachments)
                    {
                        if (attachment is not MimePart part) continue;
                        if (!part.FileName?.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) == true) continue;

                        found++;
                        var filePath = Path.Combine(tempDir, $"{uid}_{part.FileName}");

                        if (!File.Exists(filePath))
                        {
                            using var stream = File.Create(filePath);
                            await part.Content.DecodeToAsync(stream);
                        }

                        progress?.Report($"Parsing {part.FileName}...");

                        try
                        {
                            var parsed = BillPdfParser.ParseFromFile(filePath);
                            if (parsed != null && parsed.Amount > 0)
                            {
                                var bills = BillPdfParser.SplitDualFuel(parsed);
                                foreach (var bill in bills)
                                {
                                    MockDataService.AddBill(bill);
                                    added++;
                                }
                            }
                        }
                        catch { }
                    }
                }

                settings.LastScanDate = DateTime.Now;
                SaveSettings(settings);

                await client.DisconnectAsync(true);

                try { Directory.Delete(tempDir, true); } catch { }

                return (found, added, null);
            }
            catch (Exception ex)
            {
                return (found, added, ex.Message);
            }
        }
    }
}