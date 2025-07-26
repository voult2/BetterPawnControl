using HarmonyLib;
using RimWorld;

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
