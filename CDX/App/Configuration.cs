namespace CDX.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Configuration
    {
        public Configuration()
        {
            CDXName = "CDXSDL.SDLApplication";
            GUIName = "GadgetGUI.GUISystem";
            //Driver = "direct3d";
            //Driver = "direct3d11";
            Driver = "opengl";
            //Driver = "opengles2";
            //Driver = "software";
            FullScreen = false;
            ShowFPS = true;
            FPSPosX = 10;
            FPSPosY = 4;
            MaxFPS = 75;
            VSync = false;
            Width = 1024;
            Height = 768;
            //BackBufferWidth = 1024;
            //BackBufferHeight = 576;
        }

        public string CDXName { get; set; }
        public string GUIName { get; set; }
        public string Driver { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int BackBufferWidth { get; set; }
        public int BackBufferHeight { get; set; }
        public bool FullScreen { get; set; }
        public int MaxFPS { get; set; }
        public bool VSync { get; set; }
        public bool ShowFPS { get; set; }
        public float FPSPosX { get; set; }
        public float FPSPosY { get; set; }
    }
}
