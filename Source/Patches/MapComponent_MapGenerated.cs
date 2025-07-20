using HarmonyLib;
using Verse;

namespace BetterPawnControl.Patches
{
    [HarmonyPatch(typeof(MapComponentUtility), nameof(MapComponent.MapGenerated))]
    static class MapGenerated
    {
        static void Postfix(Map map)
        {
            AssignManager.ProcessNewMap(map);
            ScheduleManager.ProcessNewMap(map);
            WorkManager.ProcessNewMap(map);
            AnimalManager.ProcessNewMap(map);
            MechManager.ProcessNewMap(map);
            if (Widget_ModsAvailable.WTBAvailable)
            {
                WeaponsManager.ProcessNewMap(map);
            }
            if (Widget_ModsAvailable.MiscRobotsAvailable)
            {
                RobotManager.ProcessNewMap(map);
            }
        }
    }
}
