using DomZdravlja.Data;
using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Services;

public class StockIntakeService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public StockIntakeService(IDbContextFactory<AppDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<List<StockIntake>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StockIntakes.AsNoTracking()
            .OrderByDescending(i => i.ReceivedAt)
            .ToListAsync();
    }

    public async Task<List<StockIntake>> GetRecentAsync(int count = 10)
    {
        var intakes = await GetAllAsync();
        return intakes.Take(count).ToList();
    }

    public async Task<string?> RecordIntakeAsync(StockIntake intake)
    {
        if (intake.Quantity <= 0)
            return "Količina mora biti veća od nule.";

        await using var context = await _contextFactory.CreateDbContextAsync();
        var medicine = await context.Medicines.FindAsync(intake.MedicineId);
        if (medicine is null)
            return "Lijek nije pronađen.";

        intake.ReceivedAt = DateTime.Now;
        context.StockIntakes.Add(intake);

        medicine.Quantity += intake.Quantity;
        if (intake.UpdatedExpiryDate.HasValue)
            medicine.ExpiryDate = intake.UpdatedExpiryDate.Value;

        await context.SaveChangesAsync();
        return null;
    }
}
