using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SmartCubeMobile;

public class NotificationListener : IDisposable
{
    private readonly UdpClient udp;

    public NotificationListener(int port)
    {
        udp = new UdpClient(port);
    }

    public void Dispose()
    {
        udp.Dispose();
    }
    public async Task<NotificationPacket> WaitForNotificationAsync(
#if DEBUGMAUI
                                                                    MainViewModel ourviewmodel,
#endif
                                                                    TimeSpan timeout,
                                                                    string origin)
    {
        //using var udp = new UdpClient(port);
        using var cts = new CancellationTokenSource(timeout);

        while (!cts.IsCancellationRequested)
        {
#if DEBUGMAUI
            await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Inside loop");
#endif
            try
            {
                var receiveTask = udp.ReceiveAsync();

                var completed = await Task.WhenAny(
                    receiveTask,
                    Task.Delay(Timeout.Infinite, cts.Token));

                if (completed != receiveTask)
                {
#if DEBUGMAUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Timeout");
#endif
                    return null;    // timeout
                }
                var result = await receiveTask;

                string json = Encoding.UTF8.GetString(result.Buffer);

                var packet = JsonSerializer.Deserialize<NotificationPacket>(json);

                if (packet == null)
                {
#if DEBUGMAUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Packet is null");
#endif
                    continue;
                }
                if (!NotificationValidator.IsValid(packet, origin))
                {
#if DEBUGMAUI
                    await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Packet is invalid");
#endif
                    continue;
                }
#if DEBUGMAUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Returning packet" + packet.Text);
#endif
                return packet;
            }
            catch (OperationCanceledException)
            {
#if DEBUGMAUI
                await SmartRoutinesV2018.TextBlockUpdate(ourviewmodel, $"Step3 Operation cancelled");
#endif
                return null;
            }
        }
        return null;
    }
}