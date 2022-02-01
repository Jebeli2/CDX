namespace CDX.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MapLayer
    {
        private string name = string.Empty;
        private byte alpha = 255;
        private bool visible;
        private float offsetX;
        private float offsetY;
        private float renderOffsetX;
        private float renderOffsetY;
        private bool renderOffsetDirty = true;
        private MapLayer? parent;

        public string Name
        {
            get => name;
            set => name = value;
        }

        public byte Alpha
        {
            get => alpha;
            set => alpha = value;
        }

        public bool Visible
        {
            get => visible;
            set => visible = value;
        }

        public float OffsetX
        {
            get => offsetX;
            set
            {
                offsetX = value;
                InvalidateRenderOffset();
            }
        }

        public float OffsetY
        {
            get => offsetY;
            set
            {
                offsetY = value;
                InvalidateRenderOffset();
            }
        }

        public float RenderOffsetX
        {
            get
            {
                if (renderOffsetDirty) { CalculateRenderOffsets(); }
                return renderOffsetX;
            }
        }

        public float RenderOffsetY
        {
            get
            {
                if (renderOffsetDirty) { CalculateRenderOffsets(); }
                return renderOffsetY;
            }
        }

        public MapLayer? Parent
        {
            get => parent;
            set
            {
                if (value == this) throw new InvalidOperationException("Can't set self as parent");
                parent = value;
                InvalidateRenderOffset();
            }
        }

        public void InvalidateRenderOffset()
        {
            renderOffsetDirty = true;
        }

        protected void CalculateRenderOffsets()
        {
            if (parent != null)
            {
                parent.CalculateRenderOffsets();
                renderOffsetX = parent.RenderOffsetX + offsetX;
                renderOffsetY = parent.RenderOffsetY + offsetY;
            }
            else
            {
                renderOffsetX = offsetX;
                renderOffsetY = offsetY;
            }
            renderOffsetDirty = false;
        }
    }
}
