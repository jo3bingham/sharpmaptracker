using System.Collections.Generic;
using System.Drawing;
using SharpTibiaProxy.Domain;
using SharpTibiaProxy.Util;
using SharpTibiaProxy;

namespace OpenTibiaCommons.Domain
{
    public class OtTile
    {
        private Location location;
        private OtItem ground;
        private List<OtItem> items;
        private Color mapColor;
        private int downItemCount;

        public Location Location { get { return location; } }
        public OtItem Ground { get { return ground; } }
        public Color MapColor { get { return mapColor; } }
        public int ItemCount { get { return items == null ? 0 : items.Count; } }

        public List<OtItem> Items
        {
            get
            {
                if (items == null)
                    items = new List<OtItem>();
                return items;
            }
        }

        public OtTile(Location location)
        {
            this.location = location;
        }

        public void AddItem(OtItem item)
        {
            if (item.Type.Group == OtItemGroup.Ground)
            {
                ground = item;
                mapColor = Misc.GetAutomapColor(item.Type.MinimapColor);
            }
            else
            {
                if (item.Type.AlwaysOnTop)
                {
                    bool inserted = false;

                    for (int i = downItemCount; i < ItemCount; i++)
                    {
                        if (Items[i].Type.AlwaysOnTopOrder < item.Type.AlwaysOnTopOrder)
                            continue;

                        Items.Insert(i, item);
                        inserted = true;
                        break;
                    }

                    if (!inserted)
                        Items.Add(item);
                }
                else if (item.Type.IsMoveable)
                {
                    bool inserted = false;

                    for (int i = downItemCount; i < ItemCount; i++)
                    {
                        if (Items[i].Type.AlwaysOnTopOrder < item.Type.AlwaysOnTopOrder)
                        {
                            Items.Insert(i, item);
                            inserted = true;
                            break;
                        }
                    }

                    if (!inserted)
                    {
                        Items.Add(item);
                    }
                }
                else
                {
                    Items.Insert(0, item);
                    ++downItemCount;
                }

                Color color = Misc.GetAutomapColor(item.Type.MinimapColor);
                if (color != Color.Black)
                    mapColor = color;
            }
        }

        internal void InternalAddItem(OtItem item)
        {
            if (item.Type.Group == OtItemGroup.Ground)
            {
                ground = item;
                mapColor = Misc.GetAutomapColor(item.Type.MinimapColor);
            }
            else
            {
                if (item.Type.AlwaysOnTop)
                {
                    bool inserted = false;

                    for (int i = downItemCount; i < ItemCount; i++)
                    {
                        if (Items[i].Type.AlwaysOnTopOrder <= item.Type.AlwaysOnTopOrder)
                            continue;

                        Items.Insert(i, item);
                        inserted = true;
                        break;
                    }

                    if (!inserted)
                    {
                        Items.Add(item);
                    }
                }
                else if (item.Type.IsMoveable)
                {
                    bool inserted = false;

                    for (int i = downItemCount; i < ItemCount; i++)
                    {
                        if (Items[i].Type.AlwaysOnTopOrder < item.Type.AlwaysOnTopOrder)
                        {
                            Items.Insert(i, item);
                            inserted = true;
                            break;
                        }
                    }

                    if (!inserted)
                    {
                        Items.Add(item);
                    }
                }
                else
                {
                    Items.Insert(0, item);
                    ++downItemCount;
                }

                Color color = Misc.GetAutomapColor(item.Type.MinimapColor);
                if (color != Color.Black)
                    mapColor = color;
            }
        }

        public void Clear()
        {
            if (items != null)
                items.Clear();

            downItemCount = 0;
        }

        public uint HouseId { get; set; }
        public uint Flags { get; set; }
    }
}
