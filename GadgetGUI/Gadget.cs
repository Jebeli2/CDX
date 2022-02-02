namespace GadgetGUI
{
    using CDX;
    using CDX.App;
    using CDX.Graphics;
    using CDX.GUI;
    using CDX.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Gadget : GUIObject, IGadget
    {
        private readonly Window window;

        private GadgetFlags flags;
        private GadgetActivation activation;
        private bool transparentBackground;

        private IImageRegion? background;
        private IBox? bounds;
        private Icons icon;

        public Gadget(GUISystem gui, Window window)
            : base(gui)
        {
            this.window = window;
            SetBorders(1, 1, 1, 1);
            activation = GadgetActivation.RelVerify | GadgetActivation.Immediate;
            window.AddGadget(this);
        }

        public Icons Icon
        {
            get => icon;
            set
            {
                if (icon != value)
                {
                    icon = value;
                }
            }
        }
        public IBox GetBounds()
        {
            if (bounds == null)
            {
                bounds = CalculateBounds();
            }
            return bounds;
        }

        public override void Update(FrameTime time)
        {

        }

        public override void Render(IGraphics gfx, FrameTime time)
        {
            GUIRenderer.Instance.RenderGaget(gfx, this);
        }


        public event EventHandler<EventArgs> GadgetDown;
        public event EventHandler<EventArgs> GadgetUp;

        //public Gadget? Parent => parent;

        public int GadgetID { get; set; }

        public Window Window => window;

        public bool Active
        {
            get => (activation & GadgetActivation.ActiveGadget) == GadgetActivation.ActiveGadget;
            set
            {

                if (value)
                {
                    Activation |= GadgetActivation.ActiveGadget;
                }
                else
                {
                    Activation &= ~GadgetActivation.ActiveGadget;
                }
            }
        }

        public bool Enabled
        {
            get => (flags & GadgetFlags.Disabled) == 0;
            set
            {
                if (value)
                {
                    Flags &= ~GadgetFlags.Disabled;
                }
                else
                {
                    Flags |= GadgetFlags.Disabled;
                }
            }
        }

        public bool MouseHover
        {
            get => (activation & GadgetActivation.MouseHover) == GadgetActivation.MouseHover;
            set
            {
                if (value)
                {
                    Activation |= GadgetActivation.MouseHover;
                }
                else
                {
                    Activation &= ~GadgetActivation.MouseHover;
                }
            }
        }

        public bool TransparentBackground
        {
            get => transparentBackground;
            set => transparentBackground = value;
        }


        public GadgetFlags Flags
        {
            get => flags;
            set
            {
                if (flags != value)
                {
                    flags = value;
                    InvalidateBounds();
                }
            }
        }

        public GadgetActivation Activation
        {
            get => activation;
            set
            {
                if (activation != value)
                {
                    activation = value;
                    InvalidateBounds();
                }
            }
        }
        public bool RelVeriy
        {
            get => (activation & GadgetActivation.RelVerify) == GadgetActivation.RelVerify;
        }

        public bool Immediate
        {
            get => (activation & GadgetActivation.Immediate) == GadgetActivation.Immediate;
        }

        public IImageRegion? Background
        {
            get => background;
            set
            {
                if (background != value)
                {
                    background = value;
                }
            }
        }

        internal void InvalidateBounds()
        {
            bounds = null;
        }


        //public void RaiseGadgetDown()
        //{
        //    EventHelpers.Raise(this, GadgetDown, EventArgs.Empty);
        //}

        //public void RaiseGadgetUp()
        //{
        //    EventHelpers.Raise(this, GadgetUp, EventArgs.Empty);
        //}

        //public void Invalidate()
        //{
        //    boundsDirty = true;
        //}

        //public void InvalidateDown()
        //{
        //    boundsDirty = true;
        //    foreach (Gadget gad in children)
        //    {
        //        gad.InvalidateDown();
        //    }
        //}

        //public void InvalidateUp()
        //{
        //    boundsDirty = true;
        //    parent?.InvalidateUp();
        //}

        //public void ClearMouseOver()
        //{
        //    mouseOver = false;
        //}

        //public void MarkMouseOver()
        //{
        //    mouseOver = true;
        //}

        //public virtual void SetPosition(int x, int y)
        //{
        //    if (x != leftEdge || y != topEdge)
        //    {
        //        leftEdge = x;
        //        topEdge = y;
        //        InvalidateDown();
        //    }
        //}

        //public virtual void SetSize(int w, int h)
        //{
        //    if (w != width || h != height)
        //    {
        //        width = w;
        //        height = h;
        //        InvalidateDown();
        //    }
        //}

        //public void Update(FrameTime time)
        //{
        //    if (boundsDirty) { CalculateBounds(); }
        //    foreach (Gadget gad in children)
        //    {
        //        gad.Update(time);
        //    }
        //}
        //public void Render(IGraphics gfx, FrameTime time)
        //{
        //    Render(gfx);
        //    foreach (Gadget gad in children)
        //    {
        //        gad.Render(gfx, time);
        //    }
        //}

        //protected virtual void Render(IGraphics gfx)
        //{
        //    GUIRenderer.Instance.RenderGaget(gfx, this);
        //}

        //public virtual Gadget? FindGadget(int x, int y)
        //{
        //    foreach (Gadget g in children.Reverse<Gadget>())
        //    {
        //        if (g.Visible && g.Contains(x, y))
        //        {
        //            return g.FindGadget(x, y);
        //        }
        //    }
        //    if (parent != null && visible && Contains(x, y))
        //    {
        //        return this;
        //    }
        //    return null;
        //}


        //public void SetMousePosition(int x, int y)
        //{
        //    mouseX = x - bx;
        //    mouseY = y - by;
        //}

        //public bool Contains(int x, int y)
        //{
        //    return x >= bx && y >= by && x < bx + bw && y < by + bh;
        //}


        //private Rectangle ParentBounds
        //{
        //    get
        //    {
        //        if (parent != null)
        //        {
        //            return new Rectangle(parent.BoundsX, parent.BoundsY, parent.BoundsW, parent.BoundsH);
        //        }
        //        else
        //        {
        //            return new Rectangle(0, 0, gui.ScreenWidth, gui.ScreenHeight);
        //        }
        //    }
        //}
        //private Rectangle ParentInnerBounds
        //{
        //    get
        //    {
        //        if (parent != null)
        //        {
        //            return new Rectangle(parent.BoundsX + parent.BorderLeft, parent.BoundsY + parent.BorderTop, parent.BoundsW - parent.BorderLeft - parent.BorderRight, parent.BoundsH - parent.BorderTop - parent.BorderBottom);
        //        }
        //        else
        //        {
        //            return new Rectangle(0, 0, gui.ScreenWidth, gui.ScreenHeight);
        //        }
        //    }
        //}

        //private int GetParentBoundsLeft()
        //{
        //    if (parent != null)
        //    {
        //        int result = parent.bx;
        //        if ((activation & GadgetActivation.LeftBorder) == 0)
        //        {
        //            result += parent.borderLeft;
        //        }
        //        //if ((activation & GadgetActivation.RightBorder) != 0)
        //        //{
        //        //    result -= parent.bo;
        //        //}
        //        return result;
        //    }
        //    return 0;
        //}

        //private int GetParentBorderTop()
        //{
        //    if (parent != null)
        //    {
        //        int result = parent.by;
        //        if ((activation & GadgetActivation.TopBorder) == 0)
        //        {
        //            result += parent.borderTop;
        //        }
        //        //if ((activation & GadgetActivation.BottomBorder) != 0)
        //        //{
        //        //    result -= parent.borderBottom;
        //        //}
        //        return result;
        //    }
        //    return 0;
        //}

        //private int GetParentBorderWidth()
        //{
        //    if (parent != null)
        //    {
        //        int result = parent.bw;
        //        if ((activation & GadgetActivation.LeftBorder) == 0)
        //        {
        //            result -= parent.borderLeft;
        //        }
        //        if ((activation & GadgetActivation.RightBorder) == 0)
        //        {
        //            result -= parent.borderRight;
        //        }
        //        return result;
        //    }
        //    return gui.ScreenWidth;
        //}
        //private int GetParentBorderHeight()
        //{
        //    if (parent != null)
        //    {
        //        int result = parent.bh;
        //        if ((activation & GadgetActivation.TopBorder) == 0)
        //        {
        //            result -= parent.borderTop;
        //        }
        //        if ((activation & GadgetActivation.BottomBorder) == 0)
        //        {
        //            result -= parent.borderBottom;
        //        }
        //        return result;
        //    }
        //    return gui.ScreenHeight;
        //}

        //private int AddRel(GadgetFlags flag, int value)
        //{
        //    return (flags & flag) == flag ? value : 0;
        //}

        //protected void CalculateBounds()
        //{
        //    int x = GetParentBoundsLeft();
        //    int y = GetParentBorderTop();
        //    int w = GetParentBorderWidth();
        //    int h = GetParentBorderHeight();
        //    bx = AddRel(GadgetFlags.RelRight, w) + LeftEdge;
        //    by = AddRel(GadgetFlags.RelBottom, h) + TopEdge;
        //    bw = AddRel(GadgetFlags.RelWidth, w) + Width;
        //    bh = AddRel(GadgetFlags.RelHeight, h) + Height;
        //    bx += x;
        //    by += y;
        //    boundsDirty = false;
        //}

        public override void SetDimensions(int x, int y, int w, int h)
        {
            base.SetDimensions(x, y, w, h);
            InvalidateBounds();
        }

        public override void SetBorders(int left, int top, int right, int bottom)
        {
            base.SetBorders(left, top, right, bottom);
            InvalidateBounds();
        }

        public override bool Contains(int x, int y)
        {
            return GetBounds().Contains(x, y);
            //return base.Contains(x, y);
        }

        private int GetWindowX()
        {
            int x = window.LeftEdge;
            if ((activation & GadgetActivation.LeftBorder) == 0)
            {
                x += window.BorderLeft;
            }
            //        if ((activation & GadgetActivation.RightBorder) == 0)
            //        {
            //            result -= parent.borderRight;
            //        }
            return x;
        }

        private int GetWindowY()
        {
            int y = window.TopEdge;
            if ((activation & GadgetActivation.TopBorder) == 0)
            {
                y += window.BorderTop;
            }
            return y;
        }

        private int GetWindowW()
        {
            int w = window.Width;
            if ((activation & GadgetActivation.LeftBorder) == 0)
            {
                w -= window.BorderLeft;
            }
            if ((activation & GadgetActivation.RightBorder) == 0)
            {
                w -= window.BorderRight;
            }
            return w;
        }
        private int GetWindowH()
        {
            int h = window.Height;
            if ((activation & GadgetActivation.TopBorder) == 0)
            {
                h -= window.BorderTop;
            }
            if ((activation & GadgetActivation.BottomBorder) == 0)
            {
                h -= window.BorderBottom;
            }
            return h;
        }

        private IBox CalculateBounds()
        {
            Box bounds = new Box(this);
            int x = GetWindowX();
            int y = GetWindowY();
            int w = GetWindowW();
            int h = GetWindowH();
            int bx = AddRel(GadgetFlags.RelRight, w) + LeftEdge;
            int by = AddRel(GadgetFlags.RelBottom, h) + TopEdge;
            int bw = AddRel(GadgetFlags.RelWidth, w) + Width;
            int bh = AddRel(GadgetFlags.RelHeight, h) + Height;
            bx += x;
            by += y;
            bounds.SetDimensions(bx, by, bw, bh);
            return bounds;
        }
        private int AddRel(GadgetFlags flag, int value)
        {
            return (flags & flag) == flag ? value : 0;
        }

        public override string ToString()
        {
            return $"Gadget '{GadgetID}'";
        }
    }
}
