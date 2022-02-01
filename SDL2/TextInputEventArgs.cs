namespace SDL2
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
            return $"TextInput: {text}";
        }
    }

    public delegate void TextInputEventHandler(object sender, TextInputEventArgs e);
}
