using Verse;

namespace BetterPawnControl
{
    public class AnimalLink : IExposable
    {
        internal int zone = 0; 
        internal Pawn animal = null;
        internal Pawn master = null;
        internal Area area = null;
        internal bool followDrafted = true;
        internal bool followFieldwork = true;
        internal int mapId = 0;

        public AnimalLink() { }

        public AnimalLink(int zone, Pawn animal, Pawn master, Area area, bool followDrafted, bool followFieldwork, int mapId)
        {
            this.zone = zone;
            this.animal = animal;
            this.master = master;
            this.area = area;
            this.followDrafted = followDrafted;
            this.followFieldwork = followFieldwork;
            this.mapId = mapId;
        }

        public override string ToString()
        {
            return "Policy:" + zone + "  Animal: " + animal + "  Master: " + master + "  Area: " + area + " FollowDrafted: " + followDrafted + " FollowFieldwork: " + followFieldwork + " MapID: " + mapId;
        }

        /// <summary>
        /// Data for saving/loading
        /// </summary>
        public void ExposeData()
        {
            Scribe_Values.LookValue<int>(ref zone, "zone", 0, true);
            Scribe_References.LookReference<Pawn>(ref animal, "animal");
            Scribe_References.LookReference<Pawn>(ref master, "master");
            Scribe_References.LookReference<Area>(ref area, "area");
            Scribe_Values.LookValue<bool>(ref followDrafted, "followDrafted", true, true);
            Scribe_Values.LookValue<bool>(ref followFieldwork, "followFieldwork", true, true);
            Scribe_Values.LookValue<int>(ref mapId, "mapId", 0, true);
        }
    }
}
