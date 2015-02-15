using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTibiaCommons.IO;
using SharpTibiaProxy.Domain;

namespace OpenTibiaCommons.Domain
{
    public class OtTeleport : OtItem
    {
        public OtTeleport(OtItemType type) : base(type) { }

        public override void DeserializeAttribute(OtItemAttribute attribute, OtPropertyReader reader)
        {
            if (attribute == OtItemAttribute.TELE_DEST)
                SetAttribute(OtItemAttribute.TELE_DEST, reader.ReadLocation());
            else
                base.DeserializeAttribute(attribute, reader);
        }

        public override void SerializeAttribute(OtItemAttribute attribute, OtPropertyWriter writer)
        {
            if (attribute == OtItemAttribute.TELE_DEST)
                writer.Write((Location)GetAttribute(attribute));
            else
                base.SerializeAttribute(attribute, writer);
        }
    }
}
