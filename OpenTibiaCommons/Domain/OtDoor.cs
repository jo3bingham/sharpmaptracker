using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTibiaCommons.IO;

namespace OpenTibiaCommons.Domain
{
    public class OtDoor : OtItem
    {
        public OtDoor(OtItemType type) : base(type) { }

        public override void DeserializeAttribute(OtItemAttribute attribute, OtPropertyReader reader)
        {
            if (attribute == OtItemAttribute.HOUSEDOORID)
                SetAttribute(OtItemAttribute.HOUSEDOORID, reader.ReadByte());
            else
                base.DeserializeAttribute(attribute, reader);
        }

        public override void SerializeAttribute(OtItemAttribute attribute, OtPropertyWriter writer)
        {
            if (attribute == OtItemAttribute.HOUSEDOORID)
                writer.Write((byte)GetAttribute(attribute));
            else
                base.SerializeAttribute(attribute, writer);
        }

    }
}
