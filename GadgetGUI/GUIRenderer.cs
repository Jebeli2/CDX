﻿namespace GadgetGUI
{
    using CDX;
    using CDX.GUI;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class GUIRenderer
    {
        private static GUIRenderer instance = new GUIRenderer();
        public static GUIRenderer Instance => instance;
        public Color TextColor { get; set; }
        public Color SelectedTextColor { get; set; }
        public Color WindowBackActive { get; set; }
        public Color WindowBackInactive { get; set; }
        public Color WindowBackActiveHover { get; set; }
        public Color WindowBackInactiveHover { get; set; }
        public Color WindowBorderActive { get; set; }
        public Color WindowBorderInactive { get; set; }
        public Color WindowBorderActiveHover { get; set; }
        public Color WindowBorderInactiveHover { get; set; }
        public Color ShineColor { get; set; }
        public Color ShadowColor { get; set; }
        public Color ButtonActive { get; set; }
        public Color ButtonInactive { get; set; }
        public Color ButtonActiveHover { get; set; }
        public Color ButtonInactiveHover { get; set; }
        public Color DarkBackColor { get; set; }

        public Color CellBackground { get; set; }
        public Color CellHover { get; set; }
        public Color CellSelected { get; set; }

        private GUIRenderer()
        {
            WindowBackActive = Color.FromArgb(128, 45, 45, 45);
            WindowBackInactive = Color.FromArgb(128, 43, 43, 43);
            WindowBackActiveHover = Color.FromArgb(128, 45 + 20, 45 + 20, 45 + 20);
            WindowBackInactiveHover = Color.FromArgb(128, 43 + 20, 43 + 20, 43 + 20);
            WindowBorderActive = Color.FromArgb(200, 62, 92, 154);
            WindowBorderInactive = Color.FromArgb(200, 50, 50, 50);
            WindowBorderActiveHover = Color.FromArgb(200, 62 + 20, 92 + 20, 154 + 20);
            WindowBorderInactiveHover = Color.FromArgb(200, 50 + 20, 50 + 20, 50 + 20);
            ButtonActive = Color.FromArgb(128, 64, 64, 64);
            ButtonInactive = Color.FromArgb(128, 74, 74, 74);
            ButtonActiveHover = Color.FromArgb(128, 64 + 20, 64 + 20, 64 + 20);
            ButtonInactiveHover = Color.FromArgb(128, 74 + 20, 74 + 20, 74 + 20);
            ShineColor = Color.FromArgb(128, 250, 250, 250);
            ShadowColor = Color.FromArgb(128, 40, 40, 40);
            TextColor = Color.FromArgb(238, 238, 238);
            SelectedTextColor = Color.FromArgb(255, 255, 255, 255);
            DarkBackColor = Color.FromArgb(230, 55, 55, 55);
            CellBackground = Color.FromArgb(230, 43, 43, 43);
            CellHover = Color.FromArgb(230, 102, 102, 102);
            CellSelected = Color.FromArgb(128, 200, 200, 200);


        }

        private void GetWindowColors(Window window, out Color fc, out Color bg)
        {
            if (window.Active && window.MouseHover)
            {
                fc = WindowBorderActiveHover;
                bg = WindowBackActiveHover;
            }
            else if (window.Active && !window.MouseHover)
            {
                fc = WindowBorderActive;
                bg = WindowBackActive;
            }
            else if (!window.Active && window.MouseHover)
            {
                fc = WindowBorderInactiveHover;
                bg = WindowBackInactiveHover;
            }
            else
            {
                fc = WindowBorderInactive;
                bg = WindowBackInactive;
            }
        }

        private void GetGadgetColors(Gadget gadget, out Color fc, out Color bg)
        {
            if (gadget.Active && gadget.MouseHover)
            {
                fc = ShineColor;
                bg = ButtonActiveHover;
            }
            else if (gadget.Active && !gadget.MouseHover)
            {
                fc = ShineColor;
                bg = ButtonActive;
            }
            else if (!gadget.Active && gadget.MouseHover)
            {
                fc = ShineColor;
                bg = ButtonInactiveHover;
            }
            else
            {
                fc = ShineColor;
                bg = ButtonInactive;
            }
        }

        public void RenderWindow(IGraphics gfx, Window window)
        {
            Rectangle rect = GetBounds(window);
            Rectangle inner = GetInnerBounds(window);
            GetWindowColors(window, out Color fc, out Color bg);
            //Color fc = window.Active ?  window.MouseHover ? WindowBorderActiveHover : WindowBorderActive : ;
            //Color bg = window.Active ? WindowBackActive : WindowBackInactive;
            if (!window.Borderless)
            {
                gfx.Color = fc;
                if (window.BorderTop > 2) gfx.FillRect(rect.Left, rect.Top, rect.Width, window.BorderTop);
                if (window.BorderLeft > 2) gfx.FillRect(rect.Left, inner.Top, window.BorderLeft, inner.Height);
                if (window.BorderRight > 2) gfx.FillRect(rect.Right - window.BorderRight - 1, inner.Top, window.BorderRight, inner.Height);
                if (window.BorderBottom > 2) gfx.FillRect(rect.Left, rect.Bottom - window.BorderBottom - 1, rect.Width, window.BorderBottom);


                RenderBox(gfx, rect, ShineColor, ShadowColor);
                RenderBox(gfx, inner, ShadowColor, ShineColor);
                if (!string.IsNullOrEmpty(window.Title))
                {
                    gfx.DrawText(null, window.Title, inner.X, rect.Y, inner.Width, window.BorderTop, TextColor);
                }

                //    //DrawGadgetBorder(gfx, rect, false, false);
                //    //gfx.DrawText(null, window.Title, inner.X, rect.Y, inner.Width, window.BorderTop, TextColor);

            }
            //if (window.Background != null)
            //{

            //}
            //else
            //{
            //    //inner.Offset(1, 1);
            //    //inner.Width -= 2;
            //    //inner.Height -= 2;
            gfx.Color = bg;
            gfx.FillRect(inner);
            //}

        }

        public void RenderGaget(IGraphics gfx, Gadget gadget)
        {
            IBox bounds = gadget.GetBounds();
            Rectangle rect = GetBounds(bounds);
            Rectangle inner = GetInnerBounds(bounds);
            //Color fc = gadget.Active ? ShadowColor : ShineColor;
            //Color bc = gadget.Active ? ShineColor : ShadowColor;
            GetGadgetColors(gadget, out Color fc, out Color bg);
            if (!gadget.TransparentBackground)
            {
                gfx.Color = bg;
                gfx.FillRect(rect);
            }
            if (gadget.MouseHover)
            {
                gfx.Color = fc;
                gfx.DrawRect(rect);
            }
            //RenderBox(gfx, rect, fc, bc);
            //RenderBox(gfx, inner, bc, fc);
            if (gadget.Active)
            {
                inner.X += 1;
                inner.Y += 1;
                //inner.Width -= 2;
                //inner.Height -= 1;
            }
            if (!string.IsNullOrEmpty(gadget.Text))
            {
                gfx.DrawText(null, gadget.Text, inner.X, inner.Y, inner.Width, inner.Height, TextColor);
            }
            if (gadget.Icon != CDX.Graphics.Icons.NONE)
            {
                gfx.DrawIcon(gadget.Icon, inner.X, inner.Y, inner.Width, inner.Height, TextColor);
            }
        }

        private static void RenderBox(IGraphics gfx, Rectangle rect, Color shinePen, Color shadowPen)
        {
            gfx.Color = shinePen;
            gfx.DrawLine(rect.Left, rect.Top, rect.Right - 2, rect.Top);
            gfx.DrawLine(rect.Left, rect.Top, rect.Left, rect.Bottom - 2);
            gfx.Color = shadowPen;
            gfx.DrawLine(rect.Left, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
            gfx.DrawLine(rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom - 1);
        }

        private static Rectangle GetInnerBounds(IBox box)
        {
            Rectangle rect = GetBounds(box);
            rect.X += box.BorderLeft;
            rect.Y += box.BorderTop;
            rect.Width -= (box.BorderLeft + box.BorderRight);
            rect.Height -= (box.BorderTop + box.BorderBottom);
            return rect;
        }

        private static Rectangle GetBounds(IBox box)
        {
            return new Rectangle(box.LeftEdge, box.TopEdge, box.Width, box.Height);
        }
    }
}
