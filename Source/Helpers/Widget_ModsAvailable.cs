using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    public static class Widget_ModsAvailable
    {
        private const string WORKTAB = "Work Tab";
        private const string ANIMALTAB= "Animal Tab";
        private const string CSL = "Children, school and learning";
        private const string AAF = "Assign Animal Food";

        static Widget_ModsAvailable() 
        {
            var harmony = new Harmony("VouLT.BetterPawnControl");
            harmony.PatchAll();
        }

        public static bool AnimalTabAvailable
        {
            get
            {
                return LoadedModManager.RunningMods.Any(mod => mod.Name == ANIMALTAB);
            }
        }

        public static bool WorkTabAvailable
        {
            get
            {
                return  LoadedModManager.RunningMods.Any(mod => mod.Name == WORKTAB);
            }
        }

        public static bool DisableBPCOnWorkTab
        {
            get
            {
                return WorkTabAvailable && LoadedModManager.GetMod<BetterPawnControl>().GetSettings<Settings>().disableBPCOnWorkTab;
            }
        }


        public static bool CSLAvailable
        {
            get
            {
                return LoadedModManager.RunningMods.Any(mod => mod.Name == CSL);
            }
        }

        public static bool AAFAvailable
        {
            get
            {
                return LoadedModManager.RunningMods.Any(mod => mod.Name == AAF);
            }
        }
    }
}
