namespace CDX.Graphics
{
    using CDX.Content;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ITextFont : IResource
    {
        int YSize { get; }
        string FamilyName { get; }
        string StyleName { get; }
        FontStyle FontStyle { get; }
        int Outline { get; }
        FontHinting Hinting { get; }
        int Height { get; }
        int Ascent { get; }
        int Descent { get; }
        int LineSkip { get; }
        int Kerning { get; }
        Size MeasureText(string? text);
    }
}
