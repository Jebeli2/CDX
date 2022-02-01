namespace CDXSDL
{
    using CDX.Content;
    using CDX.Graphics;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    internal class SDLTexture : Resource, IImage
    {
        private readonly SDLRenderer renderer;
        private readonly IntPtr handle;
        private readonly int width;
        private readonly int height;
        private readonly uint format;
        private readonly int access;
        private TextureFilter textureFilter;
        private byte alphaMod;
        private Color colorMod;

        internal SDLTexture(ContentFlags flags, SDLRenderer renderer, IntPtr handle, string name)
            : base(name, flags | ContentFlags.Image)
        {
            this.renderer = renderer;
            this.handle = handle;
            _ = SDLRenderer.SDL_QueryTexture(this.handle, out format, out access, out width, out height);
            _ = SDLRenderer.SDL_GetTextureScaleMode(this.handle, out textureFilter);
            _ = SDLRenderer.SDL_GetTextureAlphaMod(this.handle, out alphaMod);
            _ = SDLRenderer.SDL_GetTextureColorMod(this.handle, out byte r, out byte g, out byte b);
            colorMod = Color.FromArgb(r, g, b);
        }

        public IntPtr Handle => handle;

        public int Width => width;
        public int Height => height;

        public TextureFilter TextureFilter
        {
            get => textureFilter;
            set
            {
                if (textureFilter != value)
                {
                    //textureFilter = value;
                    _ = SDLRenderer.SDL_SetTextureScaleMode(handle, value);
                    _ = SDLRenderer.SDL_GetTextureScaleMode(handle, out textureFilter);
                }
            }
        }

        public byte AlphaMod
        {
            get => alphaMod;
            set
            {
                if (alphaMod != value)
                {
                    alphaMod = value;
                    _ = SDLRenderer.SDL_SetTextureAlphaMod(handle, alphaMod);
                    if (alphaMod == 255)
                    {
                        SDLRenderer.SDL_SetTextureBlendMode(handle, SDLRenderer.SDL_BlendMode.NONE);
                    }
                    else
                    {
                        SDLRenderer.SDL_SetTextureBlendMode(handle, SDLRenderer.SDL_BlendMode.BLEND);
                    }
                }
            }
        }

        public Color ColorMod
        {
            get => colorMod;
            set
            {
                if (colorMod != value)
                {
                    colorMod = value;
                    _ = SDLRenderer.SDL_SetTextureColorMod(handle, colorMod.R, colorMod.G, colorMod.B);
                }
            }
        }

        protected override void DisposeUnmanaged()
        {
            SDLRenderer.SDL_DestroyTexture(handle);
        }

        private const string LibName = "SDL2_image";
        [Flags]
        internal enum IMG_InitFlags
        {
            IMG_INIT_JPG = 0x00000001,
            IMG_INIT_PNG = 0x00000002,
            IMG_INIT_TIF = 0x00000004,
            IMG_INIT_WEBP = 0x00000008
        }

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int IMG_Init(IMG_InitFlags flags);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void IMG_Quit();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr IMG_LoadTexture(IntPtr renderer, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string fileName);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr IMG_LoadTexture_RW(IntPtr renderer, IntPtr src, int freesrc);
    }
}
