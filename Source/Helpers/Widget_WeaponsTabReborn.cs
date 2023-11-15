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
    public static class Widget_WeaoponsTabReborn
    {
        private static bool init = false;

        private static Type loadoutDictionary;
        private static MethodInfo setLoadoutId;
        private static MethodInfo getLoadoutId;
        private static MethodInfo getLoadoutNameById;
        private static MethodInfo getWeaponsLoadoutsDatabase;
        private static MethodInfo getDefaultLoadoutId;

        public static bool Integrated()
        {
            return init;
        }            

        static Widget_WeaoponsTabReborn()
        {
            var isModActive = LoadedModManager.RunningMods.Any(mod => mod.Name == "[1001]Weapons Tab Reborn");
            if (!isModActive)
                return;

            loadoutDictionary = AccessTools.TypeByName("WeaponsTabReborn.LoadoutDictionary");
            if (loadoutDictionary != null)
            {
                setLoadoutId = AccessTools.Method(loadoutDictionary, "BPC_SetLoadoutId", new Type[] { typeof(Pawn), typeof(int) });
                getLoadoutId = AccessTools.Method(loadoutDictionary, "BPC_GetLoadoutId", new Type[] { typeof(Pawn) });
                getLoadoutNameById = AccessTools.Method(loadoutDictionary, "BPC_GetLoadoutNamebyId", new Type[] { typeof(int) });
                getWeaponsLoadoutsDatabase = AccessTools.Method(loadoutDictionary, "BPC_GetWeaponsLoadoutsDatabase", new Type[] { });
                getDefaultLoadoutId = AccessTools.Method(loadoutDictionary, "BPC_GetDefaultLoadoutId", new Type[] { });
            }

            init = loadoutDictionary != null
                && setLoadoutId != null
                && getLoadoutId != null
                && getLoadoutNameById != null
                && getWeaponsLoadoutsDatabase != null
                && getDefaultLoadoutId != null;

            if (init)
                Log.Message("[BPC] Weapons Tab Reborn functionality integrated");
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("[BPC] Error in Weapons Tab Reborn integration - functionality disabled:");

                if (loadoutDictionary == null)
                    sb.AppendLine(" - Type WeaponsTabReborn.LoadoutDictionary is not found.");
                if (setLoadoutId == null)
                    sb.AppendLine(" - Method WeaponsTabReborn.LoadoutDictionary:BPC_SetLoadout(Pawn, int) is not found.");
                if (getLoadoutId == null)
                    sb.AppendLine(" - Method WeaponsTabReborn.LoadoutDictionary:BPC_GetLoadout() is not found.");
                if (getLoadoutNameById == null)
                    sb.AppendLine(" - Method WeaponsTabReborn.LoadoutDictionary:BPC_GetLoadoutNamebyId() is not found.");
                if (getWeaponsLoadoutsDatabase == null)
                    sb.AppendLine(" - Method WeaponsTabReborn.LoadoutDictionary:BPC_GetWeaponsLoadoutsDatabase() is not found.");
                if (getDefaultLoadoutId == null)
                    sb.AppendLine(" - Method WeaponsTabReborn.LoadoutDictionary:BPC_GetDefaultLoadoutId() is not found.");

                Log.Error(sb.ToString());
            }
        }

        public static void SetLoadoutId(Pawn pawn, int loadoutID)
        {
            if (pawn == null)
                throw new ArgumentNullException(nameof(pawn));

            if (!init)
                return;

            if (loadoutID == -1)
                throw new ArgumentException("[BPC] LoadoutID is argument is invalid. It should not be the default value '-1'");

            setLoadoutId.Invoke(null, new object[] { pawn, loadoutID });
        }

        public static int GetLoadoutId(Pawn pawn)
        {            
            if (!init)
                return -1;

            return (int) getLoadoutId.Invoke(null, new object[] { pawn });
        }

        public static string GetLoadoutNameById(int loadoutId)
        {
            if (!init)
                return "error";

            return (string) getLoadoutNameById.Invoke(null, new object[] { loadoutId });
        }

        public static Dictionary<string, int> GetWeaponsLoadoutsDatabase()
        {
            if (!init)
                return null;

            return (Dictionary<string, int>) getWeaponsLoadoutsDatabase.Invoke(null, new object[] { });
        }

        public static int GetDefaultLoadoutId()
        {
            if (!init)
                return -1;

            return (int) getDefaultLoadoutId.Invoke(null, new object[] { });
        }
    }
}
