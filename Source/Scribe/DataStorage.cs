using HugsLib;
using HugsLib.Utils;
using Verse;
using RimWorld;
using System.Collections.Generic;

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
            var obj = 
                UtilityWorldObjectManager.GetUtilityWorldObject<WorldDataStore>();
        }

        private class WorldDataStore : UtilityWorldObject
        {
            public override void ExposeData()
            {
                base.ExposeData();

                if (Scribe.mode == LoadSaveMode.LoadingVars ||
                    Scribe.mode == LoadSaveMode.Saving)
                {
                    Scribe_References.Look<Outfit>(
                        ref AssignManager._defaultOutfit, 
                        "DefaultOutfit");

                    Scribe_References.Look<DrugPolicy>(
                        ref AssignManager._defaultDrugPolicy, 
                        "DefaultDrugPolicy");

                    Scribe_References.Look<FoodRestriction>(
                        ref AssignManager._defaultFoodPolicy, 
                        "DefaultFoodPolicy");

                    Scribe_References.Look<FoodRestriction>(
                        ref AssignManager._defaultPrisonerFoodPolicy, 
                        "DefaultPrisonerFoodPolicy");

                    Scribe_Collections.Look<Policy>(
                        ref AssignManager.policies,
                        "AssignPolicies", LookMode.Deep);

                    Scribe_Collections.Look<AssignLink>(
                        ref AssignManager.links,
                        "AssignLinks", LookMode.Deep);

                    if (AssignManager.links == null)
                    {
                        //this is only required if the save file contains
                        //empty links
                        AssignManager.InstantiateLinks();
                    }

                    Scribe_Collections.Look<string>(
                        ref AssignManager.Prisoners,
                        "Prisoners", LookMode.Value);

                    if (AssignManager.Prisoners == null)
                    {
                        //this is only required if the save file contains
                        //empty prisoners
                        AssignManager.InstantiatePrisoners();
                    }

                    Scribe_Collections.Look<MapActivePolicy>(
                        ref AssignManager.activePolicies,
                        "AssignActivePolicies", LookMode.Deep);

                    Scribe_Collections.Look<Policy>(
                        ref AnimalManager.policies,
                        "AnimalPolicies", LookMode.Deep);

                    Scribe_Collections.Look<AnimalLink>(
                        ref AnimalManager.links,
                        "AnimalLinks", LookMode.Deep);

                    if (AnimalManager.links == null)
                    {
                        //this is only required if the save file contains
                        //empty links. Not sure how this can happen though :(
                        AnimalManager.InstantiateLinks();
                    }

                    Scribe_Collections.Look<MapActivePolicy>(
                        ref AnimalManager.activePolicies,
                        "AnimalActivePolicies", LookMode.Deep);

                    Scribe_Collections.Look<Policy>(
                        ref RestrictManager.policies,
                        "RestrictPolicies", LookMode.Deep);

                    Scribe_Collections.Look<RestrictLink>(
                        ref RestrictManager.links,
                        "RestrictLinks", LookMode.Deep);

                    if (RestrictManager.links == null)
                    {
                        //this is only required if the save file contains
                        //empty links. Not sure how this can happen though :(
                        RestrictManager.InstantiateLinks();
                    }

                    Scribe_Collections.Look<Policy>(
                        ref WorkManager.policies,
                        "WorkPolicies", LookMode.Deep);

                    Scribe_Collections.Look<WorkLink>(
                        ref WorkManager.links,
                        "WorkLinks", LookMode.Deep);

                    Scribe_Collections.Look<MapActivePolicy>(
                        ref WorkManager.activePolicies,
                        "WorkActivePolicies", LookMode.Deep);

                    if (Scribe.mode == LoadSaveMode.LoadingVars && 
                        WorkManager.activePolicies == null)
                    {
                        //this only happens with existing saves. New saves have
                        //no WorkPolicy data so let's initialize!
                        WorkManager.ForceInit();
                    }
                }

                if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
                {
                    Scribe_References.Look<Outfit>(
                        ref AssignManager._defaultOutfit, 
                        "DefaultOutfit");

                    Scribe_References.Look<DrugPolicy>(
                        ref AssignManager._defaultDrugPolicy, 
                        "DefaultDrugPolicy");

                    Scribe_References.Look<FoodRestriction>(
                        ref AssignManager._defaultFoodPolicy, 
                        "DefaultFoodPolicy");

                    Scribe_References.Look<FoodRestriction>(
                        ref AssignManager._defaultPrisonerFoodPolicy, 
                        "DefaultPrisonerFoodPolicy");
                }
            }
        }
    }
}
