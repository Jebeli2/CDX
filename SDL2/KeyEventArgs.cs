namespace SDL2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class KeyEventArgs : EventArgs
    {
        private readonly ScanCode scanCode;
        private readonly KeyCode keyCode;
        private readonly KeyMod keyMod;
        private readonly KeyButtonState state;
        private readonly bool repeat;

        public KeyEventArgs(ScanCode scanCode, KeyCode keyCode, KeyMod keyMod, KeyButtonState state, bool repeat)
        {
            this.scanCode = scanCode;
            this.keyCode = keyCode;
            this.keyMod = keyMod;
            this.state = state;
            this.repeat = repeat;
        }

        public ScanCode ScanCode => scanCode;
        public KeyCode KeyCode => keyCode;
        public KeyMod KeyMod => keyMod;
        public KeyButtonState State => state;
        public bool Repeat => repeat;

        public override string ToString()
        {
            if (repeat)
            {
                return $"Key {(int)keyCode} {scanCode} - {keyCode}: {state} (repeat)";
            }
            else
            {
                return $"Key {(int)keyCode} {scanCode} - {keyCode}: {state}";
            }
        }
    }

    public delegate void KeyEventHandler(object? sender, KeyEventArgs e);
}
