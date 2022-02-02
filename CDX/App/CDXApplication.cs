namespace CDX.App
{
    using CDX.Audio;
    using CDX.Content;
    using CDX.Graphics;
    using CDX.Logging;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class CDXApplication
    {
        protected bool quitRequested;

        private IContentManager contentManager;
        private TimeSpan accumulatedElapsedTime;
        private TimeSpan targetElapsedTime = TimeSpan.FromTicks(166667);
        private TimeSpan maxElapsedTime = TimeSpan.FromMilliseconds(500);
        private TimeSpan warnSlowTime = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan oneSecondTimeSpan = new(0, 0, 1);
        private TimeSpan fpsTimer = oneSecondTimeSpan;
        private TimeSpan slowTimer = TimeSpan.Zero;
        private int maxFramesPerSecond = 60;
        private int frameCounter = 60;
        private int framesPerSecond = 60;
        private int prevFPS;
        private string fpsText = "60 fps";



        private readonly Stopwatch frameTimer = new();
        private readonly FrameTime frameTime = new();
        private long previousTicks;
        private int updateFrameLag;
        private bool suppressDraw;
        private bool limitFps = true;

        protected IAudio audio;

        protected CDXApplication()
        {
            contentManager = new ContentManager(this);
            audio = NoAudio.Instance;
        }


        public IContentManager Content => contentManager;
        public IAudio Audio => audio;
        public string FPSText => fpsText;
        public void Run(ApplicationContext context)
        {
            Initialize();
            InitContent();
            InitWindows(context);
            MainLoop();
            Shutdown();
        }

        public void AddWindow(Window window)
        {
            CDXWindow cdx = CreateWindow(window);
            if (window.Visible)
            {                
                cdx.Show();
            }
        }

        public void Exit()
        {
            quitRequested = true;
        }
        public int MaxFramesPerSecond
        {
            get => maxFramesPerSecond;
            set
            {
                maxFramesPerSecond = value;
                framesPerSecond = value;
                frameCounter = value;
                prevFPS = value;
                double millisPerFrame = 1000.0 / maxFramesPerSecond;
                targetElapsedTime = TimeSpan.FromMilliseconds(millisPerFrame);
                fpsText = framesPerSecond.ToString() + " fps";
            }
        }

        public abstract int GetDisplayCount();
        //public abstract ulong GetTicks();
        public abstract void Delay(uint ms);

        private void InitContent()
        {
            System.Resources.ResourceManager? res = FindResourceManager();
            if (res != null)
            {
                contentManager.AddResources(res);
            }
        }

        private System.Resources.ResourceManager? FindResourceManager()
        {
            Assembly? ass = Assembly.GetEntryAssembly();
            if (ass != null)
            {
                AssemblyName assName = ass.GetName();
                if (assName != null && assName.Name != null)
                {
                    string name = assName.Name;
                    string resName = name + ".Properties.Resources";
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager(resName, ass);
                    return temp;
                }
            }
            return null;
        }


        private void InitWindows(ApplicationContext context)
        {
            MaxFramesPerSecond = context.Configuration.MaxFPS;
            if (context.Configuration.VSync)
            {
                MaxFramesPerSecond = 60;
            }

            int numDisplays = GetDisplayCount();
            int display = 0;

            foreach (Window win in context.Windows)
            {
                win.Display = display;
                win.Driver = context.Configuration.Driver;
                win.VSync = context.Configuration.VSync;
                win.FullScreen = context.Configuration.FullScreen;
                win.Width = context.Configuration.Width;
                win.Height = context.Configuration.Height;
                win.ShowFPS = context.Configuration.ShowFPS;
                win.FPSPosX = context.Configuration.FPSPosX;
                win.FPSPosY = context.Configuration.FPSPosY;
                win.GUI = Application.CreateGUISystem(context.Configuration.GUIName);
                AddWindow(win);
                display++;
                display %= numDisplays;
            }

        }
        //private void OldMainLoop()
        //{
        //    frameTimer.Start();
        //    ulong deltaTime = 0;
        //    const float fpsAlpha = 0.2f;
        //    const float fpsAlphaNeg = 1.0f - fpsAlpha;
        //    int frameCounter = 0;
        //    float frameTime = maxFramesPerSecond;

        //    while (!quitRequested)
        //    {

        //        ulong lastUpdateTime = GetTicks();
        //        MessageLoop();
        //        ulong currentTime = GetTicks();
        //        deltaTime = currentTime - lastUpdateTime;
        //        DoRender(frameTime);
        //        ulong endUpdateTime = GetTicks();
        //        deltaTime = endUpdateTime - currentTime;
        //        uint maxTime = (uint)(1000 / maxFramesPerSecond);
        //        if (deltaTime < maxTime)
        //        {
        //            Delay((uint)(maxTime - deltaTime));
        //        }
        //        ulong endFrameTime = GetTicks();
        //        deltaTime = endFrameTime - lastUpdateTime;
        //        frameCounter++;
        //        if (frameCounter > maxFramesPerSecond / 4) { frameCounter = 0; }
        //        frameTime = fpsAlpha * (deltaTime / 1000.0f) + fpsAlphaNeg * frameTime;
        //        if (frameCounter == 0)
        //        {
        //            fps = (int)(1.0f / frameTime);
        //            if (fps != prevFPS)
        //            {
        //                prevFPS = fps;
        //                fpsText = fps.ToString() + " fps";
        //            }
        //        }
        //    }
        //}

        private void MainLoop()
        {
            DoUpdate(frameTime);
            frameTimer.Start();
            while (!quitRequested)
            {
                MessageLoop();
                Tick();
            }
        }

        private void Tick()
        {
            RetryTick:

            long currentTicks = frameTimer.ElapsedTicks;

            accumulatedElapsedTime += TimeSpan.FromTicks(currentTicks - previousTicks);
            previousTicks = currentTicks;
            if (limitFps && accumulatedElapsedTime < targetElapsedTime)
            {
                Sleep((targetElapsedTime - accumulatedElapsedTime).TotalMilliseconds);
                goto RetryTick;
            }
            if (accumulatedElapsedTime > maxElapsedTime) { accumulatedElapsedTime = maxElapsedTime; }
            if (limitFps)
            {
                frameTime.SetElapsedTime(targetElapsedTime);
                int stepCount = 0;
                while (accumulatedElapsedTime >= targetElapsedTime && !quitRequested)
                {
                    frameTime.AddTotal(targetElapsedTime);
                    slowTimer += targetElapsedTime;
                    accumulatedElapsedTime -= targetElapsedTime;
                    ++stepCount;
                    DoUpdate(frameTime);
                }
                updateFrameLag += Math.Max(0, stepCount - 1);
                if (frameTime.IsRunningSlowly)
                {
                    if (updateFrameLag == 0)
                    {
                        frameTime.IsRunningSlowly = false;
                        slowTimer = TimeSpan.Zero;
                        Logger.Verbose($"Stopped Running Slowly");
                    }
                    else if (slowTimer > warnSlowTime)
                    {
                        Logger.Warn($"Running slowly for more than {slowTimer}");
                    }
                }
                else if (updateFrameLag >= 5)
                {
                    frameTime.IsRunningSlowly = true;
                    Logger.Verbose($"Started Running Slowly");
                }
                if (stepCount == 1 && updateFrameLag > 0) { updateFrameLag--; }
                frameTime.SetElapsedTime(targetElapsedTime * stepCount);
            }
            else
            {
                frameTime.Add(accumulatedElapsedTime);
                accumulatedElapsedTime = TimeSpan.Zero;
                DoUpdate(frameTime);
            }
            if (suppressDraw)
            {
                suppressDraw = false;
            }
            else
            {
                DoRender(frameTime);
            }
        }

        private void DoUpdate(FrameTime time)
        {
            fpsTimer += time.ElapsedTime;
            if (fpsTimer >= oneSecondTimeSpan)
            {
                framesPerSecond = frameCounter;
                frameCounter = 0;
                fpsTimer -= oneSecondTimeSpan;
                UpdateFPS();
            }
            Update(time);
        }

        private void UpdateFPS()
        {
            if (framesPerSecond != prevFPS)
            {
                prevFPS = framesPerSecond;
                fpsText = framesPerSecond.ToString() + " fps";
            }
        }

        private void DoRender(FrameTime time)
        {
            frameCounter++;
            Render(time);
        }

        private void Sleep(double ms)
        {
            if (ms > 0)
            {
                uint msInt = (uint)ms;
                if (msInt > 0)
                {
                    Delay(msInt);
                }
            }
        }

        protected abstract void Initialize();
        protected abstract void Shutdown();
        protected abstract void Update(FrameTime time);
        protected abstract void Render(FrameTime time);
        protected abstract void MessageLoop();
        protected abstract CDXWindow CreateWindow(Window window);
    }
}

