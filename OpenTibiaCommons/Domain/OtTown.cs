using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpTibiaProxy.Domain;

namespace OpenTibiaCommons.Domain
{
    public class OtTown
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public Location TempleLocation { get; set; }
    }
}
