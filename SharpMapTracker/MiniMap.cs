using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpTibiaProxy.Domain;
using OpenTibiaCommons.Domain;
using SharpTibiaProxy;

namespace SharpMapTracker
{
    public partial class MiniMap : UserControl
    {
        private const int PIXEL_FACTOR = 2;
        private const int MINIMAP_SIZE = 192;

        public event EventHandler<MiniMapClickEventArgs> MiniMapClick;

        private int updateOngoing;
        private Location centerLocation;
        private bool highlightMissingTiles;
        private Bitmap bitmap;

        public MiniMap()
        {
            bitmap = new Bitmap(MINIMAP_SIZE * PIXEL_FACTOR, MINIMAP_SIZE * PIXEL_FACTOR);

            InitializeComponent();
            MouseMove += new MouseEventHandler(MiniMap_MouseMove);
            MouseClick += new MouseEventHandler(MiniMap_MouseClick);
        }

        public bool HighlightMissingTiles
        {
            get { return highlightMissingTiles; }
            set
            {
                highlightMissingTiles = value;
                Invalidate();
            }
        }

        public OtMap Map { get; set; }

        public Location CenterLocation
        {
            get { return centerLocation; }
            set
            {
                centerLocation = value;
                Invalidate();
            }
        }

        public int Floor
        {
            get { return centerLocation != null ? centerLocation.Z : 0; }
            set
            {
                if (centerLocation == null)
                    return;
                if (value > 15)
                    value = 15;
                else if (value < 0)
                    value = 0;

                centerLocation = new Location(centerLocation.X, centerLocation.Y, value);

                Invalidate();
            }
        }

        void MiniMap_MouseClick(object sender, MouseEventArgs e)
        {
            if (CenterLocation == null)
                return;

            var pos = LocalToGlobal(e.X, e.Y);

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                CenterLocation = pos;
                Invalidate();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MiniMapClick.Raise(this, new MiniMapClickEventArgs { Location = pos });
            }
        }

        void MiniMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (CenterLocation == null)
                return;

            var pos = LocalToGlobal(e.X, e.Y);
            coorLabel.Text = String.Format("[{0}, {1}, {2}]", pos.X, pos.Y, CenterLocation.Z);
        }

        protected Location LocalToGlobal(int x, int y)
        {
            int xoffset = centerLocation.X - MINIMAP_SIZE / 2;
            int yoffset = centerLocation.Y - MINIMAP_SIZE / 2;

            return new Location(((x * MINIMAP_SIZE) / Width) + xoffset, ((y * MINIMAP_SIZE) / Height) + yoffset, CenterLocation.Z);
        }

        public void BeginUpdate()
        {
            updateOngoing = updateOngoing + 1;
        }

        public void EndUpdate()
        {
            if (updateOngoing > 0)
            {
                updateOngoing = updateOngoing - 1;
                if (updateOngoing == 0)
                    Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (CenterLocation != null && Map != null && updateOngoing == 0)
            {
                FastBitmap processor = new FastBitmap(bitmap);

                processor.LockImage();

                int xoffset = CenterLocation.X - MINIMAP_SIZE / 2;
                int yoffset = CenterLocation.Y - MINIMAP_SIZE / 2;

                for (int x = 0; x < MINIMAP_SIZE; x++)
                {
                    for (int y = 0; y < MINIMAP_SIZE; y++)
                    {

                        var color = Color.Black;

                        var tile = Map.GetTile(new Location(x + xoffset, y + yoffset, CenterLocation.Z));
                        if (tile != null)
                            color = tile.MapColor;
                        else if (highlightMissingTiles)
                            color = Color.Fuchsia;

                        for (int px = 0; px < PIXEL_FACTOR; px++)
                        {
                            for (int py = 0; py < PIXEL_FACTOR; py++)
                            {
                                processor.SetPixel(x * PIXEL_FACTOR + px, y * PIXEL_FACTOR + py, color);
                            }
                        }
                    }
                }

                processor.UnlockImage();

                e.Graphics.DrawImage(bitmap, 0, 0, Width, Height);

                var centerX = Width / 2;
                var centerY = Height / 2;

                var pen = new Pen(Color.White, 2);

                e.Graphics.DrawLine(pen, centerX - 4, centerY, centerX + 4, centerY);
                e.Graphics.DrawLine(pen, centerX, centerY - 4, centerX, centerY + 4);
            }
            else
                base.OnPaint(e);
        }
    }

    public class MiniMapClickEventArgs : EventArgs
    {
        public Location Location { get; set; }
    }

}
