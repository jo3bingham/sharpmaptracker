using System;

namespace SharpTibiaProxy.Network
{
    public class Adler
    {
        public const uint ADLER_BASE = 0xFFF1;
        public const uint ADLER_START = 0x0001;

        public static uint Generate(NetworkMessage msg, bool write = false)
        {
            if (msg.Size - 6 <= 0)
                throw new ArgumentException("msg.Size");

            var unSum1 = ADLER_START & 0xFFFF;
            var unSum2 = (ADLER_START >> 16) & 0xFFFF;

            for (var i = 6; i < msg.Size; i++)
            {
                unSum1 = (unSum1 + msg.Buffer[i]) % ADLER_BASE;
                unSum2 = (unSum1 + unSum2) % ADLER_BASE;
            }

            var result = (unSum2 << 16) + unSum1;

            if (write)
            {
                if (msg is OutMessage)
                    ((OutMessage)msg).WriteChecksum(result);
                else
                    throw new Exception("Check sum can only be writen to a OutMessage");
            }

            return result;
        }
    }
}
