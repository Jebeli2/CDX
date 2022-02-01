namespace SDL2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    internal static class SDLImg
    {
        private const string nativeLibName = "SDL2_image";
        [Flags]
        internal enum IMG_InitFlags
        {
            IMG_INIT_JPG = 0x00000001,
            IMG_INIT_PNG = 0x00000002,
            IMG_INIT_TIF = 0x00000004,
            IMG_INIT_WEBP = 0x00000008
        }

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int IMG_Init(IMG_InitFlags flags);

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void IMG_Quit();

        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr IMG_LoadTexture(IntPtr renderer, [In()][MarshalAs(UnmanagedType.LPUTF8Str)] string fileName);
        [DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr IMG_LoadTexture_RW(IntPtr renderer, IntPtr src, int freesrc);
    }
}
