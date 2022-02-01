namespace CDX.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IScreen:IBox
    {
        string? Title { get; set; }
    }
}
