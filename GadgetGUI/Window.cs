namespace GadgetGUI
{
    using CDX;
    using CDX.App;
    using CDX.Graphics;
    using CDX.GUI;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Window : GUIObject, IWindow
    {
        private readonly Screen screen;
        private WindowFlags windowFlags;
        private readonly List<Gadget> gadgets = new();
        public Window(GUISystem gui, Screen screen)
            : base(gui)
        {
            this.screen = screen;
            windowFlags = WindowFlags.SizeGadget | WindowFlags.DragBar | WindowFlags.DepthGadget | WindowFlags.HasZoom | WindowFlags.CloseGadget | WindowFlags.SizeBBottom | WindowFlags.Activate;
            SetBorders(4, 28, 4, 4);
            //SetBorders(32, 28, 32, 28);
            this.screen.AddWindow(this);
        }

        public Screen Screen => screen;
        public string? Title { get => Text; set => Text = value; }

        public WindowFlags WindowFlags
        {
            get => windowFlags;
            set
            {
                if (windowFlags != value)
                {
                    windowFlags = value;
                }
            }
        }

        public bool Borderless
        {
            get => (windowFlags & WindowFlags.Borderless) == WindowFlags.Borderless;
            set
            {
                if (value)
                {
                    WindowFlags |= WindowFlags.Borderless;
                }
                else
                {
                    WindowFlags &= ~WindowFlags.Borderless;
                }
            }
        }
        public bool Active
        {
            get => (windowFlags & WindowFlags.WindowActive) == WindowFlags.WindowActive;
            set
            {
                if (value)
                {
                    WindowFlags |= WindowFlags.WindowActive;
                }
                else
                {
                    WindowFlags &= ~WindowFlags.WindowActive;
                }
            }
        }
        public bool MouseHover
        {
            get => (windowFlags & WindowFlags.MouseHover) == WindowFlags.MouseHover;
            set
            {
                if (value)
                {
                    WindowFlags |= WindowFlags.MouseHover;
                }
                else
                {
                    WindowFlags &= ~WindowFlags.MouseHover;
                }
            }
        }

        public override void Update(FrameTime time)
        {
            foreach (Gadget gad in gadgets)
            {
                gad.Update(time);
            }
        }

        public override void Render(IGraphics gfx, FrameTime time)
        {
            GUIRenderer.Instance.RenderWindow(gfx, this);
            foreach (Gadget gad in gadgets)
            {
                gad.Render(gfx, time);
            }

        }

        public void AddGadget(Gadget gadget)
        {
            gadgets.Add(gadget);
        }

        public Gadget? FindGadget(int x, int y)
        {
            for (int i = gadgets.Count - 1; i >= 0; i--)
            {
                Gadget gad = gadgets[i];
                if (gad.Contains(x, y))
                {
                    return gad;
                }
            }
            return null;
        }


        public override string ToString()
        {
            return $"Window '{Title}'";
        }

        //public bool AdjustWindowMinMax()
        //{
        //    CalculateBounds();
        //    int w = BoundsW;
        //    int h = BoundsH;
        //    if (maxWidth > 0 && w > maxWidth) { w = maxWidth; }
        //    if (maxHeight > 0 && h > maxHeight) { h = maxHeight; }
        //    if (w < minWidth) { w = minWidth; }
        //    if (h < minHeight) { h = minHeight; }
        //    if (w != BoundsW || h != BoundsH)
        //    {
        //        SetSize(w, h);
        //        CalculateBounds();
        //        Invalidate();
        //        return true;
        //    }
        //    else
        //    {
        //        Invalidate();
        //    }
        //    return false;
        //}

        //public bool AdjustSizingWindowBounds(out bool preventX, out bool preventY)
        //{
        //    preventX = false;
        //    preventY = false;
        //    CalculateBounds();
        //    int diffX = 0;
        //    int diffY = 0;
        //    if (BoundsX + BoundsW > gui.ScreenWidth) { diffX = (BoundsX + BoundsW) - gui.ScreenWidth; }
        //    if (BoundsY + BoundsH > gui.ScreenHeight) { diffY = (BoundsY + BoundsH) - gui.ScreenHeight; }
        //    if (diffX != 0 || diffY != 0)
        //    {
        //        preventX = diffX != 0;
        //        preventY = diffY != 0;
        //        SetSize(BoundsW - diffX, BoundsH - diffY);
        //        CalculateBounds();
        //        Invalidate();
        //        return true;
        //    }
        //    else
        //    {
        //        Invalidate();
        //    }
        //    return false;
        //}

        //public bool AdjustMovingWindowBounds(out bool preventX, out bool preventY)
        //{
        //    preventX = false;
        //    preventY = false;
        //    CalculateBounds();
        //    int diffX = 0;
        //    int diffY = 0;
        //    if (BoundsX + BoundsW > gui.ScreenWidth) { diffX = (BoundsX + BoundsW) - gui.ScreenWidth; }
        //    if (BoundsY + BoundsH > gui.ScreenHeight) { diffY = (BoundsY + BoundsH) - gui.ScreenHeight; }
        //    if (BoundsX < 0) { diffX = BoundsX; }
        //    if (BoundsY < 0) { diffY = BoundsY; }
        //    if (diffX != 0 || diffY != 0)
        //    {
        //        preventX = diffX != 0;
        //        preventY = diffY != 0;
        //        SetPosition(BoundsX - diffX, BoundsY - diffY);
        //        CalculateBounds();
        //        Invalidate();
        //        return true;
        //    }
        //    else
        //    {
        //        Invalidate();
        //    }
        //    return false;
        //}

        //internal void MoveWindow(int dx, int dy)
        //{
        //    if (dx != 0 || dy != 0)
        //    {
        //        SetPosition(LeftEdge + dx, TopEdge + dy);
        //    }
        //}
        //internal void SizeWindow(int dx, int dy)
        //{
        //    if (dx != 0 || dy != 0)
        //    {
        //        SetSize(Width + dx, Height + dy);
        //    }
        //}
    }
}
