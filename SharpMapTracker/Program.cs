using System;
using System.Collections.Generic;
using System.Linq;
using SharpTibiaProxy.Domain;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SharpMapTracker
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (!File.Exists("items.otb"))
            {
                MessageBox.Show("File items.otb not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
