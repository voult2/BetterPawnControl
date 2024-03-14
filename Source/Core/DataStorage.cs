using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BetterPawnControl
{
    public class DataStorage : WorldComponent
	{
		public DataStorage(World world) : base(world)
		{
		}

		public override void ExposeData()
		{
			base.ExposeData();

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                //Let's make sure all static variables are cleaned.
                //This shows there's a fundamental problem with this code
                //A code refractor is require to remove the Static Managers 
                //and replace it with GameComponents
                AssignManager.links = null;
                AssignManager.policies = null;
                AssignManager.activePolicies = null;
                AssignManager.DefaultDrugPolicy = null;
                AssignManager.DefaultFoodPolicy = null;
                AssignManager.DefaultOutfit = null;
                AssignManager.DefaultPrisonerFoodPolicy = null;
                AssignManager.DefaultSlaveFoodPolicy = null;
                AssignManager.DefaultSlaveOutfit = null;
                AnimalManager.links = null;
                AnimalManager.policies = null;
                AnimalManager.activePolicies = null;
                WorkManager.links = null;
                WorkManager.policies = null;
                WorkManager.activePolicies = null;
                ScheduleManager.links = null;
                ScheduleManager.policies = null;
                ScheduleManager.activePolicies = null;
                MechManager.links = null;
                MechManager.policies = null;
                MechManager.activePolicies = null;
                AlertManager.alertLevelsList = null;
				WeaponsManager.links = null;
				WeaponsManager.policies = null;
				WeaponsManager.activePolicies = null;
				System.GC.Collect();
            }

            if (Scribe.mode == LoadSaveMode.LoadingVars || Scribe.mode == LoadSaveMode.Saving)
			{
				Scribe_References.Look<ApparelPolicy>(ref AssignManager._defaultOutfit,"DefaultOutfit");
				Scribe_References.Look<FoodPolicy>(ref AssignManager._defaultFoodPolicy, "DefaultFoodPolicy");
				Scribe_References.Look<DrugPolicy>(ref AssignManager._defaultDrugPolicy, "DefaultDrugPolicy");				
				Scribe_Values.Look<MedicalCareCategory>(ref AssignManager._defaultMedCare, "DefaultColonistMedCare");

				Scribe_References.Look<FoodPolicy>(ref AssignManager._defaultPrisonerFoodPolicy, "DefaultPrisonerFoodPolicy");
				Scribe_Values.Look<MedicalCareCategory>(ref AssignManager._defaulPrisonerMedCare, "DefaultPrisionerMedCare");

				if (ModsConfig.IdeologyActive)
				{
					Scribe_References.Look<ApparelPolicy>(ref AssignManager._defaultSlaveOutfit, "DefaultSlaveOutfit");
					Scribe_References.Look<FoodPolicy>(ref AssignManager._defaultSlaveFoodPolicy, "DefaultSlaveFoodPolicy");
					Scribe_References.Look<DrugPolicy>(ref AssignManager._defaultSlaveDrugPolicy, "DefaultSlaveDrugPolicy");
					Scribe_Values.Look<MedicalCareCategory>(ref AssignManager._defaulSlaveMedCare, "DefaultSlaveMedCare");
				}
				
				Scribe_Collections.Look<Policy>(ref AssignManager.policies,"AssignPolicies", LookMode.Deep);
				Scribe_Collections.Look<AssignLink>(ref AssignManager.links, "AssignLinks", LookMode.Deep);
				Scribe_Collections.Look<MapActivePolicy>(ref AssignManager.activePolicies, "AssignActivePolicies", LookMode.Deep);
				
				Scribe_Collections.Look<Policy>(ref AnimalManager.policies, "AnimalPolicies", LookMode.Deep);
				Scribe_Collections.Look<AnimalLink>(ref AnimalManager.links, "AnimalLinks", LookMode.Deep);
				Scribe_Collections.Look<MapActivePolicy>(ref AnimalManager.activePolicies, "AnimalActivePolicies", LookMode.Deep);
				
				Scribe_Collections.Look<Policy>(ref ScheduleManager.policies, "RestrictPolicies", LookMode.Deep);
				Scribe_Collections.Look<ScheduleLink>(ref ScheduleManager.links, "ScheduleLinks", LookMode.Deep);
                Scribe_Collections.Look<MapActivePolicy>(ref ScheduleManager.activePolicies, "RestrictActivePolicies", LookMode.Deep);
                
				Scribe_Collections.Look<Policy>(ref WorkManager.policies, "WorkPolicies", LookMode.Deep);
				Scribe_Collections.Look<WorkLink>(ref WorkManager.links, "WorkLinks", LookMode.Deep);
				Scribe_Collections.Look<MapActivePolicy>(ref WorkManager.activePolicies, "WorkActivePolicies", LookMode.Deep);
				
				Scribe_Collections.Look<Policy>(ref MechManager.policies, "MechPolicies", LookMode.Deep);
				Scribe_Collections.Look<MechLink>(ref MechManager.links, "MechLinks", LookMode.Deep);
				Scribe_Collections.Look<MapActivePolicy>(ref MechManager.activePolicies, "MechActivePolicies", LookMode.Deep);

				if (Widget_ModsAvailable.WTBAvailable)
                {
					Scribe_Collections.Look<Policy>(ref WeaponsManager.policies, "WeaponsPolicies", LookMode.Deep);
					Scribe_Collections.Look<WeaponsLink>(ref WeaponsManager.links, "WeaponsLinks", LookMode.Deep);
					Scribe_Collections.Look<MapActivePolicy>(ref WeaponsManager.activePolicies, "WeaponsActivePolicies", LookMode.Deep);
				}

				Scribe_Values.Look<int>(ref AlertManager._alertLevel, "ActiveLevel", 0, true);
				Scribe_Collections.Look<AlertLevel>(ref AlertManager.alertLevelsList, "AlertLevelsList", LookMode.Deep);

				if (Scribe.mode == LoadSaveMode.LoadingVars && WorkManager.activePolicies == null)
				{
					//this only happens with existing saves. Existing saves have no WorkPolicy data so let's initialize!
					WorkManager.ForceInit();
				}

                if (Scribe.mode == LoadSaveMode.LoadingVars && MechManager.activePolicies == null)
                {
					//this only happens with existing saves. Existing saves have no related data so let's initialize!
					MechManager.ForceInit();
                }

				if (Scribe.mode == LoadSaveMode.LoadingVars && WeaponsManager.activePolicies == null)
				{
					//this only happens with existing saves. Existing saves have no related data so let's initialize!
					WeaponsManager.ForceInit();
				}
			}

			if (Scribe.mode == LoadSaveMode.ResolvingCrossRefs)
			{
				Scribe_References.Look<ApparelPolicy>(ref AssignManager._defaultOutfit, "DefaultOutfit");
				Scribe_References.Look<DrugPolicy>(ref AssignManager._defaultDrugPolicy, "DefaultDrugPolicy");
				Scribe_References.Look<FoodPolicy>(ref AssignManager._defaultFoodPolicy, "DefaultFoodPolicy");
				Scribe_Values.Look<MedicalCareCategory>(ref AssignManager._defaultMedCare, "DefaultMedCare");

				Scribe_References.Look<FoodPolicy>(ref AssignManager._defaultPrisonerFoodPolicy, "DefaultPrisonerFoodPolicy");
				Scribe_Values.Look<MedicalCareCategory>(ref AssignManager._defaulPrisonerMedCare, "DefaultPrisonerMedCare");

				Scribe_Values.Look<int>(ref WeaponsManager._defaultLoadoutId, "DefaultWeaponsLoadout");

				Scribe_References.Look<ApparelPolicy>(ref AssignManager._defaultSlaveOutfit, "DefaultSlaveOutfit");
				Scribe_References.Look<FoodPolicy>(ref AssignManager._defaultSlaveFoodPolicy, "DefaultSlaveFoodPolicy");
				Scribe_References.Look<DrugPolicy>(ref AssignManager._defaultSlaveDrugPolicy, "DefaultSlaveDrugPolicy");
				Scribe_Values.Look<MedicalCareCategory>(ref AssignManager._defaulSlaveMedCare, "DefaultSlaveMedCare");
			}
		}
    }
}
