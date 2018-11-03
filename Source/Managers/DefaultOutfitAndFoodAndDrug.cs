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
                    if (!p.IsPrisoner && !AssignManager.links.Exists(x => x.colonist == p))
                    {
                        //not found so set an outfit and drug and food
                        p.outfits.CurrentOutfit = AssignManager.DefaultOutfit;
                        p.drugs.CurrentPolicy = AssignManager.DefaultDrugPolicy;
                        p.foodRestriction.CurrentFoodRestriction = AssignManager.DefaultFoodPolicy;
                    }

                    if (!p.IsPrisoner && AssignManager.Prisoners.Exists(x => x == p.GetUniqueLoadID()))
                    {
                        //found but was prisioner
                        AssignManager.Prisoners.Remove(p.GetUniqueLoadID());
                        
                        //and is was a colonist so set back default food 
                        if (AssignManager.links.Exists(x => x.colonist == p))
                        {
                            p.foodRestriction.CurrentFoodRestriction = AssignManager.DefaultFoodPolicy;
                        }                        
                    }
                    
                    if (p.IsPrisoner && !AssignManager.Prisoners.Exists(x => x == p.GetUniqueLoadID())) 
                    {
                        p.foodRestriction.CurrentFoodRestriction = AssignManager.DefaultPrisonerFoodPolicy;
                        AssignManager.Prisoners.Add(p.GetUniqueLoadID());
                    }
                }
            }
        }
    }
}