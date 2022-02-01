namespace CDX.Maps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Map
    {
        private readonly List<MapLayer> layers = new();


        public int LayerCount => layers.Count;
        public IEnumerable<MapLayer> Layer => layers;

        public void AddLayer(MapLayer layer)
        {
            layers.Add(layer);
        }

        public bool RemoveLayer(MapLayer layer)
        {
            return layers.Remove(layer);
        }

        public bool RemoveLayerAt(int index)
        {
            if (index >= 0 && index < layers.Count)
            {
                layers.RemoveAt(index);
                return true;
            }
            return false;
        }
    }
}
