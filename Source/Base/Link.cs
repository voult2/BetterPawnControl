using Verse;

namespace BetterPawnControl
{
    public class Link
    {
        internal int zone = 0;
        internal int mapId = 0;

        public Link() { }

        public Link(
            int zone, int mapId)
        {
            this.zone = zone;
            this.mapId = mapId;
        }

        public override string ToString()
        {
            return
                "Policy:" + zone +
                "  MapID: " + mapId;
        }
    }
}
