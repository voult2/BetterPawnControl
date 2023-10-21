using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
    public class ScheduleLink : Link, IExposable
    {
        //internal int zone = 0;
        internal Pawn colonist = null;
        internal Area area = null;
        internal List<TimeAssignmentDef> schedule;
        //internal int mapId = 0;

        public ScheduleLink() { }

        public ScheduleLink(ScheduleLink link)
        {
            this.zone = link.zone;
            this.colonist = link.colonist;
            this.area = link.area;
            if (link.schedule != null)
            {
                this.schedule = new List<TimeAssignmentDef>(link.schedule);
            }
            this.mapId = link.mapId;
        }

        public ScheduleLink(int zone, Pawn colonist, Area area, List<TimeAssignmentDef> times, int mapId)
        {
            this.zone = zone;
            this.colonist = colonist;
            this.area = area;
            if (times != null)
            {
                this.schedule = new List<TimeAssignmentDef>(times);
            }
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
            Scribe_Collections.Look(ref schedule, "schedule", LookMode.Def);
            if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs && schedule == null && colonist.timetable != null)
            {
                //this means the current save does not contain schedule data. So let's start new
                this.schedule = new List<TimeAssignmentDef>(colonist.timetable.times);
            }
            Scribe_Values.Look<int>(ref mapId, "mapId", 0, true);
        }
    }
}
