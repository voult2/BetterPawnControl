using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace BetterPawnControl
{
    public class WorkLink : IExposable
    {
        internal int zone = 0;
        internal Pawn colonist = null;
        internal Dictionary<WorkTypeDef, int> settings =  null;
        internal int mapId = 0;

        public WorkLink() { }

        public WorkLink(
            int zone, Pawn colonist, Dictionary<WorkTypeDef, int> settings, int mapId)
        {
            this.zone = zone;
            this.colonist = colonist;
            this.settings = settings;
            this.mapId = mapId;
        }

        public override string ToString()
        {
            return
                "Policy:" + zone +
                "  Pawn: " + colonist +
                "  WorkSettings: " + settings +
                "  MapID: " + mapId;
        }

        /// <summary>
        /// Data for saving/loading
        /// </summary>
        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref zone, "zone", 0, true);
            Scribe_References.Look<Pawn>(ref colonist, "colonist");
            Scribe_Values.Look<int>(ref mapId, "mapId", 0, true);

            List<WorkTypeDef> keys = new List<WorkTypeDef>();
            List<int> values = new List<int>();

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                foreach (KeyValuePair<WorkTypeDef, int> entry in settings)
                {
                    keys.Add(entry.Key);
                    values.Add(entry.Value);
                }
                Scribe_Collections.Look(ref settings, "settings", LookMode.Def, LookMode.Value, ref keys, ref values);
            }

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Scribe_Collections.Look(ref settings, "settings", LookMode.Def, LookMode.Value, ref keys, ref values);
            }
        }
    }
}
