namespace CDX.Maps.Tiled
{
    using CDX.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ITiledMapTile
    {
        int Id { get; set; }
        BlendMode BlendMode { get; set; }
        IImageRegion ImageRegion { get; set; }
        float OffsetX { get; set; }
        float OffsetY { get; set; }
    }
}
