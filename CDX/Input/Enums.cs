namespace CDX.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public enum MouseButton
    {
        None = 0,
        Left = 1,
        Middle = 2,
        Right = 3,
        X1 = 4,
        X2 = 5
    }

    public enum KeyButtonState
    {
        Released = 0,
        Pressed = 1
    }

    public enum MouseWheelDirection
    {
        Normal,
        Flipped
    }

    public enum ControllerButton
    {
        None = -1,
        X = 0,
        C = 1,
        S = 2,
        T = 3,
        Share = 4,
        PS = 5,
        Options = 6,
        L3 = 7,
        R3 = 8,
        L1 = 9,
        R1 = 10,
        Up = 11,
        Down = 12,
        Left = 13,
        Right = 14,
        Misc = 15,
        Paddle1 = 16,
        Paddle2 = 17,
        Paddle3 = 18,
        Paddle4 = 18,
        Pad = 20,
        Max = 21
    }


    public enum ControllerAxis
    {
        INVALID = -1,
        LEFTX,
        LEFTY,
        RIGHTX,
        RIGHTY,
        TRIGGERLEFT,
        TRIGGERRIGHT,
        MAX
    }

    [Flags]
    public enum KeyMod : ushort
    {
        NONE = 0x0000,
        LSHIFT = 0x0001,
        RSHIFT = 0x0002,
        LCTRL = 0x0040,
        RCTRL = 0x0080,
        LALT = 0x0100,
        RALT = 0x0200,
        LGUI = 0x0400,
        RGUI = 0x0800,
        NUM = 0x1000,
        CAPS = 0x2000,
        MODE = 0x4000,
        SCROLL = 0x8000,
        CTRL = (LCTRL | RCTRL),
        SHIFT = (LSHIFT | RSHIFT),
        ALT = (LALT | RALT),
        GUI = (LGUI | RGUI)
    }
}
