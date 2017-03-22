using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class MainTabWindow_Assign_Policies : MainTabWindow_Assign
    {
        public override Vector2 RequestedTabSize
        {
            get
            {
                if (Widget_CombatRealism.CombatRealismAvailable)
                {
                    base.BuildPawnList();
                }
                return new Vector2(1050f, 65f + base.PawnsCount * 30f + 65f);
            }
        }

        public override void PreOpen()
        {
            base.PreOpen();
            if (Widget_CombatRealism.CombatRealismAvailable)
            {
                Widget_CombatRealism.BuildPawnList();
            }
            //PrintAllAssignPolicies();
            LoadState(AssignManager.links, pawns, AssignManager.GetActivePolicy());
            CleanDeadMaps();
            CleanDeadColonists(this.pawns);
        }

        public override void PreClose()
        {
            base.PreClose();
            CleanDeadColonists(this.pawns);
            CleanDeadMaps();
            SaveCurrentState(this.pawns);
        }

        public override void DoWindowContents(Rect fillRect)
        {
            if (AssignManager.DirtyPolicy)
            {
                LoadState(AssignManager.links, pawns, AssignManager.GetActivePolicy());
            }


            float num = 5f;
            if (Widget_CombatRealism.CombatRealismAvailable)
            {
                Widget_CombatRealism.DoWindowContents(fillRect);
            }
            else
            {
                base.DoWindowContents(fillRect);
            }

            Rect position = new Rect(0f, 0f, fillRect.width, 65f);

            GUI.BeginGroup(position);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect1 = new Rect(num, -8f, 165f, Mathf.Round(position.height / 3f));
            Widgets.Label(rect1, "BPC.CurrentAssignPolicy".Translate());
            GUI.EndGroup();

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect(num, Mathf.Round(position.height / 4f) - 4f, rect1.width, Mathf.Round(position.height / 4f) + 4f);
            if (Widgets.ButtonText(rect2, AssignManager.GetActivePolicy().label, true, false, true))
            {
                //CleanDeadColonists(this.pawns);
                SaveCurrentState(this.pawns);
                OpenAssignPolicySelectMenu(AssignManager.links, this.pawns);
            }
            num += rect1.width;
            Rect rect3 = new Rect(num, 0f, 20f, Mathf.Round(position.height / 2f));
            if (Widgets.ButtonText(rect3, "", true, false, true))
            {
                Find.WindowStack.Add(new Dialog_ManagePolicies());
            }
            Rect rect4 = new Rect(num + 3f, rect3.height / 4f, 14f, 14f);
            GUI.DrawTexture(rect4, Resources.Settings);
            TooltipHandler.TipRegion(rect4, "BPC.Settings".Translate());
        }

        private static void SaveCurrentState(List<Pawn> pawns)
        {
            int currentMap = Find.VisibleMap.uniqueID;
            //Save current state
            foreach (Pawn p in pawns)
            {
                //find colonist on the current zone in the current map
                AssignLink link = AssignManager.links.Find(x => x.colonist.Equals(p) && x.zone == AssignManager.GetActivePolicy().id && x.mapId == currentMap);

                if (link != null)
                {
                    //colonist found! save 
                    link.outfit = p.outfits.CurrentOutfit;
                    link.drugPolicy = p.drugs.CurrentPolicy;
                    link.hostilityResponse = p.playerSettings.hostilityResponse;
                    if (Widget_CombatRealism.CombatRealismAvailable)
                    {
                        link.loadoutId = Widget_CombatRealism.GetLoadoutId(p);
                    }
                }
                else
                {
                    //colonist not found. So add it to the AssignLink list
                    int loadoutId = 0;
                    if (Widget_CombatRealism.CombatRealismAvailable)
                    {
                        loadoutId = Widget_CombatRealism.GetLoadoutId(p);
                    }
                    AssignManager.links.Add(
                        new AssignLink(
                            AssignManager.GetActivePolicy().id,
                            p,
                            p.outfits.CurrentOutfit,
                            p.drugs.CurrentPolicy,
                            p.playerSettings.hostilityResponse,
                            loadoutId,
                            currentMap));
                }
            }
        }

        private static void CleanDeadColonists(List<Pawn> pawns)
        {
            for (int i = 0; i < AssignManager.links.Count; i++)
            {
                AssignLink pawn = AssignManager.links[i];
                if (!pawns.Contains(pawn.colonist))
                {
                    if (pawn.colonist == null || pawn.colonist.Dead)
                    {
                        AssignManager.links.Remove(pawn);
                    }
                }
            }
        }

        private static void CleanDeadMaps()
        {
            for (int i = 0; i < AssignManager.activePolicies.Count; i++)
            {
                MapActivePolicy map = AssignManager.activePolicies[i];
                if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                {
                    AssignManager.DeleteLinksInMap(map.mapId);
                    AssignManager.DeleteMap(map);
                }
            }
        }

        private static void LoadState(List<AssignLink> links, List<Pawn> pawns, Policy policy)
        {
            List<AssignLink> mapLinks = null;
            List<AssignLink> zoneLinks = null;
            int currentMap = Find.VisibleMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                foreach (AssignLink l in zoneLinks)
                {
                    if (l.colonist != null && l.colonist.Equals(p))
                    {
                        p.outfits.CurrentOutfit = OutfitExits(l.outfit) ? l.outfit : null;
                        p.drugs.CurrentPolicy = DrugPolicyExits(l.drugPolicy) ? l.drugPolicy : null;
                        p.playerSettings.hostilityResponse = l.hostilityResponse;
                        if (Widget_CombatRealism.CombatRealismAvailable)
                        {
                            Widget_CombatRealism.SetLoadoutById(p, l.loadoutId);
                        }
                    }
                }
            }

            AssignManager.SetActivePolicy(policy);
        }

        private static bool OutfitExits(Outfit outfit)
        {
            foreach (Outfit current in Current.Game.outfitDatabase.AllOutfits)
            {
                if (current.Equals(outfit))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool DrugPolicyExits(DrugPolicy drugPolicy)
        {
            foreach (DrugPolicy drug in Current.Game.drugPolicyDatabase.AllPolicies)
            {
                if (drug.Equals(drugPolicy))
                {
                    return true;
                }
            }
            return false;
        }

        private static void OpenAssignPolicySelectMenu(List<AssignLink> links, List<Pawn> pawns)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (Policy assignPolicy in AssignManager.policies)
            {
                list.Add(new FloatMenuOption(assignPolicy.label, delegate { LoadState(links, pawns, assignPolicy); }, MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void PrintAllAssignPolicies()
        {
            Log.Message("[BPC] === List Policies START [" + AssignManager.policies.Count + "] ===");
            foreach (Policy p in AssignManager.policies)
            {
                Log.Message("[BPC]\t" + p.ToString());
            }

            Log.Message("[BPC] === List ActivePolices START [" + AssignManager.activePolicies.Count + "] ===");
            foreach (MapActivePolicy m in AssignManager.activePolicies)
            {
                Log.Message("[BPC]\t" + m.ToString());
            }

            Log.Message("[BPC] === List links START [" + AssignManager.links.Count + "] ===");
            foreach (AssignLink assignLink in AssignManager.links)
            {
                Log.Message("[BPC]\t" + assignLink.ToString());
            }
        }
    }
}