using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace BetterPawnControl
{
    public class Settings : ModSettings
    {
        public bool automaticPawnsInterrupt = true;
        public bool disableBPCOnWorkTab = false;

        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref automaticPawnsInterrupt, "AutomaticPawnsInterrupt", true, true);
            Scribe_Values.Look<bool>(ref disableBPCOnWorkTab, "DisableBPCOnWork", false, true);
            base.ExposeData();
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
            listingStandard.CheckboxLabeled("BPC.AutomaticPawnsInterruptSetting".Translate(), ref settings.automaticPawnsInterrupt,null,0f,1f);
            listingStandard.CheckboxLabeled("BPC.DisableBPCOnWorkTabSetting".Translate(), ref settings.disableBPCOnWorkTab, "BPC.DisableBPCOnWorkTabTooltip".Translate(),0f,1f);
            listingStandard.End();
        }

        public override string SettingsCategory()
        {
            return "BPC.BetterPawnControl".Translate();
        }
    }
}