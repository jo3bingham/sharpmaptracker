using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public class ShopItem
    {
        public ushort Id { get; set; }
        public byte SubType { get; set; }
        public string Name { get; set; }
        public uint Weight { get; set; }
        public uint BuyPrice { get; set; }
        public uint SellPrice { get; set; }

        public bool IsBuyable { get { return BuyPrice > 0; } }
        public bool IsSellable { get { return SellPrice > 0; } }
    }
}
