namespace GadgetGUI
{
    using CDX;
    using CDX.App;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal abstract class GUIObject : Box
    {
        protected readonly GUISystem gui;
        private string? text;

        protected GUIObject(GUISystem gui)
        {
            this.gui = gui;
        }

        public string? Text
        {
            get => text;
            set
            {
                if (!string.Equals(text, value))
                {
                    SetText(value);
                    Invalidate();
                }
            }
        }

        public virtual void SetText(string? text)
        {
            this.text = text;
        }

        public abstract void Update(FrameTime time);
        public abstract void Render(IGraphics gfx, FrameTime time);

        public abstract void Invalidate();

    }
}
