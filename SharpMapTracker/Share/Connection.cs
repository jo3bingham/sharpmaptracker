using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SharpTibiaProxy.Network;

namespace SharpMapTracker.Share
{
    public class Connection
    {
        private string host;
        private int port;

        private Socket socket;

        public Connection(string host, int port) 
        {
            this.host = host;
            this.port = port;
        }

        public bool TryConnect()
        {
            try
            {
                Connect();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Connect()
        {
            if (socket != null)
                Disconnect();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(host, port);
            socket.LingerState = new LingerOption(true, 2);
        }

        public void Disconnect()
        {
            if (socket != null && socket.Connected)
            {
                socket.Close();
                socket = null;
            }
        }

        public void Send(OutMessage message)
        {
            socket.Send(message.Buffer, message.Size, SocketFlags.None);
        }

        public bool IsConnected { get { return socket != null && socket.Connected; } }
    }
}
