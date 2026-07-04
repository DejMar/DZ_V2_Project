using DomZdravlja.Data;
using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Services;

public class MedicineService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public MedicineService(IDbContextFactory<AppDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<List<Medicine>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Medicines.AsNoTracking().OrderBy(m => m.Name).ToListAsync();
    }

    public async Task<Medicine?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Medicines.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task AddAsync(Medicine medicine)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Medicines.Add(medicine);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Medicine medicine)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        context.Medicines.Update(medicine);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var medicine = await context.Medicines.FindAsync(id);
        if (medicine is null)
            return;

        context.Medicines.Remove(medicine);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ReduceStockAsync(int medicineId, int quantity)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var medicine = await context.Medicines.FindAsync(medicineId);
        if (medicine is null || medicine.Quantity < quantity)
            return false;

        medicine.Quantity -= quantity;
        await context.SaveChangesAsync();
        return true;
    }
}
