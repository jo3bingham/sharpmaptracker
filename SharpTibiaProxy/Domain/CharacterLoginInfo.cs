using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public struct CharacterLoginInfo
    {
        public string CharName { get; set; }
        public string WorldName { get; set; }
        public uint WorldIP { get; set; }
        public string WorldIPString { get; set; }
        public ushort WorldPort { get; set; }
    }

    public struct WorldLoginInfo
    {
        public byte ID { get; set; }
        public string Name { get; set; }
        public string Hostname { get; set; }
        public ushort Port { get; set; }
        public bool IsPreviewWorld { get; set; }
    }
}
