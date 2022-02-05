namespace CDX.Content.Flare
{
    using CDX.Actors;
    using CDX.Graphics;
    using CDX.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ModActorLoader : ModLoader, IResourceLoader<Actor>
    {
        public ModActorLoader() : base("Flare Mod Actor Loader")
        {
        }

        public Actor? Load(string name, byte[]? data)
        {
            Actor? result = null;
            using (FileParser infile = new FileParser(name, data))
            {
                result = LoadActor(infile, name);
                if (result != null)
                {
                    Logger.Info($"Actor loaded from resource '{name}'");
                }
            }
            return result;
        }

        private Actor? LoadActor(FileParser infile, string name)
        {
            Actor? actor = null;
            string actorName = "";
            List<string> categories = new();
            int turnDelay = 0;
            int waypointPause = 60;
            float speed = 3.5f;
            float meleeRange = 1.0f;
            float threatRange = 4.0f;
            float threatRangeFar = 8.0f;
            string? defeatStatus = null;
            IDictionary<string, string> animationParts = new Dictionary<string, string>();
            IDictionary<int, IList<string>> layerOrder = new Dictionary<int, IList<string>>();
            IList<string> voxIntros = new List<string>();
            while (infile.Next())
            {
                switch (infile.Section)
                {
                    case "":
                        switch (infile.Key)
                        {
                            case "name": actorName = infile.GetStrVal(); break;
                            case "categories": break;
                            case "animations": animationParts[""] = infile.GetStrVal(); break;
                            case "gfx": animationParts[""] = infile.GetStrVal(); break;
                            case "gfxpart":
                                string part = infile.PopFirstString();
                                string anim = infile.PopFirstString();
                                animationParts[part] = anim;
                                break;
                            case "layer":
                                int layer = infile.PopFirstInt();
                                List<string> order = new List<string>();
                                string orderPart = infile.PopFirstString();
                                while (!string.IsNullOrEmpty(orderPart))
                                {
                                    order.Add(orderPart);
                                    orderPart = infile.PopFirstString();
                                }
                                layerOrder[layer] = order;
                                break;
                        }
                        break;
                }
                //if (infile.MatchSectionKey("", "name"))
                //{
                //    actorName = infile.GetStrVal();
                //}
                //else if (infile.MatchSectionKey("", "categories"))
                //{
                //    var cat = infile.PopFirstString();
                //    while (!string.IsNullOrEmpty(cat))
                //    {
                //        categories.Add(cat);
                //        cat = infile.PopFirstString();
                //    }
                //}
                ////else if (infile.MatchSectionKey("", "name")) { rarity = infile.PopFirstString(); }
                ////else if (infile.MatchSectionKey("", "level")) { level = infile.PopFirstInt(); }
                //else if (infile.MatchSectionKey("", "speed")) { speed = infile.PopFirstFloat(); }
                //else if (infile.MatchSectionKey("", "melee_range")) { meleeRange = infile.PopFirstFloat(); }
                //else if (infile.MatchSectionKey("", "threat_range"))
                //{
                //    //threatRange = infile.GetFirstFloatVal(threatRange);
                //    //threatRangeFar = infile.GetFirstFloatVal(threatRange * 2);
                //}
                //else if (infile.MatchSectionKey("", "waypoint_pause")) { waypointPause = FileParser.ParseDurationMS(infile.GetStrVal()); }
                //else if (infile.MatchSectionKey("", "turn_delay")) { turnDelay = FileParser.ParseDurationMS(infile.GetStrVal()); }
                //else if (infile.MatchSectionKey("", "vox_intro")) { voxIntros.Add(infile.PopFirstString()); }
                //else if (infile.MatchSectionKey("", "animations")) { animationParts[""] = infile.PopFirstString(); }
                //else if (infile.MatchSectionKey("", "gfx")) { animationParts[""] = infile.PopFirstString(); }
                //else if (infile.MatchSectionKey("", "gfxpart"))
                //{
                //    string part = infile.PopFirstString();
                //    string anim = infile.PopFirstString();
                //    animationParts[part] = anim;
                //}
                //else if (infile.MatchSectionKey("", "layer"))
                //{
                //    int layer = infile.PopFirstInt();
                //    List<string> order = new List<string>();
                //    string orderPart = infile.PopFirstString();
                //    while (!string.IsNullOrEmpty(orderPart))
                //    {
                //        order.Add(orderPart);
                //        orderPart = infile.PopFirstString();
                //    }
                //    layerOrder[layer] = order;
                //}
                //else if (infile.MatchSectionKey("", "sfx_attack"))
                //{
                //    string animName = infile.PopFirstString();
                //    string sfxName = infile.PopFirstString();
                //    //IList<string> sfxList;
                //    //if (!sfx.SfxAttack.TryGetValue(animName, out sfxList))
                //    //{
                //    //    sfxList = new List<string>();
                //    //    sfx.SfxAttack[animName] = sfxList;
                //    //}
                //    //sfxList.Add(sfxName);
                //}
                //else if (infile.MatchSectionKey("", "sfx_hit"))
                //{
                //    //sfx.SfxHit.Add(infile.PopFirstString());
                //}
                //else if (infile.MatchSectionKey("", "sfx_die"))
                //{
                //    //sfx.SfxDie.Add(infile.PopFirstString());
                //}
                //else if (infile.MatchSectionKey("", "sfx_critdie"))
                //{
                //    //sfx.SfxCritDie.Add(infile.PopFirstString());
                //}
                //else if (infile.MatchSectionKey("", "sfx_block"))
                //{
                //    //sfx.SfxBlock.Add(infile.PopFirstString());
                //}
                //else if (infile.MatchSectionKey("", "defeat_status"))
                //{
                //    defeatStatus = infile.PopFirstString();
                //}
            }
            if (animationParts.Count > 0)
            {
                //entity = new Entity(context.Application.EntityManager, entityName);
                actor = new Actor(name);
                actor.DisplayName = actorName;
                //entity.TurnDelay = turnDelay;
                //entity.WaypointPause = waypointPause;
                //entity.DefaultSpeed = speed;
                //entity.Speed = speed;
                //entity.MeleeRange = meleeRange;
                //entity.ThreatRange = threatRange;
                //entity.ThreatRangeFar = threatRangeFar;
                //entity.DefeatStatus = defeatStatus;
                //entity.AssignSounds(sfx);
                //foreach (var vox in voxIntros) { entity.VoxIntros.Add(vox); }
                if (animationParts.Count > 1 && layerOrder.Count >= animationParts.Count)
                {
                    var animSets = new Dictionary<string, AnimationSet>();
                    foreach (var kvp in animationParts)
                    {
                        AnimationSet? animSet = ContentManager?.Load<AnimationSet>(kvp.Value);
                        if (animSet != null)
                        {
                            animSets[kvp.Key] = animSet;
                        }
                    }
                    actor.Visual = new MultiPartVisual(animSets, layerOrder);
                }
                else
                {
                    foreach (var kvp in animationParts)
                    {
                        AnimationSet? animSet = ContentManager?.Load<AnimationSet>(kvp.Value);
                        if (animSet != null)
                        {
                            actor.Visual = new AnimationSetVisual(animSet);
                            break;
                        }
                    }
                }
            }
            return actor;
        }
    }
}
