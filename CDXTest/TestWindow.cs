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
        private static readonly Random random = new Random();
        private int pc;
        private byte a = 200;
        private byte r = 150;
        private byte g = 150;
        private byte b = 150;
        private bool reverse;
        private PointF[] points = Array.Empty<PointF>();
        private IImage? img1;
        private ITextFont? font1;
        private int lines = 400;
        private short[] audioData = Array.Empty<short>();
        private readonly List<short> audioDataList = new List<short>();
        private IImage? bar;
        private IImage? barF;
        private IImage? box;
        private bool musicPaused;
        private const string SONG2 = @"d:\Users\jebel\Music\iTunes\iTunes Media\Music\Blur\Blur\02 Song 2.mp3";
        private class Square
        {
            public float x;
            public float y;
            public float w;
            public float h;
            public float xvelocity;
            public float yvelocity;
            public double born;
            public double lastUpdate;
            public double duration;
        }
        private const float GRAVITY = 750.0f;
        private readonly List<Square> squares = new();
        private bool leftMouseDown;

        public TestWindow()
            : base("Test Window")
        {
        }

        private static int Rand()
        {
            return random.Next();
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
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case KeyCode.LEFT:
                    lines -= 2;
                    if (lines < 2) { lines = 2; }
                    break;
                case KeyCode.RIGHT:
                    lines += 2;
                    if (lines > Width) { lines = Width; }
                    break;
            }
        }

        protected override void OnMouseButtonDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                AddSquare(e.X, e.Y);
                leftMouseDown = true;
            }
        }

        protected override void OnMouseButtonUp(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left)
            {
                leftMouseDown = false;
            }
        }

        protected override void OnMouseMoved(MouseMotionEventArgs e)
        {
            if (leftMouseDown)
            {
                AddSquare(e.X, e.Y);
            }
        }
        private void AddSquare(int x, int y)
        {
            Square s = new Square
            {
                x = x,
                y = y,
                w = Rand() % 80 + 40,
                h = Rand() % 80 + 40,
                yvelocity = -10,
                xvelocity = Rand() % 100 - 50,
                born = frameTime.TotalTime.TotalSeconds,
                lastUpdate = frameTime.TotalTime.TotalSeconds,
                duration = Rand() % 4 + 1
            };
            s.x -= s.w / 2;
            s.y -= s.h / 2;
            squares.Add(s);
        }

        protected override void OnLoad(EventArgs e)
        {
            InstallDefaultHotKeys();            

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
            box = LoadImage("box");
            //img1 = LoadImage("badlands");
            img1 = LoadImage("ice_palace");
            font1 = LoadFont("LiberationSans-Regular", 16);
            HotKeyManager.AddHotKey(ScanCode.SCANCODE_N, Audio.NextMusic);
            HotKeyManager.AddHotKey(ScanCode.SCANCODE_P, Audio.PreviousMusic);
            HotKeyManager.AddHotKey(ScanCode.SCANCODE_SPACE, TogglePause);
            Audio.AddToPlayList("Song 2");
            Audio.AddToPlayList("Deadcrush");
            Audio.AddToPlayList("Pleader");
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

        }

        private void Audio_AudioMusicFinished(object sender, AudioMusicFinishedEventArgs e)
        {
            audioDataList.Clear();
        }

        private void Audio_AudioDataReceived(object sender, AudioDataEventArgs e)
        {
            if (!musicPaused)
            {
                audioData = e.Data;
                audioDataList.AddRange(e.Data);
            }
        }

        protected override void OnShown(EventArgs e)
        {

            Audio.NextMusic();
        }


        protected override void OnClose(EventArgs e)
        {
            Audio.ClearPlayList();
            Audio.StopMusic();
            Audio.AudioDataReceived -= Audio_AudioDataReceived;
            bar?.Dispose();
            barF?.Dispose();
            box?.Dispose();
            img1?.Dispose();
            font1?.Dispose();
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            int index = 0;
            double time = e.FrameTime.TotalTime.TotalSeconds;
            int w = Width;
            int h = Height;
            while (index < squares.Count)
            {
                Square s = squares[index];
                float dT = (float)(time - s.lastUpdate);
                s.yvelocity += dT * GRAVITY;
                s.y += s.yvelocity * dT;
                s.x += s.xvelocity * dT;
                if (s.y > h - s.h)
                {
                    s.y = h - s.h;
                    s.xvelocity = 0;
                    s.yvelocity = 0;
                }
                s.lastUpdate = time;
                if (s.yvelocity <= 0 && s.lastUpdate > s.born + s.duration)
                {
                    squares.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }


        private Color leftColor1 = Color.AliceBlue;
        private Color rightColor1 = Color.Azure;
        private Color leftColor2 = Color.Red;
        private Color rightColor2 = Color.Orange;

        protected override void OnPaint(PaintEventArgs e)
        {
            IGraphics gfx = e.Graphics;
            int w = gfx.Width;
            int h = gfx.Height;
            gfx.DrawImage(img1);
            string text = (Audio?.CurrentMusic?.Name ?? "No Music") + " (" + lines + ")";
            gfx.DrawText(font1, text, w / 2, 10, 0, 0, Color.White);
            //gfx.BlendMode = BlendMode.Blend;
            PaintSquares(gfx, squares);
            PaintMusicArray(gfx, audioData, lines, true, true);
            PaintMusicList(gfx, lines, true, true);
        }

        private void PaintSquares(IGraphics gfx, IList<Square> squares)
        {
            gfx.BlendMode = BlendMode.Blend;
            //gfx.Color = Color.BurlyWood;
            foreach (var s in squares)
            {
                gfx.DrawImage(box, new Rectangle((int)s.x, (int)s.y, (int)s.w, (int)s.h));
                //gfx.FillRect(s.x, s.y, s.w, s.h);
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
                    //bar.ColorMod = Color.FromArgb(128, 255, 255);
                    barF.AlphaMod = 88;
                    barF.ColorMod = leftColor1;
                    //barF.ColorMod = Color.FromArgb(128, 255, 255);
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
                    //bar.ColorMod = Color.FromArgb(255, 128, 255);
                    barF.AlphaMod = 64;
                    barF.ColorMod = rightColor1;
                    //barF.ColorMod = Color.FromArgb(255, 128, 255);
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
                    //bar.ColorMod = Color.FromArgb(128, 128, 255);
                    barF.AlphaMod = 130;
                    barF.ColorMod = leftColor2;
                    //barF.ColorMod = Color.FromArgb(128, 128, 255);
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
                    //bar.ColorMod = Color.FromArgb(255, 128, 128);
                    barF.AlphaMod = 88;
                    barF.ColorMod = rightColor2;
                    //barF.ColorMod = Color.FromArgb(255, 128, 128);
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

        private void UpdatePoints(int w, int h, float time)
        {
            if (reverse)
            {
                r--;
                g--;
                b--;
            }
            else
            {
                r++;
                g++;
                b++;
            }
            if (r >= 255)
            {
                reverse = true;
            }
            else if (r <= 150)
            {
                reverse = false;
            }
            int numPoints = w - 20;
            if (numPoints != points.Length)
            {
                points = new PointF[numPoints];
            }
            for (int i = 0; i < numPoints; i++)
            {
                float x = i + 10;
                float y = h * 0.5f + h * 0.45f * MathF.Sin(x * time / MathF.Tau);
                points[i].X = x;
                points[i].Y = y;//= new PointF(x, y);
            }
        }
    }
}
