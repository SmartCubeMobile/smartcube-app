using Microsoft.AspNetCore.SignalR;

namespace SmartCubeMobile;

public class OtpHub : Hub
{
    public async Task RegisterSession(
        string sessionId)
    {
        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            sessionId);
    }

    public async Task UnregisterSession(
        string sessionId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            sessionId);
    }

    public async Task SendOtp(
        string sessionId,
        string otp)
    {
        await Clients.Group(sessionId)
            .SendAsync(
                "OtpReceived",
                sessionId,
                otp);
    }
}