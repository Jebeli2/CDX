namespace CDX.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MapPosSortableSprite : Sprite, ISortableSprite
    {
        private float mapPosX;
        private float mapPosY;
        private long basePrio;
        private long prio;
        public MapPosSortableSprite(ISprite sprite)
            : base(sprite)
        {

        }

        public float MapPosX
        {
            get => mapPosX;
            set => mapPosX = value;
        }

        public float MapPosY
        {
            get => mapPosY;
            set => mapPosY = value;
        }

        public long BasePrio
        {
            get => basePrio;
            set => basePrio = value;
        }
        public long Prio
        {
            get => prio;
            set => prio = value;
        }

        public int CompareTo(MapPosSortableSprite other)
        {
            return prio.CompareTo(other.prio);
        }

        public int CompareTo(ISortableSprite? other)
        {
            if (other == null) return 1;
            if (other is MapPosSortableSprite mpss) { return CompareTo(mpss); }
            return -1;
        }
    }
}
