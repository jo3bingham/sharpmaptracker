using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public enum ItemGroup
    {
        None,
        Ground,
        Container,
        FluidContainer,
        Splash,
        Deprecated
    };

    public class ItemType
    {
        public ushort Id { get; set; }
        public ushort GroundSpeed { get; set; }
        public ItemGroup Group { get; set; }
        public bool AlwaysOnTop { get; set; }
        public ushort AlwaysOnTopOrder { get; set; }
        public bool HasUseWith { get; set; }
        public ushort MaxReadChars { get; set; }
        public ushort MaxReadWriteChars { get; set; }
        public bool HasHeight { get; set; }
        public ushort MinimapColor { get; set; }
        public bool LookThrough { get; set; }
        public ushort LightLevel { get; set; }
        public ushort LightColor { get; set; }
        public bool IsStackable { get; set; }
        public bool IsReadable { get; set; }
        public bool IsMoveable { get; set; }
        public bool IsNoMovementAnimation { get; set; }
        public bool IsPickupable { get; set; }
        public bool IsHangable { get; set; }
        public bool IsHorizontal { get; set; }
        public bool IsVertical { get; set; }
        public bool IsRotatable { get; set; }
        public bool BlockObject { get; set; }
        public bool BlockProjectile { get; set; }
        public bool BlockPathFind { get; set; }
        public bool AllowDistRead { get; set; }
        public bool IsAnimation { get; set; }
        public bool WalkStack { get; set; }
        public ushort WareId { get; set; }
        public string Name { get; set; }

        public bool IsGround { get { return Group == ItemGroup.Ground; } }
        public bool IsContainer { get { return Group == ItemGroup.Container; } }
        public bool IsFluidContainer { get { return Group == ItemGroup.FluidContainer; } }
        public bool IsSplash { get { return Group == ItemGroup.Splash; } }
    }
}
