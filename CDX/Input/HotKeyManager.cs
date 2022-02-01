namespace CDX.Input
{
    using CDX.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class HotKeyManager
    {
        //private readonly  hotKeys = new ();

        private readonly Dictionary<HotKey, HotKeyAction> hotKeys = new();
        private readonly HotKey finder = new();

        public void AddHotKeyAction(HotKeyAction hotKey)
        {
            hotKeys[hotKey.HotKey] = hotKey;
        }

        public void AddHotKey(HotKey hotKey, Action action, string? name)
        {
            if (name == null) { name = "ACTION" + hotKeys.Count; }
            AddHotKeyAction(new HotKeyAction(name, hotKey, action));
        }

        public void AddHotKey(string name, ScanCode scanCode, KeyMod keyMod, Action action)
        {
            AddHotKey(new HotKey(scanCode, keyMod), action, name);
        }
        public void AddHotKey(ScanCode scanCode, KeyMod keyMod, Action action)
        {
            string name = action.Method.Name;
            AddHotKey(new HotKey(scanCode, keyMod), action, name);
        }

        public void AddHotKey(ScanCode scanCode, Action action)
        {
            AddHotKey(scanCode, KeyMod.NONE, action);
        }

        public HotKeyAction? FindHotKeyAction(HotKey hotKey)
        {
            if (hotKeys.TryGetValue(hotKey, out HotKeyAction? hotKeyAction))
            {
                return hotKeyAction;
            }
            return null;
        }

        public HotKeyAction? FindHotKeyAction(ScanCode scanCode, KeyMod keyMod)
        {
            finder.ScanCode = scanCode;
            finder.KeyMod = keyMod & ~HotKey.IgnoredKeyMods;
            return FindHotKeyAction(finder);
        }

        public HotKeyAction? FindHotKeyAction(KeyEventArgs e)
        {
            return FindHotKeyAction(e.ScanCode, e.KeyMod);
        }

        public bool ExecuteHotKeyAction(KeyEventArgs e)
        {
            HotKeyAction? action = FindHotKeyAction(e);
            if (action != null && action.Action != null)
            {
                Logger.Verbose($"Executing HotKey Action {action}");
                action.Action();
                return true;
            }
            return false;
        }

    }
}
