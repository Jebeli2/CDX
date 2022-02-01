namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TouchFingerEventArgs : EventArgs
    {
        private readonly long touchId;
        private readonly long fingerId;
        private readonly float x;
        private readonly float y;
        private readonly float dx;
        private readonly float dy;
        private readonly float pressure;

        public TouchFingerEventArgs(long touchId, long fingerId, float x, float y, float dx, float dy, float pressure)
        {
            this.touchId = touchId;
            this.fingerId = fingerId;
            this.x = x;
            this.y = y;
            this.dx = dx;
            this.dy = dy;
            this.pressure = pressure;
        }

        public long TouchId => touchId;
        public long FingerId => fingerId;
        public float X => x;
        public float Y => y;
        public float Dx => dx;
        public float Dy => dy;
        public float Pressure => pressure;

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(touchId);
            sb.Append(',');
            sb.Append(fingerId);
            sb.Append(": (");
            sb.Append(x);
            sb.Append('x');
            sb.Append(y);
            sb.Append(") (");
            sb.Append(dx);
            sb.Append('x');
            sb.Append(dy);
            sb.Append(" ) - ");
            sb.Append(pressure);
            return sb.ToString();
        }
    }

    public delegate void TouchFingerEventHandler(object sender, TouchFingerEventArgs e);
}
