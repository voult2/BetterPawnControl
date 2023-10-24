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

        // WorkTab.PriorityManager
        private static Type priorityManagerType;
        // WorkTab.PriorityManager this[ Pawn pawn ] getter
        private static MethodInfo getPawnPriorityTracker;
        // WorkTab.PriorityTracker this[ WorkGiverDef workgiver ] getter
        private static MethodInfo getWorkPriority;
        // WorkTab.WorkPriority Priorities property
        private static PropertyInfo priorities;

        // WorkTab.PriorityManager
        private static GameComponent priorityManager;

        static Widget_WorkTab()
        {
            var isModActive = LoadedModManager.RunningMods.Any(mod => mod.Name == "Work Tab");
            if (!isModActive)
                return;

            priorityManagerType = AccessTools.TypeByName("WorkTab.PriorityManager");
            if (priorityManagerType != null)
                getPawnPriorityTracker = AccessTools.Method(priorityManagerType, "get_Item");

            var priorityTrackerType = AccessTools.TypeByName("WorkTab.PriorityTracker");
            if (priorityTrackerType != null)
                getWorkPriority = AccessTools.Method(priorityTrackerType, "get_Item");

            var workPriorityType = AccessTools.TypeByName("WorkTab.WorkPriority");
            if (workPriorityType != null)
                priorities = AccessTools.Property(workPriorityType, "Priorities");

            init = priorityManagerType != null
                && getPawnPriorityTracker != null
                && getWorkPriority != null
                && priorities != null;

            if (init)
                Log.Message("[BPC] Work Tab functionality integrated");
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("[BPC] Error in Work Tab integration - functionality disabled:");

                if (priorityManagerType == null)
                    sb.AppendLine(" - Type WorkTab.PriorityManager is not found.");
                if (getPawnPriorityTracker == null)
                    sb.AppendLine(" - Method WorkTab.PriorityManager:get_Item is not found.");
                if (priorityTrackerType == null)
                    sb.AppendLine(" - Type WorkTab.PriorityTracker is not found.");
                if (getWorkPriority == null)
                    sb.AppendLine(" - Method WorkTab.PriorityTracker:get_Item is not found.");
                if (workPriorityType == null)
                    sb.AppendLine(" - Type WorkTab.WorkPriority is not found.");
                if (priorities == null)
                    sb.AppendLine(" - Property WorkTab.WorkPriority.Priorities is not found.");

                Log.Error(sb.ToString());
            }
        }

        private static object GetWorkPriority(Pawn pawn, WorkGiverDef workGiver)
        {
            if (Current.Game == null)
                return null;

            if (priorityManager == null)
            {
                var component = Current.Game.GetComponent(priorityManagerType);

                if (component == null)
                    return null;

                priorityManager = component;
            }

            var pawnPriorityTracker = getPawnPriorityTracker.Invoke(priorityManager, new object[] { pawn });
            var result = getWorkPriority.Invoke(pawnPriorityTracker, new object[] { workGiver });

            return result;
        }

        public static List<int> GetWorkTabPriorities(Pawn pawn, WorkGiverDef workGiver)
        {
            if (pawn == null)
                throw new ArgumentNullException(nameof(pawn));
            if (workGiver == null)
                throw new ArgumentNullException(nameof(workGiver));

            if (!init)
                return null;

            var workPriority = GetWorkPriority(pawn, workGiver);
            if (workPriority == null)
                return null;

            var array = (int[]) priorities.GetValue(workPriority);
            var result = new List<int>(array);

            return result;
        }

        public static void SetWorkTabPriorities(Pawn pawn, WorkGiverDef workGiver, List<int> prioritiesToSet)
        {
            if (pawn == null)
                throw new ArgumentNullException(nameof(pawn));
            if (workGiver == null)
                throw new ArgumentNullException(nameof(workGiver));
            if (prioritiesToSet == null)
                throw new ArgumentNullException(nameof(prioritiesToSet));

            if (!init)
                return;

            if (!prioritiesToSet.Any())
                return;

            var workPriority = GetWorkPriority(pawn, workGiver);
            if (workPriority == null)
                return;

            // TODO: prioritiesToSet Boundary check?
            // TODO: Pawn.AllowedToDo(workgiver) check?

            priorities.SetValue(workPriority, prioritiesToSet.ToArray());
            pawn.workSettings.Notify_UseWorkPrioritiesChanged(); // Do we need it?
        }
    }
}
