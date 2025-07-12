using UnityEngine;
using Verse;

namespace BetterPawnControl
{
    public class BetterPawnControlMod : Mod
    {
        public static BetterPawnControlSettings Settings { get; set; }

        public BetterPawnControlMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<BetterPawnControlSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("BPC.AutomaticPawnsInterruptSetting".Translate(), ref Settings.automaticPawnsInterrupt);
            listingStandard.CheckboxLabeled("BPC.SaveInventoryStock".Translate(), ref Settings.saveInventoryStock);
            if (Widget_ModsAvailable.WorkTabAvailable)
            {
                listingStandard.CheckboxLabeled("BPC.DisableBPCOnWorkTabSetting".Translate(), ref Settings.disableBPCOnWorkTab, "BPC.DisableBPCOnWorkTabTooltip".Translate());
                if (!Widget_ModsAvailable.DisableBPCOnWorkTab)
                {
                    listingStandard.CheckboxLabeled("BPC.DisableBPCWorkTabInnerPrioritiesSetting".Translate(), ref Settings.disableBPCWorkTabInnerPriorities, "BPC.DisableBPCWorkTabInnerPrioritiesTooltip".Translate());
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
