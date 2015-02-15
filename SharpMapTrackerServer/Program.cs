using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using OpenTibiaCommons.Domain;

namespace SharpMapTrackerServer
{
    class Program
    {
        static List<Connection> connections = new List<Connection>();
        static OtItems items = new OtItems();

        static void Main(string[] args)
        {
            items.Load("items.otb");

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, 41567));
            server.Listen(10);

            Console.WriteLine("[Info] Waiting for connections");

            while (true)
            {
                var socket = server.Accept();

                Console.WriteLine("[Info] Connection from " + socket.RemoteEndPoint.ToString());

                if (socket != null)
                {
                    lock (connections)
                    {
                        connections.Add(new Connection(socket, items));
                    }
                }
            }
        }
    }
}
