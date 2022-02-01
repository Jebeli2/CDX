namespace SDL2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MouseWheelEventArgs : MouseEventArgs
    {
        private readonly float preciseX;
        private readonly float preciseY;
        private readonly MouseWheelDirection direction;

        public MouseWheelEventArgs(int which, int x, int y, float preciseX, float preciseY, MouseWheelDirection direction)
            : base(which, x, y)
        {
            this.preciseX = preciseX;
            this.preciseY = preciseY;
            this.direction = direction;
        }

        public float PreciseY => preciseY;
        public float PreciseX => preciseX;
        public MouseWheelDirection Direction => direction;

        public override string ToString()
        {
            return $"Mouse {Which}: Wheel {X}x{Y} (Precise {preciseX} x {preciseY}) (Direction {direction})";
        }

    }

    public delegate void MouseWheelEventHandler(object sender, MouseWheelEventArgs e);
}
