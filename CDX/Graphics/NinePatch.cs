namespace CDX.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NinePatch
    {
        public const int TOP_LEFT = 0;
        public const int TOP_CENTER = 1;
        public const int TOP_RIGHT = 2;
        public const int MIDDLE_LEFT = 3;
        public const int MIDDLE_CENTER = 4;
        public const int MIDDLE_RIGHT = 5;
        public const int BOTTOM_LEFT = 6;
        public const int BOTTOM_CENTER = 7;
        public const int BOTTOM_RIGHT = 8;

        private readonly IImage? image;
        private readonly Rectangle[] patches = new Rectangle[9];
        private int patchWidth;
        private int patchHeight;
        private readonly List<IImageRegion> regions = new(9);

        public NinePatch(IImage image)
        {
            this.image = image;
            Fill(new Rectangle(0, 0, this.image.Width, this.image.Height));
        }

        public NinePatch(IImageRegion sprite)
        {
            image = sprite.Image;
            Fill(new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Height));
        }
        public int PatchWidth => patchWidth;
        public int PatchHeight => patchHeight;
        public IImage? Image => image;
        public IList<Rectangle> Patches => patches;

        public IList<IImageRegion> GetRegions()
        {
            if (regions.Count == 0)
            {
                FillRegions();
            }
            return regions;
        }

        private IImageRegion? GetRegion(int index)
        {
            if (image != null)
            {
                return new ImageRegion(image, patches[index].X, patches[index].Y, patches[index].Width, patches[index].Height);
            }
            return null;
        }

        private void FillRegions()
        {
            regions.Clear();
            for (int i = 0; i < patches.Length; i++)
            {
                IImageRegion? region = GetRegion(i);
                if (region != null) { regions.Add(region); }
            }
        }
        private void Fill(Rectangle src)
        {
            int w = src.Width;
            int h = src.Height;
            int pw = w / 3;
            int ph = h / 3;
            int x = src.X;
            int y = src.Y;
            patchWidth = pw;
            patchHeight = ph;
            patches[TOP_LEFT] = new Rectangle(x, y, pw, ph);
            patches[TOP_CENTER] = new Rectangle(x + pw, y, pw, ph);
            patches[TOP_RIGHT] = new Rectangle(x + pw + pw, y, pw, ph);

            patches[MIDDLE_LEFT] = new Rectangle(x, y + ph, pw, ph);
            patches[MIDDLE_CENTER] = new Rectangle(x + pw, y + ph, pw, ph);
            patches[MIDDLE_RIGHT] = new Rectangle(x + pw + pw, y + ph, pw, ph);

            patches[BOTTOM_LEFT] = new Rectangle(x, y + ph + ph, pw, ph);
            patches[BOTTOM_CENTER] = new Rectangle(x + pw, y + ph + ph, pw, ph);
            patches[BOTTOM_RIGHT] = new Rectangle(x + pw + pw, y + ph + ph, pw, ph);
        }
    }
}
