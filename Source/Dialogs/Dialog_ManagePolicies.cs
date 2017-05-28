using System.Text.RegularExpressions;
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
		private static Regex validNameRegex = new Regex("^[a-zA-Z0-9 '\\-]*$");
        private const int MAX_POLICIES = 20;

        /// <summary>
        /// Copy paste from vanilla
        /// </summary>
		public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(700f, 710f);
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

            int rows = (AnimalManager.policies.Count > AssignManager.policies.Count) ?
                         AnimalManager.policies.Count : AssignManager.policies.Count;

            for (int i = 0; i < rows; i++)
            {

                Policy animalP = (i < AnimalManager.policies.Count) ? AnimalManager.policies[i] : null;
                Policy assignP = (i < AssignManager.policies.Count) ? AssignManager.policies[i] : null;

                Rect rect = listing_Standard.GetRect(24f);
                DoRow(rect, animalP, assignP);
                listing_Standard.Gap(6f);
            }

            Rect rect2 = listing_Standard.GetRect(24f);
            DoBottonRow(rect2);
            listing_Standard.End();
        }

        /// <summary>
        /// Draw table row with AnimalPolicy label, rename button and delete button
        /// </summary>
        private static void DoRow(Rect rect, Policy animalP, Policy assignP)
        {
            //Animal column
            Rect rect2 = new Rect(rect.x, rect.y, rect.width / 2f, rect.height);
            GUI.BeginGroup(rect2);
            WidgetRow widgetRow = new WidgetRow(0f, 0f, UIDirection.RightThenUp, 99999f, 4f);
            widgetRow.Gap(4f);
            if (animalP != null)
            {
                widgetRow.Label(animalP.label, 158f);
                if (widgetRow.ButtonText("BPC.Rename".Translate(), null, true, false))
                {
                    Find.WindowStack.Add(new Dialog_RenamePolicy(animalP, Resources.Type.animal));
                }
                if (animalP.id > 0 && widgetRow.ButtonIcon(ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true), null))
                {

                    AnimalManager.DeletePolicy(animalP);
                }
            }
            GUI.EndGroup();

            //Assign column
            Rect rect3 = new Rect(rect2.width, rect.y, rect.width / 2f, rect.height);
            GUI.BeginGroup(rect3);
            WidgetRow widgetRow2 = new WidgetRow(0f, 0f, UIDirection.RightThenUp, 99999f, 4f);
            widgetRow2.Gap(4f);
            if (assignP != null)
            {
                widgetRow2.Label(assignP.label, 158f);
                if (widgetRow2.ButtonText("BPC.Rename".Translate(), null, true, false))
                {
                    Find.WindowStack.Add(new Dialog_RenamePolicy(assignP, Resources.Type.assign));
                }

                if (assignP.id > 0 && widgetRow2.ButtonIcon(ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true), null))
                {
                    AssignManager.DeletePolicy(assignP);
                }
            }
            GUI.EndGroup();
        }

        private static void DoBottonRow(Rect rect)
        {

            float half = rect.width / 2f;
            float twoThirds = 2f * half / 3f + 28f;
            float offset = 0;

            Rect rect2 = new Rect(offset, rect.y, twoThirds, rect.height + 6f);
            Rect rect3 = new Rect(half + offset, rect.y, twoThirds, rect.height + 6f);

            if (AnimalManager.policies.Count < MAX_POLICIES && Widgets.ButtonText(rect2, "BPC.NewAnimalPolicy".Translate(), true, false, true))
            {
                int lastItem = AnimalManager.policies.Count - 1;
                int label_id = AnimalManager.policies[lastItem].id;
                label_id++;
                AnimalManager.policies.Add(new Policy(label_id, "BPC.AnimalPolicy".Translate() + label_id));
            }

            if (AssignManager.policies.Count < MAX_POLICIES && Widgets.ButtonText(rect3, "BPC.NewAssignPolicy".Translate(), true, false, true))
            {
                int lastItem = AssignManager.policies.Count - 1;
                int label_id = AssignManager.policies[lastItem].id;
                label_id++;
                AssignManager.policies.Add(new Policy(label_id, "BPC.AssignPolicy".Translate() + label_id));
            }
        }
    }
}
