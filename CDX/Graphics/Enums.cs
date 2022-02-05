namespace CDX.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Flags]
    public enum BlendMode
    {
        None = 0x00000000,
        Blend = 0x00000001,
        Add = 0x00000002,
        Mod = 0x00000004,
        Mul = 0x00000008,
        Invalid = 0x7FFFFFFF
    }

    [Flags]
    public enum RenderFlip
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        Both = Horizontal | Vertical
    }

    public enum TextureFilter
    {
        Linear,
        Nearest,
        Best
    }

    [Flags]
    public enum FontStyle
    {
        Normal = 0x00,
        Bold = 0x01,
        Italic = 0x02,
        Underline = 0x04,
        Strikethrough = 0x08
    }

    public enum FontHinting
    {
        Normal = 0,
        Light = 1,
        Mono = 2,
        None = 3,
        LightSubPixel = 4
    }

    public enum HorizontalAlignment
    {
        Left,
        Right,
        Center,
        Stretch
    }

    public enum VerticalAlignment
    {
        Top,
        Bottom,
        Center,
        Stretch
    }

    public enum NinePatchFillMode
    {
        Stretch,
        Repeat
    }

    public enum AnimationType
    {
        None,
        PlayOnce,
        Looped,
        BackForth
    }
}
