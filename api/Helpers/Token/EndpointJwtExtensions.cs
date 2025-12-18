namespace api.Helpers.Token;

public static class EndpointJwtExtensions
{
  public static RouteHandlerBuilder RequireJwt(this RouteHandlerBuilder builder, params string[] roles)
  {
    return builder.AddEndpointFilter(new JwtRequireFilter(roles));
  }
}
