namespace CDX.App
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ApplicationContext
    {
        private readonly List<Window> windows = new();
        private Configuration configuration;

        public ApplicationContext(Configuration configuration, params Window[] windows)
        {
            this.configuration = configuration;
            this.windows.AddRange(windows);
        }

        public IList<Window> Windows => windows;

        public Configuration Configuration
        {
            get => configuration;
        }
    }
}
