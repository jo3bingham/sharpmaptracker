using System;

namespace SharpTibiaProxy.Network
{
    public static class Xtea
    {
        private const uint DELTA = 0x9E3779B9;

        /// <summary>
        /// Encrypts the message.
        /// </summary>
        /// <param name="msg">The message to be encrypted.</param>
        /// <param name="key">The key to be used.</param>
        public unsafe static void Encrypt(OutMessage msg, uint[] key)
        {
            if (msg == null)
                throw new ArgumentNullException("msg");

            if (key == null)
                throw new ArgumentNullException("key");

            if(msg.Encrypted)
                throw new Exception("Can't encrypt a encrypted message.");

            var msgSize = msg.Size - 6;

            var pad = msgSize % 8;
            if (pad > 0)
            {
                msgSize += (8 - pad);
                msg.Size = 6 + msgSize;
            }

            fixed (byte* bufferPtr = msg.Buffer)
            {
                var words = (uint*)(bufferPtr + 6);

                for (var pos = 0; pos < msgSize / 4; pos += 2)
                {
                    uint sum = 0;
                    uint count = 32;

                    while (count-- > 0)
                    {
                        words[pos] += (words[pos+1] << 4 ^ words[pos+1] >> 5) + words[pos+1] ^ sum
                            + key[sum & 3];
                        sum += DELTA;
                        words[pos+1] += (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ sum
                            + key[sum >> 11 & 3];
                    }
                }
            }

            msg.Encrypted = true;
        }

        /// <summary>
        /// Decrypts the message
        /// </summary>
        /// <param name="msg">The message to be dencrypted.</param>
        /// <param name="key">The key to be used.</param>
        public unsafe static void Decrypt(InMessage msg, uint[] key)
        {
            if (msg == null)
                throw new Exception("Null message.");

            if (key == null)
                throw new Exception("Null key.");

            if (!msg.Encrypted)
                throw new Exception("Can't dencrypt a dencrypted message.");

            var msgSize = msg.Size - 6;

            if (msg.Size <= 6 || msgSize % 8 > 0)
                throw new Exception("Wrong message size.");

            fixed (byte* bufferPtr = msg.Buffer)
            {
                var words = (uint*)(bufferPtr + 6);
                
                for (var pos = 0; pos < msgSize / 4; pos += 2)
                {
                    var count = 32;
                    var sum = 0xC6EF3720;

                    while (count-- > 0)
                    {
                        words[pos + 1] -= (words[pos] << 4 ^ words[pos] >> 5) + words[pos] ^ sum
                            + key[sum >> 11 & 3];
                        sum -= DELTA;
                        words[pos] -= (words[pos + 1] << 4 ^ words[pos + 1] >> 5) + words[pos + 1] ^ sum
                            + key[sum & 3];
                    }
                }
            }

            msg.Size = BitConverter.ToUInt16(msg.Buffer, 6) + 2 + 6;
            msg.Encrypted = false;
        }
    }
}
