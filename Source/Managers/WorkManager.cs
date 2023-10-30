using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class WorkManager : Manager<WorkLink>
    {
        internal static List<WorkLink> clipboard = new List<WorkLink>();

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
                //find colonist in the current zone in the current map
                WorkLink link = WorkManager.links.Find(
                    x => p.Equals(x.colonist) &&
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
                        new Dictionary<WorkGiverDef, List<int>>(),
                        currentMap);
                    WorkManager.links.Add(link);
                    WorkManager.SavePawnPriorities(p, link);
                }
            }
        }

        internal static void CleanDeadColonists(Pawn pawn)
        {
            WorkManager.links.RemoveAll(x => x.colonist == pawn);
        }

        internal static void LinksCleanUp()
        {
            for (int i = WorkManager.links.Count - 1; i >= 0; i--)
            {
                if (WorkManager.links[i].colonist == null || !WorkManager.links[i].colonist.IsColonist)
                {
                    WorkManager.links.RemoveAt(i);
                }
            }
        }

        internal static bool ActivePoliciesContainsValidMap()
        {
            bool containsValidMap = false;
            foreach (Map map in Find.Maps)
            {
                if (WorkManager.activePolicies.Any(x => x.mapId == map.uniqueID))
                {
                    containsValidMap = true;
                    break;
                }
            }
            return containsValidMap;
        }

        internal static void CleanRemovedMaps()
        {
            for (int i = 0; i < WorkManager.activePolicies.Count; i++)
            {
                MapActivePolicy map = WorkManager.activePolicies[i];
                if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                {
                    if (Find.Maps.Count == 1 && !WorkManager.ActivePoliciesContainsValidMap())
                    {
                        //this means the player was on the move without any base
                        //and just re-settled. So, let's move the settings to
                        //the new map
                        int mapid = Find.CurrentMap.uniqueID;
                        WorkManager.MoveLinksToMap(mapid);
                        map.mapId = mapid;
                    }
                    else
                    {
                        WorkManager.DeleteLinksInMap(map.mapId);
                        WorkManager.DeleteMap(map);
                    }
                }
            }
        }

        internal static void LoadState(List<WorkLink> links, List<Pawn> pawns, Policy policy)
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

        internal static void LoadState(Policy policy)
        {
            List<Pawn> pawns = Find.CurrentMap.mapPawns.FreeColonists.ToList();
            LoadState(WorkManager.links, pawns, policy);
        }

        internal static void SavePawnPriorities(Pawn p, WorkLink link)
        {
            if (link.settings != null)
            {
                foreach (var worktype in DefDatabase<WorkTypeDef>.AllDefsListForReading)
                {
                    link.settings.SetOrAdd(worktype, p.workSettings.GetPriority(worktype));

                    if (!Widget_ModsAvailable.WorkTabAvailable || Widget_ModsAvailable.DisableBPCOnWorkTab || Widget_ModsAvailable.DisableBPCWorkTabInnerPriorities)
                        continue;

                    var workGivers = GetWorkGivers(worktype);
                    foreach (var workGiver in workGivers)
                    {
                        var priorities = Widget_WorkTab.GetWorkTabPriorities(p, workGiver);
                        if (priorities != null)
                        {
                            link.settingsInner.SetOrAdd(workGiver, priorities);
                        }
                    }
                }
            }
        }

        internal static void LoadPawnPriorities(Pawn p, WorkLink link)
        {
            if (link.settings != null)
            {
                foreach (KeyValuePair<WorkTypeDef, int> entry in link.settings)
                {
                    try
                    {
                        //The SetPriority method is very performance intensive but there is nothing I can do it about it 
                        p.workSettings.SetPriority(entry.Key, link.settings.TryGetValue(entry.Key));                        
                    }
                    catch
                    {
                        //ignore the errors when setting priorities that aren't supported (such as art for slaves)
                    }
                }
            }

            if (Widget_ModsAvailable.WorkTabAvailable && !Widget_ModsAvailable.DisableBPCOnWorkTab && !Widget_ModsAvailable.DisableBPCWorkTabInnerPriorities && link.settingsInner != null )
            {
                foreach (var entryInner in link.settingsInner)
                {
                    Widget_WorkTab.SetWorkTabPriorities(p, entryInner.Key, entryInner.Value);
                }
            }
        }

        internal static void CopyToClipboard()
        {
            Policy policy = GetActivePolicy();
            if (WorkManager.clipboard != null)
            {
                clipboard = new List<WorkLink>();
            }

            WorkManager.clipboard.Clear();
            foreach (WorkLink link in WorkManager.links)
            {
                if (link.zone == policy.id)
                {
                    WorkManager.clipboard.Add(new WorkLink(link));
                }
            }
        }

        internal static void PasteToActivePolicy()
        {
            Policy policy = GetActivePolicy();
            if (!WorkManager.clipboard.NullOrEmpty() && WorkManager.clipboard[0].zone != policy.id)
            {
                WorkManager.links.RemoveAll(x => x.zone == policy.id);
                foreach (WorkLink copiedLink in WorkManager.clipboard)
                {
                    copiedLink.zone = policy.id;
                    WorkManager.links.Add(copiedLink);
                }
                WorkManager.LoadState(links, Find.CurrentMap.mapPawns.FreeColonists.ToList(), policy);
            }
        }

        internal static void PrintAllWorkPolicies(string spacer = "\n")
        {
            Log.Message("[BPC] === List Policies START [" + WorkManager.policies.Count + "] ===");
            foreach (Policy p in AssignManager.policies)
            {
                Log.Message("[BPC]\t" + p.ToString());
            }

            Log.Message("[BPC] === List ActivePolices START [" + WorkManager.activePolicies.Count + "] ===");
            foreach (MapActivePolicy m in WorkManager.activePolicies)
            {
                Log.Message("[BPC]\t" + m.ToString());
            }

            Log.Message("[BPC] === List links START [" + AssignManager.links.Count + "] ===");
            foreach (WorkLink workLink in WorkManager.links)
            {
                Log.Message("[BPC]\t" + workLink.ToString());
            }

            Log.Message(spacer);
        }
    }
}
