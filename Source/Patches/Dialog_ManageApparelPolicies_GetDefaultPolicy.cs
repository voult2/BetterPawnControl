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
    [HarmonyPatch(typeof(Dialog_ManageApparelPolicies), "GetDefaultPolicy")]
    static class Dialog_ManageApparelPolicies_GetDefaultPolicy
    {
        static void Postfix(ref ApparelPolicy __result)
        {
            __result = AssignManager.DefaultOutfit;
        }
        
    }
}
