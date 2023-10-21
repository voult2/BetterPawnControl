using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
    public class WorkLink : Link, IExposable
    {
        //internal int zone = 0;
        internal Pawn colonist = null;
        internal Dictionary<WorkTypeDef, int> settings =  null;
        internal Dictionary<WorkGiverDef, List<int>> settingsInner = null;
        //internal int mapId = 0;

        public WorkLink() { }

        public WorkLink(WorkLink link)
        {
            this.zone = link.zone;
            this.colonist = link.colonist;
            this.settings = new Dictionary<WorkTypeDef, int>(link.settings);
            this.settingsInner = new Dictionary<WorkGiverDef, List<int>>(link.settingsInner);
            this.mapId = link.mapId;
        }

        public WorkLink(int zone, Pawn colonist, Dictionary<WorkTypeDef, int> settings, Dictionary<WorkGiverDef, List<int>> settingsInner, int mapId)
        {
            this.zone = zone;
            this.colonist = colonist;
            this.settings = settings;
            this.settingsInner = settingsInner;
            this.mapId = mapId;
        }

        public override string ToString()
        {
            return
                "Policy:" + zone +
                "  Pawn: " + colonist +
                //"  WorkSettings: " + settings +
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

            List<WorkGiverDef> keysInner = new List<WorkGiverDef>();
            List<List<int>> valuesInner = new List<List<int>>();

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                foreach (KeyValuePair<WorkTypeDef, int> entry in settings)
                {
                    keys.Add(entry.Key);
                    values.Add(entry.Value);
                }
                Scribe_Collections.Look(ref settings, "settings", LookMode.Def, LookMode.Value, ref keys, ref values);
                
                foreach (KeyValuePair<WorkGiverDef, List<int>> entryInner in settingsInner)
                {
                    keysInner.Add(entryInner.Key);
                    valuesInner.Add(entryInner.Value);
                }
                Scribe_Collections.Look(ref settingsInner, "settingsInner", LookMode.Def, LookMode.Value, ref keysInner, ref valuesInner);
            }

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                Scribe_Collections.Look(ref settings, "settings", LookMode.Def, LookMode.Value, ref keys, ref values);
                Scribe_Collections.Look(ref settingsInner, "settingsInner", LookMode.Def, LookMode.Value, ref keysInner, ref valuesInner);
            }
        }
    }
}
