using System;
using System.Linq;
using System.Reflection;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    public static class Widget_CombatExtended
    {
        private const string PF_MOD_NAME = "Combat Extended";
        private const string PF_TYPE_UTILS = "CombatExtended.Utility_Loadouts";

        private const string PF_METHOD_GETLOADOUT = "GetLoadoutId";
        private const string PF_METHOD_SETLOADOUT = "SetLoadoutById";

        // public + private + static + instance;
        private const BindingFlags BINDINGDLAGS_ALL = (BindingFlags)60; 

        private static bool _anyError = false;
        private static bool _initialized = false;
        private static bool _available = false;

        private static MethodInfo _hasGetLoadoutId;
        private static MethodInfo _hasSetLoadoutById;

        public static object combatExtendedClassInstance;

        public static bool CombatExtendedAvailable
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
                    //get the assembly
                    var PF_assembly = LoadedModManager
                                        .RunningMods.First(mod => mod.Name == PF_MOD_NAME)
                                        .assemblies.loadedAssemblies.Where(asm => asm.GetName().Name == "CombatExtended").FirstOrDefault(); //thanks to Deno226

                    if (PF_assembly == null)
                    {
                        throw new Exception(
                            "[BPC] Combat Extended assembly not found.");
                    }

                    var PF_UtilsType = PF_assembly.GetType(PF_TYPE_UTILS);
                    if (PF_UtilsType == null)
                    {
                        throw new Exception("[BPC] Combat Extended type not found: " + PF_TYPE_UTILS);
                    }

                    _hasGetLoadoutId = PF_UtilsType.GetMethod(PF_METHOD_GETLOADOUT, BINDINGDLAGS_ALL);
                    if (_hasGetLoadoutId == null)
                    {
                        throw new Exception(
                            "[BPC] Combat Extended method not found: " + PF_METHOD_GETLOADOUT);
                    }

                    _hasSetLoadoutById = PF_UtilsType.GetMethod(PF_METHOD_SETLOADOUT, BINDINGDLAGS_ALL);
                    if (_hasSetLoadoutById == null)
                    {
                        throw new Exception("[BPC] Combat Extended method not found: " + PF_METHOD_SETLOADOUT);
                    }

                    Log.Message("[BPC] Combat Extended functionality integrated");
                }
                catch
                {
                    _anyError = true;
                    Log.Error("[BPC] Error in Combat Extended integration - functionality disabled");
                    throw;
                }
            }
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