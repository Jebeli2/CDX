namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ControllerEventArgs : EventArgs
    {
        private readonly int which;

        public ControllerEventArgs(int which)
        {
            this.which = which;
        }

        public int Which => which;
    }
}
