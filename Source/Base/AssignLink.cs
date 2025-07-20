using RimWorld;
using Verse;
using static BetterPawnControl.BetterPawnControlMod;

namespace BetterPawnControl
{
    public class AssignLink : Link, IExposable
    {
        //internal int zone = 0;
        internal Pawn colonist = null;
        internal ApparelPolicy outfit = null;
        internal FoodPolicy foodPolicy = null;
        internal DrugPolicy drugPolicy = null;
        internal ReadingPolicy readingPolicy = null;
        internal HostilityResponseMode hostilityResponse = HostilityResponseMode.Flee;
        internal MedicalCareCategory medicinePolicy = MedicalCareCategory.Best;
        internal ThingDef carriedMedicineThing = null;
        internal int carriedMedicineCount = 0;
        internal int loadoutId = 1;
        internal int compositableState = -1;
        //internal int mapId = 0;

        public AssignLink() { }

        public AssignLink(
            int zone, Pawn colonist, ApparelPolicy outfit, FoodPolicy foodPolicy,
            DrugPolicy drugPolicy, ReadingPolicy readingPolicy, HostilityResponseMode hostilityResponse,
            MedicalCareCategory medicinePolicy, int loadoutId, int compositableState, int mapId)
        {
            this.zone = zone;
            this.colonist = colonist;
            this.outfit = outfit;
            this.foodPolicy = foodPolicy;
            this.drugPolicy = drugPolicy;
            this.readingPolicy = readingPolicy;
            this.hostilityResponse = hostilityResponse;
            this.medicinePolicy = medicinePolicy;
            this.loadoutId = loadoutId;
            this.compositableState = compositableState;
            this.mapId = mapId;
            SetInventoryStockForMedicine(colonist.inventoryStock);
        }

        public AssignLink(AssignLink link)
        {
            this.zone = link.zone;
            this.colonist = link.colonist;
            this.outfit = link.outfit;
            this.foodPolicy = link.foodPolicy;
            this.drugPolicy = link.drugPolicy;
            this.readingPolicy = link.readingPolicy;
            this.hostilityResponse = link.hostilityResponse;
            this.medicinePolicy = link.medicinePolicy;
            this.loadoutId = link.loadoutId;
            this.compositableState = link.compositableState;
            this.carriedMedicineThing = link.carriedMedicineThing;
            this.carriedMedicineCount = link.carriedMedicineCount;
            this.mapId = link.mapId;
        }

        public override string ToString()
        {
            string outifit = outfit?.label;
            string drug = drugPolicy?.label;
            string food = foodPolicy?.label;
            string reading = readingPolicy?.label;
            return
                "PolicyID:" + zone +
                "  Pawn: " + colonist +
                "  Outfit: " + outifit +
                "  Food: " + food +
                "  DrugPolicy: " + drug +
                "  ReadingPolicy: " + reading +
                "  HostilityResponse: " + hostilityResponse +
                "  MedCare: " + medicinePolicy +
                "  CarriedMedicineThing: " + carriedMedicineThing +
                "  CarriedMedicineCount: " + carriedMedicineCount +
                "  LoadoutId: " + loadoutId +
                "  Compositable: " + compositableState +
                "  MapID: " + mapId;
        }



        /// <summary>
        /// Data for saving/loading
        /// </summary>
        public void ExposeData()
        {
            Scribe_Values.Look<int>(ref zone, "zone", 0, true);
            Scribe_References.Look<Pawn>(ref colonist, "colonist");
            Scribe_References.Look<ApparelPolicy>(ref outfit, "outfit");
            Scribe_References.Look<FoodPolicy>(ref foodPolicy, "foodPolicy");
            Scribe_References.Look<DrugPolicy>(ref drugPolicy, "drugPolicy");
            Scribe_References.Look<ReadingPolicy>(ref readingPolicy, "readingPolicy");
            Scribe_Values.Look<HostilityResponseMode>(ref hostilityResponse, "hostilityResponse", HostilityResponseMode.Flee, true);
            Scribe_Values.Look<MedicalCareCategory>(ref medicinePolicy, "medcare", MedicalCareCategory.Best, true);
            Scribe_Values.Look(ref carriedMedicineThing, "carriedMedicineThing");
            Scribe_Values.Look(ref carriedMedicineCount, "carriedMedicineCount");
            Scribe_Values.Look<int>(ref loadoutId, "loadoutId", 1, true);

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                if (loadoutId == 0)
                    this.loadoutId = 1;
            }

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                if (!Settings.saveInventoryStock)
                {
                    carriedMedicineThing = null;
                    carriedMedicineCount = 0;
                }
            }
            Scribe_Values.Look<int>(ref compositableState, "compositableState", -1, true);
            Scribe_Values.Look<int>(ref mapId, "mapId", 0, true);
        }

        public void SetInventoryStockForMedicine(Pawn_InventoryStockTracker stock)
        {
            if (!Settings.saveInventoryStock)
            {
                carriedMedicineThing = null;
                carriedMedicineCount = 0;
                return;
            }

            if (stock == null)
            {
                return;
            }

            var thing = colonist.inventoryStock.GetDesiredThingForGroup(InventoryStockGroupDefOf.Medicine);
            carriedMedicineThing = thing;
            var count = colonist.inventoryStock.GetDesiredCountForGroup(InventoryStockGroupDefOf.Medicine);
            carriedMedicineCount = count;
        }
    }
}
