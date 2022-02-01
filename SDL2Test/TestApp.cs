namespace SDL2Test
{
    using SDL2;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class TestApp
    {
        private SDLApplication app = new SDLApplication();
        private SDLWindow? window1;
        private SDLWindow? window2;
        private SDLTexture? img1;
        private SDLTexture? img2;
        private SDLTexture? img3;
        private SDLTexture? img4;
        private RenderFlip flip = RenderFlip.None;

        private byte a = 200;
        private byte r = 150;
        private byte g = 150;
        private byte b = 150;
        private bool reverse;
        private PointF[] points = new PointF[0];
        private SDLRenderer.Vertex[] vertices = new SDLRenderer.Vertex[3];
        private SDLTexture? img5;
        private SDLFont? font;
        private int pc;
        public void Run()
        {
            app.ApplicationStart += SDLApplication_ApplicationStart;
            app.ApplicationExit += SDLApplication_ApplicationExit;
            app.Run();
        }
        private void SDLApplication_ApplicationStart(object? sender, EventArgs e)
        {
            window1 = app.CreateWindow("Test Window");
            window2 = app.CreateWindow("Other Window");

            window1.KeyUp += Window_KeyUp;
            window2.KeyUp += Window_KeyUp;
            window2.MouseButtonUp += Window2_MouseButtonUp;
            window1.WindowLoad += Window1_WindowLoad;
            window2.WindowLoad += Window2_WindowLoad;
            window1.RendererPaint += Window1_RendererPaint;
            window2.RendererPaint += Window2_RendererPaint;

            window1.Show();
            window2.Show();
        }

        private void Window2_MouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (flip)
            {
                case RenderFlip.None:
                    flip = RenderFlip.Horizontal;
                    break;
                case RenderFlip.Horizontal:
                    flip = RenderFlip.Vertical;
                    break;
                case RenderFlip.Vertical:
                    flip = RenderFlip.Both;
                    break;
                case RenderFlip.Both:
                    flip = RenderFlip.None;
                    break;
            }
        }

        private void Window1_RendererPaint(object sender, RendererEventArgs e)
        {
            if (sender is SDLWindow window)
            {
                int w = window.Width;
                int h = window.Height;
                e.Renderer.DrawTexture(img5);
                e.Renderer.DrawText(font, "Hello", w / 2, 10, 0, 0, Color.White);

                e.Renderer.BlendMode = BlendMode.Blend;
                e.Renderer.Color = Color.PeachPuff;
                e.Renderer.FillRect(100, 100, w * 0.5f, h * 0.5f);
                e.Renderer.DrawVertices(vertices);
                if (pc == 0)
                {
                    UpdatePoints(w, h, e.TotalTime);
                }
                pc++;
                pc %= 8;
                e.Renderer.Color = Color.FromArgb(a, r, g, b);
                e.Renderer.DrawLines(points);
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
                points[i] = new PointF(x, y);
            }
        }
        private void Window2_RendererPaint(object sender, RendererEventArgs e)
        {
            if (sender is SDLWindow window)
            {
                int w = window.Width / 2;
                int h = window.Height / 2;
                e.Renderer.Viewport = new Rectangle(0, 0, w, h);
                e.Renderer.DrawTexture(img1, flip);
                e.Renderer.Viewport = new Rectangle(w, 0, w, h);
                e.Renderer.DrawTexture(img2, flip);
                e.Renderer.Viewport = new Rectangle(0, h, w, h);
                e.Renderer.DrawTexture(img3, flip);
                e.Renderer.Viewport = new Rectangle(w, h, w, h);
                e.Renderer.DrawTexture(img4, flip);
            }
        }


        private void Window1_WindowLoad(object? sender, EventArgs e)
        {
            if (sender is SDLWindow window)
            {
                vertices[0] = new SDLRenderer.Vertex() { Position = new PointF(400, 150), R = 255, G = 0, B = 0, A = 255 };
                vertices[1] = new SDLRenderer.Vertex() { Position = new PointF(200, 450), R = 0, G = 0, B = 255, A = 255 };
                vertices[2] = new SDLRenderer.Vertex() { Position = new PointF(600, 450), R = 0, G = 255, B = 0, A = 255 };

                //img1 = LoadTexture(IMG1);
                img5 = window.LoadTexture(Properties.Resources.fire_temple);
                font = SDLWindow.LoadFont(Properties.Resources.LiberationSans_Regular, 16);
            }
        }

        private void Window2_WindowLoad(object? sender, EventArgs e)
        {
            if (sender is SDLWindow window)
            {
                img1 = window.LoadTexture(Properties.Resources.badlands);
                img2 = window.LoadTexture(Properties.Resources.ice_palace);
                img3 = window.LoadTexture(Properties.Resources.fire_temple);
                img4 = window.LoadTexture(Properties.Resources.obelisk);
            }
        }

        private void Window_KeyUp(object? sender, KeyEventArgs e)
        {
            if (sender is SDLWindow window)
            {
                if (e.KeyCode == KeyCode.SDLK_F12)
                {
                    window.FullScreen = !window.FullScreen;
                }
            }
        }

        private void SDLApplication_ApplicationExit(object? sender, EventArgs e)
        {

        }

    }
}
