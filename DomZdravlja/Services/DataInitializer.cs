using DomZdravlja.Models;

namespace DomZdravlja.Services;

public class DataInitializer
{
    private readonly JsonFileRepository<User> _users;
    private readonly JsonFileRepository<Medicine> _medicines;
    private readonly JsonFileRepository<Ambulance> _ambulances;
    private readonly JsonFileRepository<MedicationRequest> _requests;

    public DataInitializer(
        JsonFileRepository<User> users,
        JsonFileRepository<Medicine> medicines,
        JsonFileRepository<Ambulance> ambulances,
        JsonFileRepository<MedicationRequest> requests)
    {
        _users = users;
        _medicines = medicines;
        _ambulances = ambulances;
        _requests = requests;
    }

    public async Task InitializeAsync()
    {
        if ((await _users.GetAllAsync()).Count == 0)
        {
            await _users.SaveAllAsync(
            [
                new User { Id = 1, Username = "admin", Password = "admin123", Role = UserRole.Administrator, FullName = "Admin Korisnik" },
                new User { Id = 2, Username = "moderator", Password = "mod123", Role = UserRole.Moderator, FullName = "Moderator Korisnik" },
                new User { Id = 3, Username = "korisnik1", Password = "user123", Role = UserRole.Korisnik, AmbulanceId = 1, FullName = "Dr. Ana Marković" },
                new User { Id = 4, Username = "korisnik2", Password = "user123", Role = UserRole.Korisnik, AmbulanceId = 2, FullName = "Dr. Marko Jović" }
            ]);
        }

        if ((await _ambulances.GetAllAsync()).Count == 0)
        {
            await _ambulances.SaveAllAsync(
            [
                new Ambulance { Id = 1, Name = "Opća ambulanta" },
                new Ambulance { Id = 2, Name = "Pedijatrijska ambulanta" },
                new Ambulance { Id = 3, Name = "Stomatološka ambulanta" }
            ]);
        }

        if ((await _medicines.GetAllAsync()).Count == 0)
        {
            await _medicines.SaveAllAsync(
            [
                new Medicine { Id = 1, Name = "Paracetamol 500mg", Description = "Analgetik i antipiretik", Quantity = 500, Unit = "tableta", MinimumStock = 100 },
                new Medicine { Id = 2, Name = "Ibuprofen 400mg", Description = "Protivupalni lijek", Quantity = 300, Unit = "tableta", MinimumStock = 80 },
                new Medicine { Id = 3, Name = "Amoxicillin 500mg", Description = "Antibiotik", Quantity = 150, Unit = "kapsula", MinimumStock = 50 },
                new Medicine { Id = 4, Name = "Hlorheksidin", Description = "Antiseptik", Quantity = 40, Unit = "flaša", MinimumStock = 20 },
                new Medicine { Id = 5, Name = "Fizološki rastvor", Description = "Infuzija 0.9% NaCl", Quantity = 200, Unit = "vrecica", MinimumStock = 50 }
            ]);
        }

        if ((await _requests.GetAllAsync()).Count == 0)
            await _requests.SaveAllAsync([]);
    }
}
