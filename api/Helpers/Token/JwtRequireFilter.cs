using System.Security.Claims;

namespace api.Helpers.Token;

public class JwtRequireFilter : IEndpointFilter
{
  private readonly string[] _requiredRoles;

  public JwtRequireFilter(params string[] requiredRoles)
  {
    _requiredRoles = requiredRoles;
  }
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

    if (_requiredRoles.Length > 0)
    {
      var userRoles = principal
        .Claims
        .Where(c => c.Type == ClaimTypes.Role)
        .Select(c => c.Value)
        .ToArray();

      var allowed = _requiredRoles.Any(c => userRoles.Contains(c));
      if (!allowed)
      {
        return Results.Forbid();
      }
    }

    return await next(ctx);
  }
}
