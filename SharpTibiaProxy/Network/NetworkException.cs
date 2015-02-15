using System;

namespace SharpTibiaProxy.Network
{
    public class NetworkException : Exception
    {
        public NetworkException(String message)
            : base(message)
        {
        }
    }
}
