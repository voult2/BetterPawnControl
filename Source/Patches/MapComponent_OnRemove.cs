using HarmonyLib;
using Verse;
using RimWorld;

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
