namespace CDX.Screens
{
    using CDX.App;
    using CDX.Graphics;
    using CDX.Input;
    using CDX.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class BaseScreen : IScreen
    {
        private readonly Window window;
        private readonly string name;
        private bool isVisible;
        private bool isPaused;
        private int width;
        private int height;
        private int updateCount;
        private string bgImageName = string.Empty;
        private IImage? bgImage;
        private bool firstShow;
        private GUI.IScreen? guiScreen;

        public BaseScreen(Window window, string name)
        {
            this.window = window;
            this.name = name;
            firstShow = false;
        }

        public Func<IGUISystem, GUI.IScreen?>? InitGUI { get; set; }
        public Window Window => window;
        public IAudio Audio => window.Audio;
        public IGraphics Graphics => window.Graphics;
        public IContentManager Content => window.Content;
        public string Name => name;
        public bool IsVisible => isVisible;
        public bool IsPaused => isPaused;
        public string BackgroundImageName
        {
            get => bgImageName;
            set => SetBgImage(value);
        }


        public virtual void Hide()
        {
            isVisible = false;
            updateCount = 0;
            Logger.Info($"Hide {name}");
            window.GUI.Clear();
        }
        public virtual void Pause()
        {
            isPaused = true;
            Logger.Info($"Pause {name}");
        }
        public virtual void Render(IGraphics graphics, FrameTime time)
        {
            RenderBackground(graphics);
        }
        public virtual void Resume()
        {
            isPaused = false;
            Logger.Info($"Resume {name}");
        }
        public virtual void Show()
        {
            width = Graphics.Width;
            height = Graphics.Height;
            isVisible = true;
            updateCount = 0;
            Logger.Info($"Show {name} ({width}x{height})");
            LoadBgImage();
            if (!firstShow) { FirstShow(); }
            window.GUI.ScreenResized(width, height);
            window.GUI.SetScreen(guiScreen);

        }
        public virtual void Update(FrameTime time)
        {
            if (updateCount < 2)
            {
                updateCount++;
                if (updateCount == 2)
                {

                }
            }
        }

        public virtual void Resized(int width, int height)
        {
            this.width = Graphics.Width;
            this.height = Graphics.Height;
            Logger.Info($"Resized { name} ({ width}x{ height})");
            window.GUI.ScreenResized(width, height);
        }

        protected virtual void FirstShow()
        {
            firstShow = true;
            guiScreen = InitGUI?.Invoke(window.GUI);
        }

        protected void RenderBackground(IGraphics graphics)
        {
            if (bgImage != null)
            {
                graphics.DrawImage(bgImage, 0, 0, width, height);
            }
        }
        private void SetBgImage(string name)
        {
            if (!string.Equals(bgImageName, name))
            {
                ClearBgImage();
                bgImageName = name;
                LoadBgImage();
            }
        }

        private void LoadBgImage()
        {
            if (!string.IsNullOrEmpty(bgImageName))
            {
                if (bgImage == null || bgImage.Name != bgImageName)
                {
                    bgImage = window.LoadImage(bgImageName);
                }
            }
        }

        private void ClearBgImage()
        {
            bgImage?.Dispose();
            bgImage = null;
            bgImageName = string.Empty;
        }

        public virtual void OnMouseButtonDown(MouseButtonEventArgs e)
        {

        }

        public virtual void OnMouseButtonUp(MouseButtonEventArgs e)
        {

        }

        public virtual void OnMouseMoved(MouseMotionEventArgs e)
        {

        }

        public virtual void OnMouseWheel(MouseWheelEventArgs e)
        {

        }

        public virtual void OnKeyDown(KeyEventArgs e)
        {

        }

        public virtual void OnKeyUp(KeyEventArgs e)
        {
        }

        public virtual void OnTextInput(TextInputEventArgs e)
        {

        }

    }
}
