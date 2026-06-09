using DomZdravlja.Models;

namespace DomZdravlja.Services;

public class AuthService
{
    private const string SessionKey = "UserId";
    private readonly JsonFileRepository<User> _users;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public User? CurrentUser { get; private set; }

    public AuthService(JsonFileRepository<User> users, IHttpContextAccessor httpContextAccessor)
    {
        _users = users;
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

        var users = await _users.GetAllAsync();
        CurrentUser = users.FirstOrDefault(u => u.Id == id);
    }
}
