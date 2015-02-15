using System;

namespace SharpTibiaProxy.Network
{
    public class InMessage : NetworkMessage
    {
        public InMessage(byte[] buffer, int size = 0, int readPosition = 0, int writePosition = 0)
            : base(buffer, size, readPosition, writePosition)
        {
        }

        public InMessage()
        {
        }

        public override void Reset()
        {
            m_size = 0;
            m_readPosition = 0;
            m_writePosition = 0;

            m_encrypted = true;
        }

        /// <summary>
        /// Read the message checksum.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        public uint ReadChecksum()
        {
            return BitConverter.ToUInt32(m_buffer, 2);
        }

        /// <summary>
        /// Read the message head.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        public ushort ReadHead()
        {
            return BitConverter.ToUInt16(m_buffer, 0);
        }

        /// <summary>
        /// Read the internal head from the message.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        public ushort ReadInternalHead()
        {
            CanRead(2, 6);
            return BitConverter.ToUInt16(m_buffer, 6);
        }
    }
}
