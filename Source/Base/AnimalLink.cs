using RimWorld;
using Verse;

namespace BetterPawnControl
{
    public class AnimalLink : Link, IExposable
    {
        //internal int zone = 0; 
        internal Pawn animal = null;
        internal Pawn master = null;
        internal Area area = null;
        internal bool followDrafted = true;
        internal bool followFieldwork = true;
        internal FoodPolicy foodPolicy = null;
        //internal int mapId = 0;

        public AnimalLink() { }

        public AnimalLink(
            int zone, Pawn animal, Pawn master, Area area, 
            bool followDrafted, bool followFieldwork, FoodPolicy foodPolicy, 
            int mapId)
        {
            this.zone = zone;
            this.animal = animal;
            this.master = master;
            this.area = area;
            this.followDrafted = followDrafted;
            this.followFieldwork = followFieldwork;
            this.foodPolicy = foodPolicy;
            this.mapId = mapId;
        }

        public override string ToString()
        {
            return 
                "Policy:" + zone + 
                "  Animal: " + animal + 
                "  Master: " + master + 
                "  Area: " + area + 
                "  FollowDrafted: " + followDrafted + 
                "  FollowFieldwork: " + followFieldwork + 
                "  MapID: " + mapId;
        }

        /// <summary>
        /// Data for saving/loading
        /// </summary>
        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref zone, "zone", 0, true);
            Scribe_References.Look<Pawn>(ref animal, "animal");
            Scribe_References.Look<Pawn>(ref master, "master");
            Scribe_References.Look<Area>(ref area, "area");
            Scribe_Values.Look<bool>(ref followDrafted, "followDrafted", true, true);
            Scribe_Values.Look<bool>(ref followFieldwork, "followFieldwork", true, true);
            Scribe_Values.Look<int>(ref mapId, "mapId", 0, true);
            Scribe_References.Look<FoodPolicy>(ref foodPolicy, "foodPolicy");
        }
    }
}
