using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;
using System.Linq;
using System.Collections.Generic;
using Verse.Sound;

namespace BetterPawnControl.Patches
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.SetDead))]
    static class Pawn_HealthTracker_OnDead
    {
        static void Postfix(Pawn ___pawn)
        {
            AssignManager.CleanDeadColonists(___pawn);
            ScheduleManager.CleanDeadColonists(___pawn);
            WorkManager.CleanDeadColonists(___pawn);
            AnimalManager.CleanDeadAnimals(___pawn);
        }
    }
}
