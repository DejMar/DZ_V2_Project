using DomZdravlja.Models;
using DomZdravlja.Services;

namespace DomZdravlja;

public static class ExportEndpoints
{
    private const string SessionKey = "UserId";

    public static void MapExportEndpoints(this WebApplication app)
    {
        app.MapGet("/admin/export/csv", async (HttpContext context, JsonFileRepository<User> users, ReportExportService exportService) =>
        {
            if (!await IsAdminAsync(context, users))
            {
                context.Response.StatusCode = 403;
                return;
            }

            var bytes = await exportService.GenerateCsvAsync();
            var fileName = $"izvjestaj_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            context.Response.ContentType = "text/csv; charset=utf-8";
            context.Response.Headers.ContentDisposition = $"attachment; filename=\"{fileName}\"";
            await context.Response.Body.WriteAsync(bytes);
        });

        app.MapGet("/admin/export/report", async (HttpContext context, JsonFileRepository<User> users, ReportExportService exportService) =>
        {
            if (!await IsAdminAsync(context, users))
            {
                context.Response.StatusCode = 403;
                return;
            }

            var html = await exportService.GenerateHtmlReportAsync();
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(html);
        });
    }

    private static async Task<bool> IsAdminAsync(HttpContext context, JsonFileRepository<User> users)
    {
        var userId = context.Session.GetInt32(SessionKey);
        if (userId is not int id)
            return false;

        var user = (await users.GetAllAsync()).FirstOrDefault(u => u.Id == id);
        return user is { Role: UserRole.Administrator, IsActive: true };
    }
}
