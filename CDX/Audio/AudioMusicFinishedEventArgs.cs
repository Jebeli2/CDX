namespace CDX.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AudioMusicFinishedEventArgs : AudioMusicEventArgs
    {
        private readonly MusicFinishReason reason;
        public AudioMusicFinishedEventArgs(IMusic music, MusicFinishReason reason)
            : base(music)
        {
            this.reason = reason;
        }
        public MusicFinishReason Reason => reason;
    }

    public delegate void AudioMusicFinishedEventHandler(object sender, AudioMusicFinishedEventArgs e);
}
