namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class TextInputEventArgs : EventArgs
    {
        private readonly string text;
        public TextInputEventArgs(string text)
        {
            this.text = text;
        }

        public string Text => text;

        public override string ToString()
        {
            return text;
        }
    }

    public delegate void TextInputEventHandler(object sender, TextInputEventArgs e);
}
