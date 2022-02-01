namespace CDX.Audio
{
    using CDX.Content;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMusic : IResource
    {
        MusicType MusicType { get; }
    }
}
