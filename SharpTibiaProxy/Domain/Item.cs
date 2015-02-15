using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public class Item : Thing 
    {
        public ItemType Type { get; private set; }
        public override uint Id { get { return Type.Id; } }
        public override bool IsAlwaysOnTop { get { return Type.AlwaysOnTop; } }
        public byte Count { get; set; }
        public byte SubType { get; set; }

        public override int Order
        {
            get { return Type.AlwaysOnTopOrder; }
        }

        public override bool IsBlockingPath
        {
            get { return IsBlocking; }
        }

        public Item(ItemType type, byte count = 0, byte subType = 0)
        {
            this.Type = type;
            this.Count = count;
            this.SubType = subType;
        }

        public bool IsBlocking { get { return Type.BlockObject; } }

        public bool IsGround { get { return Type.IsGround; } }

        public bool IsStackable { get { return Type.IsStackable; } }

        public bool IsSplash { get { return Type.IsSplash; } }

        public bool IsFluid { get { return Type.IsFluidContainer; } }
    }
}
