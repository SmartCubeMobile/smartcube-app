//using Microsoft.AspNetCore.SignalR.Client;
//using System.Collections.Concurrent;

//namespace SmartCubeMobile;

//public sealed class OtpService : IAsyncDisposable
//{
//    private HubConnection? _connection;

//    private readonly ConcurrentDictionary<string,
//        TaskCompletionSource<string>> _pendingSessions =
//            new();

//    public bool IsConnected =>
//        _connection?.State == HubConnectionState.Connected;

//    public async Task ConnectAsync(string hubUrl)
//    {
//        if (_connection != null)
//            return;

//        _connection = new HubConnectionBuilder()
//            .WithUrl(hubUrl)
//            .WithAutomaticReconnect()
//            .Build();

//        _connection.On<string, string>(
//            "OtpReceived",
//            (sessionId, otp) =>
//            {
//                if (_pendingSessions.TryGetValue(
//                        sessionId,
//                        out var tcs))
//                {
//                    tcs.TrySetResult(otp);
//                }
//            });

//        await _connection.StartAsync();
//    }

//    public async Task RegisterSessionAsync(
//        string sessionId)
//    {
//        if (_connection == null)
//            throw new InvalidOperationException(
//                "SignalR connection not started.");

//        await _connection.InvokeAsync(
//            "RegisterSession",
//            sessionId);
//    }

//    public async Task<string?> WaitForOtpAsync(
//        string sessionId,
//        TimeSpan timeout,
//        CancellationToken cancellationToken = default)
//    {
//        if (_connection == null)
//            throw new InvalidOperationException(
//                "SignalR connection not started.");

//        var tcs =
//            new TaskCompletionSource<string>(
//                TaskCreationOptions.RunContinuationsAsynchronously);

//        _pendingSessions[sessionId] = tcs;

//        try
//        {
//            using var linkedCts =
//                CancellationTokenSource.CreateLinkedTokenSource(
//                    cancellationToken);

//            linkedCts.CancelAfter(timeout);

//            var cancelTask =
//                Task.Delay(
//                    Timeout.Infinite,
//                    linkedCts.Token);

//            var completed =
//                await Task.WhenAny(
//                    tcs.Task,
//                    cancelTask);

//            if (completed == tcs.Task)
//            {
//                return await tcs.Task;
//            }

//            return null;
//        }
//        finally
//        {
//            _pendingSessions.TryRemove(
//                sessionId,
//                out _);
//        }
//    }

//    public async Task UnregisterSessionAsync(
//        string sessionId)
//    {
//        if (_connection == null)
//            return;

//        try
//        {
//            await _connection.InvokeAsync(
//                "UnregisterSession",
//                sessionId);
//        }
//        catch
//        {
//            // Ignore disconnect errors
//        }

//        _pendingSessions.TryRemove(
//            sessionId,
//            out _);
//    }

//    public async ValueTask DisposeAsync()
//    {
//        foreach (var session in _pendingSessions.Keys)
//        {
//            _pendingSessions.TryRemove(
//                session,
//                out _);
//        }

//        if (_connection != null)
//        {
//            await _connection.DisposeAsync();
//            _connection = null;
//        }
//    }
//}