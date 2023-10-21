using System.Linq;
using HarmonyLib;
using Verse;

namespace BetterPawnControl
{

    [HarmonyPatch(typeof(FloatMenu), "PostClose")]
    internal static class FloatMenu_PostClose
    {
        private static void Postfix()
        {
            if (Current.ProgramState == ProgramState.Playing && Find.CurrentMap != null && Find.CurrentMap.IsPlayerHome)
            {
                //Everytime a float opens in player home, BPC is saved regardless if any related pawns settings tracked by BPC are changed, and there is at least one settlement
                //A better solution could be to Harmonize the exact function where the setting is changed but it is hard to pin-point exactly where it happens. 
                AnimalManager.UpdateState(AnimalManager.links, AnimalManager.Animals().ToList(), AnimalManager.GetActivePolicy());
                ScheduleManager.UpdateState(ScheduleManager.links, ScheduleManager.Colonists().ToList(), ScheduleManager.GetActivePolicy());
                AssignManager.UpdateState(AssignManager.links, AssignManager.Colonists().ToList(), AssignManager.GetActivePolicy());
                MechManager.UpdateState(MechManager.links, MechManager.Mechs().ToList(), MechManager.GetActivePolicy());
            }           
        }
    }
}