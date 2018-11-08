using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;

namespace BetterPawnControl
{
    class MainTabWindow_Assign_Policies : MainTabWindow_Assign
    {

        public override void PreOpen()
        {
            base.PreOpen();

            AssignManager.UpdateState(
                AssignManager.links, this.Pawns.ToList(),
                AssignManager.GetActivePolicy());

            AssignManager.LoadState(
                AssignManager.links, this.Pawns.ToList(),
                AssignManager.GetActivePolicy());

            AssignManager.CleanDeadMaps();

            AssignManager.CleanDeadColonists(this.Pawns.ToList());
        }

        public override void PreClose()
        {
            base.PreClose();
            AssignManager.CleanDeadMaps();
            AssignManager.CleanDeadColonists(this.Pawns.ToList());
            AssignManager.SaveCurrentState(this.Pawns.ToList());
        }

        public override void DoWindowContents(Rect fillRect)
        {
            if (AssignManager.DirtyPolicy)
            {
                AssignManager.LoadState(
                    AssignManager.links, this.Pawns.ToList(),
                    AssignManager.GetActivePolicy());
                AssignManager.DirtyPolicy = false;
            }

            float num = 5f;
            base.DoWindowContents(fillRect);
            Rect position = new Rect(0f, 0f, fillRect.width, 65f);

            GUI.BeginGroup(position);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect1 =
                new Rect(num, -8f, 165f, Mathf.Round(position.height / 3f));
            Widgets.Label(rect1, "BPC.CurrentAssignPolicy".Translate());
            GUI.EndGroup();

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect(
                num, Mathf.Round(position.height / 4f) - 4f,
                rect1.width, Mathf.Round(position.height / 4f) + 4f);

            if (Widgets.ButtonText(
                rect2, AssignManager.GetActivePolicy().label,
                true, false, true))
            {
                //CleanDeadColonists(this.pawns);
                AssignManager.SaveCurrentState(this.Pawns.ToList());
                OpenAssignPolicySelectMenu(
                    AssignManager.links, this.Pawns.ToList());
            }
            num += rect1.width;
            Rect rect3 = new Rect(
                num, 0f, 20f, Mathf.Round(position.height / 2f));
            if (Widgets.ButtonText(rect3, "", true, false, true))
            {
                Find.WindowStack.Add(
                    new Dialog_ManagePolicies(Find.CurrentMap));
            }
            Rect rect4 = new Rect(num + 3f, rect3.height / 4f, 14f, 14f);
            GUI.DrawTexture(rect4, Resources.Settings);
            TooltipHandler.TipRegion(rect4, "BPC.Settings".Translate());
        }

        private static void OpenAssignPolicySelectMenu(
            List<AssignLink> links, List<Pawn> pawns)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (Policy assignPolicy in AssignManager.policies)
            {
                list.Add(
                    new FloatMenuOption(
                        assignPolicy.label,
                        delegate
                        {
                            AssignManager.LoadState(
                                links,
                                pawns,
                                assignPolicy);
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}