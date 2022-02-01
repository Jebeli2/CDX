
using SDLHeaderConvert;

const string OUTFILE = @"D:\Temp\SDL2.cs";
const string PATH = @"c:\Users\jebel\source\repos\SDL-main\include\";
string[] files = new string[] { "SDL.h", "SDL_events.h", "SDL_rwops.h" };

SDLConvert convert = new("SDL2");
convert.StartConvert("SDL2");
foreach (var file in files)
{
    string text = File.ReadAllText(Path.Combine(PATH, file));    
    convert.AddFile(file, text);
}
string conv = convert.EndConvert();

File.WriteAllText(OUTFILE, conv);



