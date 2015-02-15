using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace SharpTibiaProxy.Util
{
    public class Dispatcher
    {
        public const int TaskExpiration = 2000;

        public event EventHandler<DispatcherDispatchingTaskEventArgs> DispatchingTask;
        public event EventHandler<DispatcherTaskDispatchedEventArgs> TaskDispatched;

        private Thread _thread;
        private readonly Queue<Task> _queue;
        private DispatcherState _state;

        private readonly object _lock;

        public Dispatcher()
        {
            _lock = new object();
            _queue = new Queue<Task>();
            _state = DispatcherState.Terminated;
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_state == DispatcherState.Running)
                    return;

                _state = DispatcherState.Running;
                _thread = new Thread(Run) { IsBackground = false };
                _thread.Start();
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (_state != DispatcherState.Running)
                    return;

                _state = DispatcherState.Closing;
            }
        }

        public void Shutdown()
        {
            lock (_lock)
            {
                if (_state == DispatcherState.Terminated)
                    return;

                _state = DispatcherState.Terminated;
                _queue.Clear();

                Monitor.Pulse(_lock);
            }
        }

        public void Add(Task task)
        {
            lock (_lock)
            {
                if (_state != DispatcherState.Running)
                    return;

                _queue.Enqueue(task);
                Monitor.Pulse(_lock);
            }
        }

        private void Run()
        {
            while (_state != DispatcherState.Terminated)
            {
                Task task = null;

                lock (_lock)
                {
                    if (_queue.Count == 0)
                    {
                        Monitor.Wait(_lock);
                    }

                    if (_queue.Count > 0 && _state != DispatcherState.Terminated)
                    {
                        task = _queue.Dequeue();
                    }

                }

                if (task == null)
                    continue;

                if (!task.HasExpired)
                {
                    try
                    {
                        DispatchingTask.Raise(this, new DispatcherDispatchingTaskEventArgs(task));
                        task.Action();
                        TaskDispatched.Raise(this, new DispatcherTaskDispatchedEventArgs(task));
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(string.Format("[ERROR] Can't execute task {0}. Details: {1}", task.Action.Method.Name, e));
                    }
                }
            }
        }
    }

    public class DispatcherTaskDispatchedEventArgs : EventArgs
    {
        private Task _task;

        public DispatcherTaskDispatchedEventArgs(Task task)
        {
            this._task = task;
        }

        public Task Task { get { return this._task; } }
    }

    public class DispatcherDispatchingTaskEventArgs : EventArgs
    {
        private Task _task;

        public DispatcherDispatchingTaskEventArgs(Task task)
        {
            this._task = task;
        }

        public Task Task { get { return this._task; } }
    }
}
