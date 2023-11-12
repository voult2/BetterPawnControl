using HarmonyLib;
using Verse;

namespace BetterPawnControl.Patches
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.SetDead))]
    static class Pawn_HealthTracker_OnDead
    {
        static void Postfix(Pawn ___pawn)
        {
            if (___pawn.IsFreeColonist) 
            { 
                AssignManager.CleanDeadColonists(___pawn);
                ScheduleManager.CleanDeadColonists(___pawn);
                WorkManager.CleanDeadColonists(___pawn);
                AnimalManager.CleanDeadAnimals(___pawn);
                MechManager.CleanDeadMechs(___pawn);
                if (Widget_ModsAvailable.WTBAvailable)
                {
                    WeaponsManager.CleanDeadColonists(___pawn);
                }
            }
        }
    }
}
