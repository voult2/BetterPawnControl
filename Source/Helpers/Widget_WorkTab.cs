using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    public static class Widget_WorkTab
    {
        private static bool init = false;

        private static Type pawnExtensions;
        private static MethodInfo getPriorities;
        private static MethodInfo setPriority;

        static Widget_WorkTab()
        {
            var isModActive = LoadedModManager.RunningMods.Any(mod => mod.Name == "Work Tab");
            if (!isModActive)
                return;

            pawnExtensions = AccessTools.TypeByName("WorkTab.Pawn_Extensions");
            if (pawnExtensions != null)
            {
                getPriorities = AccessTools.Method(pawnExtensions, "GetPriorities", new Type[] { typeof(Pawn), typeof(WorkGiverDef) });
                setPriority = AccessTools.Method(pawnExtensions, "SetPriority", new Type[] { typeof(Pawn), typeof(WorkGiverDef), typeof(int), typeof(int), typeof(bool) });
            }

            init = pawnExtensions != null
                && getPriorities != null
                && setPriority != null;

            if (init)
                Log.Message("[BPC] Work Tab functionality integrated");
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("[BPC] Error in Work Tab integration - functionality disabled:");

                if (pawnExtensions == null)
                    sb.AppendLine(" - Type WorkTab.Pawn_Extensions is not found.");
                if (getPriorities == null)
                    sb.AppendLine(" - Method WorkTab.Pawn_Extensions:GetPriorities(Pawn, WorkGiverDef) is not found.");
                if (setPriority == null)
                    sb.AppendLine(" - Method WorkTab.Pawn_Extensions:SetPriority(Pawn, WorkGiverDef, int, int, bool) is not found.");
                
                Log.Error(sb.ToString());
            }
        }

        public static List<int> GetWorkTabPriorities(Pawn pawn, WorkGiverDef workGiver)
        {
            if (pawn == null)
                throw new ArgumentNullException(nameof(pawn));
            if (workGiver == null)
                throw new ArgumentNullException(nameof(workGiver));

            if (!init)
                return null;

            var array = (int[]) getPriorities.Invoke(null, new object[] { pawn, workGiver });
            var result = new List<int>(array);

            return result;
        }

        public static void SetWorkTabPriorities(Pawn pawn, WorkGiverDef workGiver, List<int> hours)
        {
            if (pawn == null)
                throw new ArgumentNullException(nameof(pawn));
            if (workGiver == null)
                throw new ArgumentNullException(nameof(workGiver));
            if (hours == null)
                throw new ArgumentNullException(nameof(hours));

            if (!init)
                return;

            for (int hour = 0; hour < hours.Count; hour++)
            {
                setPriority.Invoke(null, new object[] { pawn, workGiver, hours[hour], hour, true });
            }
        }
    }
}
