using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    public static class Widget_CSL
    {
        private const string PF_MOD_NAME = "Children, school and learning";

        private const BindingFlags BINDINGDLAGS_ALL = (BindingFlags)60; // public + private + static + instance;

        public static object animalClassInstance;

        private static bool _anyError = false;
        private static bool _initialized = false;
        private static bool _available = false;

        public static bool CLSAvailable
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
                        throw new Exception("[BPC] CLS assembly not found.");
                    }

   

                    Log.Message("[BPC] CLS mod found and integrated");
                }
                catch
                {
                    _anyError = true;
                    Log.Error("[BPC] Error in CLS integration - functionality disabled");
                    throw;
                }
            }
        }
    }
}
