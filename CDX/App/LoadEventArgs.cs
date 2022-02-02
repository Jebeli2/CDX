namespace CDX.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LoadEventArgs : EventArgs
    {
        private readonly CDXWindow cdx;
        private readonly IGraphics graphics;

        public LoadEventArgs(IGraphics graphics, CDXWindow cdx)
        {
            this.graphics = graphics;
            this.cdx = cdx;
        }

        public IGraphics Graphics => graphics;
        public CDXWindow CDXWindow => cdx;
    }

    public delegate void LoadEventHandler(object sender, LoadEventArgs e);
}
