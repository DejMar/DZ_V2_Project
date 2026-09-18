namespace DomZdravlja.Models;

public enum FleetStatsPeriod
{
    All,
    Year,
    Month
}

public class FleetStatsFilter
{
    public FleetStatsPeriod Period { get; set; } = FleetStatsPeriod.All;
    public int? Year { get; set; }
    public int? Month { get; set; }
}

public class FleetStatsSummary
{
    public string PeriodLabel { get; set; } = "Ukupno";
    public FleetStatsPeriod Period { get; set; } = FleetStatsPeriod.All;
    public int? Year { get; set; }
    public int? Month { get; set; }
    public int TotalVehicles { get; set; }
    public int ActiveVehicles { get; set; }
    public int AssignedVehicles { get; set; }
    public int TotalDrivers { get; set; }
    public int OpenTrips { get; set; }
    public int TotalDistanceKm { get; set; }
    public decimal TotalFuelLiters { get; set; }
    public decimal TotalFuelCost { get; set; }
    public List<VehicleStatRow> Vehicles { get; set; } = [];
    public List<DriverStatRow> Drivers { get; set; } = [];
    public List<PeriodStatRow> PeriodBreakdown { get; set; } = [];
    public List<int> AvailableYears { get; set; } = [];
}

public class PeriodStatRow
{
    public string Label { get; set; } = string.Empty;
    public int Year { get; set; }
    public int? Month { get; set; }
    public int TripCount { get; set; }
    public int TotalDistanceKm { get; set; }
    public int FuelFillCount { get; set; }
    public decimal TotalFuelLiters { get; set; }
    public decimal TotalFuelCost { get; set; }
    public decimal? AvgConsumptionLPer100Km { get; set; }
}

public class VehicleStatRow
{
    public int VehicleId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public string? DriverName { get; set; }
    public int CurrentMileage { get; set; }
    public int TripCount { get; set; }
    public int TotalDistanceKm { get; set; }
    public int FuelFillCount { get; set; }
    public decimal TotalFuelLiters { get; set; }
    public decimal TotalFuelCost { get; set; }
    public decimal? AvgConsumptionLPer100Km { get; set; }
    public bool HasOpenTrip { get; set; }
    public bool IsActive { get; set; }
}

public class DriverStatRow
{
    public int DriverId { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public int AssignedVehicleCount { get; set; }
    public int TripCount { get; set; }
    public int TotalDistanceKm { get; set; }
    public int FuelFillCount { get; set; }
    public decimal TotalFuelLiters { get; set; }
    public decimal TotalFuelCost { get; set; }
    public int OpenTripCount { get; set; }
}
