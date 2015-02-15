using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using OpenTibiaCommons.Domain;
using SharpTibiaProxy.Network;
using SharpTibiaProxy.Domain;
using OpenTibiaCommons;

namespace SharpMapTrackerServer
{
    public class Connection
    {
        private Socket socket;
        private OtMap map;
        private InMessage message;
        private OtItems items;

        public Connection(Socket socket, OtItems items)
        {
            this.socket = socket;
            this.items = items;

            message = new InMessage();
            map = new OtMap(items);

            BeginReceive();
        }

        private void BeginReceive()
        {
            message.Reset();
            socket.BeginReceive(message.Buffer, 0, 2, SocketFlags.None, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var count = socket.EndReceive(ar);

                if (count <= 0)
                {
                    OnConnectionClosed();
                    return;
                }

                message.Size = message.ReadHead() + 2;
                int read = 2;

                while (read < message.Size)
                {
                    count = socket.Receive(message.Buffer, read, message.Size - read, SocketFlags.None);

                    if (count <= 0)
                    {
                        OnConnectionClosed();
                        return;
                    }

                    read += count;
                }

                ParseMessage();
                BeginReceive();
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error] Can't process client message. Details: " + e.Message);
                OnConnectionClosed();
            }
        }

        private void ParseMessage()
        {
            message.ReadPosition = 2;
            message.Encrypted = false;

            if (Adler.Generate(message) != message.ReadUInt())
                return; //discart the message

            message.ReadUShort(); //internal head
            var cmd = message.ReadByte();

            switch (cmd)
            {
                case 0x01:
                    ParseTile();
                    break;
                default:
                    Console.WriteLine("[Error] Unknown packet type " + cmd.ToString("X2"));
                    break;
            }
        }

        private void ParseTile()
        {
            var tile = new OtTile(message.ReadLocation());

            //Console.WriteLine("[Debug] Tile received, location: " + tile.Location);

            var thingCount = message.ReadByte();
            for (int i = 0; i < thingCount; i++)
            {
                var thingType = message.ReadByte();
                if (thingType == 0x01) //Creature
                {
                    var id = message.ReadUInt();
                    var name = message.ReadString();
                    var type = (CreatureType)message.ReadByte();

                    if (type != CreatureType.PLAYER)
                        map.AddCreature(new OtCreature { Id = id, Name = name, Type = type, Location = tile.Location });
                }
                else
                {
                    var id = message.ReadUShort();
                    var subType = message.ReadByte();

                    var itemType = items.GetItemBySpriteId(id);
                    if (itemType != null)
                    {
                        var item = OtItem.Create(itemType);

                        if (item.Type.IsStackable)
                            item.SetAttribute(OtItemAttribute.COUNT, subType);
                        else if (item.Type.Group == OtItemGroup.Splash || item.Type.Group == OtItemGroup.FluidContainer)
                            item.SetAttribute(OtItemAttribute.COUNT, OtConverter.TibiaFluidToOtFluid(subType));

                        tile.AddItem(item);
                    }
                }
            }

            if (map.GetTile(tile.Location) == null)
                map.SetTile(tile);
        }

        private void OnConnectionClosed()
        {
            Console.WriteLine("[Info] Connection closed from " + socket.RemoteEndPoint.ToString() + ".");

            var mapName = "mapdump_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".otbm";

            try
            {
                if (map.Tiles.Count() == 0)
                    return;

                map.Save("maps/" + mapName);
                Console.WriteLine("[Info] Map " + mapName + " successfully saved.");
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error] Can't save map " + mapName + ". Details: " + e.Message);
            }
        }

    }
}
