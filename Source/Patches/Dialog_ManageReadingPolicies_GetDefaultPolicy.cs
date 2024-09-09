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
    [HarmonyPatch(typeof(Dialog_ManageReadingPolicies), "GetDefaultPolicy")]
    static class Dialog_ManageReadingPolicies_GetDefaultPolicy
    {
        static void Postfix(ref ReadingPolicy __result)
        {
            __result = AssignManager.DefaultReadingPolicy;
        }

    }
}
