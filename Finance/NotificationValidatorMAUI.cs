using System.Security.Cryptography;
using System.Text;

namespace SmartCubeMobile;

public static class NotificationValidator
{
    public static bool IsValid(NotificationPacket packet, string origin)
    {
        if (packet == null)
            return false;

        // Check this notification is for the required origin
        if (!packet.Title.Contains(
                origin,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // Optional: reject stale notifications
        //if (DateTime.UtcNow - msg.Timestamp.ToUniversalTime() >
        //    TimeSpan.FromMinutes(3))
        //{
        //    return false;
        //}

        string expected =
            CreateToken(
                packet.AppName,
                packet.PackageName,
                packet.Title,
                packet.Text,
                packet.Timestamp);

        return string.Equals(
            expected,
            packet.Token,
            StringComparison.OrdinalIgnoreCase);
    }

    private static string CreateToken(
        string appName,
        string packageName,
        string title,
        string text,
        DateTime timestamp)
    {
        string source =
            $"{appName}|{packageName}|{title}|{text}|{timestamp}";

        using var sha = SHA256.Create();

        byte[] hash =
            sha.ComputeHash(
                Encoding.UTF8.GetBytes(source));

        return Convert.ToHexString(hash);
    }
}