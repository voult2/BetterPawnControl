using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BetterPawnControl
{
    abstract class Manager<T>
	{
		internal static List<Policy> policies = new List<Policy>();
		internal static List<MapActivePolicy> activePolicies =
			new List<MapActivePolicy>();
		internal static List<T> links = new List<T>();
		internal static bool showPaste = false;
        internal static Dictionary<WorkTypeDef, List<WorkGiverDef>> workgivers = new Dictionary<WorkTypeDef, List<WorkGiverDef>>();
        
		static Manager()
        {
            Policy defaultPolicy = new Policy(policies.Count, "BPC.Auto".Translate());
            policies.Add(defaultPolicy);
            activePolicies.Add(new MapActivePolicy(0, defaultPolicy));
        }

        internal static void ForceInit()
        {
            policies = new List<Policy>();
            activePolicies = new List<MapActivePolicy>();
            links = new List<T>();
            Policy defaultPolicy = new Policy(policies.Count, "BPC.Auto".Translate());
            policies.Add(defaultPolicy);
            activePolicies.Add(new MapActivePolicy(0, defaultPolicy));
        }

		internal static IEnumerable<Pawn> Colonists()
		{
			try
			{
                return from p in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer) where p.IsColonist select p;
			}
			catch (Exception) 
			{ 
				return new List<Pawn>(); 
			}
        }

        internal static List<WorkGiverDef> GetWorkGivers(WorkTypeDef workType)
        {
            if (workgivers.TryGetValue(workType, out var result))
                return result;

            var list = DefDatabase<WorkGiverDef>.AllDefsListForReading
                .Where(x => x.workType == workType)
                .ToList();
            workgivers.Add(workType, list);
            
			return list;
        }

        private static bool _dirtyPolicy = false;
		public static bool DirtyPolicy
		{
			get
			{
				return _dirtyPolicy;
			}

			set
			{
				_dirtyPolicy = value;
			}
		}

		internal static Policy GetActivePolicy()
		{
			return GetActivePolicy(Find.CurrentMap.uniqueID);			
		}

		internal static void SetActivePolicy(Policy policy)
		{
			SetActivePolicy(Find.CurrentMap.uniqueID, policy);
		}

		internal static Policy GetActivePolicy(int mapId)
		{
			MapActivePolicy mapPolicy = activePolicies.Find(x => x.mapId == mapId);
			if (mapPolicy == null)
			{
				//new map!create default
				mapPolicy = new MapActivePolicy(mapId, policies[0]);
				activePolicies.Add(mapPolicy);
			}
			return mapPolicy.activePolicy;
		}

		internal static Policy GetPolicy(int selected)
		{
			return policies.Find(x => x.id == selected);
		}

		internal static void SetActivePolicy(int mapId, Policy policy)
		{
			MapActivePolicy map = activePolicies.Find(x => x.mapId == mapId);
			if (map != null)
			{
				map.activePolicy = policy;
			}
			else
			{
				activePolicies.Add(new MapActivePolicy(mapId, policy));
			}
		}

		internal static void MoveLinksToMap(int dstMap)
		{
			foreach (T link in links)
			{
				if (link.GetType() == typeof(WorkLink))
				{
					link.ChangeType<WorkLink>().mapId = dstMap;
				}

				if (link.GetType() == typeof(ScheduleLink))
				{
					link.ChangeType<ScheduleLink>().mapId = dstMap;
				}

				if (link.GetType() == typeof(AssignLink))
				{
					link.ChangeType<AssignLink>().mapId = dstMap;
				}

				if (link.GetType() == typeof(AnimalLink))
				{
					link.ChangeType<AnimalLink>().mapId = dstMap;
				}

				if (link.GetType() == typeof(MechLink))
				{
					link.ChangeType<MechLink>().mapId = dstMap;
				}
			}
		}

		internal static bool FoodPolicyExits(FoodRestriction foodPolicy)
		{
			foreach (FoodRestriction food in Current.Game.foodRestrictionDatabase.AllFoodRestrictions)
			{
				if (food.Equals(foodPolicy))
				{
					return true;
				}
			}
			return false;
		}

		internal static FoodRestriction _defaultFoodPolicy = null;
		internal static FoodRestriction DefaultFoodPolicy
		{
			get
			{
				if (_defaultFoodPolicy == null)
				{
					_defaultFoodPolicy = Current.Game.foodRestrictionDatabase.DefaultFoodRestriction();
				}
				return _defaultFoodPolicy;
			}

			set
			{
				_defaultFoodPolicy = value;
			}
		}
	}
}

