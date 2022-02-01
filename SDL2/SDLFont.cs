namespace SDL2
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLFont : IDisposable
    {
        private const string nativeLibName = "SDL2_ttf";
        private bool disposedValue;

        private readonly IntPtr handle;
        private readonly IntPtr mem;
        private readonly int ySize;

        private FontStyle fontStyle;
        private int fontOutline;
        private FontHinting fontHinting;
        private int fontHeight;
        private int fontAscent;
        private int fontDescent;
        private int fontLineSkip;
        private int fontKerning;
        private string familyName;
        private string styleName;

        internal SDLFont(IntPtr handle, int ySize)
            : this(handle, ySize, IntPtr.Zero)
        {

        }
        internal SDLFont(IntPtr handle, int ySize, IntPtr mem)
        {
            this.handle = handle;
            this.mem = mem;
            this.ySize = ySize;
            fontStyle = (FontStyle)TTF_GetFontStyle(this.handle);
            fontOutline = TTF_GetFontOutline(this.handle);
            fontHinting = (FontHinting)TTF_GetFontHinting(this.handle);
            fontHeight = TTF_FontHeight(this.handle);
            fontAscent = TTF_FontAscent(this.handle);
            fontDescent = TTF_FontDescent(this.handle);
            fontLineSkip = TTF_FontLineSkip(this.handle);
            fontKerning = TTF_GetFontKerning(this.handle);
            familyName = SDLNative.IntPtr2String(TTF_FontFaceFamilyName(this.handle)) ?? "";
            styleName = SDLNative.IntPtr2String(TTF_FontFaceStyleName(this.handle)) ?? "";
        }

        public IntPtr Handle => handle;
        public int YSize => ySize;
        public string FamilyName => familyName;
        public string StyleName => styleName;
        public FontStyle FontStyle => fontStyle;
        public int Outline => fontOutline;
        public FontHinting Hinting => fontHinting;
        public int Height => fontHeight;
        public int Ascent => fontAscent;
        public int Descent => fontDescent;
        public int LineSkip => fontLineSkip;
        public int Kerning => fontKerning;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                if (handle != IntPtr.Zero)
                {
                    TTF_CloseFont(handle);
                }
                if (mem != IntPtr.Zero)
                {

                }
                disposedValue = true;
            }
        }

        ~SDLFont()
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

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TTF_Init();

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TTF_Quit();

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TTF_WasInit();

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TTF_OpenFont([In()][MarshalAs(UnmanagedType.LPUTF8Str)] string file, int ptsize);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TTF_OpenFontRW(IntPtr src, int freesrc, int ptsize);


        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TTF_CloseFont(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr TTF_FontFaceFamilyName(IntPtr font);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr TTF_FontFaceStyleName(IntPtr font);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_FontFaceIsFixedWidth(IntPtr font);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TTF_SetFontKerning(IntPtr font, int allowed);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_GetFontKerning(IntPtr font);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_FontLineSkip(IntPtr font);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_FontDescent(IntPtr font);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_FontAscent(IntPtr font);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_FontHeight(IntPtr font);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_GetFontHinting(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TTF_SetFontHinting(IntPtr font, int hinting);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_GetFontOutline(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TTF_SetFontOutline(IntPtr font, int outline);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_GetFontStyle(IntPtr font);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TTF_SetFontStyle(IntPtr font, int style);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_SetFontSize(IntPtr font, int ptsize);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TTF_RenderUTF8_Solid(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, int fg);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TTF_RenderUTF8_Shaded(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, int fg, int bg);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TTF_RenderUTF8_Blended(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, int fg);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TTF_MeasureUTF8(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, int measure_width, out int extent, out int count);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TTF_SizeUTF8(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, out int w, out int h);
    }
}
