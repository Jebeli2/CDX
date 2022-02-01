namespace CDXSDL
{
    using CDX;
    using CDX.App;
    using CDX.Input;
    using CDX.Logging;
    using CDX.Utilities;
    using System.Drawing;
    using System.Numerics;
    using System.Runtime.InteropServices;
    using System.Text;

    public class SDLApplication : CDXApplication
    {
        private readonly List<SDLDriverInfo> driverInfos = new();
        private readonly List<SDLWindow> windows = new();
        private SDLWindow? currentWindow;
        private IntPtr evtMem;
        private SDLFont? defaultFont;
        private SDLFont? iconFont;
        private IntPtr controller;
        private int whichController = -1;
        private float controllerDeadZone = 8000.0f;
        private float controllerMaxValue = 30000.0f;

        internal SDLFont? DefaultFont => defaultFont;
        internal SDLFont? IconFont => iconFont;

        public int GetDriverIndex(string name)
        {
            for (int i = 0; i < driverInfos.Count; i++)
            {
                if (driverInfos[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }
        public override int GetDisplayCount()
        {
            return SDL_GetNumVideoDisplays();
        }

        public override void Delay(uint ms)
        {
            SDL_Delay(ms);
        }

        protected override void Initialize()
        {
            Logger.Info("Initializing SDL...");
            var dllDir = Path.Combine(Environment.CurrentDirectory, IntPtr.Size == 4 ? "x86" : "x64");
            Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDir);
            SDL_SetMainReady();
            _ = SDL_SetHint(SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING, "1");
            _ = SDL_Init(InitFlags.Everything);
            _ = SDL_SetHint(SDL_HINT_RENDER_DRIVER, "opengl");
            _ = SDL_SetHint(SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
            _ = SDL_SetHint(SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS, "0");
            _ = SDL_SetHint(SDL_HINT_RENDER_BATCHING, "1");
            _ = SDL_SetHint(SDL_HINT_GRAB_KEYBOARD, "1");
            _ = SDL_SetHint(SDL_HINT_ALLOW_ALT_TAB_WHILE_GRABBED, "1");
            _ = SDL_SetHint(SDL_BORDERLESS_WINDOWED_STYLE, "1");
            _ = SDL_SetHint(SDL_BORDERLESS_RESIZABLE_STYLE, "1");
            GetDriverInfos();
            _ = SDLTexture.IMG_Init(SDLTexture.IMG_InitFlags.IMG_INIT_PNG);
            _ = SDLFont.TTF_Init();
            _ = SDLAudio.Mix_Init(SDLAudio.MIX_InitFlags.MIX_INIT_MP3 | SDLAudio.MIX_InitFlags.MIX_INIT_OGG);
            evtMem = Marshal.AllocHGlobal(64);
            defaultFont = SDLFont.LoadFont(Properties.Resources.Roboto_Regular, nameof(Properties.Resources.Roboto_Regular), 15);
            iconFont = SDLFont.LoadFont(Properties.Resources.entypo, nameof(Properties.Resources.entypo), 16);
            audio = new SDLAudio(this);
        }

        protected override void Shutdown()
        {
            if (controller != IntPtr.Zero) { SDL_GameControllerClose(controller); }
            audio.Dispose();
            defaultFont?.Dispose();
            defaultFont = null;
            Logger.Info("Shutdown SDL...");
            SDLAudio.Mix_Quit();
            SDLFont.TTF_Quit();
            SDLTexture.IMG_Quit();
            SDL_Quit();
            Marshal.FreeHGlobal(evtMem);
        }

        private void GetDriverInfos()
        {
            int numDrivers = SDL_GetNumRenderDrivers();
            for (int i = 0; i < numDrivers; i++)
            {
                _ = SDLRenderer.SDL_GetRenderDriverInfo(i, out SDLRenderer.SDL_RendererInfo info);
                SDLDriverInfo driverInfo = new SDLDriverInfo();
                driverInfo.MaxTextureWidth = info.max_texture_width;
                driverInfo.MaxTextureHeight = info.max_texture_height;
                driverInfo.Flags = (uint)info.flags;
                driverInfo.Name = IntPtr2String(info.name) ?? "";
                for (int j = 0; j < info.num_texture_formats; j++)
                {
                    driverInfo.TextureFormats.Add(info.texture_formats[j]);
                    driverInfo.TextureFormatNames.Add(IntPtr2String(SDL_GetPixelFormatName(info.texture_formats[j])) ?? "UNKNOWN");
                }
                driverInfos.Add(driverInfo);

            }
        }

        protected override void Update(FrameTime time)
        {
            foreach (SDLWindow window in windows)
            {
                if (window.HandleCreated)
                {
                    window.RaiseUpdate(time);
                }
            }
            audio?.Update(time);
        }

        protected override void Render(FrameTime time)
        {
            foreach (SDLWindow window in windows)
            {
                if (window.HandleCreated)
                {
                    window.RaisePaint(time);
                }
            }
        }
        protected override void MessageLoop()
        {
            while (SDL_PollEvent(out SDL_Event evt) != 0 && !quitRequested)
            {
                switch (evt.type)
                {
                    case SDL_EventType.QUIT:
                        quitRequested = true;
                        break;
                    case SDL_EventType.WINDOWEVENT:
                        HandleWindowEvent(ref evt.window);
                        break;
                    case SDL_EventType.MOUSEBUTTONDOWN:
                        HandleMouseButtonDownEvent(ref evt.button);
                        break;
                    case SDL_EventType.MOUSEBUTTONUP:
                        HandleMouseButtonUpEvent(ref evt.button);
                        break;
                    case SDL_EventType.MOUSEMOTION:
                        HandleMouseMotionEvent(ref evt.motion);
                        break;
                    case SDL_EventType.MOUSEWHEEL:
                        HandleMouseWheelEvent(ref evt.wheel);
                        break;
                    case SDL_EventType.KEYDOWN:
                        HandleKeyDownEvent(ref evt.key);
                        break;
                    case SDL_EventType.KEYUP:
                        HandleKeyUpEvent(ref evt.key);
                        break;
                    case SDL_EventType.TEXTINPUT:
                        Marshal.StructureToPtr(evt, evtMem, false);
                        HandleTextInput(ref evt.text);
                        break;
                    case SDL_EventType.CONTROLLERDEVICEADDED:
                        AddController(evt.cdevice.which);
                        break;
                    case SDL_EventType.CONTROLLERDEVICEREMOVED:
                        RemController(evt.cdevice.which);
                        break;
                    case SDL_EventType.CONTROLLERBUTTONDOWN:
                        HandleControllerButtonDownEvent(ref evt.cbutton);
                        break;
                    case SDL_EventType.CONTROLLERBUTTONUP:
                        HandleControllerButtonUpEvent(ref evt.cbutton);
                        break;
                    case SDL_EventType.CONTROLLERAXISMOTION:
                        HandleControllerAxisEvent(ref evt.caxis);
                        break;
                    case SDL_EventType.FINGERDOWN:
                        HandleFingerDownEvent(ref evt.tfinger);
                        break;
                    case SDL_EventType.FINGERUP:
                        HandleFingerUpEvent(ref evt.tfinger);
                        break;
                    case SDL_EventType.FINGERMOTION:
                        HandleFingerMotionEvent(ref evt.tfinger);
                        break;
                }
            }
        }

        protected override CDXWindow CreateWindow(Window window)
        {
            SDLWindow sdlWindow = new SDLWindow(this, window);
            windows.Add(sdlWindow);
            return sdlWindow;
        }

        internal void RemoveWindow(SDLWindow sdlWindow)
        {
            windows.Remove(sdlWindow);
            if (currentWindow == sdlWindow) { currentWindow = null; }
        }

        private void AddController(int which)
        {
            RemController(which);
            if (SDL_IsGameController(which))
            {
                controller = SDL_GameControllerOpen(which);
                if (controller != IntPtr.Zero)
                {
                    Logger.Info($"Controller added: {which}");
                    whichController = which;
                }
            }
        }

        private void RemController(int which)
        {
            if (which == whichController && controller != IntPtr.Zero)
            {
                Logger.Info($"Controller added: {which}");
                SDL_GameControllerClose(controller);
                controller = IntPtr.Zero;
                whichController = -1;
            }
        }

        private void HandleWindowEvent(ref SDL_WindowEvent evt)
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
                        window.RaiseDisplayChanged();
                        break;
                    case SDL_WindowEventID.ENTER:
                        window.RaiseEnter();
                        break;
                    case SDL_WindowEventID.EXPOSED:
                        window.RaiseExposed();
                        break;
                    case SDL_WindowEventID.FOCUS_GAINED:
                        window.RaiseFocusGained();
                        currentWindow = window;
                        break;
                    case SDL_WindowEventID.FOCUS_LOST:
                        window.RaiseFocusLost();
                        break;
                    case SDL_WindowEventID.HIDDEN:
                        window.RaiseHidden();
                        break;
                    case SDL_WindowEventID.HIT_TEST:
                        break;
                    case SDL_WindowEventID.ICCPROF_CHANGED:
                        break;
                    case SDL_WindowEventID.LEAVE:
                        window.RaiseLeave();
                        break;
                    case SDL_WindowEventID.MAXIMIZED:
                        window.RaiseMaximized();
                        break;
                    case SDL_WindowEventID.MINIMIZED:
                        window.RaiseMinimized();
                        break;
                    case SDL_WindowEventID.MOVED:
                        window.RaiseMoved();
                        break;
                    case SDL_WindowEventID.RESIZED:
                        window.RaiseResized();
                        break;
                    case SDL_WindowEventID.RESTORED:
                        window.RaiseRestored();
                        break;
                    case SDL_WindowEventID.SHOWN:
                        window.RaiseShown();
                        break;
                    case SDL_WindowEventID.SIZE_CHANGED:
                        window.RaiseSizeChanged();
                        break;
                    case SDL_WindowEventID.TAKE_FOCUS:
                        window.RaiseTakeFocus();
                        currentWindow = window;
                        break;
                }
            }
        }

        private void HandleFingerDownEvent(ref SDL_TouchFingerEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                window.RaiseFingerDown(evt.touchId, evt.fingerId, evt.x, evt.y, evt.dx, evt.dy, evt.pressure);
            }
        }
        private void HandleFingerUpEvent(ref SDL_TouchFingerEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                window.RaiseFingerUp(evt.touchId, evt.fingerId, evt.x, evt.y, evt.dx, evt.dy, evt.pressure);
            }
        }
        private void HandleFingerMotionEvent(ref SDL_TouchFingerEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                window.RaiseFingerMotion(evt.touchId, evt.fingerId, evt.x, evt.y, evt.dx, evt.dy, evt.pressure);
            }
        }
        private void HandleControllerAxisEvent(ref SDL_ControllerAxisEvent evt)
        {
            SDLWindow? window = currentWindow;
            if (window != null)
            {
                Vector2 dir = GetDirection(ref evt);
                window.RaiseControllerAxis(evt.which, (ControllerAxis)evt.axis, evt.axisValue, dir);
            }
        }

        private Vector2 GetDirection(ref SDL_ControllerAxisEvent evt)
        {
            Vector2 dir = new Vector2();
            switch ((SDL_GameControllerAxis)evt.axis)
            {
                case SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX:
                    dir.X = evt.axisValue;
                    dir.Y = -SDL_GameControllerGetAxis(controller, SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY);
                    break;
                case SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTY:
                    dir.Y = -evt.axisValue;
                    dir.X = SDL_GameControllerGetAxis(controller, SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_LEFTX);
                    break;
                case SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX:
                    dir.X = evt.axisValue;
                    dir.Y = -SDL_GameControllerGetAxis(controller, SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY);
                    break;
                case SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTY:
                    dir.Y = -evt.axisValue;
                    dir.X = SDL_GameControllerGetAxis(controller, SDL_GameControllerAxis.SDL_CONTROLLER_AXIS_RIGHTX);
                    break;
            }
            float length = dir.Length();
            if (length < controllerDeadZone)
            {
                dir = Vector2.Zero;
            }
            else
            {
                float f = (length - controllerDeadZone) / (controllerMaxValue - controllerDeadZone);
                MathUtils.Clamp(f, 0.0f, 1.0f);
                dir *= f;
                dir = Vector2.Normalize(dir);
            }
            return dir;
        }

        private void HandleControllerButtonDownEvent(ref SDL_ControllerButtonEvent evt)
        {
            SDLWindow? window = currentWindow;
            if (window != null)
            {
                window.RaiseControllerButtonDown(evt.which, (ControllerButton)evt.button, (KeyButtonState)evt.state);
            }
        }
        private void HandleControllerButtonUpEvent(ref SDL_ControllerButtonEvent evt)
        {
            SDLWindow? window = currentWindow;
            if (window != null)
            {
                window.RaiseControllerButtonUp(evt.which, (ControllerButton)evt.button, (KeyButtonState)evt.state);
            }
        }

        private void HandleMouseButtonDownEvent(ref SDL_MouseButtonEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                window.RaiseMouseButtonDown(evt.which, evt.x, evt.y, (MouseButton)evt.button, (KeyButtonState)evt.state, evt.clicks);
            }
        }
        private void HandleMouseButtonUpEvent(ref SDL_MouseButtonEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                window.RaiseMouseButtonUp(evt.which, evt.x, evt.y, (MouseButton)evt.button, (KeyButtonState)evt.state, evt.clicks);
            }
        }
        private void HandleMouseMotionEvent(ref SDL_MouseMotionEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                window.RaiseMouseMove(evt.which, evt.x, evt.y, evt.xrel, evt.yrel);
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

        private void HandleKeyDownEvent(ref SDL_KeyboardEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                window.RaiseKeyDown(evt.keysym.scancode, evt.keysym.sym, evt.keysym.mod, (KeyButtonState)evt.state, evt.repeat != 0);
            }
        }
        private void HandleKeyUpEvent(ref SDL_KeyboardEvent evt)
        {
            SDLWindow? window = GetWindowFromID(evt.windowID);
            if (window != null)
            {
                window.RaiseKeyUp(evt.keysym.scancode, evt.keysym.sym, evt.keysym.mod, (KeyButtonState)evt.state, evt.repeat != 0);
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

        internal static string? IntPtr2String(IntPtr ptr, bool freePtr = false)
        {
            if (ptr == IntPtr.Zero) return null;
            string? result = Marshal.PtrToStringUTF8(ptr);
            if (freePtr) { SDL_free(ptr); }
            return result;
        }


        private const string LibName = "SDL2";

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SDL_malloc(IntPtr size);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SDL_free(IntPtr memblock);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SDL_memcpy(IntPtr dst, IntPtr src, IntPtr len);

        [Flags]
        private enum InitFlags : uint
        {
            Timer = 0x1,
            Audio = 0x10,
            Video = 0x20,
            Joystick = 0x200,
            Haptic = 0x1000,
            GameController = 0x2000,
            Events = 0x4000,
            Sensor = 0x8000,
            Everything = Timer | Audio | Video | Haptic | GameController | Events | Sensor
        };

        public const string SDL_HINT_FRAMEBUFFER_ACCELERATION = "SDL_FRAMEBUFFER_ACCELERATION";
        public const string SDL_HINT_RENDER_DRIVER = "SDL_RENDER_DRIVER";
        public const string SDL_HINT_RENDER_OPENGL_SHADERS = "SDL_RENDER_OPENGL_SHADERS";
        public const string SDL_HINT_RENDER_DIRECT3D_THREADSAFE = "SDL_RENDER_DIRECT3D_THREADSAFE";
        public const string SDL_HINT_RENDER_VSYNC = "SDL_RENDER_VSYNC";
        public const string SDL_HINT_VIDEO_X11_XVIDMODE = "SDL_VIDEO_X11_XVIDMODE";
        public const string SDL_HINT_VIDEO_X11_XINERAMA = "SDL_VIDEO_X11_XINERAMA";
        public const string SDL_HINT_VIDEO_X11_XRANDR = "SDL_VIDEO_X11_XRANDR";
        public const string SDL_HINT_GRAB_KEYBOARD = "SDL_GRAB_KEYBOARD";
        public const string SDL_HINT_VIDEO_MINIMIZE_ON_FOCUS_LOSS = "SDL_VIDEO_MINIMIZE_ON_FOCUS_LOSS";
        public const string SDL_HINT_IDLE_TIMER_DISABLED = "SDL_IOS_IDLE_TIMER_DISABLED";
        public const string SDL_HINT_ORIENTATIONS = "SDL_IOS_ORIENTATIONS";
        public const string SDL_HINT_XINPUT_ENABLED = "SDL_XINPUT_ENABLED";
        public const string SDL_HINT_GAMECONTROLLERCONFIG = "SDL_GAMECONTROLLERCONFIG";
        public const string SDL_HINT_JOYSTICK_ALLOW_BACKGROUND_EVENTS = "SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS";
        public const string SDL_HINT_ALLOW_TOPMOST = "SDL_ALLOW_TOPMOST";
        public const string SDL_HINT_TIMER_RESOLUTION = "SDL_TIMER_RESOLUTION";
        public const string SDL_HINT_RENDER_SCALE_QUALITY = "SDL_RENDER_SCALE_QUALITY";
        public const string SDL_HINT_VIDEO_HIGHDPI_DISABLED = "SDL_VIDEO_HIGHDPI_DISABLED";
        public const string SDL_HINT_CTRL_CLICK_EMULATE_RIGHT_CLICK = "SDL_CTRL_CLICK_EMULATE_RIGHT_CLICK";
        public const string SDL_HINT_VIDEO_WIN_D3DCOMPILER = "SDL_VIDEO_WIN_D3DCOMPILER";
        public const string SDL_HINT_MOUSE_RELATIVE_MODE_WARP = "SDL_MOUSE_RELATIVE_MODE_WARP";
        public const string SDL_HINT_VIDEO_WINDOW_SHARE_PIXEL_FORMAT = "SDL_VIDEO_WINDOW_SHARE_PIXEL_FORMAT";
        public const string SDL_HINT_VIDEO_ALLOW_SCREENSAVER = "SDL_VIDEO_ALLOW_SCREENSAVER";
        public const string SDL_HINT_ACCELEROMETER_AS_JOYSTICK = "SDL_ACCELEROMETER_AS_JOYSTICK";
        public const string SDL_HINT_VIDEO_MAC_FULLSCREEN_SPACES = "SDL_VIDEO_MAC_FULLSCREEN_SPACES";
        public const string SDL_HINT_WINRT_PRIVACY_POLICY_URL = "SDL_WINRT_PRIVACY_POLICY_URL";
        public const string SDL_HINT_WINRT_PRIVACY_POLICY_LABEL = "SDL_WINRT_PRIVACY_POLICY_LABEL";
        public const string SDL_HINT_WINRT_HANDLE_BACK_BUTTON = "SDL_WINRT_HANDLE_BACK_BUTTON";
        public const string SDL_HINT_NO_SIGNAL_HANDLERS = "SDL_NO_SIGNAL_HANDLERS";
        public const string SDL_HINT_IME_INTERNAL_EDITING = "SDL_IME_INTERNAL_EDITING";
        public const string SDL_HINT_ANDROID_SEPARATE_MOUSE_AND_TOUCH = "SDL_ANDROID_SEPARATE_MOUSE_AND_TOUCH";
        public const string SDL_HINT_EMSCRIPTEN_KEYBOARD_ELEMENT = "SDL_EMSCRIPTEN_KEYBOARD_ELEMENT";
        public const string SDL_HINT_THREAD_STACK_SIZE = "SDL_THREAD_STACK_SIZE";
        public const string SDL_HINT_WINDOW_FRAME_USABLE_WHILE_CURSOR_HIDDEN = "SDL_WINDOW_FRAME_USABLE_WHILE_CURSOR_HIDDEN";
        public const string SDL_HINT_WINDOWS_ENABLE_MESSAGELOOP = "SDL_WINDOWS_ENABLE_MESSAGELOOP";
        public const string SDL_HINT_WINDOWS_NO_CLOSE_ON_ALT_F4 = "SDL_WINDOWS_NO_CLOSE_ON_ALT_F4";
        public const string SDL_HINT_XINPUT_USE_OLD_JOYSTICK_MAPPING = "SDL_XINPUT_USE_OLD_JOYSTICK_MAPPING";
        public const string SDL_HINT_MAC_BACKGROUND_APP = "SDL_MAC_BACKGROUND_APP";
        public const string SDL_HINT_VIDEO_X11_NET_WM_PING = "SDL_VIDEO_X11_NET_WM_PING";
        public const string SDL_HINT_ANDROID_APK_EXPANSION_MAIN_FILE_VERSION = "SDL_ANDROID_APK_EXPANSION_MAIN_FILE_VERSION";
        public const string SDL_HINT_ANDROID_APK_EXPANSION_PATCH_FILE_VERSION = "SDL_ANDROID_APK_EXPANSION_PATCH_FILE_VERSION";
        public const string SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH = "SDL_MOUSE_FOCUS_CLICKTHROUGH";
        public const string SDL_HINT_BMP_SAVE_LEGACY_FORMAT = "SDL_BMP_SAVE_LEGACY_FORMAT";
        public const string SDL_HINT_WINDOWS_DISABLE_THREAD_NAMING = "SDL_WINDOWS_DISABLE_THREAD_NAMING";
        public const string SDL_HINT_APPLE_TV_REMOTE_ALLOW_ROTATION = "SDL_APPLE_TV_REMOTE_ALLOW_ROTATION";
        public const string SDL_HINT_AUDIO_RESAMPLING_MODE = "SDL_AUDIO_RESAMPLING_MODE";
        public const string SDL_HINT_RENDER_LOGICAL_SIZE_MODE = "SDL_RENDER_LOGICAL_SIZE_MODE";
        public const string SDL_HINT_MOUSE_NORMAL_SPEED_SCALE = "SDL_MOUSE_NORMAL_SPEED_SCALE";
        public const string SDL_HINT_MOUSE_RELATIVE_SPEED_SCALE = "SDL_MOUSE_RELATIVE_SPEED_SCALE";
        public const string SDL_HINT_TOUCH_MOUSE_EVENTS = "SDL_TOUCH_MOUSE_EVENTS";
        public const string SDL_HINT_WINDOWS_INTRESOURCE_ICON = "SDL_WINDOWS_INTRESOURCE_ICON";
        public const string SDL_HINT_WINDOWS_INTRESOURCE_ICON_SMALL = "SDL_WINDOWS_INTRESOURCE_ICON_SMALL";
        public const string SDL_HINT_IOS_HIDE_HOME_INDICATOR = "SDL_IOS_HIDE_HOME_INDICATOR";
        public const string SDL_HINT_TV_REMOTE_AS_JOYSTICK = "SDL_TV_REMOTE_AS_JOYSTICK";
        public const string SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR = "SDL_VIDEO_X11_NET_WM_BYPASS_COMPOSITOR";
        public const string SDL_HINT_MOUSE_DOUBLE_CLICK_TIME = "SDL_MOUSE_DOUBLE_CLICK_TIME";
        public const string SDL_HINT_MOUSE_DOUBLE_CLICK_RADIUS = "SDL_MOUSE_DOUBLE_CLICK_RADIUS";
        public const string SDL_HINT_JOYSTICK_HIDAPI = "SDL_JOYSTICK_HIDAPI";
        public const string SDL_HINT_JOYSTICK_HIDAPI_PS4 = "SDL_JOYSTICK_HIDAPI_PS4";
        public const string SDL_HINT_JOYSTICK_HIDAPI_PS4_RUMBLE = "SDL_JOYSTICK_HIDAPI_PS4_RUMBLE";
        public const string SDL_HINT_JOYSTICK_HIDAPI_STEAM = "SDL_JOYSTICK_HIDAPI_STEAM";
        public const string SDL_HINT_JOYSTICK_HIDAPI_SWITCH = "SDL_JOYSTICK_HIDAPI_SWITCH";
        public const string SDL_HINT_JOYSTICK_HIDAPI_XBOX = "SDL_JOYSTICK_HIDAPI_XBOX";
        public const string SDL_HINT_ENABLE_STEAM_CONTROLLERS = "SDL_ENABLE_STEAM_CONTROLLERS";
        public const string SDL_HINT_ANDROID_TRAP_BACK_BUTTON = "SDL_ANDROID_TRAP_BACK_BUTTON";
        public const string SDL_HINT_MOUSE_TOUCH_EVENTS = "SDL_MOUSE_TOUCH_EVENTS";
        public const string SDL_HINT_GAMECONTROLLERCONFIG_FILE = "SDL_GAMECONTROLLERCONFIG_FILE";
        public const string SDL_HINT_ANDROID_BLOCK_ON_PAUSE = "SDL_ANDROID_BLOCK_ON_PAUSE";
        public const string SDL_HINT_RENDER_BATCHING = "SDL_RENDER_BATCHING";
        public const string SDL_HINT_EVENT_LOGGING = "SDL_EVENT_LOGGING";
        public const string SDL_HINT_WAVE_RIFF_CHUNK_SIZE = "SDL_WAVE_RIFF_CHUNK_SIZE";
        public const string SDL_HINT_WAVE_TRUNCATION = "SDL_WAVE_TRUNCATION";
        public const string SDL_HINT_WAVE_FACT_CHUNK = "SDL_WAVE_FACT_CHUNK";
        public const string SDL_HINT_VIDO_X11_WINDOW_VISUALID = "SDL_VIDEO_X11_WINDOW_VISUALID";
        public const string SDL_HINT_GAMECONTROLLER_USE_BUTTON_LABELS = "SDL_GAMECONTROLLER_USE_BUTTON_LABELS";
        public const string SDL_HINT_VIDEO_EXTERNAL_CONTEXT = "SDL_VIDEO_EXTERNAL_CONTEXT";
        public const string SDL_HINT_JOYSTICK_HIDAPI_GAMECUBE = "SDL_JOYSTICK_HIDAPI_GAMECUBE";
        public const string SDL_HINT_DISPLAY_USABLE_BOUNDS = "SDL_DISPLAY_USABLE_BOUNDS";
        public const string SDL_HINT_VIDEO_X11_FORCE_EGL = "SDL_VIDEO_X11_FORCE_EGL";
        public const string SDL_HINT_GAMECONTROLLERTYPE = "SDL_GAMECONTROLLERTYPE";
        public const string SDL_HINT_JOYSTICK_RAWINPUT = "SDL_JOYSTICK_RAWINPUT";
        public const string SDL_HINT_AUDIO_DEVICE_APP_NAME = "SDL_AUDIO_DEVICE_APP_NAME";
        public const string SDL_HINT_AUDIO_DEVICE_STREAM_NAME = "SDL_AUDIO_DEVICE_STREAM_NAME";
        public const string SDL_HINT_PREFERRED_LOCALES = "SDL_PREFERRED_LOCALES";
        public const string SDL_HINT_THREAD_PRIORITY_POLICY = "SDL_THREAD_PRIORITY_POLICY";
        public const string SDL_HINT_EMSCRIPTEN_ASYNCIFY = "SDL_EMSCRIPTEN_ASYNCIFY";
        public const string SDL_HINT_LINUX_JOYSTICK_DEADZONES = "SDL_LINUX_JOYSTICK_DEADZONES";
        public const string SDL_HINT_ANDROID_BLOCK_ON_PAUSE_PAUSEAUDIO = "SDL_ANDROID_BLOCK_ON_PAUSE_PAUSEAUDIO";
        public const string SDL_HINT_JOYSTICK_HIDAPI_PS5 = "SDL_JOYSTICK_HIDAPI_PS5";
        public const string SDL_HINT_THREAD_FORCE_REALTIME_TIME_CRITICAL = "SDL_THREAD_FORCE_REALTIME_TIME_CRITICAL";
        public const string SDL_HINT_JOYSTICK_THREAD = "SDL_JOYSTICK_THREAD";
        public const string SDL_HINT_AUTO_UPDATE_JOYSTICKS = "SDL_AUTO_UPDATE_JOYSTICKS";
        public const string SDL_HINT_AUTO_UPDATE_SENSORS = "SDL_AUTO_UPDATE_SENSORS";
        public const string SDL_HINT_MOUSE_RELATIVE_SCALING = "SDL_MOUSE_RELATIVE_SCALING";
        public const string SDL_HINT_JOYSTICK_HIDAPI_PS5_RUMBLE = "SDL_JOYSTICK_HIDAPI_PS5_RUMBLE";
        public const string SDL_HINT_WINDOWS_FORCE_MUTEX_CRITICAL_SECTIONS = "SDL_WINDOWS_FORCE_MUTEX_CRITICAL_SECTIONS";
        public const string SDL_HINT_WINDOWS_FORCE_SEMAPHORE_KERNEL = "SDL_WINDOWS_FORCE_SEMAPHORE_KERNEL";
        public const string SDL_HINT_JOYSTICK_HIDAPI_PS5_PLAYER_LED = "SDL_JOYSTICK_HIDAPI_PS5_PLAYER_LED";
        public const string SDL_HINT_WINDOWS_USE_D3D9EX = "SDL_WINDOWS_USE_D3D9EX";
        public const string SDL_HINT_JOYSTICK_HIDAPI_JOY_CONS = "SDL_JOYSTICK_HIDAPI_JOY_CONS";
        public const string SDL_HINT_JOYSTICK_HIDAPI_STADIA = "SDL_JOYSTICK_HIDAPI_STADIA";
        public const string SDL_HINT_JOYSTICK_HIDAPI_SWITCH_HOME_LED = "SDL_JOYSTICK_HIDAPI_SWITCH_HOME_LED";
        public const string SDL_HINT_ALLOW_ALT_TAB_WHILE_GRABBED = "SDL_ALLOW_ALT_TAB_WHILE_GRABBED";
        public const string SDL_HINT_KMSDRM_REQUIRE_DRM_MASTER = "SDL_KMSDRM_REQUIRE_DRM_MASTER";
        public const string SDL_HINT_AUDIO_DEVICE_STREAM_ROLE = "SDL_AUDIO_DEVICE_STREAM_ROLE";
        public const string SDL_HINT_X11_FORCE_OVERRIDE_REDIRECT = "SDL_X11_FORCE_OVERRIDE_REDIRECT";
        public const string SDL_HINT_JOYSTICK_HIDAPI_LUNA = "SDL_JOYSTICK_HIDAPI_LUNA";
        public const string SDL_HINT_JOYSTICK_RAWINPUT_CORRELATE_XINPUT = "SDL_JOYSTICK_RAWINPUT_CORRELATE_XINPUT";
        public const string SDL_HINT_AUDIO_INCLUDE_MONITORS = "SDL_AUDIO_INCLUDE_MONITORS";
        public const string SDL_HINT_VIDEO_WAYLAND_ALLOW_LIBDECOR = "SDL_VIDEO_WAYLAND_ALLOW_LIBDECOR";
        public const string SDL_HINT_VIDEO_EGL_ALLOW_TRANSPARENCY = "SDL_VIDEO_EGL_ALLOW_TRANSPARENCY";
        public const string SDL_HINT_APP_NAME = "SDL_APP_NAME";
        public const string SDL_HINT_SCREENSAVER_INHIBIT_ACTIVITY_NAME = "SDL_SCREENSAVER_INHIBIT_ACTIVITY_NAME";
        public const string SDL_HINT_IME_SHOW_UI = "SDL_IME_SHOW_UI";
        public const string SDL_HINT_WINDOW_NO_ACTIVATION_WHEN_SHOWN = "SDL_WINDOW_NO_ACTIVATION_WHEN_SHOWN";
        public const string SDL_HINT_POLL_SENTINEL = "SDL_POLL_SENTINEL";
        public const string SDL_HINT_JOYSTICK_DEVICE = "SDL_JOYSTICK_DEVICE";
        public const string SDL_HINT_LINUX_JOYSTICK_CLASSIC = "SDL_LINUX_JOYSTICK_CLASSIC";
        public const string SDL_HINT_RENDER_LINE_METHOD = "SDL_RENDER_LINE_METHOD";
        public const string SDL_BORDERLESS_WINDOWED_STYLE = "SDL_BORDERLESS_WINDOWED_STYLE";
        public const string SDL_BORDERLESS_RESIZABLE_STYLE = "SDL_BORDERLESS_RESIZABLE_STYLE";

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
        private struct SDL_QuitEvent
        {
            public SDL_EventType type;
            public uint timestamp;
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
        private struct SDL_Keysym
        {
            public ScanCode scancode;
            public KeyCode sym;
            public KeyMod mod;
            public uint unicode;
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

        //private const int SDL_TEXTEDITINGEVENT_TEXT_SIZE = 32;
        private const int SDL_TEXTINPUTEVENT_TEXT_SIZE = 32;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct SDL_TextInputEvent
        {
            public SDL_EventType type;
            public uint timestamp;
            public uint windowID;
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = SDL_TEXTINPUTEVENT_TEXT_SIZE)]
            //public byte[] text;
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
            private uint padding4;
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
            public SDL_EventType type;
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
            public SDL_EventType type;
            public uint timestamp;
            public int which;
            public int sensor;
            public float data1;
            public float data2;
            public float data3;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SDL_TouchFingerEvent
        {
            public SDL_EventType type;
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
            public SDL_EventType type;
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
            public SDL_EventType type;
            public uint timestamp;
            public long touchId;
            public long gestureId;
            public uint numFingers;
            public float error;
            public float x;
            public float y;
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
            [FieldOffset(0)]
            public SDL_ControllerAxisEvent caxis;
            [FieldOffset(0)]
            public SDL_ControllerButtonEvent cbutton;
            [FieldOffset(0)]
            public SDL_ControllerDeviceEvent cdevice;
            [FieldOffset(0)]
            public SDL_ControllerTouchpadEvent ctouchpad;
            [FieldOffset(0)]
            public SDL_ControllerSensorEvent csensor;
            [FieldOffset(0)]
            public SDL_TouchFingerEvent tfinger;
            [FieldOffset(0)]
            public SDL_MultiGestureEvent mgesture;
            [FieldOffset(0)]
            public SDL_DollarGestureEvent dgesture;
        }

        private enum SDL_GameControllerAxis
        {
            SDL_CONTROLLER_AXIS_INVALID = -1,
            SDL_CONTROLLER_AXIS_LEFTX,
            SDL_CONTROLLER_AXIS_LEFTY,
            SDL_CONTROLLER_AXIS_RIGHTX,
            SDL_CONTROLLER_AXIS_RIGHTY,
            SDL_CONTROLLER_AXIS_TRIGGERLEFT,
            SDL_CONTROLLER_AXIS_TRIGGERRIGHT,
            SDL_CONTROLLER_AXIS_MAX
        }

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_SetMainReady();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_Init(InitFlags flags);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_Quit();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SDL_SetHint([In()][MarshalAs(UnmanagedType.LPUTF8Str)] string name, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string value);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern ulong SDL_GetTicks64();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_Delay(uint ms);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_PollEvent(out SDL_Event _event);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SDL_GetNumVideoDisplays();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SDL_RWFromMem(IntPtr mem, int size);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SDL_RWFromMem([In()][MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] mem, int size);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SDL_RWFromFile([In()][MarshalAs(UnmanagedType.LPUTF8Str)] string file, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string mode);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetNumRenderDrivers();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetPixelFormatName(uint format);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SDL_GameControllerClose(IntPtr gamecontroller);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GameControllerOpen(int joystick_index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SDL_IsGameController(int joystick_index);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern short SDL_GameControllerGetAxis(IntPtr gamecontroller, SDL_GameControllerAxis axis);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetError();
        internal static string? GetError()
        {
            return IntPtr2String(SDL_GetError());
        }
    }
}