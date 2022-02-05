namespace CDX.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum MapOrientation
    {
        Isometric,
        Orthogonal
    }

    public enum MovementType
    {
        Normal,
        Flying,
        Intangible
    }

    public enum CollisionType
    {
        Normal,
        Player,
        Entity
    }

    public enum LayerType
    {
        Unknown,
        Object,
        Background,
        Collision
    }
}
