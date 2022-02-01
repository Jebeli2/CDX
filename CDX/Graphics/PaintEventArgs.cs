namespace CDX.Graphics
{
    using CDX.App;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class PaintEventArgs : UpdateEventArgs
    {
        private readonly IGraphics graphics;

        public PaintEventArgs(IGraphics graphics, FrameTime time)
            : base(time)
        {
            this.graphics = graphics;
        }

        public IGraphics Graphics => graphics;
    }

    public delegate void PaintEventHandler(object sender, PaintEventArgs e);
}
