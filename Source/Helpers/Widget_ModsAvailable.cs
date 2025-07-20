﻿using System.Linq;
using HarmonyLib;
using Verse;
using static BetterPawnControl.BetterPawnControlMod;

namespace BetterPawnControl
{
    [StaticConstructorOnStartup]
    public static class Widget_ModsAvailable
    {
        private const string WORKTAB = "Work Tab";
        private const string ANIMALTAB= "Animal Tab";
        private const string CSL = "Children, school and learning";
        private const string AAF = "Assign Animal Food";
        private const string WTB = "[1001]Weapons Tab Reborn";
        private const string MISCROBOTS = "Misc. Robots";

        internal const string WORKTAB_MAINTAB = "WorkTab.MainTabWindow_WorkTab";
        internal const string ANIMALTAB_MAINTAB = "AnimalTab.MainTabWindow_Animals";
        internal const string NUMBERS_MAINTAB = "Numbers.MainTabWindow_Numbers";
        internal const string NUMBERS_DEFNAME = "Numbers.MainTabWindow_NumbersAnimals";       
        internal const string WEAPONSTAB_MAINTAB = "WeaponsTabReborn.MainTabWindow_Weapons";
        internal const string AIROBOTX2_MAINTAB = "AIRobot.X2_MainTabWindow_Robots";

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
                return WorkTabAvailable && Settings.disableBPCOnWorkTab;
            }
        }

        public static bool DisableBPCWorkTabInnerPriorities
        {
            get
            {
                return WorkTabAvailable && Settings.disableBPCWorkTabInnerPriorities;
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

        public static bool WTBAvailable
        {
            get
            {
                return LoadedModManager.RunningMods.Any(mod => mod.Name == WTB) && Widget_WeaponsTabReborn.Integrated(); 
            }
        }

        public static bool CEAvailable
        {
            get
            {
                return Widget_CombatExtended.CombatExtendedAvailable;
            }
        }

        public static bool MiscRobotsAvailable
        {
            get
            {
                return LoadedModManager.RunningMods.Any(mod => mod.Name == MISCROBOTS);
            }
        }

        public static bool CompositableAvailable
        {
            get
            {
                return Widget_CompositableLoadouts.CompositableLoadoutsAvailable;;
            }
        }
    }
}
