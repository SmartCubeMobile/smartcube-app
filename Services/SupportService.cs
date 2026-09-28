using Newtonsoft.Json.Linq;

namespace SmartCubeMobile.Services
{
    public class SupportTicket
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public string Area { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string Channel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Replies { get; set; }
        public DateTime? LastStaffReplyAt { get; set; }
        public string ChatUrl { get; set; }

        public bool ChatOffered => !string.IsNullOrEmpty(ChatUrl);
        public bool IsClosed => Status is "Resolved" or "Closed";
    }

    public class SupportMessage
    {
        public string Author { get; set; }
        public bool IsStaff { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Support tickets for the signed-in user, via the SmartCube server (api/support/my-tickets).
    public static class SupportService
    {
        public static readonly string[] DefaultAreas =
        {
            "Account & login", "Billing & subscription", "Bank connections", "Utilities & energy bills",
            "Energy tariffs", "Insurance", "Smart Scan", "Crypto & investments", "Download & installation",
            "Website", "Other",
        };

        private static DateTime? Date(JToken t) =>
            t == null || t.Type == JTokenType.Null ? null : t.Type == JTokenType.Date ? t.Value<DateTime>().ToLocalTime()
            : DateTime.TryParse(t.ToString(), out var d) ? d.ToLocalTime() : null;

        private static string Str(JToken t) => t == null || t.Type == JTokenType.Null ? null : t.ToString();

        private static SupportTicket ParseTicket(JToken t) => new()
        {
            Id = t["id"]?.Value<int>() ?? 0,
            Reference = Str(t["reference"]),
            Area = Str(t["area"]),
            Subject = Str(t["subject"]),
            Message = Str(t["message"]),
            Status = Str(t["status"]),
            Channel = Str(t["channel"]),
            CreatedAt = Date(t["createdAt"]) ?? DateTime.Now,
            UpdatedAt = Date(t["updatedAt"]),
            Replies = t["replies"]?.Value<int>() ?? 0,
            LastStaffReplyAt = Date(t["lastStaffReplyAt"]),
            ChatUrl = Str(t["chatUrl"]),
        };

        public static async Task<(bool Ok, string Error, List<SupportTicket> Tickets, string[] Areas)> GetMyTickets()
        {
            var (ok, error, json) = await SessionService.ApiGet("/api/support/my-tickets");
            if (!ok) return (false, error, new(), DefaultAreas);
            var tickets = (json["tickets"] as JArray)?.Select(ParseTicket).ToList() ?? new();
            var areas = (json["areas"] as JArray)?.Select(a => a.ToString()).ToArray() ?? DefaultAreas;
            return (true, null, tickets, areas);
        }

        public static async Task<(bool Ok, string Error, SupportTicket Ticket, List<SupportMessage> Messages)> GetTicket(int id)
        {
            var (ok, error, json) = await SessionService.ApiGet($"/api/support/my-tickets/{id}");
            if (!ok) return (false, error, null, new());
            var t = ParseTicket(json["ticket"]);
            var msgs = new List<SupportMessage> { new() { Author = "You", IsStaff = false, Message = t.Message, CreatedAt = t.CreatedAt } };
            if (json["replies"] is JArray arr)
                msgs.AddRange(arr.Select(r => new SupportMessage
                {
                    Author = Str(r["author"]),
                    IsStaff = r["isStaff"]?.Value<bool>() ?? false,
                    Message = Str(r["message"]),
                    CreatedAt = Date(r["createdAt"]) ?? DateTime.Now,
                }));
            return (true, null, t, msgs);
        }

        public static async Task<(bool Ok, string Error, string Reference)> Create(string area, string subject, string message)
        {
            var (ok, error, json) = await SessionService.ApiPost("/api/support/my-tickets", new { area, subject, message });
            return ok ? (true, null, Str(json["reference"])) : (false, error, null);
        }

        public static async Task<(bool Ok, string Error)> Reply(int id, string message)
        {
            var (ok, error, _) = await SessionService.ApiPost($"/api/support/my-tickets/{id}/reply", new { message });
            return (ok, error);
        }

        // ---- "new from support" tracking (per device) ----

        private static string SeenKey(int id) => $"support_seen_{id}";
        private static string NotifiedKey(int id) => $"support_notified_{id}";
        private static long Stamp(SupportTicket t) => (t.LastStaffReplyAt?.Ticks ?? 0) + (t.ChatOffered ? 1 : 0);

        // Support has replied (or offered a chat) since the user last opened this ticket.
        public static bool HasNews(SupportTicket t) =>
            Stamp(t) > 0 && Preferences.Default.Get(SeenKey(t.Id), 0L) < Stamp(t);

        public static void MarkSeen(SupportTicket t)
        {
            Preferences.Default.Set(SeenKey(t.Id), Stamp(t));
            Preferences.Default.Set(NotifiedKey(t.Id), Stamp(t));
        }

        // True once per new reply/offer, so the dashboard pops up a notice only once.
        public static bool ShouldNotify(SupportTicket t)
        {
            if (!HasNews(t)) return false;
            if (Preferences.Default.Get(NotifiedKey(t.Id), 0L) >= Stamp(t)) return false;
            Preferences.Default.Set(NotifiedKey(t.Id), Stamp(t));
            return true;
        }
    }
}
