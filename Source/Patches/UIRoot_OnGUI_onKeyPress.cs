using HarmonyLib;
using UnityEngine;
using Verse;

namespace BetterPawnControl.Patches
{
    [HarmonyPatch(typeof(UIRoot), nameof(UIRoot.UIRootOnGUI))]
	internal static class UIRoot_OnGUI_onKeyPress
	{
		static void Postfix()
		{
			if (Current.ProgramState != ProgramState.Playing || Event.current.type != EventType.KeyDown || Event.current.keyCode == KeyCode.None) return;
			if (Resources.Hotkeys.BetterPawnControlEmergency.JustPressed)
			{
				Patches.PlaySettings_DoPlaySettingsGlobalControls.EmergencyToogleButton();
				Event.current.Use();
			}
		}
	}
}