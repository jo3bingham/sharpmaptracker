using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public class LoginServer
    {
        public string Server { get; set; }
        public int Port { get; set; }

        public LoginServer(string server)
            : this(server, 7171) { }

        public LoginServer(string server, int port)
        {
            Server = server;
            Port = port;
        }

        public override string ToString()
        {
            return Server + ":" + Port;
        }
    }
}
