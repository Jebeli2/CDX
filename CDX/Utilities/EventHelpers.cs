namespace CDX.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class EventHelpers
    {
        public static void Raise<T>(object sender, EventHandler<T>? handler, T e) where T : EventArgs
        {
            handler?.Invoke(sender, e);
        }
    }
}
