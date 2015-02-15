using System;
using System.Text;
using SharpTibiaProxy.Domain;

namespace SharpTibiaProxy.Network
{
    public abstract class NetworkMessage
    {
        public const int NETWORK_MESSAGE_SIZE = 16394;
        private static readonly Encoding DefaultEnconding = Encoding.GetEncoding(1252);

        protected int m_size;
        protected int m_writePosition;
        protected int m_readPosition;
        protected readonly byte[] m_buffer;

        protected bool m_encrypted;

        protected NetworkMessage(int bufferSize = NETWORK_MESSAGE_SIZE)
        {
            m_buffer = new byte[bufferSize];
        }

        protected NetworkMessage(byte[] buffer, int size = 0, int readPosition = 0, int writePosition = 0)
        {
            m_buffer = buffer;
            m_size = size;
            m_readPosition = readPosition;
            m_writePosition = writePosition;
        }

        #region Write

        protected void CanWrite(int count, int position)
        {
            if (m_encrypted)
                throw new NetworkException("Can't write to an encrypted message.");

            if (position + count > m_buffer.Length)
                throw new NetworkException("Insufficient buffer size.");
        }

        /// <summary>
        /// Write a byte array to the message.
        /// </summary>
        /// <param name="value">Value to be written.</param>
        public void Write(byte[] value)
        {
            WriteAt(value, m_writePosition);
            m_writePosition += value.Length;
        }

        /// <summary>
        /// Write a byte array to the message at the specified position.
        /// </summary>
        /// <param name="value">Value to be written.</param>
        /// <param name="position">The position.</param>
        public void WriteAt(byte[] value, int position)
        {
            if (value == null)
                throw new NetworkException("Invalid value.");

            CanWrite(value.Length, position);

            Array.Copy(value, 0, m_buffer, position, value.Length);

            var newPosition = value.Length + position;

            if (m_size < newPosition)
                m_size = newPosition;
        }

        /// <summary>
        /// Write a byte to the message
        /// </summary>
        /// <param name="value">Value to be written.</param>
        public void WriteByte(byte value)
        {
            Write(new[] {value});
        }

        /// <summary>
        /// Write a ushort to the message.
        /// </summary>
        /// <param name="value">Value to be written.</param>
        public void WriteUShort(ushort value)
        {
            Write(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Write a short to the message.
        /// </summary>
        /// <param name="value">Value to be written.</param>
        public void WriteShort(short value)
        {
            Write(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Write a int to the message.
        /// </summary>
        /// <param name="value">Value to be written.</param>
        public void WriteInt(int value)
        {
            Write(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Write a uint to the message.
        /// </summary>
        /// <param name="value">Value to be written.</param>
        public void WriteUInt(uint value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void WriteString(string value)
        {
            WriteUShort((ushort)value.Length);
            Write(DefaultEnconding.GetBytes(value));
        }

        public void WriteLocation(Location location)
        {
            WriteUShort((ushort)location.X);
            WriteUShort((ushort)location.Y);
            WriteByte((byte)location.Z);
        }

        #endregion

        #region Read

        /// <summary>
        /// Checks whether it is possible to read the amount of bytes.
        /// </summary>
        /// <param name="count">Number of bytes to be checked.</param>
        protected void CanRead(int count)
        {
            CanRead(count, m_readPosition);
        }

        /// <summary>
        /// Checks whether it is possible to read the amount of bytes.
        /// </summary>
        /// <param name="count">Number of bytes to be checked.</param>
        /// <param name="position">The position to read.</param>
        protected void CanRead(int count, int position)
        {
            if (m_encrypted)
                throw new NetworkException("Can't read from an encrypted message.");

            if (position + count > m_size)
                throw new NetworkException("Insufficient message size.");
        }

        /// <summary>
        /// Read one byte.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        public byte ReadByte()
        {
            CanRead(1);
            return m_buffer[m_readPosition++];
        }

        /// <summary>
        /// Reads the specified number of bytes.
        /// </summary>
        /// <param name="count">The amount to be read.</param>
        /// <returns>Returns the value read.</returns>
        public byte[] ReadBytes(int count)
        {
            CanRead(count);

            var temp = new byte[count];
            Array.Copy(m_buffer, m_readPosition, temp, 0, count);
            m_readPosition += count;

            return temp;
        }

        /// <summary>
        /// Read a short integer.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        public short ReadShort()
        {
            CanRead(2);
            var value = BitConverter.ToInt16(m_buffer, m_readPosition);
            m_readPosition += 2;
            return value;
        }

        /// <summary>
        ///Read an unsigned short integer.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        public ushort ReadUShort()
        {
            CanRead(2);
            var value = BitConverter.ToUInt16(m_buffer, m_readPosition);
            m_readPosition += 2;
            return value;
        }

        /// <summary>
        /// Read an integer.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        public int ReadInt()
        {
            CanRead(4);
            var value = BitConverter.ToInt32(m_buffer, m_readPosition);
            m_readPosition += 4;
            return value;
        }

        /// <summary>
        /// Read an unsigned integer.
        /// </summary>
        /// <returns>Returns the value read.</returns>
        public uint ReadUInt()
        {
            CanRead(4);
            var value = BitConverter.ToUInt32(m_buffer, m_readPosition);
            m_readPosition += 4;
            return value;
        }

        public ulong ReadULong()
        {
            CanRead(8);
            var value = BitConverter.ToUInt64(m_buffer, m_readPosition);
            m_readPosition += 8;
            return value;
        }

        public string ReadString()
        {
            return DefaultEnconding.GetString(this.ReadBytes((int)this.ReadUShort()));
        }

        public Location ReadLocation()
        {
            return new Location(ReadUShort(), ReadUShort(), ReadByte());
        }

        public bool ReadBool()
        {
            return ReadByte() > 0;
        }

        public Outfit ReadOutfit()
        {
            var lookType = ReadUShort();
            if (lookType != 0)
                return new Outfit(lookType, ReadByte(), ReadByte(), ReadByte(),
                    ReadByte(), ReadByte(), ReadUShort());
            else
                return new Outfit(lookType, ReadUShort(), ReadUShort());
        }

        #endregion

        #region Peek

        public ushort PeekUShort()
        {
            return BitConverter.ToUInt16(m_buffer, m_readPosition);
        }

        #endregion

        /// <summary>
        /// Reset to the original positions.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// The message size.
        /// </summary>
        public int Size
        {
            get { return m_size; }
            set
            {
                if(value > m_buffer.Length)
                    throw new NetworkException("The NetworkMessage size can't be larger than the buffer size.");

                m_size = value;
            }
        }

        /// <summary>
        /// The buffer write position.
        /// </summary>
        public int WritePosition
        {
            get { return m_writePosition; }
            set
            {
                if (value > m_buffer.Length)
                    throw new NetworkException("The NetworkMessage write position can't be larger than the buffer size.");

                m_writePosition = value;
            }
        }

        /// <summary>
        /// The message read position.
        /// </summary>
        public int ReadPosition
        {
            get { return m_readPosition; }
            set
            {
                if (value > m_buffer.Length)
                    throw new NetworkException("The read position can't be larger than the buffer size.");

                m_readPosition = value;
            }
        }

        /// <summary>
        /// The value indicating whether the message is encrypted or not.
        /// </summary>
        public bool Encrypted
        {
            get { return m_encrypted; }
            set { m_encrypted = value; }
        }

        /// <summary>
        /// The buffer of the message.
        /// </summary>
        public byte[] Buffer
        {
            get { return m_buffer; }
        }
    }
}
