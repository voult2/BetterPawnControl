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



        private const BindingFlags BINDINGDLAGS_ALL = (BindingFlags)60; // public + private + static + instance;

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
                                        .assemblies.loadedAssemblies.First();

                    if (PF_assembly == null)
                    {
                        throw new Exception("[BPC] Animal Tab assembly not found.");
                    }
                }
                catch
                {
                    _anyError = true;
                    Log.Error("[BPC] Error in Animal Tab integration - functionality disabled");
                    throw;
                }
            }
        }
    }
}
