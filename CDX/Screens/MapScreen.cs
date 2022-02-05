namespace CDX.Screens
{
    using CDX.Actors;
    using CDX.App;
    using CDX.Events;
    using CDX.Graphics;
    using CDX.Input;
    using CDX.Maps;
    using CDX.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class MapScreen : BaseScreen
    {
        private string? mapName;
        private string? playerName;
        private ActorManager actorManager;
        private EventManager eventManager;
        private Map? map;
        private string? collisionTileSetName;
        private TileSet? collisionTileSet;
        private IMapRenderer mapRenderer;
        private bool panning;
        private bool hasPanned;
        private int panDX;
        private int panDY;
        private int mouseX;
        private int mouseY;
        private Tooltip? tooltip;
        private Actor? player;


        public MapScreen(Window window, string name) : base(window, name)
        {
            actorManager = new ActorManager();
            eventManager = new EventManager();
            mapRenderer = new FlareMapRenderer();
            actorManager.EventManager = eventManager;
            MousePanning = true;
            CommandMoving = true;
        }
        public bool MousePanning { get; set; }
        public bool CommandMoving { get; set; }

        public string? MapName
        {
            get => mapName;
            set => SetMapName(value);
        }

        public string? PlayerName
        {
            get => playerName;
            set => SetPlayerName(value);
        }

        public string? CollisionTileSetName
        {
            get => collisionTileSetName;
            set => SetCollisionTileSetName(value);
        }

        public bool ShowCollision
        {
            get => mapRenderer.ShowCollision;
            set => mapRenderer.ShowCollision = value;
        }

        protected override void FirstShow()
        {
            base.FirstShow();
            LoadMap();
            LoadCollisionTileSet();
        }

        public override void OnMouseButtonDown(MouseButtonEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
            if (e.Button == MouseButton.Right)
            {
                panning = true;
            }
        }

        public override void OnMouseButtonUp(MouseButtonEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
            if (e.Button == MouseButton.Right)
            {
                panning = false;
            }
            else if (e.Button == MouseButton.Left)
            {
                if (CommandMoving && player != null)
                {
                    actorManager.MakeCommands(player, mouseX, mouseY);
                }
            }
        }

        public override void OnMouseMoved(MouseMotionEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;
            if (panning)
            {
                panDX = -e.RelX;
                panDY = -e.RelY;
            }
            mapRenderer.ScreenToMap(mouseX, mouseY, out float mx, out float my);
            mx = MathUtils.RoundForMap(mx);
            my = MathUtils.RoundForMap(my);
            eventManager.CheckHotSpotEvents(mx, my);
        }

        public override void Update(FrameTime time)
        {
            if (map != null)
            {
                actorManager.Update(time);
                mapRenderer.Update(time, map);
                if (MousePanning && panning && (panDX != 0 || panDY != 0))
                {
                    Pan(panDX, panDY);
                    panDX = 0;
                    panDY = 0;
                }
                actorManager.HandleHoverSelection(mouseX, mouseY);
            }
        }

        public override void Render(IGraphics graphics, FrameTime time)
        {
            if (map != null)
            {
                if (map.BackgroundColor != Color.Black)
                {
                    graphics.ClearScreen(map.BackgroundColor);
                }
                List<ISortableSprite> front = new(actorManager.GetLivingSprites());
                List<ISortableSprite> back = new(actorManager.GetDeadSprites());
                mapRenderer.Render(graphics, time, map, front, back);
                RenderTooltip(graphics);
            }
        }

        private void RenderTooltip(IGraphics graphics)
        {
            if (tooltip != null)
            {
                tooltip.Data = eventManager.TooltipData;
                if (!tooltip.IsEmpty)
                {
                    mapRenderer.MapToScreen(eventManager.TooltipMapX, eventManager.TooltipMapY, out int tx, out int ty);
                    tooltip.Render(tx + eventManager.TooltipOffsetX, ty + eventManager.TooltipOffsetY);
                }
            }
        }

        private void Pan(int dx, int dy)
        {
            mapRenderer.ShiftCam(dx, dy);
        }

        private void SetMapName(string? name)
        {
            mapName = name;
            LoadMap();
        }

        private void SetPlayerName(string? name)
        {
            playerName = name ?? "male.txt";
            actorManager.PlayerInfo = new ActorInfo { Id = playerName, Name = "Player", PosX = -1, PosY = -1 };
        }

        private void SetCollisionTileSetName(string? name)
        {
            collisionTileSetName = name;
            LoadCollisionTileSet();
        }

        private void LoadMap()
        {
            if (!string.IsNullOrEmpty(mapName))
            {
                if (map == null || map.Name != mapName)
                {
                    map = Window?.Content?.Load<Map>(mapName);
                    if (map != null)
                    {
                        if (collisionTileSet != null)
                        {
                            map.CollisionTileSet = collisionTileSet;
                        }
                        actorManager.ContentManager = Window?.Content;
                        actorManager.Map = map;
                        actorManager.Camera = mapRenderer;
                        player = actorManager.Player;
                        eventManager.Map = map;
                        eventManager.Camera = mapRenderer;
                        eventManager.ExecuteOnLoadEvents();

                        mapRenderer.PrepareMap(map);
                        mapRenderer.SetCam(map.Width / 2, map.Height / 2);

                        tooltip = new Tooltip(Graphics);
                        tooltip.Background = Content.Load<IImage>("images/menus/tooltips.png");
                        LoadMapMusic();

                    }
                }
            }
        }

        private void LoadCollisionTileSet()
        {
            if (!string.IsNullOrEmpty(collisionTileSetName))
            {
                if (collisionTileSet == null || collisionTileSet.Name != collisionTileSetName)
                {
                    collisionTileSet = Window?.Content?.Load<TileSet>(collisionTileSetName);
                    if (collisionTileSet != null)
                    {
                        if (map != null)
                        {
                            map.CollisionTileSet = collisionTileSet;
                        }
                    }
                }
            }
        }

        private void LoadMapMusic()
        {
            if (map != null)
            {
                Audio.PlayNow(map.Music, -1);
            }
        }
    }
}
