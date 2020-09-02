using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Sound;

namespace BetterPawnControl.Patch
{
    [HarmonyPatch(typeof(PlaySettings), "DoPlaySettingsGlobalControls")]
    internal static class RimWorld_PlaySettings_DoPlaySettingsGlobalControls
    {
        private static void Postfix(WidgetRow row, bool worldView)
        {
            if (worldView || (row == null)) { return; }

            if (AlertManager.OnAlert)
            {
                if (row.ButtonIcon(Resources.EmergencyOn, "BPC.ToggleEmergencyOn".Translate(), UnityEngine.Color.red, true))
                {
                    AlertManager.OnAlert = false;
                    SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                    AlertManager.LoadState(0); //OFF
                }                
            }

            if (!AlertManager.OnAlert)
            {
                if (row.ButtonIcon(Resources.EmergencyOff, "BPC.ToggleEmergencyOff".Translate(), UnityEngine.Color.gray, true))
                {
                    AlertManager.OnAlert = true;
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                    AlertManager.SaveState(0);
                    AlertManager.LoadState(1); //ON
                    if (AlertManager.AutomaticPawnsInterrupt) 
                    {
                        AlertManager.PawnsInterruptForced();
                    }                    
                }
            }
        }
    }
}