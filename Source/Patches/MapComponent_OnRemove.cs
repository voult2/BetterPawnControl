using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;
using System.Linq;
using System.Collections.Generic;
using Verse.Sound;

namespace BetterPawnControl.Patches
{
    [HarmonyPatch(typeof(MapComponent), nameof(MapComponent.MapRemoved))]
    static class MapComponent_OnRemove
    {
        static void Postfix()
        {
            AssignManager.CleanRemovedMaps();
            ScheduleManager.CleanRemovedMaps();
            WorkManager.CleanRemovedMaps();
            AnimalManager.CleanRemovedMaps();
        }
    }
}
