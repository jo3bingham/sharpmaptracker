using System.Collections.Generic;

namespace SharpTibiaProxy.Util
{
    /// <summary>
    /// Options for the ClientChooser class
    /// </summary>
    public class ClientChooserOptions
    {
        /// <summary>
        /// If true, will not open a box if there is only one 
        /// client; just returns that client.
        /// </summary>
        public bool Smart = true;

        /// <summary>
        /// Use a custom title for the client chooser.
        /// </summary>
        public string Title = string.Empty;

        /// <summary>
        /// Show the open tibia server section
        /// </summary>
        public bool ShowOTOption = true;

        /// <summary>
        /// Default value for the Use OT checkbox
        /// </summary>
        public bool UseOT = false;

        /// <summary>
        /// Default value for the server box
        /// </summary>
        public string Server = null;

        /// <summary>
        /// Default value for the port box
        /// </summary>
        public short Port = 7171;

        /// <summary>
        /// Get already running clients and in default locations.
        /// </summary>
        public bool LookUpClients = true;

        /// <summary>
        /// Saves the selected client's path
        /// </summary>
        public bool SaveClientPath = true;

        /// <summary>
        /// Shows only offline clients
        /// </summary>
        public bool OfflineOnly = false;

        /// <summary>
        /// If the client chooser is topmost window
        /// </summary>
        public bool Topmost = true;

        /// <summary>
        /// Command-line arguments for client
        /// </summary>
        public string Arguments = "";

        /// <summary>
        /// Location of where to read/save the selected client's path. Default: %APPDATA%\SharpTibiaProxy\clientPaths.xml.
        /// </summary>
        public string SavedClientPathsLocation = System.IO.Path.Combine(Constants.AppDataPath, @"clientPaths.xml");

        /// <summary>
        /// Location of where to read/save the selected OT server. Default: %APPDATA%\SharpTibiaProxy\servers.xml.
        /// </summary>
        public string SavedServersLocation = System.IO.Path.Combine(Constants.AppDataPath, @"servers.xml");

        /// <summary>
        /// Version of the clients to look for
        /// </summary>
        public string Version = null;

        /// <summary>
        /// Run client in a single processor
        /// </summary>
        public bool UseSingleProcessor = false;

        public ClientChooserOptions()
        {
            clientPaths = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> clientPaths;
    }
}
