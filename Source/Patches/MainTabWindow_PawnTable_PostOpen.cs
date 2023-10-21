using System.Linq;
using HarmonyLib;
using RimWorld;

namespace BetterPawnControl
{
    [HarmonyPatch(typeof(MainTabWindow_PawnTable), nameof(MainTabWindow_PawnTable.PostOpen))]
    static class MainTabWindow_PawnTable_OnPostOpen
    {
        static void Postfix()
        {
            AssignManager.LoadState(AssignManager.links, AssignManager.Colonists().ToList(), AssignManager.GetActivePolicy());
            AssignManager.showPaste = false;
    
            ScheduleManager.LoadState(ScheduleManager.links, ScheduleManager.Colonists().ToList(), ScheduleManager.GetActivePolicy());
            ScheduleManager.showPaste = false;

            //if(!Widget_ModsAvailable.DisableBPCOnWorkTab)
            //{
            WorkManager.LoadState(WorkManager.links, WorkManager.Colonists().ToList(), WorkManager.GetActivePolicy());
            WorkManager.showPaste = false;
            //}

            AnimalManager.LoadState(AnimalManager.links, AnimalManager.Animals().ToList(), AnimalManager.GetActivePolicy());
              
            MechManager.LoadState(MechManager.links, MechManager.Mechs().ToList(), MechManager.GetActivePolicy());
        }
    }
}