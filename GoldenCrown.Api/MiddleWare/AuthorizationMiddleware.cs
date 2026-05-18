using GoldenCrown.Attributes;
using GoldenCrown.Database;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.MiddleWare
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ApplicationDBContext dbcontext)
        {
            var attribute = context.GetEndpoint()?.Metadata.GetMetadata<MyAuthorizeAttribute>();
            if (attribute == null)
            {
                await _next(context);
                return;
            }

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
