using DomZdravlja.Models;

namespace DomZdravlja.Services;

public class AmbulanceService
{
    private readonly JsonFileRepository<Ambulance> _repository;

    public AmbulanceService(JsonFileRepository<Ambulance> repository) => _repository = repository;

    public Task<List<Ambulance>> GetAllAsync() => _repository.GetAllAsync();

    public async Task<Ambulance?> GetByIdAsync(int id) =>
        (await _repository.GetAllAsync()).FirstOrDefault(a => a.Id == id);

    public async Task<string> GetNameByIdAsync(int id) =>
        (await GetByIdAsync(id))?.Name ?? "Nepoznata ambulanta";
}
