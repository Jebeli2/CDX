namespace CDXTest
{
    using CDX;
    using CDX.App;
    using CDX.Audio;
    using CDX.Graphics;
    using CDX.Input;
    using CDX.Logging;
    using CDX.Screens;
    using CDX.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class TestWindow : Window
    {
        private RainingBoxesApp rainingBoxes = new();
        private MusicPlayerApp music = new();


        public TestWindow()
            : base("Test Window")
        {
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            //switch (e.KeyCode)
            //{
            //    case KeyCode.LEFT:
            //        lines -= 2;
            //        if (lines < 2) { lines = 2; }
            //        break;
            //    case KeyCode.RIGHT:
            //        lines += 2;
            //        if (lines > Width) { lines = Width; }
            //        break;
            //}
        }

        protected override void OnLoad(LoadEventArgs e)
        {
            InstallDefaultHotKeys();
            AddApplet(music);
            AddApplet(rainingBoxes);
        }

        protected override void OnShown(EventArgs e)
        {
            //Audio.NextMusic();
        }


        protected override void OnClose(EventArgs e)
        {
            //Audio.ClearPlayList();
            //Audio.StopMusic();
            //Audio.AudioDataReceived -= Audio_AudioDataReceived;
            //bar?.Dispose();
            //barF?.Dispose();
            //img1?.Dispose();
            //font1?.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
        }

    }
}
