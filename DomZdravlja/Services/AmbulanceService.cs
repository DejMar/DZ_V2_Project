using DomZdravlja.Data;
using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Services;

public class AmbulanceService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public AmbulanceService(IDbContextFactory<AppDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<List<Ambulance>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Ambulances.AsNoTracking().OrderBy(a => a.Name).ToListAsync();
    }

    public async Task<Ambulance?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Ambulances.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<string> GetNameByIdAsync(int id) =>
        (await GetByIdAsync(id))?.Name ?? "Nepoznata ambulanta";
}
