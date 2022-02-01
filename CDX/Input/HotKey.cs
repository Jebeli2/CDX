namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class HotKey : IEquatable<HotKey>
    {
        public const KeyMod IgnoredKeyMods = KeyMod.NUM | KeyMod.CAPS;
        public HotKey()
        {
            ScanCode = ScanCode.SCANCODE_UNKNOWN;
            KeyMod = KeyMod.NONE;
        }

        public HotKey(ScanCode scanCode, KeyMod keyMod)
        {
            ScanCode = scanCode;
            KeyMod = keyMod;
        }

        public ScanCode ScanCode { get; set; }
        public KeyMod KeyMod { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (KeyMod != KeyMod.NONE)
            {
                sb.Append(KeyMod);
                sb.Append(' ');
            }
            if (ScanCode != ScanCode.SCANCODE_UNKNOWN)
            {
                sb.Append(ScanCode);
            }
            return sb.ToString();
        }

        public bool Equals(HotKey? other)
        {
            return other != null && ScanCode == other.ScanCode && KeyMod == other.KeyMod;
        }

        public override bool Equals(object? obj)
        {
            if (obj is HotKey other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ScanCode, KeyMod);
        }

        public bool Matches(ScanCode scanCode, KeyMod keyMod)
        {
            if (ScanCode == scanCode)
            {
                keyMod &= ~IgnoredKeyMods;
                if (KeyMod == KeyMod.NONE)
                {
                    return keyMod == 0;
                }
                else
                {
                    return (keyMod & KeyMod) == KeyMod;
                }
            }
            return false;
        }

        public bool Matches(KeyEventArgs e)
        {
            return Matches(e.ScanCode, e.KeyMod);
        }
    }
}
