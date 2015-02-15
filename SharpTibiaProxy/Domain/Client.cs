using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using SharpTibiaProxy.Network;
using SharpTibiaProxy.Util;

namespace SharpTibiaProxy.Domain
{
    public class OpenShopWindowEventArgs : EventArgs
    {
        public Shop Shop { get; set; }
    }

    public class Client : IDisposable
    {
        public static readonly LoginServer[] DefaultServers = 
        {
            new LoginServer("login01.tibia.com"),
            new LoginServer("login02.tibia.com"),
            new LoginServer("login03.tibia.com"),
            new LoginServer("login04.tibia.com"),
            new LoginServer("login05.tibia.com"),
            new LoginServer("tibia01.cipsoft.com"),
            new LoginServer("tibia02.cipsoft.com"),
            new LoginServer("tibia03.cipsoft.com"),
            new LoginServer("tibia04.cipsoft.com"),
            new LoginServer("tibia05.cipsoft.com")
        };

        public event EventHandler<OpenShopWindowEventArgs> OpenShopWindow;
        public event EventHandler Exited;

        internal Proxy Proxy { get; private set; }
        private MemoryAddresses memoryAddresses;

        public long BaseAddress { get; private set; }

        public IntPtr ProcessHandle;

        private bool disposed;

        public bool IsOpenTibiaServer { get; set; }

        public uint PlayerId { get; set; }
        public Location PlayerLocation { get; set; }
        public bool PlayerCanReportBugs { get; set; }
        public bool CanChangePvpFraming { get; set; }
        public bool ExpertModeButtonEnabled { get; set; }

        public Process Process { get; private set; }
        public Items Items { get; private set; }
        public Map Map { get; private set; }
        public BattleList BattleList { get; private set; }
        public ProtocolWorld ProtocolWorld { get; private set; }
        public Chat Chat { get; private set; }
        public Market Market { get; private set; }

        public Dispatcher Dispatcher { get; private set; }
        public Scheduler Scheduler { get; private set; }

        public ClientVersion Version { get; set; }

        public MemoryAddresses MemoryAddresses
        {
            get
            {
                if (memoryAddresses == null)
                    memoryAddresses = new MemoryAddresses(this);

                return memoryAddresses;
            }
        }

        public Client(Process process, ClientVersion version)
            : this(process, version, Path.GetDirectoryName(process.MainModule.FileName))
        {
        }

        private Client(Process process, ClientVersion version, string dataDirectory)
        {
            this.Version = version;

            this.Process = process;
            this.Process.EnableRaisingEvents = true;
            this.Process.Exited += Process_Exited;

            //this.Process.WaitForInputIdle();

            ProcessHandle = WinApi.OpenProcess(WinApi.PROCESS_ALL_ACCESS, 0, (uint)process.Id);
            BaseAddress = WinApi.GetBaseAddress(ProcessHandle).ToInt64();

            Initialize(Path.Combine(dataDirectory, "Tibia.dat"));
        }

        ~Client()
        {
            Dispose();
        }

        public Client(string datFilename, ClientVersion version)
        {
            this.Version = version;
            Initialize(datFilename);
        }

        private void Initialize(string datFilename)
        {
            Dispatcher = new Dispatcher();
            Scheduler = new Scheduler(Dispatcher);

            Dispatcher.Start();
            Scheduler.Start();

            Items = new Items();
            Items.Load(datFilename, this.Version.Number);

            Map = new Map(this);
            BattleList = new BattleList(this);
            Chat = new Chat(this);
            Market = new Market(this);
            ProtocolWorld = new ProtocolWorld(this);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            try
            {
                Dispose();
                Exited.Raise(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[Error] " + ex.Message);
            }
        }

        public string WindowText
        {
            set
            {
                if (Process == null) return;
                WinApi.SetWindowText(Process.MainWindowHandle, value);
            }
        }

        public string Rsa
        {
            get { return Memory.ReadString(ProcessHandle, MemoryAddresses.ClientRsa, 309); }
            set { Memory.WriteRSA(ProcessHandle, MemoryAddresses.ClientRsa, value); }
        }

        public LoginServer[] LoginServers
        {
            get
            {
                LoginServer[] servers = new LoginServer[MemoryAddresses.ClientServerMax];
                long address = MemoryAddresses.ClientServerStart;
                if (this.Version.Number <= ClientVersion.Version1010.Number)
                {
                    for (int i = 0; i < MemoryAddresses.ClientServerMax; i++)
                    {
                        servers[i] = new LoginServer(
                            Memory.ReadString(ProcessHandle, address),
                            (short)Memory.ReadInt32(ProcessHandle, address + MemoryAddresses.ClientServerDistancePort)
                        );
                        address += MemoryAddresses.ClientServerStep;
                    }
                }
                else if (this.Version.Number >= ClientVersion.Version1011.Number)
                {
                    IntPtr StartPointer = new IntPtr(MemoryAddresses.ClientServerStart);
                    IntPtr EndPointer = new IntPtr(MemoryAddresses.ClientServerEnd);

                    uint LoginStart = Memory.ReadUInt32(ProcessHandle, (long)StartPointer);
                    uint LoginEnd = Memory.ReadUInt32(ProcessHandle, (long)EndPointer) - (uint)MemoryAddresses.ClientServerStep;

                    uint i = 0;
                    for (uint LoginServer = LoginStart; LoginServer <= LoginEnd; LoginServer += (uint)MemoryAddresses.ClientServerStep)
                    {
                        uint addressOfIP = Memory.ReadUInt32(ProcessHandle, LoginServer + MemoryAddresses.ClientServerDistanceIP);
                        uint addressOfHostname = Memory.ReadUInt32(ProcessHandle, LoginServer + MemoryAddresses.ClientServerDistanceHostname);
                        uint addressOfPort = LoginServer + (uint)MemoryAddresses.ClientServerDistancePort;

                        if (addressOfIP != 0)
                        {
                            servers[i] = new LoginServer(Memory.ReadString(ProcessHandle, addressOfHostname), (int)Memory.ReadUInt16(ProcessHandle, addressOfPort));
                        }
                        i++;
                    }
                }
                return servers;
            }
            set
            {
                long address = MemoryAddresses.ClientServerStart;
                if (this.Version.Number <= ClientVersion.Version1010.Number)
                {
                    for (int i = 0; i < MemoryAddresses.ClientServerMax; i++)
                    {
                        Memory.WriteString(ProcessHandle, address, value[i % value.Length].Server);
                        Memory.WriteInt32(ProcessHandle, address + MemoryAddresses.ClientServerDistancePort, value[i % value.Length].Port);
                        address += MemoryAddresses.ClientServerStep;
                    }
                }
                else if (this.Version.Number >= ClientVersion.Version1011.Number)
                {
                    IntPtr StartPointer = new IntPtr(MemoryAddresses.ClientServerStart);
                    IntPtr EndPointer = new IntPtr(MemoryAddresses.ClientServerEnd);

                    uint LoginStart = Memory.ReadUInt32(ProcessHandle, (long)StartPointer);
                    uint LoginEnd = Memory.ReadUInt32(ProcessHandle, (long)EndPointer) - (uint)MemoryAddresses.ClientServerStep;

                    uint i = 0;
                    for (uint LoginServer = LoginStart; LoginServer <= LoginEnd; LoginServer += (uint)MemoryAddresses.ClientServerStep)
                    {
                        uint addressOfIP = Memory.ReadUInt32(ProcessHandle, LoginServer + MemoryAddresses.ClientServerDistanceIP);

                        if (addressOfIP != 0)
                        {
                            uint addressOfPort = LoginServer + (uint)MemoryAddresses.ClientServerDistancePort;
                            Memory.WriteInt16(ProcessHandle, addressOfPort, (short)value[i % value.Length].Port);

                            int IP = BitConverter.ToInt32(System.Net.IPAddress.Parse(System.Net.Dns.GetHostAddresses(value[i % value.Length].Server)[0].ToString()).GetAddressBytes(), 0);
                            Memory.WriteInt32(ProcessHandle, addressOfIP, IP);

                            uint addressOfHost = Memory.ReadUInt32(ProcessHandle, LoginServer + MemoryAddresses.ClientServerDistanceHostname);
                            Memory.WriteString(ProcessHandle, addressOfHost, value[i % value.Length].Server);
                        }
                        i++;
                    }
                }
            }
        }

        public int SelectedChar
        {
            get { return Memory.ReadInt32(ProcessHandle, MemoryAddresses.ClientSelectedCharacter); }
            set { Memory.WriteInt32(ProcessHandle, MemoryAddresses.ClientSelectedCharacter, value); }
        }

        public bool ProxyEnabled { get { return Proxy != null; } }

        public bool IsClinentless { get { return Process == null; } }

        public bool HasExited { get { return Process != null && Process.HasExited; } }

        public bool LoggedIn { get { return Status == Constants.LoginStatus.LoggedIn; } }

        public Constants.LoginStatus Status { get { return (Constants.LoginStatus)Memory.ReadByte(ProcessHandle, MemoryAddresses.ClientStatus); } }

        public void PlayerGoTo(Location location)
        {
            if (IsClinentless || MemoryAddresses.PlayerGoX == 0 || !LoggedIn)
                return;

            Memory.WriteUInt16(ProcessHandle, MemoryAddresses.PlayerGoX, (ushort)location.X);
            Memory.WriteUInt16(ProcessHandle, MemoryAddresses.PlayerGoY, (ushort)location.Y);
            Memory.WriteByte(ProcessHandle, MemoryAddresses.PlayerGoZ, (byte)location.Z);
            Memory.WriteByte(ProcessHandle, MemoryAddresses.ClientBattleListStart + (PlayerBattleListIndex * MemoryAddresses.ClientBattleListStep)
                + MemoryAddresses.ClientBattleListCreatureWalkDistance, 1);
        }

        public int PlayerBattleListIndex
        {
            get
            {
                if (IsClinentless || MemoryAddresses.ClientBattleListStart == 0 || !LoggedIn)
                    return -1;

                for (int i = 0; i < MemoryAddresses.ClientBattleListMaxCreatures; i++)
                {
                    if (Memory.ReadUInt32(ProcessHandle, MemoryAddresses.ClientBattleListStart + (i * MemoryAddresses.ClientBattleListStep)) == PlayerId)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }


        public void EnableProxy()
        {
            if (Process == null)
                throw new Exception("Can not enable proxy on a clientless instance.");

            if (Proxy != null)
                return;

            Proxy = new Proxy(this);
            Proxy.Enable();
        }

        public void DisableProxy()
        {
            if (Proxy == null)
                return;

            Proxy.Disable();
            Proxy = null;
        }

        #region Open Client

        public static Client Open()
        {
            return Open(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Tibia\tibia.exe"));
        }

        public static Client Open(string path)
        {
            ProcessStartInfo psi = new ProcessStartInfo(path);
            psi.UseShellExecute = true; // to avoid opening currently running Tibia's
            psi.WorkingDirectory = System.IO.Path.GetDirectoryName(path);
            return Open(psi);
        }

        public static Client Open(string path, string arguments)
        {
            ProcessStartInfo psi = new ProcessStartInfo(path, arguments);
            psi.UseShellExecute = true; // to avoid opening currently running Tibia's
            psi.WorkingDirectory = System.IO.Path.GetDirectoryName(path);
            return Open(psi);
        }

        public static Client Open(ProcessStartInfo psi)
        {
            var fileVersion = FileVersionInfo.GetVersionInfo(psi.FileName).FileVersion;
            var version = ClientVersion.GetFromFileVersion(fileVersion);

            if (version == null)
                throw new Exception("The version " + fileVersion + " is not supported.");

            Process p = Process.Start(psi);
            return new Client(p, version);
        }


        public static Client OpenMC()
        {
            return OpenMC(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Tibia\tibia.exe"), "");
        }


        public static Client OpenMC(string path, string arguments)
        {
            var fileVersion = FileVersionInfo.GetVersionInfo(path).FileVersion;
            var version = ClientVersion.GetFromFileVersion(fileVersion);

            if (version == null)
                throw new Exception("The version " + fileVersion + " is not supported.");

            WinApi.PROCESS_INFORMATION pi = new WinApi.PROCESS_INFORMATION();
            WinApi.STARTUPINFO si = new WinApi.STARTUPINFO();

            if (arguments == null)
                arguments = "";

            WinApi.CreateProcess(path, " " + arguments, IntPtr.Zero, IntPtr.Zero, false, WinApi.CREATE_SUSPENDED, IntPtr.Zero, Path.GetDirectoryName(path), ref si, out pi);

            Process p = Process.GetProcessById(Convert.ToInt32(pi.dwProcessId));

            var client = new Client(p, version, Path.GetDirectoryName(path));

            Memory.WriteByte(client.ProcessHandle, client.MemoryAddresses.ClientMultiClient, client.MemoryAddresses.ClientMultiClientJMP);

            WinApi.ResumeThread(pi.hThread);
            p.WaitForInputIdle();

            Memory.WriteByte(client.ProcessHandle, client.MemoryAddresses.ClientMultiClient, client.MemoryAddresses.ClientMultiClientJNZ);

            WinApi.CloseHandle(pi.hProcess);
            WinApi.CloseHandle(pi.hThread);

            return client;
        }

        #endregion

        #region Client Processes

        public static List<Client> GetClients()
        {

            return GetClients(null);
        }

        public static List<Client> GetClients(string version)
        {
            return GetClients(version, false);
        }

        public static List<Client> GetClients(string version, bool offline)
        {
            List<Client> clients = new List<Client>();

            foreach (Process process in Process.GetProcesses())
            {
                StringBuilder classname = new StringBuilder();
                WinApi.GetClassName(process.MainWindowHandle, classname, 12);

                if (classname.ToString().Equals("TibiaClient", StringComparison.CurrentCultureIgnoreCase))
                {
                    var clientVersion = ClientVersion.GetFromFileVersion(process.MainModule.FileVersionInfo.FileVersion);

                    //Version not supported.
                    if (clientVersion == null)
                        continue;

                    if (version == null || clientVersion.FileVersion == version)
                    {
                        var client = new Client(process, clientVersion);
                        if (!offline || !client.LoggedIn)
                            clients.Add(client);
                    }
                }
            }
            return clients;
        }

        public void Close()
        {
            if (Process != null && !Process.HasExited)
                Process.Kill();
        }

        #endregion

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            DisableProxy();

            Scheduler.Shutdown();
            Dispatcher.Shutdown();

            if (ProcessHandle != null && ProcessHandle != IntPtr.Zero)
            {
                WinApi.CloseHandle(ProcessHandle);
                ProcessHandle = IntPtr.Zero;
            }
        }

        public override string ToString()
        {
            string s = "[" + Version.Number + "] ";
            if (!LoggedIn)
                s += "Not logged in.";
            else
                s += "Logged in.";

            return s;
        }

        internal void OnOpenShopWindow(Shop shop)
        {
            OpenShopWindow.Raise(this, new OpenShopWindowEventArgs { Shop = shop });
        }
    }
}
