using Microsoft.Maui.Dispatching;

namespace SmartCubeMobile;

public class NotificationRequest
{
    private readonly UdpMessageListener listener;
    private readonly TimeSpan timeout;

    private IDispatcherTimer timer;
    private int seconds;

    private bool cancelled;

    private readonly TaskCompletionSource<NotificationResult> tcs =
        new();

    public string Origin { get; }

    public Label StatusLabel { get; set; }
    public Label TimerLabel { get; set; }
    
    public NotificationRequest(
        string origin,
        UdpMessageListener listener,
        TimeSpan timeout)
    {
        Origin = origin;
        this.listener = listener;
        this.timeout = timeout;
    }

    // =========================
    // Public API
    // =========================

    public Task<NotificationResult> StartAsync()
    {
        StatusLabel.Text = $"Waiting for {Origin}...";

        listener.RegisterRequest();
        listener.MessageReceived += OnMessageReceived;

        StartTimer();

        _ = Task.Run(async () =>
        {
            await Task.Delay(timeout);

            if (cancelled)
                return;

            tcs.TrySetResult(new NotificationResult
            {
                Message = null,
                Cancelled = false
            });

            Cleanup();
        });

        return tcs.Task;
    }

    public void Cancel()
    {
        cancelled = true;

        tcs.TrySetResult(new NotificationResult
        {
            Message = null,
            Cancelled = true
        });

        Cleanup();
    }

    // =========================
    // Message handling
    // =========================

    private void OnMessageReceived(NotificationPacket msg)
    {
        if (cancelled)
            return;

        //if (!string.Equals(
        //        msg.Origin,
        //        Origin,
        //        StringComparison.OrdinalIgnoreCase))
        //{
        //    return;
        //}

        if (!NotificationValidator.IsValid(msg, Origin))
            return;

        tcs.TrySetResult(new NotificationResult
        {
            Message = msg,
            Cancelled = false
        });

        Cleanup();
    }

    // =========================
    // Cleanup
    // =========================

    private void Cleanup()
    {
        listener.MessageReceived -= OnMessageReceived;
        listener.UnregisterRequest();

        StopTimer();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            StatusLabel.Text = cancelled
                ? $"{Origin} cancelled"
                : $"{Origin} finished";
        });
    }

    // =========================
    // Timer
    // =========================

    private void StartTimer()
    {
        seconds = 0;

        timer = Dispatcher
            .GetForCurrentThread()
            .CreateTimer();

        timer.Interval = TimeSpan.FromSeconds(1);

        timer.Tick += (_, _) =>
        {
            seconds++;

            TimerLabel.Text = $"{Origin}: {seconds}s";
        };

        timer.Start();
    }

    private void StopTimer()
    {
        timer.Stop();
    }
}