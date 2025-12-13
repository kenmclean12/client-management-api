namespace api.Helpers.Token;

public class JwtRequireFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext ctx,
        EndpointFilterDelegate next
    )
    {
        var http = ctx.HttpContext;
        var config = http.RequestServices.GetRequiredService<IConfiguration>();

        var authHeader = http.Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer "))
            return Results.Unauthorized();

        var token = authHeader["Bearer ".Length..];
        var principal = JwtHelper.Validate(token, config["JWT_SECRET"]!);

        if (principal is null)
            return Results.Unauthorized();
        http.User = principal;

        return await next(ctx);
    }
}
