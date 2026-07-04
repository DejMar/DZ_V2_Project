using DomZdravlja;
using DomZdravlja.Components;
using DomZdravlja.Data;
using DomZdravlja.Services;
using Microsoft.EntityFrameworkCore;

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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

builder.Services.AddScoped<DbSeeder>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MedicineService>();
builder.Services.AddScoped<AmbulanceService>();
builder.Services.AddScoped<RequestService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ReportExportService>();
builder.Services.AddScoped<StockIntakeService>();
builder.Services.AddScoped<DashboardService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    await using var db = await contextFactory.CreateDbContextAsync();
    await db.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedAsync();
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
