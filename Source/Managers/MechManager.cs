using System;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class MechManager : Manager<MechLink>
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

                if (p.IsGestating() || p.GetOverseer() == null)
                {
                    continue;
                }

                //find mech on the current zone
                MechLink MechLink =
                    MechManager.links.Find(
                        x => x != null && p.Equals(x.mech) &&
                        x.zone == MechManager.GetActivePolicy().id &&
                        x.mapId == currentMap);

                if (MechLink != null )
                {
                    //Mech found! save area and settings
                    MechLink.autorepair = p.GetComp<CompMechRepairable>().autoRepair;                    
                    MechLink.controlGroupIndex = p.GetMechControlGroup().Index;
                    MechLink.workmode = p.GetMechWorkMode();
                    MechLink.area = p.playerSettings.AreaRestriction;
                }
                else
                {
                    //Mech not found. So add it to the MechLink list
                    MechManager.links.Add(
                        new MechLink(
                            MechManager.GetActivePolicy().id,
                            p,
                            p.GetComp<CompMechRepairable>().autoRepair,
                            p.GetMechControlGroup().Index,
                            p.GetMechWorkMode(),
                            p.playerSettings.AreaRestriction,
                            currentMap)); ; ;
                }
            }
        }

        internal static void CleanDeadMechs(Pawn pawn)
        {
            MechManager.links.RemoveAll(x => x.mech == pawn);
        }

        internal static void LinksCleanUp()
        {
            for (int i = MechManager.links.Count - 1; i >= 0; i--)
            {
                if (MechManager.links[i].mech == null || MechManager.links[i].mech.Faction == null)
                {
                    MechManager.links.RemoveAt(i);
                }
            }
        }

        internal static void LoadState(List<MechLink> links, List<Pawn> pawns, Policy policy)
        {
            List<MechLink> mapLinks = null;
            List<MechLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x != null && x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x != null &&  x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                if (p.GetOverseer() != null)
                {
                    foreach (MechLink l in zoneLinks)
                    {
                        if (l.mech != null && l.mech.Equals(p))
                        {
                            //found mech in zone. Load state
                            p.GetComp<CompMechRepairable>().autoRepair = l.autorepair;
                            foreach (MechanitorControlGroup group in p.GetMechControlGroup().Tracker.controlGroups)
                            {
                                if (group.Index == l.controlGroupIndex && p.GetMechControlGroup().Index != l.controlGroupIndex)
                                {
                                    // Only load assign group if it's actually a different group. This is to avoid the mech to leave recharge task if the same group is assigned
                                    group.Assign(p);
                                }
                            }
                            p.GetMechControlGroup().SetWorkMode(l.workmode);  
                            p.playerSettings.AreaRestriction = l.area;
                        }
                    }
                }                
            }
            MechManager.SetActivePolicy(policy);
        }

        internal static void LoadState(Policy policy)
        {
            LoadState(MechManager.links, Mechs().ToList(), policy);
        }

        internal static void UpdateState(
            List<MechLink> links, List<Pawn> pawns, Policy policy)
        {
            List<MechLink> mapLinks = null;
            List<MechLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(l => l != null && l.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(ml => ml != null && ml.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                if (p.GetOverseer() != null)
                {
                    foreach (MechLink l in zoneLinks)
                    {
                        if (l.mech != null && l.mech.GetUniqueLoadID().Equals(p.GetUniqueLoadID()))
                        {
                            l.autorepair = p.GetComp<CompMechRepairable>().autoRepair;
                            l.controlGroupIndex = p.GetMechControlGroup().Index;
                            //if (p.GetMechControlGroup().WorkMode != l.workmode)
                            //{
                            //    p.GetMechControlGroup().SetWorkMode(l.workmode);
                            //}
                            l.workmode = p.GetMechControlGroup().WorkMode;
                            l.area = p.playerSettings.AreaRestriction;
                        }
                    }
                }
            }
            MechManager.SetActivePolicy(policy);
        }

        internal static bool ActivePoliciesContainsValidMap()
        {
            bool containsValidMap = false;
            foreach (Map map in Find.Maps)
            {
                if (MechManager.activePolicies.Any(x => x.mapId == map.uniqueID))
                {
                    containsValidMap = true;
                    break;
                }
            }
            return containsValidMap;
        }

        internal static void CleanRemovedMaps()
        {
            for (int i = 0; i < MechManager.activePolicies.Count; i++)
            {
                MapActivePolicy map = MechManager.activePolicies[i];
                if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                {
                    if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                    {
                        if (Find.Maps.Count == 1 && !MechManager.ActivePoliciesContainsValidMap())
                        {
                            //this means the player was on the move without any base
                            //and just re-settled. So, let's move the settings to
                            //the new map
                            int mapid = Find.CurrentMap.uniqueID;
                            MechManager.MoveLinksToMap(mapid);
                            map.mapId = mapid;
                        }
                        else
                        {
                            MechManager.DeleteLinksInMap(map.mapId);
                            MechManager.DeleteMap(map);
                        }
                    }
                }
            }
        }

        internal static IEnumerable<Pawn> Mechs()
        {
            try
            {
                return from p in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer) where p.IsColonyMech select p;
            }                
            catch (Exception)
            {
                //following players reports, if a settlement is abandoned the previous statement fails with a null reference, so lets return an empty list 
                return new List<Pawn>();
            }
        }


        internal static void PrintAllMechPolicies(string spacer = "\n")
        {
            Log.Message( "[BPC] ### List Mech Policies [" + MechManager.policies.Count + "] ###");
            foreach (Policy p in MechManager.policies)
            {
                Log.Message("[BPC]\t" + p.ToString());
            }

            Log.Message("[BPC] ### Mech ActivePolices START [" + MechManager.activePolicies.Count + "] ===");
            foreach (MapActivePolicy m in MechManager.activePolicies)
            {
                Log.Message("[BPC]\t" + m.ToString());
            }

            Log.Message("[BPC] ### List Mech links [" + MechManager.links.Count + "] ###");
            foreach (MechLink l in MechManager.links)
            {
                Log.Message("[BPC]\t" + l);
            }

            Log.Message(spacer);
        }

    }
}