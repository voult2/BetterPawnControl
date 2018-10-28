using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BetterPawnControl
{
    public class DefaultOutfitAndFoodAndDrug : MapComponent
    {
        public DefaultOutfitAndFoodAndDrug(Map map) : base(map) { }

        public override void MapComponentTick()
        {
            if (Current.Game.tickManager.TicksGame % 1800 == 0 &&
                !Find.WindowStack.IsOpen<MainTabWindow_Assign_Policies>())
            {
                //Log.Message("[BPC] Tick! 1800");
                //check if a new pawn has joined the player colony
                IEnumerable<Pawn> Pawns = 
                    Find.CurrentMap.mapPawns.FreeColonistsAndPrisoners;
                foreach (Pawn p in Pawns)
                {
                    if (p.IsColonist && !AssignManager.links.Exists(x => x.colonist == p))
                    {
                        //not found so set an outfit and drug
                        p.outfits.CurrentOutfit = AssignManager.DefaultOutfit;
                        p.drugs.CurrentPolicy = AssignManager.DefaultDrugPolicy;
                        p.foodRestriction.CurrentFoodRestriction = AssignManager.DefaultFoodPolicy;
                    }

                    if (p.IsColonist && AssignManager.Prisioners.Exists(x => x == p.GetUniqueLoadID()))
                    {
                        //found but was prisioner
                        AssignManager.Prisioners.Remove(p.GetUniqueLoadID());
                    }
                    
                    if (p.IsPrisoner && !AssignManager.Prisioners.Exists(x => x == p.GetUniqueLoadID())) 
                    {
                        p.foodRestriction.CurrentFoodRestriction = AssignManager.DefaultPrisonerFoodPolicy;
                        AssignManager.Prisioners.Add(p.GetUniqueLoadID());
                    }
                }
            }
        }
    }
}