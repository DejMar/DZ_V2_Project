using DomZdravlja.Models;
using DomZdravlja.Services;

namespace DomZdravlja;

public static class AuthEndpoints
{
    private const string SessionKey = "UserId";

    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/login", async (HttpContext context, JsonFileRepository<User> users) =>
        {
            var form = await context.Request.ReadFormAsync();
            var username = form["username"].ToString();
            var password = form["password"].ToString();

            var allUsers = await users.GetAllAsync();
            var user = allUsers.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);

            if (user is null || !user.IsActive)
            {
                context.Response.Redirect("/prijava?error=1");
                return;
            }

            context.Session.SetInt32(SessionKey, user.Id);

            var destination = "/pregled";

            context.Response.Redirect(destination);
        }).DisableAntiforgery();

        app.MapGet("/auth/logout", (HttpContext context) =>
        {
            context.Session.Remove(SessionKey);
            context.Response.Redirect("/prijava");
        });
    }
}
