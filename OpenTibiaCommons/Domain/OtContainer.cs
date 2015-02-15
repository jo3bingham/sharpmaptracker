using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTibiaCommons.IO;

namespace OpenTibiaCommons.Domain
{
    public class OtContainer : OtItem
    {
        private List<OtItem> items;

        public OtContainer(OtItemType type) : base(type) { }

        public override void DeserializeAttribute(OtItemAttribute attribute, OtPropertyReader reader)
        {
            if (attribute == OtItemAttribute.CONTAINER_ITEMS)
                SetAttribute(OtItemAttribute.CONTAINER_ITEMS, reader.ReadUInt32());
            else
                base.DeserializeAttribute(attribute, reader);
        }

        public override void Deserialize(OtFileReader fileReader, OtFileNode node, OtPropertyReader reader, OtItems items)
        {
            base.Deserialize(fileReader, node, reader, items);

            var itemNode = node.Child;

            while (itemNode != null)
            {
                if ((OtMap.OtMapNodeTypes)itemNode.Type != OtMap.OtMapNodeTypes.ITEM)
                    throw new Exception("Invalid item node inside container.");

                OtPropertyReader props = fileReader.GetPropertyReader(itemNode);

                var itemId = props.ReadUInt16();
                var itemType = items.GetItem(itemId);
                if (itemType == null)
                    throw new Exception("Unkonw item type " + itemId + " inside container.");

                var item = OtItem.Create(itemType);
                item.Deserialize(fileReader, itemNode, props, items);

                Items.Add(item);

                itemNode = itemNode.Next;
            }
        }

        public override void SerializeAttribute(OtItemAttribute attribute, OtPropertyWriter writer)
        {
            if (attribute == OtItemAttribute.CONTAINER_ITEMS)
                writer.Write((uint)(items != null ? items.Count : 0));
            else
                base.SerializeAttribute(attribute, writer);
        }

        public override void Serialize(OtFileWriter fileWriter, OtPropertyWriter writer)
        {
            base.Serialize(fileWriter, writer);

            if (items != null)
            {
                foreach (var item in Items)
                {
                    fileWriter.WriteNodeStart((byte)OtMap.OtMapNodeTypes.ITEM);

                    fileWriter.Write(item.Type.Id);
                    item.Serialize(fileWriter, fileWriter.GetPropertyWriter());

                    fileWriter.WriteNodeEnd();
                }
            }
        }

        public List<OtItem> Items
        {
            get
            {
                if (items == null)
                    items = new List<OtItem>();
                return items;
            }
        }

    }
}
