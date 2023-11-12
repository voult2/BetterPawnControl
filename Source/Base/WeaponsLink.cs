using Verse;

namespace BetterPawnControl
{
    public class WeaponsLink : Link, IExposable
    {
        //internal int zone = 0; 
        internal Pawn colonist = null;       
        internal int loadoutId = -1;

        public WeaponsLink() { }

        public WeaponsLink(int zone, Pawn colonist, int loadoutId, int mapId)
        {
            this.zone = zone;
            this.colonist = colonist;
            this.loadoutId = loadoutId;
            this.mapId = mapId;
        }

        public override string ToString()
        {
            return 
                "Policy:" + zone +
                "  Colonist: " + colonist + 
                "  LoudoutId: " + loadoutId +
                "  MapID: " + mapId;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref zone, "zone", 0, true);
            Scribe_References.Look<Pawn>(ref colonist, "pawn");
            Scribe_Values.Look<int>(ref loadoutId, "loadoutId", -1, true);
            Scribe_Values.Look<int>(ref mapId, "mapId", 0, true);
        }
    }
}