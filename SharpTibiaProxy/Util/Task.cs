using System;

namespace SharpTibiaProxy.Util
{
    public class Task
    {
        protected Action _action;
        protected long? _expiration;

        public Task(Action action, int? expiration = null)
        {
            if(expiration != null)
                _expiration = DateTime.Now.AddMilliseconds(expiration.Value).Ticks;

            _action = action;
        }

        public bool HasExpired
        {
            get
            {
                return _expiration != null && _expiration <= DateTime.Now.Ticks;
            }
        }

        public Action Action
        {
            get { return _action; }
        }
    }
}
