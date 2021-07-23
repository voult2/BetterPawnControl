using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;
using System.Linq;
using System.Collections.Generic;
using Verse.Sound;

namespace BetterPawnControl.Patches
{
    [HarmonyPatch(typeof(Window), nameof(Window.PreClose))]
    static class Window_PreClose
    {
        private const string WORKTAB_MAINTAB = "WorkTab.MainTabWindow_WorkTab";
        private const string ANIMALTAB_MAINTAB = "AnimalTab.MainTabWindow_Animals";
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

            if (__instance.GetType().Equals(typeof(MainTabWindow_Animals)) || __instance.GetType().FullName.Equals(ANIMALTAB_MAINTAB))
            {
                AnimalManager.SaveCurrentState(AnimalManager.Animals().ToList());
            }
        }
    }
}
