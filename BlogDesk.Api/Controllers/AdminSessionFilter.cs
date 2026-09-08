using Microsoft.AspNetCore.Mvc.Filters;
using BlogDesk.Api.Data;

namespace BlogDesk.Api.Services;

public class AdminSessionFilter : IAsyncActionFilter
{
    private readonly AppDbContext _db;
    public AdminSessionFilter(AppDbContext db) => _db = db;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var token = context.HttpContext.Request.Headers["X-Admin-Token"].ToString();
        var session = _db.AdminSessions.FirstOrDefault(s => s.Token == token && s.ExpiresAt > DateTime.UtcNow);

        if (session is null)
        {
            context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
            return;
        }

        context.HttpContext.Items["AdminEmail"] = session.Email;
        await next();
    }
}