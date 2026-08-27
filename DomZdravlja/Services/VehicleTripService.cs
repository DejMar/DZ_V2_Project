using DomZdravlja.Data;
using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Services;

public class VehicleTripService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public VehicleTripService(IDbContextFactory<AppDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<List<VehicleTrip>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.VehicleTrips.AsNoTracking()
            .OrderByDescending(t => t.StartedAt)
            .ToListAsync();
    }

    public async Task<List<VehicleTrip>> GetByDriverIdAsync(int driverId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.VehicleTrips.AsNoTracking()
            .Where(t => t.DriverId == driverId)
            .OrderByDescending(t => t.StartedAt)
            .ToListAsync();
    }

    public async Task<VehicleTrip?> GetOpenTripForDriverAsync(int driverId)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.VehicleTrips.AsNoTracking()
            .Where(t => t.DriverId == driverId && t.EndMileage == null)
            .OrderByDescending(t => t.StartedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> StartTripAsync(VehicleTrip trip)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        var vehicle = await context.Vehicles.FindAsync(trip.VehicleId);
        if (vehicle is null || !vehicle.IsActive)
            return "Vozilo nije pronađeno ili nije aktivno.";

        if (vehicle.AssignedDriverId != trip.DriverId)
            return "Vozilo nije dodijeljeno ovom vozaču.";

        var hasOpen = await context.VehicleTrips.AnyAsync(t =>
            t.DriverId == trip.DriverId && t.EndMileage == null);
        if (hasOpen)
            return "Već imate otvorenu vožnju. Prvo je zatvorite.";

        if (trip.StartMileage < vehicle.CurrentMileage)
            return $"Početna kilometraža ne može biti manja od trenutne ({vehicle.CurrentMileage} km).";

        trip.StartedAt = DateTime.Now;
        trip.EndMileage = null;
        trip.EndedAt = null;
        context.VehicleTrips.Add(trip);

        if (trip.StartMileage > vehicle.CurrentMileage)
            vehicle.CurrentMileage = trip.StartMileage;

        await context.SaveChangesAsync();
        return null;
    }

    public async Task<string?> EndTripAsync(int tripId, int driverId, int endMileage, string note = "")
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var trip = await context.VehicleTrips.FindAsync(tripId);
        if (trip is null || trip.DriverId != driverId || trip.EndMileage is not null)
            return "Otvorena vožnja nije pronađena.";

        if (endMileage < trip.StartMileage)
            return "Završna kilometraža ne može biti manja od početne.";

        trip.EndMileage = endMileage;
        trip.EndedAt = DateTime.Now;
        if (!string.IsNullOrWhiteSpace(note))
            trip.Note = note;

        var vehicle = await context.Vehicles.FindAsync(trip.VehicleId);
        if (vehicle is not null && endMileage > vehicle.CurrentMileage)
            vehicle.CurrentMileage = endMileage;

        await context.SaveChangesAsync();
        return null;
    }
}
