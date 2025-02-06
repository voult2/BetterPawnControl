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
                // find robot in the current zone
                var robotLink = links.Find(
                    x => x != null && p.Equals(x.robot) &&
                    x.zone == GetActivePolicy().id &&
                    x.mapId == currentMap);

                if (robotLink != null)
                {
                    robotLink.area = p.playerSettings.AreaRestrictionInPawnCurrentMap;
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

        internal static void CleanRemovedMaps()
        {
            foreach (var map in activePolicies)
            {
                if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                {
                    if (Find.Maps.Count == 1 && !ActivePoliciesContainsValidMap())
                    {
                        //this means the player was on the move without any base
                        //and just re-settled. So, let's move the settings to
                        //the new map
                        int mapId = Find.CurrentMap.uniqueID;
                        MoveLinksToMap(mapId);
                        map.mapId = mapId;
                    }
                    else
                    {
                        DeleteLinksInMap(map.mapId);
                        DeleteMap(map);
                    }
                }
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
