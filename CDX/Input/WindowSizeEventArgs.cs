namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WindowSizeEventArgs : EventArgs
    {
        private readonly int width;
        private readonly int height;
        public WindowSizeEventArgs(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public int Width => width;
        public int Height => height;

        public override string ToString()
        {
            return $"Size: {width} x {height}";
        }
    }
    public delegate void WindowSizeEventHandler(object sender, WindowSizeEventArgs e);
}
