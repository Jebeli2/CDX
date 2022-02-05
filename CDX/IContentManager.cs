namespace CDX
{
    using CDX.Content;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IContentManager
    {
        void AddResources(System.Resources.ResourceManager resourceManager);
        void AddModPath(string path);
        void RegisterResourceLoader<T>(IResourceLoader<T> loader) where T : IResource;
        byte[]? FindResource(string? name);
        T? Load<T>(string name) where T : IResource;
    }
}
