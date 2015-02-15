using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTibiaCommons.IO;
using System.Diagnostics;
using System.Xml.Linq;
using SharpTibiaProxy;

namespace OpenTibiaCommons.Domain
{
    public class OtItems
    {
        #region Enums

        private enum OtbItemGroup
        {
            NONE = 0,
            GROUND,
            CONTAINER,
            WEAPON,
            AMMUNITION,
            ARMOR,
            CHARGES,
            TELEPORT,
            MAGICFIELD,
            WRITEABLE,
            KEY,
            SPLASH,
            FLUID,
            DOOR,
            DEPRECATED,
            LAST
        };

        private enum OtbItemAttr
        {
            ITEM_ATTR_FIRST = 0x10,
            ITEM_ATTR_SERVERID = ITEM_ATTR_FIRST,
            ITEM_ATTR_CLIENTID,
            ITEM_ATTR_NAME,
            ITEM_ATTR_DESCR,			/*deprecated*/
            ITEM_ATTR_SPEED,
            ITEM_ATTR_SLOT,				/*deprecated*/
            ITEM_ATTR_MAXITEMS,			/*deprecated*/
            ITEM_ATTR_WEIGHT,			/*deprecated*/
            ITEM_ATTR_WEAPON,			/*deprecated*/
            ITEM_ATTR_AMU,				/*deprecated*/
            ITEM_ATTR_ARMOR,			/*deprecated*/
            ITEM_ATTR_MAGLEVEL,			/*deprecated*/
            ITEM_ATTR_MAGFIELDTYPE,		/*deprecated*/
            ITEM_ATTR_WRITEABLE,		/*deprecated*/
            ITEM_ATTR_ROTATETO,			/*deprecated*/
            ITEM_ATTR_DECAY,			/*deprecated*/
            ITEM_ATTR_SPRITEHASH,
            ITEM_ATTR_MINIMAPCOLOR,
            ITEM_ATTR_07,
            ITEM_ATTR_08,
            ITEM_ATTR_LIGHT,			/*deprecated*/

            //1-byte aligned
            ITEM_ATTR_DECAY2,			/*deprecated*/
            ITEM_ATTR_WEAPON2,			/*deprecated*/
            ITEM_ATTR_AMU2,				/*deprecated*/
            ITEM_ATTR_ARMOR2,			/*deprecated*/
            ITEM_ATTR_WRITEABLE2,		/*deprecated*/
            ITEM_ATTR_LIGHT2,
            ITEM_ATTR_TOPORDER,
            ITEM_ATTR_WRITEABLE3,		/*deprecated*/
            ITEM_ATTR_WAREID,

            ITEM_ATTR_LAST
        };

        private enum RootAttr
        {
            ROOT_ATTR_VERSION = 0x01
        };

        [FlagsAttribute]
        private enum OtbItemFlags
        {
            BLOCK_SOLID = 1,
            BLOCK_PROJECTILE = 2,
            BLOCK_PATHFIND = 4,
            HAS_HEIGHT = 8,
            USEABLE = 16,
            PICKUPABLE = 32,
            MOVEABLE = 64,
            STACKABLE = 128,
            FLOORCHANGEDOWN = 256,
            FLOORCHANGENORTH = 512,
            FLOORCHANGEEAST = 1024,
            FLOORCHANGESOUTH = 2048,
            FLOORCHANGEWEST = 4096,
            ALWAYSONTOP = 8192,
            READABLE = 16384,
            ROTABLE = 32768,
            HANGABLE = 65536,
            VERTICAL = 131072,
            HORIZONTAL = 262144,
            CANNOTDECAY = 524288,		/*deprecated*/
            ALLOWDISTREAD = 1048576,
            CORPSE = 2097152,			/*deprecated*/
            CLIENTCHARGES = 4194304,	/*deprecated*/
            LOOKTHROUGH = 8388608,
            ANIMATION = 16777216,
            WALKSTACK = 33554432
        };

        #endregion

        private Dictionary<ushort, OtItemType> clientItemMap = new Dictionary<ushort, OtItemType>();
        private Dictionary<ushort, OtItemType> serverItemMap = new Dictionary<ushort, OtItemType>();

        public uint MajorVersion { get; set; }
        public uint MinorVersion { get; set; }
        public uint BuildNumber { get; set; }

        public void AddItem(OtItemType item)
        {
            serverItemMap[item.Id] = item;
            clientItemMap[item.SpriteId] = item;
        }

        public OtItemType GetItem(ushort id)
        {
            if (serverItemMap.ContainsKey(id))
                return serverItemMap[id];

            return null;
        }

        public OtItemType GetItemBySpriteId(ushort spriteId)
        {
            if (clientItemMap.ContainsKey(spriteId))
                return clientItemMap[spriteId];

            return null;
        }

        public void Load(string fileName)
        {
            LoadOtb(fileName);
            LoadXml(Path.ChangeExtension(fileName, "xml"));
        }

        public void LoadOtb(string fileName)
        {
            if (!File.Exists(fileName))
                throw new Exception(string.Format("File not found {0}.", fileName));

            using (var reader = new OtFileReader(fileName))
            {
                var node = reader.GetRootNode();
                OtPropertyReader props = reader.GetPropertyReader(node);

                props.ReadByte(); //junk?
                props.ReadUInt32();

                byte attr = props.ReadByte();
                if ((RootAttr)attr == RootAttr.ROOT_ATTR_VERSION)
                {
                    var datalen = props.ReadUInt16();

                    if (datalen != 140)
                        throw new Exception("Size of version header is invalid.");

                    MajorVersion = props.ReadUInt32();
                    MinorVersion = props.ReadUInt32();
                    BuildNumber = props.ReadUInt32();
                }

                if (MajorVersion == 0xFFFFFFFF)
                    Trace.WriteLine("[Warning] items.otb using generic client version.");
                else if (MajorVersion < 3)
                    throw new Exception("Old version of items.otb detected, a newer version of items.otb is required.");
                else if (MajorVersion > 3)
                    throw new Exception("New version of items.otb detected, a newer version of the server is required.");

                node = node.Child;

                while (node != null)
                {
                    props = reader.GetPropertyReader(node);

                    OtItemType item = new OtItemType();
                    byte itemGroup = (byte)node.Type;

                    switch ((OtbItemGroup)itemGroup)
                    {
                        case OtbItemGroup.NONE: item.Group = OtItemGroup.None; break;
                        case OtbItemGroup.GROUND: item.Group = OtItemGroup.Ground; break;
                        case OtbItemGroup.SPLASH: item.Group = OtItemGroup.Splash; break;
                        case OtbItemGroup.FLUID: item.Group = OtItemGroup.FluidContainer; break;
                        case OtbItemGroup.CONTAINER: item.Group = OtItemGroup.Container; break;
                        case OtbItemGroup.DEPRECATED: item.Group = OtItemGroup.Deprecated; break;
                        default: break;
                    }

                    OtbItemFlags flags = (OtbItemFlags)props.ReadUInt32();

                    item.BlockObject = ((flags & OtbItemFlags.BLOCK_SOLID) == OtbItemFlags.BLOCK_SOLID);
                    item.BlockProjectile = ((flags & OtbItemFlags.BLOCK_PROJECTILE) == OtbItemFlags.BLOCK_PROJECTILE);
                    item.BlockPathFind = ((flags & OtbItemFlags.BLOCK_PATHFIND) == OtbItemFlags.BLOCK_PATHFIND);
                    item.IsPickupable = ((flags & OtbItemFlags.PICKUPABLE) == OtbItemFlags.PICKUPABLE);
                    item.IsMoveable = ((flags & OtbItemFlags.MOVEABLE) == OtbItemFlags.MOVEABLE);
                    item.IsStackable = ((flags & OtbItemFlags.STACKABLE) == OtbItemFlags.STACKABLE);
                    item.AlwaysOnTop = ((flags & OtbItemFlags.ALWAYSONTOP) == OtbItemFlags.ALWAYSONTOP);
                    item.IsVertical = ((flags & OtbItemFlags.VERTICAL) == OtbItemFlags.VERTICAL);
                    item.IsHorizontal = ((flags & OtbItemFlags.HORIZONTAL) == OtbItemFlags.HORIZONTAL);
                    item.IsHangable = ((flags & OtbItemFlags.HANGABLE) == OtbItemFlags.HANGABLE);
                    item.IsRotatable = ((flags & OtbItemFlags.ROTABLE) == OtbItemFlags.ROTABLE);
                    item.IsReadable = ((flags & OtbItemFlags.READABLE) == OtbItemFlags.READABLE);
                    item.HasUseWith = ((flags & OtbItemFlags.USEABLE) == OtbItemFlags.USEABLE);
                    item.HasHeight = ((flags & OtbItemFlags.HAS_HEIGHT) == OtbItemFlags.HAS_HEIGHT);
                    item.LookThrough = ((flags & OtbItemFlags.LOOKTHROUGH) == OtbItemFlags.LOOKTHROUGH);
                    item.AllowDistRead = ((flags & OtbItemFlags.ALLOWDISTREAD) == OtbItemFlags.ALLOWDISTREAD);
                    item.IsAnimation = ((flags & OtbItemFlags.ANIMATION) == OtbItemFlags.ANIMATION);
                    item.WalkStack = ((flags & OtbItemFlags.WALKSTACK) == OtbItemFlags.WALKSTACK);

                    while (props.PeekChar() != -1)
                    {
                        byte attribute = props.ReadByte();
                        UInt16 datalen = props.ReadUInt16();

                        switch ((OtbItemAttr)attribute)
                        {
                            case OtbItemAttr.ITEM_ATTR_SERVERID:
                                if (datalen != sizeof(UInt16))
                                    throw new Exception("Unexpected data length of server id block (Should be 2 bytes)");

                                item.Id = props.ReadUInt16();
                                break;

                            case OtbItemAttr.ITEM_ATTR_CLIENTID:
                                if (datalen != sizeof(UInt16))
                                    throw new Exception("Unexpected data length of client id block (Should be 2 bytes)");

                                item.SpriteId = props.ReadUInt16();
                                break;

                            case OtbItemAttr.ITEM_ATTR_WAREID:
                                if (datalen != sizeof(UInt16))
                                    throw new Exception("Unexpected data length of ware id block (Should be 2 bytes)");

                                item.WareId = props.ReadUInt16();
                                break;

                            case OtbItemAttr.ITEM_ATTR_SPEED:
                                if (datalen != sizeof(UInt16))
                                    throw new Exception("Unexpected data length of speed block (Should be 2 bytes)");

                                item.GroundSpeed = props.ReadUInt16();
                                break;

                            case OtbItemAttr.ITEM_ATTR_NAME:
                                item.Name = new string(props.ReadChars(datalen));
                                break;

                            case OtbItemAttr.ITEM_ATTR_SPRITEHASH:
                                if (datalen != 16)
                                    throw new Exception("Unexpected data length of sprite hash (Should be 16 bytes)");

                                item.SpriteHash = props.ReadBytes(16);
                                break;

                            case OtbItemAttr.ITEM_ATTR_MINIMAPCOLOR:
                                if (datalen != 2)
                                    throw new Exception("Unexpected data length of minimap color (Should be 2 bytes)");

                                item.MinimapColor = props.ReadUInt16();
                                break;

                            case OtbItemAttr.ITEM_ATTR_07:
                                //read/write-able
                                if (datalen != 2)
                                    throw new Exception("Unexpected data length of attr 07 (Should be 2 bytes)");

                                item.MaxReadWriteChars = props.ReadUInt16();
                                break;

                            case OtbItemAttr.ITEM_ATTR_08:
                                //readable
                                if (datalen != 2)
                                    throw new Exception("Unexpected data length of attr 08 (Should be 2 bytes)");

                                item.MaxReadChars = props.ReadUInt16();
                                break;

                            case OtbItemAttr.ITEM_ATTR_LIGHT2:
                                if (datalen != sizeof(UInt16) * 2)
                                    throw new Exception("Unexpected data length of item light (2) block");

                                item.LightLevel = props.ReadUInt16();
                                item.LightColor = props.ReadUInt16();
                                break;

                            case OtbItemAttr.ITEM_ATTR_TOPORDER:
                                if (datalen != sizeof(byte))
                                    throw new Exception("Unexpected data length of item toporder block (Should be 1 byte)");

                                item.AlwaysOnTopOrder = props.ReadByte();
                                break;

                            default:
                                //skip unknown attributes
                                props.ReadBytes(datalen);
                                break;
                        }
                    }

                    AddItem(item);
                    node = node.Next;
                }

            }
        }

        public void LoadXml(string fileName)
        {
            if (!File.Exists(fileName))
                throw new Exception(string.Format("File not found {0}.", fileName));

            try
            {
                var xml = XElement.Load(fileName);

                foreach (var item in xml.Elements("item"))
                {
                    if (item.Attribute("id") != null)
                    {
                        var id = item.Attribute("id").GetUInt16();
                        if (id != 0)
                            LoadItem(id, item);
                        else
                        {
                            string ids = item.Attribute("id").GetString();
                            if (ids != string.Empty)
                            {
                                string[] splitIds = ids.Split(new char[] { '-' });
                                for (ushort i = Convert.ToUInt16(splitIds[0]); i <= Convert.ToUInt16(splitIds[1]); i++)
                                    LoadItem(i, item);
                            }
                        }
                    }
                    else if (item.Attribute("fromid") != null && item.Attribute("toid") != null)
                    {
                        var fromid = item.Attribute("fromid").GetUInt16();
                        var toid = item.Attribute("toid").GetUInt16();

                        for (ushort i = fromid; i <= toid; i++)
                            LoadItem(i, item);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Can not load items.xml", e);
            }
        }

        private void LoadItem(ushort id, XElement item)
        {
            if (id > 20000 && id < 20100)
            {
                id = (ushort)(id - 20000);
                AddItem(new OtItemType { Id = id });
            }

            var itemType = GetItem(id);

            if (itemType == null)
            {
                Trace.WriteLine("[Warning] Item " + id + " in item.xml not found in items.otb");
                return;
            }

            itemType.Name = item.Attribute("name").GetString();
            itemType.Article = item.Attribute("article").GetString();
            itemType.Plural = item.Attribute("plural").GetString();

            LoadAttributes(itemType, item);
        }

        private void LoadAttributes(OtItemType itemType, XElement element)
        {
            foreach (var property in element.Elements("attribute"))
            {
                switch (property.Attribute("key").GetString())
                {
                    case "description":
                        itemType.Description = property.Attribute("value").GetString();
                        break;
                    case "type":

                        switch (property.Attribute("value").GetString())
                        {
                            case "container":
                                itemType.Group = OtItemGroup.Container;
                                break;
                            case "key":
                                itemType.Group = OtItemGroup.Key;
                                break;
                            case "magicfield":
                                itemType.Group = OtItemGroup.MagicField;
                                break;
                            case "depot":
                                itemType.Group = OtItemGroup.Depot;
                                break;
                            case "mailbox":
                                itemType.Group = OtItemGroup.MailBox;
                                break;
                            case "trashholder":
                                itemType.Group = OtItemGroup.TrashHolder;
                                break;
                            case "teleport":
                                itemType.Group = OtItemGroup.Teleport;
                                break;
                            case "door":
                                itemType.Group = OtItemGroup.Door;
                                break;
                            case "bed":
                                itemType.Group = OtItemGroup.Bed;
                                break;
                            case "rune":
                                itemType.Group = OtItemGroup.Rune;
                                break;
                        }

                        break;
                }
            }
        }
    }
}
