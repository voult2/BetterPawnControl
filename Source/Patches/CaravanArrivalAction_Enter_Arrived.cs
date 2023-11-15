using HarmonyLib;
using RimWorld.Planet;

namespace BetterPawnControl
{

    [HarmonyPatch(typeof(CaravanArrivalAction_Enter), nameof(CaravanArrivalAction_Enter.Arrived))]
    internal static class CaravanArrivalAction_Enter_Arrived
    {
        private static void Postfix(MapParent ___mapParent)
        {
            if(___mapParent.Map.IsPlayerHome)
            {
                AssignManager.LoadState(AssignManager.GetActivePolicy());
                ScheduleManager.LoadState(ScheduleManager.GetActivePolicy());   
                AnimalManager.LoadState(AnimalManager.GetActivePolicy());
                MechManager.LoadState(MechManager.GetActivePolicy());

                if (!Widget_ModsAvailable.DisableBPCOnWorkTab)
                {
                    WorkManager.LoadState(WorkManager.GetActivePolicy());
                }
                              
                if (Widget_ModsAvailable.WTBAvailable)
                {
                    WeaponsManager.LoadState(WeaponsManager.GetActivePolicy());
                }
            }
        }
    }
}   