namespace CDX.Content
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Resource : IResource, IDisposable
    {
        private readonly string name;
        private ContentFlags flags;
        private bool disposedValue;

        public Resource(string name, ContentFlags flags)
        {
            this.name = name;
            this.flags = flags;
        }

        public string Name => name;
        public ContentFlags Flags
        {
            get => flags;
            internal set => flags = value;
        }
        public bool Disposed => disposedValue;

        ContentFlags IResource.Flags { get => flags; set => flags = value; }

        public override string ToString()
        {
            return "'" + name + "'";
        }

        protected virtual void DisposeUnmanaged()
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                DisposeUnmanaged();
                disposedValue = true;
            }
        }

        ~Resource()
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
