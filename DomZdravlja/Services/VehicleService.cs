using DomZdravlja.Data;
using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Services;

public class VehicleService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public VehicleService(IDbContextFactory<AppDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<List<Vehicle>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Vehicles.AsNoTracking()
            .OrderBy(v => v.PlateNumber)
            .ToListAsync();
    }

    public async Task<List<Vehicle>> GetByDriverIdAsync(int driverId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Vehicles.AsNoTracking()
            .Where(v => v.AssignedDriverId == driverId && v.IsActive)
            .OrderBy(v => v.PlateNumber)
            .ToListAsync();
    }

    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<bool> PlateExistsAsync(string plateNumber, int? excludeId = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var plate = plateNumber.Trim().ToUpperInvariant();
        var query = context.Vehicles.AsNoTracking()
            .Where(v => v.PlateNumber.ToUpper() == plate);

        if (excludeId.HasValue)
            query = query.Where(v => v.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        vehicle.PlateNumber = vehicle.PlateNumber.Trim().ToUpperInvariant();
        vehicle.IsActive = true;
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        vehicle.PlateNumber = vehicle.PlateNumber.Trim().ToUpperInvariant();
        context.Vehicles.Update(vehicle);
        await context.SaveChangesAsync();
    }

    public async Task SetActiveAsync(int id, bool isActive)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var vehicle = await context.Vehicles.FindAsync(id);
        if (vehicle is null)
            return;

        vehicle.IsActive = isActive;
        await context.SaveChangesAsync();
    }

    public async Task UpdateMileageAsync(int vehicleId, int mileage)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var vehicle = await context.Vehicles.FindAsync(vehicleId);
        if (vehicle is null)
            return;

        if (mileage > vehicle.CurrentMileage)
            vehicle.CurrentMileage = mileage;

        await context.SaveChangesAsync();
    }
}
