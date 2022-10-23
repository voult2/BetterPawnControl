using HarmonyLib;
using UnityEngine;
using Verse;
using RimWorld;
using System.Linq;
using System.Collections.Generic;
using Verse.Sound;

namespace BetterPawnControl.Patches
{

    [HarmonyPatch(typeof(PawnTable), nameof(PawnTable.PawnTableOnGUI))]
    static class PawnTable_PawnTableOnGUI
    {
        private const string NUMBERS_DEFNAME = "Numbers_Animals";

        static void Postfix(PawnTable __instance, Vector2 position, PawnTableDef ___def)
        {

            if (___def == PawnTableDefOf.Assign)
            {
                DrawAssignBPCButtons(__instance, position);
            }

            if (___def == PawnTableDefOf.Restrict)
            {
                DrawSheduleBPCButtons(__instance, position);
            }

            if (___def == PawnTableDefOf.Work && !Widget_ModsAvailable.DisableBPCOnWorkTab)
            {
                DrawWorkBPCButtons(__instance, position);
            }

            if (___def == PawnTableDefOf.Animals || ___def.defName == NUMBERS_DEFNAME)
            {
                DrawAnimalBPCButtons(__instance, position);
            }
        }

        private static void DrawWorkBPCButtons(PawnTable __instance, Vector2 position)
        {
            if (WorkManager.DirtyPolicy)
            {
                WorkManager.LoadState(WorkManager.links, WorkManager.Colonists().ToList(), WorkManager.GetActivePolicy());
                WorkManager.DirtyPolicy = false;
            }
                       
            DrawBPCButtons_WorkTab(position, 5f, __instance.Size.y + 15f, WorkManager.Colonists().ToList());
        }

        private static void DrawAnimalBPCButtons(PawnTable __instance, Vector2 position)
        {
            if (AnimalManager.DirtyPolicy)
            {
                AnimalManager.LoadState(AnimalManager.links, AnimalManager.Animals().ToList(), AnimalManager.GetActivePolicy());
                AnimalManager.DirtyPolicy = false;
            }

            if (Widget_ModsAvailable.AnimalTabAvailable)
            {
                position.x = position.x + 40f;
                position.y = position.y + 7f;
            }

            DrawBPCButtons_AnimalTab(position, 5f, __instance.Size.y + 15f, AnimalManager.Animals().ToList());
        }

        private static void DrawSheduleBPCButtons(PawnTable __instance, Vector2 position)
        {
            if (ScheduleManager.DirtyPolicy)
            {
                ScheduleManager.LoadState(ScheduleManager.links, ScheduleManager.Colonists().ToList(), ScheduleManager.GetActivePolicy());
                ScheduleManager.DirtyPolicy = false;
            }

            if (Widget_ModsAvailable.CSLAvailable)
            {
                position.x = position.x + 87f;
            }

            DrawBPCButtons_ScheduleTab(position, 5f, __instance.Size.y + 15f, ScheduleManager.Colonists().ToList());
        }

        private static void DrawAssignBPCButtons(PawnTable __instance, Vector2 position)
        {
            if (AssignManager.DirtyPolicy)
            {
                AssignManager.LoadState(AssignManager.links, AssignManager.Colonists().ToList(), AssignManager.GetActivePolicy());
                AssignManager.DirtyPolicy = false;
            }

            DrawBPCButtons_AssignTab(position, 5f, __instance.Size.y + 15f, AssignManager.Colonists().ToList());
        }

        private static void DrawBPCButtons_AssignTab(Vector2 position, float X_ExtraSpace, float Y_ExtraSpace, List<Pawn> colonists)
        {

            Rect pos = new Rect(position.x + X_ExtraSpace, position.y + Y_ExtraSpace, 600f, 65f);
            float offSetX = 0f;

            GUI.BeginGroup(pos);

            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect1 = new Rect(offSetX, -8f, 165f, Mathf.Round(pos.height / 3f));
            Widgets.Label(rect1, "BPC.CurrentAssignPolicy".Translate());

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect(offSetX, Mathf.Round(pos.height / 4f) - 4f, rect1.width, Mathf.Round(pos.height / 4f) + 4f);

            if (Widgets.ButtonText(rect2, AssignManager.GetActivePolicy().label, true, false, true, null))
            {
                AssignManager.SaveCurrentState(colonists);
                OpenAssignPolicySelectMenu(AssignManager.links, colonists);
            }

            offSetX += rect1.width;
            Rect rect3 = new Rect(offSetX, 0f, 20f, Mathf.Round(pos.height / 2f));
            if (Widgets.ButtonText(rect3, "", true, false, true, null))
            {
                Find.WindowStack.Add(new Dialog_ManagePolicies(Find.CurrentMap));
            }
            Rect rect4 = new Rect(offSetX + 3f, rect3.height / 4f, 14f, 14f);
            GUI.DrawTexture(rect4, Resources.Textures.Settings);
            TooltipHandler.TipRegion(rect4, "BPC.Settings".Translate());

            offSetX += rect3.width;
            Rect rect5 = new Rect(offSetX + 3f, rect3.height / 4f - 6f, 21f, 28f);
            if (Widgets.ButtonImage(rect5, ContentFinder<Texture2D>.Get("UI/Buttons/Copy", true)))
            {
                AssignManager.CopyToClipboard();
                SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                AssignManager.showPaste = true;
            }
            TooltipHandler.TipRegion(rect5, "BPC.Copy".Translate());

            if (AssignManager.showPaste)
            {
                offSetX += rect3.width;
                Rect rect6 = new Rect(offSetX + 3f, rect3.height / 4f - 6f, 21f, 28f);
                if (Widgets.ButtonImage(rect6, ContentFinder<Texture2D>.Get("UI/Buttons/Paste", true)))
                {
                    AssignManager.PasteToActivePolicy();
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                }
                TooltipHandler.TipRegion(rect6, "BPC.Paste".Translate());
            }

            GUI.EndGroup();
        }

        private static void OpenAssignPolicySelectMenu(List<AssignLink> links, List<Pawn> pawns)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (Policy assignPolicy in AssignManager.policies)
            {
                list.Add(
                    new FloatMenuOption(assignPolicy.label,
                        delegate
                        {
                            AssignManager.LoadState(links, pawns, assignPolicy);
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void DrawBPCButtons_ScheduleTab(Vector2 position, float X_ExtraSpace, float Y_ExtraSpace, List<Pawn> colonists)
        {

            Rect pos = new Rect(position.x + X_ExtraSpace, position.y + Y_ExtraSpace, 600f, 65f);
            float offSetX = 0f;

            GUI.BeginGroup(pos);

            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect1 = new Rect(offSetX, -8f, 165f, Mathf.Round(pos.height / 3f));
            Widgets.Label(rect1, "BPC.CurrentRestrictPolicy".Translate());

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect(offSetX, Mathf.Round(pos.height / 4f) - 4f, rect1.width, Mathf.Round(pos.height / 4f) + 4f);

            if (Widgets.ButtonText(rect2, ScheduleManager.GetActivePolicy().label, true, false, true, null))
            {
                ScheduleManager.SaveCurrentState(colonists);
                OpenSchedulePolicySelectMenu(ScheduleManager.links, colonists);
            }
            offSetX += rect1.width;
            Rect rect3 = new Rect(offSetX, 0f, 20f, Mathf.Round(pos.height / 2f));
            if (Widgets.ButtonText(rect3, "", true, false, true, null))
            {
                Find.WindowStack.Add(new Dialog_ManagePolicies(Find.CurrentMap));
            }
            Rect rect4 = new Rect(offSetX + 3f, rect3.height / 4f, 14f, 14f);
            GUI.DrawTexture(rect4, Resources.Textures.Settings);
            TooltipHandler.TipRegion(rect4, "BPC.Settings".Translate());

            offSetX += rect3.width;
            Rect rect5 = new Rect(offSetX + 3f, rect3.height / 4f - 6f, 21f, 28f);
            if (Widgets.ButtonImage(rect5, ContentFinder<Texture2D>.Get("UI/Buttons/Copy", true)))
            {
                ScheduleManager.CopyToClipboard();
                SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                ScheduleManager.showPaste = true;
            }
            TooltipHandler.TipRegion(rect5, "BPC.CopySchedule".Translate());

            if (ScheduleManager.showPaste)
            {
                offSetX += rect3.width;
                Rect rect6 = new Rect(offSetX + 3f, rect3.height / 4f - 6f, 21f, 28f);
                if (Widgets.ButtonImage(rect6, ContentFinder<Texture2D>.Get("UI/Buttons/Paste", true)))
                {
                    ScheduleManager.PasteToActivePolicy();
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                }
                TooltipHandler.TipRegion(rect6, "BPC.PasteSchedule".Translate());
            }
            GUI.EndGroup();
        }

        private static void OpenSchedulePolicySelectMenu(List<ScheduleLink> links, List<Pawn> pawns)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (Policy restrictPolicy in ScheduleManager.policies)
            {
                list.Add(
                    new FloatMenuOption(restrictPolicy.label,
                        delegate
                        {
                            ScheduleManager.LoadState(links, pawns, restrictPolicy);
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void DrawBPCButtons_WorkTab(Vector2 position, float X_ExtraSpace, float Y_ExtraSpace, List<Pawn> colonists)
        {
            Rect pos = new Rect(position.x + X_ExtraSpace, position.y + Y_ExtraSpace, 600f, 65f);
            float offSetX = 0f;

            GUI.BeginGroup(pos);

            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect1 = new Rect(offSetX, -8f, 165f, Mathf.Round(pos.height / 3f));
            Widgets.Label(rect1, "BPC.CurrentWorkPolicy".Translate());

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect( offSetX, Mathf.Round(pos.height / 4f) - 4f,
                rect1.width, Mathf.Round(pos.height / 4f) + 4f);

            if (Widgets.ButtonText(
                rect2, WorkManager.GetActivePolicy().label,
                true, false, true, null))
            {
                WorkManager.SaveCurrentState(colonists);
                OpenWorkPolicySelectMenu(WorkManager.links, colonists);
            }
            offSetX += rect1.width;
            Rect rect3 = new Rect(offSetX, 0f, 20f, Mathf.Round(pos.height / 2f));
            if (Widgets.ButtonText(rect3, "", true, false, true, null))
            {
                Find.WindowStack.Add(new Dialog_ManagePolicies(Find.CurrentMap));
            }
            Rect rect4 = new Rect(offSetX + 3f, rect3.height / 4f, 14f, 14f);
            GUI.DrawTexture(rect4, Resources.Textures.Settings);
            TooltipHandler.TipRegion(rect4, "BPC.Settings".Translate());

            offSetX += rect3.width;
            Rect rect5 = new Rect(offSetX + 3f, rect3.height / 4f - 6f, 21f, 28f);
            if (Widgets.ButtonImage(rect5, ContentFinder<Texture2D>.Get("UI/Buttons/Copy", true)))
            {
                WorkManager.CopyToClipboard();
                SoundDefOf.Tick_High.PlayOneShotOnCamera(null);
                WorkManager.showPaste = true;
            }
            TooltipHandler.TipRegion(rect5, "BPC.Copy".Translate());

            if (WorkManager.showPaste)
            {
                offSetX += rect3.width;
                Rect rect6 = new Rect(offSetX + 3f, rect3.height / 4f - 6f, 21f, 28f);
                if (Widgets.ButtonImage(rect6, ContentFinder<Texture2D>.Get("UI/Buttons/Paste", true)))
                {
                    WorkManager.PasteToActivePolicy();
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera(null);
                }
                TooltipHandler.TipRegion(rect6, "BPC.Paste".Translate());
            }            
            GUI.EndGroup();
        }

        internal static void OpenWorkPolicySelectMenu(
            List<WorkLink> links, List<Pawn> pawns)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (Policy workPolicy in WorkManager.policies)
            {
                list.Add(
                    new FloatMenuOption(workPolicy.label,
                        delegate
                        {
                            WorkManager.LoadState(links, pawns, workPolicy);
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }

        private static void DrawBPCButtons_AnimalTab(Vector2 position, float X_ExtraSpace, float Y_ExtraSpace, List<Pawn> animals)
        {
            Rect pos = new Rect(position.x + X_ExtraSpace, position.y + Y_ExtraSpace, 600f, 65f);
            float offSetX = 0f;

            GUI.BeginGroup(pos);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect1 = new Rect(offSetX, -8f, 165f, Mathf.Round(pos.height / 3f));
            Widgets.Label(rect1, "BPC.CurrentAnimalPolicy".Translate());

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = new Rect(offSetX, Mathf.Round(pos.height / 4f) - 4f, rect1.width, Mathf.Round(pos.height / 4f) + 4f);

            if (Widgets.ButtonText(rect2, AnimalManager.GetActivePolicy().label, true, false, true, null))
            {
                AnimalManager.SaveCurrentState(AnimalManager.Animals().ToList());
                OpenAnimalPolicySelectMenu(AnimalManager.links, AnimalManager.Animals().ToList());
            }
            offSetX += rect1.width;
            Rect rect3 = new Rect(offSetX, 0f, 20f, Mathf.Round(pos.height / 2f));

            if (Widgets.ButtonText(rect3, "", true, false, true, null))
            {
                Find.WindowStack.Add( new Dialog_ManagePolicies(Find.CurrentMap));
            }
            Rect rect4 = new Rect(offSetX + 3f, rect3.height / 4f, 14f, 14f);
            GUI.DrawTexture(rect4, Resources.Textures.Settings);
            TooltipHandler.TipRegion(rect4, "BPC.Settings".Translate());

            GUI.EndGroup();
        }

        private static void OpenAnimalPolicySelectMenu(
            List<AnimalLink> links, List<Pawn> pawns)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (Policy animalPolicy in AnimalManager.policies)
            {
                list.Add(
                    new FloatMenuOption(animalPolicy.label,
                        delegate
                        {
                            AnimalManager.LoadState( links, pawns, animalPolicy);
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}
