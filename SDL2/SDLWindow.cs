namespace SDL2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLWindow : IDisposable
    {
        private bool disposedValue;
        private readonly SDLApplication app;
        private SDLRenderer? renderer;
        private IntPtr handle;
        private uint windowID;
        private string title;
        private bool visible;
        private int x;
        private int y;
        private int width;
        private int height;
        private int oldX;
        private int oldY;
        private int oldWidth;
        private int oldHeight;
        private int minWidth;
        private int minHeight;
        private int maxWidth;
        private int maxHeight;
        private bool textureFilter = true;
        private bool resizable;
        private bool alwaysOnTop;
        private bool useFakeFullScreen = true;
        private bool fullScreen;
        private bool showFPS = true;
        private int fpsPosX = 10;
        private int fpsPosY = 10;
        private WindowCloseOperation closeOperation = WindowCloseOperation.Close;
        private readonly EventHandlerList eventHandlerList = new();

        private static readonly object windowShownEventKey = new();
        private static readonly object windowHiddenEventKey = new();
        private static readonly object windowExposedEventKey = new();
        private static readonly object windowMovedEventKey = new();
        private static readonly object windowResizedEventKey = new();
        private static readonly object windowSizeChangedEventKey = new();
        private static readonly object windowMinimizedEventKey = new();
        private static readonly object windowMaximizedEventKey = new();
        private static readonly object windowRestoredEventKey = new();
        private static readonly object windowEnterEventKey = new();
        private static readonly object windowLeaveEventKey = new();
        private static readonly object windowFocusGainedEventKey = new();
        private static readonly object windowFocusLostEventKey = new();
        private static readonly object windowCloseEventKey = new();
        private static readonly object windowTakeFocusEventKey = new();
        private static readonly object windowHitTestEventKey = new();
        private static readonly object windowICCProfileEventKey = new();
        private static readonly object windowDisplayChangedEventKey = new();

        private static readonly object windowLoadEventKey = new();
        private static readonly object rendererPaintEventKey = new();
        private static readonly object mouseButtonDownEventKey = new();
        private static readonly object mouseButtonUpEventKey = new();
        private static readonly object mouseMoveEventKey = new();
        private static readonly object mouseWheelEventKey = new();
        private static readonly object keyDownEventKey = new();
        private static readonly object keyUpEventKey = new();
        private static readonly object textInputEventKey = new();
        internal SDLWindow(SDLApplication app, string? title = null)
        {
            this.app = app;
            this.title = title ?? "SDL";
            x = -1;
            y = -1;
            width = 640;
            height = 480;
            resizable = true;
            alwaysOnTop = false;
            this.app.AddWindow(this);
        }
        internal SDLApplication App => app;

        public event EventHandler WindowLoad
        {
            add => eventHandlerList.AddHandler(windowLoadEventKey, value); remove => eventHandlerList.RemoveHandler(windowLoadEventKey, value);
        }

        public event EventHandler WindowShown
        {
            add => eventHandlerList.AddHandler(windowShownEventKey, value); remove => eventHandlerList.RemoveHandler(windowShownEventKey, value);
        }

        public event EventHandler WindowHidden
        {
            add => eventHandlerList.AddHandler(windowHiddenEventKey, value); remove => eventHandlerList.RemoveHandler(windowHiddenEventKey, value);
        }

        public event EventHandler WindowExposed
        {
            add => eventHandlerList.AddHandler(windowExposedEventKey, value); remove => eventHandlerList.RemoveHandler(windowExposedEventKey, value);
        }

        public event EventHandler WindowMoved
        {
            add => eventHandlerList.AddHandler(windowMovedEventKey, value); remove => eventHandlerList.RemoveHandler(windowMovedEventKey, value);
        }

        public event EventHandler WindowResized
        {
            add => eventHandlerList.AddHandler(windowResizedEventKey, value); remove => eventHandlerList.RemoveHandler(windowResizedEventKey, value);
        }

        public event EventHandler WindowSizeChanged
        {
            add => eventHandlerList.AddHandler(windowSizeChangedEventKey, value); remove => eventHandlerList.RemoveHandler(windowSizeChangedEventKey, value);
        }

        public event EventHandler WindowMinimized
        {
            add => eventHandlerList.AddHandler(windowMinimizedEventKey, value); remove => eventHandlerList.RemoveHandler(windowMinimizedEventKey, value);
        }

        public event EventHandler WindowMaxmimized
        {
            add => eventHandlerList.AddHandler(windowMaximizedEventKey, value); remove => eventHandlerList.RemoveHandler(windowMaximizedEventKey, value);
        }

        public event EventHandler WindowRestored
        {
            add => eventHandlerList.AddHandler(windowRestoredEventKey, value); remove => eventHandlerList.RemoveHandler(windowRestoredEventKey, value);
        }

        public event EventHandler WindowEnter
        {
            add => eventHandlerList.AddHandler(windowEnterEventKey, value); remove => eventHandlerList.RemoveHandler(windowEnterEventKey, value);
        }

        public event EventHandler WindowLeave
        {
            add => eventHandlerList.AddHandler(windowLeaveEventKey, value); remove => eventHandlerList.RemoveHandler(windowLeaveEventKey, value);
        }

        public event EventHandler WindowFocusGained
        {
            add => eventHandlerList.AddHandler(windowFocusGainedEventKey, value); remove => eventHandlerList.RemoveHandler(windowFocusGainedEventKey, value);
        }

        public event EventHandler WindowFocusLost
        {
            add => eventHandlerList.AddHandler(windowFocusLostEventKey, value); remove => eventHandlerList.RemoveHandler(windowFocusLostEventKey, value);
        }

        public event EventHandler WindowClose
        {
            add => eventHandlerList.AddHandler(windowCloseEventKey, value); remove => eventHandlerList.RemoveHandler(windowCloseEventKey, value);
        }

        public event EventHandler WindowTakeFocus
        {
            add => eventHandlerList.AddHandler(windowTakeFocusEventKey, value); remove => eventHandlerList.RemoveHandler(windowTakeFocusEventKey, value);
        }

        public event RendererEventHandler RendererPaint
        {
            add => eventHandlerList.AddHandler(rendererPaintEventKey, value); remove => eventHandlerList.RemoveHandler(rendererPaintEventKey, value);
        }

        public event MouseButtonEventHandler MouseButtonDown
        {
            add => eventHandlerList.AddHandler(mouseButtonDownEventKey, value); remove => eventHandlerList.RemoveHandler(mouseButtonDownEventKey, value);
        }

        public event MouseButtonEventHandler MouseButtonUp
        {
            add => eventHandlerList.AddHandler(mouseButtonUpEventKey, value); remove => eventHandlerList.RemoveHandler(mouseButtonUpEventKey, value);
        }

        public event MouseMotionEventHandler MouseMove
        {
            add => eventHandlerList.AddHandler(mouseMoveEventKey, value); remove => eventHandlerList.RemoveHandler(mouseMoveEventKey, value);
        }

        public event MouseWheelEventHandler MouseWheel
        {
            add => eventHandlerList.AddHandler(mouseWheelEventKey, value); remove => eventHandlerList.RemoveHandler(mouseWheelEventKey, value);
        }

        public event KeyEventHandler KeyDown
        {
            add => eventHandlerList.AddHandler(keyDownEventKey, value); remove => eventHandlerList.RemoveHandler(keyDownEventKey, value);
        }

        public event KeyEventHandler KeyUp
        {
            add => eventHandlerList.AddHandler(keyUpEventKey, value); remove => eventHandlerList.RemoveHandler(keyUpEventKey, value);
        }

        public event TextInputEventHandler TextInput
        {
            add => eventHandlerList.AddHandler(textInputEventKey, value); remove => eventHandlerList.RemoveHandler(textInputEventKey, value);
        }

        public WindowCloseOperation CloseOperation
        {
            get => closeOperation;
            set => closeOperation = value;
        }

        public IntPtr Handle => handle;
        public bool HandleCreated => handle != IntPtr.Zero;
        public uint WindowID => windowID;

        public string Title
        {
            get => title;
            set
            {
                if (!string.Equals(title, value))
                {
                    title = value ?? string.Empty;
                    if (HandleCreated)
                    {
                        SDL_SetWindowTitle(handle, title);
                    }
                }
            }
        }

        public Point Position
        {
            get => new Point(x, y);
            set
            {
                if (x != value.X || y != value.Y)
                {
                    SetPosition(value.X, value.Y);
                }
            }
        }

        public Size Size
        {
            get => new Size(width, height);
            set
            {
                if (width != value.Width || height != value.Height)
                {
                    SetSize(value.Width, value.Height);
                }
            }
        }

        public Size MinimumSize
        {
            get => new Size(minWidth, minHeight);
        }

        public Size MaximumSize
        {
            get => new Size(maxWidth, maxHeight);
        }

        public int X
        {
            get => x;
            set
            {
                if (x != value)
                {
                    SetPosition(value, y);
                }
            }
        }

        public int Y
        {
            get => y;
            set
            {
                if (y != value)
                {
                    SetPosition(x, value);
                }
            }
        }

        public int Width
        {
            get => width;
            set
            {
                if (width != value)
                {
                    SetSize(value, height);
                }
            }
        }

        public int Height
        {
            get => height;
            set
            {
                if (height != value)
                {
                    SetSize(width, value);
                }
            }
        }
        public bool Visible
        {
            get => visible;
            set
            {
                if (visible != value)
                {
                    visible = value;
                    if (HandleCreated)
                    {
                        if (value)
                        {
                            SDL_ShowWindow(handle);
                        }
                        else
                        {
                            SDL_HideWindow(handle);
                        }
                    }
                }
            }
        }

        public bool FullScreen
        {
            get => fullScreen;
            set
            {
                if (fullScreen != value)
                {
                    fullScreen = value;
                    if (HandleCreated)
                    {
                        if (value)
                        {
                            SetFullScreen();
                        }
                        else
                        {
                            SetWindowed();
                        }
                    }
                }
            }
        }

        private void SetFullScreen()
        {
            if (useFakeFullScreen)
            {
                SetFakeFullScreen();
            }
            else
            {
                _ = SDL_SetWindowFullscreen(handle, SDL_WindowFlags.FULLSCREEN_DESKTOP);
            }
        }

        private void SetWindowed()
        {
            if (useFakeFullScreen)
            {
                SetFakeWindowed();
            }
            else
            {
                _ = SDL_SetWindowFullscreen(handle, 0);
            }
        }

        private void SetFakeFullScreen()
        {
            oldX = x;
            oldY = y;
            oldWidth = width;
            oldHeight = height;
            int index = SDL_GetWindowDisplayIndex(handle);
            _ = SDL_GetDisplayBounds(index, out Rectangle bounds);
            SDL_SetWindowBordered(handle, false);
            SDL_SetWindowResizable(handle, false);
            SDL_SetWindowTitle(handle, IntPtr.Zero);
            SDL_SetWindowAlwaysOnTop(handle, true);
            SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }

        private void SetFakeWindowed()
        {
            SetBounds(oldX, oldY, oldWidth, oldHeight);
            SDL_SetWindowBordered(handle, true);
            SDL_SetWindowResizable(handle, resizable);
            SDL_SetWindowTitle(handle, title);
            SDL_SetWindowAlwaysOnTop(handle, alwaysOnTop);
            SetBounds(oldX, oldY, oldWidth, oldHeight);
        }

        public bool Resizable
        {
            get => resizable;
            set
            {
                if (resizable != value)
                {
                    resizable = value;
                    if (HandleCreated)
                    {
                        SDL_SetWindowResizable(handle, resizable);
                    }
                }
            }
        }

        public bool AlwaysOnTop
        {
            get => alwaysOnTop;
            set
            {
                if (alwaysOnTop != value)
                {
                    alwaysOnTop = value;
                    if (HandleCreated)
                    {
                        SDL_SetWindowAlwaysOnTop(handle, alwaysOnTop);
                    }
                }
            }
        }

        public void Show()
        {
            if (HandleCreated)
            {
                SDL_ShowWindow(handle);
            }
            else
            {
                CreateHandle();
            }
        }

        public void Hide()
        {
            if (HandleCreated)
            {
                SDL_HideWindow(handle);
            }
            else
            {

            }
        }

        public void Close()
        {
            Dispose();
        }

        public void SetPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
            if (HandleCreated) { SDL_SetWindowPosition(handle, x, y); }
        }

        public void SetSize(int w, int h)
        {
            width = w;
            height = h;
            if (HandleCreated) { SDL_SetWindowSize(handle, w, h); }
        }

        public void SetBounds(int x, int y, int w, int h)
        {
            SetSize(w, h);
            SetPosition(x, y);
        }

        public SDLTexture? LoadTexture(string fileName)
        {
            return renderer?.LoadTexture(fileName);
        }

        public SDLTexture? LoadTexture(byte[] data)
        {
            return renderer?.LoadTexture(data);
        }

        public static SDLFont? LoadFont(string fileName, int ySize)
        {
            return SDLRenderer.LoadFont(fileName, ySize);
        }

        public static SDLFont? LoadFont(byte[] data, int ySize)
        {
            return SDLRenderer.LoadFont(data, ySize);
        }

        internal void RaiseLoad()
        {
            Console.WriteLine($"Window {windowID} Load");
            OnWindowLoad(EventArgs.Empty);
        }

        protected virtual void OnWindowLoad(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowLoadEventKey])?.Invoke(this, e);
        }

        internal void RaiseShown()
        {
            visible = true;
            Console.WriteLine($"Window {windowID} Shown");
            OnWindowShown(EventArgs.Empty);
        }

        protected virtual void OnWindowShown(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowShownEventKey])?.Invoke(this, e);
        }
        internal void RaiseHidden()
        {
            visible = false;
            Console.WriteLine($"Window {windowID} Hidden");
            OnWindowHidden(EventArgs.Empty);
        }

        protected virtual void OnWindowHidden(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowHiddenEventKey])?.Invoke(this, e);
        }

        internal void RaiseExposed()
        {
            Console.WriteLine($"Window {windowID} Exposed");
            OnWindowExposed(EventArgs.Empty);
        }
        protected virtual void OnWindowExposed(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowExposedEventKey])?.Invoke(this, e);
        }

        internal void RaiseMoved()
        {
            SDL_GetWindowPosition(handle, out x, out y);
            Console.WriteLine($"Window {windowID} Moved: {x} x {y}");
            OnWindowMoved(EventArgs.Empty);
        }
        protected virtual void OnWindowMoved(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowMovedEventKey])?.Invoke(this, e);
        }
        internal void RaiseResized()
        {
            UpdateWindowSize();
            Console.WriteLine($"Window {windowID} Resized: {width} x {height}");
            OnWindowResized(EventArgs.Empty);
        }

        protected virtual void OnWindowResized(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowResizedEventKey])?.Invoke(this, e);
        }

        internal void RaiseSizeChanged()
        {
            UpdateWindowSize();
            Console.WriteLine($"Window {windowID} Size Changed: {width} x {height}");
            OnWindowSizeChanged(EventArgs.Empty);
        }
        protected virtual void OnWindowSizeChanged(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowSizeChangedEventKey])?.Invoke(this, e);
        }

        private void UpdateWindowSize()
        {
            SDL_GetWindowSize(handle, out width, out height);
            renderer?.WindowResized(width, height);
        }
        internal void RaiseMinimized()
        {
            SDL_GetWindowPosition(handle, out x, out y);
            SDL_GetWindowSize(handle, out width, out height);
            Console.WriteLine($"Window {windowID} Minimized: {x} x {y} - {width} x {height}");
            OnWindowMinimized(EventArgs.Empty);
        }
        protected virtual void OnWindowMinimized(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowMinimizedEventKey])?.Invoke(this, e);
        }
        internal void RaiseMaximized()
        {
            SDL_GetWindowPosition(handle, out x, out y);
            SDL_GetWindowSize(handle, out width, out height);
            Console.WriteLine($"Window {windowID} Maximized: {x} x {y} - {width} x {height}");
            OnWindowMaximized(EventArgs.Empty);
        }
        protected virtual void OnWindowMaximized(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowMaximizedEventKey])?.Invoke(this, e);
        }
        internal void RaiseRestored()
        {
            SDL_GetWindowPosition(handle, out x, out y);
            SDL_GetWindowSize(handle, out width, out height);
            Console.WriteLine($"Window {windowID} Restored: {x} x {y} - {width} x {height}");
            OnWindowRestored(EventArgs.Empty);
        }
        protected virtual void OnWindowRestored(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowRestoredEventKey])?.Invoke(this, e);
        }
        internal void RaiseEnter()
        {
            Console.WriteLine($"Window {windowID} Enter");
            OnWindowEnter(EventArgs.Empty);
        }
        protected virtual void OnWindowEnter(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowEnterEventKey])?.Invoke(this, e);
        }
        internal void RaiseLeave()
        {
            Console.WriteLine($"Window {windowID} Leave");
            OnWindowLeave(EventArgs.Empty);
        }
        protected virtual void OnWindowLeave(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowLeaveEventKey])?.Invoke(this, e);
        }
        internal void RaiseFocusGained()
        {
            Console.WriteLine($"Window {windowID} Focus Gained");
            //EnsureFocus();
            OnWindowFocusGained(EventArgs.Empty);
        }
        protected virtual void OnWindowFocusGained(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowFocusGainedEventKey])?.Invoke(this, e);
        }
        internal void RaiseFocusLost()
        {
            Console.WriteLine($"Window {windowID} Focus Lost");
            OnWindowFocusLost(EventArgs.Empty);
        }
        protected virtual void OnWindowFocusLost(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowFocusLostEventKey])?.Invoke(this, e);
        }
        internal void RaiseClose()
        {
            Console.WriteLine($"Window {windowID} Close");
            OnWindowClose(EventArgs.Empty);
        }
        protected virtual void OnWindowClose(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowCloseEventKey])?.Invoke(this, e);
            switch (closeOperation)
            {
                case WindowCloseOperation.Close:
                    Close();
                    break;
                case WindowCloseOperation.Exit:
                    app.Exit();
                    break;
                case WindowCloseOperation.DoNothing:
                    break;
            }
        }

        internal void RaiseTakeFocus()
        {
            Console.WriteLine($"Window {windowID} Take Focus");
            //EnsureFocus();
            OnWindowTakeFocus(EventArgs.Empty);
        }

        protected virtual void OnWindowTakeFocus(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowTakeFocusEventKey])?.Invoke(this, e);
        }

        internal void RaisePaint(float deltaTime, float totalTime)
        {
            if (renderer != null)
            {
                renderer.BeginPaint();
                OnRendererPaint(new RendererEventArgs(renderer, deltaTime, totalTime));
                if (showFPS)
                {
                    renderer.ResetViewport();
                    renderer.DrawText(null, $"{app.FPS:F} fps", fpsPosX, fpsPosY, 0, 0, Color.White);
                }
                renderer.EndPaint();
            }
        }

        protected virtual void OnRendererPaint(RendererEventArgs e)
        {
            ((RendererEventHandler?)eventHandlerList[rendererPaintEventKey])?.Invoke(this, e);
        }

        internal void RaiseMouseButtonDown(int which, int x, int y, MouseButton button, KeyButtonState state, int clicks)
        {
            //EnsureFocus();
            OnMouseButtonDown(new MouseButtonEventArgs(which, x, Y, button, state, clicks));
        }

        protected virtual void OnMouseButtonDown(MouseButtonEventArgs e)
        {
            //Console.WriteLine($"Window {windowID}: {e}");
            ((MouseButtonEventHandler?)eventHandlerList[mouseButtonDownEventKey])?.Invoke(this, e);
        }
        internal void RaiseMouseButtonUp(int which, int x, int y, MouseButton button, KeyButtonState state, int clicks)
        {
            OnMouseButtonUp(new MouseButtonEventArgs(which, x, Y, button, state, clicks));
        }
        protected virtual void OnMouseButtonUp(MouseButtonEventArgs e)
        {
            //Console.WriteLine($"Window {windowID}: {e}");
            ((MouseButtonEventHandler?)eventHandlerList[mouseButtonUpEventKey])?.Invoke(this, e);
        }
        internal void RaiseMouseMove(int which, int x, int y, MouseButton button, int clicks, int relX, int relY)
        {
            OnMouseMove(new MouseMotionEventArgs(which, x, y, relX, relY));
        }
        protected virtual void OnMouseMove(MouseMotionEventArgs e)
        {
            //Console.WriteLine($"Window {windowID}: {e}");
            ((MouseMotionEventHandler?)eventHandlerList[mouseMoveEventKey])?.Invoke(this, e);
        }

        internal void RaiseMouseWheel(int which, int x, int y, float preciseX, float preciseY, MouseWheelDirection direction)
        {
            OnMouseWheel(new MouseWheelEventArgs(which, x, y, preciseX, preciseY, direction));
        }

        protected virtual void OnMouseWheel(MouseWheelEventArgs e)
        {
            //Console.WriteLine($"Window {windowID}: {e}");
            ((MouseWheelEventHandler?)eventHandlerList[mouseWheelEventKey])?.Invoke(this, e);
        }

        internal void RaiseKeyDown(ScanCode scanCode, KeyCode keyCode, KeyMod keyMod, KeyButtonState state, bool repeat)
        {
            OnKeyDown(new KeyEventArgs(scanCode, keyCode, keyMod, state, repeat));
        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            Console.WriteLine($"Window {windowID}: {e}");
            ((KeyEventHandler?)eventHandlerList[keyDownEventKey])?.Invoke(this, e);
        }

        internal void RaiseKeyUp(ScanCode scanCode, KeyCode keyCode, KeyMod keyMod, KeyButtonState state, bool repeat)
        {
            OnKeyUp(new KeyEventArgs(scanCode, keyCode, keyMod, state, repeat));
        }

        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            Console.WriteLine($"Window {windowID}: {e}");
            ((KeyEventHandler?)eventHandlerList[keyUpEventKey])?.Invoke(this, e);
        }

        internal void RaiseTextInput(string text)
        {
            OnTextInput(new TextInputEventArgs(text));
        }

        protected virtual void OnTextInput(TextInputEventArgs e)
        {
            Console.WriteLine($"Window {windowID}: {e}");
            ((TextInputEventHandler?)eventHandlerList[textInputEventKey])?.Invoke(this, e);
        }

        //internal bool EnsureFocus()
        //{
        //    IntPtr currentFocus = SDLApplication.SDL_GetKeyboardFocus();
        //    if (currentFocus != IntPtr.Zero) return true;
        //    IntPtr hwnd = GetWindowHandle();
        //    SetActiveWindow(hwnd);
        //    SetForegroundWindow(hwnd);
        //    SetActiveWindow(hwnd);
        //    SetForegroundWindow(hwnd);
        //    //if (!SetForegroundWindow(hwnd))
        //    //{
        //    //    SDL_RaiseWindow(handle);
        //    //}
        //    SDLApplication.FlushWindowEvents();
        //    //SDL_RaiseWindow(handle);
        //    //IntPtr hwnd = GetWindowHandle();
        //    //if (hwnd != IntPtr.Zero)
        //    //{
        //    //    SetActiveWindow(hwnd);
        //    //}
        //    return false;


        //    //IntPtr handle = SDLApplication.SDL_GetKeyboardFocus();
        //    //if (handle == IntPtr.Zero)
        //    //{
        //    //    //SDLApplication.FlushWindowEvents();
        //    //    Console.WriteLine($"Window {windowID}: No Focus.");
        //    //    IntPtr hwnd = GetWindowHandle();
        //    //    if (hwnd != IntPtr.Zero)
        //    //    {
        //    //        if (SetForegroundWindow(hwnd))
        //    //        {
        //    //            Console.WriteLine($"Window {windowID}: SetForeground.");
        //    //        }
        //    //        else
        //    //        {
        //    //            if (SetActiveWindow(hwnd) == IntPtr.Zero)
        //    //            {
        //    //                Console.WriteLine($"Window {windowID}: SetActive && SetForeground Failed.");
        //    //            }
        //    //            else
        //    //            {
        //    //                Console.WriteLine($"Window {windowID}: SetActive.");
        //    //            }
        //    //        }
        //    //    }
        //    //    //SDLApplication.FlushWindowEvents();
        //    //    SDL_RaiseWindow(handle);
        //    //    //SDL_RaiseWindow(handle);
        //    //    //SDL_SetWindowKeyboardGrab(handle, true);
        //    //}
        //    //else if (handle != this.handle)
        //    //{
        //    //    Console.WriteLine($"Window {windowID}: Other Focus.");
        //    //    return true;
        //    //}
        //    //else
        //    //{
        //    //    Console.WriteLine($"Window {windowID}: OK Focus.");
        //    //    return true;
        //    //}
        //}

        private void CreateHandle()
        {
            SDL_WindowFlags flags = GetCreateWindowFlags();
            int x = GetCreateWindowPosX();
            int y = GetCreateWindowPosY();
            int w = width;
            int h = height;
            handle = SDL_CreateWindow(title, x, y, w, h, flags);
            if (handle != IntPtr.Zero)
            {
                SDLNative.SDL_SetHint(SDLNative.SDL_HINT_RENDER_SCALE_QUALITY, textureFilter ? "1" : "0");

                windowID = SDL_GetWindowID(handle);
                SDL_GetWindowPosition(handle, out this.x, out this.y);
                SDL_GetWindowSize(handle, out width, out height);
                SDL_GetWindowMinimumSize(handle, out minWidth, out minHeight);
                SDL_GetWindowMaximumSize(handle, out maxWidth, out maxHeight);

                renderer = new SDLRenderer(this);
                RaiseLoad();
            }
        }

        private int GetCreateWindowPosX()
        {
            if (x < 0)
            {
                return SDL_WINDOWPOS_CENTERED_DISPLAY(0);
            }
            return x;
        }
        private int GetCreateWindowPosY()
        {
            if (y < 0)
            {
                return SDL_WINDOWPOS_CENTERED_DISPLAY(0);
            }
            return y;
        }

        private IntPtr GetWindowHandle()
        {
            SDL_SysWMinfo info = new SDL_SysWMinfo();
            SDL_VERSION(out info.version);
            info.subsystem = SDL_SYSWM_TYPE.SDL_SYSWM_WINDOWS;
            if (SDL_GetWindowWMInfo(handle, ref info))
            {
                return info.info.win.window;
            }
            return IntPtr.Zero;
        }

        private SDL_WindowFlags GetCreateWindowFlags()
        {
            SDL_WindowFlags flags = SDL_WindowFlags.ALLOW_HIGHDPI;// | SDL_WindowFlags.INPUT_FOCUS;
            if (resizable) { flags |= SDL_WindowFlags.RESIZABLE; }
            if (visible) { flags |= SDL_WindowFlags.SHOWN; }
            if (fullScreen) { flags |= SDL_WindowFlags.FULLSCREEN_DESKTOP; }
            return flags;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    app.RemoveWindow(this);
                }
                renderer?.Dispose();
                renderer = null;
                if (handle != IntPtr.Zero)
                {
                    SDL_DestroyWindow(handle);
                    handle = IntPtr.Zero;
                }
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~SDLWindow()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        [Flags]
        internal enum SDL_WindowFlags : uint
        {
            FULLSCREEN = 0x00000001,
            OPENGL = 0x00000002,
            SHOWN = 0x00000004,
            HIDDEN = 0x00000008,
            BORDERLESS = 0x00000010,
            RESIZABLE = 0x00000020,
            MINIMIZED = 0x00000040,
            MAXIMIZED = 0x00000080,
            MOUSE_GRABBED = 0x00000100,
            INPUT_FOCUS = 0x00000200,
            MOUSE_FOCUS = 0x00000400,
            FULLSCREEN_DESKTOP = (FULLSCREEN | 0x00001000),
            FOREIGN = 0x00000800,
            ALLOW_HIGHDPI = 0x00002000,
            MOUSE_CAPTURE = 0x00004000,
            ALWAYS_ON_TOP = 0x00008000,
            SKIP_TASKBAR = 0x00010000,
            UTILITY = 0x00020000,
            TOOLTIP = 0x00040000,
            POPUP_MENU = 0x00080000,
            KEYBOARD_GRABBED = 0x00100000,
            VULKAN = 0x10000000,
            METAL = 0x2000000,
            INPUT_GRABBED = MOUSE_GRABBED
        };

        public enum SDL_HitTestResult
        {
            NORMAL,
            DRAGGABLE,
            RESIZE_TOPLEFT,
            RESIZE_TOP,
            RESIZE_TOPRIGHT,
            RESIZE_RIGHT,
            RESIZE_BOTTOMRIGHT,
            RESIZE_BOTTOM,
            RESIZE_BOTTOMLEFT,
            RESIZE_LEFT
        };

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate SDL_HitTestResult SDL_HitTest(IntPtr win, IntPtr area, IntPtr data);


        private const int SDL_WINDOWPOS_UNDEFINED_MASK = 0x1FFF0000;
        private const int SDL_WINDOWPOS_CENTERED_MASK = 0x2FFF0000;
        private const int SDL_WINDOWPOS_UNDEFINED = 0x1FFF0000;
        private const int SDL_WINDOWPOS_CENTERED = 0x2FFF0000;

        private static int SDL_WINDOWPOS_UNDEFINED_DISPLAY(int X)
        {
            return (SDL_WINDOWPOS_UNDEFINED_MASK | X);
        }

        private static bool SDL_WINDOWPOS_ISUNDEFINED(int X)
        {
            return (X & 0xFFFF0000) == SDL_WINDOWPOS_UNDEFINED_MASK;
        }

        private static int SDL_WINDOWPOS_CENTERED_DISPLAY(int X)
        {
            return (SDL_WINDOWPOS_CENTERED_MASK | X);
        }

        private static bool SDL_WINDOWPOS_ISCENTERED(int X)
        {
            return (X & 0xFFFF0000) == SDL_WINDOWPOS_CENTERED_MASK;
        }
        private enum SDL_FlashOperation
        {
            CANCEL,
            BRIEFLY,
            UNTIL_FOCUSED
        };

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_DisplayMode
        {
            public uint format;
            public int w;
            public int h;
            public int refresh_rate;
            public IntPtr driverdata;
        }

        private enum SDL_SYSWM_TYPE
        {
            SDL_SYSWM_UNKNOWN,
            SDL_SYSWM_WINDOWS,
            SDL_SYSWM_X11,
            SDL_SYSWM_DIRECTFB,
            SDL_SYSWM_COCOA,
            SDL_SYSWM_UIKIT,
            SDL_SYSWM_WAYLAND,
            SDL_SYSWM_MIR,
            SDL_SYSWM_WINRT,
            SDL_SYSWM_ANDROID,
            SDL_SYSWM_VIVANTE,
            SDL_SYSWM_OS2,
            SDL_SYSWM_HAIKU,
            SDL_SYSWM_KMSDRM
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct INTERNAL_windows_wminfo
        {
            public IntPtr window; // Refers to an HWND
            public IntPtr hdc; // Refers to an HDC
            public IntPtr hinstance; // Refers to an HINSTANCE
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct INTERNAL_SysWMDriverUnion
        {
            [FieldOffset(0)]
            public INTERNAL_windows_wminfo win;
            //[FieldOffset(0)]
            //public INTERNAL_winrt_wminfo winrt;
            //[FieldOffset(0)]
            //public INTERNAL_x11_wminfo x11;
            //[FieldOffset(0)]
            //public INTERNAL_directfb_wminfo dfb;
            //[FieldOffset(0)]
            //public INTERNAL_cocoa_wminfo cocoa;
            //[FieldOffset(0)]
            //public INTERNAL_uikit_wminfo uikit;
            //[FieldOffset(0)]
            //public INTERNAL_wayland_wminfo wl;
            //[FieldOffset(0)]
            //public INTERNAL_mir_wminfo mir;
            //[FieldOffset(0)]
            //public INTERNAL_android_wminfo android;
            //[FieldOffset(0)]
            //public INTERNAL_os2_wminfo os2;
            //[FieldOffset(0)]
            //public INTERNAL_vivante_wminfo vivante;
            //[FieldOffset(0)]
            //public INTERNAL_kmsdrm_wminfo ksmdrm;
            // private int dummy;
        }

        public const int SDL_MAJOR_VERSION = 2;
        public const int SDL_MINOR_VERSION = 0;
        public const int SDL_PATCHLEVEL = 20;

        private static void SDL_VERSION(out SDL_version x)
        {
            x.major = SDL_MAJOR_VERSION;
            x.minor = SDL_MINOR_VERSION;
            x.patch = SDL_PATCHLEVEL;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_version
        {
            public byte major;
            public byte minor;
            public byte patch;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_SysWMinfo
        {
            public SDL_version version;
            public SDL_SYSWM_TYPE subsystem;
            public INTERNAL_SysWMDriverUnion info;
        }

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateWindow([In()][MarshalAs(UnmanagedType.LPUTF8Str)] string title, int x, int y, int w, int h, SDL_WindowFlags flags);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_CreateWindowAndRenderer(int width, int height, SDL_WindowFlags window_flags, out IntPtr window, out IntPtr renderer);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateWindowFrom(IntPtr data);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_DestroyWindow(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern float SDL_GetWindowBrightness(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetWindowOpacity(IntPtr window, float opacity);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetWindowOpacity(IntPtr window, out float opacity);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetWindowModalFor(IntPtr modal_window, IntPtr parent_window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetWindowInputFocus(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetWindowData(IntPtr window, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string name);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetWindowDisplayIndex(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetWindowDisplayMode(IntPtr window, out SDLNative.SDL_DisplayMode mode);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetWindowICCProfile(IntPtr window, out IntPtr mode);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern SDL_WindowFlags SDL_GetWindowFlags(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetWindowFromID(uint id);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetWindowGammaRamp(IntPtr window,
            [Out()][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)] ushort[] red,
            [Out()][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)] ushort[] green,
            [Out()][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)] ushort[] blue);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SDL_GetWindowGrab(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SDL_GetWindowKeyboardGrab(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SDL_GetWindowMouseGrab(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SDL_GetWindowID(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SDL_GetWindowPixelFormat(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_GetWindowMaximumSize(IntPtr window, out int max_w, out int max_h);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_GetWindowMinimumSize(IntPtr window, out int min_w, out int min_h);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_GetWindowPosition(IntPtr window, out int x, out int y);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_GetWindowSize(IntPtr window, out int w, out int h);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetWindowTitle(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_HideWindow(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetWindowFullscreen(IntPtr window, SDL_WindowFlags flags);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetWindowGammaRamp(IntPtr window,
           [In()][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)] ushort[] red,
           [In()][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)] ushort[] green,
           [In()][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeConst = 256)] ushort[] blue);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowGrab(IntPtr window, [In()][MarshalAs(UnmanagedType.Bool)] bool grabbed);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowKeyboardGrab(IntPtr window, [In()][MarshalAs(UnmanagedType.Bool)] bool grabbed);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowMouseGrab(IntPtr window, [In()][MarshalAs(UnmanagedType.Bool)] bool grabbed);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowIcon(IntPtr window, IntPtr icon);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowMaximumSize(IntPtr window, int max_w, int max_h);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowMinimumSize(IntPtr window, int min_w, int min_h);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowPosition(IntPtr window, int x, int y);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowSize(IntPtr window, int w, int h);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowBordered(IntPtr window, [In()][MarshalAs(UnmanagedType.Bool)] bool bordered);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetWindowBordersSize(IntPtr window, out int top, out int left, out int bottom, out int right);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowResizable(IntPtr window, [In()][MarshalAs(UnmanagedType.Bool)] bool resizable);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowAlwaysOnTop(IntPtr window, [In()][MarshalAs(UnmanagedType.Bool)] bool onTop);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowTitle(IntPtr window, [In][MarshalAs(UnmanagedType.LPUTF8Str)] string title);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowTitle(IntPtr window, IntPtr title);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_ShowWindow(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetWindowHitTest(IntPtr window, SDL_HitTest callback, IntPtr callback_data);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetGrabbedWindow();
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetWindowMouseRect(IntPtr window, [In()] ref Rectangle rect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetWindowMouseRect(IntPtr window, IntPtr rect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetWindowMouseRect(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_FlashWindow(IntPtr window, SDL_FlashOperation operation);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_MaximizeWindow(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_MinimizeWindow(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_RaiseWindow(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_RestoreWindow(IntPtr window);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowBrightness(IntPtr window, float brightness);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_SetWindowData(IntPtr window, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string name, IntPtr userdata);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetWindowDisplayMode(IntPtr window, [In()] ref SDL_DisplayMode mode);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetWindowDisplayMode(IntPtr window, IntPtr mode);

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SDL_GetWindowWMInfo(IntPtr window, ref SDL_SysWMinfo info);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetDisplayBounds(int displayIndex, out Rectangle rect);


        //[DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //private static extern bool SetForegroundWindow(IntPtr hWnd);
        //[DllImport("user32.dll")]
        //private static extern IntPtr SetActiveWindow(IntPtr hWnd);

    }
}
