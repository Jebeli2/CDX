namespace CDX.Maps.Tiled
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TiledMapTileLayer : MapLayer
    {
        private int width;
        private int height;
        private int tileWidth;
        private int tileHeight;
        private Cell?[,] cells;

        public TiledMapTileLayer(int width, int height, int tileWidth, int tileHeight)
        {
            this.width = width;
            this.height = height;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            cells = new Cell?[width, height];
        }

        public int Width => width;
        public int Height => height;
        public int TileWidth => tileWidth;
        public int TileHeight => tileHeight;

        public Cell? GetCell(int x, int y)
        {
            if (x < 0) return null;
            if (y < 0) return null;
            if (x >= width) return null;
            if (y >= height) return null;
            return cells[x, y];
        }

        public void SetCell(int x, int y, Cell? cell)
        {
            if (x < 0) return;
            if (y < 0) return;
            if (x >= width) return;
            if (y >= height) return;
            cells[x, y] = cell;
        }

        public class Cell
        {
            private ITiledMapTile? tile;
            private bool flipHorizontally;
            private bool flipVertically;
            private int rotation;

            public ITiledMapTile? Tile
            {
                get => tile;
                set => tile = value;
            }

            public bool FlipHorizontally
            {
                get => flipHorizontally;
                set => flipHorizontally = value;
            }

            public bool FlipVertically
            {
                get => flipVertically;
                set => flipVertically = value;
            }

            public int Rotation
            {
                get => rotation;
                set => rotation = value;
            }
        }
    }
}
