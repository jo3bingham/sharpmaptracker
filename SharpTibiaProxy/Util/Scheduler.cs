using System;
using System.Collections.Generic;
using System.Threading;

namespace SharpTibiaProxy.Util
{
    public class Scheduler
    {
        public const int MINTICKS = 50;

        private Thread _thread;
        private readonly PriorityQueue<Schedule> _queue;
        private SchedulerState _state;
        private uint _lastScheduletId;
        private readonly ISet<uint> _scheduleIds;

        private readonly object _lock;

        private Dispatcher _dispatcher;

        public Dispatcher Dispatcher { get { return _dispatcher; } }

        public Scheduler(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _lock = new object();
            _state = SchedulerState.Terminated;
            _queue = new PriorityQueue<Schedule>();
            _lastScheduletId = 0;
            _scheduleIds = new HashSet<uint>();
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_state == SchedulerState.Running)
                    return;
#if _DEBUG_SCHEDULER
                Logger.Debug(string.Format("Scheduler: Starting {0} scheduler.", Name));
#endif
                _state = SchedulerState.Running;
                _thread = new Thread(Run) { IsBackground = false };
                _thread.Start();
            }
        }

        public void Shutdown()
        {
            lock (_lock)
            {
                if (_state == SchedulerState.Terminated)
                    return;
#if _DEBUG_SCHEDULER
                Logger.Debug(string.Format("Scheduler: Stoping {0} scheduler.", Name));
#endif
                _state = SchedulerState.Terminated;
                _queue.Clear();
                _scheduleIds.Clear();
                _lastScheduletId = 0;
                Monitor.Pulse(_lock);
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (_state != SchedulerState.Running)
                    return;

                _state = SchedulerState.Closing;
            }
        }

        public uint Add(Schedule schedule)
        {
            lock (_lock)
            {
                if (_state != SchedulerState.Running)
                    return 0;

                if (schedule.Id == 0)
                {
                    if (_lastScheduletId == uint.MaxValue)
                        _lastScheduletId = 0;

                    schedule.Id = ++_lastScheduletId;
                }
#if _DEBUG_SCHEDULER
                Logger.Debug(string.Format("Scheduler: Adding {0} to {1} scheduler.", schedule.Id, Name));
#endif
                _scheduleIds.Add(schedule.Id);
                _queue.Push(schedule);

                if (_queue.Peek() == schedule)
                    Monitor.Pulse(_lock);
            }

            return schedule.Id;
        }

        public bool Remove(uint scheduleId)
        {
            lock (_lock)
            {
#if _DEBUG_SCHEDULER
                Logger.Debug(string.Format("Scheduler: Removing {0} from {1} scheduler.", scheduleId, Name));
#endif
                return _scheduleIds.Remove(scheduleId);
            }
        }

        private void Run()
        {
            while (_state != SchedulerState.Terminated)
            {
                Schedule schedule = null;
                var ret = true;
                var runTask = false;

                lock (_lock)
                {
                    if (_queue.Empty)
                    {
#if _DEBUG_SCHEDULER
                        Logger.Debug(string.Format("Scheduler: {0} scheduler waiting.", Name));
#endif
                        Monitor.Wait(_lock);
                    }
                    else
                        ret = _queue.Peek().LifeCycle > DateTime.Now.Ticks && Monitor.Wait(_lock, _queue.Peek().WaitTime);

                    if (ret == false && _state != SchedulerState.Terminated)
                    {
                        schedule = _queue.Pop();

                        if (schedule != null && _scheduleIds.Remove(schedule.Id))
                            runTask = true;
                    }
                }

                if (runTask)
                {
#if _DEBUG_SCHEDULER
                    Logger.Debug("Scheduler: Sending task " + schedule.Id + " to dispatcher");
#endif

                    _dispatcher.Add(schedule);

                }
            }
        }
    }
}
