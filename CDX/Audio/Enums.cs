namespace CDX.Audio
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum MusicType
    {
        None,
        Cmd,
        Wav,
        Mod,
        Mid,
        Ogg,
        MP3,
        MP3_MAD_UNUSED,
        Flac,
        MODPLUG_UNUSED,
        Opus
    }

    public enum MusicFinishReason
    {
        Finished,
        Interrupted
    }
}
