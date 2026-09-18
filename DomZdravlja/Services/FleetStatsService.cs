using DomZdravlja.Data;
using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Services;

public class FleetStatsService
{
    private static readonly string[] MonthNames =
    [
        "", "Januar", "Februar", "Mart", "April", "Maj", "Juni",
        "Juli", "August", "Septembar", "Oktobar", "Novembar", "Decembar"
    ];

    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public FleetStatsService(IDbContextFactory<AppDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<FleetStatsSummary> GetSummaryAsync(FleetStatsFilter? filter = null)
    {
        filter ??= new FleetStatsFilter();

        await using var context = await _contextFactory.CreateDbContextAsync();

        var vehicles = await context.Vehicles.AsNoTracking().ToListAsync();
        var allTrips = await context.VehicleTrips.AsNoTracking().ToListAsync();
        var allFills = await context.FuelFills.AsNoTracking().ToListAsync();
        var drivers = await context.Users.AsNoTracking()
            .Where(u => u.Role == UserRole.Vozac)
            .ToListAsync();

        var availableYears = allTrips.Select(t => t.StartedAt.Year)
            .Concat(allFills.Select(f => f.FilledAt.Year))
            .Distinct()
            .OrderByDescending(y => y)
            .ToList();

        if (availableYears.Count == 0)
            availableYears.Add(DateTime.Today.Year);

        NormalizeFilter(filter, availableYears);

        var trips = FilterTrips(allTrips, filter);
        var fills = FilterFills(allFills, filter);
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
            PeriodLabel = BuildPeriodLabel(filter),
            Period = filter.Period,
            Year = filter.Year,
            Month = filter.Month,
            TotalVehicles = vehicles.Count,
            ActiveVehicles = vehicles.Count(v => v.IsActive),
            AssignedVehicles = vehicles.Count(v => v.AssignedDriverId.HasValue),
            TotalDrivers = drivers.Count,
            OpenTrips = trips.Count(t => t.EndMileage is null),
            TotalDistanceKm = vehicleRows.Sum(v => v.TotalDistanceKm),
            TotalFuelLiters = vehicleRows.Sum(v => v.TotalFuelLiters),
            TotalFuelCost = vehicleRows.Sum(v => v.TotalFuelCost),
            Vehicles = vehicleRows,
            Drivers = driverRows,
            PeriodBreakdown = BuildPeriodBreakdown(allTrips, allFills, filter),
            AvailableYears = availableYears
        };
    }

    private static void NormalizeFilter(FleetStatsFilter filter, List<int> availableYears)
    {
        if (filter.Period == FleetStatsPeriod.All)
        {
            filter.Year = null;
            filter.Month = null;
            return;
        }

        filter.Year ??= availableYears.FirstOrDefault(DateTime.Today.Year);

        if (filter.Period == FleetStatsPeriod.Month)
            filter.Month = Math.Clamp(filter.Month ?? DateTime.Today.Month, 1, 12);
        else
            filter.Month = null;
    }

    private static List<VehicleTrip> FilterTrips(List<VehicleTrip> trips, FleetStatsFilter filter) =>
        filter.Period switch
        {
            FleetStatsPeriod.Year => trips.Where(t => t.StartedAt.Year == filter.Year).ToList(),
            FleetStatsPeriod.Month => trips.Where(t =>
                t.StartedAt.Year == filter.Year && t.StartedAt.Month == filter.Month).ToList(),
            _ => trips
        };

    private static List<FuelFill> FilterFills(List<FuelFill> fills, FleetStatsFilter filter) =>
        filter.Period switch
        {
            FleetStatsPeriod.Year => fills.Where(f => f.FilledAt.Year == filter.Year).ToList(),
            FleetStatsPeriod.Month => fills.Where(f =>
                f.FilledAt.Year == filter.Year && f.FilledAt.Month == filter.Month).ToList(),
            _ => fills
        };

    private static string BuildPeriodLabel(FleetStatsFilter filter) =>
        filter.Period switch
        {
            FleetStatsPeriod.Year => $"Godina {filter.Year}",
            FleetStatsPeriod.Month => $"{MonthNames[filter.Month ?? 1]} {filter.Year}",
            _ => "Ukupno (svi periodi)"
        };

    private static List<PeriodStatRow> BuildPeriodBreakdown(
        List<VehicleTrip> allTrips,
        List<FuelFill> allFills,
        FleetStatsFilter filter)
    {
        if (filter.Period == FleetStatsPeriod.Month)
            return [];

        if (filter.Period == FleetStatsPeriod.All)
        {
            var years = allTrips.Select(t => t.StartedAt.Year)
                .Concat(allFills.Select(f => f.FilledAt.Year))
                .Distinct()
                .OrderByDescending(y => y);

            return years.Select(year => BuildPeriodRow(
                allTrips.Where(t => t.StartedAt.Year == year),
                allFills.Where(f => f.FilledAt.Year == year),
                year.ToString(),
                year,
                null)).ToList();
        }

        // Year view → monthly breakdown for selected year
        var yearValue = filter.Year ?? DateTime.Today.Year;
        return Enumerable.Range(1, 12)
            .Select(month => BuildPeriodRow(
                allTrips.Where(t => t.StartedAt.Year == yearValue && t.StartedAt.Month == month),
                allFills.Where(f => f.FilledAt.Year == yearValue && f.FilledAt.Month == month),
                $"{MonthNames[month]} {yearValue}",
                yearValue,
                month))
            .Where(row => row.TripCount > 0 || row.FuelFillCount > 0)
            .ToList();
    }

    private static PeriodStatRow BuildPeriodRow(
        IEnumerable<VehicleTrip> trips,
        IEnumerable<FuelFill> fills,
        string label,
        int year,
        int? month)
    {
        var tripList = trips.ToList();
        var fillList = fills.ToList();
        var closed = tripList.Where(t => t.EndMileage.HasValue).ToList();
        var distance = closed.Sum(t => t.EndMileage!.Value - t.StartMileage);
        var liters = fillList.Sum(f => f.Liters);

        return new PeriodStatRow
        {
            Label = label,
            Year = year,
            Month = month,
            TripCount = tripList.Count,
            TotalDistanceKm = distance,
            FuelFillCount = fillList.Count,
            TotalFuelLiters = liters,
            TotalFuelCost = fillList.Where(f => f.Cost.HasValue).Sum(f => f.Cost!.Value),
            AvgConsumptionLPer100Km = distance > 0 ? Math.Round(liters / distance * 100m, 2) : null
        };
    }
}
