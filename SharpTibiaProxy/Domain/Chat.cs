using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SharpTibiaProxy.Domain
{
    public class CreatureSpeakEventArgs : EventArgs
    {
        public uint Id { get; set; }
        public Creature Creature { get; set; }
        public string Text { get; set; }
        public MessageClasses Type { get; set; }
        public Location Location { get; set; }
    }

    public class PlayerSpeakEventArgs : EventArgs
    {
        public MessageClasses Type { get; set; }
        public string Receiver { get; set; }
        public ushort ChannelId { get; set; }
        public string Text { get; set; }
    }

    public class Chat
    {
        private Client client;

        public event EventHandler<CreatureSpeakEventArgs> CreatureSpeak;
        public event EventHandler<PlayerSpeakEventArgs> PlayerSpeak;

        public Chat(Client client)
        {
            this.client = client;
        }

        public void Say(string text)
        {
            if (text.Length > 255)
                throw new Exception("Value can't have more then 255 characters.");

            client.ProtocolWorld.SendServerSay(text, MessageClasses.SPEAK_SAY);
        }

        public void SayToNpc(string text)
        {
            if (text.Length > 255)
                throw new Exception("Value can't have more then 255 characters.");

            client.ProtocolWorld.SendServerSay(text, MessageClasses.NPC_TO);
        }

        internal void OnCreatureSpeak(uint statementId, string name, ushort level, MessageClasses type, Location location, string text)
        {
            var creature = client.BattleList.GetCreature(name);

            if (creature == null)
                return;

            creature.Level = level;
            CreatureSpeak.Raise(this, new CreatureSpeakEventArgs { Id = statementId, Creature = creature, Text = text, Type = type, Location = location });
        }

        internal void OnPlayerSpeak(string receiver, ushort channelId, MessageClasses type, string text)
        {
            PlayerSpeak.Raise(this, new PlayerSpeakEventArgs { Receiver = receiver, ChannelId = channelId, Type = type, Text = text });
        }
    }
}
