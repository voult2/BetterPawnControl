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
                        //empty links
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
                        //empty links
                        RestrictManager.InstantiateLinks();
                    }

                    Scribe_Collections.Look<MapActivePolicy>(
                        ref RestrictManager.activePolicies,
                        "RestrictActivePolicies", LookMode.Deep);
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
