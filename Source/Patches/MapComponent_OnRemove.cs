using HarmonyLib;
using Verse;

namespace BetterPawnControl.Patches
{
    [HarmonyPatch(typeof(MapComponentUtility), nameof(MapComponentUtility.MapRemoved))]
    static class MapComponent_OnRemove
    {
        static void Postfix(Map map)
        {
            LastMapManager.lastMapId = map.uniqueID;

            AssignManager.CleanRemovedMaps(map);
            ScheduleManager.CleanRemovedMaps(map);
            WorkManager.CleanRemovedMaps(map);
            AnimalManager.CleanRemovedMaps(map);
            MechManager.CleanRemovedMaps(map);
            if (Widget_ModsAvailable.WTBAvailable)
            {
                WeaponsManager.CleanRemovedMaps(map);
            }
            if (Widget_ModsAvailable.MiscRobotsAvailable)
            {
                RobotManager.CleanRemovedMaps(map);
            }
        }
    }
}
