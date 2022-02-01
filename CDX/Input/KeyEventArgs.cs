namespace CDX.Input
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

        public bool HandledByHotKeyManager { get; set; }

        //public bool MatchesHotKey(KeyCode key, KeyMod mod)
        //{
        //    if (key == keyCode)
        //    {
        //        if (mod == KeyMod.NONE) return true;
        //        return (keyMod & mod) != 0;
        //    }
        //    return false;
        //}
        //public bool MatchesHotKey(KeyCode key, KeyMod mod, KeyButtonState state)
        //{
        //    if (key == keyCode && this.state == state)
        //    {
        //        if (mod == KeyMod.NONE) return true;
        //        return (keyMod & mod) != 0;
        //    }
        //    return false;
        //}
        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(scanCode);
            sb.Append(" - ");
            sb.Append(keyCode);
            if (keyMod != KeyMod.NONE)
            {
                sb.Append(" - ");
                sb.Append(keyMod);
            }
            sb.Append(' ');
            sb.Append(state);
            if (repeat)
            {
                sb.Append(" (repeat)");
            }
            return sb.ToString();
        }
    }

    public delegate void KeyEventHandler(object sender, KeyEventArgs e);
}
