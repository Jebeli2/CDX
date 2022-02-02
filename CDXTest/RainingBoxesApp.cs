/*
 Inspired by Lesson 08 of Maxwell Slaters cpp course: "Timing: Frame Rate, Physics, Animation"
 https://thenumbat.github.io/cpp-course/sdl2/08/08.html

 */
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

    internal class RainingBoxesApp : CDXApplet
    {
        private static readonly Random random = new Random();
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
        private double time;
        private IImage? box;

        public RainingBoxesApp()
            : base("Boxes")
        {

        }

        public override void OnLoad(LoadEventArgs e)
        {
            box = LoadImage("box");
        }

        public override void OnUpdate(UpdateEventArgs e)
        {
            time = e.FrameTime.TotalTime.TotalSeconds;
            UpdateSquares();
        }
        public override void OnPaint(PaintEventArgs e)
        {
            PaintSquares(e.Graphics, squares);
        }

        public override void OnMouseButtonDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left) { leftMouseDown = true; }
            if (leftMouseDown) { AddSquare(e.X, e.Y); }
        }

        public override void OnMouseButtonUp(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left) { leftMouseDown = false; }
        }

        public override void OnMouseMoved(MouseMotionEventArgs e)
        {
            if (leftMouseDown) { AddSquare(e.X, e.Y); }
        }

        private static int Rand() { return random.Next(); }
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
                born = time,
                lastUpdate = time,
                duration = Rand() % 4 + 1
            };
            s.x -= s.w / 2;
            s.y -= s.h / 2;
            squares.Add(s);
        }

        private void UpdateSquares()
        {
            int index = 0;
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

        private void PaintSquares(IGraphics gfx, IList<Square> squares)
        {
            gfx.BlendMode = BlendMode.Blend;
            foreach (var s in squares)
            {
                gfx.DrawImage(box, new Rectangle((int)s.x, (int)s.y, (int)s.w, (int)s.h));
            }
        }
    }
}
