namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IControllerListener
    {
        void OnControllerButtonDown(ControllerButtonEventArgs e);
        void OnControllerButtonUp(ControllerButtonEventArgs e);
        void OnControllerAxis(ControllerAxisEventArgs e);

    }
}
