namespace CDX.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AudioDataEventArgs : AudioMusicEventArgs
    {
        private readonly short[] data;
        public AudioDataEventArgs(IMusic music, short[] data)
            : base(music)
        {
            this.data = data;
        }

        public short[] Data => data;
    }

    public delegate void AudioDataEventHandler(object sender, AudioDataEventArgs e);
}
