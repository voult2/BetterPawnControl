using System.Linq;
using HarmonyLib;
using RimWorld;

namespace BetterPawnControl
{
    [HarmonyPatch(typeof(MainTabWindow_PawnTable), nameof(MainTabWindow_PawnTable.PostOpen))]
    static class MainTabWindow_PawnTable_OnPostOpen
    {

        private const string WORKTAB_MAINTAB = "WorkTab.MainTabWindow_WorkTab";
        private const string NUMBERS_MAINTAB = "Numbers.MainTabWindow_Numbers";
        private const string ANIMALTAB_MAINTAB = "AnimalTab.MainTabWindow_Animals";
        private const string NUMBERS_DEFNAME = "Numbers.MainTabWindow_NumbersAnimals";

        static void Postfix(MainTabWindow_PawnTable __instance)
        {

            if (__instance.GetType().Equals(typeof(MainTabWindow_Assign)))
            {
                AssignManager.LoadState(AssignManager.links, AssignManager.Colonists().ToList(), AssignManager.GetActivePolicy());
                AssignManager.showPaste = false;
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Schedule)))
            {

                ScheduleManager.LoadState(ScheduleManager.links, ScheduleManager.Colonists().ToList(), ScheduleManager.GetActivePolicy());
                ScheduleManager.showPaste = false;
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Work)) || __instance.GetType().FullName.Equals(WORKTAB_MAINTAB) || __instance.GetType().FullName.Equals(NUMBERS_MAINTAB))
            {
                if (!Widget_ModsAvailable.DisableBPCOnWorkTab)
                {
                    WorkManager.LoadState(WorkManager.links, WorkManager.Colonists().ToList(), WorkManager.GetActivePolicy());
                    WorkManager.showPaste = false;
                }
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Animals)) || __instance.GetType().FullName.Equals(ANIMALTAB_MAINTAB) || __instance.GetType().FullName.Equals(NUMBERS_DEFNAME))
            {
                AnimalManager.LoadState(AnimalManager.links, AnimalManager.Animals().ToList(), AnimalManager.GetActivePolicy());
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Mechs)))
            {
                MechManager.LoadState(MechManager.links, MechManager.Mechs().ToList(), MechManager.GetActivePolicy());
            }
        }
    }
}