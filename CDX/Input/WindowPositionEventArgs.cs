namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class WindowPositionEventArgs : EventArgs
    {
        private readonly int x;
        private readonly int y;

        public WindowPositionEventArgs(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X => x;
        public int Y => y;

        public override string ToString()
        {
            return $"Position: {x} x {y}";
        }
    }

    public delegate void WindowPositionEventHandler(object sender, WindowPositionEventArgs e);
}
