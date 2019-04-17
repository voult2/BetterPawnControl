using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class RestrictManager : Manager<RestrictLink>
    {
        internal static List<RestrictLink> clipboard = new List<RestrictLink>();

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
            int activePolicyId = RestrictManager.GetActivePolicy().id;
            //Save current state
            foreach (Pawn p in pawns)
            {
                //find colonist in the current zone in the current map
                RestrictLink link = RestrictManager.links.Find(
                    x => x.colonist.Equals(p) &&
                    x.zone == activePolicyId &&
                    x.mapId == currentMap);

                if (link != null)
                {
                    //colonist found! save 
                    link.area = p.playerSettings.AreaRestriction;
                    RestrictManager.CopySchedule(p.timetable.times, link.schedule);
                }
                else
                {
                    //colonist not found. So add it to the RestrictLink list
                    RestrictManager.links.Add(
                        new RestrictLink(
                            activePolicyId,
                            p,
                            p.playerSettings.AreaRestriction,
                            p.timetable.times,
                            currentMap));
                }
            }
        }

        internal static void CopySchedule(
                                List<TimeAssignmentDef> src,
                                List<TimeAssignmentDef> dst)
        {
            dst.Clear();
            dst.AddRange(src);
        }

        internal static void CleanDeadColonists(List<Pawn> pawns)
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

        internal static bool ActivePoliciesContainsValidMap()
        {
            bool containsValidMap = false;
            foreach (Map map in Find.Maps)
            {
                if (RestrictManager.activePolicies.Any(x => x.mapId == map.uniqueID))
                {
                    containsValidMap = true;
                    break;
                }
            }
            return containsValidMap;
        }

        internal static void CleanDeadMaps()
        {
            for (int i = 0; i < RestrictManager.activePolicies.Count; i++)
            {
                MapActivePolicy map = RestrictManager.activePolicies[i];
                if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                {
                    if (Find.Maps.Count == 1 && !RestrictManager.ActivePoliciesContainsValidMap())
                    {
                        //this means the player was on the move without any base
                        //and just re-settled. So, let's move the settings to
                        //the new map
                        int mapid = Find.CurrentMap.uniqueID;
                        RestrictManager.MoveLinksToMap(mapid);
                        map.mapId = mapid;
                    }
                    else
                    {
                        RestrictManager.DeleteLinksInMap(map.mapId);
                        RestrictManager.DeleteMap(map);
                    }
                }
            }
        }

        internal static void UpdateState(
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

        internal static void LoadState(
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
                        RestrictManager.CopySchedule(l.schedule, p.timetable.times);
                    }
                }
            }

            RestrictManager.SetActivePolicy(policy);
        }

        internal static void PrintAllAssignPolicies()
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

        internal static void CopyToClipboard()
        {
            Policy policy = GetActivePolicy();
            if (RestrictManager.clipboard != null)
            {
                RestrictManager.clipboard = new List<RestrictLink>();
            }

            RestrictManager.clipboard.Clear();
            foreach (RestrictLink link in RestrictManager.links)
            {
                if (link.zone == policy.id)
                {
                    RestrictManager.clipboard.Add(link);
                }
            }
        }

        internal static void PasteToActivePolicy()
        {
            Policy policy = GetActivePolicy();
            if (!RestrictManager.clipboard.NullOrEmpty() && RestrictManager.clipboard[0].zone != policy.id)
            {
                //RestrictManager.links.RemoveAll(x => x.zone == policy.id);
                foreach (RestrictLink copiedLink in RestrictManager.clipboard)
                {
                    foreach (RestrictLink link in RestrictManager.links)
                    {
                        if (link.colonist == copiedLink.colonist && link.zone == policy.id)
                        {
                            RestrictManager.CopySchedule(copiedLink.schedule, link.schedule);
                            Log.Message("Copied Schedule");
                        }
                    }
                }

                RestrictManager.LoadState(
                                links,
                                Find.CurrentMap.mapPawns.FreeColonists.ToList(),
                                policy);
            }
        }
    }
}
