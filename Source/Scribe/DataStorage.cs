using HugsLib;
using HugsLib.Utils;
using Verse;

namespace BetterPawnControl
{
    public class DataStorage : ModBase
    {
        public override string ModIdentifier
        {
            get { return "BetterPawnControl"; }
        }

        public override void WorldLoaded()
        {
            var obj = UtilityWorldObjectManager.GetUtilityWorldObject<WorldDataStore>();

        }

        private class WorldDataStore : UtilityWorldObject
        {
            public override void ExposeData()
            {
                base.ExposeData();

                if (Scribe.mode == LoadSaveMode.LoadingVars || Scribe.mode == LoadSaveMode.Saving)
                {
                    Scribe_Collections.Look<Policy>(ref AssignManager.policies, "AssignPolicies", LookMode.Deep);
                    Scribe_Collections.Look<AssignLink>(ref AssignManager.links, "AssignLinks", LookMode.Deep);
                    Scribe_Collections.Look<MapActivePolicy>(ref AssignManager.activePolicies, "AssignActivePolicies", LookMode.Deep);

                    Scribe_Collections.Look<Policy>(ref AnimalManager.policies, "AnimalPolicies", LookMode.Deep);
                    Scribe_Collections.Look<AnimalLink>(ref AnimalManager.links, "AnimalLinks", LookMode.Deep);
                    Scribe_Collections.Look<MapActivePolicy>(ref AnimalManager.activePolicies, "AnimalActivePolicies", LookMode.Deep);
                }
            }
        }
    }
}
