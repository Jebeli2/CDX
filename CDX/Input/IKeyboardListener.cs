namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IKeyboardListener
    {
        void OnKeyDown(KeyEventArgs e);

        void OnKeyUp(KeyEventArgs e);

        void OnTextInput(TextInputEventArgs e);
    }
}
