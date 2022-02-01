namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class HotKeyAction
    {

        public HotKeyAction(string name)
            : this(name, new HotKey())
        {

        }
        public HotKeyAction(string name, HotKey hotKey, Action? action = null)
        {
            Name = name;
            HotKey = hotKey;
            Action = action;
        }
        public string Name { get; set; }
        public HotKey HotKey { get; set; }
        public Action? Action { get; set; }

        public override string ToString()
        {
            return $"{Name}:{HotKey}";
        }
    }
}
