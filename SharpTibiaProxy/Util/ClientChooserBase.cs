using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using SharpTibiaProxy.Domain;

namespace SharpTibiaProxy.Util
{
    public class OtServer
    {
        public LoginServer LoginServer;
        public string ClientVersion;
    }

    public struct ClientPathInfo
    {
        public string Path;
        public string Version;

        public ClientPathInfo(string path, string version)
        {
            Path = path;
            Version = version;
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", Path, Version);
        }
    }

    public class ClientChooserBase
    {
        public const string NewClientDefaultText = "New default client...";
        public const string NewClientCustomText = "New client (choose location)...";

        public static Client ChooseClient(ClientChooserOptions options, object selectedItem, LoginServer loginServer)
        {
            Client client = null;
            if (selectedItem.GetType() == typeof(string))
            {
                switch ((string)selectedItem)
                {
                    case NewClientDefaultText:
                        client = Client.OpenMC();
                        break;
                    case NewClientCustomText:
                        OpenFileDialog dialog = new OpenFileDialog();
                        dialog.Filter =
                           "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
                        dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                        dialog.Title = "Select a Tibia client executable";
                        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(dialog.FileName);
                            if (fvi.ProductName.Equals("Tibia Player"))
                            {
                                client = Client.Open(dialog.FileName, options.Arguments);
                                if (options.SaveClientPath == true)
                                {
                                    ClientChooserBase.SaveClientPath(
                                        options.SavedClientPathsLocation,
                                        dialog.FileName,
                                        fvi.FileVersion);
                                }

                            }
                            else
                            {
                                client = null;
                            }
                        }
                        else
                        {
                            client = null;
                        }
                        break;
                }
            }
            else if (selectedItem.GetType() == typeof(ClientPathInfo))
            {
                string clientPath = ((ClientPathInfo)selectedItem).Path;
                client = Client.OpenMC(clientPath, options.Arguments);
            }
            else
            {
                client = (Client)selectedItem;
            }

            // Set OT server
            if (client != null)
            {
                if (options.UseOT)
                {
                    client.LoginServers = new LoginServer[] { loginServer };
                    client.IsOpenTibiaServer = true;

                    client.WindowText = string.Format("SharpMapTracker - {0}:{1}", loginServer.Server, loginServer.Port);
                    SaveOtServer(options.SavedServersLocation, loginServer, client.Version.FileVersion);
                }
                else
                {
                    client.WindowText = "SharpMapTracker - Global";
                }

                // Set client to run on one processor if instructed by the user
                if (options.UseSingleProcessor)
                    client.Process.ProcessorAffinity = (IntPtr)1;
            }

            return client;
        }

        public static List<OtServer> GetServers(string location)
        {
            List<OtServer> servers = new List<OtServer>();
            if (File.Exists(location))
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(location);
                    string ip;
                    short port;
                    string version;
                    foreach (XmlElement server in document["servers"])
                    {
                        ip = server.GetAttribute("ip");
                        port = short.Parse(server.GetAttribute("port"));
                        version = server.GetAttribute("version");
                        servers.Add(new OtServer { LoginServer = new LoginServer(ip, port), ClientVersion = version });
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return servers;
        }

        public static List<ClientPathInfo> GetClientPaths(string location)
        {
            List<ClientPathInfo> clientPaths = new List<ClientPathInfo>();
            if (File.Exists(location))
            {
                try
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(location);
                    string path;
                    string version;
                    foreach (XmlElement clientPath in document["clientPaths"])
                    {
                        path = clientPath.GetAttribute("location");
                        version = clientPath.GetAttribute("version");
                        clientPaths.Add(new ClientPathInfo(path, version));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return clientPaths;
        }

        public static void SaveClientPath(string location, string fileName, string fileVersion)
        {
            XmlDocument document = new XmlDocument();
            XmlElement clientPaths = null;
            bool exists = false;
            if (File.Exists(location))
            {
                document.Load(location);
                clientPaths = document["clientPaths"];
                foreach (XmlElement clientPath in clientPaths)
                {
                    if (clientPath.GetAttribute("location").Equals(fileName))
                    {
                        if (document["clientPaths"].FirstChild != clientPath)
                        {
                            document["clientPaths"].RemoveChild(clientPath);
                            document["clientPaths"].InsertBefore(
                                clientPath,
                                document["clientPaths"].FirstChild);
                        }
                        document.Save(location);
                        if (!clientPath.GetAttribute("version").Equals(fileVersion))
                        {
                            clientPath.SetAttribute("version", fileVersion);
                            document.Save(location);
                            exists = true;
                            break;
                        }
                        exists = true;
                        break;
                    }
                }
            }
            else
            {
                XmlDeclaration Declaration = document.CreateXmlDeclaration("1.0", "", "");
                document.AppendChild(Declaration);
                clientPaths = document.CreateElement("clientPaths");
                document.AppendChild(clientPaths);
            }
            if (!exists)
            {
                XmlElement clientPath = document.CreateElement("clientPath");
                XmlAttribute locationAttr = document.CreateAttribute("location");
                locationAttr.InnerText = fileName;
                XmlAttribute version = document.CreateAttribute("version");
                version.InnerText = fileVersion;

                clientPath.Attributes.Append(locationAttr);
                clientPath.Attributes.Append(version);
                clientPaths.AppendChild(clientPath);

                if (!Directory.Exists(Constants.AppDataPath))
                    Directory.CreateDirectory(Constants.AppDataPath);

                document.Save(location);
            }
        }

        public static void SaveOtServer(string location, LoginServer ot, string version)
        {
            XmlDocument document = new XmlDocument();
            XmlElement servers = null;
            bool exists = false;

            if (File.Exists(location))
            {
                document.Load(location);
                servers = document["servers"];
                foreach (XmlElement clientPath in servers)
                {
                    if (clientPath.GetAttribute("ip").Equals(ot.Server) &&
                        clientPath.GetAttribute("port").Equals(ot.Port.ToString()))
                    {
                        exists = true;
                        break;
                    }
                }
            }
            else
            {
                XmlDeclaration Declaration = document.CreateXmlDeclaration("1.0", "", "");
                document.AppendChild(Declaration);
                servers = document.CreateElement("servers");
                document.AppendChild(servers);
            }

            if (!exists)
            {
                XmlElement server = document.CreateElement("server");

                XmlAttribute ipAttr = document.CreateAttribute("ip");
                ipAttr.InnerText = ot.Server;

                XmlAttribute portAttr = document.CreateAttribute("port");
                portAttr.InnerText = ot.Port.ToString();

                XmlAttribute versionAttr = document.CreateAttribute("version");
                versionAttr.InnerText = version;

                server.Attributes.Append(ipAttr);
                server.Attributes.Append(portAttr);
                server.Attributes.Append(versionAttr);

                servers.AppendChild(server);

                if (!Directory.Exists(Constants.AppDataPath))
                    Directory.CreateDirectory(Constants.AppDataPath);

                document.Save(location);
            }
        }
    }
}
