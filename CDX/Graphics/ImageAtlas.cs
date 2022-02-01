namespace CDX.Graphics
{
    using CDX.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ImageAtlas
    {
        private readonly ObjectSet<IImage> images = new ObjectSet<IImage>(4);
        private readonly List<IImageRegion> regions = new List<IImageRegion>();


        public IImageRegion AddRegion(IImage image, int x, int y, int width, int height)
        {
            images.Add(image);
            ImageRegion region = new ImageRegion(image, x, y, width, height);
            regions.Add(region);
            return region;
        }
    }
}
