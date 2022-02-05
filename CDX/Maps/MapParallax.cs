namespace CDX.Maps
{
    using CDX.App;
    using CDX.Content;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MapParallax : Resource
    {
        private readonly List<ParallaxLayer> layers = new();
        public MapParallax(string name) : base(name, ContentFlags.Data)
        {
        }

        public IList<ParallaxLayer> Layers => layers;
        public IList<ParallaxLayer> GetMatchingLayers(string mapLayer)
        {
            List<ParallaxLayer> list = new();
            foreach (var layer in layers)
            {
                if (string.IsNullOrEmpty(mapLayer))
                {
                    if (string.IsNullOrEmpty(layer.MapLayer))
                    {
                        list.Add(layer);
                    }
                }
                else if (mapLayer.Equals(layer.MapLayer, StringComparison.OrdinalIgnoreCase))
                {
                    list.Add(layer);
                }
            }
            return list;
        }

        public ParallaxLayer AddLayer()
        {
            ParallaxLayer layer = new ParallaxLayer();
            layers.Add(layer);
            return layer;
        }

        public void Update(FrameTime time)
        {
            foreach (var layer in layers)
            {
                layer.Update(time);
            }
        }
    }
}
