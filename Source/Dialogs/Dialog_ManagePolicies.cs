using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

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
        private static List<PolicySection> sections = new List<PolicySection>();
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

            var count = CreateSections();
            NTABSCOLUMNS = count;
        }

        private int CreateSections()
        {
            sections.Clear();

            sections.Add(PolicySection.Create(Resources.Type.work, ref WorkManager.policies));
            sections.Add(PolicySection.Create(Resources.Type.restrict, ref ScheduleManager.policies));
            sections.Add(PolicySection.Create(Resources.Type.assign, ref AssignManager.policies));
            sections.Add(PolicySection.Create(Resources.Type.animal, ref AnimalManager.policies));
            sections.Add(PolicySection.Create(Resources.Type.mech, ref MechManager.policies));

            if (Widget_ModsAvailable.WTBAvailable)
            {
                sections.Add(PolicySection.Create(Resources.Type.weapons, ref WeaponsManager.policies));
            }

            if (Widget_ModsAvailable.MiscRobotsAvailable)
            {
                sections.Add(PolicySection.Create(Resources.Type.robots, ref RobotManager.policies));
            }

            return sections.Count;
        }

        public override void PostOpen()
        {
            base.PostOpen();

            var settings = LoadedModManager.GetMod<BetterPawnControl>().GetSettings<Settings>();

            Nullable<float> number = settings.settingsWindowPosX;
            windowRect.x = number ?? ResolutionUtility.NativeResolution.width / 2;

            number = settings.settingsWindowPosY;
            windowRect.y = number ?? ResolutionUtility.NativeResolution.height / 2;

            number = settings.settingsWindowWidth;
            windowRect.width = number ?? 1280f;

            number = settings.settingsWindowHeight;
            windowRect.height = number ?? 870f;

            windowRect.width = windowRect.width < 1280f ? 1280f : windowRect.width;
            windowRect.height = windowRect.height < 870f ? 870f : windowRect.height;
        }

        public override void PreClose()
        {
            base.PreClose();
            var settings = LoadedModManager.GetMod<BetterPawnControl>().GetSettings<Settings>();
            settings.settingsWindowPosX = windowRect.x;
            settings.settingsWindowPosY = windowRect.y;
            settings.settingsWindowWidth = windowRect.width;
            settings.settingsWindowHeight = windowRect.height;
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

            int rows = sections.Max(x => x.Policies.Count);

            float border = 3f;
            float columWidth = policiesHeader.width / NTABSCOLUMNS;
            float columHeight = (NORMAL_HEIGHT + SMALL_HEIGHT) * rows + 38f + NORMAL_HEIGHT + SMALL_HEIGHT + 8f;

            for (int num = 0; num < sections.Count; num++)
            {
                DoBackground(columWidth * num, policiesHeader.y, columWidth, columHeight, border, 0f);
            }

            DoHeaderRow(policiesHeader);

            for (int rowNum = 0; rowNum < rows; rowNum++)
            {
                Rect policiesRow = listing_Standard.GetRect(NORMAL_HEIGHT);
                DoRow(policiesRow, rowNum);
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
            for (int num = 0; num < sections.Count; num++)
            {
                DoBackground(columWidth * num, alertRow.y, columWidth, columHeight, border, 0f);
            }
            
            DoAlertRow(alertRow);
            listing_Standard.Gap(NORMAL_HEIGHT);

            Rect lineWithTextDefaults = listing_Standard.GetRect(NORMAL_HEIGHT);
            DrawHorizontalLineWithTextCentered(inRect, lineWithTextDefaults, 50f, "BPC.DefaultsConfigText");
            listing_Standard.Gap(SMALL_HEIGHT);

            Rect defaultsHeaders = listing_Standard.GetRect(NORMAL_HEIGHT);

            columWidth = defaultsHeaders.width / 5f;
            columHeight = NORMAL_HEIGHT * 8f;

            if (Widget_ModsAvailable.WTBAvailable)
            {
                columHeight = NORMAL_HEIGHT * 11f;
            }
                                   
            DoBackground(columWidth * 0f, defaultsHeaders.y, 2f * columWidth, columHeight, border, 0f);
            DoBackground(columWidth * 2f, defaultsHeaders.y, columWidth, columHeight, border, 0f);
            DoBackground(columWidth * 3f, defaultsHeaders.y, 2f * columWidth, columHeight, border, 0f);

            DoDefaultsRowHeaders(defaultsHeaders);
            //listing_Standard.Gap(-10f);
            listing_Standard.Gap(-5f);

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

            Rect defaultsRowFive = listing_Standard.GetRect(NORMAL_HEIGHT);
            DoDefaultsRowLabels(defaultsRowFive, 3);
            listing_Standard.Gap(-5f);

            Rect defaultsRowSix = listing_Standard.GetRect(NORMAL_HEIGHT);
            DoDefaultsRowButtons(defaultsRowSix, 3);
            listing_Standard.Gap(SMALL_HEIGHT);

            if (Widget_ModsAvailable.WTBAvailable)
            {
                Rect defaultsRowSeven = listing_Standard.GetRect(NORMAL_HEIGHT);
                DoDefaultsRowLabels(defaultsRowSeven, 4);
                listing_Standard.Gap(-5f);

                Rect defaultsRowEight = listing_Standard.GetRect(NORMAL_HEIGHT);
                DoDefaultsRowButtons(defaultsRowEight, 4);
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
        /// Draw table row with Policy label, rename button and delete button
        /// </summary>
        private static void DoRow(Rect rect, int policyNum)
        {
            float columnWidth = rect.width / NTABSCOLUMNS;

            for (int num = 0; num < sections.Count; num++)
            {
                var section = sections[num];
                var policy = policyNum < section.Policies.Count
                    ? section.Policies[policyNum]
                    : null;
                Rect columnRect = new Rect((columnWidth * num) + OFFSETX, rect.y, columnWidth, rect.height);
                DoColumn(columnRect, policy, section.Type);
            }
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
                        case Resources.Type.robots:
                            RobotManager.DeletePolicy(policy);
                            if (policy == AlertManager.GetAlertPolicy(1, type))
                            {
                                AlertManager.SetAlertPolicy(1, type, RobotManager.GetPolicy(0));
                            }
                            break;
                    }
                }
            }
            GUI.EndGroup();
        }

        private static void DoHeaderRow(Rect rect)
        {
            float totalColumns = rect.width / NTABSCOLUMNS;
            float columnWidth = totalColumns - 4f;

            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            for (int num = 0; num < sections.Count; num++)
            {
                var section = sections[num];
                float offset = columnWidth * num;
                var columnRect = new Rect(offset, rect.y, columnWidth, rect.height);

                Widgets.Label(columnRect, section.HeaderLabelKey.Translate());
            }
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void DoNewPoliciesRow(Rect rect)
        {
            float columnWidth = rect.width / NTABSCOLUMNS;
            float buttonWidth = (columnWidth / 4f) * 3f;
            for (int num = 0; num < sections.Count; num++)
            {
                var section = sections[num];
                var newPolicyRowRect = new Rect(OFFSETX + (columnWidth * num), rect.y, buttonWidth, rect.height + 6f);
                var button = Widgets.ButtonText(newPolicyRowRect, section.NewButtonLabelKey.Translate(), true, false, true);
                if (section.Policies.Count < MAX_POLICIES && button)
                {
                    var labelId = section.Policies.Last().id + 1;
                    section.Policies.Add(new Policy(labelId, section.HeaderLabelKey.Translate() + labelId));
                }
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
            Widgets.Label(rect2, "BPC.PrisonerDefaultHeader".Translate());
            
            if (ModsConfig.IdeologyActive) 
            {
                Widgets.Label(rect3, "BPC.SlaveDefaultHeader".Translate());
            }

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
                Rect labelPrisonerDefaultFood = new Rect(two, rect.y, one, buttonHeight);
                Rect labelSlaveDefaultOutfit = new Rect(three, rect.y, one, buttonHeight);
                Rect labelSlaveDefaultFood = new Rect(four, rect.y, one, buttonHeight);
                
                Widgets.Label(labelDefaultOutfit, "BPC.SelectedDefaultOutfit".Translate());
                Widgets.Label(labelDefaultFood, "BPC.SelectedDefaultFood".Translate());
                Widgets.Label(labelPrisonerDefaultFood, "BPC.SelectedPrisonerDefaultFood".Translate());

                if (ModsConfig.IdeologyActive)
                {
                    Widgets.Label(labelSlaveDefaultOutfit, "BPC.SelectedSlaveDefaultOutfit".Translate());
                    Widgets.Label(labelSlaveDefaultFood, "BPC.SelectedSlaveDefaultFood".Translate());
                }
            }
            else if (rowNumber == 2)
            {
                Rect labelDefaultDrugs = new Rect(0, rect.y, one, buttonHeight);
                Rect labelDefaultReading = new Rect(one, rect.y, one, buttonHeight);
                Rect labelPrisonerDefaultMed = new Rect(two, rect.y, one, buttonHeight);
                Rect labelSlaveDrugs = new Rect(three, rect.y, one, buttonHeight);
                Rect labelSlaveReading = new Rect(four, rect.y, one, buttonHeight);

                Widgets.Label(labelDefaultDrugs, "BPC.SelectedDefaultDrug".Translate());
                Widgets.Label(labelDefaultReading, "BPC.SelectedDefaultReading".Translate());
                Widgets.Label(labelPrisonerDefaultMed, "BPC.SelectedPrisonerDefaultMeds".Translate());
                if (ModsConfig.IdeologyActive)
                {
                    Widgets.Label(labelSlaveDrugs, "BPC.SelectedSlaveDefaultDrugs".Translate());
                    Widgets.Label(labelSlaveReading, "BPC.SelectedSlaveDefaultReading".Translate());
                }
            }
            else if (rowNumber == 3)
            {
                Rect labelDefaultMeds = new Rect(0, rect.y, one, buttonHeight);
                Rect labelSlaveDefaultMeds = new Rect(three, rect.y, one, buttonHeight);

                Widgets.Label(labelDefaultMeds, "BPC.SelectedDefaultMeds".Translate());
                if (ModsConfig.IdeologyActive)
                {
                    Widgets.Label(labelSlaveDefaultMeds, "BPC.SelectedSlaveDefaultMeds".Translate());
                }
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
                Rect buttonPrisonerDefaultFood = new Rect(two + alignCenter, rect.y, buttonWidth, buttonHeight);
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

                if (Widgets.ButtonText(buttonPrisonerDefaultFood, AssignManager.DefaultPrisonerFoodPolicy.label, true, false, true))
                {
                    OpenFoodSelectMenu(PawnType.Prisoner);
                }

                if (ModsConfig.IdeologyActive)
                {
                    if (Widgets.ButtonText(buttonSlaveDefaultOutfit, AssignManager.DefaultSlaveOutfit.label, true, false, true))
                    {
                        OpenOutfitSelectMenu(PawnType.Slave);
                    }

                    if (Widgets.ButtonText(buttonSlaveDefaultFood, AssignManager.DefaultSlaveFoodPolicy.label, true, false, true))
                    {
                        OpenFoodSelectMenu(PawnType.Slave);
                    }
                }
            }
            else if (rowNumber == 2)
            {
                Rect buttonDefaultDrugs = new Rect(0f + alignCenter, rect.y, buttonWidth, buttonHeight);
                Rect buttonDefaultReading = new Rect(one + alignCenter, rect.y, buttonWidth, buttonHeight);
                Rect buttonPrisonerDefaultMeds = new Rect(two + alignCenter, rect.y, buttonWidth, buttonHeight);
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

                if (Widgets.ButtonText(buttonPrisonerDefaultMeds, AssignManager.DefaultPrisonerMedicinePolicy.GetLabel(), true, false, true))
                {
                    OpenMedicineSelectMenu(PawnType.Prisoner);
                }

                if (ModsConfig.IdeologyActive)
                {
                    if (Widgets.ButtonText(buttonSlaveDefaultDrugs, AssignManager.DefaultSlaveDrugPolicy.label, true, false, true))
                    {
                        OpenDrugSelectMenu(PawnType.Slave);
                    }

                    if (Widgets.ButtonText(buttonSlaveDefaultReading, AssignManager.DefaultSlaveReadingPolicy.label, true, false, true))
                    {
                        OpenReadingSelectMenu(PawnType.Slave);
                    }
                }
            }
            else if (rowNumber == 3)
            {
                Rect buttonDefaultMeds = new Rect(0f + alignCenter, rect.y, buttonWidth, buttonHeight);
                Rect buttonSlaveDefaultMeds = new Rect(three + alignCenter, rect.y, buttonWidth, buttonHeight);

                if (Widgets.ButtonText(buttonDefaultMeds, AssignManager.DefaultMedsPolicy.GetLabel(), true, false, true))
                {
                    OpenMedicineSelectMenu(PawnType.Colonist);
                }

                if (ModsConfig.IdeologyActive)
                {
                    if (Widgets.ButtonText(buttonSlaveDefaultMeds, AssignManager.DefaultSlaveMedicinePolicy.GetLabel(), true, false, true))
                    {
                        OpenMedicineSelectMenu(PawnType.Slave);
                    }
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

        private static void OpenOutfitSelectMenu(PawnType type)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (var apparelPolicy in Current.Game.outfitDatabase.AllOutfits)
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
                            else if (ModsConfig.IdeologyActive && type == PawnType.Slave)
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

            foreach (var drugPolicy in Current.Game.drugPolicyDatabase.AllPolicies)
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
                            else if (ModsConfig.IdeologyActive && type == PawnType.Slave)
                            {
                                AssignManager.DefaultSlaveDrugPolicy = drugPolicy;
                            }
                            
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void OpenReadingSelectMenu(PawnType type)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (var readingPolicy in Current.Game.readingPolicyDatabase.AllReadingPolicies)
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
                            else if (ModsConfig.IdeologyActive && type == PawnType.Slave)
                            {
                                AssignManager.DefaultSlaveReadingPolicy = readingPolicy;
                            }

                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        internal static MedicalCareCategory[] medicalCategories = Enum.GetValues(typeof(MedicalCareCategory)).Cast<MedicalCareCategory>().ToArray();

        private static void OpenMedicineSelectMenu(PawnType type)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            foreach (MedicalCareCategory category in medicalCategories)
            {
                list.Add(
                    new FloatMenuOption(
                        category.GetLabel(),
                        delegate
                        {
                            if (type == PawnType.Colonist)
                            {
                                AssignManager.DefaultMedsPolicy = category;
                            }
                            else if (type == PawnType.Prisoner)
                            {
                                AssignManager.DefaultPrisonerMedicinePolicy = category;
                            }
                            else if (ModsConfig.IdeologyActive && type == PawnType.Slave)
                            {
                                AssignManager.DefaultSlaveMedicinePolicy = category;
                            }
                        },
                        MenuOptionPriority.Default,
                        extraPartWidth: 30,
                        extraPartOnGUI: rect => {
                            int IconSize = 24;
                            Rect optionIconRect = new Rect(0f, 0f, IconSize, IconSize)
                                .CenteredOnXIn(rect)
                                .CenteredOnYIn(rect);
                            GUI.DrawTexture(optionIconRect, Resources.Textures.medcareGraphics[(int)category]);
                        return false;
                        }));
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

        private static void OpenFoodSelectMenu(PawnType type)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (var foodPolicy in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
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
                            else if (ModsConfig.IdeologyActive && type == PawnType.Slave)
                            {
                                AssignManager.DefaultSlaveFoodPolicy = foodPolicy;
                            }
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void DoAlertRow(Rect rect)
        {
            float columnWidth = rect.width / (NTABSCOLUMNS * 2);
            float buttonWidth = 4f * columnWidth / 5f;
            int alertLevel = 1;
            float posX = 0;
            
            for (int num = 0; num < sections.Count; num++)
            {
                var section = sections[num];
                Rect label = new Rect(posX, rect.y, columnWidth, rect.height + 6f);
                posX += columnWidth;
                Rect buttonAlert = new Rect(posX, rect.y, buttonWidth, rect.height + 6f);
                posX += columnWidth;

                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(label, section.HeaderLabelKey.Translate());
                DrawAlertButton(alertLevel, buttonAlert, section);
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static void DrawAlertButton(int alertLevel, Rect buttonWorkAlert, PolicySection section)
        {
            var type = section.Type;
            if (Widgets.ButtonText(buttonWorkAlert, AlertManager.GetAlertPolicy(alertLevel, type).label, true, false, true))
            {
                OpenPolicySelectMenu(alertLevel, section);
            }
        }

        private static void OpenPolicySelectMenu(int alertLevel, PolicySection section)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (var policy in section.Policies)
            {
                FillMenu(section.Type, alertLevel, list, policy);
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

        class PolicySection
        {
            public Resources.Type Type { get; private set; }
            public string HeaderLabelKey { get; private set; }
            public string NewButtonLabelKey { get; private set; }
            public List<Policy> Policies { get; private set; }

            public static PolicySection Create(Resources.Type type, ref List<Policy> policies)
            {
                var newSection = new PolicySection
                {
                    Type = type,
                    HeaderLabelKey = GetHeaderLabelKey(type),
                    NewButtonLabelKey = GetNewButtonLabelKey(type),
                    Policies = policies,
                };
                return newSection;
            }

            private static string GetHeaderLabelKey(Resources.Type type)
            {
                switch (type)
                {
                    case Resources.Type.work:
                        return "BPC.WorkTab";
                    case Resources.Type.restrict:
                        return "BPC.RestrictTab";
                    case Resources.Type.assign:
                        return "BPC.AssignTab";
                    case Resources.Type.animal:
                        return "BPC.AnimalTab";
                    case Resources.Type.mech:
                        return "BPC.MechTab";
                    case Resources.Type.weapons:
                        return "BPC.WeaponsTab";
                    case Resources.Type.robots:
                        return "BPC.RobotsTab";
                }

                throw new NotImplementedException($"Type {type} is not supported.");
            }

            private static string GetNewButtonLabelKey(Resources.Type type)
            {
                switch (type)
                {
                    case Resources.Type.work:
                        return "BPC.NewWorkPolicy";
                    case Resources.Type.restrict:
                        return "BPC.NewRestrictPolicy";
                    case Resources.Type.assign:
                        return "BPC.NewAssignPolicy";
                    case Resources.Type.animal:
                        return "BPC.NewAnimalPolicy";
                    case Resources.Type.mech:
                        return "BPC.NewMechPolicy";
                    case Resources.Type.weapons:
                        return "BPC.NewWeaponsPolicy";
                    case Resources.Type.robots:
                        return "BPC.NewRobotsPolicy";
                }

                throw new NotImplementedException($"Type {type} is not supported.");
            }
        }
    }
}
