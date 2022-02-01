namespace CDXSDL
{
    using CDX;
    using CDX.App;
    using CDX.Audio;
    using CDX.Logging;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    internal class SDLAudio : BaseAudio, IAudio
    {
        private bool disposedValue;
        private readonly MixFuncDelegate mixFunc;
        private readonly MusicFinishedDelegate musicFinished;
        private readonly SDLApplication app;
        private SDLMusic? currentMusic;
        private SDLMusic? lastFinishedMusic;

        internal SDLAudio(SDLApplication app)
            : base(app)
        {
            this.app = app;
            UseTmpFilesForMusic = true;
            MusicVolume = MIX_MAX_VOLUME;
            SoundVolume = MIX_MAX_VOLUME;
            mixFunc = MixPostMix;
            musicFinished = MixMusicFinished;
            OpenAudio();
        }

        public override IMusic? CurrentMusic => currentMusic;
        protected override void SetMusicVolume(int volume)
        {
            _ = Mix_VolumeMusic(volume);
        }

        protected override void SetSoundVolume(int volume)
        {

        }
        private void OpenAudio()
        {
            //if (Mix_OpenAudio(22050, MIX_DEFAULT_FORMAT, 2, 2048) != 0)
            if (Mix_OpenAudio(22050, MIX_DEFAULT_FORMAT, 2, 1024) != 0)
            {
                Logger.Error($"Couldn't open Audio");
            }
            else
            {
                _ = Mix_AllocateChannels(128);
                Logger.Info($"Audio created.");
                Mix_HookMusicFinished(musicFinished);
                Mix_SetPostMix(mixFunc, IntPtr.Zero);
            }
        }

        private void CloseAudio()
        {
            _ = Mix_HaltMusic();
            Mix_SetPostMix(IntPtr.Zero, IntPtr.Zero);
            Mix_HookMusicFinished(IntPtr.Zero);
            Mix_CloseAudio();
            Logger.Info($"Audio closed");
        }

        public override IMusic? LoadMusic(string name)
        {
            byte[]? data = app.Content.FindResource(name);
            if (data != null)
            {
                return LoadMusic(data, name);

            }
            SDLMusic? music = null;
            if (File.Exists(name))
            {
                IntPtr handle = Mix_LoadMUS(name);
                if (handle != IntPtr.Zero)
                {
                    music = new SDLMusic(CDX.Content.ContentFlags.File, handle, name);
                    Logger.Info($"Music loaded from file '{name}'");
                }
            }
            return music;
        }

        public override IMusic? LoadMusic(byte[]? data, string name)
        {
            SDLMusic? music = null;
            if (data != null)
            {
                music = InternalLoadMusic(data, name);
            }
            return music;
        }

        private SDLMusic? InternalLoadMusic(byte[] data, string name)
        {
            SDLMusic? music = null;
            if (UseTmpFilesForMusic)
            {
                string path = Path.GetTempPath();
                string fileName = Path.Combine(path, Path.GetFileName(name));
                File.WriteAllBytes(fileName, data);
                IntPtr handle = Mix_LoadMUS(fileName);
                if (handle != IntPtr.Zero)
                {
                    music = new SDLMusic(CDX.Content.ContentFlags.File, handle, name);
                    Logger.Info($"Music loaded from resource '{name}' (via temporary file '{fileName}')");
                }
            }
            else
            {
                IntPtr rw = SDLApplication.SDL_RWFromMem(data, data.Length);
                IntPtr handle = Mix_LoadMUS_RW(rw, 1);
                if (handle != IntPtr.Zero)
                {
                    music = new SDLMusic(CDX.Content.ContentFlags.Resource, handle, name);
                    Logger.Info($"Music loaded from resource '{name}'");
                }
            }
            return music;
        }

        public override bool IsPaused
        {
            get
            {
                return Mix_PausedMusic() == 1;
            }
        }

        public override void PauseMusic()
        {
            if (currentMusic != null)
            {
                Mix_PauseMusic();
            }
        }

        protected override void PlayMusic(IMusic? music, int loops = -1, bool forceRestart = false)
        {
            if (music is SDLMusic sdlMusic)
            {
                if (sdlMusic.Handle != IntPtr.Zero)
                {
                    if (forceRestart || currentMusic != sdlMusic)
                    {
                        if (currentMusic != null)
                        {
                            NotifyFinishedMusic(currentMusic, MusicFinishReason.Interrupted);
                        }
                        //currentMusic = sdlMusic;
                        if (Mix_PlayMusic(sdlMusic.Handle, loops) != 0)
                        {
                            Logger.Error($"Could not play music {sdlMusic} of type {sdlMusic.MusicType}: {SDLApplication.GetError()}");
                        }
                        else
                        {
                            _ = Mix_VolumeMusic(MusicVolume);
                            Logger.Info($"Music {sdlMusic} started");
                            currentMusic = sdlMusic;
                            lastFinishedMusic = null;
                        }
                    }
                }
            }
        }

        public override void ResumeMusic()
        {
            if (currentMusic != null)
            {
                Mix_ResumeMusic();
            }
        }

        public override void StopMusic()
        {
            if (currentMusic != null)
            {
                _ = Mix_HaltMusic();
                //currentMusic = null;
            }
        }

        public override void Update(FrameTime time)
        {
            CheckPlayListQueue();
        }

        private void MixPostMix(IntPtr udata, IntPtr stream, int len)
        {
            if (currentMusic != null && HasAudioDataHandler)
            {
                short[] data = new short[len / 2];
                Marshal.Copy(stream, data, 0, data.Length);
                OnAudioDataReceived(new AudioDataEventArgs(currentMusic, data));
            }
        }
        private void MixMusicFinished()
        {
            if (currentMusic != null)
            {
                NotifyFinishedMusic(currentMusic, MusicFinishReason.Finished);
            }
            else
            {
                Logger.Debug($"Music finished callback called on no music");
            }
        }

        private void NotifyFinishedMusic(SDLMusic music, MusicFinishReason reason)
        {
            if (music == lastFinishedMusic)
            {
                Logger.Warn($"Music {music} stopped again ({reason})");
            }
            else
            {
                lastFinishedMusic = music;
                Logger.Debug($"Music {music} stopped ({reason})");
                if (HasMusicFinishedHandler)
                {
                    OnAudioMusicDone(new AudioMusicFinishedEventArgs(music, reason));
                }
                MusicFinished(music, reason);
                if (music.Flags.HasFlag(CDX.Content.ContentFlags.Internal) && !music.Disposed)
                {
                    music.Dispose();
                }
                currentMusic = null;
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }
                CloseAudio();
                disposedValue = true;
            }
        }


        private const string LibName = "SDL2_mixer";

        private const int MIX_CHANNEL_POST = -2;

        private const ushort AUDIO_U8 = 0x0008;
        private const ushort AUDIO_S8 = 0x8008;
        private const ushort AUDIO_U16LSB = 0x0010;
        private const ushort AUDIO_S16LSB = 0x8010;
        private const ushort AUDIO_U16MSB = 0x1010;
        private const ushort AUDIO_S16MSB = 0x9010;
        private const ushort AUDIO_U16 = AUDIO_U16LSB;
        private const ushort AUDIO_S16 = AUDIO_S16LSB;
        private const ushort AUDIO_S32LSB = 0x8020;
        private const ushort AUDIO_S32MSB = 0x9020;
        private const ushort AUDIO_S32 = AUDIO_S32LSB;
        private const ushort AUDIO_F32LSB = 0x8120;
        private const ushort AUDIO_F32MSB = 0x9120;
        private const ushort AUDIO_F32 = AUDIO_F32LSB;

        private const byte MIX_MAX_VOLUME = 128;
        private const int MIX_DEFAULT_FREQUENCY = 44100;
        private static readonly ushort MIX_DEFAULT_FORMAT = BitConverter.IsLittleEndian ? AUDIO_S16LSB : AUDIO_S16MSB;

        [Flags]
        internal enum MIX_InitFlags
        {
            MIX_INIT_FLAC = 0x00000001,
            MIX_INIT_MOD = 0x00000002,
            MIX_INIT_MP3 = 0x00000008,
            MIX_INIT_OGG = 0x00000010,
            MIX_INIT_MID = 0x00000020,
            MIX_INIT_OPUS = 0x00000040
        }
        internal enum Mix_MusicType
        {
            MUS_NONE,
            MUS_CMD,
            MUS_WAV,
            MUS_MOD,
            MUS_MID,
            MUS_OGG,
            MUS_MP3,
            MUS_MP3_MAD_UNUSED,
            MUS_FLAC,
            MUS_MODPLUG_UNUSED,
            MUS_OPUS
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MixFuncDelegate(IntPtr udata, IntPtr stream, int len);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void MusicFinishedDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Mix_EffectFunc_t(int chan, IntPtr stream, int len, IntPtr udata);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Mix_EffectDone_t(int channel, IntPtr udata);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mix_Init(MIX_InitFlags flags);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Mix_Quit();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_OpenAudio(int frequency, ushort format, int channels, int chunksize);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Mix_CloseAudio();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_AllocateChannels(int numchans);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Mix_LoadWAV_RW(IntPtr src, int freesrc);

        public static IntPtr Mix_LoadWAV(string file)
        {
            IntPtr rwops = SDLApplication.SDL_RWFromFile(file, "rb");
            return Mix_LoadWAV_RW(rwops, 1);
        }

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Mix_LoadMUS([In()][MarshalAs(UnmanagedType.LPUTF8Str)] string file);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Mix_LoadMUS_RW(IntPtr rwops, int freesrc);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Mix_QuickLoad_WAV([In()][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] mem);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr Mix_QuickLoad_RAW([In()][MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1, SizeParamIndex = 1)] byte[] mem, uint len);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Mix_FreeChunk(IntPtr chunk);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Mix_FreeMusic(IntPtr music);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_PlayMusic(IntPtr music, int loops);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_FadeInMusic(IntPtr music, int loops, int ms);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_FadeOutMusic(int ms);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern double Mix_MusicDuration(IntPtr music);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_VolumeMusic(int volume);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_HaltMusic();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Mix_PauseMusic();

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Mix_ResumeMusic();
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Mix_GetMusicTitle(IntPtr music);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Mix_GetMusicTitleTag(IntPtr music);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Mix_GetMusicArtistTag(IntPtr music);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Mix_GetMusicAlbumTag(IntPtr music);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Mix_GetMusicCopyrightTag(IntPtr music);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_RegisterEffect(int channel, Mix_EffectFunc_t f, Mix_EffectDone_t d, IntPtr arg);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_UnregisterEffect(int channel, Mix_EffectFunc_t f);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_UnregisterAllEffects(int channel);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Mix_HookMusicFinished(MusicFinishedDelegate music_finished);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Mix_HookMusicFinished(IntPtr music_finished);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern Mix_MusicType Mix_GetMusicType(IntPtr music);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Mix_SetPostMix(MixFuncDelegate mix_func, IntPtr arg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Mix_SetPostMix(IntPtr mix_func, IntPtr arg);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_PausedMusic();
    }
}
