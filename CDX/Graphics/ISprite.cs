namespace CDX.Graphics
{
    using CDX.App;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ISprite : IImageRegion
    {
        int OffsetX { get; }
        int OffsetY { get; }

        bool Update(FrameTime time);
    }
}
