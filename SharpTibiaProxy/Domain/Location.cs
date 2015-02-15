using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public class Location
    {
        public static readonly Location Invalid = new Location(-1, -1, -1);

        private int x, y, z;

        public Location(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            //this.stack = stack;
        }

        public int X { get { return x; } }
        public int Y { get { return y; } }
        public int Z { get { return z; } }
        //public int Stack { get { return stack; } }

        public bool IsCreature
        {
            get
            {
                return this.x == 65535;
            }
        }

        public uint GetCretureId(int stack)
        {
            if (this.IsCreature)
                return checked((uint)(this.y | this.z << 16 | stack << 24));

            throw new InvalidOperationException("Attempted to get the creature id of a location that is not a creature reference.");
        }

        public ulong ToIndex()
        {
            return (ulong)((uint)x & 0xFFFF) << 24 | ((uint)y & 0xFFFF) << 8 | ((uint)z & 0xFF);
        }

        public static ulong ToIndex(int x, int y, int z)
        {
            return (ulong)((uint)x & 0xFFFF) << 24 | ((uint)y & 0xFFFF) << 8 | ((uint)z & 0xFF);
        }

        public static Location FromIndex(ulong index)
        {
            return new Location((int)((index >> 24) & 0xFFFF), (int)((index >> 8) & 0xFFFF), (int)(index & 0xFF));
        }

        public override bool Equals(object obj)
        {
            var other = obj as Location;
            return other != null && other.x == x && other.y == y && other.z == z;
        }

        public override int GetHashCode()
        {
            return (x + y + z) ^ 31;
        }

        public override string ToString()
        {
            return "[" + x + ", " + y + ", " + z + "]";
        }

    }
}
