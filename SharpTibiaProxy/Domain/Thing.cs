using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public abstract class Thing
    {
        public abstract uint Id { get; }
        public abstract int Order { get; }
        public abstract bool IsAlwaysOnTop { get; }
        public abstract bool IsBlockingPath { get; }
    }
}
