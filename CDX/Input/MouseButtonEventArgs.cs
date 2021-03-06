namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MouseButtonEventArgs : MouseEventArgs
    {
        private readonly MouseButton button;
        private readonly int clicks;
        private readonly KeyButtonState state;

        public MouseButtonEventArgs(int which, int x, int y, MouseButton button, KeyButtonState state, int clicks)
            : base(which, x, y)
        {
            this.button = button;
            this.state = state;
            this.clicks = clicks;
        }

        public MouseButton Button => button;
        public KeyButtonState State => state;
        public int Clicks => clicks;

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(Which);
            sb.Append(": ");
            sb.Append(button);
            sb.Append(' ');
            sb.Append(state);
            sb.Append(" (");
            sb.Append(X);
            sb.Append('x');
            sb.Append(Y);
            sb.Append(')');
            if (clicks > 1)
            {
                sb.Append(" (");
                sb.Append(clicks);
                sb.Append(" clicks)");
            }
            return sb.ToString();
            //return $"Mouse {Which}: {button} {state} ({X}x{Y})";
        }
    }
    public delegate void MouseButtonEventHandler(object sender, MouseButtonEventArgs e);
}
