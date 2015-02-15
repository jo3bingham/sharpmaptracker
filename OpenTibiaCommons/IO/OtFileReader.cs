using System.IO;
using System;

namespace OpenTibiaCommons.IO
{
    public class OtFileReader : IDisposable
    {
        protected byte[] buffer;
        protected FileStream fileStream;
        protected BinaryReader reader;
        protected OtFileNode root;

        public OtFileReader(string fileName)
        {
            fileStream = File.Open(fileName, FileMode.Open);
            reader = new BinaryReader(fileStream);

            var version = reader.ReadUInt32();

            if (version > 0)
                throw new Exception("Invalid file version.");

            if (SafeSeek(4))
            {
                root = new OtFileNode { Start = 4 };

                if (reader.ReadByte() != OtFileNode.NODE_START || !ParseNode(root))
                    throw new Exception("Invalid file format.");
            }
            else
            {
                throw new Exception("Invalid file format.");
            }
        }

        public OtFileNode GetRootNode()
        {
            return root;
        }

        public void Close()
        {
            if (fileStream != null)
            {
                fileStream.Close();
            }
        }

        private bool ParseNode(OtFileNode node)
        {
            var currentNode = node;
            int val;

            while (true)
            {
                // read node type
                val = fileStream.ReadByte();
                if (val != -1)
                {
                    
                    currentNode.Type = val;
                    var setPropSize = false;

                    while (true)
                    {
                        // search child and next node
                        val = fileStream.ReadByte();

                        
                        if (val == OtFileNode.NODE_START)
                        {
                            var childNode = new OtFileNode {Start = fileStream.Position};
                            setPropSize = true;

                            currentNode.PropsSize = fileStream.Position - currentNode.Start - 2;
                            currentNode.Child = childNode;

                            if (!ParseNode(childNode))
                                return false;
                        }
                        else if (val == OtFileNode.NODE_END)
                        {
                            if (!setPropSize)
                                currentNode.PropsSize = fileStream.Position - currentNode.Start - 2;

                            val = fileStream.ReadByte();

                            if (val != -1)
                            {
                                if (val == OtFileNode.NODE_START)
                                {
                                    // start next node
                                    var nextNode = new OtFileNode {Start = fileStream.Position};
                                    currentNode.Next = nextNode;
                                    currentNode = nextNode;
                                    break;
                                }

                                if (val == OtFileNode.NODE_END)
                                {
                                    // up 1 level and move 1 position back
                                    // safeTell(pos) && safeSeek(pos)
                                    fileStream.Seek(-1, SeekOrigin.Current);
                                    return true;
                                }
                                
                                // bad format
                                return false;

                            }

                            // end of file?
                            return true;
                        }
                        else if (val == OtFileNode.ESCAPE)
                        {
                            fileStream.ReadByte();
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        private byte[] GetProperties(OtFileNode node, out long size)
        {
            if (buffer == null || buffer.Length < node.PropsSize)
                buffer = new byte[node.PropsSize];

            fileStream.Seek(node.Start + 1, SeekOrigin.Begin);
            fileStream.Read(buffer, 0, (int)node.PropsSize);

            uint j = 0;
            var escaped = false;

            for (uint i = 0; i < node.PropsSize; ++i, ++j)
            {
                if (buffer[i] == OtFileNode.ESCAPE)
                {
                    ++i;
                    buffer[j] = buffer[i];
                    escaped = true;
                }
                else if (escaped)
                {
                    buffer[j] = buffer[i];
                }
            }
            size = j;
            return buffer;
        }

        public OtPropertyReader GetPropertyReader(OtFileNode node)
        {
            long size;
            var buff = GetProperties(node, out size);

            return new OtPropertyReader(new MemoryStream(buff, 0, (int)size));
        }

        protected bool SafeSeek(long pos)
        {
            if (fileStream == null || fileStream.Length < pos)
                return false;

            return fileStream.Seek(pos, SeekOrigin.Begin) == pos;
        }

        public void Dispose()
        {
            Close();
        }
    }
}