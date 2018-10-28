using System.Collections.Generic;
using Verse;
using RimWorld;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class AssignManager : Manager<AssignLink>
    {
        
        internal static List<string> Prisoners = new List<string>();

        internal static Outfit _defaultOutfit = null;
        internal static Outfit DefaultOutfit
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

        internal static FoodRestriction _defaultFoodPolicy = null;
        internal static FoodRestriction DefaultFoodPolicy
        {
            get
            {
                if (_defaultFoodPolicy == null)
                {
                    _defaultFoodPolicy = Current.Game.foodRestrictionDatabase.DefaultFoodRestriction();
                }
                return _defaultFoodPolicy;
            }

            set
            {
                _defaultFoodPolicy = value;
            }
        }

        internal static FoodRestriction _defaultPrisonerFoodPolicy = null;
        internal static FoodRestriction DefaultPrisonerFoodPolicy
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
    }
}
