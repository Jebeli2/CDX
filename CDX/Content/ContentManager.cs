namespace CDX.Content
{
    using CDX.App;
    using CDX.Logging;
    using CDX.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ContentManager : IContentManager
    {
        protected readonly CDXApplication app;
        private readonly List<System.Resources.ResourceManager> resourceManagers = new();
        private readonly List<Flare.ModManager> modManagers = new();
        private readonly List<IResLoader> loaders = new();
        protected internal ContentManager(CDXApplication app)
        {
            this.app = app;
        }

        public void AddResources(System.Resources.ResourceManager resourceManager)
        {
            resourceManagers.Add(resourceManager);
        }

        public void AddModPath(string path)
        {
            Flare.ModManager modManager = new Flare.ModManager(path);
            if (modManager.ModCount > 0)
            {
                modManagers.Add(modManager);
            }
        }
        public void RegisterResourceLoader<T>(IResourceLoader<T> loader) where T : IResource
        {
            loader.ContentManager = this;
            loaders.Add(loader);
        }
        public byte[]? FindResource(string? name)
        {
            byte[]? data = null;
            if (!string.IsNullOrEmpty(name))
            {
                data = FindInFileSystem(name);
                if (data != null) return data;
                data = FindInResManagers(name);
                if (data != null) return data;
                data = FindInModManagers(name);
                if (data != null) return data;
            }
            return null;
        }

        private byte[]? FindInModManagers(string name)
        {
            byte[]? data = null;
            foreach (Flare.ModManager modManager in modManagers)
            {
                string? fileName = modManager.Locate(name);
                if (!string.IsNullOrEmpty(fileName))
                {
                    return FindInFileSystem(fileName);
                }
            }
            return data;
        }

        private byte[]? FindInResManagers(string name)
        {
            name = AdjustFileNameForResManager(name);
            foreach (System.Resources.ResourceManager rm in resourceManagers)
            {
                object? obj = rm.GetObject(name);
                if (obj != null)
                {
                    if (obj is byte[] data) { return data; }
                    else if (obj is string str)
                    {
                        return Encoding.UTF8.GetBytes(str);
                    }
                    UnmanagedMemoryStream? ums = rm.GetStream(name);
                    if (ums != null)
                    {
                        byte[] umsData = new byte[ums.Length];
                        ums.Read(umsData, 0, umsData.Length);
                        return umsData;
                    }
                }
            }
            return null;
        }

        private byte[]? FindInFileSystem(string name)
        {
            try
            {
                if (File.Exists(name))
                {
                    return File.ReadAllBytes(name);
                }
            }
            catch (IOException ioe)
            {
                Logger.Warn($"IOException during file read for '{name}': {ioe.Message}");
            }
            return null;
        }

        public T? Load<T>(string name) where T : IResource
        {
            byte[]? data = FindResource(name);
            foreach (var loader in GetResourceLoaders<T>())
            {
                T? result = loader.Load(name, data);
                if (result != null) return result;

            }
            if (data == null)
            {
                Logger.Error($"No resource found for {name}");
            }
            else
            {
                Logger.Error($"No loader found for {name}");
            }
            return default;
        }

        private IEnumerable<IResourceLoader<T>> GetResourceLoaders<T>() where T : IResource
        {
            foreach (var loader in loaders)
            {
                if (loader is IResourceLoader<T> resourceLoader) { yield return resourceLoader; }
            }
        }

        private static string AdjustFileNameForResManager(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            string fn = Path.GetFileName(fileName);
            string? ext = FileUtils.GetFileExtension(fn);
            if (ext == "txt")
            {
                fn = fn.Replace('.', '_');
            }
            else
            {
                fn = FileUtils.RemoveFileExtension(fn);
            }
            return fn;
        }

    }
}
