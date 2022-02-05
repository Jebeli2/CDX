namespace CDX.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ISortableSprite : ISprite, IComparable<ISortableSprite>
    {
        float MapPosX { get; }
        float MapPosY { get; }
        long Prio { get; set; }
        long BasePrio { get; set; }
    }
}
