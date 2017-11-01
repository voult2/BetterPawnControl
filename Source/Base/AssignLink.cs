using RimWorld;
using Verse;

namespace BetterPawnControl
{
    public class AssignLink : IExposable
    {
        internal int zone = 0;
        internal Pawn colonist = null;
        internal Outfit outfit = null;
        internal DrugPolicy drugPolicy = null;
        internal HostilityResponseMode hostilityResponse =
            HostilityResponseMode.Flee;
        internal int loadoutId = 1;
        internal int mapId = 0;

        public AssignLink() { }

        public AssignLink(
            int zone, Pawn colonist, Outfit outfit, 
            DrugPolicy drugPolicy, HostilityResponseMode hostilityResponse, 
            int loadoutId, int mapId)
        {
            this.zone = zone;
            this.colonist = colonist;
            this.outfit = outfit;
            this.drugPolicy = drugPolicy;
            this.hostilityResponse = hostilityResponse;
            this.loadoutId = loadoutId;
            this.mapId = mapId;
        }

        public override string ToString()
        {
            string tmp = outfit?.label;
            string tmp2 = drugPolicy?.label;
            return 
                "Policy:" + zone + 
                "  Pawn: " + colonist + 
                "  Outfit: " + tmp + 
                "  DrugPolicy: " + tmp2 + 
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
            Scribe_References.Look<Outfit>(ref outfit, "outfit");
            Scribe_References.Look<DrugPolicy>(ref drugPolicy, "drugPolicy");
            Scribe_Values.Look<HostilityResponseMode>(
                ref hostilityResponse, 
                "hostilityResponse", HostilityResponseMode.Flee, true);
            Scribe_Values.Look<int>(ref loadoutId, "loadoutId", 1, true);
            Scribe_Values.Look<int>(ref mapId, "mapId", 0, true);
        }
    }
}
