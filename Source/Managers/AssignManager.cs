using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class AssignManager : Manager<AssignLink>
    {
        internal static List<AssignLink> clipboard = new List<AssignLink>();

        internal static ApparelPolicy _defaultOutfit = null;
        internal static ApparelPolicy DefaultOutfit
        {
            get
            {
                if (_defaultOutfit == null)
                {

                    _defaultOutfit = Current.Game.outfitDatabase.DefaultOutfit();
                }
                return _defaultOutfit;
            }

            set
            {
                _defaultOutfit = value;
            }
        }

        internal static DrugPolicy _defaultDrugPolicy = null;
        internal static DrugPolicy DefaultDrugPolicy
        {
            get
            {
                if (_defaultDrugPolicy == null)
                {
                    _defaultDrugPolicy = Current.Game.drugPolicyDatabase.DefaultDrugPolicy();
                }
                return _defaultDrugPolicy;
            }

            set
            {
                _defaultDrugPolicy = value;
            }
        }


        internal static FoodPolicy _defaultPrisonerFoodPolicy = null;
        internal static FoodPolicy DefaultPrisonerFoodPolicy
        {
            get
            {
                if (_defaultPrisonerFoodPolicy == null)
                {
                    _defaultPrisonerFoodPolicy = DefaultFoodPolicy;
                }
                return _defaultPrisonerFoodPolicy;
            }

            set
            {
                _defaultPrisonerFoodPolicy = value;
            }
        }

        internal static MedicalCareCategory _defaultMedCare = MedicalCareCategory.NormalOrWorse;
        internal static MedicalCareCategory DefaultMedCare 
        {
            get
            {
                return _defaultMedCare;
            }

            set
            {
                _defaultMedCare = value;
            }
        }

        internal static MedicalCareCategory _defaulPrisonerMedCare = MedicalCareCategory.HerbalOrWorse;
        internal static MedicalCareCategory DefaultPrisonerMedCare
        {
            get
            {
                return _defaulPrisonerMedCare;
            }

            set
            {
                _defaulPrisonerMedCare = value;
            }
        }

        internal static MedicalCareCategory _defaulSlaveMedCare = MedicalCareCategory.HerbalOrWorse;
        internal static MedicalCareCategory DefaultSlaveMedCare
        {
            get
            {
                return _defaulSlaveMedCare;
            }

            set
            {
                _defaulSlaveMedCare = value;
            }
        }

        internal static ApparelPolicy _defaultSlaveOutfit = null;
        internal static ApparelPolicy DefaultSlaveOutfit
        {
            get
            {
                if (_defaultSlaveOutfit == null)
                {

                    _defaultSlaveOutfit = Current.Game.outfitDatabase.DefaultOutfit();
                }
                return _defaultSlaveOutfit;
            }

            set
            {
                _defaultSlaveOutfit = value;
            }
        }

        internal static FoodPolicy _defaultSlaveFoodPolicy = null;
        internal static FoodPolicy DefaultSlaveFoodPolicy
        {
            get
            {
                if (_defaultSlaveFoodPolicy == null)
                {
                    _defaultSlaveFoodPolicy = DefaultFoodPolicy;
                }
                return _defaultSlaveFoodPolicy;
            }

            set
            {
                _defaultSlaveFoodPolicy = value;
            }
        }

        internal static DrugPolicy _defaultSlaveDrugPolicy = null;
        internal static DrugPolicy DefaultSlaveDrugPolicy
        {
            get
            {
                if (_defaultSlaveDrugPolicy == null)
                {
                    _defaultSlaveDrugPolicy = Current.Game.drugPolicyDatabase.DefaultDrugPolicy();
                }
                return _defaultSlaveDrugPolicy;
            }

            set
            {
                _defaultSlaveDrugPolicy = value;
            }
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
            //Save current state
            foreach (Pawn p in pawns)
            {
                //find colonist on the current zone in the current map
                AssignLink link = AssignManager.links.Find(
                    x => p.Equals(x.colonist) &&
                    x.zone == AssignManager.GetActivePolicy().id &&
                    x.mapId == currentMap);

                if (link != null)
                {
                    //colonist found! save 
                    link.outfit = p.outfits.CurrentApparelPolicy;
                    link.drugPolicy = p.drugs.CurrentPolicy;
                    link.hostilityResponse = p.playerSettings.hostilityResponse;
                    link.foodPolicy = p.foodRestriction.CurrentFoodPolicy;
                    //AssignManager.SavePawnInventoryStock(p, link);
                    if (Widget_CombatExtended.CombatExtendedAvailable)
                    {
                        link.loadoutId = Widget_CombatExtended.GetLoadoutId(p);
                    }
                }
                else
                {
                    //colonist not found. So add it to the AssignLink list
                    int loadoutId = 0;
                    if (Widget_CombatExtended.CombatExtendedAvailable)
                    {
                        loadoutId = Widget_CombatExtended.GetLoadoutId(p);
                    }

                    ApparelPolicy outfit = p.outfits.CurrentApparelPolicy;
                    if (outfit == Current.Game.outfitDatabase.DefaultOutfit())
                    {
                        outfit = AssignManager.DefaultOutfit;
                    }

                    DrugPolicy drug = p.drugs.CurrentPolicy;
                    if (drug == Current.Game.drugPolicyDatabase.DefaultDrugPolicy())
                    {
                        drug = AssignManager.DefaultDrugPolicy;
                    }

                    FoodPolicy food = p.foodRestriction.CurrentFoodPolicy;
                    if (food == Current.Game.foodRestrictionDatabase.DefaultFoodRestriction())
                    {
                        food = AssignManager.DefaultFoodPolicy;
                    }

                    link = new AssignLink(
                            AssignManager.GetActivePolicy().id,
                            p,
                            outfit,
                            food,
                            drug,
                            p.playerSettings.hostilityResponse,
                            loadoutId,
                            currentMap);
                    AssignManager.links.Add(link);
                    //SavePawnInventoryStock(p, link);
                }
            }
        }

        //internal static void SavePawnInventoryStock(Pawn p, AssignLink link)
        //{
        //    if (link.stockEntries != null)
        //    {
        //        foreach (var key in p.inventoryStock.stockEntries.Keys)
        //        {
        //            InventoryStockEntry test = new InventoryStockEntry
        //            {
        //                thingDef = p.inventoryStock.stockEntries.TryGetValue(key).thingDef,
        //                count = p.inventoryStock.stockEntries.TryGetValue(key).count
        //            };
        //            link.stockEntries.SetOrAdd(key, test);
        //        }
        //    }
        //}

        //internal static void LoadPawnInventoryStock(Pawn p, AssignLink link)
        //{
        //    if (link.stockEntries != null)
        //    {
        //        foreach (var key in link.stockEntries.Keys)
        //        {
        //            p.inventoryStock.stockEntries.SetOrAdd(key, link.stockEntries.TryGetValue(key));
        //        }
        //    }
        //}
        
        internal static void CleanDeadColonists(Pawn pawn)
        {
            AssignManager.links.RemoveAll(x => x.colonist == pawn);
        }

        internal static void LinksCleanUp()
        {
            for (int i = AssignManager.links.Count - 1; i >= 0; i--)
            {
                if (AssignManager.links[i].colonist == null || !AssignManager.links[i].colonist.IsColonist)
                {
                    AssignManager.links.RemoveAt(i);
                }
            }
        }

        internal static bool ActivePoliciesContainsValidMap()
        {
            bool containsValidMap = false;
            foreach (Map map in Find.Maps)
            {
                if (AssignManager.activePolicies.Any(x => x.mapId == map.uniqueID))
                {
                    containsValidMap = true;
                    break;
                }
            }
            return containsValidMap;
        }

        internal static void CleanRemovedMaps()
        {
            for (int i = 0; i < AssignManager.activePolicies.Count; i++)
            {
                MapActivePolicy map = AssignManager.activePolicies[i];
                if (!Find.Maps.Any(x => x.uniqueID == map.mapId))
                {
                    if (Find.Maps.Count == 1 && !AssignManager.ActivePoliciesContainsValidMap())
                    {
                        //this means the player was on the move without any base
                        //and just re-settled. So, let's move the settings to
                        //the new map
                        int mapid = Find.CurrentMap.uniqueID;
                        AssignManager.MoveLinksToMap(mapid);
                        map.mapId = mapid;
                    }
                    else
                    {
                        AssignManager.DeleteLinksInMap(map.mapId);
                        AssignManager.DeleteMap(map);
                    }
                }
            }
        }
        
        internal static void UpdateState( List<AssignLink> links, List<Pawn> pawns, Policy policy)
        {
            List<AssignLink> mapLinks = null;
            List<AssignLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                foreach (AssignLink l in zoneLinks)
                {
                    if (l.colonist != null && l.colonist.GetUniqueLoadID().Equals(p.GetUniqueLoadID()))
                    {
                        l.hostilityResponse = p.playerSettings.hostilityResponse;
                        l.foodPolicy = p.foodRestriction.CurrentFoodPolicy;
                        l.outfit = p.outfits.CurrentApparelPolicy;                 
                    }
                }
            }

            AssignManager.SetActivePolicy(policy);
        }

        internal static void LoadState(List<AssignLink> links, List<Pawn> pawns, Policy policy)
        {
            List<AssignLink> mapLinks = null;
            List<AssignLink> zoneLinks = null;
            int currentMap = Find.CurrentMap.uniqueID;

            //get all links from the current map
            mapLinks = links.FindAll(x => x.mapId == currentMap);
            //get all links from the selected zone
            zoneLinks = mapLinks.FindAll(x => x.zone == policy.id);

            foreach (Pawn p in pawns)
            {
                foreach (AssignLink l in zoneLinks)
                {
                    if (l.colonist != null && l.colonist.GetUniqueLoadID().Equals(p.GetUniqueLoadID()))
                    {
                        p.outfits.CurrentApparelPolicy = OutfitExits(l.outfit) ? l.outfit : null;
                        p.drugs.CurrentPolicy = DrugPolicyExits(l.drugPolicy) ? l.drugPolicy : null;
                        p.foodRestriction.CurrentFoodPolicy = FoodPolicyExits(l.foodPolicy) ? l.foodPolicy : null;
                        p.playerSettings.hostilityResponse = l.hostilityResponse;

                        if (Widget_CombatExtended.CombatExtendedAvailable)
                        {
                            Widget_CombatExtended.SetLoadoutById(p, l.loadoutId);
                        }
                    }
                }
            }

            AssignManager.SetActivePolicy(policy);
        }

        internal static void LoadState(Policy policy)
        {
            List<Pawn> pawns = Find.CurrentMap.mapPawns.FreeColonists.ToList();
            LoadState(AssignManager.links, pawns, policy);
        }

        internal static bool OutfitExits(ApparelPolicy outfit)
        {
            foreach (ApparelPolicy current in Current.Game.outfitDatabase.AllOutfits)
            {
                if (current.Equals(outfit))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool DrugPolicyExits(DrugPolicy drugPolicy)
        {
            foreach (DrugPolicy drug in Current.Game.drugPolicyDatabase.AllPolicies)
            {
                if (drug.Equals(drugPolicy))
                {
                    return true;
                }
            }
            return false;
        }

        internal static void CopyToClipboard()
        {
            Policy policy = GetActivePolicy();
            if (AssignManager.clipboard != null)
            {
                clipboard = new List<AssignLink>();
            }

            WorkManager.clipboard.Clear();
            foreach (AssignLink link in AssignManager.links)
            {
                if (link.zone == policy.id)
                {
                    AssignManager.clipboard.Add(new AssignLink(link));
                }
            }
        }

        internal static void PasteToActivePolicy()
        {
            Policy policy = GetActivePolicy();
            if (!AssignManager.clipboard.NullOrEmpty() && AssignManager.clipboard[0].zone != policy.id)
            {
                WorkManager.links.RemoveAll(x => x.zone == policy.id);
                foreach (AssignLink copiedLink in AssignManager.clipboard)
                {
                    copiedLink.zone = policy.id;
                    AssignManager.links.Add(copiedLink);
                }
                AssignManager.LoadState(links, Find.CurrentMap.mapPawns.FreeColonists, policy);
            }
        }

        internal static void SetDefaultsForFreeColonist(Pawn p)
        {
            if (p != null && p.outfits != null && p.foodRestriction != null && p.drugs != null)
            {
                p.outfits.CurrentApparelPolicy = AssignManager.DefaultOutfit;
                p.drugs.CurrentPolicy = AssignManager.DefaultDrugPolicy;
                p.foodRestriction.CurrentFoodPolicy = AssignManager.DefaultFoodPolicy;
                //p.playerSettings.medCare = AssignManager.DefaultMedCare;
            }
        }

        internal static void SetDefaultsForPrisoner(Pawn p)
        {
            if (p!= null && p.foodRestriction != null)
            {
                p.foodRestriction.CurrentFoodPolicy = AssignManager.DefaultPrisonerFoodPolicy;
            }
            //p.playerSettings.medCare = AssignManager.DefaultPrisonerMedCare;     
        }

        internal static void SetDefaultsForSlave(Pawn p)
        {
            if (p != null && p.outfits != null && p.foodRestriction != null &&  p.drugs != null)
            {
                p.outfits.CurrentApparelPolicy = AssignManager.DefaultSlaveOutfit;
                p.drugs.CurrentPolicy = AssignManager.DefaultSlaveDrugPolicy;
                p.foodRestriction.CurrentFoodPolicy = AssignManager.DefaultSlaveFoodPolicy;
                //p.playerSettings.medCare = AssignManager.DefaultSlaveMedCare;
            }
        }

        internal static void PrintAllAssignPolicies(string spacer = "\n")
        {
            Log.Message("[BPC] === List Policies START [" + AssignManager.policies.Count + "] ===");
            foreach (Policy p in AssignManager.policies)
            {
                Log.Message("[BPC]\t" + p.ToString());
            }

            Log.Message("[BPC] === List ActivePolices START [" + AssignManager.activePolicies.Count + "] ===");
            foreach (MapActivePolicy m in AssignManager.activePolicies)
            {
                Log.Message("[BPC]\t" + m.ToString());
            }

            Log.Message("[BPC] === List links START [" + AssignManager.links.Count + "] ===");
            foreach (AssignLink assignLink in AssignManager.links)
            {
                Log.Message("[BPC]\t" + assignLink.ToString());
            }

            Log.Message(spacer);
        }
    }
}
