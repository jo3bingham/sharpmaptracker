using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace SharpTibiaProxy.Domain
{
    public class Items
    {
        private Dictionary<ushort, ItemType> items;

        public int ItemCount { get; private set; }

        public Items()
        {
            items = new Dictionary<ushort, ItemType>();
        }

        public ItemType Get(ushort id)
        {
            if (items.ContainsKey(id))
                return items[id];

            return null;
        }

        public bool Load(string filename, int version)
        {
            FileStream fileStream = File.OpenRead(filename);
            try
            {
                using (BinaryReader reader = new BinaryReader(fileStream))
                {
                    UInt32 datSignature = reader.ReadUInt32();
                    //if (signature != 0 && datSignature != signature)
                    //{
                    //    Trace.WriteLine(String.Format("Plugin: Bad signature, dat signature is {0} and signature is {0}", datSignature, signature));
                    //    return false;
                    // }

                    //get max id
                    ItemCount = reader.ReadUInt16();
                    UInt16 creatureCount = reader.ReadUInt16();
                    UInt16 effectCount = reader.ReadUInt16();
                    UInt16 distanceCount = reader.ReadUInt16();

                    UInt16 minclientID = 100; //items starts at 100
                    int maxclientID = ItemCount + creatureCount;

                    UInt16 id = minclientID;
                    while (id <= maxclientID)
                    {
                        ItemType item = new ItemType();

                        item.Id = id;
                        item.AlwaysOnTopOrder = 5;
                        item.IsMoveable = true;

                        items[id] = item;

                        // read the options until we find 0xff
                        byte optbyte;
                        do
                        {
                            optbyte = reader.ReadByte();
                            //Trace.WriteLine(String.Format("{0:X}", optbyte));

                            switch (optbyte)
                            {
                                case 0x00: //groundtile
                                    item.GroundSpeed = reader.ReadUInt16();
                                    item.AlwaysOnTopOrder = 0;
                                    item.Group = ItemGroup.Ground;
                                    break;
                                case 0x01: //all OnTop
                                    item.AlwaysOnTop = true;
                                    item.AlwaysOnTopOrder = 1;
                                    break;
                                case 0x02: //can walk trough (open doors, arces, bug pen fence)
                                    item.AlwaysOnTop = true;
                                    item.AlwaysOnTopOrder = 2;
                                    break;
                                case 0x03: //can walk trough (arces)
                                    item.AlwaysOnTop = true;
                                    item.AlwaysOnTopOrder = 3;
                                    break;
                                case 0x04: //container
                                    item.Group = ItemGroup.Container;
                                    break;
                                case 0x05: //stackable
                                    item.IsStackable = true;
                                    break;
                                case 0x06: //unknown
                                    break;
                                case 0x07: //useable
                                    item.HasUseWith = true;
                                    break;
                                case 0x08: //read/write-able
                                    item.IsReadable = true;
                                    //item.isWriteable = true;
                                    item.MaxReadWriteChars = reader.ReadUInt16();
                                    break;
                                case 0x09: //readable
                                    item.IsReadable = true;
                                    item.MaxReadChars = reader.ReadUInt16();
                                    break;
                                case 0x0A: //fluid containers
                                    item.Group = ItemGroup.FluidContainer;
                                    break;
                                case 0x0B: //splashes
                                    item.Group = ItemGroup.Splash;
                                    break;
                                case 0x0C: //blocks solid objects (creatures, walls etc)
                                    item.BlockObject = true;
                                    break;
                                case 0x0D: //not moveable
                                    item.IsMoveable = false;
                                    break;
                                case 0x0E: //blocks missiles (walls, magic wall etc)
                                    item.BlockProjectile = true;
                                    break;
                                case 0x0F: //blocks pathfind algorithms (monsters)
                                    item.BlockPathFind = true;
                                    break;
                                case 0x10:
                                    item.IsNoMovementAnimation = true;
                                    break;
                                case 0x11: //blocks monster movement (flowers, parcels etc)
                                    item.IsPickupable = true;
                                    break;
                                case 0x12: //hangable objects (wallpaper etc)
                                    item.IsHangable = true;
                                    break;
                                case 0x13: //horizontal wall
                                    item.IsHorizontal = true;
                                    break;
                                case 0x14: //vertical wall
                                    item.IsVertical = true;
                                    break;
                                case 0x15: //rotatable
                                    item.IsRotatable = true;
                                    break;
                                case 0x16: //light info
                                    item.LightLevel = reader.ReadUInt16();
                                    item.LightColor = reader.ReadUInt16();
                                    break;
                                case 0x17: //unknown
                                    break;
                                case 0x18: //changes floor
                                    break;
                                case 0x19: //unknown
                                    reader.BaseStream.Seek(4, SeekOrigin.Current);
                                    break;
                                case 0x1A:
                                    item.HasHeight = true;
                                    /*UInt16 height =*/
                                    reader.ReadUInt16();
                                    break;
                                case 0x1B: //unknown
                                    break;
                                case 0x1C: //unknown
                                    break;
                                case 0x1D: //minimap color
                                    item.MinimapColor = reader.ReadUInt16();
                                    break;
                                case 0x1E: //in-game help info
                                    UInt16 opt = reader.ReadUInt16();
                                    if (opt == 1112)
                                        item.IsReadable = true;
                                    break;
                                case 0x1F: //full tile
                                    item.WalkStack = true;
                                    break;
                                case 0x20: //look through (borders)
                                    item.LookThrough = true;
                                    break;
                                case 0x21: //unknown
                                    reader.ReadUInt16();
                                    break;
                                case 0x22: //market
                                    reader.ReadUInt16(); // category
                                    item.WareId = reader.ReadUInt16(); // trade as
                                    reader.ReadUInt16(); // show as
                                    var size = reader.ReadUInt16();
                                    item.Name = new string(reader.ReadChars(size));
                                    reader.ReadUInt16(); // profession
                                    reader.ReadUInt16(); // level
                                    break;
                                case 0x23: //default action
                                    reader.ReadUInt16();
                                    break;
                                case 0xFE: //is usable
                                    break;
                                case 0xFF: //end of attributes
                                    break;
                                default:
                                    Trace.WriteLine(String.Format("Plugin: Error while parsing, unknown optbyte 0x{0:X} at id {1}", optbyte, id));
                                    return false;
                            }
                        } while (optbyte >= 0 && optbyte != 0xFF);

                        if (version >= ClientVersion.Version1057.Number)
                        {
                            var isOutfit = id > ItemCount ? reader.ReadByte() : 1;
                            for (int j = 0; j < isOutfit; j++)
                            {
                                var frameGroup = id > ItemCount ? reader.ReadByte() : 0;
                                var width = reader.ReadByte();
                                var height = reader.ReadByte();
                                if ((width > 1) || (height > 1))
                                {
                                    reader.BaseStream.Position++;
                                }

                                var frames = reader.ReadByte();
                                var xdiv = reader.ReadByte();
                                var ydiv = reader.ReadByte();
                                var zdiv = reader.ReadByte();
                                var animationLength = reader.ReadByte();
                                item.IsAnimation = animationLength > 1;

                                if (item.IsAnimation && version > ClientVersion.Version1041.Number)
                                {
                                    reader.ReadByte();
                                    reader.ReadUInt32();
                                    reader.ReadByte();
                                    for (int i = 0; i < animationLength; i++)
                                    {
                                        reader.ReadUInt32();
                                        reader.ReadUInt32();
                                    }
                                }

                                var numSprites =
                                    (UInt32)width * (UInt32)height *
                                    (UInt32)frames *
                                    (UInt32)xdiv * (UInt32)ydiv * zdiv *
                                    (UInt32)animationLength;

                                // Read the sprite ids
                                for (UInt32 i = 0; i < numSprites; ++i)
                                {
                                    var spriteId = reader.ReadUInt32();
                                    /*Sprite sprite;
                                    if (!sprites.TryGetValue(spriteId, out sprite))
                                    {
                                        sprite = new Sprite();
                                        sprite.id = spriteId;
                                        sprites[spriteId] = sprite;
                                    }
                                    item.spriteList.Add(sprite);*/
                                }
                                ++id;
                            }
                        }
                        else
                        {
                            var width = reader.ReadByte();
                            var height = reader.ReadByte();
                            if ((width > 1) || (height > 1))
                            {
                                reader.BaseStream.Position++;
                            }

                            var frames = reader.ReadByte();
                            var xdiv = reader.ReadByte();
                            var ydiv = reader.ReadByte();
                            var zdiv = reader.ReadByte();
                            var animationLength = reader.ReadByte();
                            item.IsAnimation = animationLength > 1;

                            if (item.IsAnimation && version > ClientVersion.Version1041.Number)
                            {
                                reader.ReadByte();
                                reader.ReadUInt32();
                                reader.ReadByte();
                                for (int i = 0; i < animationLength; i++)
                                {
                                    reader.ReadUInt32();
                                    reader.ReadUInt32();
                                }
                            }

                            var numSprites =
                                (UInt32)width * (UInt32)height *
                                (UInt32)frames *
                                (UInt32)xdiv * (UInt32)ydiv * zdiv *
                                (UInt32)animationLength;

                            // Read the sprite ids
                            for (UInt32 i = 0; i < numSprites; ++i)
                            {
                                var spriteId = reader.ReadUInt32();
                                /*Sprite sprite;
                                if (!sprites.TryGetValue(spriteId, out sprite))
                                {
                                    sprite = new Sprite();
                                    sprite.id = spriteId;
                                    sprites[spriteId] = sprite;
                                }
                                item.spriteList.Add(sprite);*/
                            }
                            ++id;
                        }
                    }
                }
            }
            finally
            {
                fileStream.Close();
            }

            return true;

        }



    }
}
