using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCubeMobile
{
    public class TickScheduler
    {
        private readonly List<PeriodicTickTask> _tasks = new();
        private CancellationTokenSource _cts;
        private bool _started;

        public void Register(PeriodicTickTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            _tasks.Add(task);
        }

        public void Start()
        {
            if (_started)
            {
                return;
            }

            _cts = new CancellationTokenSource();
            _started = true;

            foreach (var task in _tasks)
            {
                task.Start(_cts.Token);
            }
        }

        public void Stop()
        {
            if (!_started)
            {
                return;
            }

            _cts.Cancel();
            _started = false;
        }
    }
    public abstract class PeriodicTickTask
    {
        private readonly TimeSpan _interval;
        private bool _running;
        private CancellationToken _token;
        protected PeriodicTickTask(TimeSpan interval)
        {
            _interval = interval;
        }

        public void Start(CancellationToken token)
        {
            if (_running)
            {
                return;
            }

            _token = token;
            _running = true;

            //Task.Run(async () =>
            //{
            //    try
            //    {
            //        while (!_token.IsCancellationRequested)
            //        {
            //            await ExecuteAsync();                        
            //            await Task.Delay(_interval, _token);
            //        }
            //    }
            //    catch (TaskCanceledException)
            //    {
            //        // Expected when stopping
            //        Console.WriteLine("here");
            //    }
            //    finally
            //    {
            //        _running = false;
            //    }
            //}, _token);

            Task.Run(async () =>
            {
                try
                {
                    while (!_token.IsCancellationRequested)
                    {
                        try
                        {
                            await ExecuteAsync();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(
                                $"Tick task exception: {ex}");
                        }

                        await Task.Delay(_interval, _token);
                    }
                }
                catch (TaskCanceledException)
                {
                    // Normal shutdown
                }
                finally
                {
                    _running = false;
                }
            }, _token);
        }

        protected abstract Task ExecuteAsync();
    }
}
