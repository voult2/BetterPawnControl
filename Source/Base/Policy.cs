using Verse;

namespace BetterPawnControl
{
    public class Policy : IExposable, IRenameable
    {
        internal int id = 0;
        public string label = "BPC.Auto".Translate();

        public string RenamableLabel
        {
            get => label;
            set => label = value;
        }

        public string BaseLabel => label;

        public string InspectLabel => RenamableLabel;

        public Policy() { }

        public Policy(int id, string label)
        {
            this.id = id;
            this.label = label;
        }

        public override string ToString()
        {
            return "Id:" + id + "  Label: " + label;
        }

        public virtual bool Equals(Policy other)
        {
            return this.id == other.id && this.label == other.label;
        }

        /// <summary>
        /// Data for saving/loading
        /// </summary>
        public void ExposeData()
        {
            Scribe_Values.Look<string>(ref label, "label", "Default", true);
            Scribe_Values.Look<int>(ref id, "id", 0, true);
        }
    }
}