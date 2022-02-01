namespace SDL2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public class SDLTexture : IDisposable
    {
        private bool disposedValue;
        private readonly SDLRenderer renderer;
        private readonly IntPtr handle;
        private readonly int width;
        private readonly int height;
        private readonly uint format;
        private readonly int access;

        internal SDLTexture(SDLRenderer renderer, IntPtr handle)
        {
            this.renderer = renderer;
            this.handle = handle;
            _ = SDLRenderer.SDL_QueryTexture(this.handle, out format, out access, out width, out height);
        }

        public IntPtr Handle => handle;

        public int Width => width;
        public int Heigt => height;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                SDLRenderer.SDL_DestroyTexture(handle);
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~SDLTexture()
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
