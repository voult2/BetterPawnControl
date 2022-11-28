using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace BetterPawnControl
{

    [HarmonyPatch(typeof(CaravanArrivalAction_Enter), nameof(CaravanArrivalAction_Enter.Arrived))]
    internal static class CaravanArrivalAction_Enter_Arrived
    {
        private static void Postfix(MapParent ___mapParent)
        {
            if(___mapParent.Map.IsPlayerHome)
            {
                AssignManager.LoadState(AssignManager.GetActivePolicy());
            }            
        }
    }
}   