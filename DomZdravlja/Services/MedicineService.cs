using DomZdravlja.Models;

namespace DomZdravlja.Services;

public class MedicineService
{
    private readonly JsonFileRepository<Medicine> _repository;

    public MedicineService(JsonFileRepository<Medicine> repository) => _repository = repository;

    public Task<List<Medicine>> GetAllAsync() => _repository.GetAllAsync();

    public async Task<Medicine?> GetByIdAsync(int id) =>
        (await _repository.GetAllAsync()).FirstOrDefault(m => m.Id == id);

    public async Task AddAsync(Medicine medicine)
    {
        var medicines = await _repository.GetAllAsync();
        medicine.Id = medicines.Count == 0 ? 1 : medicines.Max(m => m.Id) + 1;
        medicines.Add(medicine);
        await _repository.SaveAllAsync(medicines);
    }

    public async Task UpdateAsync(Medicine medicine)
    {
        var medicines = await _repository.GetAllAsync();
        var index = medicines.FindIndex(m => m.Id == medicine.Id);
        if (index >= 0)
        {
            medicines[index] = medicine;
            await _repository.SaveAllAsync(medicines);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var medicines = await _repository.GetAllAsync();
        medicines.RemoveAll(m => m.Id == id);
        await _repository.SaveAllAsync(medicines);
    }

    public async Task<bool> ReduceStockAsync(int medicineId, int quantity)
    {
        var medicines = await _repository.GetAllAsync();
        var medicine = medicines.FirstOrDefault(m => m.Id == medicineId);
        if (medicine is null || medicine.Quantity < quantity)
            return false;

        medicine.Quantity -= quantity;
        await _repository.SaveAllAsync(medicines);
        return true;
    }
}
