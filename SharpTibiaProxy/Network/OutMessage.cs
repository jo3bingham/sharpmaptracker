using System;

namespace SharpTibiaProxy.Network
{
    public class OutMessage : NetworkMessage
    {
        public OutMessage()
        {
            m_writePosition = 8;
            m_size = 8;
        }

        public override void Reset()
        {
            m_readPosition = 0;
            m_writePosition = 8;
            m_size = 8;

            m_encrypted = false;
        }

        /// <summary>
        /// Write the message head.
        /// </summary>
        public void WriteHead()
        {
            Array.Copy(BitConverter.GetBytes((ushort) m_size - 2), m_buffer, 2);
        }

        /// <summary>
        /// Write the message internal head.
        /// </summary>
        public void WriteInternalHead()
        {
            Array.Copy(BitConverter.GetBytes((ushort)m_size - 8), 0, m_buffer, 6, 2);
        }

        /// <summary>
        /// Write the message checksum.
        /// </summary>
        /// <param name="value">The value to be written.</param>
        public void WriteChecksum(uint value)
        {
            Array.Copy(BitConverter.GetBytes(value), 0, m_buffer, 2, 4);
        }
    }
}
