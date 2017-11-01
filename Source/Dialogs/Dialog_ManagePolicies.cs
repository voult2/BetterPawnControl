using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using RimWorld;


namespace BetterPawnControl
{
    class Dialog_ManagePolicies : Window
    {
        private Map map;

        /// <summary>
        /// Copy paste from vanilla
        /// </summary>
		private static Regex validNameRegex = new Regex("^[a-zA-Z0-9 '\\-]*$");
        private const int MAX_POLICIES = 15;

        /// <summary>
        /// Copy paste from vanilla
        /// </summary>
		public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1000f, 700f);
            }
        }

        /// <summary>
        /// Copy paste from vanilla
        /// </summary>
        public Dialog_ManagePolicies(Map map)
        {
            this.map = map;
            this.forcePause = true;
            this.doCloseX = true;
            this.closeOnEscapeKey = true;
            this.doCloseButton = true;
            this.closeOnClickedOutside = true;
            this.absorbInputAroundWindow = true;
        }

        /// <summary>
        /// Draw AnimalPolicys management table à lá vanilla
        /// </summary>
        public override void DoWindowContents(Rect inRect)
        {

            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = inRect.width;
            listing_Standard.Begin(inRect);

            Rect rect = listing_Standard.GetRect(60f);
            DoHeaderRow(rect);

            int rows = 
                MaxNumber(
                    MaxNumber(
                        AnimalManager.policies.Count, 
                        AssignManager.policies.Count), 
                    RestrictManager.policies.Count);

            for (int i = 0; i < rows; i++)
            {
                Policy restrictP = (i < RestrictManager.policies.Count) ? 
                    RestrictManager.policies[i] : null;
                Policy assignP = (i < AssignManager.policies.Count) ? 
                    AssignManager.policies[i] : null;
                Policy animalP = (i < AnimalManager.policies.Count) ? 
                    AnimalManager.policies[i] : null;

                Rect rect2 = listing_Standard.GetRect(24f);
                DoRow(rect2, restrictP, assignP, animalP);
                listing_Standard.Gap(6f);
            }

            Rect rect3 = listing_Standard.GetRect(24f);
            DoNewPoliciesRow(rect3);
            listing_Standard.Gap(6f);

            Rect rect4 = listing_Standard.GetRect(24f);
            Widgets.DrawLineHorizontal(rect4.x, rect4.y + 12f, inRect.width);
            listing_Standard.Gap(6f);

            Rect rect5 = listing_Standard.GetRect(24f);
            DoDefaultOutfitRow(rect5);
            listing_Standard.End();
        }

        /// <summary>
        /// Draw table row with AnimalPolicy label, rename button and delete 
        /// button
        /// </summary>
        private static void DoRow(
            Rect rect, Policy restrictP, Policy assignP, Policy animalP)
        {

            float one = rect.width / 3f;
            float two = one * 2f;
            float width = one - 4f;

            //Restrict column
            Rect rect1 = new Rect(rect.x, rect.y, one, rect.height);
            DoColumn(rect1, restrictP, Resources.Type.restrict);

            Rect rect2 = new Rect(one, rect.y, one, rect.height);
            DoColumn(rect2, assignP, Resources.Type.assign);

            Rect rect3 = new Rect(two, rect.y, one, rect.height);
            DoColumn(rect3, animalP, Resources.Type.animal);
        }

        private static void DoColumn(
            Rect rect, Policy policy, Resources.Type type)
        {
            GUI.BeginGroup(rect);
            WidgetRow widgetRow = 
                new WidgetRow(0f, 0f, UIDirection.RightThenUp, 99999f, 4f);
            widgetRow.Gap(4f);
            if (policy != null)
            {
                widgetRow.Label(policy.label, 138f);
                if (widgetRow.ButtonText(
                    "BPC.Rename".Translate(), null, true, false))
                {
                    Find.WindowStack.Add(new Dialog_RenamePolicy(policy, type));
                }
                if (policy.id > 0 && 
                    widgetRow.ButtonIcon(
                        ContentFinder<Texture2D>.Get(
                            "UI/Buttons/Delete", true), null))
                {
                    switch (type)
                    {
                        case Resources.Type.assign:
                            AssignManager.DeletePolicy(policy);
                            break;
                        case Resources.Type.animal:
                            AnimalManager.DeletePolicy(policy);
                            break;
                        case Resources.Type.restrict:
                            RestrictManager.DeletePolicy(policy);
                            break;
                    }
                }
            }
            GUI.EndGroup();
        }

        private static void DoHeaderRow(Rect rect)
        {
            float one = rect.width / 3f;
            float two = one * 2f;
            float width = one - 4f;
            float offset = 0f;

            Rect rect1 = new Rect(offset, rect.y, width, rect.height + 6f);
            Rect rect2 = new Rect(one, rect.y, width, rect.height + 6f);
            Rect rect3 = new Rect(two + offset, rect.y, width, rect.height + 6f);

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect1, "BPC.RestrictTab".Translate());
            Widgets.Label(rect2, "BPC.AssignTab".Translate());
            Widgets.Label(rect3, "BPC.AnimalTab".Translate());
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void DoNewPoliciesRow(Rect rect)
        {

            float one = rect.width / 3f;
            float two = one * 2f;
            float buttonWidth = (one / 4f) * 3f;

            float offset = 0f;
            //float offset = one / 2f - buttonWidth / 2f;

            Rect rect1 = 
                new Rect(offset, rect.y, buttonWidth, rect.height + 6f);
            Rect rect2 = 
                new Rect(offset + one, rect.y, buttonWidth, rect.height + 6f);
            Rect rect3 =
                new Rect(offset + two, rect.y, buttonWidth, rect.height + 6f);

            if (RestrictManager.policies.Count < MAX_POLICIES && 
                Widgets.ButtonText(
                    rect1, "BPC.NewRestrictPolicy".Translate(), 
                    true, false, true))
            {
                int lastItem = RestrictManager.policies.Count - 1;
                int label_id = RestrictManager.policies[lastItem].id;
                label_id++;
                RestrictManager.policies.Add(
                    new Policy(
                        label_id, 
                        "BPC.RestrictPolicy".Translate() + label_id));
            }

            if (AssignManager.policies.Count < MAX_POLICIES && 
                Widgets.ButtonText(
                    rect2, 
                    "BPC.NewAssignPolicy".Translate(), true, false, true))
            {
                int lastItem = AssignManager.policies.Count - 1;
                int label_id = AssignManager.policies[lastItem].id;
                label_id++;
                AssignManager.policies.Add(
                    new Policy(label_id, 
                    "BPC.AssignPolicy".Translate() + label_id));

                Rect rect4 = 
                    new Rect(
                        offset, rect2.height + 6f, 
                        buttonWidth, rect.height + 6f);
                if (Widgets.ButtonText(
                    rect2, 
                    "BPC.DefaultOutfit".Translate(), true, false, true))
                {
                    OpenOutfitSelectMenu();
                }

            }

            if (AnimalManager.policies.Count < MAX_POLICIES && 
                Widgets.ButtonText(
                    rect3, "BPC.NewAnimalPolicy".Translate(), 
                    true, false, true))
            {
                int lastItem = AnimalManager.policies.Count - 1;
                int label_id = AnimalManager.policies[lastItem].id;
                label_id++;
                AnimalManager.policies.Add(
                    new Policy(
                        label_id, "BPC.AnimalPolicy".Translate() + label_id));
            }
        }

        private static void DoDefaultOutfitRow(Rect rect)
        {
            float one = rect.width / 4f;
            float two = one * 2f;

            Rect label = new Rect(one, rect.y, one, rect.height + 6f);
            Rect button = new Rect(two, rect.y, one, rect.height + 6f);

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(label, "BPC.SelectedDefaultOutfit".Translate());
            Text.Anchor = TextAnchor.UpperLeft;

            if (Widgets.ButtonText(
                button, AssignManager.DefaultOutfit.label, true, false, true))
            {
                OpenOutfitSelectMenu();
            }
        }

        private static int MaxNumber(int first, int second)
        {
            return (first > second ? first : second);
        }

        private static void OpenOutfitSelectMenu()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (Outfit outfit in Current.Game.outfitDatabase.AllOutfits)
            {
                list.Add(
                    new FloatMenuOption(
                        outfit.label, 
                        delegate 
                        {
                            AssignManager.DefaultOutfit = outfit;
                        }, 
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}
