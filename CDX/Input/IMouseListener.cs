namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMouseListener
    {
        void OnMouseButtonDown(MouseButtonEventArgs e);

        void OnMouseButtonUp(MouseButtonEventArgs e);

        void OnMouseMoved(MouseMotionEventArgs e);

        void OnMouseWheel(MouseWheelEventArgs e);
    }
}
