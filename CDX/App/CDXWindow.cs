namespace CDX.App
{
    using CDX.Graphics;
    using CDX.Input;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class CDXWindow
    {
        protected readonly Window window;
        protected IGraphics graphics;
        protected bool showFPS;
        protected float fpsPosX;
        protected float fpsPosY;

        private readonly EventHandlerList eventHandlerList = new();

        private static readonly object windowLoadEventKey = new();
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
        private static readonly object windowDisplayChangedEventKey = new();
        private static readonly object mouseButtonDownEventKey = new();
        private static readonly object mouseButtonUpEventKey = new();
        private static readonly object mouseMoveEventKey = new();
        private static readonly object mouseWheelEventKey = new();
        private static readonly object keyDownEventKey = new();
        private static readonly object keyUpEventKey = new();
        private static readonly object textInputEventKey = new();

        private static readonly object rendererPaintEventKey = new();
        private static readonly object updateEventKey = new();
        private static readonly object controlerButtonUpEvenetKey = new();
        private static readonly object controlerButtonDownEvenetKey = new();
        private static readonly object controlerAxisEvenetKey = new();

        private static readonly object touchFingerUpEventKey = new();
        private static readonly object touchFingerDownEventKey = new();
        private static readonly object touchFingerMotionEventKey = new();

        protected CDXWindow()
        {
            graphics = NoGraphics.Instance;
            window = new Window(this);
        }
        protected CDXWindow(Window window)
        {
            graphics = NoGraphics.Instance;
            this.window = window;
            this.window.LinkWindow(this);
        }

        public string Driver
        {
            get => window.Driver;
        }

        public bool VSync
        {
            get => window.VSync;
        }
        public bool ShowFPS
        {
            get => showFPS;
            set => showFPS = value;
        }

        public float FPSPosX
        {
            get => fpsPosX;
            set => fpsPosX = value;
        }
        public float FPSPosY
        {
            get => fpsPosY;
            set => fpsPosY = value;
        }
        public abstract uint WindowID { get; }
        public abstract IAudio Audio { get; }
        public IGraphics Graphics => graphics;

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

        public event WindowPositionEventHandler WindowMoved
        {
            add => eventHandlerList.AddHandler(windowMovedEventKey, value); remove => eventHandlerList.RemoveHandler(windowMovedEventKey, value);
        }

        public event WindowSizeEventHandler WindowResized
        {
            add => eventHandlerList.AddHandler(windowResizedEventKey, value); remove => eventHandlerList.RemoveHandler(windowResizedEventKey, value);
        }

        public event WindowSizeEventHandler WindowSizeChanged
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

        public event EventHandler WindowDisplayChanged
        {
            add => eventHandlerList.AddHandler(windowDisplayChangedEventKey, value); remove => eventHandlerList.RemoveHandler(windowDisplayChangedEventKey, value);
        }

        public event PaintEventHandler WindowPaint
        {
            add => eventHandlerList.AddHandler(rendererPaintEventKey, value); remove => eventHandlerList.RemoveHandler(rendererPaintEventKey, value);
        }

        public event UpdateEventHandler WindowUpdate
        {
            add => eventHandlerList.AddHandler(updateEventKey, value); remove => eventHandlerList.RemoveHandler(updateEventKey, value);
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

        public event ControllerButtonEventHandler ControllerButtonDown
        {
            add => eventHandlerList.AddHandler(controlerButtonDownEvenetKey, value); remove => eventHandlerList.RemoveHandler(controlerButtonDownEvenetKey, value);
        }
        public event ControllerButtonEventHandler ControllerButtonUp
        {
            add => eventHandlerList.AddHandler(controlerButtonUpEvenetKey, value); remove => eventHandlerList.RemoveHandler(controlerButtonUpEvenetKey, value);
        }
        public event ControllerAxisEventHandler ControllerAxis
        {
            add => eventHandlerList.AddHandler(controlerAxisEvenetKey, value); remove => eventHandlerList.RemoveHandler(controlerAxisEvenetKey, value);
        }

        public event TouchFingerEventHandler TouchFingerDown
        {
            add => eventHandlerList.AddHandler(touchFingerDownEventKey, value); remove => eventHandlerList.RemoveHandler(touchFingerDownEventKey, value);
        }
        public event TouchFingerEventHandler TouchFingerUp
        {
            add => eventHandlerList.AddHandler(touchFingerUpEventKey, value); remove => eventHandlerList.RemoveHandler(touchFingerUpEventKey, value);
        }
        public event TouchFingerEventHandler TouchFingerMotion
        {
            add => eventHandlerList.AddHandler(touchFingerMotionEventKey, value); remove => eventHandlerList.RemoveHandler(touchFingerMotionEventKey, value);
        }

        protected virtual void OnWindowLoad(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowLoadEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowShown(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowShownEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowHidden(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowHiddenEventKey])?.Invoke(this, e);
        }

        protected virtual void OnWindowExposed(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowExposedEventKey])?.Invoke(this, e);
        }

        protected virtual void OnWindowMoved(WindowPositionEventArgs e)
        {
            ((WindowPositionEventHandler?)eventHandlerList[windowMovedEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowResized(WindowSizeEventArgs e)
        {
            ((WindowSizeEventHandler?)eventHandlerList[windowResizedEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowSizeChanged(WindowSizeEventArgs e)
        {
            ((WindowSizeEventHandler?)eventHandlerList[windowSizeChangedEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowMinimized(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowMinimizedEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowMaximized(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowMaximizedEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowRestored(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowRestoredEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowEnter(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowEnterEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowLeave(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowLeaveEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowFocusGained(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowFocusGainedEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowFocusLost(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowFocusLostEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowClose(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowCloseEventKey])?.Invoke(this, e);
            //switch (closeOperation)
            //{
            //    case WindowCloseOperation.Close:
            //        Close();
            //        break;
            //    case WindowCloseOperation.Exit:
            //        app.Exit();
            //        break;
            //    case WindowCloseOperation.DoNothing:
            //        break;
            //}
        }
        protected virtual void OnWindowTakeFocus(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowTakeFocusEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowDisplayChanged(EventArgs e)
        {
            ((EventHandler?)eventHandlerList[windowDisplayChangedEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowPaint(PaintEventArgs e)
        {
            ((PaintEventHandler?)eventHandlerList[rendererPaintEventKey])?.Invoke(this, e);
        }
        protected virtual void OnWindowUpdate(UpdateEventArgs e)
        {
            ((UpdateEventHandler?)eventHandlerList[updateEventKey])?.Invoke(this, e);
        }
        protected virtual void OnMouseButtonDown(MouseButtonEventArgs e)
        {
            ((MouseButtonEventHandler?)eventHandlerList[mouseButtonDownEventKey])?.Invoke(this, e);
        }
        protected virtual void OnMouseButtonUp(MouseButtonEventArgs e)
        {
            ((MouseButtonEventHandler?)eventHandlerList[mouseButtonUpEventKey])?.Invoke(this, e);
        }
        protected virtual void OnMouseMove(MouseMotionEventArgs e)
        {
            ((MouseMotionEventHandler?)eventHandlerList[mouseMoveEventKey])?.Invoke(this, e);
        }
        protected virtual void OnMouseWheel(MouseWheelEventArgs e)
        {
            ((MouseWheelEventHandler?)eventHandlerList[mouseWheelEventKey])?.Invoke(this, e);
        }
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            ((KeyEventHandler?)eventHandlerList[keyDownEventKey])?.Invoke(this, e);
        }
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            ((KeyEventHandler?)eventHandlerList[keyUpEventKey])?.Invoke(this, e);
        }
        protected virtual void OnTextInput(TextInputEventArgs e)
        {
            ((TextInputEventHandler?)eventHandlerList[textInputEventKey])?.Invoke(this, e);
        }
        protected virtual void OnControllerButtonDown(ControllerButtonEventArgs e)
        {
            ((ControllerButtonEventHandler?)eventHandlerList[controlerButtonDownEvenetKey])?.Invoke(this, e);
        }
        protected virtual void OnControllerButtonUp(ControllerButtonEventArgs e)
        {
            ((ControllerButtonEventHandler?)eventHandlerList[controlerButtonUpEvenetKey])?.Invoke(this, e);
        }
        protected virtual void OnControllerAxis(ControllerAxisEventArgs e)
        {
            ((ControllerAxisEventHandler?)eventHandlerList[controlerAxisEvenetKey])?.Invoke(this, e);
        }
        protected virtual void OnTouchFingerDown(TouchFingerEventArgs e)
        {
            ((TouchFingerEventHandler?)eventHandlerList[touchFingerDownEventKey])?.Invoke(this, e);
        }
        protected virtual void OnTouchFingerUp(TouchFingerEventArgs e)
        {
            ((TouchFingerEventHandler?)eventHandlerList[touchFingerUpEventKey])?.Invoke(this, e);
        }

        protected virtual void OnTouchFingerMotion(TouchFingerEventArgs e)
        {
            ((TouchFingerEventHandler?)eventHandlerList[touchFingerMotionEventKey])?.Invoke(this, e);
        }

        public abstract void SetTitle(string title);
        public abstract void SetVisible(bool visible);
        public abstract void SetResizable(bool resizable);
        public abstract void SetAlwaysOnTop(bool alwaysOnTop);
        public abstract void SetBorderless(bool borderless);
        public abstract void SetFullScreen(bool fullScreen);
        public abstract void SetPosition(int x, int y);
        public abstract void SetSize(int width, int height);
        public abstract void SetMousePosition(int x, int y);
        public abstract void Show();
        public abstract void Hide();
        public abstract void Close();
    }
}
