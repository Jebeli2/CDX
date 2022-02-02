namespace CDX.App
{
    using CDX.Audio;
    using CDX.Graphics;
    using CDX.Input;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CDXApplet : IMouseListener, IKeyboardListener, ITouchFingerListener, IControllerListener
    {
        private CDXWindow? cdx;
        private int width;
        private int height;
        private bool enabled = true;
        private bool installed;

        public int Width => width;
        public int Height => height;

        internal bool Initialized { get; set; }
        internal bool Shown { get; set; }
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public bool Installed
        {
            get => installed;
            internal set => installed = value;
        }

        public IGraphics Graphics => cdx?.Graphics ?? NoGraphics.Instance;
        public IAudio Audio => cdx?.Audio ?? NoAudio.Instance;

        public IImage? LoadImage(string fileName)
        {
            return cdx?.Graphics?.LoadImage(fileName);
        }


        public virtual void OnPaint(PaintEventArgs e)
        {

        }

        public virtual void OnUpdate(UpdateEventArgs e)
        {

        }
        internal void InternalOnSizeChanged(int width, int height)
        {
            this.width = width;
            this.height = height;
            OnSizeChanged(width, height);
        }

        public virtual void OnSizeChanged(int width, int height)
        {
        }

        internal void InternalOnLoad(LoadEventArgs e)
        {
            cdx = e.CDXWindow;
            width = e.Graphics.Width;
            height = e.Graphics.Height;
            OnLoad(e);
        }
        public virtual void OnLoad(LoadEventArgs e)
        {

        }

        internal void InternalOnShown(EventArgs e)
        {
            if (!Shown)
            {
                OnShown(e);
                Shown = true;
            }
        }
        public virtual void OnShown(EventArgs e)
        {

        }
        public virtual void OnHidden(EventArgs e)
        {

        }

        public virtual void OnClose(EventArgs e)
        {

        }

        public virtual void OnKeyDown(KeyEventArgs e)
        {

        }

        public virtual void OnKeyUp(KeyEventArgs e)
        {
        }

        public virtual void OnTextInput(TextInputEventArgs e)
        {

        }
        public virtual void OnControllerButtonDown(ControllerButtonEventArgs e)
        {

        }

        public virtual void OnControllerButtonUp(ControllerButtonEventArgs e)
        {

        }

        public virtual void OnMouseButtonDown(MouseButtonEventArgs e)
        {

        }

        public virtual void OnMouseButtonUp(MouseButtonEventArgs e)
        {

        }

        public virtual void OnControllerAxis(ControllerAxisEventArgs e)
        {

        }

        public virtual void OnMouseMoved(MouseMotionEventArgs e)
        {

        }

        public virtual void OnMouseWheel(MouseWheelEventArgs e)
        {

        }

        public virtual void OnTouchFingerDown(TouchFingerEventArgs e)
        {

        }
        public virtual void OnTouchFingerUp(TouchFingerEventArgs e)
        {

        }

        public virtual void OnTouchFingerMotion(TouchFingerEventArgs e)
        {

        }


    }
}
