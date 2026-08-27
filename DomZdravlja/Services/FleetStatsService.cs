using DomZdravlja.Data;
using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Services;

public class FleetStatsService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public FleetStatsService(IDbContextFactory<AppDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<FleetStatsSummary> GetSummaryAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var vehicles = await context.Vehicles.AsNoTracking().ToListAsync();
        var trips = await context.VehicleTrips.AsNoTracking().ToListAsync();
        var fills = await context.FuelFills.AsNoTracking().ToListAsync();
        var drivers = await context.Users.AsNoTracking()
            .Where(u => u.Role == UserRole.Vozac)
            .ToListAsync();

        var driverNames = drivers.ToDictionary(d => d.Id, d => d.FullName);

        var vehicleRows = vehicles.Select(v =>
        {
            var vehicleTrips = trips.Where(t => t.VehicleId == v.Id).ToList();
            var closedTrips = vehicleTrips.Where(t => t.EndMileage.HasValue).ToList();
            var vehicleFills = fills.Where(f => f.VehicleId == v.Id).ToList();
            var distance = closedTrips.Sum(t => t.EndMileage!.Value - t.StartMileage);
            var liters = vehicleFills.Sum(f => f.Liters);

            return new VehicleStatRow
            {
                VehicleId = v.Id,
                DisplayName = v.DisplayName,
                PlateNumber = v.PlateNumber,
                DriverName = v.AssignedDriverId is int id && driverNames.TryGetValue(id, out var name) ? name : null,
                CurrentMileage = v.CurrentMileage,
                TripCount = vehicleTrips.Count,
                TotalDistanceKm = distance,
                FuelFillCount = vehicleFills.Count,
                TotalFuelLiters = liters,
                TotalFuelCost = vehicleFills.Where(f => f.Cost.HasValue).Sum(f => f.Cost!.Value),
                AvgConsumptionLPer100Km = distance > 0 ? Math.Round(liters / distance * 100m, 2) : null,
                HasOpenTrip = vehicleTrips.Any(t => t.EndMileage is null),
                IsActive = v.IsActive
            };
        }).OrderBy(v => v.PlateNumber).ToList();

        var driverRows = drivers.Select(d =>
        {
            var driverTrips = trips.Where(t => t.DriverId == d.Id).ToList();
            var closedTrips = driverTrips.Where(t => t.EndMileage.HasValue).ToList();
            var driverFills = fills.Where(f => f.DriverId == d.Id).ToList();

            return new DriverStatRow
            {
                DriverId = d.Id,
                DriverName = d.FullName,
                AssignedVehicleCount = vehicles.Count(v => v.AssignedDriverId == d.Id),
                TripCount = driverTrips.Count,
                TotalDistanceKm = closedTrips.Sum(t => t.EndMileage!.Value - t.StartMileage),
                FuelFillCount = driverFills.Count,
                TotalFuelLiters = driverFills.Sum(f => f.Liters),
                TotalFuelCost = driverFills.Where(f => f.Cost.HasValue).Sum(f => f.Cost!.Value),
                OpenTripCount = driverTrips.Count(t => t.EndMileage is null)
            };
        }).OrderBy(d => d.DriverName).ToList();

        return new FleetStatsSummary
        {
            TotalVehicles = vehicles.Count,
            ActiveVehicles = vehicles.Count(v => v.IsActive),
            AssignedVehicles = vehicles.Count(v => v.AssignedDriverId.HasValue),
            TotalDrivers = drivers.Count,
            OpenTrips = trips.Count(t => t.EndMileage is null),
            TotalDistanceKm = vehicleRows.Sum(v => v.TotalDistanceKm),
            TotalFuelLiters = vehicleRows.Sum(v => v.TotalFuelLiters),
            TotalFuelCost = vehicleRows.Sum(v => v.TotalFuelCost),
            Vehicles = vehicleRows,
            Drivers = driverRows
        };
    }
}
