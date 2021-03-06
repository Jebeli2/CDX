namespace CDX.Maps
{
    using CDX.Actors;
    using CDX.Content;
    using CDX.Events;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class Map : Resource
    {
        private readonly int width;
        private readonly int height;
        private readonly List<MapLayer> layers = new();
        private TileSet? tileSet;
        private TileSet? collisionTileSet;
        private IMapCollision? collision;
        private MapParallax? parallax;
        private Color backgroundColor;
        private List<ActorInfo> actorInfos = new();
        private List<EventInfo> eventInfos = new();

        public Map(string name, int width, int height) : base(name, ContentFlags.Data)
        {
            this.width = width;
            this.height = height;
            Title = name;
            Music = "";
            backgroundColor = Color.Black;
        }

        public int Width => width;
        public int Height => height;
        public string Title { get; set; }
        public string Music { get; set; }
        public Color BackgroundColor
        {
            get => backgroundColor;
            set => backgroundColor = value;
        }
        public TileSet? TileSet
        {
            get => tileSet;
            set => tileSet = value;
        }

        public TileSet? CollisionTileSet
        {
            get => collisionTileSet;
            set => collisionTileSet = value;
        }

        public MapParallax? Parallax
        {
            get => parallax;
            set => parallax = value;
        }
        public IMapCollision? Collision => collision;

        public MapOrientation Orientation { get; set; }
        public int StartPosX { get; set; }
        public int StartPosY { get; set; }


        public IEnumerable<ActorInfo> ActorInfos => actorInfos;

        public void AddActorInfo(ActorInfo info)
        {
            actorInfos.Add(info);
        }

        public IEnumerable<EventInfo> EventInfos => eventInfos;

        public void AddEventInfo(EventInfo info)
        {
            eventInfos.Add(info);
        }
        public int LayerCount => layers.Count;
        public IEnumerable<MapLayer> Layers => layers;

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

        public void InitCollision()
        {
            MapLayer? collisionLayer = layers.FirstOrDefault(x => x.Type == LayerType.Collision);
            if (collisionLayer != null)
            {
                collision = new AStar.MapCollision(collisionLayer);
                layers.Remove(collisionLayer);
            }
            else
            {
                collision = new AStar.MapCollision(width, height);
            }

        }
    }
}
