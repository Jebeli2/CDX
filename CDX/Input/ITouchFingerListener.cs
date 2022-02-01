namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ITouchFingerListener
    {
        void OnTouchFingerDown(TouchFingerEventArgs e);
        void OnTouchFingerUp(TouchFingerEventArgs e);

        void OnTouchFingerMotion(TouchFingerEventArgs e);

    }
}
