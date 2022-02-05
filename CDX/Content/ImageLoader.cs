namespace CDX.Content
{
    using CDX.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ImageLoader : IResourceLoader<IImage>
    {
        private readonly Window window;
        public ImageLoader(Window window)
        {
            this.window = window;
        }
        public string Name => "Window Image Loader";

        public IContentManager? ContentManager
        {
            get { return window.Content; }
            set { }
        }

        public IImage? Load(string name, byte[]? data)
        {
            if (data != null)
            {
                return window.LoadImage(data, name);
            }
            else
            {
                return window.LoadImage(name);
            }
        }
    }
}
