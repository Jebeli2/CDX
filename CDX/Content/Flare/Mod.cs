namespace CDX.Content.Flare
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Mod
    {
        private string name;

        public Mod(string name)
        {
            this.name = name;
        }

        public string Name => name;
        public string Path { get; set; }
        public string GetFileName(string fileName)
        {
            return Path + "mods/" + name + "/" + fileName;
        }
    }
}
