using HarmonyLib;
using Verse;

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
            MechManager.CleanRemovedMaps();
            if (Widget_ModsAvailable.WTBAvailable)
            {
                WeaponsManager.CleanRemovedMaps();
            }
        }
    }
}
