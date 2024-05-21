using System;
using System.Linq;
using System.Text;
using HarmonyLib;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    public static class Widget_MiscRobots
    {
        private static bool init = false;
        private static Type aiRobotType;

        static Widget_MiscRobots()
        {
            var isModActive = LoadedModManager.RunningMods.Any(mod => mod.Name == "Misc. Robots");
            if (!isModActive)
                return;

            aiRobotType = AccessTools.TypeByName($"AIRobot.X2_AIRobot");

            init = aiRobotType != null;

            if (init)
            {
                Log.Message("[BPC] Misc. Robots functionality integrated");
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("[BPC] Error in Misc. Robots integration - functionality disabled:");

                if (aiRobotType == null)
                    sb.AppendLine(" - Type AIRobot.X2_AIRobot is not found.");

                Log.Error(sb.ToString());
            }
        }

        public static bool IsPawnRobot(Pawn pawn)
        {
            if (pawn == null || !init)
                return false;
            var result = pawn.GetType() == aiRobotType;
            return result;
        }
    }
}
