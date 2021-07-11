using HarmonyLib;
using System.Linq;
using UnityEngine;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    public static class Widget_Harmony_ModsAvailable
    {        
        static Widget_Harmony_ModsAvailable() 
        {
            var harmony = new Harmony("VouLT.BetterPawnControl");
            harmony.PatchAll();                       
        }

        public static bool AnimalTabAvailable
        {
            get
            {                
                return LoadedModManager.RunningMods.Any(mod => mod.Name == "Animal Tab");
            }
        }

        public static bool WorkTabAvailable
        {
            get
            {
                return LoadedModManager.RunningMods.Any(mod => mod.Name == "Work Tab");
            }
        }

        public static bool CSLAvailable
        {
            get
            {
                return LoadedModManager.RunningMods.Any(mod => mod.Name == "Children, school and learning");
            }
        }

    }
}
