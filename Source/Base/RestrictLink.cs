using Verse;

namespace BetterPawnControl
{
    public class RestrictLink : Link, IExposable
    {
        //internal int zone = 0;
        internal Pawn colonist = null;
        internal Area area = null;
        //internal int mapId = 0;

        public RestrictLink() { }

        public RestrictLink(int zone, Pawn colonist, Area area, int mapId)
        {
            this.zone = zone;
            this.colonist = colonist;
            this.area = area;
            this.mapId = mapId;
        }

        public override string ToString()
        {
            return 
                "Policy:" + zone +
                "  Colonist: " + colonist + 
                "  Area: " + area  + 
                "  MapID: " + mapId;
        }

        /// <summary>
        /// Data for saving/loading
        /// </summary>
        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref zone, "zone", 0, true);
            Scribe_References.Look<Pawn>(ref colonist, "colonist");
            Scribe_References.Look<Area>(ref area, "area");
            Scribe_Values.Look<int>(ref mapId, "mapId", 0, true);
        }
    }
}
