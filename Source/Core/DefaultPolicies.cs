using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace BetterPawnControl
{
    public class DefaultPolicies : MapComponent
    {
        public DefaultPolicies(Map map) : base(map) { }

        public override void MapComponentTick()
        {
            if (Current.Game.tickManager.TicksGame % 1800 == 0 && !Find.WindowStack.IsOpen<MainTabWindow_Assign>())
            {
                //check if a new pawn has joined the player colony
                IEnumerable<Pawn> pawns = Find.CurrentMap.mapPawns.FreeColonistsAndPrisoners;
                foreach (Pawn p in pawns)
                {
                    if (!p.IsPrisoner && !p.IsSlave && !AssignManager.links.Exists(x => x.colonist == p))
                    {
                        //new colonist so set an outfit and drug and food and meds
                        p.outfits.CurrentOutfit = AssignManager.DefaultOutfit;
                        p.drugs.CurrentPolicy = AssignManager.DefaultDrugPolicy;
                        p.foodRestriction.CurrentFoodRestriction = AssignManager.DefaultFoodPolicy;
                        p.playerSettings.medCare = AssignManager.DefaultMedCare;
                        AssignManager.SaveCurrentState(new List<Pawn>() {p});
                    }

                    if (!p.IsPrisoner && AssignManager.prisoners.Exists(x => x == p.GetUniqueLoadID()))
                    {
                        //found but was former prisioner
                        AssignManager.prisoners.Remove(p.GetUniqueLoadID());

                        if (AssignManager.links.Exists(x => x.colonist == p))
                        {
                            //and is now a free colonist so set back defaults
                            p.outfits.CurrentOutfit = AssignManager.DefaultOutfit;
                            p.drugs.CurrentPolicy = AssignManager.DefaultDrugPolicy;
                            p.foodRestriction.CurrentFoodRestriction = AssignManager.DefaultFoodPolicy;
                            p.playerSettings.medCare = AssignManager.DefaultMedCare;
                        }
                    }
                    
                    if (p.IsPrisoner && !AssignManager.prisoners.Exists(x => x == p.GetUniqueLoadID()))
                    {
                        //new prisioner                        
                        p.foodRestriction.CurrentFoodRestriction = AssignManager.DefaultPrisonerFoodPolicy;
                        p.playerSettings.medCare = AssignManager.DefaultPrisonerMedCare;
                        AssignManager.prisoners.Add(p.GetUniqueLoadID());
                                                                     
                        if( ModsConfig.IdeologyActive && AssignManager.slaves.Exists(x => x == p.GetUniqueLoadID()))
                        {
                            //former slave
                            AssignManager.slaves.Remove(p.GetUniqueLoadID());
                        }
                        //if free colonist becomes prisoner and rejoins the colony, his policies will be recovered
                    }

                    if (ModsConfig.IdeologyActive)
                    {
                        if (p.IsSlave && !AssignManager.slaves.Exists(x => x == p.GetUniqueLoadID()))
                        {
                            //new slave
                            p.outfits.CurrentOutfit = AssignManager.DefaultSlaveOutfit;
                            p.drugs.CurrentPolicy = AssignManager.DefaultSlaveDrugPolicy;
                            p.foodRestriction.CurrentFoodRestriction = AssignManager.DefaultSlaveFoodPolicy;
                            p.playerSettings.medCare = AssignManager.DefaultSlaveMedCare;
                            AssignManager.slaves.Add(p.GetUniqueLoadID());
                            //if former colonist becomes prisoner, if he rejoins the colony, his policies will be recovered
                        }

                        if (!p.IsSlave && AssignManager.slaves.Exists(x => x == p.GetUniqueLoadID()))
                        {
                            //found but was slave
                            AssignManager.slaves.Remove(p.GetUniqueLoadID());

                            if (AssignManager.links.Exists(x => x.colonist == p))
                            {
                                //and is now a colonist so set back default defaults
                                p.outfits.CurrentOutfit = AssignManager.DefaultOutfit;
                                p.drugs.CurrentPolicy = AssignManager.DefaultDrugPolicy;
                                p.foodRestriction.CurrentFoodRestriction = AssignManager.DefaultFoodPolicy;
                                p.playerSettings.medCare = AssignManager.DefaultMedCare;
                            }
                        }

                        if (!p.IsSlave && AssignManager.slaves.Exists(x => x == p.GetUniqueLoadID()))
                        {
                            //found former slave
                            AssignManager.slaves.Remove(p.GetUniqueLoadID());
                        }
                    }

                    //List<Pawn> animals = Find.CurrentMap.mapPawns.SpawnedColonyAnimals;
                    //foreach (Pawn p in animals)
                    //{
                    //    Log.Message("IsAnimal");
                    //    if (!AnimalManager.links.Exists(x => x.animal == p))
                    //    {
                    //        p.playerSettings.medCare = AssignManager.DefaultAnimalMedCare;
                    //    }
                    //}
                }
            }
        }
    }
}