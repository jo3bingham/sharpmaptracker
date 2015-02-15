using System;

namespace SharpTibiaProxy.Util
{
    /// <summary>
    /// Helper methods for reading memory.
    /// </summary>
    public static class Memory
    {
        /// <summary>
        /// Read a specified number of bytes from a process.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="bytesToRead"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(IntPtr handle, long address, uint bytesToRead)
        {
            IntPtr ptrBytesRead;
            byte[] buffer = new byte[bytesToRead];

            WinApi.ReadProcessMemory(handle, new IntPtr(address), buffer, bytesToRead, out ptrBytesRead);

            return buffer;
        }

        /// <summary>
        /// Read a byte from memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static byte ReadByte(IntPtr handle, long address)
        {
            return ReadBytes(handle, address, 1)[0];
        }

        /// <summary>
        /// Read a short from memory (16-bits).
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static short ReadInt16(IntPtr handle, long address)
        {
            return BitConverter.ToInt16(ReadBytes(handle, address, 2), 0);
        }

        /// <summary>
        /// Read a ushort from memory (16-bits).
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static ushort ReadUInt16(IntPtr handle, long address)
        {
            return BitConverter.ToUInt16(ReadBytes(handle, address, 2), 0);
        }

        [Obsolete("Please use ReadInt16")]
        public static short ReadShort(IntPtr handle, long address)
        {
            return BitConverter.ToInt16(ReadBytes(handle, address, 2), 0);
        }

        /// <summary>
        /// Read an integer from the process (32-bits)
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static int ReadInt32(IntPtr handle, long address)
        {
            return BitConverter.ToInt32(ReadBytes(handle, address, 4), 0);
        }

        /// <summary>
        /// Read an uinteger from the process (32-bits)
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static uint ReadUInt32(IntPtr handle, long address)
        {
            return BitConverter.ToUInt32(ReadBytes(handle, address, 4), 0);
        }

        /// <summary>
        /// Read an unsigned long from the process (64-bits)
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static ulong ReadUInt64(IntPtr handle, long address)
        {
            return BitConverter.ToUInt64(ReadBytes(handle, address, 8), 0);
        }

        [Obsolete("Please use ReadInt32.")]
        public static int ReadInt(IntPtr handle, long address)
        {
            return BitConverter.ToInt32(ReadBytes(handle, address, 4), 0);
        }

        /// <summary>
        /// Read a 32-bit double from the process
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static double ReadDouble(IntPtr handle, long address)
        {
            return BitConverter.ToDouble(ReadBytes(handle, address, 8), 0);
        }

        /// <summary>
        /// Read a string from memmory. Splits at 00 and returns first section to avoid junk. Uses default length of 255. Use ReadString(IntPtr handle, long address, int length) to read longer strings, such as the RSA key.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string ReadString(IntPtr handle, long address)
        {
            return ReadString(handle, address, 0);
        }

        /// <summary>
        /// Read a string from memmory. Splits at 00 and returns first section to avoid junk.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="length">the length of the bytes to read</param>
        /// <returns></returns>
        public static string ReadString(IntPtr handle, long address, uint length)
        {
            if (length > 0)
            {
                byte[] buffer;
                buffer = ReadBytes(handle, address, length);
                return System.Text.ASCIIEncoding.Default.GetString(buffer).Split(new Char())[0];
            }
            else
            {
                string s = "";
                byte temp = ReadByte(handle, address++);
                while (temp != 0)
                {
                    s += (char)temp;
                    temp = ReadByte(handle, address++);
                }
                return s;
            }
        }

        /// <summary>
        /// Write a specified number of bytes to a process.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="bytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool WriteBytes(IntPtr handle, long address, byte[] bytes, uint length)
        {
            IntPtr bytesWritten;

            // Write to memory
            int result = WinApi.WriteProcessMemory(handle, new IntPtr(address), bytes, length, out bytesWritten);

            return result != 0;
        }

        /// <summary>
        /// Write an integer (32-bits) to memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteInt32(IntPtr handle, long address, int value)
        {
            return WriteBytes(handle, address, BitConverter.GetBytes(value), 4);
        }

        /// <summary>
        /// Write an uinteger (32-bits) to memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteUInt32(IntPtr handle, long address, uint value)
        {
            return WriteBytes(handle, address, BitConverter.GetBytes(value), 4);
        }

        /// <summary>
        /// Write an unsigned long (64-bits) to memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteUInt64(IntPtr handle, long address, ulong value)
        {
            return WriteBytes(handle, address, BitConverter.GetBytes(value), 8);
        }

        /// <summary>
        /// Write an integer (16-bits) to memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteInt16(IntPtr handle, long address, short value)
        {
            return WriteBytes(handle, address, BitConverter.GetBytes(value), 2);
        }

        /// <summary>
        /// Write an uinteger (16-bits) to memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteUInt16(IntPtr handle, long address, ushort value)
        {
            return WriteBytes(handle, address, BitConverter.GetBytes(value), 2);
        }

        [Obsolete("Please use WriteInt32.")]
        public static bool WriteInt(IntPtr handle, long address, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes, 4);
        }

        /// <summary>
        /// Write a double value to memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteDouble(IntPtr handle, long address, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            return WriteBytes(handle, address, bytes, 8);
        }

        /// <summary>
        /// Write a byte to memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool WriteByte(IntPtr handle, long address, byte value)
        {
            return WriteBytes(handle, address, new byte[] { value }, 1);
        }

        /// <summary>
        /// Write a string to memory without using econding.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool WriteStringNoEncoding(IntPtr handle, long address, string str)
        {
            str += '\0';
            byte[] bytes = str.ToByteArray();
            return WriteBytes(handle, address, bytes, (uint)bytes.Length);
        }

        /// <summary>
        /// Write a string to memory.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool WriteString(IntPtr handle, long address, string str)
        {
            str += '\0';
            byte[] bytes = System.Text.ASCIIEncoding.Default.GetBytes(str);
            return WriteBytes(handle, address, bytes, (uint)bytes.Length);
        }

        /// <summary>
        /// Set the RSA key. Different from WriteString because must overcome protection.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="address"></param>
        /// <param name="newKey"></param>
        /// <returns></returns>
        public static bool WriteRSA(IntPtr handle, long address, string newKey)
        {
            IntPtr bytesWritten;
            int result;
            WinApi.MemoryProtection oldProtection = 0;

            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            byte[] bytes = enc.GetBytes(newKey);

            // Make it so we can write to the memory block
            WinApi.VirtualProtectEx(
                handle,
                new IntPtr(address),
                new IntPtr(bytes.Length),
                WinApi.MemoryProtection.ExecuteReadWrite, ref oldProtection);

            // Write to memory
            result = WinApi.WriteProcessMemory(handle, new IntPtr(address), bytes, (uint)bytes.Length, out bytesWritten);

            // Put the protection back on the memory block
            WinApi.VirtualProtectEx(handle, new IntPtr(address), new IntPtr(bytes.Length), oldProtection, ref oldProtection);

            return (result != 0);
        }
    }
}
