using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BetterPawnControl.Patches
{
    [HarmonyPatch(typeof(Dialog_ManageFoodPolicies), "GetDefaultPolicy")]
    static class Dialog_ManageFoodPolicies_GetDefaultPolicy
    {
        static void Postfix(ref FoodPolicy __result)
        {
            __result = AssignManager.DefaultFoodPolicy;
        }

    }
}
