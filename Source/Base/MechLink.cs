using Verse;

namespace BetterPawnControl
{
    public class MechLink : Link, IExposable
    {
        //internal int zone = 0; 
        internal Pawn mech = null;       
        //internal bool autorepair = true;
        internal int controlGroupIndex = 0;
        internal MechWorkModeDef workmode = null;
        internal Area area = null;

        public MechLink() { }

        public MechLink(
            int zone, Pawn mech, int controlGroupIndex, MechWorkModeDef workmode, Area area, int mapId)
        {
            this.zone = zone;
            this.mech = mech;
            //this.autorepair = autorepair;
            this.controlGroupIndex = controlGroupIndex;
            this.workmode = workmode;
            this.area = area;
            this.mapId = mapId;
        }

        public override string ToString()
        {
            return 
                "Policy:" + zone + 
                "  Mech: " + mech + 
                "  ControlGroupIndex: " + controlGroupIndex +
                "  WorkMode: " + workmode +
                "  Area: " + area +
                "  MapID: " + mapId;
        }

        /// <summary>
        /// Data for saving/loading
        /// </summary>
        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref zone, "zone", 0, true);
            Scribe_References.Look<Pawn>(ref mech, "mech");
            Scribe_Values.Look<int>(ref controlGroupIndex, "controlGroupIndex", 0, true);
            Scribe_Defs.Look<MechWorkModeDef>(ref workmode, "workmode");
            Scribe_References.Look<Area>(ref area, "area");
            Scribe_Values.Look<int>(ref mapId, "mapId", 0, true);
        }
    }
}