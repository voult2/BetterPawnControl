using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using RimWorld;
using System;

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
        enum PawnType { Colonist, Prisoner };

        /// <summary>
        /// Copy paste from vanilla
        /// </summary>
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1300f, 840f);
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
            //this.closeOnEscapeKey = true;
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

            Rect lineWithTextPolicies = listing_Standard.GetRect(24f);
            DrawHorizontalLineWithTextCentered(inRect, lineWithTextPolicies, 0f, "BPC.PoliciesConfigText");
            listing_Standard.Gap(6f);

            Rect policiesHeader = listing_Standard.GetRect(38f);
            DoHeaderRow(policiesHeader);

            int rows =
                MaxNumber(
                    MaxNumber(
                        MaxNumber(
                            AnimalManager.policies.Count,
                            AssignManager.policies.Count),
                        RestrictManager.policies.Count),
                    WorkManager.policies.Count);

            for (int i = 0; i < rows; i++)
            {

                Policy workP = (i < WorkManager.policies.Count) ?
                    WorkManager.policies[i] : null;
                Policy restrictP = (i < RestrictManager.policies.Count) ?
                    RestrictManager.policies[i] : null;
                Policy assignP = (i < AssignManager.policies.Count) ?
                    AssignManager.policies[i] : null;
                Policy animalP = (i < AnimalManager.policies.Count) ?
                    AnimalManager.policies[i] : null;

                Rect policiesRow = listing_Standard.GetRect(24f);
                DoRow(policiesRow, workP, restrictP, assignP, animalP);
                listing_Standard.Gap(6f);
            }

            Rect newPolicyRow = listing_Standard.GetRect(24f);
            DoNewPoliciesRow(newPolicyRow);
            listing_Standard.Gap(24f);

            Rect lineWithTextDefaults = listing_Standard.GetRect(24f);
            DrawHorizontalLineWithTextCentered(inRect, lineWithTextDefaults, 50f,"BPC.DefaultsConfigText");
            listing_Standard.Gap(6f);

            Rect defaultsRow = listing_Standard.GetRect(24f);
            DoDefaultsRow(defaultsRow);
            listing_Standard.Gap(24f);

            Rect lineWithTextAlertRow = listing_Standard.GetRect(24f);
            DrawHorizontalLineWithTextCentered(inRect, lineWithTextAlertRow, 50f, "BPC.AlertConfigText", true);
            listing_Standard.Gap(6f);

            Rect alertRow = listing_Standard.GetRect(24f);
            DoAlertRow(alertRow);
            listing_Standard.Gap(12f);

            Rect AutomaticInterruptPawnsRow = listing_Standard.GetRect(24f);
            DrawAutomaticInterruptPawnsRow(AutomaticInterruptPawnsRow);
            listing_Standard.Gap(12f);

            listing_Standard.End();
        }
        private void DrawAutomaticInterruptPawnsRow(Rect rect)
        {
            rect.width = rect.width / 2;
            //rect.x = rect.width;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.CheckboxLabeled(rect, "BPC.AutomaticPawnsInterruptSetting".Translate(), ref AlertManager._automaticPawnsInterrupt, false, null, null, true);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void DrawHorizontalLineWithTextCentered(Rect inRect, Rect rect, float offset, string text, bool icon = false)
        {
            Widgets.DrawLineHorizontal(rect.x + inRect.width / 8, rect.y + 12f, inRect.width / 6);
            GUI.BeginGroup(rect);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleCenter;
            WidgetRow widgetRow = new WidgetRow(inRect.width / 3 + offset, 0);
            widgetRow.Label(text.Translate());
            if (icon) 
            {
                widgetRow.Icon(Resources.EmergencyOn, "BPC.EmergencyLocationTooltip".Translate());
            }
            GUI.EndGroup();
            Widgets.DrawLineHorizontal(rect.x + (inRect.width / 6 * 4) + (inRect.width / 6 / 2), rect.y + 12f, inRect.width / 6);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        /// <summary>
        /// Draw table row with AnimalPolicy label, rename button and delete 
        /// button
        /// </summary>
        private static void DoRow(
            Rect rect, Policy workP, Policy restrictP, Policy assignP, Policy animalP)
        {

            float one = rect.width / 4f;
            float two = one * 2f;
            float three = one * 3f;


            Rect rect1 = new Rect(rect.x, rect.y, one, rect.height);
            DoColumn(rect1, workP, Resources.Type.work);

            Rect rect2 = new Rect(one, rect.y, one, rect.height);
            DoColumn(rect2, restrictP, Resources.Type.restrict);

            Rect rect3 = new Rect(two, rect.y, one, rect.height);
            DoColumn(rect3, assignP, Resources.Type.assign);

            Rect rect4 = new Rect(three, rect.y, one, rect.height);
            DoColumn(rect4, animalP, Resources.Type.animal);
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
                        case Resources.Type.work:
                            WorkManager.DeletePolicy(policy);
                            break;
                    }
                }
            }
            GUI.EndGroup();
        }

        private static void DoHeaderRow(Rect rect)
        {
            float one = rect.width / 4f;
            float two = one * 2f;
            float three = one * 3f;
            float width = one - 4f;
            float offset = 0f;

            Rect rect1 = new Rect(offset, rect.y, width, rect.height);
            Rect rect2 = new Rect(one, rect.y, width, rect.height);
            Rect rect3 = new Rect(two + offset, rect.y, width, rect.height);
            Rect rect4 = new Rect(three + offset, rect.y, width, rect.height);

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect1, "BPC.WorkTab".Translate());
            Widgets.Label(rect2, "BPC.RestrictTab".Translate());
            Widgets.Label(rect3, "BPC.AssignTab".Translate());
            Widgets.Label(rect4, "BPC.AnimalTab".Translate());
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void DoNewPoliciesRow(Rect rect)
        {
            float one = rect.width / 4f;
            float two = one * 2f;
            float three = one * 3f;
            float buttonWidth = (one / 4f) * 3f;

            float offset = 0f;

            Rect rect1 =
                new Rect(offset, rect.y, buttonWidth, rect.height + 6f);
            Rect rect2 =
                new Rect(offset + one, rect.y, buttonWidth, rect.height + 6f);
            Rect rect3 =
                new Rect(offset + two, rect.y, buttonWidth, rect.height + 6f);
            Rect rect4 =
                new Rect(offset + three, rect.y, buttonWidth, rect.height + 6f);

            if (WorkManager.policies.Count < MAX_POLICIES &&
                Widgets.ButtonText(
                    rect1, "BPC.NewWorkPolicy".Translate(),
                    true, false, true))
            {
                int lastItem = WorkManager.policies.Count - 1;
                int label_id = WorkManager.policies[lastItem].id;
                label_id++;
                WorkManager.policies.Add(
                    new Policy(
                        label_id, "BPC.WorkPolicy".Translate() + label_id));
            }

            if (RestrictManager.policies.Count < MAX_POLICIES &&
                Widgets.ButtonText(
                    rect2, "BPC.NewRestrictPolicy".Translate(),
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
                    rect3,
                    "BPC.NewAssignPolicy".Translate(), true, false, true))
            {
                int lastItem = AssignManager.policies.Count - 1;
                int label_id = AssignManager.policies[lastItem].id;
                label_id++;
                AssignManager.policies.Add(
                    new Policy(label_id,
                    "BPC.AssignPolicy".Translate() + label_id));
            }

            if (AnimalManager.policies.Count < MAX_POLICIES &&
                Widgets.ButtonText(
                    rect4, "BPC.NewAnimalPolicy".Translate(),
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

        private static void DoDefaultsRow(Rect rect)
        {
            float one = rect.width / 8f;
            float two = one * 2f;
            float three = one * 3f;
            float four = one * 4f;
            float five = one * 5f;
            float six = one * 6f;
            float seven = one * 7f;
            float buttonWidth = 4f * one / 5f;

            Rect labelDefaultOutfit =
                new Rect(0, rect.y, one, rect.height + 6f);
            Rect buttonDefaultOutfit =
                new Rect(one, rect.y, buttonWidth, rect.height + 6f);

            Rect labelDefaultFood =
                new Rect(two, rect.y, one, rect.height + 6f);
            Rect buttonDefaultFood =
                new Rect(three, rect.y, buttonWidth, rect.height + 6f);

            Rect labelDefaultDrug =
                new Rect(four, rect.y, one, rect.height + 6f);
            Rect buttonDefaultDrug =
                new Rect(five, rect.y, buttonWidth, rect.height + 6f);

            Rect labelPrisionerDefaultFood =
                new Rect(six, rect.y, one, rect.height + 6f);
            Rect buttonPrisionerDefaultFood =
                new Rect(seven, rect.y, buttonWidth, rect.height + 6f);

            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(
                labelDefaultOutfit, "BPC.SelectedDefaultOutfit".Translate());

            Widgets.Label(
                labelDefaultFood, "BPC.SelectedDefaultFood".Translate());

            Widgets.Label(
                labelDefaultDrug, "BPC.SelectedDefaultDrug".Translate());

            Widgets.Label(
                labelPrisionerDefaultFood, "BPC.SelectedPrisionerDefaultFood".Translate());
  

            if (Widgets.ButtonText(
                buttonDefaultOutfit,
                AssignManager.DefaultOutfit.label, true, false, true))
            {
                OpenOutfitSelectMenu();
            }

            if (Widgets.ButtonText(
                buttonDefaultFood,
                AssignManager.DefaultFoodPolicy.label, true, false, true))
            {
                OpenFoodSelectMenu(PawnType.Colonist);
            }

            if (Widgets.ButtonText(
                buttonDefaultDrug,
                AssignManager.DefaultDrugPolicy.label, true, false, true))
            {
                OpenDrugSelectMenu();
            }

            if (Widgets.ButtonText(
                buttonPrisionerDefaultFood,
                AssignManager.DefaultPrisonerFoodPolicy.label, true, false, true))
            {
                OpenFoodSelectMenu(PawnType.Prisoner);
            }
            Text.Anchor = TextAnchor.UpperLeft;
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

        private static void OpenDrugSelectMenu()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (DrugPolicy drugPolicy in Current.Game.drugPolicyDatabase.AllPolicies)
            {
                list.Add(
                    new FloatMenuOption(
                        drugPolicy.label,
                        delegate
                        {
                            AssignManager.DefaultDrugPolicy = drugPolicy;
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void OpenFoodSelectMenu(PawnType type)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (FoodRestriction foodPolicy in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
            {
                list.Add(
                    new FloatMenuOption(
                        foodPolicy.label,
                        delegate
                        {
                            if (type == PawnType.Colonist)
                            {
                                AssignManager.DefaultFoodPolicy = foodPolicy;
                            }
                            else
                            {
                                AssignManager.DefaultPrisonerFoodPolicy = foodPolicy;
                            }
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void DoAlertRow(Rect rect)
        {
            float one = rect.width / 8f;
            float two = one * 2f;
            float three = one * 3f;
            float four = one * 4f;
            float five = one * 5f;
            float six = one * 6f;
            float seven = one * 7f;
            float buttonWidth = 4f * one / 5f;
            int alertLevel = 1;

            Rect labelWork =
                new Rect(0, rect.y, one, rect.height + 6f);
            Rect buttonWorkAlert =
                new Rect(one, rect.y, buttonWidth, rect.height + 6f);

            Rect labelRestrict =
                new Rect(two, rect.y, one, rect.height + 6f);
            Rect buttonRestrictAlert =
                new Rect(three, rect.y, buttonWidth, rect.height + 6f);

            Rect labelAssign =
                new Rect(four, rect.y, one, rect.height + 6f);
            Rect buttonAssignAlert =
                new Rect(five, rect.y, buttonWidth, rect.height + 6f);

            Rect labelAnimal =
                new Rect(six, rect.y, one, rect.height + 6f);
            Rect buttonAnimalAlert =
                new Rect(seven, rect.y, buttonWidth, rect.height + 6f);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(labelWork, "BPC.WorkTab".Translate());
            Widgets.Label(labelRestrict, "BPC.RestrictTab".Translate());
            Widgets.Label(labelAssign, "BPC.AssignTab".Translate());
            Widgets.Label(labelAnimal, "BPC.AnimalTab".Translate());


            DrawAlertButton(alertLevel, buttonWorkAlert, Resources.Type.work);
            DrawAlertButton(alertLevel, buttonRestrictAlert, Resources.Type.restrict);
            DrawAlertButton(alertLevel, buttonAssignAlert, Resources.Type.assign);
            DrawAlertButton(alertLevel, buttonAnimalAlert, Resources.Type.animal);
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void DrawAlertButton(int alertLevel, Rect buttonWorkAlert, Resources.Type type)
        {
            if (Widgets.ButtonText(
                buttonWorkAlert,
                AlertManager.GetAlertPolicy(alertLevel, type).label, true, false, true))
            {
                OpenPolicySelectMenu(type, alertLevel);
            }
        }

        private static void OpenPolicySelectMenu(Resources.Type type, int alertLevel)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            switch(type)
            {
                case Resources.Type.work:
                    foreach (Policy policy in WorkManager.policies)
                    {
                        FillMenu(type, alertLevel, list, policy);
                    }
                    break;
                
                case Resources.Type.restrict:
                    foreach (Policy policy in RestrictManager.policies)
                    {
                        FillMenu(type, alertLevel, list, policy);
                    }
                    break;

                case Resources.Type.assign:
                    foreach (Policy policy in AssignManager.policies)
                    {
                        FillMenu(type, alertLevel, list, policy);
                    }
                    break;

                case Resources.Type.animal:
                    foreach (Policy policy in AnimalManager.policies)
                    {
                        FillMenu(type, alertLevel, list, policy);
                    }
                    break;
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void FillMenu(Resources.Type type, int alertLevel, List<FloatMenuOption> list, Policy policy)
        {
            list.Add(
                new FloatMenuOption(
                    policy.label,
                    delegate
                    {
                        AlertManager.SaveState(
                            alertLevel, type, policy);

                    },
                    MenuOptionPriority.Default, null, null, 0f, null));
        }
    }
}
