namespace SDL2
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLRenderer : IDisposable
    {
        private class TextCache
        {
            public TextCache(SDLFont font, string text, Color color, int width, int height, IntPtr handle)
            {
                Font = font;
                Text = text;
                Color = color;
                Width = width;
                Height = height;
                Handle = handle;
            }

            public SDLFont Font;
            public string Text;
            public Color Color;
            public int Width;
            public int Height;
            public IntPtr Handle;

            public bool Matches(SDLFont font, string text, Color color)
            {
                return (text == Text) && (font == Font) && (color == Color);
            }
        }

        private bool disposedValue;
        private SDLApplication app;
        private SDLWindow window;
        private uint rendererID;
        private IntPtr handle;
        private SDL_BlendMode blendMode;
        private byte colorR;
        private byte colorG;
        private byte colorB;
        private byte colorA;
        private Color color;
        private int width;
        private int height;
        private Rectangle viewPort;

        private readonly Dictionary<string, TextCache> textCache = new();
        private readonly List<string> textCacheKeys = new();
        private int textCacheLimit = 100;

        private SDLRenderer() { throw new NotImplementedException(); }
        internal SDLRenderer(SDLWindow window)
        {
            this.window = window;
            app = this.window.App;
            rendererID = window.WindowID;
            handle = CreateHandle(this.window);
        }

        private IntPtr CreateHandle(SDLWindow window)
        {
            IntPtr handle = SDL_CreateRenderer(window.Handle, 0, SDL_RendererFlags.ACCELERATED);
            if (handle != IntPtr.Zero)
            {
                _ = SDL_GetRenderDrawBlendMode(handle, out blendMode);
                _ = SDL_GetRenderDrawColor(handle, out colorR, out colorG, out colorB, out colorA);
                _ = SDL_GetRendererOutputSize(handle, out width, out height);
                _ = SDL_RenderGetViewport(handle, out viewPort);
            }
            return handle;
        }

        internal void WindowResized(int width, int height)
        {
            this.width = width;
            this.height = height;
            _ = SDL_GetRendererOutputSize(handle, out this.width, out this.height);
            _ = SDL_RenderGetViewport(handle, out viewPort);
        }

        public Rectangle Viewport
        {
            get => viewPort;
            set
            {
                viewPort = value;
                _ = SDL_RenderSetViewport(handle, ref viewPort);
            }
        }

        public void ResetViewport()
        {
            _ = SDL_RenderSetViewport(handle, IntPtr.Zero);
            viewPort = new Rectangle(0, 0, width, height);
        }

        public Color Color
        {
            get => color;
            set
            {
                if (color != value)
                {
                    color = value;
                    colorR = value.R;
                    colorG = value.G;
                    colorB = value.B;
                    colorA = value.A;
                    _ = SDL_SetRenderDrawColor(handle, colorR, colorG, colorB, colorA);
                }
            }
        }

        public BlendMode BlendMode
        {
            get => (BlendMode)blendMode;
            set
            {
                if (blendMode != (SDL_BlendMode)value)
                {
                    blendMode = (SDL_BlendMode)value;
                    _ = SDL_SetRenderDrawBlendMode(handle, blendMode);
                }
            }
        }

        internal void BeginPaint()
        {
            Color = Color.Black;
            BlendMode = BlendMode.None;
            //_ = SDL_SetRenderTarget(handle, IntPtr.Zero);
            //_ = SDL_SetRenderDrawColor(handle, 0, 0, 0, 255);
            //_ = SDL_SetRenderDrawBlendMode(handle, SDL_BlendMode.NONE);
            _ = SDL_RenderClear(handle);
        }

        internal void EndPaint()
        {
            //_ = SDL_SetRenderTarget(handle, IntPtr.Zero);
            SDL_RenderPresent(handle);
        }
        public void DrawTexture(SDLTexture? texture)
        {
            if (texture != null)
            {
                _ = SDL_RenderCopyF(handle, texture.Handle, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public void DrawTexture(SDLTexture? texture, RectangleF dst)
        {
            if (texture != null)
            {
                _ = SDL_RenderCopyF(handle, texture.Handle, IntPtr.Zero, ref dst);
            }
        }
        public void DrawTexture(SDLTexture? texture, Rectangle dst)
        {
            if (texture != null)
            {
                _ = SDL_RenderCopy(handle, texture.Handle, IntPtr.Zero, ref dst);
            }
        }

        public void DrawTexture(SDLTexture? texture, Rectangle src, RectangleF dst)
        {
            if (texture != null)
            {
                _ = SDL_RenderCopyF(handle, texture.Handle, ref src, ref dst);
            }
        }
        public void DrawTexture(SDLTexture? texture, Rectangle src, Rectangle dst)
        {
            if (texture != null)
            {
                _ = SDL_RenderCopy(handle, texture.Handle, ref src, ref dst);
            }
        }

        public void DrawTexture(SDLTexture? texture, RenderFlip flip)
        {
            if (texture != null)
            {
                _ = SDL_RenderCopyEx(handle, texture.Handle, IntPtr.Zero, IntPtr.Zero, 0.0, IntPtr.Zero, (SDL_RendererFlip)flip);
            }
        }

        public void DrawPoint(int x, int y)
        {
            _ = SDL_RenderDrawPoint(handle, x, y);
        }
        public void DrawPoint(float x, float y)
        {
            _ = SDL_RenderDrawPointF(handle, x, y);
        }

        public void DrawLine(int x1, int y1, int x2, int y2)
        {
            _ = SDL_RenderDrawLine(handle, x1, y1, x2, y2);
        }
        public void DrawLine(float x1, float y1, float x2, float y2)
        {
            _ = SDL_RenderDrawLineF(handle, x1, y1, x2, y2);
        }

        public void DrawLines(IEnumerable<Point> points)
        {
            Point[] pts = points.AsArray();
            if (pts.Length > 0)
            {
                _ = SDL_RenderDrawLines(handle, pts, pts.Length);
            }
        }

        public void DrawLines(IEnumerable<PointF> points)
        {
            PointF[] pts = points.AsArray();
            if (pts.Length > 0)
            {
                _ = SDL_RenderDrawLinesF(handle, pts, pts.Length);
            }
        }

        public void DrawRect(int x, int y, int w, int h)
        {
            Rectangle rect = new Rectangle(x, y, w, h);
            _ = SDL_RenderDrawRect(handle, ref rect);
        }

        public void DrawRect(float x, float y, float w, float h)
        {
            RectangleF rect = new RectangleF(x, y, w, h);
            _ = SDL_RenderDrawRectF(handle, ref rect);
        }

        public void DrawRect(Rectangle rect)
        {
            _ = SDL_RenderDrawRect(handle, ref rect);
        }
        public void DrawRect(RectangleF rect)
        {
            _ = SDL_RenderDrawRectF(handle, ref rect);
        }
        public void FillRect(int x, int y, int w, int h)
        {
            Rectangle rect = new Rectangle(x, y, w, h);
            _ = SDL_RenderFillRect(handle, ref rect);
        }

        public void FillRect(float x, float y, float w, float h)
        {
            RectangleF rect = new RectangleF(x, y, w, h);
            _ = SDL_RenderFillRectF(handle, ref rect);
        }

        public void FillRect(Rectangle rect)
        {
            _ = SDL_RenderFillRect(handle, ref rect);
        }

        public void FillRect(RectangleF rect)
        {
            _ = SDL_RenderFillRectF(handle, ref rect);
        }

        public void DrawVertices(IEnumerable<Vertex> vertices)
        {
            Vertex[] vs = vertices.AsArray();
            if (vs.Length > 0)
            {
                _ = SDL_RenderGeometry(handle, IntPtr.Zero, vs, vs.Length, IntPtr.Zero, 0);
            }
        }
        public Size MeasureText(SDLFont? font, string? text)
        {
            int w = 0;
            int h = 0;
            if (!string.IsNullOrEmpty(text))
            {
                if (font == null) { font = app.DefaultFont; }
                if (font != null)
                {
                    _ = SDLFont.TTF_SizeUTF8(font.Handle, text, out w, out h);
                }
            }
            return new Size(w, h);
        }
        public void DrawText(SDLFont? font, string? text, float x, float y, float width, float height, Color color, HorizontalAlignment hAlign = HorizontalAlignment.Center, VerticalAlignment vAlign = VerticalAlignment.Center)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (font == null) { font = app.DefaultFont; }
            if (font != null)
            {
                DrawTextCache(GetTextCache(font, text, color), x, y, width, height, hAlign, vAlign);
            }
        }

        private void DrawTextCache(TextCache? textCache, float x, float y, float width, float height, HorizontalAlignment hAlign, VerticalAlignment vAlign)
        {
            if (textCache != null)
            {
                int w = textCache.Width;
                int h = textCache.Height;
                if (width > 0)
                {
                    switch (hAlign)
                    {
                        case HorizontalAlignment.Left:
                            //nop
                            break;
                        case HorizontalAlignment.Right:
                            x = x + width - w;
                            break;
                        case HorizontalAlignment.Center:
                            x = x + width / 2 - w / 2;
                            break;
                    }
                }
                if (height > 0)
                {
                    switch (vAlign)
                    {
                        case VerticalAlignment.Top:
                            // nop
                            break;
                        case VerticalAlignment.Bottom:
                            y = y + height - h;
                            break;
                        case VerticalAlignment.Center:
                            y = y + height / 2 - h / 2;
                            break;
                    }
                }
                RectangleF dstRect = new RectangleF(x, y, w, h);
                BlendMode = BlendMode.Blend;
                _ = SDL_RenderCopyF(handle, textCache.Handle, IntPtr.Zero, ref dstRect);
            }
        }

        private TextCache? CreateTextCache(SDLFont? font, string? text, Color color)
        {
            TextCache? textCache = null;
            if (font != null && !string.IsNullOrEmpty(text))
            {
                IntPtr fontHandle = font.Handle;
                if (fontHandle != IntPtr.Zero)
                {
                    IntPtr surface = SDLFont.TTF_RenderUTF8_Blended(fontHandle, text, color.ToArgb());
                    if (surface != IntPtr.Zero)
                    {
                        IntPtr texHandle = SDL_CreateTextureFromSurface(handle, surface);
                        if (texHandle != IntPtr.Zero)
                        {
                            _ = SDL_QueryTexture(texHandle, out _, out _, out int w, out int h);
                            _ = SDL_SetTextureAlphaMod(texHandle, color.A);
                            textCache = new TextCache(font, text, color, w, h, texHandle);
                        }
                        SDL_FreeSurface(surface);
                    }
                }
            }
            return textCache;
        }
        private void CheckTextCache()
        {
            if (textCache.Count >= textCacheLimit)
            {
                //Logger.Debug($"Text Cache Limit Reached: {textCacheLimit} entries, cleaning up");
                Console.WriteLine($"Renderer {rendererID}: Text Cache Limit Reached {textCacheLimit} entries, cleaning up");
                int len = textCacheKeys.Count / 2;
                var halfKeys = textCacheKeys.GetRange(0, len);
                textCacheKeys.RemoveRange(0, len);
                ClearTextCache(halfKeys);
            }
        }

        private void ClearTextCache(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                if (textCache.TryGetValue(key, out var tc))
                {
                    if (textCache.Remove(key))
                    {
                        SDL_DestroyTexture(tc.Handle);
                    }
                }
            }
        }
        private TextCache? GetTextCache(SDLFont font, string text, Color color)
        {
            CheckTextCache();
            if (textCache.TryGetValue(text, out var tc))
            {
                if (tc.Matches(font, text, color)) return tc;
            }
            tc = CreateTextCache(font, text, color);
            if (tc != null)
            {
                textCache[text] = tc;
                textCacheKeys.Add(text);
            }
            return tc;
        }
        public SDLTexture? LoadTexture(string fileName)
        {
            SDLTexture? texture = null;
            if (fileName != null)
            {
                IntPtr tex = SDLImg.IMG_LoadTexture(handle, fileName);
                if (tex != IntPtr.Zero)
                {
                    texture = new SDLTexture(this, tex);
                }
            }
            return texture;
        }

        public SDLTexture? LoadTexture(byte[] data)
        {
            SDLTexture? texture = null;
            if (data != null)
            {
                IntPtr rw = SDLNative.SDL_RWFromMem(data, data.Length);
                if (rw != IntPtr.Zero)
                {
                    IntPtr tex = SDLImg.IMG_LoadTexture_RW(handle, rw, 1);
                    if (tex != IntPtr.Zero)
                    {
                        texture = new SDLTexture(this, tex);
                    }
                }
            }
            return texture;
        }

        public static SDLFont? LoadFont(string fileName, int ptSize)
        {
            SDLFont? font = null;
            if (fileName != null)
            {
                IntPtr fnt = SDLFont.TTF_OpenFont(fileName, ptSize);
                if (fnt != IntPtr.Zero)
                {
                    font = new SDLFont(fnt, ptSize);
                }
            }
            return font;
        }

        public static SDLFont? LoadFont(byte[] data, int ptSize)
        {
            SDLFont? font = null;
            if (data != null)
            {
                int size = data.Length;
                IntPtr mem = Marshal.AllocHGlobal(size);
                Marshal.Copy(data, 0, mem, size);
                IntPtr rw = SDLNative.SDL_RWFromMem(mem, size);
                IntPtr handle = SDLFont.TTF_OpenFontRW(rw, 1, ptSize);
                if (handle != IntPtr.Zero)
                {
                    font = new SDLFont(handle, ptSize, mem);
                }
            }
            return font;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }
                SDL_DestroyRenderer(handle);
                disposedValue = true;
            }
        }


        ~SDLRenderer()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        [Flags]
        private enum SDL_RendererFlags : uint
        {
            SOFTWARE = 0x00000001,
            ACCELERATED = 0x00000002,
            PRESENTVSYNC = 0x00000004,
            TARGETTEXTURE = 0x00000008
        }

        [Flags]
        internal enum SDL_RendererFlip
        {
            SDL_FLIP_NONE = 0x00000000,
            SDL_FLIP_HORIZONTAL = 0x00000001,
            SDL_FLIP_VERTICAL = 0x00000002
        }

        [Flags]
        internal enum SDL_BlendMode
        {
            NONE = 0x00000000,
            BLEND = 0x00000001,
            ADD = 0x00000002,
            MOD = 0x00000004,
            MUL = 0x00000008,
            INVALID = 0x7FFFFFFF
        }

        internal enum SDL_ScaleMode
        {
            ScaleModeNearest,
            ScaleModeLinear,
            ScaleModeBest
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex
        {
            public PointF Position;
            public byte R;
            public byte G;
            public byte B;
            public byte A;
            public PointF TexCoord;

            public Color Color
            {
                get { return Color.FromArgb(R, G, B, A); }
                set
                {
                    R = value.R;
                    G = value.G;
                    B = value.B;
                    A = value.A;
                }

            }
        }

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateRenderer(IntPtr window, int index, SDL_RendererFlags flags);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_DestroyRenderer(IntPtr renderer);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateTextureFromSurface(IntPtr renderer, IntPtr surface);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_FreeSurface(IntPtr surface);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetRenderDrawBlendMode(IntPtr renderer, out SDL_BlendMode blendMode);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetRenderDrawColor(IntPtr renderer, out byte r, out byte g, out byte b, out byte a);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetRendererOutputSize(IntPtr renderer, out int w, out int h);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderClear(IntPtr renderer);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, [In()] ref Rectangle srcrect, [In()] ref Rectangle dstrect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, IntPtr srcrect, [In()] ref Rectangle dstrect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, [In()] ref Rectangle srcrect, IntPtr dstrect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyF(IntPtr renderer, IntPtr texture, [In()] ref Rectangle srcrect, [In()] ref RectangleF dstrect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyF(IntPtr renderer, IntPtr texture, IntPtr srcrect, [In()] ref RectangleF dstrect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyF(IntPtr renderer, IntPtr texture, [In()] ref Rectangle srcrect, IntPtr dstrect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyF(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref Rectangle srcrect, ref Rectangle dstrect, double angle, ref Point center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref Rectangle dstrect, double angle, ref Point center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref Rectangle srcrect, IntPtr dstrect, double angle, ref Point center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref Rectangle srcrect, ref Rectangle dstrect, double angle, IntPtr center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect, double angle, ref Point center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref Rectangle dstrect, double angle, IntPtr center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref Rectangle srcrect, IntPtr dstrect, double angle, IntPtr center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect, double angle, IntPtr center, SDL_RendererFlip flip);

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, ref RectangleF srcrect, ref RectangleF dstrect, double angle, ref PointF center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref RectangleF dstrect, double angle, ref PointF center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, ref RectangleF srcrect, IntPtr dstrect, double angle, ref PointF center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, ref RectangleF srcrect, ref RectangleF dstrect, double angle, IntPtr center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect, double angle, ref PointF center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref RectangleF dstrect, double angle, IntPtr center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, ref RectangleF srcrect, IntPtr dstrect, double angle, IntPtr center, SDL_RendererFlip flip);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect, double angle, IntPtr center, SDL_RendererFlip flip);

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawLine(IntPtr renderer, int x1, int y1, int x2, int y2);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawLines(IntPtr renderer, [In()] Point[] points, int count);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawLineF(IntPtr renderer, float x1, float y1, float x2, float y2);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawLinesF(IntPtr renderer, [In()] PointF[] points, int count);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawPoint(IntPtr renderer, int x, int y);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawPointF(IntPtr renderer, float x, float y);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawRect(IntPtr renderer, [In()] ref Rectangle rect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawRectF(IntPtr renderer, [In()] ref RectangleF rect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderFillRect(IntPtr renderer, [In()] ref Rectangle rect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderFillRectF(IntPtr renderer, [In()] ref RectangleF rect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_RenderGetClipRect(IntPtr renderer, out Rectangle rect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_RenderGetLogicalSize(IntPtr renderer, out int w, out int h);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_RenderGetScale(IntPtr renderer, out float scaleX, out float scaleY);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_RenderWindowToLogical(IntPtr renderer, int windowX, int windowY, out float logicalX, out float logicalY);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_RenderLogicalToWindow(IntPtr renderer, float logicalX, float logicalY, out int windowX, out int windowY);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderGetViewport(IntPtr renderer, out Rectangle rect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_RenderPresent(IntPtr renderer);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderSetClipRect(IntPtr renderer, [In()] ref Rectangle rect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderSetClipRect(IntPtr renderer, IntPtr rect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderSetLogicalSize(IntPtr renderer, int w, int h);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderSetScale(IntPtr renderer, float scaleX, float scaleY);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderSetIntegerScale(IntPtr renderer, [In()][MarshalAs(UnmanagedType.Bool)] bool enable);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderSetViewport(IntPtr renderer, [In()] ref Rectangle rect);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderSetViewport(IntPtr renderer, IntPtr rect);

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetRenderDrawBlendMode(IntPtr renderer, SDL_BlendMode blendMode);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetRenderDrawColor(IntPtr renderer, byte r, byte g, byte b, byte a);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderSetVSync(IntPtr renderer, int vsync);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SDL_RenderIsClipEnabled(IntPtr renderer);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderFlush(IntPtr renderer);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SDL_RenderTargetSupported(IntPtr renderer);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_GetRenderTarget(IntPtr renderer);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetRenderTarget(IntPtr renderer, IntPtr texture);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderGeometry(IntPtr renderer, IntPtr texture, [In] Vertex[] vertices, int num_vertices, [In] int[] indices, int num_indices);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderGeometry(IntPtr renderer, IntPtr texture, [In] Vertex[] vertices, int num_vertices, IntPtr indices, int num_indices);

        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SDL_CreateTexture(IntPtr renderer, uint format, int access, int w, int h);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SDL_DestroyTexture(IntPtr texture);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureScaleMode(IntPtr texture, SDL_ScaleMode scaleMode);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetTextureScaleMode(IntPtr texture, out SDL_ScaleMode scaleMode);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureUserData(IntPtr texture, IntPtr userdata);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SDL_GetTextureUserData(IntPtr texture);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetTextureAlphaMod(IntPtr texture, out byte alpha);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetTextureBlendMode(IntPtr texture, out SDL_BlendMode blendMode);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetTextureColorMod(IntPtr texture, out byte r, out byte g, out byte b);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_QueryTexture(IntPtr texture, out uint format, out int access, out int w, out int h);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureAlphaMod(IntPtr texture, byte alpha);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureBlendMode(IntPtr texture, SDL_BlendMode blendMode);
        [DllImport(SDLNative.LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureColorMod(IntPtr texture, byte r, byte g, byte b);



    }
}
