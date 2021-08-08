using HarmonyLib;
using Verse;
using RimWorld;
using System.Linq;
using System.Collections.Generic;

namespace BetterPawnControl.Patches
{
    [HarmonyPatch(typeof(Window), nameof(Window.PreClose))]
    static class Window_PreClose
    {
        private const string WORKTAB_MAINTAB = "WorkTab.MainTabWindow_WorkTab";
        private const string ANIMALTAB_MAINTAB = "AnimalTab.MainTabWindow_Animals";
        private const string NUMBERS_DEFNAME = "Numbers.MainTabWindow_NumbersAnimals";

        static void Postfix(Window __instance)
        {
            if (__instance.GetType().Equals(typeof(MainTabWindow_Assign))) 
            {
                AssignManager.SaveCurrentState(AssignManager.Colonists().ToList());
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Work)) || __instance.GetType().FullName.Equals(WORKTAB_MAINTAB))
            {
                WorkManager.SaveCurrentState(WorkManager.Colonists().ToList());
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Schedule)))
            {
                ScheduleManager.SaveCurrentState(ScheduleManager.Colonists().ToList());
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Animals)) || __instance.GetType().FullName.Equals(ANIMALTAB_MAINTAB) || __instance.GetType().FullName.Equals(NUMBERS_DEFNAME))
            {
                AnimalManager.SaveCurrentState(AnimalManager.Animals().ToList());
            }
        }
    }
}
