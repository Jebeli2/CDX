namespace CDX.Audio
{
    using CDX.App;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class BaseAudio : IAudio
    {
        private bool disposedValue;
        private readonly CDXApplication app;
        private readonly EventHandlerList eventHandlerList = new();
        private readonly object audioDataReceivedKey = new();
        private readonly object audioMusicDoneKey = new();
        private int musicVolume;
        private int soundVolume;
        private bool repeatPlayList = true;
        private int playListIndex = -1;
        private readonly List<PlayListEntry> playList = new();
        private readonly Queue<PlayListEntry> playQueue = new();
        //private int updateCount;

        protected BaseAudio(CDXApplication app)
        {
            this.app = app;
        }

        public bool UseTmpFilesForMusic { get; set; }

        public event AudioDataEventHandler AudioDataReceived
        {
            add { eventHandlerList.AddHandler(audioDataReceivedKey, value); }
            remove { eventHandlerList.RemoveHandler(audioDataReceivedKey, value); }
        }

        public event AudioMusicFinishedEventHandler AudioMusicFinished
        {
            add { eventHandlerList.AddHandler(audioMusicDoneKey, value); }
            remove { eventHandlerList.RemoveHandler(audioMusicDoneKey, value); }
        }

        public abstract bool IsPaused { get; }

        public int MusicVolume
        {
            get => musicVolume;
            set
            {
                if (musicVolume != value)
                {
                    musicVolume = value;
                    SetMusicVolume(musicVolume);
                }
            }
        }
        public int SoundVolume
        {
            get => soundVolume;
            set
            {
                if (soundVolume != value)
                {
                    soundVolume = value;
                    SetSoundVolume(soundVolume);
                }
            }
        }

        public abstract IMusic? CurrentMusic { get; }

        //public void BeginUpdate()
        //{
        //    updateCount++;
        //}

        //public void EndUpdate()
        //{
        //    updateCount--;
        //}
        //public bool IsUpdating => updateCount > 0;


        protected void OnAudioDataReceived(AudioDataEventArgs e)
        {
            ((AudioDataEventHandler?)eventHandlerList[audioDataReceivedKey])?.Invoke(this, e);
        }

        protected void OnAudioMusicDone(AudioMusicFinishedEventArgs e)
        {
            ((AudioMusicFinishedEventHandler?)eventHandlerList[audioMusicDoneKey])?.Invoke(this, e);
        }

        //protected void HandleMusicFinished(IMusic music)
        //{
        //    if ((music.Flags & Content.ContentFlags.Internal) == Content.ContentFlags.Internal)
        //    {
        //        music.Dispose();
        //    }
        //}

        protected bool HasAudioDataHandler
        {
            get
            {
                return eventHandlerList[audioDataReceivedKey] != null;
            }
        }
        protected bool HasMusicFinishedHandler
        {
            get
            {
                return eventHandlerList[audioMusicDoneKey] != null;
            }
        }

        public void ClearPlayList()
        {
            playList.Clear();
            playListIndex = -1;
        }

        public void AddToPlayList(string name, int loops = 1)
        {
            PlayListEntry entry = new PlayListEntry() { Name = name, Loop = loops };
            playList.Add(entry);
        }

        public void PlayNow(string name, int loops = 1)
        {
            PlayListEntry entry = new PlayListEntry() { Name = name, Loop = loops };
            playListIndex = -1;
            playList.Clear();
            playList.Add(entry);
            NextMusic();
        }

        protected void CheckPlayListQueue()
        {
            if (playQueue.Count > 0)
            {
                PlayListEntry entry = playQueue.Dequeue();
                Play(entry);
            }
        }

        protected void CheckPlayList()
        {
            PlayListEntry? entry = CurrentEntry;
            if (entry != null)
            {
                playQueue.Enqueue(entry);
            }
        }

        public PlayListEntry? CurrentEntry
        {
            get
            {
                if (playListIndex >= 0 && playListIndex < playList.Count)
                {
                    return playList[playListIndex];
                }
                return null;
            }
        }

        public void NextMusic()
        {
            playListIndex++;
            if (playListIndex >= playList.Count && repeatPlayList)
            {
                playListIndex = 0;
            }
            CheckPlayList();
        }

        public void PreviousMusic()
        {
            playListIndex--;
            if (playListIndex < 0 && repeatPlayList)
            {
                playListIndex = playList.Count - 1;
            }
            CheckPlayList();
        }


        private bool Play(PlayListEntry entry)
        {
            IMusic? music = LoadPlayListEntry(entry);
            if (music != null)
            {
                //StopMusic();
                PlayMusic(music, entry.Loop, true);
                return true;
            }
            return false;
        }

        private IMusic? LoadPlayListEntry(PlayListEntry? entry)
        {
            IMusic? music = null;
            if (entry != null)
            {
                music = LoadMusic(entry.Name);
                if (music != null)
                {
                    music.Flags |= Content.ContentFlags.Internal;
                }
            }
            return music;
        }

        protected abstract void SetMusicVolume(int volume);
        protected abstract void SetSoundVolume(int volume);

        protected abstract void PlayMusic(IMusic? music, int loops = -1, bool forceRestart = false);

        public abstract void StopMusic();

        public abstract void PauseMusic();

        public abstract void ResumeMusic();

        public abstract IMusic? LoadMusic(string name);

        public abstract IMusic? LoadMusic(byte[]? data, string name);
        public abstract void Update(FrameTime time);

        protected void MusicFinished(IMusic music, MusicFinishReason reason)
        {
            if ((reason == MusicFinishReason.Finished) && (playList.Count > 0))
            {
                NextMusic();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                disposedValue = true;
            }
        }

        ~BaseAudio()
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
    }
}
