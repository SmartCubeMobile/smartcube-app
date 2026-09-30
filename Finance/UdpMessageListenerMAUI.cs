using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SmartCubeMobile;

public class UdpMessageListener
{
    private readonly int port;

    private UdpClient udpClient;

    private bool running;

    private int activeRequests;

    private readonly object syncRoot = new();

    public event Action<NotificationPacket> MessageReceived;
    
    public event Action StateChanged;

    public bool IsRunning
    {
        get
        {
            lock (syncRoot)
            {
                return running;
            }
        }
    }

    public int ActiveRequests
    {
        get
        {
            lock (syncRoot)
            {
                return activeRequests;
            }
        }
    }

    public UdpMessageListener(int port)
    {
        this.port = port;
    }

    public void RegisterRequest()
    {
        lock (syncRoot)
        {
            activeRequests++;

            if (!running)
            {
                Start();
            }

            RaiseStateChanged();
        }
    }

    public void UnregisterRequest()
    {
        lock (syncRoot)
        {
            activeRequests--;

            if (activeRequests <= 0)
            {
                activeRequests = 0;
                Stop();
            }

            RaiseStateChanged();
        }
    }

    private void Start()
    {
        if (running)
            return;

        running = true;

        udpClient = new UdpClient(port);

        _ = ListenLoopAsync();

        RaiseStateChanged();
    }

    private void Stop()
    {
        if (!running)
            return;

        running = false;

        try
        {
            udpClient.Close();
        }
        catch
        {
        }

        udpClient = null;

        RaiseStateChanged();
    }

    private void RaiseStateChanged()
    {
        StateChanged?.Invoke();
    }

    private async Task ListenLoopAsync()
    {
        while (running)
        {
            try
            {
                var result = await udpClient!.ReceiveAsync();

                string json = Encoding.UTF8.GetString(result.Buffer);

                NotificationPacket message =
                    JsonSerializer.Deserialize<NotificationPacket>(json);

                if (message == null)
                {
                    continue;
                }

                MessageReceived?.Invoke(message);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
    }
}