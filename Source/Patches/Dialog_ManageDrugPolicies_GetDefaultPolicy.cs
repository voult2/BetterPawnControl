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
    [HarmonyPatch(typeof(Dialog_ManageDrugPolicies), "GetDefaultPolicy")]
    static class Dialog_ManageDrugPolicies_GetDefaultPolicy
    {
        static void Postfix(ref DrugPolicy __result)
        {
            __result = AssignManager.DefaultDrugPolicy;
        }

    }
}
