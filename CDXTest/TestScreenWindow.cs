namespace CDXTest
{
    using CDX;
    using CDX.App;
    using CDX.GUI;
    using CDX.Input;
    using CDX.Screens;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class TestScreenWindow : Window
    {
        private readonly TitleScreen titleScreen;
        private readonly DemoScreen demoScreen;

        private CDX.GUI.IScreen? screen1;
        private CDX.GUI.IScreen? screen2;

        private IWindow? win1;
        private IGadget? gad1;
        private IGadget? gad2;
        private IGadget? gad3;
        private IGadget? gad4;

        private IWindow? win2;
        private IGadget? gad5;

        private IGadget? cGadget;

        private RainingBoxesApp rainingBoxes = new();
        private MusicPlayerApp musicPlayer = new();
        private LinesApp linesApp = new();
        public TestScreenWindow()
            : base("Test Screen")
        {
            InstallDefaultHotKeys();

            titleScreen = new TitleScreen(this, "Title Screen")
            {
                BackgroundImageName = "badlands",
                InitGUI = (gui) =>
                {
                    screen1 = gui.OpenScreen("Test Screen");
                    win1 = gui.OpenWindow(screen1, leftEdge: 200, topEdge: 200, width: 400, height: 400, title: "Test Window");
                    gad1 = gui.AddGadget(win1, leftEdge: 10, topEdge: 10, width: -20, height: 40, text: "Other", clickAction: OtherClick);
                    gad2 = gui.AddGadget(win1, leftEdge: 10, topEdge: 60, width: -20, height: 40, text: "Add Boxes", clickAction: () => { AddRemApp(rainingBoxes, gad2); });
                    gad3 = gui.AddGadget(win1, leftEdge: 10, topEdge: 110, width: -20, height: 40, text: "Add Music", clickAction: () => { AddRemApp(musicPlayer, gad3); });
                    gad4 = gui.AddGadget(win1, leftEdge: 10, topEdge: 160, width: -20, height: 40, text: "Add Lines", clickAction: () => { AddRemApp(linesApp, gad4); });
                    return screen1;
                }
            };
            demoScreen = new DemoScreen(this, "Demo Screen")
            {
                BackgroundImageName = "ice_palace",
                InitGUI = (gui) =>
                {
                    screen2 = gui.OpenScreen("Demo Screen");
                    win2 = gui.OpenWindow(screen2, leftEdge: 300, topEdge: 200, width: 400, height: 400, title: "Demo Window");
                    gad5 = gui.AddGadget(win2, leftEdge: 10, topEdge: 10, width: -20, height: 40, text: "Back", clickAction: BackClick);
                    return screen2;
                }
            };
        }

        protected override void OnShown(EventArgs e)
        {
            Screen = titleScreen;
            ActivateGaget(gad1);
            UseExtremeFullScreen = false;
        }

        private void AddRemApp(CDXApplet app, IGadget? gadget)
        {
            string text = "";
            if (app.Installed)
            {
                RemoveApplet(app);
                text = "Add " + app.Name;
            }
            else
            {
                AddApplet(app);
                text = "Remove " + app.Name;
            }
            if (gadget != null) { gadget.Text = text; }
        }

        private void OtherClick()
        {
            Screen = demoScreen;
        }

        private void BackClick()
        {
            Screen = titleScreen;
            ActivateGaget(gad1);
        }

        private void ActivateNextGaget()
        {
            if (cGadget == gad1)
            {
                ActivateGaget(gad2);
            }
            else if (cGadget == gad2)
            {
                ActivateGaget(gad3);
            }
            else if (cGadget == gad3)
            {
                ActivateGaget(gad4);
            }
        }

        private void ActivatePrevGadget()
        {
            if (cGadget == gad4)
            {
                ActivateGaget(gad3);
            }
            else if (cGadget == gad3)
            {
                ActivateGaget(gad2);
            }
            else if (cGadget == gad2)
            {
                ActivateGaget(gad1);
            }
        }

        private void ActivateGaget(IGadget? gadget)
        {
            cGadget = gadget;
            gui.ActivateGadget(cGadget);
        }

        private void ActivateGadgetClick()
        {
            gui.ClickActiveGadget();
        }

        protected override void OnControllerButtonUp(ControllerButtonEventArgs e)
        {
            if (e.Button == ControllerButton.Down)
            {
                ActivateNextGaget();
            }
            else if (e.Button == ControllerButton.Up)
            {
                ActivatePrevGadget();
            }
            else if (e.Button == ControllerButton.X)
            {
                ActivateGadgetClick();
            }
        }

        protected override void OnControllerAxis(ControllerAxisEventArgs e)
        {
            if (e.Direction.Y > 0.5)
            {
                ActivatePrevGadget();
            }
            else if (e.Direction.Y < -0.5)
            {
                ActivateNextGaget();
            }
        }
    }
}
