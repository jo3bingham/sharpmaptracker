using System;

namespace SharpTibiaProxy.Util
{
    public class Schedule : Task, IComparable
    {
        protected long _lifeCycle;
        protected uint _id;

        public Schedule(long delay, Action task)
            : base(task)
        {
            if (delay < Scheduler.MINTICKS)
                delay = Scheduler.MINTICKS;

            _lifeCycle = DateTime.Now.AddMilliseconds(delay).Ticks;
        }

        public long LifeCycle
        {
            get { return _lifeCycle; }
        }

        public uint Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public TimeSpan WaitTime
        {
            get
            {
                var waitTime = TimeSpan.FromTicks(_lifeCycle - DateTime.Now.Ticks);
                return waitTime.Milliseconds < 0 ? TimeSpan.Zero : waitTime;
            }
        }

        int IComparable.CompareTo(Object obj)
        {
            var task = obj as Schedule;
            return task != null ? _lifeCycle.CompareTo(task._lifeCycle) : 0;
        }
    }
}
