using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTibiaCommons.IO;

namespace OpenTibiaCommons.Domain
{
    public class OtDepot : OtItem
    {
        public OtDepot(OtItemType type) : base(type) { }

        public override void DeserializeAttribute(OtItemAttribute attribute, OtPropertyReader reader)
        {
            if (attribute == OtItemAttribute.DEPOT_ID)
                SetAttribute(OtItemAttribute.DEPOT_ID, reader.ReadUInt16());
            else
                base.DeserializeAttribute(attribute, reader);
        }

        public override void SerializeAttribute(OtItemAttribute attribute, OtPropertyWriter writer)
        {
            if (attribute == OtItemAttribute.DEPOT_ID)
                writer.Write((ushort)GetAttribute(attribute));
            else
                base.SerializeAttribute(attribute, writer);
        }

    }
}
