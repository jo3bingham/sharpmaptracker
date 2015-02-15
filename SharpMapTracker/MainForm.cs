using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpTibiaProxy.Domain;
using System.Diagnostics;
using System.IO;
using SharpTibiaProxy;
using SharpTibiaProxy.Network;
using System.Threading;
using SharpTibiaProxy.Util;
using System.Xml.Linq;
using OpenTibiaCommons.Domain;
using SharpMapTracker.Share;
using OpenTibiaCommons;

namespace SharpMapTracker
{
    public partial class MainForm : Form
    {
        private const int MAP_SCROLL_SPEED = 8;

        private Client client;
        private OtItems otItems;

        private OtMap map;
        private Dictionary<string, NpcInfo> npcs;
        private string lastPlayerSpeech;
        private DateTime lastPlayerSpeechTime;
        private uint sendNpcWordScheduleId;

        private MapShare mapShare;

        public bool TrackMoveableItems { get; set; }
        public bool TrackSplashes { get; set; }
        public bool TrackMonsters { get; set; }
        public bool TrackNPCs { get; set; }
        public bool RetrackTiles { get; set; }
        public bool TrackOnlyCurrentFloor { get; set; }
        public bool NPCAutoTalk { get; set; }
        public bool ShareTrackedMap { get; set; }

        public MainForm()
        {
            InitializeComponent();

            Text = "SharpMapTracker v" + Constants.MAP_TRACKER_VERSION;

            DataBindings.Add("TopMost", alwaysOnTopCheckBox, "Checked");
            DataBindings.Add("TrackMoveableItems", trackMoveableItemsCheckBox, "Checked");
            DataBindings.Add("TrackSplashes", trackSplashesCheckBox, "Checked");
            DataBindings.Add("TrackMonsters", trackMonstersCheckBox, "Checked");
            DataBindings.Add("TrackNPCs", trackNpcsCheckBox, "Checked");
            DataBindings.Add("TrackOnlyCurrentFloor", trackOnlyCurrentFloorCheckBox, "Checked");
            DataBindings.Add("RetrackTiles", retrackTilesToolStripMenuItem, "Checked");
            DataBindings.Add("NPCAutoTalk", npcAutoTalkCheckBox, "Checked");
            DataBindings.Add("ShareTrackedMap", shareTrackedMapCheckBox, "Checked");

            Trace.Listeners.Add(new TextBoxTraceListener(traceTextBox));
            Trace.Listeners.Add(new TextWriterTraceListener("log.txt"));

            Trace.AutoFlush = true;

            KeyDown += new KeyEventHandler(MainForm_KeyDown);
            Load += MainForm_Load;
            FormClosed += new FormClosedEventHandler(MainForm_FormClosed);
            miniMap.MiniMapClick += new EventHandler<MiniMapClickEventArgs>(miniMap_MiniMapClick);
        }

        void miniMap_MiniMapClick(object sender, MiniMapClickEventArgs e)
        {
            if (client != null)
                client.PlayerGoTo(e.Location);
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            LoadItems();
            NpcWordList.Load();

            map = new OtMap(otItems);
            map.Descriptions.Add("Created with SharpMapTracker v" + Constants.MAP_TRACKER_VERSION);

            npcs = new Dictionary<string, NpcInfo>();

            miniMap.Map = map;

            mapShare = new MapShare();
            mapShare.Start();
        }

        void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client = null;
            mapShare.Stop();
            NpcWordList.Save();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageUp || e.KeyValue == 0x6B)
            {
                e.SuppressKeyPress = true;
                miniMap.Floor--;
            }
            else if (e.KeyCode == Keys.PageDown || e.KeyValue == 0x6D)
            {
                e.SuppressKeyPress = true;
                miniMap.Floor++;
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                if (miniMap.CenterLocation != null)
                    miniMap.CenterLocation = new Location(miniMap.CenterLocation.X, miniMap.CenterLocation.Y - MAP_SCROLL_SPEED, miniMap.CenterLocation.Z);
            }
            else if (e.KeyCode == Keys.Down)
            {
                e.SuppressKeyPress = true;
                if (miniMap.CenterLocation != null)
                    miniMap.CenterLocation = new Location(miniMap.CenterLocation.X, miniMap.CenterLocation.Y + MAP_SCROLL_SPEED, miniMap.CenterLocation.Z);
            }
            else if (e.KeyCode == Keys.Left)
            {
                e.SuppressKeyPress = true;
                if (miniMap.CenterLocation != null)
                    miniMap.CenterLocation = new Location(miniMap.CenterLocation.X - MAP_SCROLL_SPEED, miniMap.CenterLocation.Y, miniMap.CenterLocation.Z);
            }
            else if (e.KeyCode == Keys.Right)
            {
                e.SuppressKeyPress = true;
                if (miniMap.CenterLocation != null)
                    miniMap.CenterLocation = new Location(miniMap.CenterLocation.X + MAP_SCROLL_SPEED, miniMap.CenterLocation.Y, miniMap.CenterLocation.Z);
            }

        }

        protected Client Client
        {
            get { return client; }
            set
            {
                if (client != null)
                {
                    client.Map.Updated -= Map_Updated;
                    client.BattleList.CreatureAdded -= BattleList_CreatureAdded;
                    client.Chat.CreatureSpeak -= Chat_CreatureSpeak;
                    client.Chat.PlayerSpeak -= Chat_PlayerSpeak;
                    client.OpenShopWindow -= Client_OpenShopWindow;
                    client.Exited -= client_Exited;

                    client.Dispose();
                }

                client = value;

                if (client != null)
                {
                    client.Map.Updated += Map_Updated;
                    client.BattleList.CreatureAdded += BattleList_CreatureAdded;
                    client.Chat.CreatureSpeak += Chat_CreatureSpeak;
                    client.Chat.PlayerSpeak += Chat_PlayerSpeak;
                    client.OpenShopWindow += Client_OpenShopWindow;
                    client.Exited += client_Exited;
                }

            }
        }

        void client_Exited(object sender, EventArgs e)
        {
            client = null;
            Trace.WriteLine("Client unloaded.");
        }

        private void UpdateCounters(int tileCount, int npcCount, int monsterCount)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int, int, int>(UpdateCounters), tileCount, npcCount, monsterCount);
                return;
            }

            tileCountLabel.Text = "Tiles: " + tileCount.ToString();
            npcCountLabel.Text = "NPCs: " + npcCount.ToString();
            monsterCountLabel.Text = "Monsters: " + monsterCount.ToString();
        }

        private void LoadItems()
        {
            try
            {
                otItems = new OtItems();
                otItems.Load("items.otb");
                Trace.WriteLine("Open Tibia items successfully loaded.");
            }
            catch (Exception e)
            {
                MessageBox.Show(this, "Unable to load items. Details: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        private void ReadTibiaCastFilesCallback(IAsyncResult ar)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<IAsyncResult>(ReadTibiaCastFilesCallback), ar);
                return;
            }

            var tile = map.Tiles.FirstOrDefault();

            if (tile != null && miniMap.CenterLocation == null)
                miniMap.CenterLocation = tile.Location;

            miniMap.EndUpdate();
        }

        private void loadClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var chooserOptions = new ClientChooserOptions();
                chooserOptions.Smart = true;
                chooserOptions.ShowOTOption = false;
                chooserOptions.OfflineOnly = true;

                var c = ClientChooser.ShowBox(chooserOptions);

                if (c != null)
                {
                    if (c.Version.OtbMajorVersion != otItems.MajorVersion || c.Version.OtbMinorVersion != otItems.MinorVersion)
                        Trace.WriteLine("[Warning] This client requires the version " + c.Version.OtbMajorVersion + "." + c.Version.OtbMinorVersion + " of items.otb.");

                    c.LoginServers = new LoginServer[] { new LoginServer("127.0.0.1", 7171) };
                    c.EnableProxy();
                    Client = c;
                    Trace.WriteLine("Client successfully loaded.");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Error] Unable to load tibia client. Details: " + ex.StackTrace.ToString());
            }
        }

        private void Client_OpenShopWindow(object sender, OpenShopWindowEventArgs e)
        {
            if (!TrackNPCs)
                return;

            var creature = client.BattleList.GetCreature(e.Shop.Name);
            if (creature == null)
                return;

            var key = creature.Name.ToLower().Trim();
            if (!npcs.ContainsKey(key))
                npcs.Add(key, new NpcInfo(creature, otItems));

            npcs[key].Shop = e.Shop;
        }

        private void Chat_PlayerSpeak(object sender, PlayerSpeakEventArgs e)
        {
            if (!TrackNPCs)
                return;

            lastPlayerSpeech = e.Text.ToLower();
            lastPlayerSpeechTime = DateTime.Now;
        }


        private void Chat_CreatureSpeak(object sender, CreatureSpeakEventArgs e)
        {
            if (!TrackNPCs)
                return;

            if (e.Creature.Type == CreatureType.NPC)
            {
                var key = e.Creature.Name.ToLower().Trim();
                if (!npcs.ContainsKey(key))
                    npcs.Add(key, new NpcInfo(e.Creature, otItems));

                var npcInfo = npcs[key];

                if (e.Type == MessageClasses.NPC_FROM)
                {
                    var playerName = client.BattleList.GetPlayer().Name;

                    if (lastPlayerSpeech != null && lastPlayerSpeechTime.AddSeconds(2) > DateTime.Now)
                        npcInfo.AddStatement(lastPlayerSpeech, e.Text.Replace(playerName, "|PLAYERNAME|"));

                    if (NPCAutoTalk && !client.IsClinentless && client.LoggedIn)
                    {
                        CancelSendNextNPCWordSchedule();
                        sendNpcWordScheduleId = client.Scheduler.Add(new Schedule(200, () => { SendNextNPCWord(npcInfo); }));
                    }
                }
                else if (e.Type == MessageClasses.SPEAK_SAY)
                {
                    npcInfo.AddVoice(e.Text, false);
                }
                else if (e.Type == MessageClasses.SPEAK_YELL)
                {
                    npcInfo.AddVoice(e.Text, true);
                }
            }
        }

        private void CancelSendNextNPCWordSchedule()
        {
            if (sendNpcWordScheduleId > 0)
            {
                client.Scheduler.Remove(sendNpcWordScheduleId);
                sendNpcWordScheduleId = 0;
            }
        }

        private void SendNextNPCWord(NpcInfo npcInfo)
        {
            CancelSendNextNPCWordSchedule();

            if (!NPCAutoTalk)
                return;

            var word = npcInfo.NotTriedWords.FirstOrDefault();
            if (word == null)
                Trace.WriteLine("No more words to say to " + npcInfo.Name + ".");
            else
            {
                lastPlayerSpeech = word;
                lastPlayerSpeechTime = DateTime.Now;

                client.Chat.SayToNpc(word);
                npcInfo.TriedWords.Add(word);

                sendNpcWordScheduleId = client.Scheduler.Add(new Schedule(1000, () => { SendNextNPCWord(npcInfo); }));
            }
        }

        private void BattleList_CreatureAdded(object sender, CreatureAddedEventArgs e)
        {
            if (!TrackNPCs)
                return;

            if (e.Creature.Type == CreatureType.NPC)
            {
                var key = e.Creature.Name.ToLower().Trim();
                if (!npcs.ContainsKey(key))
                    npcs.Add(key, new NpcInfo(e.Creature, otItems));
            }
        }

        private void Map_Updated(object sender, MapUpdatedEventArgs e)
        {
            try
            {
                lock (map)
                {
                    miniMap.BeginUpdate();

                    foreach (var tile in e.Tiles)
                    {
                        if (ShareTrackedMap && !client.IsClinentless && !client.IsOpenTibiaServer)
                            mapShare.Add(tile);

                        if (TrackOnlyCurrentFloor && tile.Location.Z != Client.PlayerLocation.Z)
                            continue;

                        var index = tile.Location.ToIndex();

                        OtTile mapTile = map.GetTile(tile.Location);
                        if (mapTile != null && !RetrackTiles)
                            continue;
                        else if (mapTile == null)
                            mapTile = new OtTile(tile.Location);

                        mapTile.Clear();

                        for (int i = 0; i < tile.ThingCount; i++)
                        {
                            var thing = tile.GetThing(i);

                            if (thing is Creature)
                            {
                                var creature = thing as Creature;

                                if (creature.Type == CreatureType.PLAYER || (!TrackMonsters && creature.Type == CreatureType.MONSTER) || (!TrackNPCs && creature.Type == CreatureType.NPC))
                                    continue;

                                map.AddCreature(new OtCreature { Id = creature.Id, Location = creature.Location, Name = creature.Name, Type = creature.Type });
                            }
                            else if (thing is Item)
                            {
                                var item = tile.GetThing(i) as Item;

                                var itemType = otItems.GetItemBySpriteId((ushort)item.Id);
                                if (itemType == null)
                                {
                                    Trace.TraceWarning("Tibia item not in items.otb. Details: item id " + item.Id.ToString());
                                    continue;
                                }

                                if (item.Type.IsMoveable && !TrackMoveableItems)
                                    continue;
                                if (item.IsSplash && !TrackSplashes)
                                    continue;

                                OtItem mapItem = OtItem.Create(itemType);

                                if (mapItem.Type.IsStackable)
                                    mapItem.SetAttribute(OtItemAttribute.COUNT, item.Count);
                                else if (mapItem.Type.Group == OtItemGroup.Splash || mapItem.Type.Group == OtItemGroup.FluidContainer)
                                    mapItem.SetAttribute(OtItemAttribute.COUNT, OtConverter.TibiaFluidToOtFluid(item.SubType));

                                mapTile.AddItem(mapItem);
                            }
                        }

                        map.SetTile(mapTile);
                    }

                    miniMap.CenterLocation = Client.PlayerLocation;
                    miniMap.EndUpdate();

                    UpdateCounters(map.TileCount, map.NpcCount, map.MonsterCount);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Error] Unable to convert tibia tile to open tibia tile. Details: " + ex.Message);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Map Files (*.otbm)|*.otbm";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lock (map)
                {
                    try
                    {
                        map.Save(saveFileDialog.FileName);
                        Trace.WriteLine("Map successfully saved.");
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("[Error] Unable to save map file. Details: " + ex.Message);
                    }
                }
            }
        }

        private void loadMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TibiaCast Files (*.otbm)|*.otbm";
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                lock (map)
                {
                    try
                    {
                        map.Load(openFileDialog.FileName, RetrackTiles);
                        var tile = map.Tiles.FirstOrDefault();

                        if (tile != null && miniMap.CenterLocation == null)
                            miniMap.CenterLocation = tile.Location;

                        UpdateCounters(map.TileCount, map.NpcCount, map.MonsterCount);

                        Trace.WriteLine("Map successfully loaded.");
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("[Error] Unable to load the map. Details: " + ex.Message);
                    }
                }
            }
        }

        private void trackTibiaCastFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TibiaCast Files (*.recording)|*.recording";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Tibiacast\\Recordings";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    if (Client == null)
                    {
                        var c = new Client("Tibia.dat", ClientVersion.Current);
                        Client = c;
                    }

                    miniMap.BeginUpdate();

                    var reader = new TibiaCastReader(Client);
                    reader.BeginRead(openFileDialog.FileNames, ReadTibiaCastFilesCallback, null);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Error while tracking tibia cast files. Details: " + ex.Message);
                }
            }
        }

        private void saveNPCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    var directory = folderBrowserDialog.SelectedPath;
                    var scriptDirectory = Path.Combine(directory, "scripts");

                    if (!Directory.Exists(scriptDirectory))
                        Directory.CreateDirectory(scriptDirectory);

                    foreach (var npcEntry in npcs.Values)
                        npcEntry.Save(directory);

                    Trace.WriteLine("NPCs successfully saved.");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Exception while to save npc files. Details: " + ex.Message);
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (map)
            {
                npcs.Clear();
                map.Clear();
                miniMap.Invalidate();
                traceTextBox.Text = "";

                UpdateCounters(map.TileCount, map.NpcCount, map.MonsterCount);
            }
        }

        private void highlitghtMissingTilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            miniMap.HighlightMissingTiles = highlitghtMissingTilesCheckBox.Checked;
        }

        private void shareTrackedMapCheckBox_Click(object sender, EventArgs e)
        {
            if (shareTrackedMapCheckBox.Checked)
                mapShare.Start();
            else
                mapShare.Stop();
        }

        private void changeIPToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
