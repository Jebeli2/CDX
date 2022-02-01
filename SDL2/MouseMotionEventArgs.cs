namespace SDL2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MouseMotionEventArgs : MouseEventArgs
    {
        private readonly int relX;
        private readonly int relY;

        public MouseMotionEventArgs(int which, int x, int y, int relX, int relY)
            : base(which, x, y)
        {
            this.relX = relX;
            this.relY = relY;
        }
        public int RelX => relX;
        public int RelY => relY;

        public override string ToString()
        {
            return $"Mouse {Which}: Moved to {X}x{Y} ({relX} x {relY}) ";
        }

    }

    public delegate void MouseMotionEventHandler(object sender, MouseMotionEventArgs e);
}
