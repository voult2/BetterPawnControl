using System.Text;
using Verse;

namespace BetterPawnControl
{
    public class RobotLink : Link, IExposable
    {
        internal Pawn robot = null;
        internal Area area = null;

        public RobotLink()
        {
        }

        public RobotLink(int zone, Pawn robot, Area area, int mapId)
        {
            this.zone = zone;
            this.robot = robot;
            this.area = area;
            this.mapId = mapId;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref zone, "zone", 0, true);
            Scribe_References.Look<Pawn>(ref robot, "robot");
            Scribe_References.Look<Area>(ref area, "area");
            Scribe_Values.Look<int>(ref mapId, "mapId", 0, true);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Policy:{zone}");
            sb.Append($"  Robot: {robot}");
            sb.Append($"  Area: {area}");
            sb.Append($"  MapID: {mapId}");
            return sb.ToString();
        }
    }
}
