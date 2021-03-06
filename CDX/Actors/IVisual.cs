namespace CDX.Actors
{
    using CDX.App;
    using CDX.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IVisual
    {
        float PosX { get; }
        float PosY { get; }
        int Direction { get; }
        string Animation { get; }
        IEnumerable<ISprite> CurrentSprites { get; }
        bool HasAnimationFinished { get; }
        bool IsActiveFrame { get; }

        bool Update(FrameTime time);

        void SetPosition(float x, float y);
        void SetDirection(int direction);
        void SetAnimation(string animation);
    }
}
