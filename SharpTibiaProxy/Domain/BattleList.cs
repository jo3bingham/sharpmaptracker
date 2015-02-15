using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public class CreatureEventArgs : EventArgs
    {
        public Creature Creature { get; set; }
    }

    public class CreatureAddedEventArgs : CreatureEventArgs { }
    public class CreatureRemovedEventArgs : CreatureEventArgs { }

    public class BattleList
    {
        private Client client;
        private Dictionary<uint, Creature> creatures;

        public event EventHandler<CreatureAddedEventArgs> CreatureAdded;
        public event EventHandler<CreatureRemovedEventArgs> CreatureRemoved;

        public BattleList(Client client)
        {
            this.client = client;
            this.creatures = new Dictionary<uint, Creature>();
        }

        public Creature GetCreature(uint id)
        {
            if (creatures.ContainsKey(id))
                return creatures[id];

            return null;
        }

        public Creature GetCreature(string name)
        {
            return creatures.FirstOrDefault(x => x.Value.Name.Equals(name)).Value;
        }

        public Creature GetPlayer()
        {
            if (creatures.ContainsKey(client.PlayerId))
                return creatures[client.PlayerId];

            return null;
        }

        public void AddCreature(Creature creature)
        {
            creatures[creature.Id] = creature;

            CreatureAdded.Raise(this, new CreatureAddedEventArgs { Creature = creature });
        }

        public void RemoveCreature(uint id)
        {
            if (creatures.ContainsKey(id))
            {
                var creature = creatures[id];
                creatures.Remove(id);

                CreatureRemoved.Raise(this, new CreatureRemovedEventArgs { Creature = creature });
            }
        }

        internal void Clear()
        {

        }

        public bool ContainsCreature(uint id)
        {
            return creatures.ContainsKey(id);
        }
    }
}
