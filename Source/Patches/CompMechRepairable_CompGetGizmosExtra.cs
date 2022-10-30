using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;

namespace BetterPawnControl
{

    [HarmonyPatch(typeof(CompMechRepairable), "CompGetGizmosExtra")]
    internal static class CompMechRepairable_CompGetGizmosExtra
    {
        private static void Postfix()
        {
            //I need a way to detect that the AutoRepair Toogle Gizmo has been changed. Couldn't find an elegant solution.
            //Currently, if a mech is selected it will dirty the policy to force a reload when the MechTab is open.
            MechManager.DirtyPolicy = true;
        }
    }
}
