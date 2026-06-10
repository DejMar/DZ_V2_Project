using DomZdravlja;
using DomZdravlja.Components;
using DomZdravlja.Models;
using DomZdravlja.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<JsonFileRepository<User>>(sp =>
    new JsonFileRepository<User>(sp.GetRequiredService<IWebHostEnvironment>(), "users.json"));
builder.Services.AddSingleton<JsonFileRepository<Medicine>>(sp =>
    new JsonFileRepository<Medicine>(sp.GetRequiredService<IWebHostEnvironment>(), "medicines.json"));
builder.Services.AddSingleton<JsonFileRepository<Ambulance>>(sp =>
    new JsonFileRepository<Ambulance>(sp.GetRequiredService<IWebHostEnvironment>(), "ambulances.json"));
builder.Services.AddSingleton<JsonFileRepository<MedicationRequest>>(sp =>
    new JsonFileRepository<MedicationRequest>(sp.GetRequiredService<IWebHostEnvironment>(), "requests.json"));

builder.Services.AddSingleton<DataInitializer>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MedicineService>();
builder.Services.AddScoped<AmbulanceService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ReportExportService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DataInitializer>();
    await initializer.InitializeAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseSession();
app.UseAntiforgery();

app.MapAuthEndpoints();
app.MapExportEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
