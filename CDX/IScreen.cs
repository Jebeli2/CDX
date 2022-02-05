namespace CDX
{
    using CDX.App;
    using CDX.Input;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IScreen : IMouseListener, IKeyboardListener
    {
        void Show();
        void Hide();
        void Update(FrameTime time);
        void Render(IGraphics graphics, FrameTime time);
        void Pause();
        void Resume();
        void Resized(int width, int height);
    }
}
