namespace CDX.Maps
{
    using CDX.App;
    using CDX.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMapRenderer : IMapCamera
    {
        bool ShowCollision { get; set; }
        void PrepareMap(Map map);
        void Update(FrameTime time, Map map);
        void Render(IGraphics graphics, FrameTime time, Map map, IList<ISortableSprite> front, IList<ISortableSprite> back);
        void ShiftCam(int dX, int dY);
        void SetCam(float x, float y);
    }
}
