namespace CDX.Graphics
{
    using CDX.Content;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IImage : IResource
    {
        int Width { get; }
        int Height { get; }
        TextureFilter TextureFilter { get; set; }
        byte AlphaMod { get; set; }
        Color ColorMod { get; set; }
    }
}
