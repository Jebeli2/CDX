namespace CDXTest
{
    using CDX;
    using CDX.App;
    using CDX.Graphics;
    using CDX.Input;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class MusicPlayerApp : CDXApplet
    {

        private int lines = 400;
        private short[] audioData = Array.Empty<short>();
        private readonly List<short> audioDataList = new List<short>();
        private IImage? bar;
        private IImage? barF;
        private bool musicPaused;
        private readonly Color leftColor1 = Color.AliceBlue;
        private readonly Color rightColor1 = Color.Azure;
        private readonly Color leftColor2 = Color.Red;
        private readonly Color rightColor2 = Color.Orange;

        public MusicPlayerApp()
            : base("Music")
        {

        }

        public override void OnLoad(LoadEventArgs e)
        {
            bar = LoadImage("Bar");
            if (bar != null)
            {
                bar.AlphaMod = 128;
                bar.TextureFilter = TextureFilter.Linear;
            }
            barF = LoadImage("BarF");
            if (barF != null)
            {
                barF.AlphaMod = 128;
                barF.TextureFilter = TextureFilter.Linear;
            }
            Audio.AddToPlayList("bach");
            Audio.AddToPlayList("battle_theme");
            Audio.AddToPlayList("boss_theme");
            Audio.AddToPlayList("cave_theme");
            Audio.AddToPlayList("dungeon_theme");
            Audio.AddToPlayList("forest_theme");
            Audio.AddToPlayList("magical_theme");
            Audio.AddToPlayList("overworld_theme");
            Audio.AddToPlayList("safe_room_theme");
            Audio.AddToPlayList("title_theme");
            Audio.AddToPlayList("town_theme");
            Audio.AddToPlayList("unrest_theme");
            Audio.AddToPlayList("wind_ambient");
            Audio.AudioDataReceived += Audio_AudioDataReceived;
            Audio.AudioMusicFinished += Audio_AudioMusicFinished;
            lines = (Width / 2) & (0xFFFF - 1);
            //HotKeyManager.AddHotKey(ScanCode.SCANCODE_N, Audio.NextMusic);
            //HotKeyManager.AddHotKey(ScanCode.SCANCODE_P, Audio.PreviousMusic);
            //HotKeyManager.AddHotKey(ScanCode.SCANCODE_SPACE, TogglePause);

        }

        public override void OnPaint(PaintEventArgs e)
        {
            PaintMusicArray(e.Graphics, audioData, lines, true, true);
            PaintMusicList(e.Graphics, lines, true, true);
            string text = (Audio.CurrentMusic?.Name ?? "No Music") + " (" + lines + ")";
            e.Graphics.DrawText(null, text, Width / 2, Height / 2, 0, 0, Color.White);
        }

        public override void OnShown(EventArgs e)
        {
            Audio.NextMusic();
        }
        private void TogglePause()
        {
            if (Audio.IsPaused)
            {
                Audio.ResumeMusic();
                musicPaused = false;
            }
            else
            {
                musicPaused = true;
                Audio.PauseMusic();
            }
        }

        private void Audio_AudioMusicFinished(object sender, CDX.Audio.AudioMusicFinishedEventArgs e)
        {
            audioDataList.Clear();
        }

        private void Audio_AudioDataReceived(object sender, CDX.Audio.AudioDataEventArgs e)
        {
            if (!musicPaused)
            {
                audioData = e.Data;
                audioDataList.AddRange(e.Data);
            }

        }
        private void PaintMusicList(IGraphics gfx, int lines, bool drawLeft, bool drawRight)
        {
            int maxLines = Math.Min(lines * 2, audioDataList.Count);
            if (maxLines > 2)
            {
                short[] data = audioDataList.Take(maxLines).ToArray();
                audioDataList.RemoveRange(0, maxLines - 2);
                PaintMusicArray2(gfx, data, lines, drawLeft, drawRight);
            }
        }

        private void PaintMusicArray2(IGraphics gfx, short[] data, int lines, bool drawLeft, bool drawRight)
        {
            if (data.Length > 0)
            {
                int w = gfx.Width;
                int h = gfx.Height;
                int len = data.Length;

                //int cut = 2;
                float offsetX = 10;
                float dW = w - offsetX * 2;
                float lineWidth = dW / lines;
                float midY = h / 2.0f;
                float facY = h / 3.0f / 32000.0f;
                float endW = w - 2 * offsetX;
                if (drawLeft && bar != null && barF != null)
                {
                    bar.AlphaMod = 88;
                    bar.ColorMod = leftColor1;
                    barF.AlphaMod = 88;
                    barF.ColorMod = leftColor1;
                    float pX = offsetX;
                    int index = 0;
                    while (pX < endW && index < data.Length)
                    {
                        float sample = data[index];
                        PaintSample(gfx, pX, midY, lineWidth, sample * facY);
                        pX += lineWidth;
                        index += 2;
                    }
                }
                if (drawRight && bar != null && barF != null)
                {
                    bar.AlphaMod = 64;
                    bar.ColorMod = rightColor1;
                    barF.AlphaMod = 64;
                    barF.ColorMod = rightColor1;
                    float pX = offsetX + lineWidth / 2;
                    int index = 1;
                    while (pX < endW && index < data.Length)
                    {
                        float sample = data[index];
                        PaintSample(gfx, pX, midY, lineWidth, sample * facY);
                        pX += lineWidth;
                        index += 2;
                    }
                }
            }
        }
        private void PaintMusicArray(IGraphics gfx, short[] data, int lines, bool drawLeft, bool drawRight)
        {
            if (data.Length > 0)
            {
                int w = gfx.Width;
                int h = gfx.Height;
                int len = data.Length;

                int cut = len / lines;
                float offsetX = 10;
                float dW = w - offsetX * 2;
                float lineWidth = dW / lines;
                float midY = h / 2.0f;
                float facY = h / 3.0f / 32000.0f;
                float endW = w - 2 * offsetX;
                if (drawLeft && bar != null && barF != null)
                {
                    bar.AlphaMod = 130;
                    bar.ColorMod = leftColor2;
                    barF.AlphaMod = 130;
                    barF.ColorMod = leftColor2;
                    float pX = offsetX;
                    int index = 0;
                    while (pX < endW && index < data.Length)
                    {
                        float sample = data[index];
                        PaintSample(gfx, pX, midY, lineWidth, sample * facY);
                        pX += lineWidth;
                        index += cut;
                    }
                }
                if (drawRight && bar != null && barF != null)
                {
                    bar.AlphaMod = 88;
                    bar.ColorMod = rightColor2;
                    barF.AlphaMod = 88;
                    barF.ColorMod = rightColor2;
                    float pX = offsetX + lineWidth / 2;
                    int index = 1;
                    while (pX < endW && index < data.Length)
                    {
                        float sample = data[index];
                        PaintSample(gfx, pX, midY, lineWidth, sample * facY);
                        pX += lineWidth;
                        index += cut;
                    }
                }
            }
        }

        private void PaintSample(IGraphics gfx, float xPos, float yMid, float lineWidth, float sample)
        {
            RectangleF dst = new RectangleF(xPos, yMid - sample, lineWidth, sample);
            if (sample < 0)
            {
                dst.Y = yMid;
                dst.Height = -sample;
                gfx.DrawImage(barF, dst);
            }
            else
            {
                gfx.DrawImage(bar, dst);
            }
        }
    }
}
