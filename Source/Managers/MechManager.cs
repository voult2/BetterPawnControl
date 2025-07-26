using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]

    class MechManager : Manager<MechLink>
    {
        internal static List<MechLink> clipboard = new List<MechLink>();

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

                if (p == null || p.IsGestating() || p.GetOverseer() == null)
                {
                    continue;
                }

                //find mech on the current zone
                MechLink MechLink =
                    MechManager.links.Find(
                        x => x != null && x.mech != null && p != null && p.Equals(x.mech) &&
                        x.zone == MechManager.GetActivePolicy().id &&
                        x.mapId == currentMap);

                if (MechLink != null)
                {
                    //Mech found! save area and settings                 
                    MechLink.controlGroupIndex = p.GetMechControlGroup().Index;
                    MechLink.workmode = p.GetMechWorkMode();
                    MechLink.area = p.playerSettings.AreaRestrictionInPawnCurrentMap;
                }
                else
                {
                    //Mech not found. So add it to the MechLink list
                    MechManager.links.Add(
                        new MechLink(
                            MechManager.GetActivePolicy().id,
                            p,
                            p.GetMechControlGroup().Index,
                            p.GetMechWorkMode(),
                            p.playerSettings.AreaRestrictionInPawnCurrentMap,
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
                if (p == null)
                {
                    continue;
                }

                if (p.GetOverseer() != null)
                {
                    foreach (MechLink l in zoneLinks)
                    {
                        if (l.mech != null && l.mech.Equals(p))
                        {
                            //found mech in zone. Load state
                            foreach (MechanitorControlGroup group in p.GetMechControlGroup().Tracker.controlGroups)
                            {
                                if (group.Index == l.controlGroupIndex && p.GetMechControlGroup().Index != l.controlGroupIndex)
                                {
                                    // Only load assign group if it's actually a different group. This is to avoid the mech to leave recharge task if the same group is assigned
                                    group.Assign(p);
                                }
                            }
                            p.GetMechControlGroup().SetWorkMode(l.workmode);  
                            p.playerSettings.AreaRestrictionInPawnCurrentMap = l.area;
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

            try
            {
                foreach (Pawn p in pawns)
                {
                    if (p.GetOverseer() != null)
                    {
                        foreach (MechLink l in zoneLinks)
                        {
                            if (l.mech != null && l.mech.GetUniqueLoadID().Equals(p.GetUniqueLoadID()))
                            {
                                l.controlGroupIndex = p.GetMechControlGroup().Index;
                                l.workmode = p.GetMechControlGroup().WorkMode;
                                l.area = p.playerSettings.AreaRestrictionInPawnCurrentMap;
                            }
                        }
                    }
                }
                MechManager.SetActivePolicy(policy);
            }
            catch (Exception ex)
            {
                Log.Warning("BPC: UpdateState: could not update state. Details: " + ex.Message + " | " + ex.StackTrace.ToString());
            }
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

        internal static void CleanRemovedMaps(Map map)
        {
            //for (int i = 0; i < MechManager.activePolicies.Count; i++)
            //{
            //    MapActivePolicy map = MechManager.activePolicies[i];
            //    if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
            //    {
            //        if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
            //        {
            //            if (Find.Maps.Count == 1 && !MechManager.ActivePoliciesContainsValidMap())
            //            {
            //                //this means the player was on the move without any base
            //                //and just re-settled. So, let's move the settings to
            //                //the new map
            //                int newMapId = Find.CurrentMap.uniqueID;
            //                MechManager.MoveLinksToMap(map.mapId, newMapId);
            //                map.mapId = newMapId;
            //            }
            //            else
            //            {
            //                MechManager.DeleteLinksInMap(map.mapId);
            //                MechManager.DeleteMap(map);
            //            }
            //        }
            //    }
            //}
            if (!map.IsPlayerHome)
            {
                MechManager.DeleteLinksInMap(map.uniqueID);
                MapActivePolicy mapActivePolicy = MechManager.GetActiveMap(map.uniqueID);
                MechManager.DeleteMap(mapActivePolicy);
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
                if (!MechManager.ActivePoliciesContainsValidMap())
                {
                    MechManager.MoveLinksToMap(LastMapManager.lastMapId, newMap.uniqueID);
                }
            }
            else
            {
                //this makes no sense
                Log.Warning("[BPC] This code shouldn't have ran");
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
                //following players reports, if a settlement is abandoned, the previous statement fails with a null reference; so lets return an empty list 
                return new List<Pawn>();
            }
        }

        internal static void CopyToClipboard()
        {
            //Save state in case user has made changes to the active policy
            MechManager.SaveCurrentState(MechManager.Mechs().ToList());

            Policy policy = GetActivePolicy();

            MechManager.clipboard.Clear();
            foreach (MechLink link in MechManager.links)
            {
                if (link.zone == policy.id)
                {
                    MechManager.clipboard.Add(new MechLink(link));
                }
            }
        }

        internal static void PasteToActivePolicy()
        {
            Policy policy = GetActivePolicy();
            if (!MechManager.clipboard.NullOrEmpty() && MechManager.clipboard[0].zone != policy.id)
            {
                MechManager.links.RemoveAll(x => x.zone == policy.id);
                foreach (MechLink copiedLink in MechManager.clipboard)
                {
                    copiedLink.zone = policy.id;
                    MechManager.links.Add(copiedLink);
                }
                MechManager.LoadState(links, Mechs().ToList(), policy);
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