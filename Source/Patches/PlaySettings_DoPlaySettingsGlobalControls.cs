using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BetterPawnControl.Patches
{
    [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
    internal static class PlaySettings_DoPlaySettingsGlobalControls
    {
        static WidgetRow emergencyButtonIcon;

        internal static void Postfix(WidgetRow row, bool worldView)
        {
            if (worldView || (row == null)) { return; }

            emergencyButtonIcon = row;

            if (AlertManager.OnAlert)
            {
                if (row.ButtonIcon(Resources.Textures.EmergencyOn, "BPC.ToggleEmergencyOn".Translate(), UnityEngine.Color.red))
                {
                    EmergencyToogleOFF(null);
                }
            }

            if (!AlertManager.OnAlert)
            {
                if (row.ButtonIcon(Resources.Textures.EmergencyOff, "BPC.ToggleEmergencyOff".Translate(), UnityEngine.Color.gray)) 
                {
                    EmergencyToogleON(null);
                }
            }
        }

        internal static void EmergencyToogleButton()
        {
            EmergencyToogleButton(emergencyButtonIcon);
        }

        private static void EmergencyToogleButton(WidgetRow icon)
        {
            
            if (AlertManager.OnAlert)
            {
                EmergencyToogleOFF(icon);
            }
            else if (!AlertManager.OnAlert)
            {
                EmergencyToogleON(icon);
            }
        }

        internal static void EmergencyToogleON(WidgetRow icon)
        {
            AlertManager.OnAlert = true;
            SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
            AlertManager.SaveState(0);
            AlertManager.LoadState(1); //ON

            if (LoadedModManager.GetMod<BetterPawnControl>().GetSettings<Settings>().automaticPawnsInterrupt)
            {
                AlertManager.PawnsInterruptForced();
            }
            
            if (icon != null)
            {
                icon.ButtonIcon(Resources.Textures.EmergencyOn, "BPC.ToggleEmergencyOn".Translate(), UnityEngine.Color.red);
            }
        }

        internal static void EmergencyToogleOFF(WidgetRow icon)
        {
            AlertManager.OnAlert = false;
            SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
            AlertManager.LoadState(0); //OFF
            
            if (icon != null)
            {
                icon.ButtonIcon(Resources.Textures.EmergencyOff, "BPC.ToggleEmergencyOff".Translate(), UnityEngine.Color.gray);
            }
        }
    }
}