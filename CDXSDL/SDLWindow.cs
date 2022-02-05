namespace CDXSDL
{
    using CDX;
    using CDX.App;
    using CDX.Graphics;
    using CDX.Input;
    using CDX.Logging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    internal class SDLWindow : CDXWindow
    {
        private IntPtr handle;
        private uint windowID;
        private readonly SDLApplication app;
        private SDLRenderer? renderer;
        private int oldX;
        private int oldY;
        private int oldWidth;
        private int oldHeight;
        private UpdateEventArgs? updateEventArgs;

        public SDLWindow(SDLApplication app, Window window)
            : base(app, window)
        {
            this.app = app;
        }
        internal SDLApplication App => app;

        public IntPtr Handle => handle;
        public bool HandleCreated => handle != IntPtr.Zero;
        public override uint WindowID => windowID;


        public override void SetTitle(string title)
        {
            if (HandleCreated)
            {
                SDL_SetWindowTitle(handle, title);
            }
        }

        public override void SetVisible(bool visible)
        {
            if (HandleCreated)
            {
                if (visible)
                {
                    SDL_ShowWindow(handle);
                }
                else
                {
                    SDL_HideWindow(handle);
                }
            }
        }

        public override void SetResizable(bool resizable)
        {
            if (HandleCreated)
            {
                SDL_SetWindowResizable(handle, resizable);
            }
        }

        public override void SetAlwaysOnTop(bool alwaysOnTop)
        {
            if (HandleCreated)
            {
                SDL_SetWindowAlwaysOnTop(handle, alwaysOnTop);
            }
        }

        public override void SetBorderless(bool borderless)
        {
            if (HandleCreated)
            {
                SDL_SetWindowBordered(handle, !borderless);
            }
        }

        public override void SetFullScreen(bool fullScreen)
        {
            if (HandleCreated)
            {
                if (fullScreen)
                {
                    GoFullScreen();
                }
                else
                {
                    GoWindowed();
                }
            }
        }

        public override void SetPosition(int x, int y)
        {
            if (HandleCreated)
            {
                SDL_SetWindowPosition(handle, x, y);
            }
        }

        public override void SetSize(int width, int height)
        {
            if (HandleCreated)
            {
                Width = width;
                Height = height;
                SDL_SetWindowSize(handle, width, height);
            }
        }

        public override void SetBackBufferSize(int width, int height)
        {
            if (HandleCreated)
            {
                renderer?.SetBackBufferSize(width, height);
            }
        }

        public override void SetMousePosition(int x, int y)
        {
            if (HandleCreated)
            {
                SDL_WarpMouseInWindow(handle, x, y);
            }
        }

        public override void Show()
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

        public override void Hide()
        {
            if (HandleCreated)
            {
                SDL_HideWindow(handle);
            }
        }

        public override void Close()
        {
            app.RemoveWindow(this);
            renderer?.Dispose();
            if (HandleCreated)
            {
                SDL_DestroyWindow(handle);
                handle = IntPtr.Zero;
                Logger.Info($"SDL Window {windowID} destroyed");
            }
        }

        private void GoFullScreen()
        {
            if (UseExtremeFullScreen)
            {
                GoExtremeFullScreen();
            }
            else
            {
                GoNormalFullScreen();
            }
        }
        private void GoExtremeFullScreen()
        {
            SDL_GetWindowPosition(handle, out oldX, out oldY);
            SDL_GetWindowSize(handle, out oldWidth, out oldHeight);
            SDL_GetDisplayBounds(0, out Rectangle bounds);
            int numDisplays = SDL_GetNumVideoDisplays();
            for (int index = 1; index < numDisplays; index++)
            {
                _ = SDL_GetDisplayBounds(index, out Rectangle otherBounds);
                if (otherBounds.Height == bounds.Height)
                {
                    bounds = Rectangle.Union(bounds, otherBounds);

                }
                else
                {
                    break;
                }
            }
            SDL_SetWindowBordered(handle, false);
            SDL_SetWindowResizable(handle, false);
            SDL_SetWindowTitle(handle, IntPtr.Zero);
            SDL_SetWindowAlwaysOnTop(handle, true);
            SDL_SetWindowSize(handle, bounds.Width, bounds.Height);
            SDL_SetWindowPosition(handle, bounds.X, bounds.Y);
        }

        private void GoNormalFullScreen()
        {
            SDL_GetWindowPosition(handle, out oldX, out oldY);
            SDL_GetWindowSize(handle, out oldWidth, out oldHeight);
            int index = SDL_GetWindowDisplayIndex(handle);
            _ = SDL_GetDisplayBounds(index, out Rectangle bounds);
            SDL_SetWindowBordered(handle, false);
            SDL_SetWindowResizable(handle, false);
            SDL_SetWindowTitle(handle, IntPtr.Zero);
            SDL_SetWindowAlwaysOnTop(handle, true);
            SDL_SetWindowSize(handle, bounds.Width, bounds.Height);
            SDL_SetWindowPosition(handle, bounds.X, bounds.Y);
        }

        private void GoWindowed()
        {
            SDL_SetWindowSize(handle, oldWidth, oldHeight);
            SDL_SetWindowPosition(handle, oldX, oldY);
            SDL_SetWindowBordered(handle, !window.Borderless);
            SDL_SetWindowResizable(handle, window.Resizable);
            SDL_SetWindowTitle(handle, window.Title);
            SDL_SetWindowAlwaysOnTop(handle, window.AlwaysOnTop);
            //SDL_SetWindowSize(handle, oldWidth, oldHeight);
            //SDL_SetWindowPosition(handle, oldX, oldY);
        }

        private void ScaleMouseX(ref int x)
        {
            if (BackBufferWidth == 0) return;
            if (Width > BackBufferWidth)
            {
                double scale = (double)Width / BackBufferWidth;
                x = (int)(x / scale);
            }
            else if (Width < BackBufferWidth)
            {
                double scale = (double)BackBufferWidth / Width;
                x = (int)(x * scale);
            }
        }
        private void ScaleMouseY(ref int y)
        {
            if (BackBufferHeight == 0) return;
            if (Height > BackBufferHeight)
            {
                double scale = (double)Height / BackBufferHeight;
                y = (int)(y / scale);
            }
            else if (Height < BackBufferHeight)
            {
                double scale = (double)BackBufferHeight / Height;
                y = (int)(y * scale);
            }
        }

        internal void RaiseLoad()
        {
            OnWindowLoad(new LoadEventArgs(graphics, this));
        }

        internal void RaiseClose()
        {
            OnWindowClose(EventArgs.Empty);
        }
        internal void RaiseHidden()
        {
            OnWindowHidden(EventArgs.Empty);
        }

        internal void RaiseShown()
        {
            OnWindowShown(EventArgs.Empty);
        }

        internal void RaiseDisplayChanged()
        {
            OnWindowDisplayChanged(EventArgs.Empty);
        }

        internal void RaiseEnter()
        {
            OnWindowEnter(EventArgs.Empty);
        }
        internal void RaiseLeave()
        {
            OnWindowLeave(EventArgs.Empty);
        }
        internal void RaiseFocusGained()
        {
            OnWindowFocusGained(EventArgs.Empty);
        }
        internal void RaiseFocusLost()
        {
            OnWindowFocusLost(EventArgs.Empty);
        }
        internal void RaiseTakeFocus()
        {
            OnWindowTakeFocus(EventArgs.Empty);
        }

        internal void RaiseExposed()
        {
            OnWindowExposed(EventArgs.Empty);
        }
        internal void RaiseMaximized()
        {
            OnWindowMaximized(EventArgs.Empty);
        }
        internal void RaiseMinimized()
        {
            OnWindowMinimized(EventArgs.Empty);
        }
        internal void RaiseRestored()
        {
            OnWindowRestored(EventArgs.Empty);
        }

        internal void RaiseMoved()
        {
            SDL_GetWindowPosition(Handle, out int x, out int y);
            OnWindowMoved(new WindowPositionEventArgs(x, y));
        }
        internal void RaiseResized()
        {
            SDL_GetWindowSize(handle, out int w, out int h);
            Width = w;
            Height = h;
            renderer?.CheckSizes();
            OnWindowResized(new WindowSizeEventArgs(w, h));
        }
        internal void RaiseSizeChanged()
        {
            SDL_GetWindowSize(handle, out int w, out int h);
            Width = w;
            Height = h;
            renderer?.CheckSizes();
            OnWindowSizeChanged(new WindowSizeEventArgs(w, h));
        }

        internal void RaisePaint(FrameTime time)
        {
            if (renderer != null)
            {
                renderer.BeginPaint();
                OnWindowPaint(renderer.GetPaintEventArgs(time));
                if (showFPS)
                {
                    renderer.ResetViewport();
                    ((IGraphics)renderer).DrawText(null, Application.FPSText, fpsPosX, fpsPosY, Color.White);
                }
                renderer.EndPaint();
            }
        }

        internal void RaiseUpdate(FrameTime time)
        {
            OnWindowUpdate(GetUpdateEventArgs(time));
        }

        internal void RaiseMouseButtonDown(int which, int x, int y, MouseButton button, KeyButtonState state, int clicks)
        {
            ScaleMouseX(ref x);
            ScaleMouseY(ref y);
            OnMouseButtonDown(new MouseButtonEventArgs(which, x, y, button, state, clicks));
        }
        internal void RaiseMouseButtonUp(int which, int x, int y, MouseButton button, KeyButtonState state, int clicks)
        {
            ScaleMouseX(ref x);
            ScaleMouseY(ref y);
            OnMouseButtonUp(new MouseButtonEventArgs(which, x, y, button, state, clicks));
        }
        internal void RaiseMouseMove(int which, int x, int y, int relX, int relY)
        {
            ScaleMouseX(ref x);
            ScaleMouseY(ref y);
            ScaleMouseX(ref relX);
            ScaleMouseY(ref relY);
            OnMouseMove(new MouseMotionEventArgs(which, x, y, relX, relY));
        }
        internal void RaiseMouseWheel(int which, int x, int y, float preciseX, float preciseY, MouseWheelDirection direction)
        {
            OnMouseWheel(new MouseWheelEventArgs(which, x, y, preciseX, preciseY, direction));
        }
        internal void RaiseKeyDown(ScanCode scanCode, KeyCode keyCode, KeyMod keyMod, KeyButtonState state, bool repeat)
        {
            OnKeyDown(new KeyEventArgs(scanCode, keyCode, keyMod, state, repeat));
        }
        internal void RaiseKeyUp(ScanCode scanCode, KeyCode keyCode, KeyMod keyMod, KeyButtonState state, bool repeat)
        {
            OnKeyUp(new KeyEventArgs(scanCode, keyCode, keyMod, state, repeat));
        }
        internal void RaiseTextInput(string text)
        {
            OnTextInput(new TextInputEventArgs(text));
        }

        internal void RaiseControllerButtonDown(int which, ControllerButton button, KeyButtonState state)
        {
            OnControllerButtonDown(new ControllerButtonEventArgs(which, button, state));
        }
        internal void RaiseControllerButtonUp(int which, ControllerButton button, KeyButtonState state)
        {
            OnControllerButtonUp(new ControllerButtonEventArgs(which, button, state));
        }

        internal void RaiseControllerAxis(int which, ControllerAxis axis, int value, Vector2 dir)
        {
            OnControllerAxis(new ControllerAxisEventArgs(which, axis, value, dir));
        }

        internal void RaiseFingerDown(long touchId, long fingerId, float x, float y, float dx, float dy, float pressure)
        {
            OnTouchFingerDown(new TouchFingerEventArgs(touchId, fingerId, x, y, dx, dy, pressure));
        }
        internal void RaiseFingerUp(long touchId, long fingerId, float x, float y, float dx, float dy, float pressure)
        {
            OnTouchFingerUp(new TouchFingerEventArgs(touchId, fingerId, x, y, dx, dy, pressure));
        }
        internal void RaiseFingerMotion(long touchId, long fingerId, float x, float y, float dx, float dy, float pressure)
        {
            OnTouchFingerMotion(new TouchFingerEventArgs(touchId, fingerId, x, y, dx, dy, pressure));
        }

        private UpdateEventArgs GetUpdateEventArgs(FrameTime time)
        {
            if (updateEventArgs == null)
            {
                updateEventArgs = new UpdateEventArgs(time);
            }
            return updateEventArgs;
        }
        private void CreateHandle()
        {
            SDL_WindowFlags flags = GetCreateWindowFlags();
            int x = GetCreateWindowPosX();
            int y = GetCreateWindowPosY();
            int w = window.Width;
            int h = window.Height;
            handle = SDL_CreateWindow(window.Title, x, y, w, h, flags);
            if (handle != IntPtr.Zero)
            {
                windowID = SDL_GetWindowID(handle);
                Logger.Info($"SDL Window {windowID} created");
                Width = window.Width;
                Height = window.Height;
                BackBufferWidth = window.BackBufferWidth;
                BackBufferHeight = window.BackBufferHeight;
                renderer = new SDLRenderer(this);
                renderer.CheckSizes();
                graphics = renderer;
                if (window.FullScreen)
                {
                    GoFullScreen();
                }
                RaiseLoad();
            }
        }
        private int GetCreateWindowPosX()
        {
            if (window.X < 0)
            {
                return SDL_WINDOWPOS_CENTERED_DISPLAY(window.Display);
            }
            return window.X;
        }
        private int GetCreateWindowPosY()
        {
            if (window.Y < 0)
            {
                return SDL_WINDOWPOS_CENTERED_DISPLAY(window.Display);
            }
            return window.Y;
        }
        private SDL_WindowFlags GetCreateWindowFlags()
        {
            SDL_WindowFlags flags = SDL_WindowFlags.ALLOW_HIGHDPI;
            if (window.Resizable) { flags |= SDL_WindowFlags.RESIZABLE; }
            if (window.Visible) { flags |= SDL_WindowFlags.SHOWN; }
            //if (fullScreen) { flags |= SDL_WindowFlags.FULLSCREEN_DESKTOP; }
            return flags;
        }

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

        [Flags]
        private enum SDL_WindowFlags : uint
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

        private const string LibName = "SDL2";

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateWindow([In()][MarshalAs(UnmanagedType.LPUTF8Str)] string title, int x, int y, int w, int h, SDL_WindowFlags flags);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SDL_GetWindowID(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_DestroyWindow(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowResizable(IntPtr window, [In()][MarshalAs(UnmanagedType.Bool)] bool resizable);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowAlwaysOnTop(IntPtr window, [In()][MarshalAs(UnmanagedType.Bool)] bool onTop);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowTitle(IntPtr window, [In][MarshalAs(UnmanagedType.LPUTF8Str)] string title);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowTitle(IntPtr window, IntPtr title);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowBordered(IntPtr window, [In()][MarshalAs(UnmanagedType.Bool)] bool bordered);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_ShowWindow(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_HideWindow(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowPosition(IntPtr window, int x, int y);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetWindowSize(IntPtr window, int w, int h);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_GetWindowPosition(IntPtr window, out int x, out int y);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_GetWindowSize(IntPtr window, out int w, out int h);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetWindowDisplayIndex(IntPtr window);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetDisplayBounds(int displayIndex, out Rectangle rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_WarpMouseInWindow(IntPtr window, int x, int y);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetNumVideoDisplays();
    }
}
