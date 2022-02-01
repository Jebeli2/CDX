﻿namespace CDX.GUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IGadget:IBox
    {
        GadgetFlags Flags { get; }  
        GadgetActivation Activation { get; }
        string? Text { get; }
        int GadgetID { get; set; }
    }
}
