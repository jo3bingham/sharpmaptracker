using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SharpTibiaProxy.Domain;
using SharpTibiaProxy.Util;

namespace SharpTibiaProxy.Util
{
    /// <summary>
    /// Interaction logic for ClientChooserWPF.xaml
    /// </summary>
    public partial class ClientChooserWPF : Window
    {
        private static ClientChooserWPF newClientChooser;

        private static Client client;

        private const string LoginServerEnabled = "Enabled";
        private const string LoginServerDisabled = "Disabled";

        private ClientChooserOptions options;

        public ClientChooserWPF()
        {
            InitializeComponent();
            ShowInTaskbar = true;
            uxUseOT.IsExpanded = false;
            uxLoginServerLabel.Content = LoginServerDisabled;
            uxLoginServerLabel.Foreground = Brushes.PaleVioletRed;
            client = null;
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
                newClientChooser = new ClientChooserWPF();
                newClientChooser.Title = String.IsNullOrEmpty(options.Title) ? "Choose a client." : options.Title;

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

                newClientChooser.uxUseOT.IsExpanded = options.UseOT;
                if (options.UseOT)
                {
                    newClientChooser.uxLoginServer.Text = options.Server + ":" + options.Port.ToString();
                }

                newClientChooser.options = options;
                newClientChooser.Topmost = options.Topmost;
                newClientChooser.ShowDialog();
                return client;
            }
        }

        private void uxChoose_Click(object sender, RoutedEventArgs e)
        {
            ChooseClient();
        }

        private void uxUseOT_Expanded(object sender, RoutedEventArgs e)
        {
            uxLoginServerLabel.Content = LoginServerEnabled;
            uxLoginServerLabel.Foreground = Brushes.Green;
            if (uxLoginServer != null)
                uxLoginServer.Focus();
        }

        private void uxUseOT_Collapsed(object sender, RoutedEventArgs e)
        {
            uxLoginServerLabel.Content = LoginServerDisabled;
            uxLoginServerLabel.Foreground = Brushes.PaleVioletRed;
        }

        private void ChooseClient()
        {
            options.UseOT = uxUseOT.IsExpanded;
            LoginServer ls = null;
            if (options.UseOT)
            {
                string[] split = uxLoginServer.Text.Split(new char[] { ':' });
                ls = new LoginServer(split[0], short.Parse(split[1]));
            }
            client = ClientChooserBase.ChooseClient(options, uxClients.SelectedItem, ls);
            newClientChooser.Close();
        }

        private void CommonKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ChooseClient();
        }
    }
}