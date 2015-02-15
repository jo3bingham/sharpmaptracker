using SharpTibiaProxy.Domain;
using System.Collections.Generic;
using OpenTibiaCommons.IO;
using System;
namespace OpenTibiaCommons.Domain
{
    public enum OtItemAttribute
    {
        NONE = 0,
        //DESCRIPTION = 1,
        //EXT_FILE = 2,
        TILE_FLAGS = 3,
        ACTION_ID = 4,
        UNIQUE_ID = 5,
        TEXT = 6,
        DESC = 7,
        TELE_DEST = 8,
        ITEM = 9,
        DEPOT_ID = 10,
        //EXT_SPAWN_FILE = 11,
        RUNE_CHARGES = 12,
        //EXT_HOUSE_FILE = 13,
        HOUSEDOORID = 14,
        COUNT = 15,
        DURATION = 16,
        DECAYING_STATE = 17,
        WRITTENDATE = 18,
        WRITTENBY = 19,
        SLEEPERGUID = 20,
        SLEEPSTART = 21,
        CHARGES = 22,
        CONTAINER_ITEMS = 23,
        NAME = 30,
        PLURALNAME = 31,
        ATTACK = 33,
        EXTRAATTACK = 34,
        DEFENSE = 35,
        EXTRADEFENSE = 36,
        ARMOR = 37,
        ATTACKSPEED = 38,
        HITCHANCE = 39,
        SHOOTRANGE = 40,
        ARTICLE = 41,
        SCRIPTPROTECTED = 42,
        DUALWIELD = 43,
        ATTRIBUTE_MAP = 128
    };

    public class OtItem
    {
        public OtItemType Type { get; private set; }
        private Dictionary<OtItemAttribute, object> attributes;

        protected OtItem(OtItemType type)
        {
            Type = type;
        }

        public static OtItem Create(OtItemType type)
        {
            OtItem item = null;

            if (type.Group == OtItemGroup.Depot)
                item = new OtDepot(type);
            else if (type.Group == OtItemGroup.Container)
                item = new OtContainer(type);
            else if (type.Group == OtItemGroup.Teleport)
                item = new OtTeleport(type);
            else if (type.Group == OtItemGroup.MagicField)
                item = new OtMagicField(type);
            else if (type.Group == OtItemGroup.Door)
                item = new OtDoor(type);
            else if (type.Group == OtItemGroup.TrashHolder)
                item = new OtTrashHolder(type);
            else if (type.Group == OtItemGroup.MailBox)
                item = new OtMailBox(type);
            else
                item = new OtItem(type);

            return item;
        }

        public void SetAttribute(OtItemAttribute attribute, object value)
        {
            if (attributes == null)
                attributes = new Dictionary<OtItemAttribute, object>();
            attributes[attribute] = value;
        }

        public object GetAttribute(OtItemAttribute attribute)
        {
            if (attributes == null || !attributes.ContainsKey(attribute))
                return null;

            return attributes[attribute];
        }

        public virtual void SerializeAttribute(OtItemAttribute attribute, OtPropertyWriter writer)
        {
            switch (attribute)
            {
                case OtItemAttribute.COUNT:
                    writer.Write((byte)GetAttribute(attribute));
                    break;
                case OtItemAttribute.ACTION_ID:
                    writer.Write((ushort)GetAttribute(attribute));
                    break;
                case OtItemAttribute.UNIQUE_ID:
                    writer.Write((ushort)GetAttribute(attribute));
                    break;
                case OtItemAttribute.NAME:
                    writer.Write((string)GetAttribute(attribute));
                    break;
                case OtItemAttribute.PLURALNAME:
                    writer.Write((string)GetAttribute(attribute));
                    break;
                case OtItemAttribute.ARTICLE:
                    writer.Write((string)GetAttribute(attribute));
                    break;
                case OtItemAttribute.ATTACK:
                    writer.Write((int)GetAttribute(attribute));
                    break;
                case OtItemAttribute.EXTRAATTACK:
                    writer.Write((int)GetAttribute(attribute));
                    break;
                case OtItemAttribute.DEFENSE:
                    writer.Write((int)GetAttribute(attribute));
                    break;
                case OtItemAttribute.EXTRADEFENSE:
                    writer.Write((int)GetAttribute(attribute));
                    break;
                case OtItemAttribute.ARMOR:
                    writer.Write((int)GetAttribute(attribute));
                    break;
                case OtItemAttribute.ATTACKSPEED:
                    writer.Write((int)GetAttribute(attribute));
                    break;
                case OtItemAttribute.HITCHANCE:
                    writer.Write((int)GetAttribute(attribute));
                    break;
                case OtItemAttribute.SCRIPTPROTECTED:
                    writer.Write((byte)((bool)GetAttribute(attribute) ? 1 : 0));
                    break;
                case OtItemAttribute.DUALWIELD:
                    writer.Write((byte)((bool)GetAttribute(attribute) ? 1 : 0));
                    break;
                case OtItemAttribute.TEXT:
                    writer.Write((string)GetAttribute(attribute));
                    break;
                case OtItemAttribute.WRITTENDATE:
                    writer.Write((int)GetAttribute(attribute));
                    break;
                case OtItemAttribute.WRITTENBY:
                    writer.Write((string)GetAttribute(attribute));
                    break;
                case OtItemAttribute.DESC:
                    writer.Write((string)GetAttribute(attribute));
                    break;
                case OtItemAttribute.RUNE_CHARGES:
                    writer.Write((byte)GetAttribute(attribute));
                    break;
                case OtItemAttribute.CHARGES:
                    writer.Write((ushort)GetAttribute(attribute));
                    break;
                case OtItemAttribute.DURATION:
                    writer.Write((int)GetAttribute(attribute));
                    break;
                case OtItemAttribute.DECAYING_STATE:
                    writer.Write((byte)GetAttribute(attribute));
                    break;
                default:
                    throw new Exception("Unkonw item attribute: " + attribute);
            }


        }

        public virtual void Serialize(OtPropertyWriter writer)
        {
            if (attributes == null)
                return;

            foreach (var attr in attributes)
            {
                writer.Write((byte)attr.Key);
                SerializeAttribute(attr.Key, writer);
            }
        }

        public virtual void Serialize(OtFileWriter fileWriter, OtPropertyWriter writer)
        {
            Serialize(writer);
        }

        public virtual void DeserializeAttribute(OtItemAttribute attribute, OtPropertyReader reader)
        {
            switch (attribute)
            {
                case OtItemAttribute.COUNT:
                    SetAttribute(OtItemAttribute.COUNT, reader.ReadByte());
                    break;
                case OtItemAttribute.ACTION_ID:
                    SetAttribute(OtItemAttribute.ACTION_ID, reader.ReadUInt16());
                    break;
                case OtItemAttribute.UNIQUE_ID:
                    SetAttribute(OtItemAttribute.UNIQUE_ID, reader.ReadUInt16());
                    break;
                case OtItemAttribute.NAME:
                    SetAttribute(OtItemAttribute.NAME, reader.ReadString());
                    break;
                case OtItemAttribute.PLURALNAME:
                    SetAttribute(OtItemAttribute.PLURALNAME, reader.ReadString());
                    break;
                case OtItemAttribute.ARTICLE:
                    SetAttribute(OtItemAttribute.ARTICLE, reader.ReadString());
                    break;
                case OtItemAttribute.ATTACK:
                    SetAttribute(OtItemAttribute.ATTACK, reader.ReadInt32());
                    break;
                case OtItemAttribute.EXTRAATTACK:
                    SetAttribute(OtItemAttribute.EXTRAATTACK, reader.ReadInt32());
                    break;
                case OtItemAttribute.DEFENSE:
                    SetAttribute(OtItemAttribute.DEFENSE, reader.ReadInt32());
                    break;
                case OtItemAttribute.EXTRADEFENSE:
                    SetAttribute(OtItemAttribute.EXTRADEFENSE, reader.ReadInt32());
                    break;
                case OtItemAttribute.ARMOR:
                    SetAttribute(OtItemAttribute.ARMOR, reader.ReadInt32());
                    break;
                case OtItemAttribute.ATTACKSPEED:
                    SetAttribute(OtItemAttribute.ATTACKSPEED, reader.ReadInt32());
                    break;
                case OtItemAttribute.HITCHANCE:
                    SetAttribute(OtItemAttribute.HITCHANCE, reader.ReadInt32());
                    break;
                case OtItemAttribute.SCRIPTPROTECTED:
                    SetAttribute(OtItemAttribute.SCRIPTPROTECTED, reader.ReadByte() != 0);
                    break;
                case OtItemAttribute.DUALWIELD:
                    SetAttribute(OtItemAttribute.DUALWIELD, reader.ReadByte() != 0);
                    break;
                case OtItemAttribute.TEXT:
                    SetAttribute(OtItemAttribute.TEXT, reader.ReadString());
                    break;
                case OtItemAttribute.WRITTENDATE:
                    SetAttribute(OtItemAttribute.WRITTENDATE, reader.ReadInt32());
                    break;
                case OtItemAttribute.WRITTENBY:
                    SetAttribute(OtItemAttribute.WRITTENBY, reader.ReadString());
                    break;
                case OtItemAttribute.DESC:
                    SetAttribute(OtItemAttribute.DESC, reader.ReadString());
                    break;
                case OtItemAttribute.RUNE_CHARGES:
                    SetAttribute(OtItemAttribute.RUNE_CHARGES, reader.ReadByte());
                    break;
                case OtItemAttribute.CHARGES:
                    SetAttribute(OtItemAttribute.CHARGES, reader.ReadUInt16());
                    break;
                case OtItemAttribute.DURATION:
                    SetAttribute(OtItemAttribute.DURATION, reader.ReadInt32());
                    break;
                case OtItemAttribute.DECAYING_STATE:
                    SetAttribute(OtItemAttribute.DECAYING_STATE, reader.ReadByte());
                    break;

                //specific item properties
                //case OtItemAttribute.DEPOT_ID:
                //    SetAttribute(OtItemAttribute.DEPOT_ID, reader.ReadUInt16());
                //    break;
                //case OtItemAttribute.HOUSEDOORID:
                //    SetAttribute(OtItemAttribute.HOUSEDOORID, reader.ReadByte());
                //    break;
                //case OtItemAttribute.TELE_DEST:
                //    SetAttribute(OtItemAttribute.TELE_DEST, reader.ReadLocation());
                //    break;
                //case OtItemAttribute.SLEEPERGUID:
                //    SetAttribute(OtItemAttribute.SLEEPERGUID, reader.ReadUInt32());
                //    break;
                //case OtItemAttribute.SLEEPSTART:
                //    SetAttribute(OtItemAttribute.SLEEPSTART, reader.ReadUInt32());
                //    break;
                default:
                    throw new Exception("Unkonw item attribute: " + (byte)attribute);
            }
        }

        public virtual void Deserialize(OtPropertyReader reader)
        {
            while (reader.PeekChar() != -1)
            {
                var attrType = (OtItemAttribute)reader.ReadByte();
                DeserializeAttribute(attrType, reader);
            }
        }

        public virtual void Deserialize(OtFileReader fileReader, OtFileNode itemNode, OtPropertyReader reader, OtItems items)
        {
            Deserialize(reader);
        }

    }
}
