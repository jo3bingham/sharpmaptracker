using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using SharpTibiaProxy.Domain;

namespace SharpTibiaProxy.Util
{
    public partial class ClientChooser : Form
    {
        private static ClientChooser newClientChooser;
        private static Client client;

        private ClientChooserOptions options;

        public ClientChooser()
        {
            InitializeComponent();
            client = null;
            //Tibia.Version.Set(Tibia.Version.CurrentVersionString);
        }

        /// <summary>
        /// Opens a box to pick a client.
        /// </summary>
        /// <returns></returns>
        public static Client ShowBox()
        {
            return ShowBox(new ClientChooserOptions());
        }
        /// <summary>
        /// Open a box to pick a client with the desired options.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Client ShowBox(ClientChooserOptions options)
        {
            List<Client> clients = null;
            if (options.LookUpClients)
            {
                clients = Client.GetClients(options.Version, options.OfflineOnly);
            }
            if (options.Smart &&
                options.LookUpClients &&
                !options.ShowOTOption &&
                clients != null &&
                clients.Count == 1)
            {
                return clients[0];
            }
            else
            {
                newClientChooser = new ClientChooser();
                newClientChooser.Text = String.IsNullOrEmpty(options.Title) ? "Choose a client." : options.Title;

                if (options.LookUpClients)
                {
                    foreach (Client c in clients)
                    {
                        newClientChooser.uxClients.Items.Add(c);
                    }
                }

                if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Tibia\tibia.exe")))
                {
                    newClientChooser.uxClients.Items.Add(ClientChooserBase.NewClientDefaultText);
                }

                foreach (ClientPathInfo cpi in
                    ClientChooserBase.GetClientPaths(options.SavedClientPathsLocation))
                {
                    newClientChooser.uxClients.Items.Add(cpi);
                }

                newClientChooser.uxClients.Items.Add(ClientChooserBase.NewClientCustomText);
                newClientChooser.uxClients.SelectedIndex = 0;

                foreach (var ot in ClientChooserBase.GetServers(options.SavedServersLocation))
                {
                    newClientChooser.uxLoginServer.Items.Add(ot.LoginServer.Server + ":" + ot.LoginServer.Port);
                }

                if (newClientChooser.uxLoginServer.Items.Count > 0)
                    newClientChooser.uxLoginServer.SelectedIndex = 0;

                if (options.ShowOTOption)
                {
                    newClientChooser.Height = 109;
                    newClientChooser.uxUseOT.Checked = options.UseOT;
                    newClientChooser.SetOTState();
                    newClientChooser.uxLoginServer.Text = options.Server + ":" + options.Port.ToString();
                }
                else
                {
                    newClientChooser.Height = 54;
                }

                newClientChooser.options = options;
                newClientChooser.TopMost = options.Topmost;
                newClientChooser.ShowDialog();
                return client;
            }
        }

        private void uxChoose_Click(object sender, EventArgs e)
        {
            ChooseClient();
        }

        private void uxUseOT_CheckedChanged(object sender, EventArgs e)
        {
            SetOTState();
            if (uxUseOT.Checked)
                uxLoginServer.Focus();
        }

        public void SetOTState()
        {
            newClientChooser.uxLoginServer.Enabled = uxUseOT.Checked;
            newClientChooser.uxLoginServerLabel.Enabled = uxUseOT.Checked;
        }

        private void ChooseClient()
        {
            options.UseOT = uxUseOT.Checked;
            LoginServer ls = null;

            if (options.UseOT)
            {
                string[] split = uxLoginServer.Text.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 2)
                    ls = new LoginServer(split[0].Trim(), short.Parse(split[1]));
                else
                    ls = new LoginServer(uxLoginServer.Text.Trim(), 7171);
            }

            client = ClientChooserBase.ChooseClient(options, uxClients.SelectedItem, ls);
            newClientChooser.Dispose();
        }

        private void CommonKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ChooseClient();
            }
        }
    }
}