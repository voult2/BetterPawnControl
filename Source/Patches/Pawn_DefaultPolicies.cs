using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
    [HarmonyPatch(typeof(Pawn_GuestTracker), nameof(Pawn_GuestTracker.SetGuestStatus))]
    static class Pawn_GuestTracker_SetGuestStatus
    {
        static void Postfix(Pawn ___pawn)
        {
            if (___pawn != null)
            {
                if (___pawn.IsFreeColonist && !AssignManager.links.Exists(x => ___pawn.Equals(x.colonist)))
                {
                    //became a new free colonist 
                    AssignManager.SetDefaultsForFreeColonist(___pawn);
                }

                if (___pawn.IsPrisoner)
                {
                    //former free colonist become prisoner
                    AssignManager.SetDefaultsForPrisoner(___pawn);
                }


                if (___pawn.IsSlave)
                {
                    //former free colonist become a slave
                    AssignManager.SetDefaultsForSlave(___pawn);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Faction), nameof(Faction.Notify_PawnJoined))]
    static class Faction_Notify_PawnJoined
    {
        static void Postfix(Pawn p)
        {
            if (p != null)
            {
                if (p.IsFreeColonist)
                {
                    AssignManager.SetDefaultsForFreeColonist(p);
                }

                if (p.IsPrisoner)
                {
                    AssignManager.SetDefaultsForPrisoner(p);
                }

                if (p.IsSlave) 
                {
                    AssignManager.SetDefaultsForSlave(p);
                }
            }
        }
    }
}