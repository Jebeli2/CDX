namespace CDXSDL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal static class Extensions
    {
        public static T[] AsArray<T>(this IEnumerable<T> src)
        {
            if (src is T[] array) { return array; }
            return src.ToArray();
        }

    }
}
