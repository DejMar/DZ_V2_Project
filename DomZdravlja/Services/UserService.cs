using DomZdravlja.Data;
using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Services;

public class UserService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public UserService(IDbContextFactory<AppDbContext> contextFactory) => _contextFactory = contextFactory;

    public async Task<List<User>> GetAllAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.AsNoTracking().OrderBy(u => u.FullName).ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> UsernameExistsAsync(string username, int? excludeId = null)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Users.AsNoTracking()
            .Where(u => u.Username.ToLower() == username.ToLower());

        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task AddAsync(User user)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        user.IsActive = true;
        if (user.Role != UserRole.Korisnik)
            user.AmbulanceId = null;

        context.Users.Add(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        if (user.Role != UserRole.Korisnik)
            user.AmbulanceId = null;

        context.Users.Update(user);
        await context.SaveChangesAsync();
    }

    public async Task SetActiveAsync(int id, bool isActive)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var user = await context.Users.FindAsync(id);
        if (user is null)
            return;

        user.IsActive = isActive;
        await context.SaveChangesAsync();
    }
}
