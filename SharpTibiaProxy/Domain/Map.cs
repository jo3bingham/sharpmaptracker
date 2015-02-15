using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpTibiaProxy.Domain
{
    public class TileEventArgs : EventArgs
    {
        public Tile Tile { get; private set; }
        public TileEventArgs(Tile tile)
        {
            this.Tile = tile;
        }
    }
    public class TileAddedEventArgs : TileEventArgs
    {
        public TileAddedEventArgs(Tile tile) : base(tile) { }
    }
    public class TileUpdatedEventArgs : TileEventArgs
    {
        public TileUpdatedEventArgs(Tile tile) : base(tile) { }
    }
    public class MapUpdatedEventArgs : EventArgs
    {
        public List<Tile> Tiles { get; private set; }
        public MapUpdatedEventArgs(List<Tile> tiles)
        {
            Tiles = tiles;
        }
    }

    public class Map
    {
        private Client client;
        private Tile[,,] tiles;
        private Dictionary<ulong, bool> knowMap;

        public event EventHandler<TileAddedEventArgs> TileAdded;
        public event EventHandler<TileUpdatedEventArgs> TileUpdated;
        public event EventHandler<MapUpdatedEventArgs> Updated;

        public Map(Client client)
        {
            this.client = client;
            tiles = new Tile[18, 14, 8];
            knowMap = new Dictionary<ulong, bool>();
        }

        internal void Clear()
        {
            for (int x = 0; x < 18; x++)
            {
                for (int y = 0; y < 14; y++)
                {
                    for (int z = 0; z < 8; z++)
                    {
                        tiles[x, y, z] = null;
                    }
                }
            }
        }

        public void SetTile(Tile tile)
        {
            var location = tile.Location;
            var oldTile = tiles[location.X % 18, location.Y % 14, location.Z % 8];

            tiles[location.X % 18, location.Y % 14, location.Z % 8] = tile;

            knowMap[tile.Location.ToIndex()] = tile.IsBlocked;

            if (oldTile != null && location.Equals(oldTile.Location))
                OnTileUpdated(tile);
            else
                OnTileAdded(tile);
        }

        public Tile GetTile(Location location)
        {
            return tiles[location.X % 18, location.Y % 14, location.Z % 8];
        }

        protected void OnTileAdded(Tile tile)
        {
            TileAdded.Raise(this, new TileAddedEventArgs(tile));
        }

        protected void OnTileUpdated(Tile tile)
        {
            TileUpdated.Raise(this, new TileUpdatedEventArgs(tile));
        }

        internal void OnMapUpdated(List<Tile> tiles)
        {
            Updated.Raise(this, new MapUpdatedEventArgs(tiles));
        }
    }
}
