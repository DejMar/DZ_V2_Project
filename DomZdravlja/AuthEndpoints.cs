using DomZdravlja.Data;
using Microsoft.EntityFrameworkCore;

namespace DomZdravlja;

public static class AuthEndpoints
{
    private const string SessionKey = "UserId";

    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/login", async (HttpContext context, IDbContextFactory<AppDbContext> contextFactory) =>
        {
            var form = await context.Request.ReadFormAsync();
            var username = form["username"].ToString();
            var password = form["password"].ToString();

            await using var db = await contextFactory.CreateDbContextAsync();
            var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u =>
                u.Username.ToLower() == username.ToLower() &&
                u.Password == password);

            if (user is null || !user.IsActive)
            {
                context.Response.Redirect("/prijava?error=1");
                return;
            }

            context.Session.SetInt32(SessionKey, user.Id);
            context.Response.Redirect("/pregled");
        }).DisableAntiforgery();

        app.MapGet("/auth/logout", (HttpContext context) =>
        {
            context.Session.Remove(SessionKey);
            context.Response.Redirect("/prijava");
        });
    }
}
