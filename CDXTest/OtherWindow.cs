namespace CDXTest
{
    using CDX;
    using CDX.App;
    using CDX.Graphics;
    using CDX.Input;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class OtherWindow : Window
    {
        private IImage? img1;
        private IImage? img2;
        private IImage? img3;
        private IImage? img4;
        private RenderFlip flip = RenderFlip.None;
        private TextureFilter filter = TextureFilter.Nearest;
        private Color colorMod = Color.White;

        public OtherWindow()
            : base("Other Window")
        {

        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == KeyCode.f)
            {
                if (filter == TextureFilter.Linear)
                {
                    filter = TextureFilter.Nearest;
                }
                else if (filter == TextureFilter.Nearest)
                {
                    filter = TextureFilter.Best;
                }
                else
                {
                    filter = TextureFilter.Linear;
                }
                Console.WriteLine($"Filter = {filter}");
                img1.TextureFilter = filter;
                img2.TextureFilter = filter;
                img3.TextureFilter = filter;
                img4.TextureFilter = filter;
            }
            else if (e.KeyCode == KeyCode.c)
            {
                if (colorMod == Color.White)
                {
                    colorMod = Color.Blue;
                }
                else if (colorMod == Color.Blue)
                {
                    colorMod = Color.Green;
                }
                else if (colorMod == Color.Green)
                {
                    colorMod = Color.Red;
                }
                else if (colorMod == Color.Red)
                {
                    colorMod = Color.White;
                }
                Console.WriteLine($"ColorMod = {colorMod}");
                img1.ColorMod = colorMod;
                img2.ColorMod = colorMod;
                img3.ColorMod = colorMod;
                img4.ColorMod = colorMod;
            }
        }


        protected override void OnLoad(LoadEventArgs e)
        {
            InstallDefaultHotKeys();
            img1 = LoadImage("badlands");
            img2 = LoadImage("fire_temple");
            img3 = LoadImage("ice_palace");
            img4 = LoadImage("arrival");
        }

        protected override void OnMouseButtonUp(MouseButtonEventArgs e)
        {
            switch (flip)
            {
                case RenderFlip.None:
                    flip = RenderFlip.Horizontal;
                    break;
                case RenderFlip.Horizontal:
                    flip = RenderFlip.Vertical;
                    break;
                case RenderFlip.Vertical:
                    flip = RenderFlip.Both;
                    break;
                case RenderFlip.Both:
                    flip = RenderFlip.None;
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            IGraphics gfx = e.Graphics;
            int w = gfx.Width / 2;
            int h = gfx.Height / 2;
            gfx.Viewport = new Rectangle(0, 0, w, h);
            gfx.DrawImage(img1, flip);
            gfx.Viewport = new Rectangle(w, 0, w, h);
            gfx.DrawImage(img2, flip);
            gfx.Viewport = new Rectangle(0, h, w, h);
            gfx.DrawImage(img3, flip);
            gfx.Viewport = new Rectangle(w, h, w, h);
            gfx.DrawImage(img4, flip);
        }
    }
}
