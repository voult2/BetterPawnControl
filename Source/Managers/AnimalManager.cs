using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class AnimalManager : Manager<AnimalLink>
    {
        internal static void DeletePolicy(Policy policy)
        {
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
                if (p == null)
                {
                    continue;
                }

                try
                {
                    //find animal on the current zone
                    AnimalLink animalLink =
                        AnimalManager.links.Find(
                            x => x != null && x.animal != null && p != null && p.Equals(x.animal) &&
                            x.zone == AnimalManager.GetActivePolicy().id &&
                            x.mapId == currentMap);

                    if (animalLink != null)
                    {
                        //animal found! save master and area
                        animalLink.master = p.playerSettings.Master;
                        animalLink.area = p.playerSettings.AreaRestrictionInPawnCurrentMap;
                        animalLink.followDrafted = p.playerSettings.followDrafted;
                        animalLink.followFieldwork = p.playerSettings.followFieldwork;
                        if (Widget_ModsAvailable.AAFAvailable)
                        {
                            animalLink.foodPolicy = p.foodRestriction.CurrentFoodPolicy;
                        }
                    }
                    else
                    {
                        FoodPolicy food = null;
                        if (Widget_ModsAvailable.AAFAvailable)
                        {
                            food = p.foodRestriction.CurrentFoodPolicy;
                            if (food == Current.Game.foodRestrictionDatabase.DefaultFoodRestriction())
                            {
                                food = AnimalManager.DefaultFoodPolicy;
                            }
                        }

                        //animal not found. So add it to the AnimalLink list
                        AnimalManager.links.Add(
                            new AnimalLink(
                                AnimalManager.GetActivePolicy().id,
                                p,
                                p.playerSettings.Master,
                                p.playerSettings.AreaRestrictionInPawnCurrentMap,
                                p.playerSettings.followDrafted,
                                p.playerSettings.followFieldwork,
                                food,
                                currentMap));
                    }
                }
                catch
                {
                    //it seems a pawn became null during the links iteration so lets just move on
                    Log.Message("BPC: A pawn became null during animal save state: " + p == null);
                }
            }
        }

        /// <summary>
        /// Clean all animal related links if animal is dead
        /// </summary>
        internal static void CleanDeadAnimals(Pawn pawn)
        {
            AnimalManager.links.RemoveAll(x => x.animal == pawn);
        }

        internal static void LinksCleanUp()
        {
            for (int i = AnimalManager.links.Count - 1; i >= 0; i--)
            {
                if (AnimalManager.links[i].animal == null || AnimalManager.links[i].animal.Faction == null)
                {
                    AnimalManager.links.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Get and set all links from an AnimalPolicy 
        /// </summary>
        internal static void LoadState(List<AnimalLink> links, List<Pawn> pawns, Policy policy)
        {
            List<AnimalLink> mapLinks = null;
            List<AnimalLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x != null && x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x != null && x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                if (p == null)
                {
                    continue;
                }

                foreach (AnimalLink l in zoneLinks)
                {
                    if (l.animal != null && l.animal.Equals(p))
                    {
                        //found animal in zone. Update master if alive
                        p.playerSettings.Master = (l.master != null && l.master.Dead) ? null : l.master;
                        p.playerSettings.AreaRestrictionInPawnCurrentMap = l.area;
                        p.playerSettings.followDrafted = l.followDrafted;
                        p.playerSettings.followFieldwork = l.followFieldwork;
                        if (Widget_ModsAvailable.AAFAvailable)
                        {
                            p.foodRestriction.CurrentFoodPolicy = FoodPolicyExists(l.foodPolicy) ?
                                l.foodPolicy : null;
                        }
                    }
                }
            }
            AnimalManager.SetActivePolicy(policy);
        }

        internal static void LoadState(Policy policy)
        {
            //IEnumerable<Pawn> pawns = 
            //    from p in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer)
            //    where p.RaceProps.Animal
            //    select p;

            LoadState(AnimalManager.links, Animals().ToList(), policy);
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

            //get all links from the current map
            mapLinks = links.FindAll(l => l != null && l.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(ml => ml != null && ml.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                foreach (AnimalLink l in zoneLinks)
                {
                    if (l.animal != null && l.animal.GetUniqueLoadID().Equals(p.GetUniqueLoadID()))
                    {
                        l.master = p.playerSettings.Master;
                        l.followDrafted = p.playerSettings.followDrafted;
                        l.followFieldwork = p.playerSettings.followFieldwork;
                        l.area = p.playerSettings.AreaRestrictionInPawnCurrentMap;
                        if (Widget_ModsAvailable.AAFAvailable)
                        {
                            l.foodPolicy = p.foodRestriction.CurrentFoodPolicy;
                        }
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

        internal static void CleanRemovedMaps(Map map)
        {
            //for (int i = 0; i < AnimalManager.activePolicies.Count; i++)
            //{
            //    MapActivePolicy map = AnimalManager.activePolicies[i];
            //    if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
            //    {
            //        if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
            //        {
            //            if (Find.Maps.Count == 1 && !AnimalManager.ActivePoliciesContainsValidMap())
            //            {
            //                //this means the player was on the move without any base
            //                //and just re-settled. So, let's move the settings to
            //                //the new map
            //                int newMapId = Find.CurrentMap.uniqueID;
            //                AnimalManager.MoveLinksToMap(map.mapId, newMapId);
            //                map.mapId = newMapId;
            //            }
            //            else
            //            {
            //                AnimalManager.DeleteLinksInMap(map.mapId);
            //                AnimalManager.DeleteMap(map);
            //            }
            //        }
            //    }
            //}
            if (!map.IsPlayerHome)
            {
                AnimalManager.DeleteLinksInMap(map.uniqueID);
                MapActivePolicy mapActivePolicy = AnimalManager.GetActiveMap(map.uniqueID);
                AnimalManager.DeleteMap(mapActivePolicy);
            }
        }
        internal static void ProcessNewMap(Map newMap)
        {
            if (Find.Maps.Count > 1)
            {
                //Player has a base and arrived at a new map or got in a incident in a new map with caravan
                //BCP will create a new map in the MapActivePolicy list. Nothing to do here.

            }
            else if (Find.Maps.Count == 1)
            {
                //Player has no base and just arrived in a new map via caravan or via GravShip.
                //So let us move all links from the old and last map and then delete the old map
                if (!AnimalManager.ActivePoliciesContainsValidMap())
                {
                    AnimalManager.MoveLinksToMap(LastMapManager.lastMapId, newMap.uniqueID);
                }
            }
            else
            {
                //this makes no sense
                Log.Warning("[BPC] This code shouldn't have ran");
            }
        }

        internal static IEnumerable<Pawn> Animals()
        {
            try
            {
                return from p in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer)
                       where p.RaceProps.Animal
                       select p;
            }
            catch (Exception)
            {
                //following players reports, if a settlement is abandoned the previous statement fails with a null reference, so lets return an empty list 
                return new List<Pawn>();
            }
        }


        internal static void PrintAllAnimalPolicies(string spacer = "\n")
        {
            Log.Message("[BPC] ### List Animal Policies [" + AnimalManager.policies.Count + "] ###");
            foreach (Policy p in AnimalManager.policies)
            {
                Log.Message("[BPC]\t" + p.ToString());
            }

            Log.Message("[BPC] ### Animal ActivePolices START [" + AnimalManager.activePolicies.Count + "] ===");
            foreach (MapActivePolicy m in AnimalManager.activePolicies)
            {
                Log.Message("[BPC]\t" + m.ToString());
            }

            Log.Message("[BPC] ### List Animal links [" + AnimalManager.links.Count + "] ###");
            foreach (AnimalLink l in AnimalManager.links)
            {
                Log.Message("[BPC]\t" + l);
            }

            Log.Message(spacer);
        }

    }
}