using Verse;

namespace BetterPawnControl
{
    public class BetterPawnControlSettings : ModSettings
    {
        public bool automaticPawnsInterrupt = true;
        public bool disableBPCOnWorkTab = false;
        public bool disableBPCWorkTabInnerPriorities = false;
        public bool saveInventoryStock = false;
        public float? settingsWindowPosX = null;
        public float? settingsWindowPosY = null;
        public float? settingsWindowWidth = null;
        public float? settingsWindowHeight = null;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref automaticPawnsInterrupt, "AutomaticPawnsInterrupt", true, true);
            Scribe_Values.Look(ref disableBPCOnWorkTab, "DisableBPCOnWork", false, true);
            Scribe_Values.Look(ref disableBPCWorkTabInnerPriorities, "DisableBPCWorkTabInnerPriorities", false, true);
            Scribe_Values.Look(ref saveInventoryStock, "SaveInventoryStock", false, true);
            Scribe_Values.Look(ref settingsWindowPosX, "SettingsWindowPosX", null, true);
            Scribe_Values.Look(ref settingsWindowPosY, "SettingsWindowPosY", null, true);
            Scribe_Values.Look(ref settingsWindowWidth, "SettingsWindowWidth", null, true);
            Scribe_Values.Look(ref settingsWindowHeight, "SettingsWindowHeight", null, true);
        }
    }
}
