using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Diagnostics;

namespace SharpMapTracker
{
    public class NpcWordList
    {
        public static HashSet<string> Words { get; private set; }

        static NpcWordList()
        {
            Words = new HashSet<string>();
        }

        public static void AddWord(string word)
        {
            if (word == null)
                return;

            word = word.ToLower().Trim();
            if (word.Equals("bye"))
                return;

            if (word.EndsWith("'s"))
                word = word.Substring(0, word.Length - 2);
            else if(word.EndsWith("'"))
                word = word.Substring(0, word.Length - 1);

            if (word.EndsWith("s"))
                Words.Add(word.Substring(0, word.Length - 1));

            Words.Add(word);
        }

        public static void Load()
        {
            try
            {
                Words.Clear();
                var words = XElement.Load("npc-words.xml");

                foreach (var word in words.Elements("word"))
                    AddWord(word.Value);
            }
            catch (Exception)
            {
                Trace.WriteLine("Can't load npc words file.");
                foreach (var word in Constants.DEFAULT_NPC_WORDS)
                    AddWord(word);
            }
        }

        public static void Save()
        {
            try
            {
                XElement words = new XElement("words");

                foreach (var word in Words)
                    words.Add(new XElement("word", word));

                words.Save("npc-words.xml");
            }
            catch (Exception)
            {
                Trace.WriteLine("Can't save npc words file.");
            }

        }
    }
}
