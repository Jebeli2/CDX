namespace CDX.Maps.Tiled
{
    using CDX.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class StaticTiledMapTile : ITiledMapTile
    {
        private int id;
        private BlendMode blendMode;
        private IImageRegion imageRegion;
        private float offsetX;
        private float offsetY;

        public StaticTiledMapTile(IImageRegion imageRegion)
        {
            this.imageRegion = imageRegion;
            blendMode = BlendMode.Blend;
        }

        public int Id { get => id; set => id = value; }
        public BlendMode BlendMode { get => blendMode; set => blendMode = value; }
        public IImageRegion ImageRegion { get => imageRegion; set => imageRegion = value; }
        public float OffsetX { get => offsetX; set => offsetX = value; }
        public float OffsetY { get => offsetY; set => offsetY = value; }
    }
}
