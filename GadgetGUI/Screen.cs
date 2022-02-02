namespace GadgetGUI
{
    using CDX.App;
    using CDX.GUI;
    using CDX.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Screen : GUIObject, IScreen
    {
        private readonly List<Window> windows = new();
        private readonly Queue<Window> activationWindows = new();
        private readonly Queue<Gadget> activationGadgets = new();
        private Window? activeWindow;
        private Window? mouseWindow;
        private Gadget? activeGadget;
        private Gadget? mouseGadget;
        public Screen(GUISystem gui)
            : base(gui)
        {
        }

        public Window? ActiveWindow => activeWindow;
        public Gadget? ActiveGadget => activeGadget;    
        public string? Title { get => Text; set => Text = value; }

        public override void SetDimensions(int x, int y, int w, int h)
        {
            x = 0;
            y = 0;
            w = gui.ScreenWidth;
            h = gui.ScreenHeight;
            base.SetDimensions(x, y, w, h);
        }

        public override void Update(FrameTime time)
        {
            CheckWindowActivationQueue();
            CheckGadgetActivationQueue();
            foreach (Window win in windows)
            {
                win.Update(time);
            }
        }

        public override void Render(CDX.IGraphics gfx, FrameTime time)
        {
            foreach (Window win in windows)
            {
                win.Render(gfx, time);
            }
        }

        public void AddWindow(Window win)
        {
            windows.Add(win);
        }

        public void ActivateWindow(Window win)
        {
            activationWindows.Enqueue(win);
        }

        public void ActivateGadget(Gadget gad)
        {
            activationGadgets.Enqueue(gad);
        }


        public Window? FindWindow(int x, int y)
        {
            for (int i = windows.Count - 1; i >= 0; i--)
            {
                Window win = windows[i];
                if (win.Contains(x, y))
                {
                    return win;
                }
            }
            return null;
        }

        private void CheckWindowActivationQueue()
        {
            if (activationWindows.Count > 0)
            {
                Window win = activationWindows.Dequeue();
                SetActiveWindow(win);
            }
        }

        private void CheckGadgetActivationQueue()
        {
            if (activationGadgets.Count > 0)
            {
                Gadget gad = activationGadgets.Peek();
                if (gad.Window == activeWindow)
                {
                    SetActiveGadget(activationGadgets.Dequeue());
                }
            }
        }

        public void SetMouseWindow(Window? win)
        {
            if (mouseWindow != win)
            {
                if (mouseWindow != null)
                {
                    mouseWindow.MouseHover = false;
                }
                mouseWindow = win;
                if (mouseWindow != null)
                {
                    mouseWindow.MouseHover = true;
                }
            }
        }

        public void SetActiveWindow(Window? win)
        {
            if (activeWindow != win)
            {
                if (activeWindow != null)
                {
                    activeWindow.Active = false;
                    Logger.Verbose($"Window deactivated: {activeWindow}");
                }
                activeWindow = win;
                if (activeWindow != null)
                {
                    activeWindow.Active = true;
                    Logger.Verbose($"Window activated: {activeWindow}");
                }
            }
        }
        public void SetMouseGadget(Gadget? gad)
        {
            if (mouseGadget != gad)
            {
                if (mouseGadget != null)
                {
                    mouseGadget.MouseHover = false;
                }
                mouseGadget = gad;
                if (mouseGadget != null)
                {
                    mouseGadget.MouseHover = true;
                }
            }
        }

        public void SetActiveGadget(Gadget? gad)
        {
            if (activeGadget != gad)
            {
                if (activeGadget != null)
                {
                    activeGadget.Active = false;
                    Logger.Verbose($"Gadget deactivated: {activeGadget}");
                }
                activeGadget = gad;
                if (activeGadget != null)
                {
                    activeGadget.Active = true;
                    Logger.Verbose($"Gadget activated: {activeGadget}");
                }
            }
        }


    }
}
