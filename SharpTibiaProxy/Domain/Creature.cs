using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public enum CreatureType : byte
    {
        PLAYER = 0,
        MONSTER = 1,
        NPC = 2
    }

    public class Creature : Thing
    {
        private uint id;

        public override uint Id { get { return id; } }
        public override bool IsAlwaysOnTop { get { return false; } }
        public CreatureType Type { get; set; }
        public string Name { get; set; }

        public Location Location { get; set; }

        /// <summary>
        /// Level from chat
        /// </summary>
        public ushort Level { get; set; }

        public override int Order
        {
            get { return 4; }
        }

        public override bool IsBlockingPath
        {
            get { return !IsImpassable; }
        }

        public byte Health { get; set; }

        public Direction LookDirection { get; set; }

        public Direction TurnDirection { get; set; }

        public byte LightLevel { get; set; }

        public byte LightColor { get; set; }

        public ushort Speed { get; set; }

        public byte Skull { get; set; }

        public byte Shield { get; set; }

        public byte Emblem { get; set; }

        public bool IsImpassable { get; set; }

        public Outfit Outfit { get; set; }

        public Creature(uint id)
        {
            this.id = id;
        }

        public override int GetHashCode()
        {
            return (int)Id ^ 31;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Creature;
            return other != null && other.Id == Id;
        }
    }
}
