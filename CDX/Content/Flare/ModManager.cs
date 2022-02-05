namespace CDX.Content.Flare
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ModManager
    {
        private readonly Dictionary<string, string> locCache = new();
        private readonly List<string> modPaths;
        private readonly List<string> modDirs;
        private readonly List<Mod> modList;

        public ModManager(string path)
        {
            path = CleanName(path);
            if (!path.EndsWith("/")) { path += "/"; }
            modPaths = new() { path };
            modDirs = GetModDirs(path);
            modList = GetModList(path);
        }

        public int ModCount => modList.Count;
        public string Locate(string filename)
        {
            filename = CleanName(filename);
            if (string.IsNullOrEmpty(filename)) return "";
            if (locCache.TryGetValue(filename, out string? fn))
            {
                return fn;
            }
            string testPath;
            foreach (var mod in modList)
            {
                testPath = mod.GetFileName(filename);
                testPath = Path.GetFullPath(testPath);
                if (File.Exists(testPath))
                {
                    locCache[filename] = testPath;
                    return testPath;
                }
            }
            testPath = filename;
            testPath = Path.GetFullPath(testPath);
            if (File.Exists(testPath))
            {
                locCache[filename] = testPath;
                return testPath;
            }
            if (filename.EndsWith('/'))
            {
                foreach (var mod in modList)
                {
                    testPath = mod.GetFileName(filename);
                    testPath = Path.GetFullPath(testPath);
                    if (Directory.Exists(testPath))
                    {
                        locCache[filename] = testPath;
                        return testPath;
                    }
                }
            }
            return "";
        }

        public IList<string> List(string path, string ext = "txt", bool fullPaths = true)
        {
            List<string> list = new List<string>();
            string testPath;
            path = CleanName(path);
            foreach (var mod in modList)
            {
                testPath = CleanName(mod.GetFileName(path));
                if (File.Exists(testPath))
                {
                    list.Add(testPath);
                }
                else if (Directory.Exists(testPath))
                {
                    GetFileList(testPath, ext, list);
                }
            }
            if (list.Count > 0 && !fullPaths)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i] = list[i].Substring(list[i].IndexOf(path));
                }
                for (int i = 0; i < list.Count; i++)
                {
                    int j = i + 1;
                    while (j < list.Count)
                    {
                        if (list[i].Equals(list[j]))
                        {
                            list.RemoveAt(j);
                        }
                        else
                        {
                            j++;
                        }
                    }
                }
            }
            return list;
        }
        private static List<string> GetModDirs(string path)
        {
            var list = new List<string>();
            GetDirList(path + "mods", list);
            return list;
        }

        private List<Mod> GetModList(string path)
        {
            var list = new List<Mod>();
            string place1 = path + "mods.txt";
            string place2 = path + "mods/mods.txt";
            string place = "";
            if (File.Exists(place1)) { place = place1; }
            else if (File.Exists(place2)) { place = place2; }
            if (!string.IsNullOrEmpty(place))
            {
                var infile = File.OpenText(place);
                string? line = "";
                while (line != null)
                {
                    line = infile.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    if (line.StartsWith("#")) continue;
                    if (modDirs.Contains(line))
                    {
                        var mod = LoadMod(line);
                        if (mod != null)
                        {
                            list.Add(mod);
                        }
                    }
                }
                infile.Close();
            }
            return list;
        }

        private Mod? LoadMod(string name)
        {
            foreach (var path in modPaths)
            {
                string settingsFile = path + "mods/" + name + "/settings.txt";
                if (File.Exists(settingsFile))
                {
                    Mod mod = new Mod(name);
                    mod.Path = path;
                    return mod;
                }
            }
            return null;
        }

        private static void GetDirList(string dir, IList<string> dirs)
        {
            if (Directory.Exists(dir))
            {
                foreach (var d in Directory.GetDirectories(dir))
                {
                    string directory = Path.GetFileName(d);
                    dirs.Add(CleanName(directory));
                }
            }
        }

        private static void GetFileList(string dir, string ext, IList<string> files)
        {
            if (Directory.Exists(dir))
            {
                foreach (var d in Directory.GetFiles(dir, "*." + ext, SearchOption.TopDirectoryOnly))
                {
                    string file = Path.GetFileName(d);
                    files.Add(CleanName(dir + "/" + file));
                }
            }
        }

        private static string CleanName(string name)
        {
            if (name == null) return string.Empty;
            if (name == "") return "/";
            return name.Replace("\\", "/");
        }
    }
}
