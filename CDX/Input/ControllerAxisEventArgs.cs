namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Threading.Tasks;

    public class ControllerAxisEventArgs : ControllerEventArgs
    {
        private readonly ControllerAxis axis;
        private readonly int axisValue;
        private readonly Vector2 direction;
        public ControllerAxisEventArgs(int which, ControllerAxis axis, int value, Vector2 direction)
            : base(which)
        {
            this.axis = axis;
            axisValue = value;
            this.direction = direction;
        }

        public ControllerAxis Axis => axis;
        public int AxisValue => axisValue;
        public Vector2 Direction => direction;

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(Which);
            sb.Append(": ");
            sb.Append(axis);
            sb.Append(' ');
            sb.Append(axisValue);
            sb.Append(" direction = ");
            sb.Append(direction);
            return sb.ToString();
        }

    }

    public delegate void ControllerAxisEventHandler(object sender, ControllerAxisEventArgs e);
}
