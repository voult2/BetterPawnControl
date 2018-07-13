using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;

namespace BetterPawnControl
{
    class MainTabWindow_Restrict_Policies : MainTabWindow_Restrict
    {

        public override void PreOpen()
        {
            base.PreOpen();
            //PrintAllAssignPolicies();

            UpdateState(
                RestrictManager.links, this.Pawns.ToList(),
                RestrictManager.GetActivePolicy());

            LoadState(
                RestrictManager.links, this.Pawns.ToList(), 
                RestrictManager.GetActivePolicy());

            CleanDeadMaps();

            CleanDeadColonists(this.Pawns.ToList());
        }

        public override void PreClose()
        {
            base.PreClose();
            CleanDeadColonists(this.Pawns.ToList());
            CleanDeadMaps();
            SaveCurrentState(this.Pawns.ToList());
        }

        public override void DoWindowContents(Rect fillRect)
        {
            if (RestrictManager.DirtyPolicy)
            {
                LoadState(
                    RestrictManager.links, this.Pawns.ToList(), 
                    RestrictManager.GetActivePolicy());
                RestrictManager.DirtyPolicy = false;
            }


            float num = 200f;
            base.DoWindowContents(fillRect);

            Rect position = new Rect(0f, 0f, fillRect.width, 65f);

            GUI.BeginGroup(position);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect1 = 
                new Rect(num, -8f, 165f, Mathf.Round(position.height / 3f));
            Widgets.Label(rect1, "BPC.CurrentRestrictPolicy".Translate());
            GUI.EndGroup();

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect(
                num, Mathf.Round(position.height / 4f) - 4f, 
                rect1.width, Mathf.Round(position.height / 4f) + 4f);
            if (Widgets.ButtonText(
                rect2, RestrictManager.GetActivePolicy().label, 
                true, false, true))
            {
                //CleanDeadColonists(this.pawns);
                SaveCurrentState(this.Pawns.ToList());
                OpenRestrictPolicySelectMenu(
                    RestrictManager.links, this.Pawns.ToList());
            }
            num += rect1.width;
            Rect rect3 = new Rect(
                num, 0f, 20f, Mathf.Round(position.height / 2f));
            if (Widgets.ButtonText(rect3, "", true, false, true))
            {
                Find.WindowStack.Add(
                    new Dialog_ManagePolicies(Find.CurrentMap));
            }
            Rect rect4 = new Rect(num + 3f, rect3.height / 4f, 14f, 14f);
            GUI.DrawTexture(rect4, Resources.Settings);
            TooltipHandler.TipRegion(rect4, "BPC.Settings".Translate());
        }

        private static void SaveCurrentState(List<Pawn> pawns)
        {
            int currentMap = Find.CurrentMap.uniqueID;
            //Save current state
            foreach (Pawn p in pawns)
            {
                //find colonist on the current zone in the current map
                RestrictLink link = RestrictManager.links.Find(
                    x => x.colonist.Equals(p) && 
                    x.zone == RestrictManager.GetActivePolicy().id && 
                    x.mapId == currentMap);

                if (link != null)
                {
                    //colonist found! save 
                    link.area = p.playerSettings.AreaRestriction;
                }
                else
                {
                    //colonist not found. So add it to the RestrictLink list
                    RestrictManager.links.Add(
                        new RestrictLink(
                            RestrictManager.GetActivePolicy().id,
                            p,
                            p.playerSettings.AreaRestriction,
                            currentMap));
                }
            }
        }

        private static void CleanDeadColonists(List<Pawn> pawns)
        {
            for (int i = 0; i < RestrictManager.links.Count; i++)
            {
                RestrictLink pawn = RestrictManager.links[i];
                if (!pawns.Contains(pawn.colonist))
                {
                    if (pawn.colonist == null || pawn.colonist.Dead)
                    {
                        RestrictManager.links.Remove(pawn);
                    }
                }
            }
        }

        private static void CleanDeadMaps()
        {
            for (int i = 0; i < RestrictManager.activePolicies.Count; i++)
            {
                MapActivePolicy map = RestrictManager.activePolicies[i];
                if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                {
                    RestrictManager.DeleteLinksInMap(map.mapId);
                    RestrictManager.DeleteMap(map);
                }
            }
        }

        private static void UpdateState(
            List<RestrictLink> links, List<Pawn> pawns, Policy policy)
        {
            List<RestrictLink> mapLinks = null;
            List<RestrictLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                foreach (RestrictLink l in zoneLinks)
                {
                    if (l.colonist != null && l.colonist.Equals(p))
                    {
                        l.area = p.playerSettings.AreaRestriction;
                    }
                }
            }

            RestrictManager.SetActivePolicy(policy);
        }

        private static void LoadState(
            List<RestrictLink> links, List<Pawn> pawns, Policy policy)
        {
            List<RestrictLink> mapLinks = null;
            List<RestrictLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                foreach (RestrictLink l in zoneLinks)
                {
                    if (l.colonist != null && l.colonist.Equals(p))
                    {
                        p.playerSettings.AreaRestriction = l.area;
                    }
                }
            }

            RestrictManager.SetActivePolicy(policy);
        }

        private static void OpenRestrictPolicySelectMenu(
            List<RestrictLink> links, List<Pawn> pawns)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (Policy restrictPolicy in RestrictManager.policies)
            {
                list.Add(
                    new FloatMenuOption(
                        restrictPolicy.label, 
                        delegate 
                        {
                            LoadState(
                                links, 
                                pawns, 
                                restrictPolicy);
                        }, 
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void PrintAllAssignPolicies()
        {
            Log.Message("[BPC] === List Policies START [" + 
                RestrictManager.policies.Count + 
                "] ===");
            foreach (Policy p in RestrictManager.policies)
            {
                Log.Message("[BPC]\t" + p.ToString());
            }

            Log.Message("[BPC] === List ActivePolices START [" + 
                RestrictManager.activePolicies.Count + 
                "] ===");
            foreach (MapActivePolicy m in RestrictManager.activePolicies)
            {
                Log.Message("[BPC]\t" + m.ToString());
            }

            Log.Message("[BPC] === List links START [" + 
                RestrictManager.links.Count + 
                "] ===");
            foreach (RestrictLink RestrictLink in RestrictManager.links)
            {
                Log.Message("[BPC]\t" + RestrictLink.ToString());
            }
        }
    }
}