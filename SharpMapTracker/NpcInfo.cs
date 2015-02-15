using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpTibiaProxy.Domain;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.IO;
using OpenTibiaCommons.Domain;
using OpenTibiaCommons;

namespace SharpMapTracker
{
    public class NpcVoice
    {
        public string Text { get; set; }
        public bool IsYell { get; set; }
        public int Interval { get; set; }
        public DateTime LastTime { get; set; }

        public NpcVoice()
        {
            Interval = 120;
            LastTime = DateTime.Now;
        }

        public override bool Equals(object obj)
        {
            var other = obj as NpcVoice;
            return other != null && other.Text == Text;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }
    }

    public class NpcInfo
    {
        private string name;
        private Outfit outfit;
        private Shop shop;
        private Dictionary<string, string> statements;
        private HashSet<NpcVoice> voices;
        private HashSet<string> triedWords;
        private OtItems items;

        public NpcInfo(Creature creature, OtItems items)
        {
            this.name = creature.Name;
            this.outfit = creature.Outfit;

            this.statements = new Dictionary<string, string>();
            this.triedWords = new HashSet<string>();
            this.voices = new HashSet<NpcVoice>();

            NpcWordList.Words.Add(creature.Name.ToLower());
        }

        public void AddVoice(string value, bool yell)
        {
            var voice = voices.FirstOrDefault(x => x.Text == value);
            if (voice != null)
            {
                voice.Interval = (int)(DateTime.Now - voice.LastTime).TotalSeconds;
                voice.LastTime = DateTime.Now;
            }
            else
                voices.Add(new NpcVoice { Text = value, IsYell = yell });
        }

        public void AddStatement(string key, string value)
        {
            key = key.ToLower().Trim().Replace("'", "\\'");
            value = value.Replace("'", "\\'");

            NpcWordList.AddWord(key);
            if (!statements.ContainsKey(key))
            {
                if (key.EndsWith("s"))
                {
                    var otherKey = key.Substring(0, key.Length - 1);
                    if (statements.ContainsKey(otherKey) && statements[otherKey] == value)
                        return;
                }
                else
                {
                    var otherKey = key + "s";
                    if (statements.ContainsKey(otherKey) && statements[otherKey] == value)
                    {
                        statements.Remove(otherKey);
                        statements[key] = value;
                        return;
                    }
                }

                statements[key] = value;

                var matches = Regex.Matches(value, @"{([\w'\s]+)}");

                foreach (Match match in matches)
                    NpcWordList.AddWord(match.Groups[1].Value);
            }
        }

        public string Name { get { return name; } }
        public Outfit Outfit { get { return outfit; } }
        public Shop Shop { get { return shop; } set { this.shop = value; } }
        public Dictionary<string, string> Statements { get { return statements; } }
        public HashSet<string> TriedWords { get { return triedWords; } }
        public List<string> NotTriedWords { get { return NpcWordList.Words.Where(x => !triedWords.Contains(x)).ToList(); } }
        public HashSet<NpcVoice> Voices { get { return voices; } }

        public void Save(string directory)
        {
            SaveXml(Path.Combine(directory, Name + ".xml"));
            SaveScript(Path.Combine(directory, "scripts", Name + ".lua"));
        }

        public void SaveXml(string fileName)
        {
            var directory = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var npc = new XElement("npc");
            npc.Add(new XAttribute("name", Name));
            npc.Add(new XAttribute("script", "data/npc/scripts/" + Name + ".lua"));
            npc.Add(new XAttribute("walkinterval", "2000"));
            npc.Add(new XAttribute("floorchange", "0"));

            npc.Add(new XElement("health", new XAttribute("now", "100"), new XAttribute("max", "100")));
            npc.Add(new XElement("look", new XAttribute("type", Outfit.LookType),
                new XAttribute("head", Outfit.Head), new XAttribute("body", Outfit.Body),
                new XAttribute("legs", Outfit.Legs), new XAttribute("feet", Outfit.Feet),
                new XAttribute("addons", Outfit.Addons)));

            if (Voices.Count > 0)
            {
                var voices = new XElement("voices");
                foreach (var voice in Voices)
                {
                    voices.Add(new XElement("voice", new XAttribute("text", voice.Text), new XAttribute("interval2", voice.Interval),
                        new XAttribute("margin", "1"), new XAttribute("yell", voice.IsYell ? "yes" : "no")));
                }

                npc.Add(voices);
            }

            npc.Save(fileName);
        }

        public void SaveScript(string fileName)
        {
            var builder = new StringBuilder();

            builder.Append("local keywordHandler = KeywordHandler:new()\n");
            builder.Append("local npcHandler = NpcHandler:new(keywordHandler)\n");
            builder.Append("NpcSystem.parseParameters(npcHandler)\n");
            builder.Append("\n");
            builder.Append("function onCreatureAppear(cid)			npcHandler:onCreatureAppear(cid)			end\n");
            builder.Append("function onCreatureDisappear(cid)		npcHandler:onCreatureDisappear(cid)			end\n");
            builder.Append("function onCreatureSay(cid, type, msg)	npcHandler:onCreatureSay(cid, type, msg)	end\n");
            builder.Append("function onThink()						npcHandler:onThink()						end\n");
            builder.Append("\n");

            if (Statements.ContainsKey("hi"))
                builder.Append("npcHandler:setMessage(MESSAGE_GREET, '").Append(Statements["hi"]).Append("')\n");
            else if (Statements.ContainsKey("bye"))
                builder.Append("npcHandler:setMessage(MESSAGE_FAREWELL, '").Append(Statements["bye"]).Append("')\n");
            else if (Statements.ContainsKey("trade"))
                builder.Append("npcHandler:setMessage(MESSAGE_SENDTRADE, '").Append("')\n");

            builder.Append("\n");

            foreach (var statement in Statements)
            {
                if (statement.Key.Equals("hi") || statement.Key.Equals("bye") || statement.Key.Equals("trade"))
                    continue;

                builder.Append("keywordHandler:addKeyword({'").Append(statement.Key)
                    .Append("'}, StdModule.say, {npcHandler = npcHandler, onlyFocus = true, text = '")
                    .Append(statement.Value).Append("'})\n");
            }

            if (Shop != null)
            {
                builder.Append("\n");
                builder.Append("local shopModule = ShopModule:new()\n");
                builder.Append("npcHandler:addModule(shopModule)\n");
                builder.Append("\n");

                foreach (var item in Shop.Items.Where(x => x.IsBuyable))
                {
                    var otItem = items.GetItemBySpriteId(item.Id);

                    if (otItem == null)
                        continue;

                    builder.Append("shopModule:addBuyableItem({'").Append(item.Name.ToLower()).
                        Append("'}, ").Append(otItem.Id).Append(", ").Append(item.BuyPrice).Append(", ");

                    if (otItem.Group == OtItemGroup.Splash || otItem.Group == OtItemGroup.FluidContainer)
                        builder.Append(OtConverter.TibiaFluidToOtFluid(item.SubType)).Append(", ");

                    builder.Append('\'').Append(item.Name.ToLower()).Append("')\n");
                }

                builder.Append("\n");

                foreach (var item in Shop.Items.Where(x => x.IsSellable))
                {
                    var otItem = items.GetItemBySpriteId(item.Id);

                    if (otItem == null)
                        continue;

                    builder.Append("shopModule:addSellableItem({'").Append(item.Name.ToLower()).
                        Append("'}, ").Append(otItem.Id).Append(", ").Append(item.SellPrice).Append(", ");

                    if (otItem.Group == OtItemGroup.Splash || otItem.Group == OtItemGroup.FluidContainer)
                        builder.Append(OtConverter.TibiaFluidToOtFluid(item.SubType)).Append(", ");

                    builder.Append('\'').Append(item.Name.ToLower()).Append("')\n");
                }
            }


            builder.Append("\n");
            builder.Append("npcHandler:addModule(FocusModule:new())");

            File.WriteAllText(fileName, builder.ToString());
        }

    }
}
