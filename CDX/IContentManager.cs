namespace CDX
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IContentManager
    {
        void AddResources(System.Resources.ResourceManager resourceManager);
        byte[]? FindResource(string? name);
    }
}
