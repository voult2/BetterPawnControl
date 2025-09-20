using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
    [HarmonyPatch(typeof(TrainingCardUtility), nameof(TrainingCardUtility.DrawTrainingCard))]
    public static class TrainingCardUtility_DrawTrainingCard
    {
        public struct State
        {
            public bool followDrafted;
            public bool followFieldwork;
        }

        public static void Prefix(Pawn pawn, out State __state)
        {
            __state.followDrafted = pawn.playerSettings.followDrafted;
            __state.followFieldwork = pawn.playerSettings.followFieldwork;
        }

        public static void Postfix(Pawn pawn, State __state)
        {
            if( __state.followDrafted != pawn.playerSettings.followDrafted
                || __state.followFieldwork != pawn.playerSettings.followFieldwork )
            {
                AnimalManager.UpdateState(AnimalManager.links, AnimalManager.Animals().ToList(), AnimalManager.GetActivePolicy());
            }
        }
    }
}
