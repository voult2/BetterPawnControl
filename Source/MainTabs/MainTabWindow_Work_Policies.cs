using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;

namespace BetterPawnControl
{
    class MainTabWindow_Work_Policies : MainTabWindow_Work
    {

        public override void PreOpen()
        {
            base.PreOpen();

            UpdateState(
                WorkManager.links, this.Pawns.ToList(),
                WorkManager.GetActivePolicy());

            LoadState(
                WorkManager.links, this.Pawns.ToList(),
                WorkManager.GetActivePolicy());

            CleanDeadMaps();

            CleanDeadColonists(this.Pawns.ToList());
        }

        public override void PreClose()
        {
            base.PreClose();
            CleanDeadMaps();
            CleanDeadColonists(this.Pawns.ToList());
            SaveCurrentState(this.Pawns.ToList());
        }

        public override void DoWindowContents(Rect fillRect)
        {
            if (WorkManager.DirtyPolicy)
            {
                LoadState(
                    WorkManager.links, this.Pawns.ToList(),
                    WorkManager.GetActivePolicy());
                WorkManager.DirtyPolicy = false;
            }

            float offsetX = 200f;
            base.DoWindowContents(fillRect);
            Rect position = new Rect(0f, 0f, fillRect.width, 65f);

            GUI.BeginGroup(position);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect1 =
                new Rect(offsetX, -8f, 165f, Mathf.Round(position.height / 3f));
            Widgets.Label(rect1, "BPC.CurrentWorkPolicy".Translate());
            GUI.EndGroup();

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect(
                offsetX, Mathf.Round(position.height / 4f) - 4f,
                rect1.width, Mathf.Round(position.height / 4f) + 4f);

            if (Widgets.ButtonText(
                rect2, WorkManager.GetActivePolicy().label,
                true, false, true))
            {
                //CleanDeadColonists(this.pawns);
                SaveCurrentState(this.Pawns.ToList());
                OpenWorkPolicySelectMenu(
                    WorkManager.links, this.Pawns.ToList());
            }
            offsetX += rect1.width;
            Rect rect3 = new Rect(
                offsetX, 0f, 20f, Mathf.Round(position.height / 2f));
            if (Widgets.ButtonText(rect3, "", true, false, true))
            {
                Find.WindowStack.Add(
                    new Dialog_ManagePolicies(Find.CurrentMap));
            }
            Rect rect4 = new Rect(offsetX + 3f, rect3.height / 4f, 14f, 14f);
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
                WorkLink link = WorkManager.links.Find(
                    x => x.colonist.Equals(p) &&
                    x.zone == WorkManager.GetActivePolicy().id &&
                    x.mapId == currentMap);

                if (link != null)
                {
                    //colonist found! save                    
                    WorkManager.SavePawnPriorities(p, link);
                }
                else
                {
                    //colonist not found. So add it to the WorkLink list
                    link = new WorkLink(
                        WorkManager.GetActivePolicy().id,
                        p,
                        new Dictionary<WorkTypeDef, int>(),
                        currentMap);
                    WorkManager.links.Add(link);
                    WorkManager.SavePawnPriorities(p, link);

                }
            }
        }

        private static void CleanDeadColonists(List<Pawn> pawns)
        {
            for (int i = 0; i < WorkManager.links.Count; i++)
            {
                WorkLink pawn = WorkManager.links[i];
                if (!pawns.Contains(pawn.colonist))
                {
                    if (pawn.colonist == null || pawn.colonist.Dead)
                    {
                        WorkManager.links.Remove(pawn);
                    }
                }
            }
        }

        private static void CleanDeadMaps()
        {
            for (int i = 0; i < WorkManager.activePolicies.Count; i++)
            {
                MapActivePolicy map = WorkManager.activePolicies[i];
                if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                {
                    WorkManager.DeleteLinksInMap(map.mapId);
                    WorkManager.DeleteMap(map);
                }
            }
        }

        private static void UpdateState(
            List<WorkLink> links, List<Pawn> pawns, Policy policy)
        {
            List<WorkLink> mapLinks = null;
            List<WorkLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                foreach (WorkLink l in zoneLinks)
                {
                    if (l.colonist != null && l.colonist.Equals(p))
                    {                        
                        WorkManager.LoadPawnPriorities(p, l);
                    }
                }
            }

            WorkManager.SetActivePolicy(policy);
        }

        private static void LoadState(
            List<WorkLink> links, List<Pawn> pawns, Policy policy)
        {
            List<WorkLink> mapLinks = null;
            List<WorkLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                foreach (WorkLink l in zoneLinks)
                {
                    if (l.colonist != null && l.colonist.Equals(p))
                    {
                        WorkManager.LoadPawnPriorities(p, l);
                    }
                }
            }

            WorkManager.SetActivePolicy(policy);
        }

        private static void OpenWorkPolicySelectMenu(
            List<WorkLink> links, List<Pawn> pawns)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (Policy workPolicy in WorkManager.policies)
            {
                list.Add(
                    new FloatMenuOption(
                        workPolicy.label,
                        delegate
                        {
                            LoadState(
                                links,
                                pawns,
                                workPolicy);
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void PrintAllWorkPolicies()
        {
            Log.Message("[BPC] === List Work Policies START [" +
                WorkManager.policies.Count +
                "] ===");
            foreach (Policy p in WorkManager.policies)
            {
                Log.Message("[BPC]\t" + p.ToString());
            }

            Log.Message("[BPC] === List Work ActivePolices START [" +
                WorkManager.activePolicies.Count +
                "] ===");
            foreach (MapActivePolicy m in WorkManager.activePolicies)
            {
                Log.Message("[BPC]\t" + m.ToString());
            }

            Log.Message("[BPC] === List Work links START [" +
                WorkManager.links.Count +
                "] ===");
            foreach (WorkLink WorkLink in WorkManager.links)
            {
                Log.Message("[BPC]\t" + WorkLink.ToString());
            }
        }
    }
}