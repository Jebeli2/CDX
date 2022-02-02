namespace CDX.GUI
{
    using CDX.App;
    using CDX.Input;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class NoGUI : IGUISystem
    {
        public void SetCDXWindow(CDXWindow cdx)
        {

        }
        public void ScreenResized(int width, int height)
        {

        }

        public void Clear()
        {

        }

        public void SetScreen(IScreen? screen)
        {

        }


        public void Update(FrameTime time)
        {

        }
        public void Render(IGraphics gfx, FrameTime time)
        {

        }
        public IScreen? OpenScreen(string? title = null)
        {
            return null;
        }

        public IWindow? OpenWindow(IScreen? screen, int leftEdge = 0, int topEdge = 0, int width = 256, int height = 256, string title = "")
        {
            return null;
        }

        public void CloseWindow(IWindow? window)
        {

        }
        public void ActivateWindow(IWindow? window)
        {

        }
        public void ActivateGadget(IGadget? gadget)
        {

        }
        public IGadget? AddGadget(IWindow? window,
            int leftEdge = 0,
            int topEdge = 0,
            int width = 100,
            int height = 100,
            GadgetFlags flags = GadgetFlags.None,
            GadgetActivation activation = GadgetActivation.Immediate | GadgetActivation.RelVerify,
            string? text = null,
            bool disabled = false,
            Action? clickAction = null,
            int gadgetID = -1)
        {
            return null;
        }

        public void RemGadget(IGadget? gadget)
        {

        }
        public void OnTouchFingerDown(TouchFingerEventArgs e)
        {

        }
        public void OnTouchFingerUp(TouchFingerEventArgs e)
        {

        }

        public void OnTouchFingerMotion(TouchFingerEventArgs e)
        {

        }
        public void OnControllerButtonDown(ControllerButtonEventArgs e)
        {
        }
        public void OnControllerButtonUp(ControllerButtonEventArgs e)
        {
        }
        public void OnControllerAxis(ControllerAxisEventArgs e)
        {

        }

        public void OnMouseButtonDown(MouseButtonEventArgs e)
        {

        }

        public void OnMouseButtonUp(MouseButtonEventArgs e)
        {

        }

        public void OnMouseMoved(MouseMotionEventArgs e)
        {

        }

        public void OnMouseWheel(MouseWheelEventArgs e)
        {

        }

        public void OnKeyDown(KeyEventArgs e)
        {

        }

        public void OnKeyUp(KeyEventArgs e)
        {
        }

        public void OnTextInput(TextInputEventArgs e)
        {

        }

    }
}
