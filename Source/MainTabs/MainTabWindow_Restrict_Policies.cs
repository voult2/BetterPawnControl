using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Linq;

namespace BetterPawnControl
{
    class MainTabWindow_Restrict_Policies : MainTabWindow_Restrict
    {
        bool showPaste = false;

        public override void PreOpen()
        {
            base.PreOpen();

            RestrictManager.CleanDeadMaps();

            RestrictManager.UpdateState(
                RestrictManager.links, this.Pawns.ToList(),
                RestrictManager.GetActivePolicy());

            RestrictManager.LoadState(
                RestrictManager.links, this.Pawns.ToList(), 
                RestrictManager.GetActivePolicy());

            RestrictManager.CleanDeadColonists(this.Pawns.ToList());
        }

        public override void PreClose()
        {
            base.PreClose();
            RestrictManager.CleanDeadMaps();
            RestrictManager.CleanDeadColonists(this.Pawns.ToList());
            RestrictManager.SaveCurrentState(this.Pawns.ToList());
            showPaste = false;
        }

        public override void DoWindowContents(Rect fillRect)
        {
            if (RestrictManager.DirtyPolicy)
            {
                RestrictManager.LoadState(
                    RestrictManager.links, this.Pawns.ToList(), 
                    RestrictManager.GetActivePolicy());
                RestrictManager.DirtyPolicy = false;
            }

            float offsetX = 300f;
            if (Widget_CSL.CLSAvailable)
            {
                offsetX = offsetX + 87f;
            }

            base.DoWindowContents(fillRect);
            Rect position = new Rect(0f, 0f, fillRect.width, 65f);

            GUI.BeginGroup(position);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect1 = 
                new Rect(offsetX, -8f, 165f, Mathf.Round(position.height / 3f));
            Widgets.Label(rect1, "BPC.CurrentRestrictPolicy".Translate());
            GUI.EndGroup();

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect(
                offsetX, Mathf.Round(position.height / 4f) - 4f, 
                rect1.width, Mathf.Round(position.height / 4f) + 4f);

            if (Widgets.ButtonText(
                rect2, RestrictManager.GetActivePolicy().label, 
                true, false, true))
            {
                RestrictManager.SaveCurrentState(this.Pawns.ToList());
                OpenRestrictPolicySelectMenu(
                    RestrictManager.links, this.Pawns.ToList());
            }
            offsetX += rect1.width;
            Rect rect3 = new Rect(
                offsetX, 0f, 20f, Mathf.Round(position.height / 2f));
            if (Widgets.ButtonText(rect3, "", true, false, true))
            {
                Find.WindowStack.Add(
                    new Dialog_ManagePolicies(Find.CurrentMap));
            }
            Rect rect4 = new Rect(offsetX + 3f, rect3.height / 4f, 14f, 14f);
            GUI.DrawTexture(rect4, Resources.Settings);
            TooltipHandler.TipRegion(rect4, "BPC.Settings".Translate());

            offsetX += rect3.width;
            Rect rect5 = new Rect(offsetX + 3f, rect3.height / 4f - 6f, 21f, 28f);
            if (Widgets.ButtonImage(rect5, ContentFinder<Texture2D>.Get("UI/Buttons/Copy", true)))
            {
                RestrictManager.CopyToClipboard();
                SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                showPaste = true;
            }
            TooltipHandler.TipRegion(rect5, "BPC.CopySchedule".Translate());

            if (showPaste)
            {
                offsetX += rect3.width;
                Rect rect6 = new Rect(offsetX + 3f, rect3.height / 4f - 6f, 21f, 28f);
                if (Widgets.ButtonImage(rect6, ContentFinder<Texture2D>.Get("UI/Buttons/Paste", true)))
                {
                    RestrictManager.PasteToActivePolicy();
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                }
                TooltipHandler.TipRegion(rect6, "BPC.PasteSchedule".Translate());
            }
        }

        private static void OpenRestrictPolicySelectMenu(
            List<RestrictLink> links, List<Pawn> pawns)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (Policy restrictPolicy in RestrictManager.policies)
            {
                list.Add(
                    new FloatMenuOption(
                        restrictPolicy.label,
                        delegate
                        {
                            RestrictManager.LoadState(
                                links,
                                pawns,
                                restrictPolicy);
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}