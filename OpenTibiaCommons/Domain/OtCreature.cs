using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpTibiaProxy.Domain;

namespace OpenTibiaCommons.Domain
{
    public class OtCreature
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public CreatureType Type { get; set; }
        public Location Location { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as OtCreature;
            return other != null && other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
