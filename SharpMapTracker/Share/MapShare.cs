using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTibiaCommons.Domain;
using System.Threading;
using System.Diagnostics;
using SharpTibiaProxy.Network;
using SharpTibiaProxy.Domain;

namespace SharpMapTracker.Share
{
    public class MapShare
    {
        private Queue<Tile> queue;
        private Connection connection;
        private OutMessage message;
        private Thread thread;

        public MapShare()
        {
            queue = new Queue<Tile>();
            message = new OutMessage();
            connection = new Connection(Constants.MAP_SHARE_HOST, Constants.MAP_SHARE_PORT);
        }

        public void Start()
        {
            lock (queue)
            {
                if (thread != null)
                    return;

                queue.Clear();
                thread = new Thread(Run);
                thread.Start();
            }
        }

        public void Stop()
        {
            lock (queue)
            {
                queue.Clear();
                thread = null;
                Monitor.Pulse(queue);
            }
        }
        public void Add(Tile tile)
        {
            lock (queue)
            {
                if (thread == null)
                    return;

                queue.Enqueue(new Tile(tile));
                Monitor.Pulse(queue);
            }
        }

        private void Run()
        {
            while (thread != null)
            {
                try
                {
                    if (!connection.IsConnected)
                    {
                        connection.TryConnect();

                        if (!connection.IsConnected)
                            Thread.Sleep(2000);
                    }
                    else
                    {
                        Tile tile = null;

                        lock (queue)
                        {
                            if (queue.Count == 0)
                                Monitor.Wait(queue);
                            else
                                tile = queue.Dequeue();
                        }

                        if (tile != null)
                        {
                            SendTile(tile);
                        }
                    }
                }
                catch (Exception e)
                {
                    Trace.WriteLine("[Error] Map share can not complete. Details: " + e.Message);
                }
            }

            try
            {
                if (connection.IsConnected)
                    connection.Disconnect();
            }
            catch (Exception e)
            {
                Trace.WriteLine("[Error] Can't close the connection. Details: " + e.Message);
            }
        }

        private void SendTile(Tile tile)
        {
            message.Reset();
            message.WriteByte(0x01);

            message.WriteLocation(tile.Location);
            message.WriteByte((byte)tile.ThingCount);

            for (int i = 0; i < tile.ThingCount; i++)
            {
                var thing = tile.GetThing(i);

                if (thing is Creature)
                {
                    var cr = thing as Creature;
                    message.WriteByte(0x01);
                    message.WriteUInt(cr.Id);
                    message.WriteString(cr.Name);
                    message.WriteByte((byte)cr.Type);
                }
                else
                {
                    var item = thing as Item;
                    message.WriteByte(0x02);
                    message.WriteUShort((ushort)item.Id);
                    message.WriteByte(item.SubType);
                }
            }

            message.WriteInternalHead();
            Adler.Generate(message, true);
            message.WriteHead();

            connection.Send(message);
        }
    }
}
