namespace CDXSDL
{
    using CDX.Audio;
    using CDX.Content;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class SDLMusic : Resource, IMusic
    {
        private readonly IntPtr handle;
        private readonly SDLAudio.Mix_MusicType musicType;
        private readonly string? tempFile;

        public SDLMusic(ContentFlags flags, IntPtr handle, string name, string? tempFile = null)
            : base(name, flags | ContentFlags.Music)
        {
            this.handle = handle;
            this.tempFile = tempFile;
            musicType = SDLAudio.Mix_GetMusicType(this.handle);
        }

        public IntPtr Handle => handle;

        public MusicType MusicType => (MusicType)musicType;

        protected override void DisposeUnmanaged()
        {
            SDLAudio.Mix_FreeMusic(handle);
            if (tempFile != null)
            {
                File.Delete(tempFile);
            }
        }
    }
}
