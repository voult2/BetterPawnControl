using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    public static class Widget_AnimalTab
    {
        private const string PF_MOD_NAME = "Animal Tab";
        private const string PF_TYPE_UTILS = "AnimalTab.MainTabWindow_Animals";

        private const string PF_METHOD_DOWINDOW = "DoWindowContents";
        private const string PF_METHOD_DOFILTERBAR = "DoFilterBar";

        private const BindingFlags BINDINGDLAGS_ALL = (BindingFlags)60; // public + private + static + instance;

        private static MethodInfo _hasDoWindowContents;
        private static MethodInfo _hasDoFilterBar;

        public static object animalClassInstance;

        private static bool _anyError = false;
        private static bool _initialized = false;
        private static bool _available = false;

        public static bool AnimalTabAvailable
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
                                        .assemblies.loadedAssemblies.Last();

                    if (PF_assembly == null)
                    {
                        throw new Exception("[BPC] Animal Tab assembly not found.");
                    }

                    // get the Utils type
                    var PF_UtilsType = PF_assembly.GetType(PF_TYPE_UTILS);
                    if (PF_UtilsType == null)
                        throw new Exception("[BPC] Animal Tab type not found.");

                    animalClassInstance = Activator.CreateInstance(PF_UtilsType);
                    if (animalClassInstance == null)
                    {
                        throw new Exception("[BPC] AnimalTab instance not found");
                    }

                    // get the various fields and methods we need.
                    _hasDoWindowContents = PF_UtilsType.GetMethod(PF_METHOD_DOWINDOW, BINDINGDLAGS_ALL);
                    if (_hasDoWindowContents == null)
                    {
                        throw new Exception("[BPC] AnimalTab method not found: " + PF_METHOD_DOWINDOW);
                    }

                    _hasDoFilterBar = PF_UtilsType.GetMethod(PF_METHOD_DOFILTERBAR, BINDINGDLAGS_ALL);
                    if (_hasDoFilterBar == null)
                    {
                        throw new Exception("[BPC] AnimalTab method not found: " + PF_METHOD_DOFILTERBAR);

                    }

                    Log.Message("[BPC] Animal Tab functionality integrated");
                }
                catch
                {
                    _anyError = true;
                    Log.Error("[BPC] Error in Animal Tab integration - functionality disabled");
                    throw;
                }
            }
        }

        public static void DoWindowContents(Rect fillRect)
        {
            _hasDoWindowContents.Invoke(animalClassInstance, new object[] { fillRect });
        }

        public static void DoFilderBar(Rect fillRect)
        {
            _hasDoFilterBar.Invoke(animalClassInstance, new object[] { fillRect });
        }
    }
}
