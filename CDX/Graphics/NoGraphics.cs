namespace CDX.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class NoGraphics : IGraphics
    {
        private static readonly NoGraphics instance = new();

        public static NoGraphics Instance => instance;
        private NoGraphics() { }
        public int Width => 0;

        public int Height => 0;

        public Rectangle Viewport { get; set; }
        public Color Color { get; set; }
        public BlendMode BlendMode { get; set; }
        public TextureFilter TextureFilter { get; set; }

        public void DrawImage(IImage? image)
        {

        }

        public void DrawImage(IImage? image, RectangleF dst)
        {

        }

        public void DrawImage(IImage? image, Rectangle dst)
        {

        }

        public void DrawImage(IImage? image, Rectangle src, RectangleF dst)
        {

        }

        public void DrawImage(IImage? image, Rectangle src, Rectangle dst)
        {

        }

        public void DrawImage(IImage? image, RenderFlip flip)
        {

        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {

        }

        public void DrawLine(float x1, float y1, float x2, float y2)
        {

        }

        public void DrawLines(IEnumerable<Point> points)
        {

        }

        public void DrawLines(IEnumerable<PointF> points)
        {

        }

        public void DrawPoint(Point p)
        {

        }

        public void DrawPoint(PointF p)
        {

        }

        public void DrawPoint(int x, int y)
        {

        }

        public void DrawPoint(float x, float y)
        {

        }

        public void DrawRect(int x, int y, int w, int h)
        {

        }

        public void DrawRect(float x, float y, float w, float h)
        {

        }

        public void DrawRect(Rectangle rect)
        {

        }

        public void DrawRect(RectangleF rect)
        {

        }

        public void DrawText(ITextFont? font, string? text, float x, float y, float width, float height, Color color, HorizontalAlignment hAlign = HorizontalAlignment.Center, VerticalAlignment vAlign = VerticalAlignment.Center)
        {

        }

        public void DrawIcon(Icons icon, float x, float y, float width, float height, Color color, HorizontalAlignment hAlign = HorizontalAlignment.Center, VerticalAlignment vAlign = VerticalAlignment.Center)
        {

        }

        public void FillRect(int x, int y, int w, int h)
        {

        }

        public void FillRect(float x, float y, float w, float h)
        {

        }

        public void FillRect(Rectangle rect)
        {

        }

        public void FillRect(RectangleF rect)
        {

        }

        public ITextFont? LoadFont(string name, int ptSize)
        {
            return null;
        }

        public ITextFont? LoadFont(byte[] data, string name, int ptSize)
        {
            return null;
        }

        public IImage? LoadImage(string name)
        {
            return null;
        }

        public IImage? LoadImage(byte[] data, string name)
        {
            return null;
        }

        public void ResetViewport()
        {

        }
    }
}
