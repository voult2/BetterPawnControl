using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterPawnControl.Patches
{
    [HarmonyPatch(typeof(Window), nameof(Window.PreClose))]
    static class Window_PreClose
    {        
        static void Postfix(Window __instance)
        {
            if (__instance.GetType().Equals(typeof(MainTabWindow_Assign)) || __instance.GetType().FullName.Equals(Widget_ModsAvailable.WEAPONSTAB_MAINTAB)) 
            {
                AssignManager.SaveCurrentState(AssignManager.Colonists().ToList());
                AssignManager.LinksCleanUp();
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Work)) || (__instance.GetType().FullName.Equals(Widget_ModsAvailable.WORKTAB_MAINTAB) && !Widget_ModsAvailable.DisableBPCOnWorkTab) || __instance.GetType().FullName.Equals(Widget_ModsAvailable.NUMBERS_MAINTAB))
            {
                WorkManager.SaveCurrentState(WorkManager.Colonists().ToList());
                WorkManager.LinksCleanUp();
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Schedule)))
            {
                ScheduleManager.SaveCurrentState(ScheduleManager.Colonists().ToList());
                ScheduleManager.LinksCleanUp();
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Animals)) || __instance.GetType().FullName.Equals(Widget_ModsAvailable.ANIMALTAB_MAINTAB) || __instance.GetType().FullName.Equals(Widget_ModsAvailable.NUMBERS_DEFNAME))
            {
                AnimalManager.SaveCurrentState(AnimalManager.Animals().ToList());
                AnimalManager.LinksCleanUp();
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Mechs)))
            {
                MechManager.SaveCurrentState(MechManager.Mechs().ToList());
                MechManager.LinksCleanUp();
            }

            if (__instance.GetType().FullName.Equals(Widget_ModsAvailable.WEAPONSTAB_MAINTAB) && Widget_ModsAvailable.WTBAvailable)
            {
                WeaponsManager.SaveCurrentState(WeaponsManager.Colonists().ToList());
                WeaponsManager.LinksCleanUp();
            }
        }
    }
}
