namespace SDL2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RendererEventArgs : EventArgs
    {
        private readonly SDLRenderer renderer;
        private readonly float deltaTime;
        private readonly float totalTime;

        public RendererEventArgs(SDLRenderer renderer, float deltaTime, float totalTime)
        {
            this.renderer = renderer;
            this.deltaTime = deltaTime;
            this.totalTime = totalTime;
        }

        public SDLRenderer Renderer => renderer;
        public float DeltaTime => deltaTime;
        public float TotalTime => totalTime;
    }

    public delegate void RendererEventHandler(object sender, RendererEventArgs e);
}
