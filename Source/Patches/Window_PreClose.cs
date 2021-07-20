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
        static void Postfix(Window __instance)
        {
            if (__instance.GetType().Equals(typeof(MainTabWindow_Assign))) 
            {
                AssignManager.SaveCurrentState(AssignManager.Colonists().ToList());
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Work)))
            {
                WorkManager.SaveCurrentState(WorkManager.Colonists().ToList());
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Schedule)))
            {
                ScheduleManager.SaveCurrentState(ScheduleManager.Colonists().ToList());
            }

            if (__instance.GetType().Equals(typeof(MainTabWindow_Animals)))
            {
                AnimalManager.SaveCurrentState(AnimalManager.Animals().ToList());
            }
        }
    }
}
