using GoldenCrown.Attributes;
using GoldenCrown.Database;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.MiddleWare
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _scopeFactory;

        public AuthorizationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _scopeFactory = scopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            var attribute = context.GetEndpoint()?.Metadata.GetMetadata<MyAuthorizeAttribute>();
            if (attribute == null)
            {
                await _next(context);
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

            var token = context.Request.Headers[Constans.Authorization].FirstOrDefault()?.Split("").Last();
            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var session = await dbcontext.Sessions.FirstOrDefaultAsync(x => x.Token == token);
            if (session == null || session.ExpiresAt < DateTime.UtcNow)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            context.Items[Constans.UserIdContextParametr] = session.UserId;

            await _next(context);
        }
    }
}
