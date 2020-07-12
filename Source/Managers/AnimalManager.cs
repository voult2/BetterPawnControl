using System;
using Verse;
using RimWorld;
using System.Collections.Generic;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class AnimalManager : Manager<AnimalLink>
    {
        internal static void DeletePolicy(Policy policy)
        {
            //delete if not default AssignPolicy
            if (policy != null && policy.id > 0)
            {
                links.RemoveAll(x => x.zone == policy.id);
                policies.Remove(policy);
                int mapId = Find.CurrentMap.uniqueID;
                foreach (MapActivePolicy m in activePolicies)
                {
                    if (m.activePolicy.id == policy.id)
                    {
                        m.activePolicy = policies[0];
                        DirtyPolicy = true;
                    }
                }
            }
        }

        internal static void DeleteLinksInMap(int mapId)
        {
            links.RemoveAll(x => x.mapId == mapId);
        }

        internal static void DeleteMap(MapActivePolicy map)
        {
            activePolicies.Remove(map);
        }

        internal static void SaveCurrentState(List<Pawn> pawns)
        {
			int currentMap = Find.CurrentMap.uniqueID;
            //Save current state
            foreach (Pawn p in pawns)
            {
                //find animal on the current zone
                AnimalLink animalLink =
                    AnimalManager.links.Find(
                        x => x != null && p.Equals(x.animal) &&
                        x.zone == AnimalManager.GetActivePolicy().id &&
                        x.mapId == currentMap);

                if (animalLink != null)
                {
                    //animal found! save master and area
                    animalLink.master =
                        p.playerSettings.Master;
                    animalLink.area =
                        p.playerSettings.AreaRestriction;
                    animalLink.followDrafted =
                        p.playerSettings.followDrafted;
                    animalLink.followFieldwork =
                        p.playerSettings.followFieldwork;
                }
                else
                {
                    //animal not found. So add it to the AnimalLink list
                    AnimalManager.links.Add(new AnimalLink(
                        AnimalManager.GetActivePolicy().id,
                        p,
                        p.playerSettings.Master,
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
        internal static void CleanDeadAnimals(List<Pawn> pawns)
        {
            AnimalManager.links.RemoveAll(l => l == null);
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
        /// Get and set all links from an AnimalPolicy 
        /// </summary>
        internal static void LoadState(
            List<AnimalLink> links, List<Pawn> pawns, Policy policy)
        {
            List<AnimalLink> mapLinks = null;
            List<AnimalLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x != null && x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x != null &&  x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                foreach (AnimalLink l in zoneLinks)
                {
                    if (l.animal != null && l.animal.Equals(p))
                    {
                        //found animal in zone. Update master if alive
                        p.playerSettings.Master =
                            (l.master != null && l.master.Dead) ?
                                null : l.master;
                        p.playerSettings.AreaRestriction = l.area;
                        p.playerSettings.followDrafted = l.followDrafted;
                        p.playerSettings.followFieldwork = l.followFieldwork;
                    }
                }
            }
            AnimalManager.SetActivePolicy(policy);
        }

        /// <summary>
        /// Updates pawn area and master in case it was changed via inspector
        /// window
        /// </summary>
        internal static void UpdateState(
            List<AnimalLink> links, List<Pawn> pawns, Policy policy)
        {
            List<AnimalLink> mapLinks = null;
            List<AnimalLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;
            if (links == null) throw new ArgumentNullException(nameof(links));
            if (policy == null) throw new ArgumentNullException(nameof(policy));

            //get all links from the current map
            mapLinks = links.FindAll(l => l != null && l.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(ml => ml != null && ml.zone == policy.id);
            var brokenLinks = links.FindAll(x => x == null).CountAllowNull();
            if (brokenLinks > 0) Log.Warning("[BPC] AnimalManager.UpdateState was given " + brokenLinks + " dead links");

            foreach (Pawn p in pawns)
            {
                foreach (AnimalLink l in zoneLinks)
                {
                    if (l.animal != null && l.animal.Equals(p))
                    {
                        l.master = p.playerSettings.Master;
                        l.area = p.playerSettings.AreaRestriction;
                    }
                }
            }
            AnimalManager.SetActivePolicy(policy);
        }

        internal static bool ActivePoliciesContainsValidMap()
        {
            bool containsValidMap = false;
            foreach (Map map in Find.Maps)
            {
                if (AnimalManager.activePolicies.Any(x => x.mapId == map.uniqueID))
                {
                    containsValidMap = true;
                    break;
                }
            }
            return containsValidMap;
        }

        internal static void CleanDeadMaps()
        {
            for (int i = 0; i < AnimalManager.activePolicies.Count; i++)
            {
                MapActivePolicy map = AnimalManager.activePolicies[i];
                if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                {
                    if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                    {
                        if (Find.Maps.Count == 1 && !AnimalManager.ActivePoliciesContainsValidMap())
                        {
                            //this means the player was on the move without any base
                            //and just re-settled. So, let's move the settings to
                            //the new map
                            int mapid = Find.CurrentMap.uniqueID;
                            AnimalManager.MoveLinksToMap(mapid);
                            map.mapId = mapid;
                        }
                        else
                        {
                            AnimalManager.DeleteLinksInMap(map.mapId);
                            AnimalManager.DeleteMap(map);
                        }
                    }
                }
            }
        }

        internal static void PrintAllAnimalPolicies(string spacer = "\n")
        {
            Log.Message(
                "[BPC] ### List Animal Policies [" +
                AnimalManager.policies.Count +
                "] ###");
            foreach (Policy p in AnimalManager.policies)
            {
                Log.Message("[BPC]\t" + p.ToString());
            }

            Log.Message("[BPC] ### Animal ActivePolices START ["
                + AnimalManager.activePolicies.Count +
                "] ===");
            foreach (MapActivePolicy m in AnimalManager.activePolicies)
            {
                Log.Message("[BPC]\t" + m.ToString());
            }

            Log.Message("[BPC] ### List Animal links ["
                + AnimalManager.links.Count
                + "] ###");
            foreach (AnimalLink l in AnimalManager.links)
            {
                Log.Message("[BPC]\t" + l);
            }

            Log.Message(spacer);
        }

    }
}