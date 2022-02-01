namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ControllerButtonEventArgs : ControllerEventArgs
    {
        private readonly ControllerButton button;
        private readonly KeyButtonState state;

        public ControllerButtonEventArgs(int which, ControllerButton button, KeyButtonState state)
            : base(which)
        {
            this.button = button;
            this.state = state;
        }

        public ControllerButton Button => button;
        public KeyButtonState State => state;

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(Which);
            sb.Append(": ");
            sb.Append(button);
            sb.Append(' ');
            sb.Append(state);
            return sb.ToString();
        }

    }

    public delegate void ControllerButtonEventHandler(object sender, ControllerButtonEventArgs e);
}
