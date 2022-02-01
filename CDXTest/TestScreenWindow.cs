namespace CDXTest
{
    using CDX;
    using CDX.GUI;
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

        private IWindow? win2;
        private IGadget? gad4;
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
                    gad1 = gui.AddGadget(win1, leftEdge: 10, topEdge: 10, width: -20, height: 40, text: "Continue", clickAction: ContinueClick);
                    gad2 = gui.AddGadget(win1, leftEdge: 10, topEdge: 60, width: -20, height: 40, text: "New Game", clickAction: NewGameClick);
                    gad3 = gui.AddGadget(win1, leftEdge: 10, topEdge: 110, width: -20, height: 40, text: "Options", clickAction: OptionsClick);
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
                    gad4 = gui.AddGadget(win2, leftEdge: 10, topEdge: 10, width: -20, height: 40, text: "Back", clickAction: BackClick);
                    return screen2;
                }
            };
        }

        protected override void OnShown(EventArgs e)
        {
            Screen = titleScreen;
        }

        private void ContinueClick()
        {
            Screen = demoScreen;
        }

        private void NewGameClick()
        {

        }

        private void OptionsClick()
        {

        }

        private void BackClick()
        {
            Screen = titleScreen;
        }
    }
}
