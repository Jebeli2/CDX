namespace GadgetGUI
{
    using CDX.GUI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class Box : IBox
    {
        private int leftEdge;
        private int topEdge;
        private int width;
        private int height;
        private int borderTop;
        private int borderLeft;
        private int borderRight;
        private int borderBottom;

        public Box()
        {

        }

        public Box(Box other)
        {
            leftEdge = other.leftEdge;
            topEdge = other.topEdge;
            width = other.width;
            height = other.height;
            borderTop = other.borderTop;
            borderLeft = other.borderLeft;
            borderRight = other.borderRight;
            borderBottom = other.borderBottom;
        }
        public int LeftEdge
        {
            get => leftEdge;
            set
            {
                if (leftEdge != value)
                {
                    SetDimensions(value, topEdge, width, height);
                }
            }
        }

        public int TopEdge
        {
            get => topEdge;
            set
            {
                if (topEdge != value)
                {
                    SetDimensions(leftEdge, value, width, height);
                }
            }
        }

        public int Width
        {
            get => width;
            set
            {
                if (width != value)
                {
                    SetDimensions(leftEdge, topEdge, value, height);
                }
            }
        }

        public int Height
        {
            get => height;
            set
            {
                if (height != value)
                {
                    SetDimensions(leftEdge, topEdge, width, value);
                }
            }
        }

        public int BorderLeft
        {
            get => borderLeft;
            set
            {
                if (borderLeft != value)
                {
                    SetBorders(value, borderTop, borderRight, borderBottom);
                }
            }
        }
        public int BorderTop
        {
            get => borderTop;
            set
            {
                if (borderTop != value)
                {
                    SetBorders(borderLeft, value, borderRight, borderBottom);
                }
            }
        }
        public int BorderRight
        {
            get => borderRight;
            set
            {
                if (borderRight != value)
                {
                    SetBorders(borderLeft, borderTop, value, borderBottom);
                }
            }
        }
        public int BorderBottom
        {
            get => borderBottom;
            set
            {
                if (borderBottom != value)
                {
                    SetBorders(borderLeft, borderTop, borderRight, value);
                }
            }
        }

        public virtual void SetDimensions(int x, int y, int w, int h)
        {
            leftEdge = x;
            topEdge = y;
            width = w;
            height = h;
        }

        public virtual void SetBorders(int left, int top, int right, int bottom)
        {
            borderLeft = left;
            borderTop = top;
            borderRight = right;
            borderBottom = bottom;
        }

        public virtual bool Contains(int x, int y)
        {
            return x >= leftEdge && y >= topEdge && x < leftEdge + width && y < topEdge + height;
        }
    }
}
