using DomZdravlja.Models;

namespace DomZdravlja.Services;

public class UserService
{
    private readonly JsonFileRepository<User> _repository;

    public UserService(JsonFileRepository<User> repository) => _repository = repository;

    public Task<List<User>> GetAllAsync() => _repository.GetAllAsync();

    public async Task<User?> GetByIdAsync(int id) =>
        (await _repository.GetAllAsync()).FirstOrDefault(u => u.Id == id);

    public async Task<bool> UsernameExistsAsync(string username, int? excludeId = null)
    {
        var users = await _repository.GetAllAsync();
        return users.Any(u =>
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
            u.Id != excludeId);
    }

    public async Task AddAsync(User user)
    {
        var users = await _repository.GetAllAsync();
        user.Id = users.Count == 0 ? 1 : users.Max(u => u.Id) + 1;
        user.IsActive = true;
        if (user.Role != UserRole.Korisnik)
            user.AmbulanceId = null;
        users.Add(user);
        await _repository.SaveAllAsync(users);
    }

    public async Task UpdateAsync(User user)
    {
        var users = await _repository.GetAllAsync();
        var index = users.FindIndex(u => u.Id == user.Id);
        if (index < 0)
            return;

        if (user.Role != UserRole.Korisnik)
            user.AmbulanceId = null;

        users[index] = user;
        await _repository.SaveAllAsync(users);
    }

    public async Task SetActiveAsync(int id, bool isActive)
    {
        var users = await _repository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Id == id);
        if (user is null)
            return;

        user.IsActive = isActive;
        await _repository.SaveAllAsync(users);
    }
}
