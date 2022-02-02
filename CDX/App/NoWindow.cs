namespace CDX.App
{
    using CDX.Audio;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NoWindow : CDXWindow
    {
        public NoWindow(Window window)
            : base(window)
        {

        }
        public override uint WindowID => 0xFFFFFFFF;

        public override IAudio Audio => NoAudio.Instance;

        public override void Close()
        {
        }

        public override void Hide()
        {
        }

        public override void SetAlwaysOnTop(bool alwaysOnTop)
        {
        }

        public override void SetBorderless(bool borderless)
        {
        }

        public override void SetFullScreen(bool fullScreen)
        {
        }

        public override void SetPosition(int x, int y)
        {
        }

        public override void SetResizable(bool resizable)
        {
        }

        public override void SetSize(int width, int height)
        {
        }
        public override void SetMousePosition(int x, int y)
        {

        }
        public override void SetTitle(string title)
        {
        }

        public override void SetVisible(bool visible)
        {
        }

        public override void Show()
        {
        }
    }
}
