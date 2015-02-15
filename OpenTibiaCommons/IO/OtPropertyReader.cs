using System.IO;
using System.Text;
using SharpTibiaProxy.Domain;

namespace OpenTibiaCommons.IO
{
    public class OtPropertyReader : BinaryReader
    {
        public OtPropertyReader(Stream stream)
            : base(stream) { }

        public string GetString()
        {
            var len = ReadUInt16();
            return Encoding.Default.GetString(ReadBytes(len));
        }

        public Location ReadLocation()
        {
            return new Location(ReadUInt16(), ReadUInt16(), ReadByte());
        }
    }
}