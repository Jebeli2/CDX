namespace CDX
{
    using CDX.App;
    using CDX.Audio;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAudio : IDisposable
    {
        /// <summary>
        /// This is for SDL_mixer.
        /// Should LoadMusic(byte[]? data...) use temporary files or not?
        /// If yes, the bytes in data will be copied to a temporary file and
        /// Mix_LoadMUS will be used to load the music. If not, the bytes
        /// will be loaded with Mix_LoadMUS_RW.
        /// Mix_LoadMUS_RW seems to have a lot of issues with some types of 
        /// music, hence the option to not use it.
        /// </summary>
        bool UseTmpFilesForMusic { get; set; }

        event AudioDataEventHandler AudioDataReceived;
        event AudioMusicFinishedEventHandler AudioMusicFinished;
        int MusicVolume { get; set; }
        int SoundVolume { get; set; }
        IMusic? CurrentMusic { get; }
        bool IsPaused { get; }
        void ClearPlayList();
        void AddToPlayList(string name, int loops = 1);
        void PlayNow(string name, int loops = 1);
        void NextMusic();
        void PreviousMusic();        
        void StopMusic();
        void PauseMusic();
        void ResumeMusic();
        IMusic? LoadMusic(string name);
        IMusic? LoadMusic(byte[]? data, string name);

        void Update(FrameTime time);
    }
}
