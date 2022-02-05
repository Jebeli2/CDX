namespace CDX.Content
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Resources;
    using System.Text;
    using System.Threading.Tasks;

    public class NoContent : IContentManager
    {
        private static NoContent instance = new();
        public static IContentManager Instance => instance;
        private NoContent() { }
        public void AddResources(ResourceManager resourceManager)
        {
        }

        public void AddModPath(string path)
        {

        }

        public byte[]? FindResource(string? name)
        {
            return null;
        }

        public void RegisterResourceLoader<T>(IResourceLoader<T> loader) where T : IResource
        {

        }
        public T? Load<T>(string name) where T : IResource
        {
            return default;
        }
    }
}
