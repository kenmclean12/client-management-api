namespace api.Helpers.Token;

public static class EndpointJwtExtensions
{
    public static RouteHandlerBuilder RequireJwt(this RouteHandlerBuilder builder)
    {
        return builder.AddEndpointFilter<JwtRequireFilter>();
    }
}
