using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;

namespace BetterPawnControl
{

    [HarmonyPatch(typeof(FloatMenu), "PostClose")]
    internal static class FloatMenu_PostClose
    {
        private static void Postfix()
        {
            if (Current.ProgramState == ProgramState.Playing)
            {
                AnimalManager.UpdateState(AnimalManager.links, AnimalManager.Animals().ToList(), AnimalManager.GetActivePolicy());
                ScheduleManager.UpdateState(ScheduleManager.links, ScheduleManager.Colonists().ToList(), ScheduleManager.GetActivePolicy());
                AssignManager.UpdateState(AssignManager.links, AssignManager.Colonists().ToList(), AssignManager.GetActivePolicy());
            }           
        }
    }
}