namespace SDL2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    //using System.Threading.Tasks;

    public class SDLApplication
    {
        private readonly List<SDLWindow> windows = new();
        //private static SDLWindow? focusedWindow;
        //private static SDLWindow? desiredFocusedWindow;
        //private static bool settingFocus;
        //private static int focusAttempts;
        private bool initialized;
        private bool quitRequested;
        private uint maxFramesPerSecond = 60;
        private float fps = 60;
        private SDLFont? defaultFont;
        private readonly EventHandlerList eventHandlerList = new();

        private readonly object appExitEventKey = new();
        private readonly object appStartEventKey = new();


        private IntPtr evtMem;

        public IEnumerable<SDLWindow> OpenWindows => windows.Where(x => x.Visible);

        public float FPS => fps;

        public SDLWindow CreateWindow(string? title = null)
        {
            SDLWindow window = new SDLWindow(this, title);
            return window;
        }

        public SDLFont? DefaultFont
        {
            get => defaultFont;
            set => defaultFont = value;
        }

        public bool IsTextInputActive
        {
            get
            {
                if (initialized)
                {
                    return SDL_IsTextInputActive();
                }
                return false;
            }
            set
            {
                if (value)
                {
                    StartTextInput();
                }
                else
                {
                    StopTextInput();
                }
            }
        }

        public void StartTextInput()
        {
            if (initialized) { SDL_StartTextInput(); }
        }

        public void StopTextInput()
        {
            if (initialized) { SDL_StopTextInput(); }
        }


        public event EventHandler ApplicationExit
        {
            add => eventHandlerList.AddHandler(appExitEventKey, value); remove => eventHandlerList.RemoveHandler(appExitEventKey, value);
        }

        public event EventHandler ApplicationStart
        {
            add => eventHandlerList.AddHandler(appStartEventKey, value); remove => eventHandlerList.RemoveHandler(appStartEventKey, value);
        }

        private void RaiseApplicationExit()
        {
            OnApplicationExit(EventArgs.Empty);
        }

        private void RaiseApplicationStart()
        {
            OnApplicationStart(EventArgs.Empty);
        }

        private void OnApplicationExit(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[appExitEventKey])?.Invoke(this, e);
        }

        private void OnApplicationStart(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[appStartEventKey])?.Invoke(this, e);
        }
        internal void AddWindow(SDLWindow window)
        {
            windows.Add(window);
        }

        internal void RemoveWindow(SDLWindow window)
        {
            windows.Remove(window);
            if (windows.Count == 0)
            {
                Exit();
            }
        }

        public void Run(SDLWindow? window = null)
        {
            Initialize();
            window?.Show();
            MainLoop();
            Finish();
        }

        public void Exit()
        {
            quitRequested = true;
        }

        //public string GetError()
        //{
        //    IntPtr error = SDLNative.SDL_GetError();
        //    string? errorText = Marshal.PtrToStringUTF8(error);
        //    return errorText ?? "";
        //}

        private void Initialize()
        {
            if (!initialized)
            {
                var dllDir = Path.Combine(Environment.CurrentDirectory, IntPtr.Size == 4 ? "x86" : "x64");
                Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDir);
                SDLNative.SDL_SetMainReady();
                _ = SDLNative.SDL_SetHint(SDLNative.SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
                _ = SDLNative.SDL_SetHint(SDLNative.SDL_HINT_RENDER_DRIVER, "opengl");
                _ = SDLNative.SDL_SetHint(SDLNative.SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
                _ = SDLNative.SDL_SetHint(SDLNative.SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS, "0");
                _ = SDLNative.SDL_SetHint(SDLNative.SDL_HINT_RENDER_BATCHING, "1");
                _ = SDLNative.SDL_SetHint(SDLNative.SDL_HINT_GRAB_KEYBOARD, "1");
                _ = SDLNative.SDL_SetHint(SDLNative.SDL_HINT_ALLOW_ALT_TAB_WHILE_GRABBED, "1");
                _ = SDLNative.SDL_SetHint(SDLNative.SDL_BORDERLESS_WINDOWED_STYLE, "1");
                _ = SDLNative.SDL_SetHint(SDLNative.SDL_BORDERLESS_RESIZABLE_STYLE, "1");
                //_ = SDLNative.SDL_SetHint(SDLNative.SDL_HINT_ALLOW_TOPMOST, "0");
                _ = SDLNative.SDL_Init(SDLNative.InitFlags.Everything);
                _ = SDLImg.IMG_Init(SDLImg.IMG_InitFlags.IMG_INIT_PNG);
                _ = SDLFont.TTF_Init();
                initialized = true;
                defaultFont = SDLRenderer.LoadFont(Properties.Resources.Roboto_Regular, 15);
                evtMem = Marshal.AllocHGlobal(64);
                RaiseApplicationStart();
            }
        }

        private void Finish()
        {
            RaiseApplicationExit();
            while (windows.Count > 0)
            {
                SDLWindow window = windows[^1];
                window.Dispose();
            }
            if (initialized)
            {
                defaultFont?.Dispose();
                defaultFont = null;
                Marshal.FreeHGlobal(evtMem);
                SDLFont.TTF_Quit();
                SDLImg.IMG_Quit();
                SDLNative.SDL_Quit();
                initialized = false;
            }
        }
        private void MainLoop()
        {
            ulong deltaTime = 0;
            const float fpsAlpha = 0.2f;
            const float fpsAlphaNeg = 1.0f - fpsAlpha;
            int frameCounter = 0;
            float frameTime = maxFramesPerSecond;
            while (!quitRequested)
            {
                ulong lastUpdateTime = SDL_GetTicks64();
                MessageLoop(deltaTime / 1000.0f, lastUpdateTime / 1000.0f);
                ulong currentTime = SDL_GetTicks64();
                deltaTime = currentTime - lastUpdateTime;
                PaintLoop(deltaTime / 1000.0f, currentTime / 1000.0f);
                ulong endUpdateTime = SDL_GetTicks64();
                deltaTime = endUpdateTime - currentTime;
                uint maxTime = 1000 / maxFramesPerSecond;
                if (deltaTime < maxTime)
                {
                    SDL_Delay((uint)(maxTime - deltaTime));
                }
                ulong endFrameTime = SDL_GetTicks64();
                deltaTime = endFrameTime - lastUpdateTime;
                frameCounter++;
                if (frameCounter > maxFramesPerSecond / 4) { frameCounter = 0; }
                frameTime = fpsAlpha * (deltaTime / 1000.0f) + fpsAlphaNeg * frameTime;
                if (frameCounter == 0)
                {
                    fps = 1.0f / frameTime;
                }
            }
        }

        private void PaintLoop(float deltaTime, float totalTime)
        {
            foreach (SDLWindow window in OpenWindows)
            {
                window.RaisePaint(deltaTime, totalTime);
            }
        }
        private void MessageLoop(float deltaTime, float totalTime)
        {
            //SDL_Event evt = new SDL_Event();
            //int size = Marshal.SizeOf(evt);

            while (SDL_PollEvent(out SDL_Event evt) != 0 && !quitRequested)
            {
                switch (evt.type)
                {
                    case SDL_EventType.QUIT:
                        quitRequested = true;
                        break;
                    case SDL_EventType.WINDOWEVENT:
                        HandleWindowEvent(ref evt.window, deltaTime, totalTime);
                        //CheckFocusedWindow();
                        break;
                    case SDL_EventType.MOUSEBUTTONDOWN:
                    case SDL_EventType.MOUSEBUTTONUP:
                        HandleMouseButtonEvent(ref evt.button);
                        break;
                    case SDL_EventType.MOUSEMOTION:
                        HandleMouseMotionEvent(ref evt.motion);
                        break;
                    case SDL_EventType.MOUSEWHEEL:
                        HandleMouseWheelEvent(ref evt.wheel);
                        break;
                    case SDL_EventType.KEYDOWN:
                    case SDL_EventType.KEYUP:
                        HandleKeyEvent(ref evt.key);
                        break;
                    case SDL_EventType.TEXTINPUT:
                        Marshal.StructureToPtr(evt, evtMem, false);
                        HandleTextInput(ref evt.text);
                        break;
                }
            }
        }

        private void HandleTextInput(ref SDL_TextInputEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                byte[] data = new byte[64];
                Marshal.Copy(evtMem, data, 0, 64);
                int length = 0;
                while (data[length + 12] != 0 && length < SDL_TEXTINPUTEVENT_TEXT_SIZE)
                {
                    length++;
                }
                if (length > 0)
                {
                    string str = Encoding.UTF8.GetString(data, 12, length);
                    if (!string.IsNullOrEmpty(str))
                    {
                        window.RaiseTextInput(str);
                    }
                }
            }
        }

        private void HandleKeyEvent(ref SDL_KeyboardEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                if (evt.type == SDL_EventType.KEYDOWN)
                {
                    window.RaiseKeyDown(evt.keysym.scancode, evt.keysym.sym, evt.keysym.mod, (KeyButtonState)evt.state, evt.repeat != 0);
                }
                else if (evt.type == SDL_EventType.KEYUP)
                {
                    window.RaiseKeyUp(evt.keysym.scancode, evt.keysym.sym, evt.keysym.mod, (KeyButtonState)evt.state, evt.repeat != 0);
                }
            }
        }

        private void HandleMouseButtonEvent(ref SDL_MouseButtonEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                if (evt.type == SDL_EventType.MOUSEBUTTONDOWN)
                {
                    window.RaiseMouseButtonDown(evt.which, evt.x, evt.y, (MouseButton)evt.button, (KeyButtonState)evt.state, evt.clicks);
                }
                else if (evt.type == SDL_EventType.MOUSEBUTTONUP)
                {
                    window.RaiseMouseButtonUp(evt.which, evt.x, evt.y, (MouseButton)evt.button, (KeyButtonState)evt.state, evt.clicks);
                }
            }
        }
        private void HandleMouseMotionEvent(ref SDL_MouseMotionEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                window.RaiseMouseMove(evt.which, evt.x, evt.y, MouseButton.None, 0, evt.xrel, evt.yrel);
            }
        }

        private void HandleMouseWheelEvent(ref SDL_MouseWheelEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                window.RaiseMouseWheel(evt.which, evt.x, evt.y, evt.preciseX, evt.preciseY, (MouseWheelDirection)evt.direction);
            }
        }
        private void HandleWindowEvent(ref SDL_WindowEvent evt, float deltaTime, float totalTime)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                switch (evt.windowEvent)
                {
                    case SDL_WindowEventID.CLOSE:
                        window.RaiseClose();
                        break;
                    case SDL_WindowEventID.DISPLAY_CHANGED:
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                    case SDL_WindowEventID.ENTER:
                        window.RaiseEnter();
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                    case SDL_WindowEventID.EXPOSED:
                        window.RaiseExposed();
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                    case SDL_WindowEventID.FOCUS_GAINED:
                        window.RaiseFocusGained();
                        //if (!settingFocus)
                        //{
                        //    SetDesiredFocusedWindow(window);
                        //}
                        //SetFocusedWindow(window);
                        //window.RaiseFocusGained();
                        //SetFocusedWindow(window);
                        //window.RaisePaint(deltaTime, totalTime);

                        break;
                    case SDL_WindowEventID.FOCUS_LOST:
                        window.RaiseFocusLost();
                        //if (!settingFocus)
                        //{
                        //    window.RaiseFocusLost();
                        //}
                        break;
                    case SDL_WindowEventID.HIDDEN:
                        window.RaiseHidden();
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                    case SDL_WindowEventID.HIT_TEST:
                        break;
                    case SDL_WindowEventID.ICCPROF_CHANGED:
                        break;
                    case SDL_WindowEventID.LEAVE:
                        window.RaiseLeave();
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                    case SDL_WindowEventID.MAXIMIZED:
                        window.RaiseMaximized();
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                    case SDL_WindowEventID.MINIMIZED:
                        window.RaiseMinimized();
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                    case SDL_WindowEventID.MOVED:
                        window.RaiseMoved();
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                    case SDL_WindowEventID.RESIZED:
                        window.RaiseResized();
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                    case SDL_WindowEventID.RESTORED:
                        window.RaiseRestored();
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                    case SDL_WindowEventID.SHOWN:
                        window.RaiseShown();
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                    case SDL_WindowEventID.SIZE_CHANGED:
                        window.RaiseSizeChanged();
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                    case SDL_WindowEventID.TAKE_FOCUS:
                        window.RaiseTakeFocus();
                        window.RaisePaint(deltaTime, totalTime);
                        break;
                }
            }
        }

        private SDLWindow? GetWindowFromID(uint id)
        {
            foreach (SDLWindow window in windows)
            {
                if (window.WindowID == id)
                {
                    return window;
                }
            }
            return null;
        }

        private enum SDL_EventType : uint
        {
            QUIT = 0x100,
            APP_TERMINATING,
            APP_LOWMEMORY,
            APP_WILLENTERBACKGROUND,
            APP_DIDENTERBACKGROUND,
            APP_WILLENTERFOREGROUND,
            APP_DIDENTERFOREGROUND,
            LOCALECHANGED,
            DISPLAYEVENT = 0x150,
            WINDOWEVENT = 0x200,
            SYSWMEVENT,
            KEYDOWN = 0x300,
            KEYUP,
            TEXTEDITING,
            TEXTINPUT,
            KEYMAPCHANGED,
            MOUSEMOTION = 0x400,
            MOUSEBUTTONDOWN,
            MOUSEBUTTONUP,
            MOUSEWHEEL,
            JOYAXISMOTION = 0x600,
            JOYBALLMOTION,
            JOYHATMOTION,
            JOYBUTTONDOWN,
            JOYBUTTONUP,
            JOYDEVICEADDED,
            JOYDEVICEREMOVED,
            CONTROLLERAXISMOTION = 0x650,
            CONTROLLERBUTTONDOWN,
            CONTROLLERBUTTONUP,
            CONTROLLERDEVICEADDED,
            CONTROLLERDEVICEREMOVED,
            CONTROLLERDEVICEREMAPPED,
            CONTROLLERTOUCHPADDOWN,
            CONTROLLERTOUCHPADMOTION,
            CONTROLLERTOUCHPADUP,
            CONTROLLERSENSORUPDATE,
            FINGERDOWN = 0x700,
            FINGERUP,
            FINGERMOTION,
            DOLLARGESTURE = 0x800,
            DOLLARRECORD,
            MULTIGESTURE,
            CLIPBOARDUPDATE = 0x900,
            DROPFILE = 0x1000,
            DROPTEXT,
            DROPBEGIN,
            DROPCOMPLETE,
            AUDIODEVICEADDED = 0x1100,
            AUDIODEVICEREMOVED,
            SENSORUPDATE = 0x1200,
            RENDER_TARGETS_RESET = 0x2000,
            RENDER_DEVICE_RESET,
            POLLSENTINEL = 0x7F00,
            USEREVENT = 0x8000
        }

        private enum SDL_DisplayEventID : byte
        {
            NONE,
            ORIENTATION,
            CONNECTED,
            DISCONNECTED
        }

        private enum SDL_WindowEventID : byte
        {
            NONE,
            SHOWN,
            HIDDEN,
            EXPOSED,
            MOVED,
            RESIZED,
            SIZE_CHANGED,
            MINIMIZED,
            MAXIMIZED,
            RESTORED,
            ENTER,
            LEAVE,
            FOCUS_GAINED,
            FOCUS_LOST,
            CLOSE,
            TAKE_FOCUS,
            HIT_TEST,
            ICCPROF_CHANGED,
            DISPLAY_CHANGED
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_Keysym
        {
            public ScanCode scancode;
            public KeyCode sym;
            public KeyMod mod;
            public uint unicode;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_GenericEvent
        {
            public SDL_EventType type;
            public uint timestamp;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_DisplayEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public uint display;
            public SDL_DisplayEventID displayEvent;
            private byte padding1;
            private byte padding2;
            private byte padding3;
            public int data1;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_WindowEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public uint windowID;
            public SDL_WindowEventID windowEvent;
            private byte padding1;
            private byte padding2;
            private byte padding3;
            public int data1;
            public int data2;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_MouseMotionEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public uint windowID;
            public int which;
            public byte state;
            private byte padding1;
            private byte padding2;
            private byte padding3;
            public int x;
            public int y;
            public int xrel;
            public int yrel;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_MouseButtonEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public uint windowID;
            public int which;
            public byte button;
            public byte state;
            public byte clicks;
            private byte padding1;
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_MouseWheelEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public uint windowID;
            public int which;
            public int x;
            public int y;
            public uint direction;
            public float preciseX;
            public float preciseY;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_JoyAxisEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public int which;
            public byte axis;
            private byte padding1;
            private byte padding2;
            private byte padding3;
            public short axisValue;
            public ushort padding4;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_JoyBallEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public int which;
            public byte ball;
            private byte padding1;
            private byte padding2;
            private byte padding3;
            public short xrel;
            public short yrel;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_JoyHatEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public int which;
            public byte hat;
            public byte hatValue;
            private byte padding1;
            private byte padding2;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_JoyButtonEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public int which;
            public byte button;
            public byte state;
            private byte padding1;
            private byte padding2;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_JoyDeviceEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public int which;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_ControllerAxisEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public int which;
            public byte axis;
            private byte padding1;
            private byte padding2;
            private byte padding3;
            public short axisValue;
            private ushort padding4;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_ControllerButtonEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public int which;
            public byte button;
            public byte state;
            private byte padding1;
            private byte padding2;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_ControllerDeviceEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public int which;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_ControllerTouchpadEvent
        {
            public uint type;
            public uint timestamp;
            public int which;
            public int touchpad;
            public int finger;
            public float x;
            public float y;
            public float pressure;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_ControllerSensorEvent
        {
            public uint type;
            public uint timestamp;
            public int which;
            public int sensor;
            public float data1;
            public float data2;
            public float data3;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_AudioDeviceEvent
        {
            public uint type;
            public uint timestamp;
            public uint which;
            public byte iscapture;
            private byte padding1;
            private byte padding2;
            private byte padding3;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_TouchFingerEvent
        {
            public uint type;
            public uint timestamp;
            public long touchId;
            public long fingerId;
            public float x;
            public float y;
            public float dx;
            public float dy;
            public float pressure;
            public uint windowID;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_MultiGestureEvent
        {
            public uint type;
            public uint timestamp;
            public long touchId;
            public float dTheta;
            public float dDist;
            public float x;
            public float y;
            public ushort numFingers;
            public ushort padding;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_DollarGestureEvent
        {
            public uint type;
            public uint timestamp;
            public long touchId;
            public long gestureId;
            public uint numFingers;
            public float error;
            public float x;
            public float y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_DropEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public IntPtr file;
            public uint windowID;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_SensorEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public int which;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public float[] data;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_QuitEvent
        {
            public SDL_EventType type;
            public uint timestamp;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_KeyboardEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public uint windowID;
            public byte state;
            public byte repeat;
            private byte padding2;
            private byte padding3;
            public SDL_Keysym keysym;
        }

        private const int SDL_TEXTEDITINGEVENT_TEXT_SIZE = 32;
        private const int SDL_TEXTINPUTEVENT_TEXT_SIZE = 32;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SDL_TextInputEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public uint windowID;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = SDL_TEXTINPUTEVENT_TEXT_SIZE, ArraySubType = UnmanagedType.I1)]
            //public byte[] text;
            //[FieldOffset(63)]
            //public byte lastByte;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = SDL_TEXTINPUTEVENT_TEXT_SIZE)]
            //public byte[] text;
        }

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 64)]
        private struct SDL_Event
        {
            [FieldOffset(0)]
            public SDL_EventType type;
            [FieldOffset(0)]
            public SDL_QuitEvent quit;
            [FieldOffset(0)]
            public SDL_WindowEvent window;
            [FieldOffset(0)]
            public SDL_MouseMotionEvent motion;
            [FieldOffset(0)]
            public SDL_MouseButtonEvent button;
            [FieldOffset(0)]
            public SDL_MouseWheelEvent wheel;
            [FieldOffset(0)]
            public SDL_KeyboardEvent key;
            [FieldOffset(0)]
            public SDL_TextInputEvent text;
            //[FieldOffset(63)]
            //public byte lastByte;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            //public ulong[] padding;

            //[FieldOffset(0)]
            //public SDL_DisplayEvent display;
            //[FieldOffset(0)]
            //public SDL_JoyAxisEvent jaxis;
            //[FieldOffset(0)]
            //public SDL_JoyBallEvent jball;
            //[FieldOffset(0)]
            //public SDL_JoyHatEvent jhat;
            //[FieldOffset(0)]
            //public SDL_JoyButtonEvent jbutton;
            //[FieldOffset(0)]
            //public SDL_JoyDeviceEvent jdevice;
            //[FieldOffset(0)]
            //public SDL_ControllerAxisEvent caxis;
            //[FieldOffset(0)]
            //public SDL_ControllerButtonEvent cbutton;
            //[FieldOffset(0)]
            //public SDL_ControllerDeviceEvent cdevice;
            //[FieldOffset(0)]
            //public SDL_ControllerTouchpadEvent ctouchpad;
            //[FieldOffset(0)]
            //public SDL_ControllerSensorEvent csensor;
            //[FieldOffset(0)]
            //public SDL_AudioDeviceEvent adevice;
            //[FieldOffset(0)]
            //public SDL_SensorEvent sensor;
            //[FieldOffset(0)]
            //public SDL_TouchFingerEvent tfinger;
            //[FieldOffset(0)]
            //public SDL_MultiGestureEvent mgesture;
            //[FieldOffset(0)]
            //public SDL_DollarGestureEvent dgesture;
            //[FieldOffset(0)]
            //public SDL_DropEvent drop;
        }

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_PumpEvents();
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_PollEvent(out SDL_Event _event);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_WaitEvent(out SDL_Event _event);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_WaitEventTimeout(out SDL_Event _event, int timeout);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_PushEvent(ref SDL_Event _event);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_FlushEvent(SDL_EventType type);

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_FlushEvents(SDL_EventType min, SDL_EventType max);

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_Delay(uint ms);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern uint SDL_GetTicks();
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong SDL_GetTicks64();
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong SDL_GetPerformanceCounter();
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong SDL_GetPerformanceFrequency();

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_StartTextInput();

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SDL_IsTextInputActive();

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_StopTextInput();

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetTextInputRect(ref Rectangle rect);

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SDL_HasScreenKeyboardSupport();
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SDL_GetKeyboardFocus();
    }
}
