﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Noise;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class ScheduleManager : Manager<ScheduleLink>
    {
        internal static List<ScheduleLink> clipboard = new List<ScheduleLink>();

        internal static void FixActivePolicies()
        {
            activePolicies = new List<MapActivePolicy>();
            activePolicies.Add(new MapActivePolicy(0, new Policy(0, "BPC.Auto".Translate())));
        }


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
            int activePolicyId = ScheduleManager.GetActivePolicy().id;
            //Save current state
            foreach (Pawn p in pawns)
            {
                //find colonist in the current zone in the current map
                ScheduleLink link = ScheduleManager.links.Find(
                    x => p.Equals(x.colonist) &&
                    x.zone == activePolicyId &&
                    x.mapId == currentMap);

                if (link != null)
                {
                    //colonist found! save 
                    link.area = p.playerSettings.AreaRestrictionInPawnCurrentMap;
                    if (p.timetable != null)
                    {
                        ScheduleManager.CopySchedule(p.timetable.times, link.schedule);
                    }
                }
                else
                {
                    //colonist not found. So add it to the ScheduleLink list
                    ScheduleManager.links.Add(
                        new ScheduleLink(
                            activePolicyId,
                            p,
                            p.playerSettings.AreaRestrictionInPawnCurrentMap,
                            p.timetable != null ? p.timetable.times : null,
                            currentMap));
                }
            }
        }

        internal static bool CopySchedule(List<TimeAssignmentDef> src, List<TimeAssignmentDef> dst)
        {
            var isEquals = false;

            if (src.Count == dst.Count)
            {
                isEquals = true;
                for (var i = 0; i < src.Count; i++)
                {
                    if (src[i] != dst[i])
                    {
                        isEquals = false;
                        break;
                    }
                }
            }

            dst.Clear();
            dst.AddRange(src);

            return !isEquals;
        }

        internal static void CleanDeadColonists(Pawn pawn)
        {
            ScheduleManager.links.RemoveAll(x => x.colonist == pawn);
        }

        internal static void LinksCleanUp()
        {
            for (int i = ScheduleManager.links.Count - 1; i >= 0; i--)
            {
                if (ScheduleManager.links[i].colonist == null || !ScheduleManager.links[i].colonist.IsColonist)
                {
                    ScheduleManager.links.RemoveAt(i);
                }
            }
        }

        internal static bool ActivePoliciesContainsValidMap()
        {
            bool containsValidMap = false;
            foreach (Map map in Find.Maps)
            {
                if (ScheduleManager.activePolicies.Any(x => x.mapId == map.uniqueID))
                {
                    containsValidMap = true;
                    break;
                }
            }
            return containsValidMap;
        }

        internal static void CleanRemovedMaps(Map map)
        {
            //for (int i = 0; i < ScheduleManager.activePolicies.Count; i++)
            //{
            //    MapActivePolicy map = ScheduleManager.activePolicies[i];
            //    if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
            //    {
            //        if (Find.Maps.Count == 1 && !ScheduleManager.ActivePoliciesContainsValidMap())
            //        {
            //            //this means the player was on the move without any base
            //            //and just re-settled. So, let's move the settings to
            //            //the new map
            //            int newMapId = Find.CurrentMap.uniqueID;
            //            ScheduleManager.MoveLinksToMap(map.mapId, newMapId);
            //            map.mapId = newMapId;
            //        }
            //        else
            //        {
            //            ScheduleManager.DeleteLinksInMap(map.mapId);
            //            ScheduleManager.DeleteMap(map);
            //        }
            //    }
            //}
            if (!map.IsPlayerHome)
            {
                ScheduleManager.DeleteLinksInMap(map.uniqueID);
                MapActivePolicy mapActivePolicy = ScheduleManager.GetActiveMap(map.uniqueID);
                ScheduleManager.DeleteMap(mapActivePolicy);
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
                if (!ScheduleManager.ActivePoliciesContainsValidMap())
                {
                    ScheduleManager.MoveLinksToMap(LastMapManager.lastMapId, newMap.uniqueID);
                }
            }
            else
            {
                //this makes no sense
                Log.Warning("[BPC] This code shouldn't have ran");
            }
        }

        internal static void UpdateState(List<ScheduleLink> links, List<Pawn> pawns, Policy policy)
        {
            List<ScheduleLink> mapLinks = null;
            List<ScheduleLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                foreach (ScheduleLink l in zoneLinks)
                {
                    if (l.colonist != null && l.colonist.GetUniqueLoadID().Equals(p.GetUniqueLoadID()))
                    {
                        l.area = p.playerSettings.AreaRestrictionInPawnCurrentMap;
                    }
                }
            }

            ScheduleManager.SetActivePolicy(policy);
        }

        internal static void LoadState(List<ScheduleLink> links, List<Pawn> pawns, Policy policy)
        {
            List<ScheduleLink> mapLinks = null;
            List<ScheduleLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                var tick = false;
                foreach (ScheduleLink l in zoneLinks)
                {
                    if (l.colonist != null && l.colonist.Equals(p))
                    {
                        if (p.playerSettings.AreaRestrictionInPawnCurrentMap != l.area)
                        {
                            p.playerSettings.AreaRestrictionInPawnCurrentMap = l.area;
                            tick = true;
                        }

                        if (l.schedule != null && p.timetable != null)
                        {
                            var updated = ScheduleManager.CopySchedule(l.schedule, p.timetable.times);
                            tick = updated == true;
                        }
                    }
                }

                if (tick)
                    p.Tick();
            }

            ScheduleManager.SetActivePolicy(policy);
        }

        internal static void LoadState(Policy policy)
        {
            List<Pawn> pawns = Find.CurrentMap.mapPawns.FreeColonists.ToList();
            LoadState(ScheduleManager.links, pawns, policy);
        }

        internal static void PrintAllSchedulePolicies(string spacer = "\n")
        {
            Log.Message("[BPC] === List Policies START [" + ScheduleManager.policies.Count + "] ===");
            foreach (Policy p in ScheduleManager.policies)
            {
                Log.Message("[BPC]\t" + p.ToString());
            }

            Log.Message("[BPC] === List ActivePolices START [" + ScheduleManager.activePolicies.Count + "] ===");
            foreach (MapActivePolicy m in ScheduleManager.activePolicies)
            {
                Log.Message("[BPC]\t" + m.ToString());
            }

            Log.Message("[BPC] === List links START [" + ScheduleManager.links.Count + "] ===");
            foreach (ScheduleLink ScheduleLink in ScheduleManager.links)
            {
                Log.Message("[BPC]\t" + ScheduleLink.ToString());
            }

            Log.Message(spacer);
        }

        internal static void CopyToClipboard()
        {
            //Save state in case user has made changes to the active policy
            ScheduleManager.SaveCurrentState(ScheduleManager.Colonists().ToList());
            Policy policy = GetActivePolicy();
            //if (ScheduleManager.clipboard != null)
            //{
            //    ScheduleManager.clipboard = new List<ScheduleLink>();
            //}

            ScheduleManager.clipboard.Clear();
            foreach (ScheduleLink link in ScheduleManager.links)
            {
                if (link.zone == policy.id)
                {
                    ScheduleManager.clipboard.Add(new ScheduleLink(link));
                }
            }
        }
        internal static void PasteToActivePolicy()
        {
            Policy policy = GetActivePolicy();
            if (!ScheduleManager.clipboard.NullOrEmpty() && ScheduleManager.clipboard[0].zone != policy.id)
            {
                ScheduleManager.links.RemoveAll(x => x.zone == policy.id);
                foreach (ScheduleLink copiedLink in ScheduleManager.clipboard)
                {
                    copiedLink.zone = policy.id;
                    ScheduleManager.links.Add(copiedLink);
                }
                ScheduleManager.LoadState(links, Find.CurrentMap.mapPawns.FreeColonists.ToList(), policy);
            }
        }
    }
}
