using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;

/// <summary>
/// Adds the ability to create profiles for the Animal Tab
/// <author>Alexandre Brito</author>
/// </summary>

namespace BetterPawnControl
{
    /// <summary>
    /// Enherit everthing from vanilla and avoid breaking it at all costs
    /// </summary>

    public class MainTabWindow_Animals_Policies : MainTabWindow_Animals
    {

        public override void PreOpen()
        {
            base.PreOpen();
            ////PrintAllAnimalPolicies();
            LoadState(AnimalManager.links, this.Pawns.ToList(), AnimalManager.GetActivePolicy());
            CleanDeadMaps();
            CleanDeadAnimals(this.Pawns.ToList());

        }

        public override void PreClose()
        {
            base.PreClose();
            CleanDeadMaps();
            CleanDeadAnimals(this.Pawns.ToList());
            SaveCurrentState(this.Pawns.ToList());
        }

        /// <summary>
        /// Draw base and two new buttons!
        /// </summary>
        public override void DoWindowContents(Rect fillRect)
        {
            if (AnimalManager.DirtyPolicy)
            {
                LoadState(AnimalManager.links, this.Pawns.ToList(), AnimalManager.GetActivePolicy());
            }

            float num = 5f;
            base.DoWindowContents(fillRect);

            Rect position = new Rect(0f, 0f, fillRect.width, 65f);

            GUI.BeginGroup(position);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect1 = new Rect(num, -8f, 165f, Mathf.Round(position.height / 3f));
            Widgets.Label(rect1, "BPC.CurrentAnimalPolicy".Translate());
            GUI.EndGroup();

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect(num, Mathf.Round(position.height / 4f) - 4f, rect1.width, Mathf.Round(position.height / 4f) + 4f);
            if (Widgets.ButtonText(rect2, AnimalManager.GetActivePolicy().label, true, false, true))
            {
                SaveCurrentState(this.Pawns.ToList());
                OpenAnimalPolicySelectMenu(AnimalManager.links, this.Pawns.ToList());
            }
            num += rect1.width;
            Rect rect3 = new Rect(num, 0f, 20f, Mathf.Round(position.height / 2f));
            if (Widgets.ButtonText(rect3, "", true, false, true))
            {
                Find.WindowStack.Add(new Dialog_ManagePolicies(Find.VisibleMap));
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
                //find animal on the current zone
                AnimalLink animalLink = AnimalManager.links.Find(x => x.animal.Equals(p) && x.zone == AnimalManager.GetActivePolicy().id && x.mapId == currentMap);

                if (animalLink != null)
                {
                    //animal found! save master and area
                    animalLink.master = p.playerSettings.master;
                    animalLink.area = p.playerSettings.AreaRestriction;
                    animalLink.followDrafted = p.playerSettings.followDrafted;
                    animalLink.followFieldwork = p.playerSettings.followFieldwork;
                }
                else
                {
                    //animal not found. So add it to the AnimalLink list
                    AnimalManager.links.Add(new AnimalLink(
                        AnimalManager.GetActivePolicy().id,
                        p,
                        p.playerSettings.master,
                        p.playerSettings.AreaRestriction,
                        p.playerSettings.followDrafted,
                        p.playerSettings.followFieldwork,
                        currentMap));
                }
            }
        }

        /// <summary>
        /// Clean all animal related links if animal is dead
        /// </summary>
        private static void CleanDeadAnimals(List<Pawn> pawns)
        {
            for (int i = 0; i < AnimalManager.links.Count; i++)
            {
                AnimalLink pawn = AnimalManager.links[i];
                if (!pawns.Contains(pawn.animal))
                {
                    if (pawn.animal == null || pawn.animal.Dead)
                    {
                        AnimalManager.links.Remove(pawn);
                    }
                }
            }
        }

        /// <summary>
        /// Get and set all links from a AnimalPolicy 
        /// </summary>
        private static void LoadState(List<AnimalLink> links, List<Pawn> pawns, Policy policy)
        {
            List<AnimalLink> mapLinks = null;
            List<AnimalLink> zoneLinks = null;
            int currentMap = Find.VisibleMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                foreach (AnimalLink l in zoneLinks)
                {
                    if (l.animal != null && l.animal.Equals(p))
                    {
                        //found animal in zone. Update master if alive
                        p.playerSettings.master = (l.master != null && l.master.Dead) ? null : l.master;
                        p.playerSettings.AreaRestriction = l.area;
                        p.playerSettings.followDrafted = l.followDrafted;
                        p.playerSettings.followFieldwork = l.followFieldwork;
                    }
                }
            }
            AnimalManager.SetActivePolicy(policy);
        }

        /// <summary>
        /// Open select AnimalPolicy menu
        /// </summary>
        private static void OpenAnimalPolicySelectMenu(List<AnimalLink> links, List<Pawn> pawns)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (Policy animalPolicy in AnimalManager.policies)
            {
                list.Add(new FloatMenuOption(animalPolicy.label, delegate { LoadState(links, pawns, animalPolicy); }, MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void CleanDeadMaps()
        {
            for (int i = 0; i < AnimalManager.activePolicies.Count; i++)
            {
                MapActivePolicy map = AnimalManager.activePolicies[i];
                if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                {
                    AnimalManager.DeleteLinksInMap(map.mapId);
                    AnimalManager.DeleteMap(map);
                }
            }
        }

        private static void PrintAllAnimalPolicies()
        {
            Log.Message("[BPC] ### List Animal Policies [" + AnimalManager.policies.Count + "] ###");
            foreach (Policy p in AnimalManager.policies)
            {
                Log.Message("[BPC]\t" + p.ToString());
            }

            Log.Message("[BPC] ### List ActivePolices START [" + AnimalManager.activePolicies.Count + "] ===");
            foreach (MapActivePolicy m in AnimalManager.activePolicies)
            {
                Log.Message("[BPC]\t" + m.ToString());
            }

            Log.Message("[BPC] ### List Animal links [" + AnimalManager.links.Count + "] ###");
            foreach (AnimalLink l in AnimalManager.links)
            {
                Log.Message("[BPC]\t" + l.ToString());
            }
        }
    }
}