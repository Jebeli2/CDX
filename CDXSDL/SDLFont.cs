namespace CDXSDL
{
    using CDX.Content;
    using CDX.Graphics;
    using CDX.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    internal class SDLFont : Resource, ITextFont
    {
        private const string LibName = "SDL2_ttf";

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

        internal SDLFont(ContentFlags flags, IntPtr handle, int ySize, string name)
            : this(flags, handle, ySize, name, IntPtr.Zero)
        {

        }
        internal SDLFont(ContentFlags flags, IntPtr handle, int ySize, string name, IntPtr mem)
            : base(name, flags | ContentFlags.Font)
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
            familyName = SDLApplication.IntPtr2String(TTF_FontFaceFamilyName(this.handle)) ?? "";
            styleName = SDLApplication.IntPtr2String(TTF_FontFaceStyleName(this.handle)) ?? "";
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

        public static SDLFont? LoadFont(string fileName, int ptSize)
        {
            SDLFont? font = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                IntPtr fnt = TTF_OpenFont(fileName, ptSize);
                if (fnt != IntPtr.Zero)
                {
                    font = new SDLFont(ContentFlags.File, fnt, ptSize, fileName);
                    Logger.Info($"Font loaded from file '{fileName}'");
                }
            }
            return font;
        }

        public static SDLFont? LoadFont(byte[] data, string name, int ptSize)
        {
            SDLFont? font = null;
            if (data != null)
            {
                int size = data.Length;
                IntPtr mem = Marshal.AllocHGlobal(size);
                Marshal.Copy(data, 0, mem, size);
                IntPtr rw = SDLApplication.SDL_RWFromMem(mem, size);
                IntPtr handle = TTF_OpenFontRW(rw, 1, ptSize);
                if (handle != IntPtr.Zero)
                {
                    font = new SDLFont(ContentFlags.Resource, handle, ptSize, name, mem);
                    Logger.Info($"Font loaded from resource '{name}'");
                }
            }
            return font;
        }

        protected override void DisposeUnmanaged()
        {
            if (handle != IntPtr.Zero)
            {
                TTF_CloseFont(handle);
            }
            if (mem != IntPtr.Zero)
            {

            }
        }

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TTF_Init();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TTF_Quit();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TTF_WasInit();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TTF_OpenFont([In()][MarshalAs(UnmanagedType.LPUTF8Str)] string file, int ptsize);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TTF_OpenFontRW(IntPtr src, int freesrc, int ptsize);


        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void TTF_CloseFont(IntPtr font);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr TTF_FontFaceFamilyName(IntPtr font);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr TTF_FontFaceStyleName(IntPtr font);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_FontFaceIsFixedWidth(IntPtr font);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TTF_SetFontKerning(IntPtr font, int allowed);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_GetFontKerning(IntPtr font);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_FontLineSkip(IntPtr font);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_FontDescent(IntPtr font);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_FontAscent(IntPtr font);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_FontHeight(IntPtr font);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_GetFontHinting(IntPtr font);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TTF_SetFontHinting(IntPtr font, int hinting);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_GetFontOutline(IntPtr font);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TTF_SetFontOutline(IntPtr font, int outline);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_GetFontStyle(IntPtr font);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TTF_SetFontStyle(IntPtr font, int style);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_SetFontSize(IntPtr font, int ptsize);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TTF_RenderUTF8_Solid(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, int fg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TTF_RenderUTF8_Shaded(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, int fg, int bg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TTF_RenderUTF8_Blended(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, int fg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TTF_MeasureUTF8(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, int measure_width, out int extent, out int count);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int TTF_SizeUTF8(IntPtr font, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string text, out int w, out int h);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr TTF_RenderGlyph_Blended(IntPtr font, ushort ch, int fg);
    }

}
