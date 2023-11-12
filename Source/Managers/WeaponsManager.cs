using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class WeaponsManager : Manager<WeaponsLink>
    {
        internal static int _defaultLoadoutId = -1;
        internal static int DefaultWeaponsLoadoutById
        {
            get
            {
                if (_defaultLoadoutId == -1)
                {
                    _defaultLoadoutId = Widget_WeaoponsTabReborn.GetDefaultLoadoutId();

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
                WeaponsLink weaponLink =
                    WeaponsManager.links.Find(
                        x => x != null && p.Equals(x.colonist) &&
                        x.zone == WeaponsManager.GetActivePolicy().id &&
                        x.mapId == currentMap);

                if (weaponLink != null )
                {
                    //Weapon found! save             
                    weaponLink.loadoutId = Widget_WeaoponsTabReborn.GetLoadoutId(p);                }
                else
                {
                    //Weapon not found. So add it to the WeaponLink list
                    WeaponsManager.links.Add(
                        new WeaponsLink(
                            WeaponsManager.GetActivePolicy().id,
                            p,
                            Widget_WeaoponsTabReborn.GetLoadoutId(p),
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
                foreach (WeaponsLink l in zoneLinks)
                {
                    if (l.colonist != null && l.colonist.Equals(p))
                    {
                        Widget_WeaoponsTabReborn.SetLoadoutId(p, l.loadoutId);
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

        internal static void CleanRemovedMaps()
        {
            for (int i = 0; i < WeaponsManager.activePolicies.Count; i++)
            {
                MapActivePolicy map = WeaponsManager.activePolicies[i];
                if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                {
                    if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                    {
                        if (Find.Maps.Count == 1 && !WeaponsManager.ActivePoliciesContainsValidMap())
                        {
                            //this means the player was on the move without any base
                            //and just re-settled. So, let's move the settings to
                            //the new map
                            int mapid = Find.CurrentMap.uniqueID;
                            WeaponsManager.MoveLinksToMap(mapid);
                            map.mapId = mapid;
                        }
                        else
                        {
                            WeaponsManager.DeleteLinksInMap(map.mapId);
                            WeaponsManager.DeleteMap(map);
                        }
                    }
                }
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