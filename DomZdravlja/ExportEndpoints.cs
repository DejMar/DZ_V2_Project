using DomZdravlja.Data;
using DomZdravlja.Models;
using DomZdravlja.Services;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja;

public static class ExportEndpoints
{
    private const string SessionKey = "UserId";

    public static void MapExportEndpoints(this WebApplication app)
    {
        app.MapGet("/admin/export/csv", async (HttpContext context, IDbContextFactory<AppDbContext> contextFactory, ReportExportService exportService) =>
        {
            if (!await IsAdminAsync(context, contextFactory))
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

        app.MapGet("/admin/export/report", async (HttpContext context, IDbContextFactory<AppDbContext> contextFactory, ReportExportService exportService) =>
        {
            if (!await IsAdminAsync(context, contextFactory))
            {
                context.Response.StatusCode = 403;
                return;
            }

            var html = await exportService.GenerateHtmlReportAsync();
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(html);
        });
    }

    private static async Task<bool> IsAdminAsync(HttpContext context, IDbContextFactory<AppDbContext> contextFactory)
    {
        var userId = context.Session.GetInt32(SessionKey);
        if (userId is not int id)
            return false;

        await using var db = await contextFactory.CreateDbContextAsync();
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
        return user is { Role: UserRole.Administrator, IsActive: true };
    }
}
