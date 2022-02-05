namespace CDX.Content.Flare
{
    using CDX.Graphics;
    using CDX.Logging;
    using CDX.Maps;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ModParallaxLoader : ModLoader, IResourceLoader<MapParallax>
    {
        public ModParallaxLoader()
            : base("Flare Mod Parallax Loader")
        {

        }

        public MapParallax? Load(string name, byte[]? data)
        {
            MapParallax? result = null;
            using (FileParser infile = new FileParser(name, data))
            {
                result = LoadParallax(infile, name);
                if (result != null)
                {
                    Logger.Info($"MapParallax loaded from resource '{name}'");
                }
            }
            return result;
        }

        private MapParallax LoadParallax(FileParser infile, string name)
        {
            MapParallax parallax = new MapParallax(name);
            ParallaxLayer? layer = null;
            while (infile.Next())
            {
                if (infile.MatchNewSection("layer")) { layer = parallax.AddLayer(); }
                if (layer != null && infile.MatchSectionKey("layer", "image")) { layer.Image = ContentManager?.Load<IImage>(infile.GetStrVal()); }
                else if (layer != null && infile.MatchSectionKey("layer", "speed")) { layer.Speed = infile.GetFloatVal(); }
                else if (layer != null && infile.MatchSectionKey("layer", "fixed_speed"))
                {
                    layer.FixedSpeedX = infile.PopFirstFloat();
                    layer.FixedSpeedY = infile.PopFirstFloat();
                }
                else if (layer != null && infile.MatchSectionKey("layer", "map_layer")) { layer.MapLayer = infile.Val; }
            }
            return parallax;
        }
    }
}
