using Verse;
using RimWorld;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    class AssignManager : Manager<AssignLink>
    {
        internal static Outfit _defaultOutfit = null;
        internal static Outfit DefaultOutfit
        {
            get
            {
                if (_defaultOutfit == null)
                {
                    
                    _defaultOutfit = Current.Game.outfitDatabase.AllOutfits[0];
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
                    _defaultDrugPolicy = Current.Game.drugPolicyDatabase.AllPolicies[0];
                }
                return _defaultDrugPolicy;
            }

            set
            {
                _defaultDrugPolicy = value;
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
