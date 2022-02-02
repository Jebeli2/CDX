namespace CDX
{
    using CDX.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGraphics
    {
        int Width { get; }
        int Height { get; }
        Rectangle Viewport { get; set; }
        void ResetViewport();
        Color Color { get; set; }
        BlendMode BlendMode { get; set; }
        TextureFilter TextureFilter { get; set; }
        void ClearScreen(Color color);
        void DrawPoint(Point p);
        void DrawPoint(PointF p);
        void DrawPoint(int x, int y);
        void DrawPoint(float x, float y);
        void DrawLine(int x1, int y1, int x2, int y2);
        void DrawLine(float x1, float y1, float x2, float y2);
        void DrawLines(IEnumerable<Point> points);
        void DrawLines(IEnumerable<PointF> points);
        void DrawRect(int x, int y, int w, int h);
        void DrawRect(float x, float y, float w, float h);
        void DrawRect(Rectangle rect);
        void DrawRect(RectangleF rect);
        void FillRect(int x, int y, int w, int h);
        void FillRect(float x, float y, float w, float h);
        void FillRect(Rectangle rect);
        void FillRect(RectangleF rect);
        void DrawImage(IImage? image);
        void DrawImage(IImage? image, RectangleF dst);
        void DrawImage(IImage? image, Rectangle dst);
        void DrawImage(IImage? image, Rectangle src, RectangleF dst);
        void DrawImage(IImage? image, Rectangle src, Rectangle dst);
        void DrawImage(IImage? image, RenderFlip flip);
        void DrawImage(IImage? image, int x, int y, int w, int h)
        {
            if (image != null)
            {
                DrawImage(image, new Rectangle(x, y, w, h));
            }
        }
        void DrawImageRegion(IImageRegion? image, RectangleF dst)
        {
            if (image != null)
            {
                DrawImage(image.Image, image.SourceRect, dst);
            }
        }
        void DrawImageRegion(IImageRegion? image, float x, float y, float width, float height)
        {
            if (image != null)
            {
                DrawImage(image.Image, image.SourceRect, new RectangleF(x, y, width, height));
            }
        }

        void DrawImageRegion(IImageRegion? image, float x, float y)
        {
            if (image != null)
            {
                DrawImage(image.Image, image.SourceRect, new RectangleF(x, y, image.Width, image.Height));
            }
        }
        void DrawImageRegion(IImageRegion? image, int x, int y)
        {
            if (image != null)
            {
                DrawImage(image.Image, image.SourceRect, new Rectangle(x, y, image.Width, image.Height));
            }
        }

        void DrawText(ITextFont? font, string? text, float x, float y, float width, float height, Color color, HorizontalAlignment hAlign = HorizontalAlignment.Center, VerticalAlignment vAlign = VerticalAlignment.Center);
        void DrawText(ITextFont? font, string? text, float x, float y, Color color)
        {
            DrawText(font, text, x, y, 0, 0, color, HorizontalAlignment.Left, VerticalAlignment.Top);
        }
        void DrawText(ITextFont? font, string? text, float x, float y)
        {
            DrawText(font, text, x, y, Color);
        }

        void DrawIcon(Icons icon, float x, float y, float width, float height, Color color, HorizontalAlignment hAlign = HorizontalAlignment.Center, VerticalAlignment vAlign = VerticalAlignment.Center);
        void DrawIcon(Icons icon, float x, float y, Color color)
        {
            DrawIcon(icon, x, y, 0, 0, color, HorizontalAlignment.Left, VerticalAlignment.Top);
        }

        void DrawIcon(Icons icon, float x, float y)
        {
            DrawIcon(icon, x, y, Color);
        }

        void DrawNinePatch(NinePatch? patch, int x, int y, int width, int height, NinePatchFillMode mode = NinePatchFillMode.Repeat, bool drawCenterToo = true)
        {
            if (patch != null)
            {
                var sprites = patch.GetRegions();
                int pw = patch.PatchWidth;
                int ph = patch.PatchHeight;
                if (sprites.Count >= 9 && pw >= 0 && ph >= 0)
                {
                    if (mode == NinePatchFillMode.Repeat)
                    {
                        int startX = x + pw;
                        int endX = x + width - pw;
                        int startY = y + ph;
                        int endY = y + height - ph;

                        DrawImageRegion(sprites[NinePatch.TOP_LEFT], x, y);
                        for (int ix = startX; ix < endX; ix += pw)
                        {
                            DrawImageRegion(sprites[NinePatch.TOP_CENTER], ix, y);
                        }
                        DrawImageRegion(sprites[NinePatch.TOP_RIGHT], x + width - pw, y);
                        for (int iy = startY; iy < endY; iy += ph)
                        {
                            DrawImageRegion(sprites[NinePatch.MIDDLE_LEFT], x, iy);
                            if (drawCenterToo)
                            {
                                for (int ix = startX; ix < endX; ix += pw)
                                {
                                    DrawImageRegion(sprites[NinePatch.MIDDLE_CENTER], ix, iy);
                                }
                            }
                            DrawImageRegion(sprites[NinePatch.MIDDLE_RIGHT], x + width - pw, iy);
                        }
                        DrawImageRegion(sprites[NinePatch.BOTTOM_LEFT], x, y + height - ph);
                        for (int ix = startX; ix < endX; ix += pw)
                        {
                            DrawImageRegion(sprites[NinePatch.BOTTOM_CENTER], ix, y + height - ph);
                        }
                        DrawImageRegion(sprites[NinePatch.BOTTOM_RIGHT], x + width - pw, y + height - ph);

                    }
                    else if (mode == NinePatchFillMode.Stretch)
                    {
                        int innerWidth = width - 2 * pw;
                        int innerHeight = height - 2 * ph;
                        DrawImageRegion(sprites[NinePatch.TOP_LEFT], x, y);
                        DrawImageRegion(sprites[NinePatch.TOP_CENTER], x + pw, y, innerWidth, ph);
                        DrawImageRegion(sprites[NinePatch.TOP_RIGHT], x + width - pw, y);
                        DrawImageRegion(sprites[NinePatch.MIDDLE_LEFT], x, y + ph, pw, innerHeight);
                        DrawImageRegion(sprites[NinePatch.MIDDLE_CENTER], x + pw, y + ph, innerWidth, innerHeight);
                        DrawImageRegion(sprites[NinePatch.MIDDLE_RIGHT], x + width - pw, y + ph, pw, innerHeight);
                        DrawImageRegion(sprites[NinePatch.BOTTOM_LEFT], x, y + height - ph);
                        DrawImageRegion(sprites[NinePatch.BOTTOM_CENTER], x + pw, y + height - ph, innerWidth, ph);
                        DrawImageRegion(sprites[NinePatch.BOTTOM_RIGHT], x + width - pw, y + height - ph);
                    }
                }
            }
        }

        void SetTarget(IImage? image);
        void ClearTarget();

        IImage? CreateImage(string name, int width, int height);
        IImage? LoadImage(string name);
        IImage? LoadImage(byte[] data, string name);
        ITextFont? LoadFont(string name, int ptSize);
        ITextFont? LoadFont(byte[] data, string name, int ptSize);

    }
}
