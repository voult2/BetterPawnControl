using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Noise;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class WeaponsManager : Manager<WeaponsLink>
    {
        internal static int _defaultLoadoutId = -1;

        internal static List<WeaponsLink> clipboard = new List<WeaponsLink>();
        internal static int DefaultWeaponsLoadoutById
        {
            get
            {
                if (_defaultLoadoutId == -1)
                {
                    _defaultLoadoutId = Widget_WeaponsTabReborn.GetDefaultLoadoutId();
                }
                return _defaultLoadoutId;
            }

            set
            {
                _defaultLoadoutId = value;
            }
        }

        internal static void DeletePolicy(Policy policy)
        {
            //delete if not default WeaponPolicy
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

                WeaponsLink weaponLink =
                    WeaponsManager.links.Find(
                        x => x != null && x.colonist != null && p != null && p.Equals(x.colonist) &&
                        x.zone == WeaponsManager.GetActivePolicy().id &&
                        x.mapId == currentMap);

                if (weaponLink != null )
                {
                    //Weapon found! save             
                    weaponLink.loadoutId = Widget_WeaponsTabReborn.GetLoadoutId(p);                }
                else
                {
                    //Weapon not found. So add it to the WeaponLink list
                    WeaponsManager.links.Add(
                        new WeaponsLink(
                            WeaponsManager.GetActivePolicy().id,
                            p,
                            Widget_WeaponsTabReborn.GetLoadoutId(p),
                            currentMap)); ; ;
                }
            }
        }

        internal static void CleanDeadColonists(Pawn pawn)
        {
            WeaponsManager.links.RemoveAll(x => x.colonist == pawn);
        }

        internal static void LinksCleanUp()
        {
            for (int i = WeaponsManager.links.Count - 1; i >= 0; i--)
            {
                if (WeaponsManager.links[i].colonist == null || !WeaponsManager.links[i].colonist.IsColonist)
                {
                    WeaponsManager.links.RemoveAt(i);
                }
            }
        }

        internal static void LoadState(List<WeaponsLink> links, List<Pawn> pawns, Policy policy)
        {
            List<WeaponsLink> mapLinks = null;
            List<WeaponsLink> zoneLinks = null;
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

                foreach (WeaponsLink l in zoneLinks)
                {
                    if (l.colonist != null && l.colonist.Equals(p))
                    {
                        Widget_WeaponsTabReborn.SetLoadoutId(p, l.loadoutId);
                    }
                }             
            }
            WeaponsManager.SetActivePolicy(policy);
        }

        internal static void LoadState(Policy policy)
        {
            List<Pawn> pawns = Find.CurrentMap.mapPawns.FreeColonists.ToList();
            LoadState(WeaponsManager.links, pawns, policy);
        }

        internal static bool ActivePoliciesContainsValidMap()
        {
            bool containsValidMap = false;
            foreach (Map map in Find.Maps)
            {
                if (WeaponsManager.activePolicies.Any(x => x.mapId == map.uniqueID))
                {
                    containsValidMap = true;
                    break;
                }
            }
            return containsValidMap;
        }

        internal static void CleanRemovedMaps(Map map)
        {
            //for (int i = 0; i < WeaponsManager.activePolicies.Count; i++)
            //{
            //    MapActivePolicy map = WeaponsManager.activePolicies[i];
            //    if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
            //    {
            //        if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
            //        {
            //            if (Find.Maps.Count == 1 && !WeaponsManager.ActivePoliciesContainsValidMap())
            //            {
            //                //this means the player was on the move without any base
            //                //and just re-settled. So, let's move the settings to
            //                //the new map
            //                int newMapId = Find.CurrentMap.uniqueID;
            //                WeaponsManager.MoveLinksToMap(map.mapId, newMapId);
            //                map.mapId = newMapId;
            //            }
            //            else
            //            {
            //                WeaponsManager.DeleteLinksInMap(map.mapId);
            //                WeaponsManager.DeleteMap(map);
            //            }
            //        }
            //    }
            //}
            if (!map.IsPlayerHome)
            {
                WeaponsManager.DeleteLinksInMap(map.uniqueID);
                MapActivePolicy mapActivePolicy = WeaponsManager.GetActiveMap(map.uniqueID);
                WeaponsManager.DeleteMap(mapActivePolicy);
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
                if (!WeaponsManager.ActivePoliciesContainsValidMap())
                {
                    WeaponsManager.MoveLinksToMap(LastMapManager.lastMapId, newMap.uniqueID);
                }
            }
            else
            {
                //this makes no sense
                Log.Warning("[BPC] This code shouldn't have ran");
            }
        }

        internal static void CopyToClipboard()
        {
            //Save state in case user has made changes to the active policy
            WeaponsManager.SaveCurrentState(AssignManager.Colonists().ToList());

            Policy policy = GetActivePolicy();

            WeaponsManager.clipboard.Clear();
            foreach (WeaponsLink link in WeaponsManager.links)
            {
                if (link.zone == policy.id)
                {
                    WeaponsManager.clipboard.Add(new WeaponsLink(link));
                }
            }
        }

        internal static void PasteToActivePolicy()
        {
            Policy policy = GetActivePolicy();
            if (!WeaponsManager.clipboard.NullOrEmpty() && WeaponsManager.clipboard[0].zone != policy.id)
            {
                WeaponsManager.links.RemoveAll(x => x.zone == policy.id);
                foreach (WeaponsLink copiedLink in WeaponsManager.clipboard)
                {
                    copiedLink.zone = policy.id;
                    WeaponsManager.links.Add(copiedLink);
                }
                WeaponsManager.LoadState(links, Find.CurrentMap.mapPawns.FreeColonists, policy);
            }
        }


        internal static void PrintAllWeaponsPolicies(string spacer = "\n")
        {
            Log.Message( "[BPC] ### List Weapons Policies [" + WeaponsManager.policies.Count + "] ###");
            foreach (Policy p in WeaponsManager.policies)
            {
                Log.Message("[BPC]\t" + p.ToString());
            }

            Log.Message("[BPC] ### Weapons ActivePolices START [" + WeaponsManager.activePolicies.Count + "] ===");
            foreach (MapActivePolicy m in WeaponsManager.activePolicies)
            {
                Log.Message("[BPC]\t" + m.ToString());
            }

            Log.Message("[BPC] ### List Weapons links [" + WeaponsManager.links.Count + "] ###");
            foreach (WeaponsLink l in WeaponsManager.links)
            {
                Log.Message("[BPC]\t" + l);
            }

            Log.Message(spacer);
        }

    }
}