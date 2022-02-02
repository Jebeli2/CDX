namespace GadgetGUI
{
    using CDX;
    using CDX.App;
    using CDX.Graphics;
    using CDX.GUI;
    using CDX.Input;
    using CDX.Logging;
    using CDX.Utilities;

    using IScreen = CDX.GUI.IScreen;

    public class GUISystem : IGUISystem
    {
        private Screen? screen;

        //private CDXWindow cdx = new NoWindow();
        //private readonly List<Window> windows = new();
        private int mouseX;
        private int mouseY;
        //private bool changeingMousePosition;
        private bool selectMouseDown;
        //private Window? mouseWindow;
        //private Gadget? mouseGadget;
        //private Window? activeWindow;
        //private Gadget? activeGadget;
        private Gadget? downGadget;
        private Gadget? upGadget;
        private int nextGadgetID;
        private int screenWidth;
        private int screenHeight;


        private int sysGadgetWidth = 32;
        private int sysGadgetHeight = 28;

        private const int SYSGAD_DRAG = -1;
        private const int SYSGAD_CLOSE = -2;
        private const int SYSGAD_DEPTH = -3;
        private const int SYSGAD_ZOOM = -4;
        private const int SYSGAD_SIZE = -5;

        private IImage? buttonImg;

        public GUISystem()
        {
            Logger.Info($"GUI System created.");
        }

        public int ScreenWidth => screenWidth;
        public int ScreenHeight => screenHeight;
        //public void SetCDXWindow(CDXWindow cdx)
        //{
        //    this.cdx = cdx;
        //}
        public void ScreenResized(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;
        }

        public void Clear()
        {
            screen = null;
        }
        public void SetScreen(IScreen? screen)
        {
            if (screen is Screen scr)
            {
                if (this.screen != scr)
                {
                    this.screen = scr;
                    downGadget = null;
                    upGadget = null;
                    selectMouseDown = false;
                    FakeMouseMoved();
                }
            }
        }

        public void Initialize(CDXWindow cdx)
        {
            buttonImg = cdx.Graphics.LoadImage(Properties.Resources.buttonST9P, "button");
            if (buttonImg != null)
            {
                GUIRenderer.Instance.MakeButtonPatches(buttonImg);
            }
        }

        public void Shutdown()
        {
            buttonImg?.Dispose();
            buttonImg = null;
        }

        public void Update(FrameTime time)
        {
            screen?.Update(time);
        }
        public void Render(IGraphics gfx, FrameTime time)
        {
            screen?.Render(gfx, time);
        }

        public IScreen? OpenScreen(string? title = null)
        {
            Screen screen = new Screen(this);
            screen.Title = title;
            return screen;
        }


        public IWindow? OpenWindow(IScreen? screen, int leftEdge = 0, int topEdge = 0, int width = 256, int height = 256, string title = "")
        {
            return InternalOpenWindow(screen, leftEdge, topEdge, width, height, title);
        }

        private IWindow? InternalOpenWindow(IScreen? screen, int leftEdge, int topEdge, int width, int height, string title)
        {
            if (screen is Screen scr)
            {

                Window win = new Window(this, scr)
                {
                    LeftEdge = leftEdge,
                    TopEdge = topEdge,
                    Width = width,
                    Height = height,
                    Title = title
                };
                AddSystemGadgets(win);
                if ((win.WindowFlags & WindowFlags.Activate) == WindowFlags.Activate)
                {
                    scr.ActivateWindow(win);
                }
                return win;
            }
            return null;
        }

        private void AddSystemGadgets(Window win)
        {
            if ((win.WindowFlags & WindowFlags.DragBar) == WindowFlags.DragBar)
            {
                _ = new Gadget(this, win)
                {
                    LeftEdge = 0,
                    TopEdge = 0,
                    Width = 0,
                    Height = sysGadgetHeight,
                    Flags = GadgetFlags.RelWidth,
                    TransparentBackground = true,
                    GadgetID = SYSGAD_DRAG,
                    Activation = GadgetActivation.TopBorder | GadgetActivation.RightBorder | GadgetActivation.LeftBorder
                };
            }

            if ((win.WindowFlags & WindowFlags.CloseGadget) == WindowFlags.CloseGadget)
            {
                _ = new Gadget(this, win)
                {
                    LeftEdge = 0,
                    TopEdge = 0,
                    Width = sysGadgetWidth,
                    Height = sysGadgetHeight,
                    TransparentBackground = true,
                    Icon = CDX.Graphics.Icons.ENTYPO_ICON_CROSS,
                    GadgetID = SYSGAD_CLOSE,
                    Activation = GadgetActivation.TopBorder | GadgetActivation.LeftBorder
                };
            }

            int gadX = 0;


            if ((win.WindowFlags & WindowFlags.DepthGadget) == WindowFlags.DepthGadget)
            {
                gadX += sysGadgetWidth;
                _ = new Gadget(this, win)
                {
                    LeftEdge = -gadX,
                    TopEdge = 0,
                    Width = sysGadgetWidth,
                    Height = sysGadgetHeight,
                    Flags = GadgetFlags.RelRight,
                    TransparentBackground = true,
                    Icon = CDX.Graphics.Icons.ENTYPO_ICON_DOCUMENTS,
                    GadgetID = SYSGAD_DEPTH,
                    Activation = GadgetActivation.TopBorder | GadgetActivation.RightBorder
                };
            }

            if ((win.WindowFlags & WindowFlags.HasZoom) == WindowFlags.HasZoom)
            {
                gadX += sysGadgetWidth;
                _ = new Gadget(this, win)
                {
                    LeftEdge = -gadX,
                    TopEdge = 0,
                    Width = sysGadgetWidth,
                    Height = sysGadgetHeight,
                    Flags = GadgetFlags.RelRight,
                    TransparentBackground = true,
                    Icon = CDX.Graphics.Icons.ENTYPO_ICON_RESIZE_FULL_SCREEN,
                    GadgetID = SYSGAD_ZOOM,
                    Activation = GadgetActivation.TopBorder | GadgetActivation.RightBorder
                };
            }

            if ((win.WindowFlags & WindowFlags.SizeGadget) == WindowFlags.SizeGadget)
            {
                if ((win.WindowFlags & WindowFlags.SizeBRight) == WindowFlags.SizeBRight)
                {
                    win.BorderRight = sysGadgetWidth;
                }
                if ((win.WindowFlags & WindowFlags.SizeBBottom) == WindowFlags.SizeBBottom)
                {
                    win.BorderBottom = sysGadgetHeight;
                }
                _ = new Gadget(this, win)
                {
                    LeftEdge = -sysGadgetWidth,
                    TopEdge = -sysGadgetHeight,
                    Width = sysGadgetWidth,
                    Height = sysGadgetHeight,
                    Flags = GadgetFlags.RelRight | GadgetFlags.RelBottom,
                    TransparentBackground = true,
                    Icon = CDX.Graphics.Icons.ENTYPO_ICON_RETWEET,
                    GadgetID = SYSGAD_SIZE,
                    Activation = GadgetActivation.BottomBorder | GadgetActivation.RightBorder
                };
            }
        }

        public void CloseWindow(IWindow? window)
        {
            //if (window is Window win)
            //{
            //    windows.Remove(win);
            //}
        }
        public void ActivateWindow(IWindow? window)
        {
            if (window is Window win)
            {
                win.Screen.ActivateWindow(win);
            }
        }

        public void ActivateGadget(IGadget? gadget)
        {
            if (gadget is Gadget gad)
            {
                gad.Window.Screen.ActivateGadget(gad);
            }
        }

        public void ClickActiveGadget()
        {
            Gadget? gadget = screen?.ActiveGadget;
            if (gadget != null)
            {
                gadget.RaiseGadgetUp();
            }
        }


        public IGadget? AddGadget(IWindow? window,
            int leftEdge = 0,
            int topEdge = 0,
            int width = 100,
            int height = 100,
            GadgetFlags flags = GadgetFlags.None,
            GadgetActivation activation = GadgetActivation.Immediate | GadgetActivation.RelVerify,
            string? text = null,
            bool disabled = false,
            Action? clickAction = null,
            int gadgetID = -1)
        {
            if (window is Window win)
            {
                if (topEdge <= 0) { flags |= GadgetFlags.RelBottom; }
                if (leftEdge <= 0) { flags |= GadgetFlags.RelRight; }
                if (width <= 0) { flags |= GadgetFlags.RelWidth; }
                if (height <= 0) { flags |= GadgetFlags.RelHeight; }
                if (disabled) { flags |= GadgetFlags.Disabled; }

                if (gadgetID < 0) { gadgetID = nextGadgetID++; }
                else { nextGadgetID = Math.Max(nextGadgetID, gadgetID); }
                Gadget gadget = new Gadget(this, win)
                {
                    LeftEdge = leftEdge,
                    TopEdge = topEdge,
                    Width = width,
                    Height = height,
                    Flags = flags,
                    Activation = activation,
                    Text = text,
                    GadgetID = gadgetID
                };
                if (clickAction != null)
                {
                    gadget.GadgetUp += (s, e) => { clickAction(); };
                }
                return gadget;
            }
            return null;
        }

        public void RemGadget(IGadget? gadget)
        {
            //if (gadget is Gadget gad)
            //{
            //    gad.Parent?.RemChild(gad);
            //}
        }

        private bool CheckWindowDragging(MouseMotionEventArgs e)
        {
            if (screen != null &&
                selectMouseDown &&
                screen.ActiveGadget != null &&
                screen.ActiveGadget.GadgetID == SYSGAD_DRAG &&
                screen.ActiveWindow != null &&
                screen.ActiveGadget.Window == screen.ActiveWindow)
            {
                screen.ActiveWindow.MoveWindow(e.RelX, e.RelY);
                //        if (activeWindow.AdjustMovingWindowBounds(out bool preventX, out bool preventY))
                //        {
                //            int x = e.X;
                //            int y = e.Y;
                //            if (preventX) { x -= e.RelX; }
                //            if (preventY) { y -= e.RelY; }
                //            Logger.Verbose($"Preventing Mouse Move: {x}x{y}");
                //            SetMousePosition(x, y);
                //}
                return true;
            }
            return false;
        }

        private bool CheckWindowSizing(MouseMotionEventArgs e)
        {
            if (screen != null &&
                selectMouseDown &&
                screen.ActiveGadget != null &&
                screen.ActiveGadget.GadgetID == SYSGAD_SIZE &&
                screen.ActiveWindow != null &&
                screen.ActiveGadget.Window == screen.ActiveWindow)
            {
                screen.ActiveWindow.SizeWindow(e.RelX, e.RelY);
                //        if (activeWindow.AdjustSizingWindowBounds(out bool preventX, out bool preventY))
                //        {
                //            int x = e.X;
                //            int y = e.Y;
                //            if (preventX) { x -= e.RelX; }
                //            if (preventY) { y -= e.RelY; }
                //            Logger.Verbose($"Preventing Mouse Move: {x}x{y}");
                //            SetMousePosition(x, y);
                //        }
                return true;
            }
            return false;
        }

        private bool CheckGadgetDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                downGadget = screen?.MouseGadget;
                if (downGadget != null)
                {
                    if (downGadget.Immediate)
                    {
                        downGadget.RaiseGadgetDown();
                        return true;
                    }
                }
            }
            return false;
        }

        private bool CheckGadgetUp(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                upGadget = screen?.MouseGadget;
                if (upGadget != null && upGadget == downGadget)
                {
                    if (upGadget.RelVeriy)
                    {
                        upGadget.RaiseGadgetUp();
                        return true;
                    }
                }
            }
            return false;
        }

        //private void OnGadgetDown(Event)

        private void Touch2Screen(float x, float y, out int sx, out int sy)
        {
            sx = (int)(x * screenWidth);
            sy = (int)(y * screenHeight);
        }

        public void OnControllerButtonDown(ControllerButtonEventArgs e)
        {
        }
        public void OnControllerButtonUp(ControllerButtonEventArgs e)
        {
        }
        public void OnControllerAxis(ControllerAxisEventArgs e)
        {

        }

        public void OnTouchFingerDown(TouchFingerEventArgs e)
        {
            Touch2Screen(e.X, e.Y, out mouseX, out mouseY);
            if (screen == null) return;
            Window? win = screen.FindWindow(mouseX, mouseY);
            screen.SetMouseWindow(win);
            screen.SetActiveWindow(win);
            Gadget? gad = win?.FindGadget(mouseX, mouseY);
            screen.SetMouseGadget(gad);
            screen.SetActiveGadget(gad);
        }
        public void OnTouchFingerUp(TouchFingerEventArgs e)
        {
            Touch2Screen(e.X, e.Y, out mouseX, out mouseY);
            if (screen == null) return;
            Window? win = screen.FindWindow(mouseX, mouseY);
            screen.SetMouseWindow(win);
            Gadget? gad = win?.FindGadget(mouseX, mouseY);
            screen.SetMouseGadget(gad);

        }

        public void OnTouchFingerMotion(TouchFingerEventArgs e)
        {
            Touch2Screen(e.X, e.Y, out mouseX, out mouseY);
            if (screen == null) return;
            Window? win = screen.FindWindow(mouseX, mouseY);
            screen.SetMouseWindow(win);
            Gadget? gad = win?.FindGadget(mouseX, mouseY);
            screen.SetMouseGadget(gad);
        }

        public void OnMouseButtonDown(MouseButtonEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
            if (e.Button == MouseButton.Left) { selectMouseDown = true; }
            if (screen == null) return;
            Window? win = screen.FindWindow(mouseX, mouseY);
            screen.SetMouseWindow(win);
            screen.SetActiveWindow(win);
            Gadget? gad = win?.FindGadget(mouseX, mouseY);
            screen.SetMouseGadget(gad);
            screen.SetActiveGadget(gad);
            //Window? window = FindWindow(mouseX, mouseY);
            //SetMouseWindow(window);
            //Gadget? gadget = window?.FindGadget(mouseX, mouseY);
            //SetMouseGadget(gadget);
            //SetActiveWindow(window);
            //SetActiveGadget(gadget);
            if (CheckGadgetDown(e))
            {

            }
        }

        public void OnMouseButtonUp(MouseButtonEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
            if (e.Button == MouseButton.Left) { selectMouseDown = false; }
            if (screen == null) return;
            Window? win = screen.FindWindow(mouseX, mouseY);
            screen.SetMouseWindow(win);
            Gadget? gad = win?.FindGadget(mouseX, mouseY);
            screen.SetMouseGadget(gad);
            //Window? window = FindWindow(mouseX, mouseY);
            //SetMouseWindow(window);
            //Gadget? gadget = window?.FindGadget(mouseX, mouseY);
            //SetMouseGadget(gadget);
            if (CheckGadgetUp(e))
            {

            }
        }

        public void OnMouseMoved(MouseMotionEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
            if (screen == null) return;
            Window? win = screen.FindWindow(mouseX, mouseY);
            screen.SetMouseWindow(win);
            Gadget? gad = win?.FindGadget(mouseX, mouseY);
            screen.SetMouseGadget(gad);
            if (CheckWindowDragging(e)) return;
            if (CheckWindowSizing(e)) return;

            //if (changeingMousePosition)
            //{
            //    changeingMousePosition = false;
            //    return;
            //}
            //if (CheckWindowDrag(e)) return;
            //if (CheckWindowSizing(e)) return;
            //Window? window = FindWindow(mouseX, mouseY);
            //SetMouseWindow(window);
            //Gadget? gadget = window?.FindGadget(mouseX, mouseY);
            //SetMouseGadget(gadget);
        }

        private void FakeMouseMoved()
        {
            if (screen == null) return;
            Window? win = screen.FindWindow(mouseX, mouseY);
            screen.SetMouseWindow(win);
            Gadget? gad = win?.FindGadget(mouseX, mouseY);
            screen.SetMouseGadget(gad);
        }

        public void OnMouseWheel(MouseWheelEventArgs e)
        {

        }

        public void OnKeyDown(KeyEventArgs e)
        {

        }

        public void OnKeyUp(KeyEventArgs e)
        {
        }

        public void OnTextInput(TextInputEventArgs e)
        {

        }

    }
}