using Verse;

namespace BetterPawnControl
{
    public class MapActivePolicy : IExposable
    {
        internal int mapId = 0;
        internal Policy activePolicy = null;

        public MapActivePolicy() { }

        public MapActivePolicy(int mapId, Policy activePolicy)
        {
            this.mapId = mapId;
            this.activePolicy = activePolicy;
        }

        public override string ToString()
        {
            return "mapId:" + mapId + "  activePolicy: " + activePolicy;
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue<int>(ref mapId, "mapId", 0, true);
            Scribe_Deep.LookDeep<Policy>(ref activePolicy, "activePolicy");
        }
    }
}