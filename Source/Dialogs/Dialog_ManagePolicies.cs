﻿using System.Collections.Generic;
using UnityEngine;
using Verse;
using RimWorld;
using System;
using System.Linq;

namespace BetterPawnControl
{
    class Dialog_ManagePolicies : Window
    {
        private Map map;

        /// <summary>
        /// Copy paste from vanilla
        /// </summary>
		//private static readonly Regex validNameRegex = new Regex("^[a-zA-Z0-9 '\\-]*$");
        private const int MAX_POLICIES = 15;
        private const float NORMAL_HEIGHT = 24f;
        private const float SMALL_HEIGHT = 6f;
        private const float OFFSETX = 20f;       
        private static float NTABSCOLUMNS = 5f;
        enum PawnType { Colonist, Prisoner, Slave };

        /// <summary>
        /// Copy paste from vanilla
        /// </summary>
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1600f, 855f);
            }
        }

        public Dialog_ManagePolicies(Map map)
        {
            this.map = map;
            this.forcePause = true;
            this.doCloseX = true;
            //this.closeOnEscapeKey = true;
            this.doCloseButton = true;
            this.closeOnClickedOutside = true;
            this.absorbInputAroundWindow = true;
            this.resizeable = true;
            this.draggable = true;
            if (Widget_ModsAvailable.WTBAvailable)
            {
                NTABSCOLUMNS = 6f;
            }
        }

        public override void PostOpen()
        {
            base.PostOpen();
            
            Nullable<float> number = LoadedModManager.GetMod<BetterPawnControl>().GetSettings<Settings>().settingsWindowPosX;
            windowRect.x = number  ?? ResolutionUtility.NativeResolution.width / 2;

            number = LoadedModManager.GetMod<BetterPawnControl>().GetSettings<Settings>().settingsWindowPosY;
            windowRect.y = number ?? ResolutionUtility.NativeResolution.height / 2;

            number = LoadedModManager.GetMod<BetterPawnControl>().GetSettings<Settings>().settingsWindowWidth;
            windowRect.width = number ?? 1280f;

            number = LoadedModManager.GetMod<BetterPawnControl>().GetSettings<Settings>().settingsWindowHeight;
            windowRect.height = number ?? 870f;

            windowRect.width = windowRect.width < 1280f ? 1280f : windowRect.width;
            windowRect.height = windowRect.height < 870f ? 870f : windowRect.height;
        }

        public override void PreClose()
        {
            base.PreClose();
            LoadedModManager.GetMod<BetterPawnControl>().GetSettings<Settings>().settingsWindowPosX = windowRect.x;
            LoadedModManager.GetMod<BetterPawnControl>().GetSettings<Settings>().settingsWindowPosY = windowRect.y;
            LoadedModManager.GetMod<BetterPawnControl>().GetSettings<Settings>().settingsWindowWidth = windowRect.width;
            LoadedModManager.GetMod<BetterPawnControl>().GetSettings<Settings>().settingsWindowHeight = windowRect.height;
        }

        /// <summary>
        /// Draw AnimalPolicys management table à lá vanilla
        /// </summary>
        public override void DoWindowContents(Rect inRect)
        {

            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = inRect.width;
            listing_Standard.Begin(inRect);

            Rect lineWithTextPolicies = listing_Standard.GetRect(NORMAL_HEIGHT);
            DrawHorizontalLineWithTextCentered(inRect, lineWithTextPolicies, 0f, "BPC.PoliciesConfigText");
            listing_Standard.Gap(SMALL_HEIGHT);

            Rect policiesHeader = listing_Standard.GetRect(NORMAL_HEIGHT + 8f);

            int rows = MaxNumber(
                WeaponsManager.policies.Count,
                MechManager.policies.Count,
                AnimalManager.policies.Count,
                AssignManager.policies.Count,
                ScheduleManager.policies.Count,
                WorkManager.policies.Count
            );

            float border = 3f;
            float columWidth = policiesHeader.width / NTABSCOLUMNS;
            float columHeight = (NORMAL_HEIGHT + SMALL_HEIGHT) * rows + 38f + NORMAL_HEIGHT + SMALL_HEIGHT + 8f;
            DoBackground(columWidth * 0f, policiesHeader.y, columWidth, columHeight, border, 0f);
            DoBackground(columWidth * 1f, policiesHeader.y, columWidth, columHeight, border, 0f);
            DoBackground(columWidth * 2f, policiesHeader.y, columWidth, columHeight, border, 0f);
            DoBackground(columWidth * 3f, policiesHeader.y, columWidth, columHeight, border, 0f);   
            DoBackground(columWidth * 4f, policiesHeader.y, columWidth, columHeight, border, 0f);
            
            if (Widget_ModsAvailable.WTBAvailable)
            {
                DoBackground(columWidth * 5f, policiesHeader.y, columWidth, columHeight, border, 0f);
            }

            DoHeaderRow(policiesHeader);

            for (int i = 0; i < rows; i++)
            {
                Policy workP = (i < WorkManager.policies.Count) ?
                    WorkManager.policies[i] : null;
                Policy restrictP = (i < ScheduleManager.policies.Count) ?
                    ScheduleManager.policies[i] : null;
                Policy assignP = (i < AssignManager.policies.Count) ?
                    AssignManager.policies[i] : null;
                Policy animalP = (i < AnimalManager.policies.Count) ?
                    AnimalManager.policies[i] : null;
                Policy mechP = (i < MechManager.policies.Count) ?
                    MechManager.policies[i] : null;
                Policy weaponsP = (i < WeaponsManager.policies.Count) ?
                    WeaponsManager.policies[i] : null;

                Rect policiesRow = listing_Standard.GetRect(NORMAL_HEIGHT);
                DoRow(policiesRow, workP, restrictP, assignP, animalP, mechP, weaponsP);
                listing_Standard.Gap(6f);
            }

            Rect newPolicyRow = listing_Standard.GetRect(NORMAL_HEIGHT);
            DoNewPoliciesRow(newPolicyRow);
            listing_Standard.Gap(NORMAL_HEIGHT);

            Rect lineWithTextAlertRow = listing_Standard.GetRect(NORMAL_HEIGHT);
            DrawHorizontalLineWithTextCentered(inRect, lineWithTextAlertRow, -30f, "BPC.AlertConfigText", true);
            listing_Standard.Gap(SMALL_HEIGHT);

            Rect alertRow = listing_Standard.GetRect(NORMAL_HEIGHT);
            border = 3f;
            columWidth = alertRow.width / NTABSCOLUMNS;
            columHeight = NORMAL_HEIGHT + 18f;
            DoBackground(columWidth * 0f, alertRow.y, columWidth, columHeight, border, 0f); // Column 1
            DoBackground(columWidth * 1f, alertRow.y, columWidth, columHeight, border, 0f); // Column 2
            DoBackground(columWidth * 2f, alertRow.y, columWidth, columHeight, border, 0f); // Column 3
            DoBackground(columWidth * 3f, alertRow.y, columWidth, columHeight, border, 0f); // Column 4
            DoBackground(columWidth * 4f, alertRow.y, columWidth, columHeight, border, 0f); // Column 5
            if (Widget_ModsAvailable.WTBAvailable)
            {
                DoBackground(columWidth * 5f, alertRow.y, columWidth, columHeight, border, 0f); // Column 6 - Weapons
            }
            
            DoAlertRow(alertRow);
            listing_Standard.Gap(NORMAL_HEIGHT);

            Rect lineWithTextDefaults = listing_Standard.GetRect(NORMAL_HEIGHT);
            DrawHorizontalLineWithTextCentered(inRect, lineWithTextDefaults, 50f, "BPC.DefaultsConfigText");
            listing_Standard.Gap(SMALL_HEIGHT);

            Rect defaultsHeaders = listing_Standard.GetRect(NORMAL_HEIGHT);

            columWidth = defaultsHeaders.width / 5f;
            columHeight = NORMAL_HEIGHT * 5f - 8f;
            
            if (Widget_ModsAvailable.WTBAvailable)
            {
                columHeight = NORMAL_HEIGHT * 8f - 8f;
            }
                                   
            DoBackground(columWidth * 0f, defaultsHeaders.y, 2f * columWidth, columHeight, border, 0f);
            DoBackground(columWidth * 2f, defaultsHeaders.y, columWidth, columHeight, border, 0f);
            DoBackground(columWidth * 3f, defaultsHeaders.y, 2f * columWidth, columHeight, border, 0f);

            DoDefaultsRowHeaders(defaultsHeaders);
            listing_Standard.Gap(-10f);

            Rect defaultsRowOne = listing_Standard.GetRect(NORMAL_HEIGHT);
            DoDefaultsRowLabels(defaultsRowOne, 1);
            listing_Standard.Gap(-5f);

            Rect defaultsRowTwo = listing_Standard.GetRect(NORMAL_HEIGHT);
            DoDefaultsRowButtons(defaultsRowTwo, 1);
            listing_Standard.Gap(0f);

            Rect defaultsRowThree = listing_Standard.GetRect(NORMAL_HEIGHT);
            DoDefaultsRowLabels(defaultsRowThree, 2);
            listing_Standard.Gap(-5f);

            Rect defaultsRowFour = listing_Standard.GetRect(NORMAL_HEIGHT);
            DoDefaultsRowButtons(defaultsRowFour, 2);
            listing_Standard.Gap(SMALL_HEIGHT);

            if (Widget_ModsAvailable.WTBAvailable)
            {
                Rect defaultsRowFive = listing_Standard.GetRect(NORMAL_HEIGHT);
                DoDefaultsRowLabels(defaultsRowFive, 3);
                listing_Standard.Gap(-5f);

                Rect defaultsRowSix = listing_Standard.GetRect(NORMAL_HEIGHT);
                DoDefaultsRowButtons(defaultsRowSix, 3);
                listing_Standard.Gap(SMALL_HEIGHT);
            }

            listing_Standard.End();
        }


        private static void DoBackground(float x, float y, float width, float height, float border, float gap)
        {
            Rect bg = new Rect(x + border + gap, y - 4f - border, width - 2f * (border + gap), height);
            Widgets.DrawBoxSolid(bg, Color.HSVToRGB(0.583333f, 0.28f, 0.14f));
        }

        private static void DrawHorizontalLineWithTextCentered(Rect inRect, Rect rect, float offset, string text, bool icon = false)
        {
            GUI.color = Color.gray;
            //Widgets.DrawLineHorizontal(rect.x + inRect.width / 8, rect.y + 12f, inRect.width / 6);
            GUI.BeginGroup(rect);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleCenter;
            WidgetRow widgetRow = new WidgetRow(inRect.width / 3 + offset, 0f);
            widgetRow.Label(text.Translate());
            if (icon) 
            {
                widgetRow.Icon(Resources.Textures.EmergencyOn, "BPC.EmergencyLocationTooltip".Translate());
            }
            GUI.EndGroup();
            //Widgets.DrawLineHorizontal(rect.x + (inRect.width / 6 * 4) + (inRect.width / 6 / 2), rect.y + 12f, inRect.width / 6);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
        }

        /// <summary>
        /// Draw table row with AnimalPolicy label, rename button and delete 
        /// button
        /// </summary>
        private static void DoRow( Rect rect, Policy workP, Policy restrictP, Policy assignP, Policy animalP, Policy mechP, Policy weaponsP)
        {
            float one = rect.width / NTABSCOLUMNS;
            float two = one * 2f;
            float three = one * 3f;
            float four = one * 4f;
            float five = one * 5f;

            Rect rect1 = new Rect(rect.x + OFFSETX, rect.y, one, rect.height);
            DoColumn(rect1, workP, Resources.Type.work);

            Rect rect2 = new Rect(one + OFFSETX, rect.y, one, rect.height);
            DoColumn(rect2, restrictP, Resources.Type.restrict);

            Rect rect3 = new Rect(two + OFFSETX, rect.y, one, rect.height);
            DoColumn(rect3, assignP, Resources.Type.assign);

            Rect rect4 = new Rect(three + OFFSETX, rect.y, one, rect.height);
            DoColumn(rect4, animalP, Resources.Type.animal);

            Rect rect5 = new Rect(four + OFFSETX, rect.y, one, rect.height);
            DoColumn(rect5, mechP, Resources.Type.mech);

            Rect rect6 = new Rect(five + OFFSETX, rect.y, one, rect.height);
            DoColumn(rect6, weaponsP, Resources.Type.weapons);
        }

        private static void DoColumn(Rect rect, Policy policy, Resources.Type type)
        {
            GUI.BeginGroup(rect);
            WidgetRow widgetRow = new WidgetRow(0f, 0f, UIDirection.RightThenUp, 99999f, 4f);
            widgetRow.Gap(4f);
            if (policy != null)
            {
                widgetRow.Label(policy.label, 138f);
                if (widgetRow.ButtonText(
                    "BPC.Rename".Translate(), null, true, false))
                {
                    Find.WindowStack.Add(new Dialog_RenamePolicy(policy, type));
                }
                if (policy.id > 0 && widgetRow.ButtonIcon(ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true), null))
                {
                    switch (type)
                    {
                        case Resources.Type.assign:
                            AssignManager.DeletePolicy(policy);
                            if (policy == AlertManager.GetAlertPolicy(1, type))
                            {
                                AlertManager.SetAlertPolicy(1, type, AssignManager.GetPolicy(0));
                            }
                            break;
                        case Resources.Type.animal:
                            AnimalManager.DeletePolicy(policy);
                            if (policy == AlertManager.GetAlertPolicy(1, type))
                            {
                                AlertManager.SetAlertPolicy(1, type, AnimalManager.GetPolicy(0));
                            }
                            break;
                        case Resources.Type.restrict:
                            ScheduleManager.DeletePolicy(policy);
                            if (policy == AlertManager.GetAlertPolicy(1, type))
                            {
                                AlertManager.SetAlertPolicy(1, type, ScheduleManager.GetPolicy(0));
                            }
                            break;
                        case Resources.Type.work:
                            WorkManager.DeletePolicy(policy);
                            if (policy == AlertManager.GetAlertPolicy(1, type))
                            {
                                AlertManager.SetAlertPolicy(1, type, WorkManager.GetPolicy(0));
                            }
                            break;
                        case Resources.Type.mech:
                            MechManager.DeletePolicy(policy);
                            if (policy == AlertManager.GetAlertPolicy(1, type))
                            {
                                AlertManager.SetAlertPolicy(1, type, MechManager.GetPolicy(0));
                            }
                            break;
                        case Resources.Type.weapons:
                            WeaponsManager.DeletePolicy(policy);
                            if (policy == AlertManager.GetAlertPolicy(1, type))
                            {
                                AlertManager.SetAlertPolicy(1, type, WeaponsManager.GetPolicy(0));
                            }
                            break;
                    }
                }
            }
            GUI.EndGroup();
        }

        private static void DoHeaderRow(Rect rect)
        {
            float one = rect.width / NTABSCOLUMNS;
            float two = one * 2f;
            float three = one * 3f;
            float four = one * 4f;
            float five = one * 5f;
            float width = one - 4f;
            float offset = 0f;

            Rect rect1 = new Rect(offset, rect.y, width, rect.height);
            Rect rect2 = new Rect(one, rect.y, width, rect.height);
            Rect rect3 = new Rect(two + offset, rect.y, width, rect.height);
            Rect rect4 = new Rect(three + offset, rect.y, width, rect.height);
            Rect rect5 = new Rect(four + offset, rect.y, width, rect.height);
            Rect rect6 = new Rect(five + offset, rect.y, width, rect.height);

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect1, "BPC.WorkTab".Translate());
            Widgets.Label(rect2, "BPC.RestrictTab".Translate());
            Widgets.Label(rect3, "BPC.AssignTab".Translate());
            Widgets.Label(rect4, "BPC.AnimalTab".Translate());
            Widgets.Label(rect5, "BPC.MechTab".Translate());
            if (Widget_ModsAvailable.WTBAvailable)
            {
                Widgets.Label(rect6, "BPC.WeaponsTab".Translate());
            }
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void DoNewPoliciesRow(Rect rect)
        {
            float one = rect.width / NTABSCOLUMNS;
            float two = one * 2f;
            float three = one * 3f;
            float four = one * 4f;
            float five = one * 5f;
            float buttonWidth = (one / 4f) * 3f;

            Rect rect1 = new Rect(OFFSETX, rect.y, buttonWidth, rect.height + 6f);
            Rect rect2 = new Rect(OFFSETX + one, rect.y, buttonWidth, rect.height + 6f);
            Rect rect3 = new Rect(OFFSETX + two, rect.y, buttonWidth, rect.height + 6f);
            Rect rect4 = new Rect(OFFSETX + three, rect.y, buttonWidth, rect.height + 6f);
            Rect rect5 = new Rect(OFFSETX + four, rect.y, buttonWidth, rect.height + 6f);
            Rect rect6 = new Rect(OFFSETX + five, rect.y, buttonWidth, rect.height + 6f);

            if (WorkManager.policies.Count < MAX_POLICIES && Widgets.ButtonText(rect1, "BPC.NewWorkPolicy".Translate(), true, false, true))
            {
                int lastItem = WorkManager.policies.Count - 1;
                int label_id = WorkManager.policies[lastItem].id;
                label_id++;
                WorkManager.policies.Add(new Policy(label_id, "BPC.WorkPolicy".Translate() + label_id));
            }

            if (ScheduleManager.policies.Count < MAX_POLICIES && Widgets.ButtonText(rect2, "BPC.NewRestrictPolicy".Translate(), true, false, true))
            {
                int lastItem = ScheduleManager.policies.Count - 1;
                int label_id = ScheduleManager.policies[lastItem].id;
                label_id++;
                ScheduleManager.policies.Add(new Policy(label_id, "BPC.RestrictPolicy".Translate() + label_id));
            }

            if (AssignManager.policies.Count < MAX_POLICIES && Widgets.ButtonText(rect3, "BPC.NewAssignPolicy".Translate(), true, false, true))
            {
                int lastItem = AssignManager.policies.Count - 1;
                int label_id = AssignManager.policies[lastItem].id;
                label_id++;
                AssignManager.policies.Add( new Policy(label_id, "BPC.AssignPolicy".Translate() + label_id));
            }

            if (AnimalManager.policies.Count < MAX_POLICIES && Widgets.ButtonText(rect4, "BPC.NewAnimalPolicy".Translate(), true, false, true))
            {
                int lastItem = AnimalManager.policies.Count - 1;
                int label_id = AnimalManager.policies[lastItem].id;
                label_id++;
                AnimalManager.policies.Add(new Policy(label_id, "BPC.AnimalPolicy".Translate() + label_id));
            }

            if (MechManager.policies.Count < MAX_POLICIES && Widgets.ButtonText(rect5, "BPC.NewMechPolicy".Translate(), true, false, true))
            {
                int lastItem = MechManager.policies.Count - 1;
                int label_id = MechManager.policies[lastItem].id;
                label_id++;
                MechManager.policies.Add(new Policy(label_id, "BPC.MechPolicy".Translate() + label_id));
            }
            
            if (WeaponsManager.policies.Count < MAX_POLICIES && Widgets.ButtonText(rect6, "BPC.NewWeaponsPolicy".Translate(), true, false, true))
            {
                int lastItem = WeaponsManager.policies.Count - 1;
                int label_id = WeaponsManager.policies[lastItem].id;
                label_id++;
                WeaponsManager.policies.Add(new Policy(label_id, "BPC.WeaponsPolicy".Translate() + label_id));
            }

        }

        private static void DoDefaultsRowHeaders(Rect rect)
        {
            float one = rect.width / 5f;
            float two = one * 2f;
            float three = one * 3f;
            //float four = one * 4f;
            //float width = one * 0.8f;

            Rect rect1 = new Rect(rect.x, rect.y, one*2f, rect.height);
            Rect rect2 = new Rect(two, rect.y, one, rect.height);
            Rect rect3 = new Rect(three , rect.y, one*2f, rect.height);

            GUI.color = Color.gray;
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleCenter;

            Widgets.Label(rect1, "BPC.ColonistDefaultHeader".Translate());
            Widgets.Label(rect2, "BPC.PrisionerDefaultHeader".Translate());
            Widgets.Label(rect3, "BPC.SlaveDefaultHeader".Translate());

            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void DoDefaultsRowLabels(Rect rect, byte rowNumber)
        {
            float one = rect.width / 5f;
            float two = one * 2f;
            float three = one * 3f;
            float four = one * 4f;
            float buttonWidth = 4f * one / 5f;
            float buttonHeight = rect.height;

            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleCenter;

            if (rowNumber == 1)
            {
                Rect labelDefaultOutfit = new Rect(0, rect.y, one, buttonHeight);
                Rect labelDefaultFood = new Rect(one, rect.y, one, buttonHeight);
                Rect labelPrisionerDefaultFood = new Rect(two, rect.y, one, buttonHeight);
                Rect labelSlaveDefaultOutfit = new Rect(three, rect.y, one, buttonHeight);
                Rect labelSlaveDefaultFood = new Rect(four, rect.y, one, buttonHeight);
                
                Widgets.Label(labelDefaultOutfit, "BPC.SelectedDefaultOutfit".Translate());
                Widgets.Label(labelDefaultFood, "BPC.SelectedDefaultFood".Translate());
                Widgets.Label(labelPrisionerDefaultFood, "BPC.SelectedPrisionerDefaultFood".Translate());
                Widgets.Label(labelSlaveDefaultOutfit, "BPC.SelectedSlaveDefaultOutfit".Translate());
                Widgets.Label(labelSlaveDefaultFood, "BPC.SelectedSlaveDefaultFood".Translate());
            }
            else if (rowNumber == 2)
            {
                Rect labelDefaultDrugs = new Rect(0, rect.y, one, buttonHeight);
                Rect labelDefaultReading = new Rect(one, rect.y, one, buttonHeight);
                Rect labelSlaveDrugs = new Rect(three, rect.y, one, buttonHeight);
                Rect labelSlaveReading = new Rect(four, rect.y, one, buttonHeight);

                Widgets.Label(labelDefaultDrugs, "BPC.SelectedDefaultDrug".Translate());
                Widgets.Label(labelDefaultReading, "BPC.SelectedDefaultReading".Translate());
                Widgets.Label(labelSlaveDrugs, "BPC.SelectedSlaveDefaultDrugs".Translate());
                Widgets.Label(labelSlaveReading, "BPC.SelectedSlaveDefaultReading".Translate());
            }
            else
            {
                if (Widget_ModsAvailable.WTBAvailable)
                {
                    Rect labelDefaultWeapons = new Rect(0, rect.y, one, buttonHeight);
                    Widgets.Label(labelDefaultWeapons, "BPC.SelectedDefaultWeapons".Translate());
                }
            }

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void DoDefaultsRowButtons(Rect rect, byte rowNumber)
        {
            float one = rect.width / 5f;
            float two = one * 2f;
            float three = one * 3f;
            float four = one * 4f;
            float buttonWidth =  0.6f * one;
            float buttonHeight = rect.height;
            float alignCenter = 0.2f * one;

            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.MiddleCenter;

            if (rowNumber == 1)
            {
                Rect buttonDefaultOutfit = new Rect(0f + alignCenter, rect.y, buttonWidth, buttonHeight);
                Rect buttonDefaultFood = new Rect(one + alignCenter, rect.y, buttonWidth, buttonHeight);
                Rect buttonPrisionerDefaultFood = new Rect(two + alignCenter, rect.y, buttonWidth, buttonHeight);
                Rect buttonSlaveDefaultOutfit = new Rect(three + alignCenter, rect.y, buttonWidth, buttonHeight);
                Rect buttonSlaveDefaultFood = new Rect(four + alignCenter, rect.y, buttonWidth, buttonHeight);

                if (Widgets.ButtonText(buttonDefaultOutfit, AssignManager.DefaultOutfit.label, true, false, true))
                {
                    OpenOutfitSelectMenu(PawnType.Colonist);
                }

                if (Widgets.ButtonText(buttonDefaultFood, AssignManager.DefaultFoodPolicy.label, true, false, true))
                {
                    OpenFoodSelectMenu(PawnType.Colonist);
                }

                if (Widgets.ButtonText(buttonPrisionerDefaultFood, AssignManager.DefaultPrisonerFoodPolicy.label, true, false, true))
                {
                    OpenFoodSelectMenu(PawnType.Prisoner);
                }

                if (Widgets.ButtonText(buttonSlaveDefaultOutfit, AssignManager.DefaultSlaveOutfit.label, true, false, true))
                {
                    OpenOutfitSelectMenu(PawnType.Slave);
                }

                if (Widgets.ButtonText(buttonSlaveDefaultFood, AssignManager.DefaultSlaveFoodPolicy.label, true, false, true))
                {
                    OpenFoodSelectMenu(PawnType.Slave);
                }
            }
            else if (rowNumber == 2)
            {

                Rect buttonDefaultDrugs = new Rect(0f + alignCenter, rect.y, buttonWidth, buttonHeight);
                Rect buttonDefaultReading = new Rect(one + alignCenter, rect.y, buttonWidth, buttonHeight);
                Rect buttonSlaveDefaultDrugs = new Rect(three + alignCenter, rect.y, buttonWidth, buttonHeight);
                Rect buttonSlaveDefaultReading = new Rect(four + alignCenter, rect.y, buttonWidth, buttonHeight);

                if (Widgets.ButtonText(buttonDefaultDrugs, AssignManager.DefaultDrugPolicy.label, true, false, true))
                {
                    OpenDrugSelectMenu(PawnType.Colonist);
                }

                if (Widgets.ButtonText(buttonDefaultReading, AssignManager.DefaultReadingPolicy.label, true, false, true))
                {
                    OpenReadingSelectMenu(PawnType.Colonist);
                }

                if (Widgets.ButtonText(buttonSlaveDefaultDrugs, AssignManager.DefaultSlaveDrugPolicy.label, true, false, true))
                {
                    OpenDrugSelectMenu(PawnType.Slave);
                }

                if (Widgets.ButtonText(buttonSlaveDefaultReading, AssignManager.DefaultSlaveReadingPolicy.label, true, false, true))
                {
                    OpenReadingSelectMenu(PawnType.Slave);
                }
            }
            else
            {
                if (Widget_ModsAvailable.WTBAvailable)
                {
                    Rect buttonDefaultWeapons = new Rect(0f + alignCenter, rect.y, buttonWidth, buttonHeight);
                    if (Widgets.ButtonText(buttonDefaultWeapons, Widget_WeaoponsTabReborn.GetLoadoutNameById(WeaponsManager.DefaultWeaponsLoadoutById), true, false, true))
                    {
                        OpenWeaponsSelectMenu(PawnType.Colonist);
                    }
                }
            }
            
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }
        
        private static int MaxNumber(params int[] numbers)
        {
            var max = numbers.Max();
            return max;
        }

        private static void OpenOutfitSelectMenu(PawnType type)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (ApparelPolicy apparelPolicy in Current.Game.outfitDatabase.AllOutfits)
            {
                list.Add(
                    new FloatMenuOption(
                        apparelPolicy.label,
                        delegate
                        {
                            if (type == PawnType.Colonist)
                            {
                                AssignManager.DefaultOutfit = apparelPolicy;
                            }
                            else //if (type == PawnType.Slave)
                            {
                                AssignManager.DefaultSlaveOutfit = apparelPolicy;
                            }                            
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void OpenDrugSelectMenu(PawnType type)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (DrugPolicy drugPolicy in Current.Game.drugPolicyDatabase.AllPolicies)
            {
                list.Add(
                    new FloatMenuOption(
                        drugPolicy.label,
                        delegate
                        {
                            if (type == PawnType.Colonist)
                            {
                                AssignManager.DefaultDrugPolicy = drugPolicy;
                            }
                            else //if (type == PawnType.Slave)
                            {
                                AssignManager.DefaultSlaveDrugPolicy = drugPolicy;
                            }
                            
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void OpenFoodSelectMenu(PawnType type)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (FoodPolicy foodPolicy in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
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
                            else if (type == PawnType.Prisoner)
                            {
                                AssignManager.DefaultPrisonerFoodPolicy = foodPolicy;
                            }
                            else //if (type == PawnType.Slave)
                            {
                                AssignManager.DefaultSlaveFoodPolicy = foodPolicy;
                            }
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void OpenWeaponsSelectMenu(PawnType type)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            Dictionary<string, int> weaponsLoadoutDatabase = Widget_WeaoponsTabReborn.GetWeaponsLoadoutsDatabase();
            if (weaponsLoadoutDatabase != null)
            {
                foreach (var weaponLoadout in weaponsLoadoutDatabase)
                {
                    list.Add(
                        new FloatMenuOption(
                            weaponLoadout.Key,
                            delegate
                            {
                                WeaponsManager.DefaultWeaponsLoadoutById = weaponLoadout.Value;
                            },
                            MenuOptionPriority.Default, null, null, 0f, null));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }            
        }

        private static void OpenReadingSelectMenu(PawnType type)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (ReadingPolicy readingPolicy in Current.Game.readingPolicyDatabase.AllReadingPolicies)
            {
                list.Add(
                    new FloatMenuOption(
                        readingPolicy.label,
                        delegate
                        {
                            if (type == PawnType.Colonist)
                            {
                                AssignManager.DefaultReadingPolicy = readingPolicy;
                            }
                            else //if (type == PawnType.Slave)
                            {
                                AssignManager.DefaultSlaveReadingPolicy = readingPolicy;
                            }

                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }


        private static void DoAlertRow(Rect rect)
        {
            float one = rect.width / (NTABSCOLUMNS * 2);
            float two = one * 2f;
            float three = one * 3f;
            float four = one * 4f;
            float five = one * 5f;
            float six = one * 6f;
            float seven = one * 7f;
            float eight = one * 8f;
            float nine = one * 9f;
            float buttonWidth = 4f * one / 5f;
            int alertLevel = 1;

            Rect labelWork = new Rect(0, rect.y, one, rect.height + 6f);
            Rect buttonWorkAlert = new Rect(one, rect.y, buttonWidth, rect.height + 6f);

            Rect labelRestrict = new Rect(two, rect.y, one, rect.height + 6f);
            Rect buttonRestrictAlert = new Rect(three, rect.y, buttonWidth, rect.height + 6f);

            Rect labelAssign = new Rect(four, rect.y, one, rect.height + 6f);
            Rect buttonAssignAlert = new Rect(five, rect.y, buttonWidth, rect.height + 6f);

            Rect labelAnimal = new Rect(six, rect.y, one, rect.height + 6f);
            Rect buttonAnimalAlert = new Rect(seven, rect.y, buttonWidth, rect.height + 6f);

            Rect labelMech = new Rect(eight, rect.y, one, rect.height + 6f);
            Rect buttonMechAlert = new Rect(nine, rect.y, buttonWidth, rect.height + 6f);

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(labelWork, "BPC.WorkTab".Translate());
            Widgets.Label(labelRestrict, "BPC.RestrictTab".Translate());
            Widgets.Label(labelAssign, "BPC.AssignTab".Translate());
            Widgets.Label(labelAnimal, "BPC.AnimalTab".Translate());
            Widgets.Label(labelMech, "BPC.MechTab".Translate());

            DrawAlertButton(alertLevel, buttonWorkAlert, Resources.Type.work);
            DrawAlertButton(alertLevel, buttonRestrictAlert, Resources.Type.restrict);
            DrawAlertButton(alertLevel, buttonAssignAlert, Resources.Type.assign);
            DrawAlertButton(alertLevel, buttonAnimalAlert, Resources.Type.animal);
            DrawAlertButton(alertLevel, buttonMechAlert, Resources.Type.mech);

            if (Widget_ModsAvailable.WTBAvailable)
            {
                float ten = one * 10f;
                float eleven = one * 11f;
                Rect labelWeapons = new Rect(ten, rect.y, one, rect.height + 6f);
                Rect buttonWeaponsAlert = new Rect(eleven, rect.y, buttonWidth, rect.height + 6f);
                Widgets.Label(labelWeapons, "BPC.WeaponsTab".Translate());
                DrawAlertButton(alertLevel, buttonWeaponsAlert, Resources.Type.weapons);
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void DrawAlertButton(int alertLevel, Rect buttonWorkAlert, Resources.Type type)
        {
            if (Widgets.ButtonText(buttonWorkAlert, AlertManager.GetAlertPolicy(alertLevel, type).label, true, false, true))
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
                    foreach (Policy policy in ScheduleManager.policies)
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
                case Resources.Type.mech:
                    foreach (Policy policy in MechManager.policies)
                    {
                        FillMenu(type, alertLevel, list, policy);
                    }
                    break;
                case Resources.Type.weapons:
                    foreach (Policy policy in WeaponsManager.policies)
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
                        AlertManager.SaveState(alertLevel, type, policy);

                    },
                    MenuOptionPriority.Default, null, null, 0f, null));
        }
    }
}
