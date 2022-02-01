namespace CDX.Content
{
    using CDX.App;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ContentManager : IContentManager
    {
        protected readonly CDXApplication app;
        private readonly List<System.Resources.ResourceManager> resourceManagers = new();

        protected internal ContentManager(CDXApplication app)
        {
            this.app = app;
        }

        public void AddResources(System.Resources.ResourceManager resourceManager)
        {
            resourceManagers.Add(resourceManager);
        }

        public byte[]? FindResource(string? name)
        {
            if (name != null)
            {
                if (File.Exists(name))
                {
                    return File.ReadAllBytes(name);
                }
                foreach (System.Resources.ResourceManager rm in resourceManagers)
                {
                    object? obj = rm.GetObject(name);
                    if (obj != null)
                    {
                        if (obj is byte[] data) { return data; }

                        UnmanagedMemoryStream? ums = rm.GetStream(name);
                        if (ums != null)
                        {
                            byte[] umsData = new byte[ums.Length];
                            ums.Read(umsData, 0, umsData.Length);
                            return umsData;
                        }
                    }
                }
            }
            return null;
        }
    }
}
