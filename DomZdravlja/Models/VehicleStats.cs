namespace DomZdravlja.Models;

public class FleetStatsSummary
{
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
