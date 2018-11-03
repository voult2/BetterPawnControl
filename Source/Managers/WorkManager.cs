using System.Collections.Generic;
using Verse;
using RimWorld;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class WorkManager : Manager<WorkLink>
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

        internal static void SavePawnPriorities(Pawn p, WorkLink link)
        {
            if (link.settings != null)
            {
                foreach (var worktype in DefDatabase<WorkTypeDef>.AllDefsListForReading)
                {
                    if (link.settings.ContainsKey(worktype))
                    {
                        link.settings[worktype] = p.workSettings.GetPriority(worktype);
                    }
                    else
                    {
                        link.settings.Add(worktype, p.workSettings.GetPriority(worktype));
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
                    p.workSettings.SetPriority(entry.Key, link.settings.TryGetValue(entry.Key));
                }
            }
        }
    }
}
