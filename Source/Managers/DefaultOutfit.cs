using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetterPawnControl
{
    public class DefaultOutfit : MapComponent
    {

        public DefaultOutfit(Map map) : base(map) { }

        public override void MapComponentTick()
        {

            if (Current.Game.tickManager.TicksGame % 1800 == 0 &&
                !Find.WindowStack.IsOpen<MainTabWindow_Assign_Policies>())
            {
                //Log.Message("[BPC] Tick! 600");
                //check if a new pawn has joined the player colonies
                IEnumerable<Pawn> Pawns = 
                    Find.VisibleMap.mapPawns.FreeColonists;
                foreach (Pawn p in Pawns)
                {
                    if (!AssignManager.links.Exists(x => x.colonist == p))
                    {
                        //not found so set an outfit
                        p.outfits.CurrentOutfit = AssignManager.DefaultOutfit;
                    }
                }

            }
        }
    }
}