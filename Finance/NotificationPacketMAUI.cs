namespace SmartCubeMobile;

public class Notification
{
    public string AppName { get; set; } = "";
    public string PackageName { get; set; } = "";
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime Timestamp { get; set; }
}

public class NotificationPacket : Notification
{
    public string Token { get; set; }
}