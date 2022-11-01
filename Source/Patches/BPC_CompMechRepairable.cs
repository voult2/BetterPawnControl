using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetterPawnControl.Patches
{
    public class BPC_CompMechRepairable : CompMechRepairable
    {
		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			if (parent.Faction == Faction.OfPlayer)
			{
				Command_Toggle command_Toggle = new Command_Toggle();
				command_Toggle.defaultLabel = "CommandAutoRepair".Translate();
				command_Toggle.defaultDesc = "CommandAutoRepairDesc".Translate();
				command_Toggle.icon = ContentFinder<Texture2D>.Get("UI/Gizmos/AutoRepair");
				command_Toggle.isActive = () => autoRepair;
				command_Toggle.toggleAction = (Action)Delegate.Combine(command_Toggle.toggleAction, (Action)delegate
				{
					autoRepair = !autoRepair;
					if (this.parent is Pawn pawn)
                    {
						if (pawn.IsColonyMech)
                        {
							MechManager.links.Find(x => x.Equals(pawn) && x.zone == MechManager.GetActivePolicy().id).autorepair = autoRepair;
                        }
                    }
				});
				yield return command_Toggle;
			}
		}
	}
}
