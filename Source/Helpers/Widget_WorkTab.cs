using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BetterPawnControl.Helpers
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
        // WorkTab.WorkPriority this[ int hour] setter
        private static MethodInfo setHourForWorkPriority;

        // WorkTab.PriorityManager
        private static GameComponent priorityManager;

        static Widget_WorkTab()
        {
            var isModActive = LoadedModManager.RunningMods.Any(mod => mod.Name == "Work Tab");
            if (!isModActive)
                return;

            priorityManagerType = AccessTools.TypeByName("WorkTab.PriorityManager");
            getPawnPriorityTracker = AccessTools.Method("WorkTab.PriorityManager:get_Item");

            getWorkPriority = AccessTools.Method("WorkTab.PriorityTracker:get_Item");

            var workPriorityType = AccessTools.TypeByName("WorkTab.WorkPriority");
            if (workPriorityType != null)
                priorities = AccessTools.Property(workPriorityType, "Priorities");
            setHourForWorkPriority = AccessTools.Method("WorkTab.WorkPriority:set_Item");

            init = priorityManagerType != null
                && getPawnPriorityTracker != null
                && getWorkPriority != null
                && priorities != null
                && setHourForWorkPriority != null;

            if (init)
                Log.Message("[BPC] Work Tab functionality integrated");
            else
                Log.Message("[BPC] Error in Work Tab integration - functionality disabled");
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

        public static void SetWorkTabPriorities(Pawn pawn, WorkGiverDef workGiver, List<int> priorities)
        {
            if (pawn == null)
                throw new ArgumentNullException(nameof(pawn));
            if (workGiver == null)
                throw new ArgumentNullException(nameof(workGiver));
            if (priorities == null)
                throw new ArgumentNullException(nameof(priorities));

            if (!init)
                return;

            if (!priorities.Any())
                return;

            var workPriority = GetWorkPriority(pawn, workGiver);
            if (workPriority == null)
                return;
            
            for (var hour = 0; hour < priorities.Count; hour++)
            {
                setHourForWorkPriority.Invoke(workPriority, new object[] { hour, priorities[hour] });
            }
        }
    }
}
