using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace BetterPawnControl
{
    public class Settings : ModSettings
    {
        public bool automaticPawnsInterrupt = true;
        public bool disableBPCOnWorkTab = false;
        public bool disableBPCWorkTabInnerPriorities = false;
        public float settingsWindowPosX = -160f;
        public float settingsWindowPosY = 84f;
        public float settingsWindowWidth = 1600f;
        public float settingsWindowHeight = 855f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref automaticPawnsInterrupt, "AutomaticPawnsInterrupt", true, true);
            Scribe_Values.Look<bool>(ref disableBPCOnWorkTab, "DisableBPCOnWork", false, true);
            Scribe_Values.Look<bool>(ref disableBPCWorkTabInnerPriorities, "DisableBPCWorkTabInnerPriorities", false, true);
            Scribe_Values.Look<float>(ref settingsWindowPosX, "SettingsWindowPosX", -160f, true);
            Scribe_Values.Look<float>(ref settingsWindowPosY, "SettingsWindowPosY", 84f, true);
            Scribe_Values.Look<float>(ref settingsWindowWidth, "SettingsWindowWidth", 1600f, true);
            Scribe_Values.Look<float>(ref settingsWindowHeight, "SettingsWindowHeight", 855f, true);
        }
    }

    public class BetterPawnControl : Mod
    {
        Settings settings;

        public BetterPawnControl(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<Settings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("BPC.AutomaticPawnsInterruptSetting".Translate(), ref settings.automaticPawnsInterrupt);
            if(Widget_ModsAvailable.WorkTabAvailable)
            {
                listingStandard.CheckboxLabeled("BPC.DisableBPCOnWorkTabSetting".Translate(), ref settings.disableBPCOnWorkTab, "BPC.DisableBPCOnWorkTabTooltip".Translate());
                if (!Widget_ModsAvailable.DisableBPCOnWorkTab)
                {
                    listingStandard.CheckboxLabeled("BPC.DisableBPCWorkTabInnerPrioritiesSetting".Translate(), ref settings.disableBPCWorkTabInnerPriorities, "BPC.DisableBPCWorkTabInnerPrioritiesTooltip".Translate());
                }                    
            }
            listingStandard.End();
        }

        public override string SettingsCategory()
        {
            return "BPC.BetterPawnControl".Translate();
        }
    }
}