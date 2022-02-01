﻿namespace CDX
{
    using CDX.App;
    using CDX.Audio;
    using CDX.Graphics;
    using CDX.GUI;
    using CDX.Input;
    using CDX.Logging;
    using CDX.Screens;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Window
    {
        private CDXWindow cdxWindow;
        private string title;
        private bool visible = true;
        private bool resizable = true;
        private bool alwaysOnTop;
        private bool borderless;
        private bool fullScreen;
        private int display;
        private string driver = "";
        private bool vSync = false;
        private int x = -1;
        private int y = -1;
        private int width = 640;
        private int height = 480;
        private bool showFPS;
        private float fpsPosX;
        private float fpsPosY;
        private readonly HotKeyManager hotKeyManager;
        protected FrameTime frameTime = new();
        protected IScreen screen;
        protected IGUISystem gui;
        private int axisThreshold = 8000;

        internal Window(CDXWindow cdx)
        {
            this.title = "";
            screen = new NoScreen();
            hotKeyManager = new();
            cdxWindow = cdx;
            gui = new NoGUI();
        }
        public Window(string title)
        {
            this.title = title;
            screen = new NoScreen();
            hotKeyManager = new();
            cdxWindow = new NoWindow(this);
            gui = new NoGUI();
        }

        public IGraphics Graphics => cdxWindow.Graphics;
        public IAudio Audio => cdxWindow.Audio;

        public IGUISystem GUI
        {
            get => gui;
            set
            {
                gui = value;
                //gui.SetCDXWindow(cdxWindow);
                gui.ScreenResized(width, height);
            }
        }
        public HotKeyManager HotKeyManager => hotKeyManager;

        public IScreen Screen
        {
            get => screen;
            set
            {
                if (screen != value)
                {
                    screen.Hide();
                    screen = value;
                    screen.Show();
                }
            }
        }
        public uint WindowID => cdxWindow?.WindowID ?? 0;
        public string Title
        {
            get => title;
            set
            {
                if (value != title)
                {
                    title = value;
                    cdxWindow.SetTitle(title);
                }
            }
        }

        public bool Visible
        {
            get => visible;
            set
            {
                if (value != visible)
                {
                    visible = value;
                    cdxWindow.SetVisible(visible);
                }
            }
        }

        public bool Resizable
        {
            get => resizable;
            set
            {
                if (resizable != value)
                {
                    resizable = value;
                    cdxWindow.SetResizable(resizable);
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
                    cdxWindow.SetAlwaysOnTop(alwaysOnTop);
                }
            }
        }

        public bool Borderless
        {
            get => borderless;
            set
            {
                if (borderless != value)
                {
                    borderless = value;
                    cdxWindow.SetBorderless(borderless);
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
                    cdxWindow.SetFullScreen(fullScreen);
                }
            }
        }

        public bool ShowFPS
        {
            get => showFPS;
            set
            {
                if (showFPS != value)
                {
                    showFPS = value;
                    cdxWindow.ShowFPS = value;
                }
            }
        }

        public float FPSPosX
        {
            get => fpsPosX;
            set
            {
                if (fpsPosX != value)
                {
                    fpsPosX = value;
                    cdxWindow.FPSPosX = value;
                }
            }
        }
        public float FPSPosY
        {
            get => fpsPosY;
            set
            {
                if (fpsPosY != value)
                {
                    fpsPosY = value;
                    cdxWindow.FPSPosY = value;
                }
            }
        }

        public int Display
        {
            get => display;
            set
            {
                if (display != value)
                {
                    display = value;
                }
            }
        }

        public string Driver
        {
            get => driver;
            set
            {
                if (driver != value)
                {
                    driver = value;
                }
            }
        }

        public bool VSync
        {
            get => vSync;
            set
            {
                if (vSync != value)
                {
                    vSync = value;
                }
            }
        }

        public int X
        {
            get => x;
            set
            {
                if (x != value)
                {
                    x = value;
                    cdxWindow.SetPosition(x, y);
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
                    y = value;
                    cdxWindow.SetPosition(x, y);
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
                    width = value;
                    cdxWindow.SetSize(width, height);
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
                    height = value;
                    cdxWindow.SetSize(width, height);
                }
            }
        }

        public void Show()
        {
            visible = true;
            cdxWindow.Show();
        }

        public void Hide()
        {
            visible = false;
            cdxWindow.Hide();
        }

        public void Close()
        {
            UnlinkWindow(cdxWindow);
            cdxWindow.Close();
        }
        public void ToggleFullScreen()
        {
            FullScreen = !fullScreen;
        }

        public IImage? LoadImage(string fileName)
        {
            return cdxWindow.Graphics.LoadImage(fileName);
        }

        public IImage? LoadImage(byte[] data, string name)
        {
            return cdxWindow.Graphics.LoadImage(data, name);
        }

        public ITextFont? LoadFont(string fileName, int ptSize)
        {
            return cdxWindow.Graphics.LoadFont(fileName, ptSize);
        }

        public ITextFont? LoadFont(byte[] data, string name, int ptSize)
        {
            return cdxWindow.Graphics.LoadFont(data, name, ptSize);
        }

        public IMusic? LoadMusic(string fileName)
        {
            return cdxWindow.Audio.LoadMusic(fileName);
        }
        public IMusic? LoadMusic(byte[] data, string name)
        {
            return cdxWindow.Audio.LoadMusic(data, name);
        }

        public void InstallDefaultHotKeys()
        {
            hotKeyManager.AddHotKey(ScanCode.SCANCODE_RETURN, KeyMod.LALT, ToggleFullScreen);
            hotKeyManager.AddHotKey(ScanCode.SCANCODE_RETURN, KeyMod.RALT | KeyMod.LCTRL, ToggleFullScreen);
            hotKeyManager.AddHotKey(ScanCode.SCANCODE_F12, KeyMod.NONE, ToggleFullScreen);
        }
        protected virtual void OnPaint(PaintEventArgs e)
        {

        }

        protected virtual void OnUpdate(UpdateEventArgs e)
        {

        }

        protected virtual void OnLoad(EventArgs e)
        {

        }
        protected virtual void OnShown(EventArgs e)
        {

        }
        protected virtual void OnHidden(EventArgs e)
        {

        }

        protected virtual void OnClose(EventArgs e)
        {

        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {

        }

        protected virtual void OnKeyUp(KeyEventArgs e)
        {
        }

        protected virtual void OnTextInput(TextInputEventArgs e)
        {

        }
        protected virtual void OnControllerButtonDown(ControllerButtonEventArgs e)
        {

        }

        protected virtual void OnControllerButtonUp(ControllerButtonEventArgs e)
        {

        }

        protected virtual void OnMouseButtonDown(MouseButtonEventArgs e)
        {

        }

        protected virtual void OnMouseButtonUp(MouseButtonEventArgs e)
        {

        }

        protected virtual void OnControllerAxis(ControllerAxisEventArgs e)
        {

        }

        protected virtual void OnMouseMoved(MouseMotionEventArgs e)
        {

        }

        protected virtual void OnMouseWheel(MouseWheelEventArgs e)
        {

        }

        private string GetEventLogMsg(EventArgs e, string? name = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Window ");
            sb.Append(WindowID);
            sb.Append(" (");
            sb.Append(title);
            sb.Append("): ");
            if (name != null)
            {
                sb.Append(name);
            }
            if (e != EventArgs.Empty)
            {
                sb.Append(" ");
                sb.Append(e);
            }
            return sb.ToString();
        }

        internal void LinkWindow(CDXWindow cdx)
        {
            cdxWindow = cdx;
            cdxWindow.ShowFPS = showFPS;
            cdxWindow.FPSPosX = fpsPosX;
            cdxWindow.FPSPosY = fpsPosY;
            cdxWindow.WindowPaint += CdxWindow_WindowPaint;
            cdxWindow.WindowUpdate += CdxWindow_WindowUpdate;
            cdxWindow.WindowLoad += CdxWindow_WindowLoad;
            cdxWindow.WindowClose += CdxWindow_WindowClose;
            cdxWindow.WindowDisplayChanged += CdxWindow_WindowDisplayChanged;
            cdxWindow.WindowEnter += CdxWindow_WindowEnter;
            cdxWindow.WindowFocusLost += CdxWindow_WindowFocusLost;
            cdxWindow.WindowRestored += CdxWindow_WindowRestored;
            cdxWindow.WindowTakeFocus += CdxWindow_WindowTakeFocus;
            cdxWindow.WindowLeave += CdxWindow_WindowLeave;
            cdxWindow.WindowSizeChanged += CdxWindow_WindowSizeChanged;
            cdxWindow.WindowExposed += CdxWindow_WindowExposed;
            cdxWindow.WindowMoved += CdxWindow_WindowMoved;
            cdxWindow.WindowFocusGained += CdxWindow_WindowFocusGained;
            cdxWindow.WindowHidden += CdxWindow_WindowHidden;
            cdxWindow.WindowMaxmimized += CdxWindow_WindowMaxmimized;
            cdxWindow.WindowMinimized += CdxWindow_WindowMinimized;
            cdxWindow.WindowResized += CdxWindow_WindowResized;
            cdxWindow.WindowShown += CdxWindow_WindowShown;
            cdxWindow.MouseButtonDown += CdxWindow_MouseButtonDown;
            cdxWindow.MouseButtonUp += CdxWindow_MouseButtonUp;
            cdxWindow.MouseMove += CdxWindow_MouseMove;
            cdxWindow.MouseWheel += CdxWindow_MouseWheel;
            cdxWindow.KeyDown += CdxWindow_KeyDown;
            cdxWindow.KeyUp += CdxWindow_KeyUp;
            cdxWindow.TextInput += CdxWindow_TextInput;
            cdxWindow.ControllerButtonDown += CdxWindow_ControllerButtonDown;
            cdxWindow.ControllerButtonUp += CdxWindow_ControllerButtonUp;
            cdxWindow.ControllerAxis += CdxWindow_ControllerAxis;
            //gui?.SetCDXWindow(cdxWindow);
        }


        private void UnlinkWindow(CDXWindow cdx)
        {
            cdx.WindowPaint -= CdxWindow_WindowPaint;
            cdx.WindowUpdate -= CdxWindow_WindowUpdate;
            cdx.WindowLoad -= CdxWindow_WindowLoad;
            cdx.WindowClose -= CdxWindow_WindowClose;
            cdx.WindowDisplayChanged -= CdxWindow_WindowDisplayChanged;
            cdx.WindowEnter -= CdxWindow_WindowEnter;
            cdx.WindowFocusLost -= CdxWindow_WindowFocusLost;
            cdx.WindowRestored -= CdxWindow_WindowRestored;
            cdx.WindowTakeFocus -= CdxWindow_WindowTakeFocus;
            cdx.WindowLeave -= CdxWindow_WindowLeave;
            cdx.WindowSizeChanged -= CdxWindow_WindowSizeChanged;
            cdx.WindowExposed -= CdxWindow_WindowExposed;
            cdx.WindowMoved -= CdxWindow_WindowMoved;
            cdx.WindowFocusGained -= CdxWindow_WindowFocusGained;
            cdx.WindowHidden -= CdxWindow_WindowHidden;
            cdx.WindowMaxmimized -= CdxWindow_WindowMaxmimized;
            cdx.WindowMinimized -= CdxWindow_WindowMinimized;
            cdx.WindowResized -= CdxWindow_WindowResized;
            cdx.WindowShown -= CdxWindow_WindowShown;
            cdx.MouseButtonDown -= CdxWindow_MouseButtonDown;
            cdx.MouseButtonUp -= CdxWindow_MouseButtonUp;
            cdx.MouseMove -= CdxWindow_MouseMove;
            cdx.MouseWheel -= CdxWindow_MouseWheel;
            cdx.KeyDown -= CdxWindow_KeyDown;
            cdx.KeyUp -= CdxWindow_KeyUp;
            cdx.TextInput -= CdxWindow_TextInput;
            cdx.ControllerButtonDown -= CdxWindow_ControllerButtonDown;
            cdx.ControllerButtonUp -= CdxWindow_ControllerButtonUp;
            cdxWindow.ControllerAxis -= CdxWindow_ControllerAxis;
        }

        private void CdxWindow_ControllerAxis(object sender, ControllerAxisEventArgs e)
        {
            if (e.Axis == ControllerAxis.TRIGGERLEFT || e.Axis == ControllerAxis.TRIGGERRIGHT || e.AxisValue < -axisThreshold || e.AxisValue > axisThreshold)
            {
                Logger.Verbose(GetEventLogMsg(e, "ControllerAxis"));
            }
            OnControllerAxis(e);
        }

        private void CdxWindow_ControllerButtonUp(object sender, ControllerButtonEventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "ControllerButtonUp"));
            OnControllerButtonUp(e);
        }

        private void CdxWindow_ControllerButtonDown(object sender, ControllerButtonEventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "ControllerButtonDown"));
            OnControllerButtonDown(e);
        }

        private void CdxWindow_TextInput(object sender, TextInputEventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "TextInput"));
            OnTextInput(e);
            screen.OnTextInput(e);
            gui.OnTextInput(e);
        }

        private void CdxWindow_KeyUp(object sender, KeyEventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "KeyUp"));
            if (HotKeyManager.ExecuteHotKeyAction(e))
            {
                e.HandledByHotKeyManager = true;
            }
            OnKeyUp(e);
            screen.OnKeyUp(e);
            gui.OnKeyUp(e);
        }

        private void CdxWindow_KeyDown(object sender, KeyEventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "KeyDown"));
            OnKeyDown(e);
            screen.OnKeyDown(e);
            gui.OnKeyDown(e);
        }

        private void CdxWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "MouseWheel"));
            OnMouseWheel(e);
            screen.OnMouseWheel(e);
            gui.OnMouseWheel(e);
        }

        private void CdxWindow_MouseMove(object sender, MouseMotionEventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "MouseMotion"));
            OnMouseMoved(e);
            screen.OnMouseMoved(e);
            gui.OnMouseMoved(e);
        }

        private void CdxWindow_MouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "MouseButtonUp"));
            OnMouseButtonUp(e);
            screen.OnMouseButtonUp(e);
            gui.OnMouseButtonUp(e);
        }

        private void CdxWindow_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "MouseButtonDown"));
            OnMouseButtonDown(e);
            screen.OnMouseButtonDown(e);
            gui.OnMouseButtonDown(e);
        }


        private void CdxWindow_WindowPaint(object sender, PaintEventArgs e)
        {
            frameTime = e.FrameTime;
            OnPaint(e);
            screen.Render(frameTime);
            gui.Render(e.Graphics, frameTime);
        }
        private void CdxWindow_WindowUpdate(object sender, UpdateEventArgs e)
        {
            frameTime = e.FrameTime;
            OnUpdate(e);
            screen.Update(frameTime);
            gui.Update(frameTime);
        }


        private void CdxWindow_WindowShown(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "Shown"));
            OnShown(e);
            //screen.Show();
        }

        private void CdxWindow_WindowResized(object? sender, WindowSizeEventArgs e)
        {
            width = e.Width;
            height = e.Height;
            Logger.Verbose(GetEventLogMsg(e, "Resized"));
            screen.Resized(width, height);
            gui.ScreenResized(width, Height);
        }

        private void CdxWindow_WindowMinimized(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "Minimized"));
        }

        private void CdxWindow_WindowMaxmimized(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "Maximized"));
        }

        private void CdxWindow_WindowHidden(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "Hidden"));
            OnHidden(e);
            //screen.Hide();
        }

        private void CdxWindow_WindowFocusGained(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "FocusGained"));
        }

        private void CdxWindow_WindowExposed(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "Exposed"));
        }

        private void CdxWindow_WindowMoved(object? sender, WindowPositionEventArgs e)
        {
            x = e.X;
            y = e.Y;
            Logger.Verbose(GetEventLogMsg(e, "Moved"));
        }

        private void CdxWindow_WindowSizeChanged(object? sender, WindowSizeEventArgs e)
        {
            width = e.Width;
            height = e.Height;
            Logger.Verbose(GetEventLogMsg(e, "SizeChanged"));
            screen.Resized(width, height);
            gui.ScreenResized(width, Height);
        }

        private void CdxWindow_WindowLeave(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "Leave"));
        }

        private void CdxWindow_WindowTakeFocus(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "TakeFocus"));
        }

        private void CdxWindow_WindowRestored(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "Restored"));
        }

        private void CdxWindow_WindowFocusLost(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "FocusLost"));
        }

        private void CdxWindow_WindowEnter(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "Enter"));
        }

        private void CdxWindow_WindowDisplayChanged(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "DisplayChanged"));
        }

        private void CdxWindow_WindowClose(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "Close"));
            OnClose(e);
            screen.Hide();
            Close();
        }

        private void CdxWindow_WindowLoad(object? sender, EventArgs e)
        {
            Logger.Verbose(GetEventLogMsg(e, "Load"));
            OnLoad(e);
        }
    }
}
