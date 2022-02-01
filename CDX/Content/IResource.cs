namespace CDX.Content
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IResource : IDisposable
    {
        string Name { get; }
        bool Disposed { get; }
        ContentFlags Flags { get; internal set; }
    }
}
