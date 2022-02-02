namespace CDXSDL
{
    using CDX;
    using CDX.App;
    using CDX.Graphics;
    using CDX.Logging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    internal class SDLRenderer : IGraphics, IDisposable
    {
        private bool disposedValue;
        private readonly SDLWindow window;
        private readonly SDLApplication app;
        private IntPtr handle;
        private SDL_BlendMode blendMode;
        private TextureFilter textureFilter = TextureFilter.Nearest;
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
        private readonly Dictionary<Icons, IconCache> iconCache = new();
        private readonly List<Icons> iconCacheKeys = new();
        private int iconCacheLimit = 100;

        private PaintEventArgs? paintEventArgs;

        public SDLRenderer(SDLWindow window)
        {
            this.window = window;
            app = this.window.App;
            handle = CreateHandle(this.window);

        }

        private IntPtr CreateHandle(SDLWindow window)
        {
            int driverIndex = app.GetDriverIndex(window.Driver);
            SDL_RendererFlags flags = SDL_RendererFlags.ACCELERATED;
            if (window.VSync) { flags |= SDL_RendererFlags.PRESENTVSYNC; }
            IntPtr handle = SDL_CreateRenderer(window.Handle, driverIndex, flags);
            if (handle != IntPtr.Zero)
            {
                _ = SDL_GetRenderDrawBlendMode(handle, out blendMode);
                _ = SDL_GetRenderDrawColor(handle, out colorR, out colorG, out colorB, out colorA);
                _ = SDL_GetRendererOutputSize(handle, out width, out height);
                _ = SDL_RenderGetViewport(handle, out viewPort);
                _ = SDL_GetRendererInfo(handle, out SDL_RendererInfo info);
                Logger.Info($"SDLRenderer {window.WindowID} created: {Marshal.PtrToStringUTF8(info.name)} ({info.max_texture_width}x{info.max_texture_height} max texture size)");
            }
            return handle;
        }

        public PaintEventArgs GetPaintEventArgs(FrameTime time)
        {
            if (paintEventArgs == null)
            {
                paintEventArgs = new PaintEventArgs(this, time);
            }
            return paintEventArgs;
        }

        public int Width => width;
        public int Height => height;
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

        public TextureFilter TextureFilter
        {
            get => textureFilter;
            set => textureFilter = value;
        }
        internal void UpdateSize()
        {
            _ = SDL_GetRendererOutputSize(handle, out width, out height);
            _ = SDL_RenderGetViewport(handle, out viewPort);
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

        public void DrawPoint(Point p)
        {
            _ = SDL_RenderDrawPoint(handle, p.X, p.Y);
        }
        public void DrawPoint(PointF p)
        {
            _ = SDL_RenderDrawPointF(handle, p.X, p.Y);
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
            Rectangle rect = new(x, y, w, h);
            _ = SDL_RenderDrawRect(handle, ref rect);
        }

        public void DrawRect(float x, float y, float w, float h)
        {
            RectangleF rect = new(x, y, w, h);
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
            Rectangle rect = new(x, y, w, h);
            _ = SDL_RenderFillRect(handle, ref rect);
        }

        public void FillRect(float x, float y, float w, float h)
        {
            RectangleF rect = new(x, y, w, h);
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

        public void DrawImage(IImage? image)
        {
            if (image is SDLTexture texture)
            {
                _ = SDL_RenderCopyF(handle, texture.Handle, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public void DrawImage(IImage? image, RectangleF dst)
        {
            if (image is SDLTexture texture)
            {
                _ = SDL_RenderCopyF(handle, texture.Handle, IntPtr.Zero, ref dst);
            }
        }
        public void DrawImage(IImage? image, Rectangle dst)
        {
            if (image is SDLTexture texture)
            {
                _ = SDL_RenderCopy(handle, texture.Handle, IntPtr.Zero, ref dst);
            }
        }

        public void DrawImage(IImage? image, Rectangle src, RectangleF dst)
        {
            if (image is SDLTexture texture)
            {
                _ = SDL_RenderCopyF(handle, texture.Handle, ref src, ref dst);
            }
        }
        public void DrawImage(IImage? image, Rectangle src, Rectangle dst)
        {
            if (image is SDLTexture texture)
            {
                _ = SDL_RenderCopy(handle, texture.Handle, ref src, ref dst);
            }
        }

        public void DrawImage(IImage? image, RenderFlip flip)
        {
            if (image is SDLTexture texture)
            {
                _ = SDL_RenderCopyEx(handle, texture.Handle, IntPtr.Zero, IntPtr.Zero, 0.0, IntPtr.Zero, (SDL_RendererFlip)flip);
            }
        }

        public void DrawText(ITextFont? font, string? text, float x, float y, float width, float height, Color color, HorizontalAlignment hAlign = HorizontalAlignment.Center, VerticalAlignment vAlign = VerticalAlignment.Center)
        {
            if (!string.IsNullOrEmpty(text))
            {
                if (font is SDLFont sdlFont)
                {
                    DrawTextCache(GetTextCache(sdlFont, text, color), x, y, width, height, hAlign, vAlign);
                }
                else if (app.DefaultFont != null)
                {
                    DrawTextCache(GetTextCache(app.DefaultFont, text, color), x, y, width, height, hAlign, vAlign);
                }
            }
        }

        public void DrawIcon(Icons icon, float x, float y, float width, float height, Color color, HorizontalAlignment hAlign = HorizontalAlignment.Center, VerticalAlignment vAlign = VerticalAlignment.Center)
        {
            if (icon != Icons.NONE && app.IconFont != null)
            {
                DrawIconCache(GetIconCache(app.IconFont, icon, color), x, y, width, height, hAlign, vAlign);
            }
        }

        public void ClearScreen(Color color)
        {
            SDL_SetRenderDrawColor(handle, color.R, color.G, color.B, color.A);
            SDL_RenderClear(handle);
        }

        public void SetTarget(IImage? image)
        {
            if (image != null && image is SDLTexture tex)
            {
                SDL_SetRenderTarget(handle, tex.Handle);
                SDL_SetRenderDrawBlendMode(handle, blendMode);
            }
            else
            {
                ClearTarget();
            }
        }
        public void ClearTarget()
        {
            SDL_SetRenderTarget(handle, IntPtr.Zero);
            SDL_SetRenderDrawBlendMode(handle, blendMode);
        }

        public IImage? CreateImage(string name, int width, int height)
        {
            SDLTexture? texture = null;
            if (!string.IsNullOrEmpty(name))
            {
                _ = SDLApplication.SDL_SetHint(SDLApplication.SDL_HINT_RENDER_SCALE_QUALITY, ((int)textureFilter).ToString());
                IntPtr tex = SDL_CreateTexture(handle, SDL_PIXELFORMAT_ARGB8888, SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, width, height);
                if (tex != IntPtr.Zero)
                {
                    texture = new SDLTexture(CDX.Content.ContentFlags.Created, this, tex, name);
                    
                    Logger.Info($"Image created from scratch '{name}'");
                }
            }
            return texture;
        }


        public IImage? LoadImage(string name)
        {
            byte[]? data = app.Content.FindResource(name);
            if (data != null)
            {
                return LoadImage(data, name);
            }
            SDLTexture? texture = null;
            if (!string.IsNullOrEmpty(name))
            {
                _ = SDLApplication.SDL_SetHint(SDLApplication.SDL_HINT_RENDER_SCALE_QUALITY, ((int)textureFilter).ToString());
                IntPtr tex = SDLTexture.IMG_LoadTexture(handle, name);
                if (tex != IntPtr.Zero)
                {
                    texture = new SDLTexture(CDX.Content.ContentFlags.File, this, tex, name);
                    Logger.Info($"Image loaded from file '{name}'");
                }
            }
            return texture;
        }
        public IImage? LoadImage(byte[] data, string name)
        {
            SDLTexture? texture = null;
            if (data != null)
            {
                IntPtr rw = SDLApplication.SDL_RWFromMem(data, data.Length);
                if (rw != IntPtr.Zero)
                {
                    _ = SDLApplication.SDL_SetHint(SDLApplication.SDL_HINT_RENDER_SCALE_QUALITY, ((int)textureFilter).ToString());
                    IntPtr tex = SDLTexture.IMG_LoadTexture_RW(handle, rw, 1);
                    if (tex != IntPtr.Zero)
                    {
                        texture = new SDLTexture(CDX.Content.ContentFlags.Resource, this, tex, name);
                        Logger.Info($"Image loaded from resource '{name}'");
                    }
                }
            }
            return texture;
        }

        public ITextFont? LoadFont(string name, int ptSize)
        {
            byte[]? data = app.Content.FindResource(name);
            if (data != null)
            {
                return SDLFont.LoadFont(data, name, ptSize);
            }
            return SDLFont.LoadFont(name, ptSize);
        }

        public ITextFont? LoadFont(byte[] data, string name, int ptSize)
        {
            return SDLFont.LoadFont(data, name, ptSize);
        }


        private void DrawTextCache(TextCache? textCache, float x, float y, float width, float height, HorizontalAlignment hAlign, VerticalAlignment vAlign)
        {
            if (textCache != null)
            {
                int w = textCache.Width;
                int h = textCache.Height;
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
                RectangleF dstRect = new RectangleF(x, y, w, h);
                BlendMode = BlendMode.Blend;
                _ = SDL_RenderCopyF(handle, textCache.Handle, IntPtr.Zero, ref dstRect);
            }
        }

        private void DrawIconCache(IconCache? iconCache, float x, float y, float width, float height, HorizontalAlignment hAlign = HorizontalAlignment.Center, VerticalAlignment vAlign = VerticalAlignment.Center)
        {
            if (iconCache != null)
            {
                int w = iconCache.Width;
                int h = iconCache.Height;
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
                RectangleF dstRect = new RectangleF(x, y, w, h);
                BlendMode = BlendMode.Blend;
                _ = SDL_RenderCopyF(handle, iconCache.Handle, IntPtr.Zero, ref dstRect);
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

        private IconCache? CreateIconCache(SDLFont? font, Icons icon, Color color)
        {
            IconCache? iconCache = null;
            if (font != null)
            {
                IntPtr fontHandle = font.Handle;
                if (fontHandle != IntPtr.Zero)
                {
                    IntPtr surface = SDLFont.TTF_RenderGlyph_Blended(fontHandle, (ushort)icon, color.ToArgb());
                    if (surface != IntPtr.Zero)
                    {
                        IntPtr texture = SDL_CreateTextureFromSurface(handle, surface);
                        if (texture != IntPtr.Zero)
                        {
                            if (texture != IntPtr.Zero)
                            {
                                _ = SDL_QueryTexture(texture, out _, out _, out int w, out int h);
                                _ = SDL_SetTextureAlphaMod(texture, color.A);
                                iconCache = new IconCache()
                                {
                                    Icon = icon,
                                    Color = color,
                                    Width = w,
                                    Height = h,
                                    Handle = texture
                                };
                            }
                        }
                        SDL_FreeSurface(surface);
                    }
                }
            }
            return iconCache;
        }
        private void CheckTextCache()
        {
            if (textCache.Count >= textCacheLimit)
            {
                int len = textCacheKeys.Count / 2;
                var halfKeys = textCacheKeys.GetRange(0, len);
                textCacheKeys.RemoveRange(0, len);
                Logger.Verbose($"Text cache limit {textCacheLimit} reached. Cleaning up...");
                ClearTextCache(halfKeys);
            }
        }

        private void CheckIconCache()
        {
            if (iconCache.Count > iconCacheLimit)
            {
                int len = iconCacheKeys.Count / 2;
                var halfKeys = iconCacheKeys.GetRange(0, len);
                iconCacheKeys.RemoveRange(0, len);
                Logger.Verbose($"Icon cache limit {textCacheLimit} reached. Cleaning up...");
                ClearIconCache(halfKeys);
            }
        }
        private void ClearTextCache()
        {
            foreach (var kvp in textCache)
            {
                TextCache tc = kvp.Value;
                SDL_DestroyTexture(tc.Handle);
            }
            textCache.Clear();
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
        private void ClearIconCache()
        {
            foreach (var kvp in iconCache)
            {
                IconCache tc = kvp.Value;
                SDL_DestroyTexture(tc.Handle);
            }
            iconCache.Clear();
        }

        private void ClearIconCache(IEnumerable<Icons> keys)
        {
            foreach (var key in keys)
            {
                if (iconCache.TryGetValue(key, out var tc))
                {
                    if (iconCache.Remove(key))
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

        private IconCache? GetIconCache(SDLFont font, Icons icon, Color color)
        {
            CheckIconCache();
            if (iconCache.TryGetValue(icon, out var ic))
            {
                if (ic.Matches(icon, color)) { return ic; }
            }
            ic = CreateIconCache(font, icon, color);
            if (ic != null)
            {
                iconCache[icon] = ic;
                iconCacheKeys.Add(icon);
            }
            return ic;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                if (handle != IntPtr.Zero)
                {
                    ClearTextCache();
                    ClearIconCache();
                    SDL_DestroyRenderer(handle);
                    handle = IntPtr.Zero;
                    Logger.Info($"SDL Renderer {window.WindowID} destroyed");
                }
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

        private class TextCache
        {
            internal TextCache(SDLFont font, string text, Color color, int width, int height, IntPtr handle)
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

        private class IconCache
        {
            public Icons Icon;
            public Color Color;
            public int Width;
            public int Height;
            public IntPtr Handle;

            public bool Matches(Icons icon, Color color)
            {
                return (icon == Icon) && (color == Color);
            }
        }

        private enum SDL_PixelType
        {
            SDL_PIXELTYPE_UNKNOWN,
            SDL_PIXELTYPE_INDEX1,
            SDL_PIXELTYPE_INDEX4,
            SDL_PIXELTYPE_INDEX8,
            SDL_PIXELTYPE_PACKED8,
            SDL_PIXELTYPE_PACKED16,
            SDL_PIXELTYPE_PACKED32,
            SDL_PIXELTYPE_ARRAYU8,
            SDL_PIXELTYPE_ARRAYU16,
            SDL_PIXELTYPE_ARRAYU32,
            SDL_PIXELTYPE_ARRAYF16,
            SDL_PIXELTYPE_ARRAYF32
        }

        private enum SDL_PackedLayout
        {
            SDL_PACKEDLAYOUT_NONE,
            SDL_PACKEDLAYOUT_332,
            SDL_PACKEDLAYOUT_4444,
            SDL_PACKEDLAYOUT_1555,
            SDL_PACKEDLAYOUT_5551,
            SDL_PACKEDLAYOUT_565,
            SDL_PACKEDLAYOUT_8888,
            SDL_PACKEDLAYOUT_2101010,
            SDL_PACKEDLAYOUT_1010102
        }

        private enum SDL_PackedOrder
        {
            SDL_PACKEDORDER_NONE,
            SDL_PACKEDORDER_XRGB,
            SDL_PACKEDORDER_RGBX,
            SDL_PACKEDORDER_ARGB,
            SDL_PACKEDORDER_RGBA,
            SDL_PACKEDORDER_XBGR,
            SDL_PACKEDORDER_BGRX,
            SDL_PACKEDORDER_ABGR,
            SDL_PACKEDORDER_BGRA
        }
        private static uint SDL_DEFINE_PIXELFORMAT(SDL_PixelType type, uint order, SDL_PackedLayout layout, byte bits, byte bytes)
        {
            return (uint)((1 << 28) |
                   (((byte)type) << 24) |
                   (((byte)order) << 20) |
                   (((byte)layout) << 16) |
                   (bits << 8) |
                   (bytes)
             );
        }

        public static readonly uint SDL_PIXELFORMAT_ARGB8888 = SDL_DEFINE_PIXELFORMAT(SDL_PixelType.SDL_PIXELTYPE_PACKED32, (uint)SDL_PackedOrder.SDL_PACKEDORDER_ARGB, SDL_PackedLayout.SDL_PACKEDLAYOUT_8888, 32, 4);

        private enum SDL_TextureAccess
        {
            SDL_TEXTUREACCESS_STATIC,
            SDL_TEXTUREACCESS_STREAMING,
            SDL_TEXTUREACCESS_TARGET
        }

        [Flags]
        internal enum SDL_RendererFlags : uint
        {
            SOFTWARE = 0x00000001,
            ACCELERATED = 0x00000002,
            PRESENTVSYNC = 0x00000004,
            TARGETTEXTURE = 0x00000008
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

        [Flags]
        private enum SDL_RendererFlip
        {
            SDL_FLIP_NONE = 0x00000000,
            SDL_FLIP_HORIZONTAL = 0x00000001,
            SDL_FLIP_VERTICAL = 0x00000002
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SDL_RendererInfo
        {
            //public IntPtr name; // const char*
            //[MarshalAs(UnmanagedType.LPUTF8Str)]
            public IntPtr name;
            public SDL_RendererFlags flags;
            public uint num_texture_formats;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public uint[] texture_formats;
            public int max_texture_width;
            public int max_texture_height;
        }

        private const string LibName = "SDL2";

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateRenderer(IntPtr window, int index, SDL_RendererFlags flags);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_DestroyRenderer(IntPtr renderer);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateTextureFromSurface(IntPtr renderer, IntPtr surface);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_FreeSurface(IntPtr surface);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetRenderDrawBlendMode(IntPtr renderer, out SDL_BlendMode blendMode);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetRenderDrawColor(IntPtr renderer, out byte r, out byte g, out byte b, out byte a);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetRendererOutputSize(IntPtr renderer, out int w, out int h);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderClear(IntPtr renderer);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderGetViewport(IntPtr renderer, out Rectangle rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_RenderPresent(IntPtr renderer);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetRenderDrawBlendMode(IntPtr renderer, SDL_BlendMode blendMode);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetRenderDrawColor(IntPtr renderer, byte r, byte g, byte b, byte a);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderSetViewport(IntPtr renderer, [In()] ref Rectangle rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderSetViewport(IntPtr renderer, IntPtr rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawLine(IntPtr renderer, int x1, int y1, int x2, int y2);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawLines(IntPtr renderer, [In()] Point[] points, int count);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawLineF(IntPtr renderer, float x1, float y1, float x2, float y2);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawLinesF(IntPtr renderer, [In()] PointF[] points, int count);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawPoint(IntPtr renderer, int x, int y);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawPointF(IntPtr renderer, float x, float y);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawRect(IntPtr renderer, [In()] ref Rectangle rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderDrawRectF(IntPtr renderer, [In()] ref RectangleF rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderFillRect(IntPtr renderer, [In()] ref Rectangle rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderFillRectF(IntPtr renderer, [In()] ref RectangleF rect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateTexture(IntPtr renderer, uint format, SDL_TextureAccess access, int w, int h);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SDL_DestroyTexture(IntPtr texture);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_QueryTexture(IntPtr texture, out uint format, out int access, out int w, out int h);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetRenderTarget(IntPtr renderer, IntPtr texture);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, [In()] ref Rectangle srcrect, [In()] ref Rectangle dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, IntPtr srcrect, [In()] ref Rectangle dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, [In()] ref Rectangle srcrect, IntPtr dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyF(IntPtr renderer, IntPtr texture, [In()] ref Rectangle srcrect, [In()] ref RectangleF dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyF(IntPtr renderer, IntPtr texture, IntPtr srcrect, [In()] ref RectangleF dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyF(IntPtr renderer, IntPtr texture, [In()] ref Rectangle srcrect, IntPtr dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyF(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref Rectangle srcrect, ref Rectangle dstrect, double angle, ref Point center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref Rectangle dstrect, double angle, ref Point center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref Rectangle srcrect, IntPtr dstrect, double angle, ref Point center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref Rectangle srcrect, ref Rectangle dstrect, double angle, IntPtr center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect, double angle, ref Point center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref Rectangle dstrect, double angle, IntPtr center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref Rectangle srcrect, IntPtr dstrect, double angle, IntPtr center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect, double angle, IntPtr center, SDL_RendererFlip flip);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, ref RectangleF srcrect, ref RectangleF dstrect, double angle, ref PointF center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref RectangleF dstrect, double angle, ref PointF center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, ref RectangleF srcrect, IntPtr dstrect, double angle, ref PointF center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, ref RectangleF srcrect, ref RectangleF dstrect, double angle, IntPtr center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect, double angle, ref PointF center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref RectangleF dstrect, double angle, IntPtr center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, ref RectangleF srcrect, IntPtr dstrect, double angle, IntPtr center, SDL_RendererFlip flip);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyExF(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect, double angle, IntPtr center, SDL_RendererFlip flip);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureAlphaMod(IntPtr texture, byte alpha);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureBlendMode(IntPtr texture, SDL_BlendMode blendMode);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureColorMod(IntPtr texture, byte r, byte g, byte b);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_SetTextureScaleMode(IntPtr texture, [In()] TextureFilter scaleMode);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetTextureScaleMode(IntPtr texture, out TextureFilter scaleMode);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetTextureAlphaMod(IntPtr texture, out byte alpha);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetTextureColorMod(IntPtr texture, out byte r, out byte g, out byte b);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetRendererInfo(IntPtr renderer, out SDL_RendererInfo info);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int SDL_GetRenderDriverInfo(int index, out SDL_RendererInfo info);

    }
}
