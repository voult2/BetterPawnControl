using RimWorld;
using Verse;

namespace BetterPawnControl
{
    public class AssignLink : Link, IExposable
    {
        //internal int zone = 0;
        internal Pawn colonist = null;
        internal ApparelPolicy outfit = null;
        internal FoodPolicy foodPolicy = null;
        internal DrugPolicy drugPolicy = null;
        internal HostilityResponseMode hostilityResponse = HostilityResponseMode.Flee;
        internal int loadoutId = 1;
        //internal int mapId = 0;

        public AssignLink() { }

        public AssignLink(
            int zone, Pawn colonist, ApparelPolicy outfit, FoodPolicy foodPolicy,
            DrugPolicy drugPolicy, HostilityResponseMode hostilityResponse, 
            int loadoutId, int mapId)
        {
            this.zone = zone;
            this.colonist = colonist;
            this.outfit = outfit;
            this.foodPolicy = foodPolicy;
            this.drugPolicy = drugPolicy;
            this.hostilityResponse = hostilityResponse;
            this.loadoutId = loadoutId;
            this.mapId = mapId;
        }

        public AssignLink(AssignLink link)
        {
            this.zone = link.zone;
            this.colonist = link.colonist;
            this.outfit = link.outfit;
            this.foodPolicy = link.foodPolicy;
            this.drugPolicy = link.drugPolicy;
            this.hostilityResponse = link.hostilityResponse;
            this.loadoutId = link.loadoutId;
            this.mapId = link.mapId;
        }

        public override string ToString()
        {
            string outifit = outfit?.label;
            string drug = drugPolicy?.label;
            string food = foodPolicy?.label;
            return
                "PolicyID:" + zone +
                "  Pawn: " + colonist +
                "  Outfit: " + outifit +
                "  Food: " + food +
                "  DrugPolicy: " + drug +
                "  HostilityResponse: " + hostilityResponse +
                "  LoadoutId: " + loadoutId +
                "  MapID: " + mapId;
        }



        /// <summary>
        /// Data for saving/loading
        /// </summary>
        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref zone, "zone", 0, true);
            Scribe_References.Look<Pawn>(ref colonist, "colonist");
            Scribe_References.Look<ApparelPolicy>(ref outfit, "outfit");
            Scribe_References.Look<FoodPolicy>(ref foodPolicy, "foodPolicy");
            Scribe_References.Look<DrugPolicy>(ref drugPolicy, "drugPolicy");
            Scribe_Values.Look<HostilityResponseMode>(ref hostilityResponse, "hostilityResponse", HostilityResponseMode.Flee, true);
            Scribe_Values.Look<int>(ref loadoutId, "loadoutId", 1, true);
            if (Scribe.mode == LoadSaveMode.LoadingVars && loadoutId == 0)
            {
                this.loadoutId = 1;                
            }
            Scribe_Values.Look<int>(ref mapId, "mapId", 0, true);
        }
    }
}
