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

        public bool IsBorderGadget
        {
            get => (activation & (GadgetActivation.LeftBorder | GadgetActivation.RightBorder | GadgetActivation.TopBorder | GadgetActivation.BottomBorder)) != 0;
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

        public override void Invalidate()
        {
            window.Invalidate();
        }

        internal void InvalidateBounds()
        {
            bounds = null;
            window.Invalidate();
        }


        public void RaiseGadgetDown()
        {
            EventHelpers.Raise(this, GadgetDown, EventArgs.Empty);
        }

        public void RaiseGadgetUp()
        {
            EventHelpers.Raise(this, GadgetUp, EventArgs.Empty);
        }

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
        }

        private int GetWindowX()
        {
            int x = window.LeftEdge;
            if ((activation & GadgetActivation.LeftBorder) == 0)
            {
                x += window.BorderLeft;
            }
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
