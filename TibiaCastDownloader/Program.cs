using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using NDesk.Options;

namespace TibiaCastDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            int startPage = 1;
            int endPage = 10;
            string outputFolder = @".\";
            bool showHelp = false;

            var optionSet = new OptionSet()
            {
                {"start-page=", "the start page.", (int v) => startPage = v},
                {"end-page=", "the end page.", (int v) => endPage = v},
                {"output-folder=", "the output folder", (string v) => outputFolder = v},
                {"h|help",  "show this message and exit", v => showHelp = v != null}
            };

            try
            {
                optionSet.Parse(args);

                if (showHelp)
                    Console.WriteLine("usage: tibiacast-downloader.exe --start-page=1 --end-page=10 --output-folder=\"C:\\recordings\"");
                else
                    DownloadRecordings(startPage, endPage, outputFolder);
            }
            catch (Exception e)
            {
                Console.Write("tibiacast-downloader: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `tibiacast-downloader --help' for more information.");
            }
        }

        static void DownloadRecordings(int startPage, int endPage, string outputFolder)
        {
            if (!Directory.Exists(outputFolder))
            {
                Console.WriteLine("Output folder does not exists or is not accessible.");
                return;
            }

            string tibiacastClientFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Tibiacast\Tibiacast Client.exe");
            if (!File.Exists(tibiacastClientFileName))
            {
                Console.WriteLine("Tibiacast client not found.");
                return;
            }



            var recordDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Tibiacast\Recordings");

            if (!Directory.Exists(recordDirectory))
            {
                Console.WriteLine("Tibiacast client recording directory not found.");
                return;
            }

            WebClient webClient = new WebClient();
            Process process = Process.GetProcessesByName("Tibiacast Client").FirstOrDefault();

            for (int page = startPage; page <= endPage; page++)
            {
                Console.WriteLine("Downloading page " + page + " content.");
                var pageContent = webClient.DownloadString("https://www.tibiacast.com/recordings?sort=Sort%20by%20date&page=" + page);

                var matches = Regex.Matches(pageContent, @"tibiacast:recording:([\d]+)");
                Console.WriteLine("Found " + matches.Count + " records on page " + page + ".");

                int count = 0;

                foreach (Match match in matches)
                {
                    if (process == null)
                    {
                        Console.WriteLine("Opening Tibiacast Client.");
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.FileName = tibiacastClientFileName;
                        process = Process.Start(startInfo);
                        process.WaitForInputIdle();
                    }

                    var recordId = match.Groups[1].Value;
                    var recordFileName = Path.Combine(recordDirectory, recordId + ".recording");
                    var destinationFileName = Path.Combine(outputFolder, recordId + ".recording");

                    if (File.Exists(destinationFileName))
                    {
                        Console.WriteLine("Skiping record " + recordId + ".");
                        continue;
                    }

                    var timeoutTime = DateTime.Now.AddSeconds(20);
                    Console.WriteLine("Sending record " + recordId + " to Tibiacast Client.");

                    Process.Start(tibiacastClientFileName, "tibiacast:recording:" + recordId);

                    while (timeoutTime > DateTime.Now)
                    {
                        if (File.Exists(recordFileName))
                        {
                            File.Move(recordFileName, destinationFileName);
                            break;
                        }

                        Thread.Sleep(500);
                    }

                    count++;
                }

                if (count > 0)
                {
                    if (process != null && !process.HasExited)
                    {
                        Console.WriteLine("Killing the Tibiacast Client before moving to next page.");
                        process.Kill();
                        process = null;
                    }
                }
            }

            Console.WriteLine("All pages were processed.");

        }
    }
}
