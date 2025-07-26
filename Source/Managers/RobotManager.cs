using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class RobotManager : Manager<RobotLink>
    {
        internal static IEnumerable<Pawn> Robots()
        {
            try
            {
                var robots = from pawn in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer)
                             where Widget_MiscRobots.IsPawnRobot(pawn)
                             select pawn;
                return robots;
            }
            catch
            {
                return new List<Pawn>();
            }
        }

        internal static void LoadState(Policy policy)
        {
            LoadState(links, Robots().ToList(), policy);
        }

        internal static void LoadState(List<RobotLink> links, List<Pawn> pawns, Policy policy)
        {
            var currentMap = Find.CurrentMap.uniqueID;
            var mapLinks = links.FindAll(x => x != null && x.mapId == currentMap);
            var zoneLinks = mapLinks.FindAll(x => x != null && x.zone == policy.id);
            
            foreach (var pawn in pawns)
            {
                if (pawn == null)
                {
                    continue;
                }

                foreach (var robotLink in zoneLinks)
                {
                    if (robotLink.robot != pawn) continue;

                    pawn.playerSettings.AreaRestrictionInPawnCurrentMap = robotLink.area;
                }
            }

            SetActivePolicy(policy);
        }

        internal static void SaveCurrentState(List<Pawn> pawns)
        {
            int currentMap = Find.CurrentMap.uniqueID;
            foreach (Pawn p in pawns)
            {
                if (p == null)
                {
                    continue;
                }

                try
                {
                    // find robot in the current zone
                    RobotLink link = links.Find(
                        x => x != null && p != null && p.Equals(x.robot) &&
                        x.zone == GetActivePolicy().id &&
                        x.mapId == currentMap);

                    if (link != null)
                    {
                        link.area = p.playerSettings.AreaRestrictionInPawnCurrentMap;
                    }
                    else
                    {
                        links.Add(
                            new RobotLink(
                                GetActivePolicy().id,
                                p,
                                p.playerSettings.AreaRestrictionInPawnCurrentMap,
                                currentMap
                            )
                        );
                    }
                }
                catch
                {
                    //it seems a pawn became null during the links iteration so lets just move on
                    Log.Message("BPC: A pawn became null during robot save state: " + p == null);
                }
            }
        }

        internal static void DeletePolicy(Policy policy)
        {
            if (policy == null || policy.id <= 0) return;

            links.RemoveAll(x => x.zone == policy.id);
            policies.Remove(policy);
            var mapId = Find.CurrentMap.uniqueID;
            foreach (MapActivePolicy m in activePolicies)
            {
                if (m.activePolicy.id == policy.id)
                {
                    m.activePolicy = policies[0];
                    DirtyPolicy = true;
                }
            }
        }

        internal static void CleanRemovedMaps(Map map)
        {
            //foreach (var map in activePolicies)
            //{
            //    if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
            //    {
            //        if (Find.Maps.Count == 1 && !ActivePoliciesContainsValidMap())
            //        {
            //            //this means the player was on the move without any base
            //            //and just re-settled. So, let's move the settings to
            //            //the new map
            //            int newMapId = Find.CurrentMap.uniqueID;
            //            RobotManager.MoveLinksToMap(map.mapId, newMapId);
            //            map.mapId = newMapId;
            //        }
            //        else
            //        {
            //            DeleteLinksInMap(map.mapId);
            //            DeleteMap(map);
            //        }
            //    }
            //}
            if (!map.IsPlayerHome)
            {
                RobotManager.DeleteLinksInMap(map.uniqueID);
                MapActivePolicy mapActivePolicy = RobotManager.GetActiveMap(map.uniqueID);
                RobotManager.DeleteMap(mapActivePolicy);
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
                if (!RobotManager.ActivePoliciesContainsValidMap())
                {
                    RobotManager.MoveLinksToMap(LastMapManager.lastMapId, newMap.uniqueID);
                }
            }
            else
            {
                //this makes no sense
                Log.Warning("[BPC] This code shouldn't have ran");
            }
        }

        private static void DeleteMap(MapActivePolicy map)
        {
            activePolicies.Remove(map);
        }

        private static void DeleteLinksInMap(int mapId)
        {
            links.RemoveAll(x => x.mapId == mapId);
        }

        private static bool ActivePoliciesContainsValidMap()
        {
            bool containsValidMap = false;
            foreach (Map map in Find.Maps)
            {
                if (RobotManager.activePolicies.Any(x => x.mapId == map.uniqueID))
                {
                    containsValidMap = true;
                    break;
                }
            }
            return containsValidMap;
        }

        internal static void CleanDeadRobots(Pawn pawn)
        {
            RobotManager.links.RemoveAll(x => x.robot == pawn);
        }

        internal static void LinksCleanUp()
        {
            RobotManager.links.RemoveAll(x => x.robot == null || x.robot.Faction == null);
        }
    }
}
