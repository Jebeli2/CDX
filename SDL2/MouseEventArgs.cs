namespace SDL2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MouseEventArgs
    {
        private readonly int which;
        private readonly int x;
        private readonly int y;

        public MouseEventArgs(int which, int x, int y)
        {
            this.which = which; 
            this.x = x;
            this.y = y;
        }
        public int Which => which;
        public int X => x;
        public int Y => y;
    }

}
