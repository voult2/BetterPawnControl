using HarmonyLib;
using RimWorld;
using System.Linq;

namespace BetterPawnControl
{
    [HarmonyPatch(typeof(MainTabWindow_PawnTable), nameof(MainTabWindow_PawnTable.PostOpen))]
    static class MainTabWindow_PawnTable_OnPostOpen
    {
        static void Postfix()
        {
            //AssignManager.UpdateState(AssignManager.links, AssignManager.Colonists().ToList(), AssignManager.GetActivePolicy());
            AssignManager.LoadState(AssignManager.links, AssignManager.Colonists().ToList(), AssignManager.GetActivePolicy());
            AssignManager.showPaste = false;
    
            //ScheduleManager.UpdateState(ScheduleManager.links, ScheduleManager.Colonists().ToList(), ScheduleManager.GetActivePolicy());
            ScheduleManager.LoadState(ScheduleManager.links, ScheduleManager.Colonists().ToList(), ScheduleManager.GetActivePolicy());
            ScheduleManager.showPaste = false;

            if( !Widget_ModsAvailable.DisableBPCOnWorkTab)
            {
                WorkManager.LoadState(WorkManager.links, WorkManager.Colonists().ToList(), WorkManager.GetActivePolicy());
                WorkManager.showPaste = false;
            }

            //AnimalManager.UpdateState(AnimalManager.links, AnimalManager.Animals().ToList(), AnimalManager.GetActivePolicy());
            AnimalManager.LoadState(AnimalManager.links, AnimalManager.Animals().ToList(), AnimalManager.GetActivePolicy());

            if (MechManager.DirtyPolicy)
            {
                //In case the auto-repair gizmo was opened. It would be great to find a better solution. 
                MechManager.UpdateState(MechManager.links, MechManager.Mechs().ToList(), MechManager.GetActivePolicy());
            }                
            MechManager.LoadState(MechManager.links, MechManager.Mechs().ToList(), MechManager.GetActivePolicy());
        }
    }
}