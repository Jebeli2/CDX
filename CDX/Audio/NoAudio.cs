namespace CDX.Audio
{
    using CDX.App;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class NoAudio : IAudio
    {
        private static readonly NoAudio instance = new();
        public static NoAudio Instance => instance;
        private NoAudio() { }
        public bool UseTmpFilesForMusic { get; set; }
        public int MusicVolume { get; set; }
        public int SoundVolume { get; set; }

        public IMusic? CurrentMusic => null;

        public bool IsPaused => false;

        public event AudioDataEventHandler AudioDataReceived { add { } remove { } }
        public event AudioMusicFinishedEventHandler AudioMusicFinished { add { } remove { } }

        public void AddToPlayList(string name, int loops = 1)
        {
        }

        public void ClearPlayList()
        {
        }

        public void Dispose()
        {

        }

        public IMusic? LoadMusic(string name)
        {
            return null;
        }

        public IMusic? LoadMusic(byte[]? data, string name)
        {
            return null;
        }

        public void NextMusic()
        {
        }

        public void PauseMusic()
        {
        }

        public void PreviousMusic()
        {
        }

        public void ResumeMusic()
        {
        }

        public void StopMusic()
        {
        }

        public void Update(FrameTime time)
        {
        }
    }
}
