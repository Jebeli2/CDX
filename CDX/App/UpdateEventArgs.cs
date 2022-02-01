namespace CDX.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UpdateEventArgs : EventArgs
    {
        private readonly FrameTime frameTime;
        public UpdateEventArgs(FrameTime frameTime)
        {
            this.frameTime = frameTime;
        }

        public FrameTime FrameTime => frameTime;

    }

    public delegate void UpdateEventHandler(object sender, UpdateEventArgs e);
}
