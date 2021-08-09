using HarmonyLib;
using Verse;
using RimWorld;

namespace BetterPawnControl.Patches
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.ExitMap))]
    static class Pawn_OnExitMap
    {
        static void Postfix(Pawn __instance)
        {
            if (__instance != null)
            {
                if (AssignManager.slaves.Exists(x => x == __instance.GetUniqueLoadID()))
                {
                    AssignManager.slaves.Remove(__instance.GetUniqueLoadID());
                }

                if (AssignManager.prisoners.Exists(x => x == __instance.GetUniqueLoadID()))
                {
                    AssignManager.prisoners.Remove(__instance.GetUniqueLoadID());
                }
            }
        }
    }
}
