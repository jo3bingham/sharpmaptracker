using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SharpTibiaProxy.Domain;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SharpTibiaProxy.Network
{
	public enum Protocol
	{
		None, Login, World
	}

	public class Proxy
	{
		private Client client;

		private int loginClientPort;
		private int worldClientPort;

		private Socket loginClientSocket;
		private Socket worldClientSocket;

		private Socket clientSocket;
		private Socket serverSocket;

		private InMessage clientInMessage;
		private OutMessage clientOutMessage;

		private InMessage serverInMessage;
		private OutMessage serverOutMessage;

		private bool accepting;

		private LoginServer[] loginServers;

		private Protocol protocol = Protocol.None;

		private uint[] xteaKey;
		private CharacterLoginInfo[] charList;

		private int pendingSend;

		public Proxy(Client client)
		{
			this.client = client;
		}

		public void Enable()
		{
			client.Rsa = Constants.RSAKey.OpenTibiaM;

			loginClientPort = GetFreePort();
			worldClientPort = GetFreePort(loginClientPort + 1);

			if (!client.IsOpenTibiaServer && client.LoginServers[0].Server == "127.0.0.1")
				loginServers = Client.DefaultServers;
			else
				loginServers = client.LoginServers;

			client.LoginServers = new LoginServer[] { new LoginServer("127.0.0.1", loginClientPort) };
            
            if (client.Version.Number == ClientVersion.Version1011.Number)
                Util.Memory.WriteBytes(client.ProcessHandle, client.MemoryAddresses.ClientProxyCheckFunctionPointer, client.MemoryAddresses.ClientProxyCheckFunctionNOP, 15);

			clientInMessage = new InMessage();
			clientOutMessage = new OutMessage();

			serverInMessage = new InMessage();
			serverOutMessage = new OutMessage();

			StartListen();
		}

		public void Disable()
		{
			Close();

            if (client.Version.Number == ClientVersion.Version1011.Number)
                Util.Memory.WriteBytes(client.ProcessHandle, client.MemoryAddresses.ClientProxyCheckFunctionPointer, client.MemoryAddresses.ClientProxyCheckFunctionOriginal, 15);

            if (!client.HasExited)
                client.LoginServers = loginServers;
		}

		private void StartListen()
		{
			try
			{
				lock (this)
				{
					if (accepting)
						return;

#if DEBUG_PROXY
					Trace.WriteLine("[DEBUG] Proxy [StartListen]");
#endif

					protocol = Protocol.None;

					loginClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					loginClientSocket.Bind(new IPEndPoint(IPAddress.Any, loginClientPort));
					loginClientSocket.Listen(1);
					loginClientSocket.BeginAccept(ClientBeginAcceptCallback, Protocol.Login);

					worldClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					worldClientSocket.Bind(new IPEndPoint(IPAddress.Any, worldClientPort));
					worldClientSocket.Listen(1);
					worldClientSocket.BeginAccept(ClientBeginAcceptCallback, Protocol.World);

					accepting = true;
				}

			}
			catch (Exception ex)
			{
				Trace.WriteLine("[Error] Proxy [StartListen]: " + ex.Message);
			}
		}

		private void ClientBeginAcceptCallback(IAsyncResult ar)
		{
			try
			{
				lock (this)
				{
					if (!accepting)
						return;

#if DEBUG_PROXY
					Trace.WriteLine("[DEBUG] Proxy [ClientBeginAcceptCallback]");
#endif

					if (loginClientSocket == null || worldClientSocket == null)
						return;

					accepting = false;

					Protocol protocol = (Protocol)ar.AsyncState;
					clientSocket = null;

					if (protocol == Protocol.Login)
						clientSocket = loginClientSocket.EndAccept(ar);
					else
					{
						clientSocket = worldClientSocket.EndAccept(ar);

						serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        CharacterLoginInfo selectedChar = charList[client.SelectedChar];

                        if (client.Version.Number == ClientVersion.Version1011.Number)
                        {
                            if (selectedChar.WorldIP == 0)
                            {
                                serverSocket.Connect(new IPEndPoint(Dns.GetHostAddresses("game.tibia.ciproxy.com")[0], selectedChar.WorldPort));
                            }
                            else
                            {
                                serverSocket.Connect(new IPEndPoint(selectedChar.WorldIP, selectedChar.WorldPort));
                            }
                        }
                        else
                        {
                            serverSocket.Connect(new IPEndPoint(selectedChar.WorldIP, selectedChar.WorldPort));
                        }

						serverInMessage.Reset();
						serverSocket.BeginReceive(serverInMessage.Buffer, 0, 2, SocketFlags.None, ServerReceiveCallback, null);
					}

					clientSocket.LingerState = new LingerOption(true, 2);

					loginClientSocket.Close();
					worldClientSocket.Close();

					loginClientSocket = null;
					worldClientSocket = null;

					clientInMessage.Reset();
					clientSocket.BeginReceive(clientInMessage.Buffer, 0, 2, SocketFlags.None, ClientReceiveCallback, null);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine("[Error] Proxy [ClientBeginAcceptCallback]: " + ex.Message);
			}
		}

		private void ServerReceiveCallback(IAsyncResult ar)
		{
			try
			{
				if (serverSocket == null)
					return;

#if DEBUG_PROXY
				Trace.WriteLine("[DEBUG] Proxy [ServerReceiveCallback]");
#endif

				int count = serverSocket.EndReceive(ar);

				if (count <= 0)
					throw new Exception("Connection lost.");

				serverInMessage.Size = serverInMessage.ReadHead() + 2;
				int read = 2;

				while (read < serverInMessage.Size)
				{
					count = serverSocket.Receive(serverInMessage.Buffer, read, serverInMessage.Size - read, SocketFlags.None);

					if (count <= 0)
						throw new Exception("Connection lost.");

					read += count;
				}

				ParseServerMessage();

				serverInMessage.Reset();
				serverSocket.BeginReceive(serverInMessage.Buffer, 0, 2, SocketFlags.None, ServerReceiveCallback, null);
			}
			catch (Exception ex)
			{
#if DEBUG_PROXY
				Trace.WriteLine("[DEBUG] Proxy [ServerReceiveCallback] " + ex.Message);
#endif
				Restart();
			}
		}

		public void SendToClient(OutMessage message)
		{
			if (message != clientOutMessage)
			{
				message.WriteInternalHead();
				Xtea.Encrypt(message, xteaKey);
				Adler.Generate(message, true);
				message.WriteHead();
			}

			lock (clientSocket)
			{
				pendingSend++;
				clientSocket.Send(message.Buffer, 0, message.Size, SocketFlags.None);
				pendingSend--;
			}
		}

		private void ParseServerMessage()
		{
#if DEBUG_PROXY
			Trace.WriteLine("[DEBUG] Proxy [ParseServerMessage]");
#endif
			switch (protocol)
			{
				case Protocol.None:
					
					clientOutMessage.Reset();
					Array.Copy(serverInMessage.Buffer, clientOutMessage.Buffer, serverInMessage.Size);
					clientOutMessage.Size = serverInMessage.Size;
					SendToClient(clientOutMessage);

					break;
				case Protocol.Login:
					ParseServerLoginMessage();
					break;
				case Protocol.World:
					ParseServerWorldMessage();
					break;
			}
		}

		private void ParseServerWorldMessage()
		{
#if DEBUG_PROXY
			Trace.WriteLine("[DEBUG] Proxy [ParseServerWorldMessage]");
#endif
			clientOutMessage.Reset();
			Array.Copy(serverInMessage.Buffer, clientOutMessage.Buffer, serverInMessage.Size);
			clientOutMessage.Size = serverInMessage.Size;

			serverInMessage.ReadPosition = 2;

			if (Adler.Generate(serverInMessage) != serverInMessage.ReadChecksum())
				throw new Exception("Wrong checksum.");

			Xtea.Decrypt(serverInMessage, xteaKey);
			serverInMessage.Size = serverInMessage.ReadInternalHead() + 8;
			serverInMessage.ReadPosition = 8;

			client.ProtocolWorld.ParseServerMessage(serverInMessage);

			SendToClient(clientOutMessage);
		}

		private void ParseServerLoginMessage()
		{
#if DEBUG_PROXY
			Trace.WriteLine("[DEBUG] Proxy [ParseServerLoginMessage]");
#endif
			serverInMessage.ReadPosition = 2;

			if (Adler.Generate(serverInMessage) != serverInMessage.ReadChecksum())
				throw new Exception("Wrong checksum.");

			Xtea.Decrypt(serverInMessage, xteaKey);
			serverInMessage.Size = serverInMessage.ReadInternalHead() + 8;
			serverInMessage.ReadPosition = 8;

			clientOutMessage.Reset();
			Array.Copy(serverInMessage.Buffer, clientOutMessage.Buffer, serverInMessage.Size);
			clientOutMessage.Size = serverInMessage.Size;

			while (serverInMessage.ReadPosition < serverInMessage.Size)
			{
				byte cmd = serverInMessage.ReadByte();

				switch (cmd)
				{
					case 0x0A: //Error message
						var msg = serverInMessage.ReadString();
						break;
					case 0x0B: //For your information
						serverInMessage.ReadString();
						break;
                    case 0x0C: //token succes
                    case 0x0D: //token error
                        serverInMessage.ReadByte();
                        break;
                    case 0x11: //update
                        serverInMessage.ReadString();
                        break;
					case 0x14: //MOTD
						serverInMessage.ReadString();
						break;
					case 0x1E: //Patching exe/dat/spr messages
					case 0x1F:
					case 0x20:
						//DisconnectClient(0x0A, "A new client is avalible, please download it first!");
						break;
					case 0x28: //session key
                        serverInMessage.ReadString();
						break;
                    case 0x64: //character list
                        if (client.Version.Number <= ClientVersion.Version1011.Number)
                        {
                            int nChar = (int)serverInMessage.ReadByte();
                            charList = new CharacterLoginInfo[nChar];

                            for (int i = 0; i < nChar; i++)
                            {
                                charList[i].CharName = serverInMessage.ReadString();
                                charList[i].WorldName = serverInMessage.ReadString();
                                clientOutMessage.WriteAt(new byte[] { 127, 0, 0, 1 }, serverInMessage.ReadPosition);
                                charList[i].WorldIP = serverInMessage.ReadUInt();
                                clientOutMessage.WriteAt(BitConverter.GetBytes((ushort)worldClientPort), serverInMessage.ReadPosition);
                                charList[i].WorldPort = serverInMessage.ReadUShort();

                                if (client.Version.Number >= ClientVersion.Version981.Number)
                                    serverInMessage.ReadByte(); //isPreviewWorld
                            }

                            if (client.Version.Number >= ClientVersion.Version1011.Number)
                                serverInMessage.ReadUShort(); //PremiumTime
                        }
                        else if (client.Version.Number >= ClientVersion.Version1012.Number)
                        {
                            clientOutMessage.WritePosition = serverInMessage.ReadPosition;

                            byte nWorlds = serverInMessage.ReadByte();
                            clientOutMessage.WriteByte(nWorlds);
                            WorldLoginInfo[] worldList = new WorldLoginInfo[nWorlds];
                            for (byte i = 0; i < nWorlds; i++)
                            {
                                worldList[i].ID = serverInMessage.ReadByte();
                                clientOutMessage.WriteByte(worldList[i].ID);
                                worldList[i].Name = serverInMessage.ReadString();
                                clientOutMessage.WriteString(worldList[i].Name);
                                worldList[i].Hostname = serverInMessage.ReadString();
                                clientOutMessage.WriteString("127.0.0.1");
                                worldList[i].Port = serverInMessage.ReadUShort();
                                clientOutMessage.WriteUShort((ushort)worldClientPort);
                                worldList[i].IsPreviewWorld = serverInMessage.ReadBool();
                                clientOutMessage.WriteByte(Convert.ToByte(worldList[i].IsPreviewWorld));
                            }

                            byte nChars = serverInMessage.ReadByte();
                            clientOutMessage.WriteByte(nChars);
                            charList = new CharacterLoginInfo[nChars];
                            for (byte j = 0; j < nChars; j++)
                            {
                                byte WorldID = serverInMessage.ReadByte();
                                clientOutMessage.WriteByte(WorldID);
                                charList[j].CharName = serverInMessage.ReadString();
                                clientOutMessage.WriteString(charList[j].CharName);
                                charList[j].WorldName = worldList[WorldID].Name;
                                charList[j].WorldIP = BitConverter.ToUInt32(Dns.GetHostAddresses(worldList[WorldID].Hostname)[0].GetAddressBytes(), 0);
                                charList[j].WorldPort = worldList[WorldID].Port;
                                charList[j].WorldIPString = worldList[WorldID].Hostname;
                            }

                            ushort PremiumTime = serverInMessage.ReadUShort();
                            clientOutMessage.WriteUShort(PremiumTime);

                            clientOutMessage.Size = clientOutMessage.WritePosition;
                        }
						break;
					default:
						break;
				}
			}

			clientOutMessage.WriteInternalHead();
			Xtea.Encrypt(clientOutMessage, xteaKey);
			Adler.Generate(clientOutMessage, true);
			clientOutMessage.WriteHead();

			SendToClient(clientOutMessage);
		}

		private void ClientReceiveCallback(IAsyncResult ar)
		{
			try
			{
				if (clientSocket == null)
					return;

#if DEBUG_PROXY
				Trace.WriteLine("[DEBUG] Proxy [ClientReceiveCallback]");
#endif

				int count = clientSocket.EndReceive(ar);

				if (count <= 0)
					throw new Exception("Connection lost.");

				clientInMessage.Size = clientInMessage.ReadHead() + 2;
				int read = 2;

				while (read < clientInMessage.Size)
				{
					count = clientSocket.Receive(clientInMessage.Buffer, read, clientInMessage.Size - read, SocketFlags.None);

					if (count <= 0)
						throw new Exception("Connection lost.");

					read += count;
				}

				ParseClientMessage();

				clientInMessage.Reset();
				clientSocket.BeginReceive(clientInMessage.Buffer, 0, 2, SocketFlags.None, ClientReceiveCallback, null);
			}
			catch (Exception ex)
			{
#if DEBUG_PROXY
				Trace.WriteLine("[DEBUG] Proxy [ClientReceiveCallback] " + ex.Message);
#endif
				Restart();
			}
		}

		private void ParseClientMessage()
		{
#if DEBUG_PROXY
			Trace.WriteLine("[DEBUG] Proxy [ParseClientMessage]");
#endif

			switch (protocol)
			{
				case Protocol.None:
					ParseFirstClientMessage();
					break;
				case Protocol.Login:
					throw new Exception("Invalid client message.");
				case Protocol.World:
					ParseClientWorldMessage();
					break;
			}
		}

		private void ParseClientWorldMessage()
		{
#if DEBUG_PROXY
			Trace.WriteLine("[DEBUG] Proxy [ParseClientWorldMessage]");
#endif
			serverOutMessage.Reset();
			Array.Copy(clientInMessage.Buffer, serverOutMessage.Buffer, clientInMessage.Size);
			serverOutMessage.Size = clientInMessage.Size;

			clientInMessage.ReadPosition = 2;

			if (Adler.Generate(clientInMessage) != clientInMessage.ReadChecksum())
				throw new Exception("Wrong checksum.");

			Xtea.Decrypt(clientInMessage, xteaKey);
			clientInMessage.Size = clientInMessage.ReadInternalHead() + 8;
			clientInMessage.ReadPosition = 8;

			client.ProtocolWorld.ParseClientMessage(clientInMessage);

			SendToServer(serverOutMessage);
		}

		public void SendToServer(OutMessage message)
		{
			if (message != serverOutMessage)
			{
				message.WriteInternalHead();
				Xtea.Encrypt(message, xteaKey);
				Adler.Generate(message, true);
				message.WriteHead();
			}

			lock (serverSocket)
			{
				serverSocket.Send(message.Buffer, 0, message.Size, SocketFlags.None);
			}
		}

		private void ParseFirstClientMessage()
		{
#if DEBUG_PROXY
			Trace.WriteLine("[DEBUG] Proxy [ParseFirstClientMessage]");
#endif

			clientInMessage.ReadPosition = 2;
			clientInMessage.Encrypted = false;

			if (Adler.Generate(clientInMessage) != clientInMessage.ReadUInt())
				throw new Exception("Wrong checksum.");

			byte protocolId = clientInMessage.ReadByte();

			if (protocolId == 0x01) //Login
			{
				protocol = Protocol.Login;
                ushort clientType = clientInMessage.ReadUShort();
                ushort protocolVersion = clientInMessage.ReadUShort();

                if (client.Version.Number >= ClientVersion.Version981.Number)
                {
                    uint clientVersion = clientInMessage.ReadUInt();
                }

				uint datSignature = clientInMessage.ReadUInt();
				uint sprSignature = clientInMessage.ReadUInt();
				uint picSignature = clientInMessage.ReadUInt();

                if (client.Version.Number >= ClientVersion.Version981.Number)
                {
                    byte clientPreviewState = clientInMessage.ReadByte();
                }

				Rsa.OpenTibiaDecrypt(clientInMessage);

                if (client.Version.Number >= ClientVersion.Version1073.Number)
                {
                    int tempPos = clientInMessage.ReadPosition;
                    clientInMessage.ReadPosition = clientInMessage.Size - 128;
                    Rsa.OpenTibiaDecrypt(clientInMessage);
                    clientInMessage.ReadPosition = tempPos;
                }

                Array.Copy(clientInMessage.Buffer, serverOutMessage.Buffer, clientInMessage.Size);
                serverOutMessage.Size = clientInMessage.Size;
                serverOutMessage.WritePosition = clientInMessage.ReadPosition - 1; //the first byte is zero

				xteaKey = new uint[4];
				xteaKey[0] = clientInMessage.ReadUInt();
				xteaKey[1] = clientInMessage.ReadUInt();
				xteaKey[2] = clientInMessage.ReadUInt();
				xteaKey[3] = clientInMessage.ReadUInt();

				var acc = clientInMessage.ReadString(); //account name
				var pass = clientInMessage.ReadString(); //password

                if (client.IsOpenTibiaServer)
                {
                    Rsa.OpenTibiaEncrypt(serverOutMessage);
                }
                else
                {
                    Rsa.RealTibiaEncrypt(serverOutMessage);
                    if (client.Version.Number >= ClientVersion.Version1073.Number)
                    {
                        serverOutMessage.WritePosition = serverOutMessage.Size - 128;
                        Rsa.RealTibiaEncrypt(serverOutMessage);
                    }
                }

				Adler.Generate(serverOutMessage, true);
				serverOutMessage.WriteHead();

				serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				serverSocket.Connect(loginServers[0].Server, loginServers[0].Port);

				serverSocket.Send(serverOutMessage.Buffer, 0, serverOutMessage.Size, SocketFlags.None);

				serverInMessage.Reset();
				serverSocket.BeginReceive(serverInMessage.Buffer, 0, 2, SocketFlags.None, ServerReceiveCallback, null);
			}
			else if (protocolId == 0x0A) //Game
			{
				protocol = Protocol.World;

				ushort clientType = clientInMessage.ReadUShort();
				ushort protocolVersion = clientInMessage.ReadUShort();

                if (client.Version.Number >= ClientVersion.Version981.Number)
                {
                    uint clientVersion = clientInMessage.ReadUInt();
                    ushort contentRevision = 0;
                    if (client.Version.Number >= ClientVersion.Version1071.Number)
                        contentRevision = clientInMessage.ReadUShort();
                    byte clientPreviewState = clientInMessage.ReadByte();
                }

				Rsa.OpenTibiaDecrypt(clientInMessage);

				Array.Copy(clientInMessage.Buffer, serverOutMessage.Buffer, clientInMessage.Size);
				serverOutMessage.Size = clientInMessage.Size;
				serverOutMessage.WritePosition = clientInMessage.ReadPosition - 1; //the first byte is zero

				xteaKey = new uint[4];
				xteaKey[0] = clientInMessage.ReadUInt();
				xteaKey[1] = clientInMessage.ReadUInt();
				xteaKey[2] = clientInMessage.ReadUInt();
				xteaKey[3] = clientInMessage.ReadUInt();

				clientInMessage.ReadByte();

				//var accountName = clientInMessage.ReadString();
                var sessionKey = clientInMessage.ReadString();
				var characterName = clientInMessage.ReadString();
				//var password = clientInMessage.ReadString();

				if (client.IsOpenTibiaServer)
					Rsa.OpenTibiaEncrypt(serverOutMessage);
				else
					Rsa.RealTibiaEncrypt(serverOutMessage);

				Adler.Generate(serverOutMessage, true);
				serverOutMessage.WriteHead();

				serverSocket.Send(serverOutMessage.Buffer, 0, serverOutMessage.Size, SocketFlags.None);
			}
			else
			{
				throw new Exception("Invalid protocol " + protocolId.ToString("X2"));
			}
		}


		private void Close()
		{
#if DEBUG_PROXY
			Trace.WriteLine("[DEBUG] Proxy [Close]");
#endif

			if (loginClientSocket != null)
			{
				try
				{
					loginClientSocket.Close();
					loginClientSocket = null;
				}
				catch (Exception ex)
				{
					Trace.TraceWarning("Proxy [Restart]: " + ex.Message);
				}
			}

			if (worldClientSocket != null)
			{
				try
				{
					worldClientSocket.Close();
					worldClientSocket = null;
				}
				catch (Exception ex)
				{
					Trace.TraceWarning("Proxy [Restart]: " + ex.Message);
				}
			}
			if (clientSocket != null && clientSocket.Connected)
			{
				try
				{
					clientSocket.Close();
					clientSocket = null;
				}
				catch (Exception ex)
				{
					Trace.TraceWarning("Proxy [Restart]: " + ex.Message);
				}
			}

			if (serverSocket != null && serverSocket.Connected)
			{
				try
				{
					serverSocket.Close();
					serverSocket = null;
				}
				catch (Exception ex)
				{
					Trace.TraceWarning("Proxy [Restart]: " + ex.Message);
				}
			}
		}

		private void Restart()
		{
			lock (this)
			{
				if (accepting)
					return;

#if DEBUG_PROXY
				Trace.WriteLine("[DEBUG] Proxy [Restart]");
#endif

				if (pendingSend > 0)
				{
					client.Scheduler.Add(new Util.Schedule(500, Restart));
					return;
				}

				Close();

				clientInMessage.Reset();
				clientOutMessage.Reset();
				serverInMessage.Reset();
				serverOutMessage.Reset();

				StartListen();
			}
		}

		private static int GetFreePort()
		{
			return GetFreePort(7979);
		}

		private static int GetFreePort(int start)
		{
			while (!CheckPort(start))
				start++;
			return start;
		}

		private static bool CheckPort(int port)
		{
			try
			{
				TcpListener tcpScan = new TcpListener(IPAddress.Any, port);
				tcpScan.Start();
				tcpScan.Stop();

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
