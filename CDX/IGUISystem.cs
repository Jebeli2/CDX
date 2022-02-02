namespace CDX
{
    using CDX.App;
    using CDX.GUI;
    using CDX.Input;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public interface IGUISystem : IMouseListener, IKeyboardListener, ITouchFingerListener, IControllerListener
    {
        void ScreenResized(int width, int height);
        void Clear();
        void SetScreen(GUI.IScreen? screen);
        void Update(FrameTime time);
        void Render(IGraphics gfx, FrameTime time);

        GUI.IScreen? OpenScreen(string? title = null);

        IWindow? OpenWindow(GUI.IScreen? screen, int leftEdge = 0, int topEdge = 0, int width = 256, int height = 256, string title = "");
        void CloseWindow(IWindow? window);
        void ActivateWindow(IWindow? window);
        void ActivateGadget(IGadget? gadget);
        IGadget? AddGadget(IWindow? window,
            int leftEdge = 0,
            int topEdge = 0,
            int width = 100,
            int height = 100,
            GadgetFlags flags = GadgetFlags.None,
            GadgetActivation activation = GadgetActivation.Immediate | GadgetActivation.RelVerify,
            string? text = null,
            bool disabled = false,
            Action? clickAction = null,
            int gadgetID = -1);

        void RemGadget(IGadget? gadget);

    }
}
