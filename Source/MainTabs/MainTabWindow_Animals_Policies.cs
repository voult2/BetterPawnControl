using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using System.Linq;
using System;

/// <summary>
/// Adds the ability to create profiles for the Animal Tab
/// <author>Alexandre Brito</author>
/// </summary>

namespace BetterPawnControl
{
    /// <summary>
    /// Enherit everthing from vanilla and avoid breaking it at all costs      
    /// </summary>

    public class MainTabWindow_Animals_Policies : MainTabWindow_Animals
    {

        protected override float ExtraTopSpace
        {
            get
            {
                float offsetY = 0f;
                if (Widget_AnimalTab.AnimalTabAvailable)
                {
                    offsetY = 30f+12f;
                }
                return offsetY;
            }
        }

        public override void PreOpen()
        {
            base.PreOpen();

            AnimalManager.CleanDeadMaps();

            AnimalManager.UpdateState(
                AnimalManager.links, this.Pawns.ToList(),
                AnimalManager.GetActivePolicy());

            AnimalManager.LoadState(
                AnimalManager.links, this.Pawns.ToList(), 
                AnimalManager.GetActivePolicy());

            AnimalManager.CleanDeadAnimals(this.Pawns.ToList());
        }

        public override void PreClose()
        {
            base.PreClose();
            AnimalManager.CleanDeadMaps();
            AnimalManager.CleanDeadAnimals(this.Pawns.ToList());
            AnimalManager.SaveCurrentState(this.Pawns.ToList());
        }

        /// <summary>
        /// Draw base and two new buttons!
        /// </summary>
        public override void DoWindowContents(Rect fillRect)
        {
            if (AnimalManager.DirtyPolicy)
            {
                AnimalManager.LoadState(
                    AnimalManager.links, this.Pawns.ToList(), 
                    AnimalManager.GetActivePolicy());
                AnimalManager.DirtyPolicy = false;
            }

            float offsetX = 5f;
            base.DoWindowContents(fillRect);           
            Rect position = new Rect(0f, 0f, fillRect.width, 65f);  

            GUI.BeginGroup(position);
            Text.Font = GameFont.Tiny;
            Text.Anchor = TextAnchor.LowerCenter;
            Rect rect1 = 
                new Rect(offsetX, -8f, 165f, Mathf.Round(position.height / 3f));
            Widgets.Label(rect1, "BPC.CurrentAnimalPolicy".Translate());
            GUI.EndGroup();

            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect rect2 = 
                new Rect(
                    offsetX, Mathf.Round(position.height / 4f) - 4f, 
                    rect1.width, Mathf.Round(position.height / 4f) + 4f);

            if (Widgets.ButtonText(
                rect2, AnimalManager.GetActivePolicy().label, 
                true, false, true))
            {
                AnimalManager.SaveCurrentState(this.Pawns.ToList());
                OpenAnimalPolicySelectMenu(
                    AnimalManager.links, this.Pawns.ToList());
            }
            offsetX += rect1.width;
            Rect rect3 = 
                new Rect(offsetX, 0f, 20f, Mathf.Round(position.height / 2f));

            if (Widgets.ButtonText(rect3, "", true, false, true))
            {
                Find.WindowStack.Add(
                    new Dialog_ManagePolicies(Find.CurrentMap));
            }
            Rect rect4 = new Rect(offsetX + 3f, rect3.height / 4f, 14f, 14f);
            GUI.DrawTexture(rect4, Resources.Settings);
            TooltipHandler.TipRegion(rect4, "BPC.Settings".Translate());
        }

        private static void OpenAnimalPolicySelectMenu(
            List<AnimalLink> links, List<Pawn> pawns)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            foreach (Policy animalPolicy in AnimalManager.policies)
            {
                list.Add(
                    new FloatMenuOption(
                        animalPolicy.label,
                        delegate
                        {
                            AnimalManager.LoadState(
                                links,
                                pawns,
                                animalPolicy);
                        },
                        MenuOptionPriority.Default, null, null, 0f, null));
            }
            Find.WindowStack.Add(new FloatMenu(list));
        }
    }
}