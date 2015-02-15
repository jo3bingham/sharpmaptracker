using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public class Shop
    {
        public string Name { get; private set; }
        public List<ShopItem> Items { get; private set; }

        public Shop(string name)
        {
            this.Name = name;
            Items = new List<ShopItem>();
        }
    }
}
