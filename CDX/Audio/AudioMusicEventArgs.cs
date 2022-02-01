namespace CDX.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AudioMusicEventArgs : EventArgs
    {
        private readonly IMusic music;
        public AudioMusicEventArgs(IMusic music)
        {
            this.music = music;
        }
        public IMusic Music => music;
    }

    public delegate void AudioMusicEventHandler(object sender, AudioMusicEventArgs e);
}
