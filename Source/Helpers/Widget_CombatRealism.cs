using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    public static class Widget_CombatRealism
    {
        private const string PF_MOD_NAME = "Combat Realism";
        private const string PF_TYPE_MAINTAB = "Combat_Realism.MainTabWindow_OutfitsAndLoadouts";
        private const string PF_TYPE_UTILS = "Combat_Realism.Utility_Loadouts";

        private const string PF_METHOD_DOWINDOW = "DoWindowContents";
        private const string PF_METHOD_PAWNLIST = "BuildPawnList";
        private const string PF_METHOD_GETLOADOUT = "GetLoadoutId";
        private const string PF_METHOD_SETLOADOUT = "SetLoadoutById";

        private const BindingFlags BINDINGDLAGS_ALL = (BindingFlags)60; // public + private + static + instance;

        private static bool _anyError = false;
        private static bool _initialized = false;
        private static bool _available = false;

        private static MethodInfo _hasDoWindowContents;
        private static MethodInfo _hasBuildPawnList;
        private static MethodInfo _hasGetLoadoutId;
        private static MethodInfo _hasSetLoadoutById;

        public static object combatRealismClassInstance;
        public static object combatRealismClassInstance2;

        public static bool CombatRealismAvailable
        {
            get
            {
                if (!_initialized)
                    Initialize();
                return (_available && !_anyError);
            }
        }

        private static void Initialize()
        {
            _available = LoadedModManager.RunningMods.Any(mod => mod.Name == PF_MOD_NAME);
            _initialized = true;
            if (_available)
            {
                try
                {
                    // get the assembly
                    var PF_assembly = LoadedModManager
                                        .RunningMods.First(mod => mod.Name == PF_MOD_NAME)
                                        .assemblies.loadedAssemblies.First();

                    if (PF_assembly == null)
                    {
                        throw new Exception("[BPC] Combat Realism assembly not found.");
                    }

                    // get the Utils type
                    var PF_MainTabType = PF_assembly.GetType(PF_TYPE_MAINTAB);
                    if (PF_MainTabType == null)
                    {
                        throw new Exception("[BPC] Combat Realism type not found: " + PF_TYPE_MAINTAB);
                    }

                    var PF_UtilsType = PF_assembly.GetType(PF_TYPE_UTILS);
                    if (PF_UtilsType == null)
                    {
                        throw new Exception("[BPC] Combat Realism type not found: " + PF_TYPE_UTILS);
                    }

                    combatRealismClassInstance = Activator.CreateInstance(PF_MainTabType);

                    if (combatRealismClassInstance == null)
                    {
                        throw new Exception("[BPC] Combat Realism instance not found");
                    }

                    // get the various fields and methods we need.
                    _hasDoWindowContents = PF_MainTabType.GetMethod(PF_METHOD_DOWINDOW, BINDINGDLAGS_ALL);
                    if (_hasDoWindowContents == null)
                    {
                        throw new Exception("[BPC] Combat Realism method not found: " + PF_METHOD_DOWINDOW);
                    }

                    _hasBuildPawnList = PF_MainTabType.GetMethod(PF_METHOD_PAWNLIST, BINDINGDLAGS_ALL);
                    if (_hasBuildPawnList == null)
                    {
                        throw new Exception("[BPC] Combat Realism method not found: " + PF_METHOD_PAWNLIST);
                    }

                    _hasGetLoadoutId = PF_UtilsType.GetMethod(PF_METHOD_GETLOADOUT, BINDINGDLAGS_ALL);
                    if (_hasGetLoadoutId == null)
                    {
                        throw new Exception("[BPC] Combat Realism method not found: " + PF_METHOD_GETLOADOUT);
                    }

                    _hasSetLoadoutById = PF_UtilsType.GetMethod(PF_METHOD_SETLOADOUT, BINDINGDLAGS_ALL);
                    if (_hasSetLoadoutById == null)
                    {
                        throw new Exception("[BPC] Combat Realism method not found: " + PF_METHOD_SETLOADOUT);
                    }

                    Log.Message("[BPC] Combat Realism functionality integrated");
                }
                catch
                {
                    _anyError = true;
                    Log.Error("[BPC] Error in Combat Realism integration - functionality disabled");
                    throw;
                }
            }
        }

        public static void DoWindowContents(Rect fillRect)
        {
            _hasDoWindowContents.Invoke(combatRealismClassInstance, new object[] { fillRect });
        }

        public static void BuildPawnList()
        {
            _hasBuildPawnList.Invoke(combatRealismClassInstance, null);
        }

        public static int GetLoadoutId(Pawn pawn)
        {
            return (int)_hasGetLoadoutId.Invoke(null, new object[] { pawn });
        }

        public static void SetLoadoutById(Pawn pawn, int id)
        {
            _hasSetLoadoutById.Invoke(null, new object[] { pawn, id });
        }

    }
}
