namespace CDX.Content
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Flags]
    public enum ContentFlags
    {
        None = 0,
        Image = 1,
        Font = 2,
        Music = 4,
        File = 8,
        Data = 16,
        Resource = 32,
        Created = 64,
        Internal = 128
    }
}
