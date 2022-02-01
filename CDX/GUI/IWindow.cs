namespace CDX.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IWindow : IBox
    {
        WindowFlags WindowFlags { get; }
        string? Title { get; set; }
    }
}
