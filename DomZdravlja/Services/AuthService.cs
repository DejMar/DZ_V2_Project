using DomZdravlja.Data;
using DomZdravlja.Models;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja.Services;

public class AuthService
{
    private const string SessionKey = "UserId";
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public User? CurrentUser { get; private set; }

    public AuthService(IDbContextFactory<AppDbContext> contextFactory, IHttpContextAccessor httpContextAccessor)
    {
        _contextFactory = contextFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => CurrentUser is not null;

    public bool IsInRole(UserRole role) => CurrentUser?.Role == role;

    public async Task EnsureLoadedAsync()
    {
        if (CurrentUser is not null)
            return;

        var userId = _httpContextAccessor.HttpContext?.Session.GetInt32(SessionKey);
        if (userId is not int id)
            return;

        await using var context = await _contextFactory.CreateDbContextAsync();
        CurrentUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
    }
}
