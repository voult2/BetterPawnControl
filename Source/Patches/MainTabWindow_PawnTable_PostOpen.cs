using System.Linq;
using HarmonyLib;
using RimWorld;

namespace BetterPawnControl
{
    [HarmonyPatch(typeof(MainTabWindow_PawnTable), nameof(MainTabWindow_PawnTable.PostOpen))]
    static class MainTabWindow_PawnTable_OnPostOpen
    {
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

            if (__instance.GetType().Equals(typeof(MainTabWindow_Work)) || __instance.GetType().FullName.Equals(Widget_ModsAvailable.WORKTAB_MAINTAB) || __instance.GetType().FullName.Equals(Widget_ModsAvailable.NUMBERS_MAINTAB))
            {
                if (!Widget_ModsAvailable.DisableBPCOnWorkTab)
                {
                    WorkManager.LoadState(WorkManager.links, WorkManager.Colonists().ToList(), WorkManager.GetActivePolicy());
                    WorkManager.showPaste = false;
                }
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Animals)) || __instance.GetType().FullName.Equals(Widget_ModsAvailable.ANIMALTAB_MAINTAB) || __instance.GetType().FullName.Equals(Widget_ModsAvailable.NUMBERS_DEFNAME))
            {
                AnimalManager.LoadState(AnimalManager.links, AnimalManager.Animals().ToList(), AnimalManager.GetActivePolicy());
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Mechs)))
            {
                MechManager.LoadState(MechManager.links, MechManager.Mechs().ToList(), MechManager.GetActivePolicy());
            }
            

            if (__instance.GetType().FullName.Equals(Widget_ModsAvailable.WEAPONSTAB_MAINTAB) && Widget_ModsAvailable.WTBAvailable)
            {
                WeaponsManager.LoadState(WeaponsManager.links, WeaponsManager.Colonists().ToList(), WeaponsManager.GetActivePolicy());
            }
        }
    }
}