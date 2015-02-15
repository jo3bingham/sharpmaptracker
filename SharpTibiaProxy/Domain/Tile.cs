using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SharpTibiaProxy.Domain
{
    public class Tile
    {
#if DEBUG_TILE
        private static readonly Location DEBUG_LOCATION = new Location(32806, 32343, 12);
        private bool debugEnable;
#endif

        public Location Location { get; private set; }
        public int ThingCount { get { return things.Count; } }

        public List<Thing> things { get; private set; }

        public Tile(Location location)
        {
            Location = location;
            things = new List<Thing>();

#if DEBUG_TILE
            if (location.Equals(DEBUG_LOCATION))
                debugEnable = true;
#endif
        }

        public Tile(Tile tile)
        {
            Location = tile.Location;
            things = new List<Thing>(tile.things);
        }

        public bool IsBlocked { get { return things.Any(x => x.IsBlockingPath); } }

        public void AddThing(Thing thing)
        {
            AddThing(0xFF, thing);
        }

        public void AddThing(int index, Thing thing)
        {
#if DEBUG_TILE
            if (debugEnable)
                Trace.WriteLine("[AddThing] Id: " + thing.Id + ", Index: " + index);
#endif
            if (thing == null)
                throw new Exception("[AddThing] Null thing.");

            if (index != 0xFF)
            {
                if (index == 0)
                {
                    var item = thing as Item;
                    if (item == null || !item.IsGround)
                        throw new Exception("[AddThing] Invalid ground item.");
                }

                if (things.Count < index)
                {
                    things.Add(thing);
                }
                else
                {
                    things.Insert(index, thing);
                }
            }
            else
            {
                if (thing is Creature)
                {
                    int pos = things.Count;
                    for (int i = 0; i < things.Count; i++)
                    {
                        if (things[i].Order > thing.Order)
                        {
                            pos = i;
                            break;
                        }
                    }

                    things.Insert(pos, thing);
                    ((Creature)thing).Location = Location;
                }
                else
                {
                    int pos = things.Count;
                    for (int i = 0; i < things.Count; i++)
                    {
                        if (things[i].Order >= thing.Order)
                        {
                            pos = i;
                            break;
                        }
                    }

                    things.Insert(pos, thing);
                }
            }

            if (things.Count > 10)
                RemoveThing(10);

        }

        public void RemoveThing(int index)
        {
#if DEBUG_TILE
            if (debugEnable)
                Trace.WriteLine("[RemoveThing] Index: " + index);
#endif

            if (index < 0 || index >= ThingCount)
                throw new Exception("[RemoveThing] Invalid stack value: " + index);

            things.RemoveAt(index);
        }

        public Thing GetThing(int index)
        {
#if DEBUG_TILE
            if (debugEnable)
                Trace.WriteLine("[GetThing] Index: " + index);
#endif

            if (index < 0 || index >= ThingCount)
                return null;

            return things[index];
        }

        public void ReplaceThing(int index, Thing thing)
        {
#if DEBUG_TILE
            if (debugEnable)
                Trace.WriteLine("[ReplaceThing] Id: " + thing.Id + ", Index: " + index);
#endif

            if (index < 0 || index >= ThingCount)
                throw new Exception("[ReplaceThing] Invalid stack value: " + index);

            if (index == 0)
            {
                var item = thing as Item;
                if (item == null || !item.IsGround)
                    throw new Exception("[ReplaceThing] Invalid ground item.");
            }

            things[index] = thing;

            if(thing is Creature)
                ((Creature)thing).Location = Location;
        }

        public void Clear()
        {
#if DEBUG_TILE
            if (debugEnable)
                Trace.WriteLine("[Clear]");
#endif
            things.Clear();
        }
    }
}
