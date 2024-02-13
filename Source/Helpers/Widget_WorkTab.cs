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

        private static Type priorityManager;
        private static MethodInfo getPriorityTrackerByPawn;

        private static GameComponent gcPriorityManager;

        private static Type priorityTracker;
        private static MethodInfo invalidateCache;
        private static MethodInfo onChange;

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

            priorityManager = AccessTools.TypeByName("WorkTab.PriorityManager");
            if (priorityManager != null)
            {
                getPriorityTrackerByPawn = AccessTools.PropertyGetter(priorityManager, "Item");
            }

            priorityTracker = AccessTools.TypeByName("WorkTab.PriorityTracker");
            if (priorityTracker != null)
            {
                invalidateCache = AccessTools.Method(priorityTracker, "InvalidateCache", new Type[] { typeof(WorkGiverDef), typeof(bool) });
                onChange = AccessTools.Method(priorityTracker, "OnChange", null);
            }

            init = pawnExtensions != null
                && getPriorities != null
                && setPriority != null
                && priorityManager != null
                && getPriorityTrackerByPawn != null
                && priorityTracker != null
                && invalidateCache != null
                && onChange != null;

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

                if (priorityManager == null)
                    sb.AppendLine(" - Type WorkTab.PriorityManager is not found.");
                if (getPriorityTrackerByPawn == null)
                    sb.AppendLine(" - Method WorkTab.PriorityManager:get_Item(Pawn) is not found.");

                if (priorityTracker == null)
                    sb.AppendLine(" - Type WorkTab.PriorityTracker is not found.");
                if (invalidateCache == null)
                    sb.AppendLine(" - Method WorkTab.PriorityTracker:InvalidateCache(WorkGiverDef, bool) is not found.");
                if (onChange == null)
                    sb.AppendLine(" - Method WorkTab.PriorityTracker:OnChange() is not found.");

                Log.Error(sb.ToString());
            }
        }

        public static void ClearCache()
        {
            gcPriorityManager = null;
        }

        public static List<int> GetWorkTabPriorities(Pawn pawn, WorkGiverDef workGiver)
        {
            if (pawn == null)
                throw new ArgumentNullException(nameof(pawn));
            if (workGiver == null)
                throw new ArgumentNullException(nameof(workGiver));

            if (!init)
                return null;

            if (gcPriorityManager == null)
            {
                gcPriorityManager = Current.Game?.GetComponent(priorityManager);
            }

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
                setPriority.Invoke(null, new object[] { pawn, workGiver, hours[hour], hour, false });
            }

            if (gcPriorityManager == null)
            {
                gcPriorityManager = Current.Game?.GetComponent(priorityManager);
            }

            if (gcPriorityManager != null)
            {
                var pt = getPriorityTrackerByPawn.Invoke(gcPriorityManager, new object[] { pawn });
                if (pt != null)
                {
                    invalidateCache.Invoke(pt, new object[] { workGiver, true });
                    onChange.Invoke(pt, null);
                }
            }
        }
    }
}
