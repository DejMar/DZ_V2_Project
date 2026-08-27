using DomZdravlja.Data;
using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Services;

public class FuelFillService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public FuelFillService(IDbContextFactory<AppDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<List<FuelFill>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FuelFills.AsNoTracking()
            .OrderByDescending(f => f.FilledAt)
            .ToListAsync();
    }

    public async Task<List<FuelFill>> GetByDriverIdAsync(int driverId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FuelFills.AsNoTracking()
            .Where(f => f.DriverId == driverId)
            .OrderByDescending(f => f.FilledAt)
            .ToListAsync();
    }

    public async Task<List<FuelFill>> GetByVehicleIdAsync(int vehicleId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FuelFills.AsNoTracking()
            .Where(f => f.VehicleId == vehicleId)
            .OrderByDescending(f => f.FilledAt)
            .ToListAsync();
    }

    public async Task<string?> RecordFillAsync(FuelFill fill)
    {
        if (fill.Liters <= 0)
            return "Količina goriva mora biti veća od nule.";

        await using var context = await _contextFactory.CreateDbContextAsync();
        var vehicle = await context.Vehicles.FindAsync(fill.VehicleId);
        if (vehicle is null || !vehicle.IsActive)
            return "Vozilo nije pronađeno ili nije aktivno.";

        if (vehicle.AssignedDriverId != fill.DriverId)
            return "Vozilo nije dodijeljeno ovom vozaču.";

        if (fill.Mileage < vehicle.CurrentMileage)
            return $"Kilometraža točenja ne može biti manja od trenutne ({vehicle.CurrentMileage} km).";

        if (vehicle.TankCapacityLiters > 0 && fill.Liters > vehicle.TankCapacityLiters * 1.2m)
            return $"Količina prelazi kapacitet rezervoara ({vehicle.TankCapacityLiters} L).";

        fill.FilledAt = DateTime.Now;
        context.FuelFills.Add(fill);

        if (fill.Mileage > vehicle.CurrentMileage)
            vehicle.CurrentMileage = fill.Mileage;

        await context.SaveChangesAsync();
        return null;
    }
}
