using System.Threading.Tasks;
using api.Data;
using api.Models.User;
using api.Services.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Endpoints.Auth;

public static class AuthService
{
  public static async Task MapAuthEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/auth").WithTags("User");

    group.MapPost("/login", async (LoginRequest req, AppDbContext db, TokenService tokenService) =>
    {
      var user = await db.Users.FirstOrDefaultAsync((u) => u.Email == req.Email);
      if (user is null) return Results.Unauthorized();

      var hasher = new PasswordHasher<User>();
      var result = hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
      if (result != PasswordVerificationResult.Success) return Results.Unauthorized();

      var jwt = tokenService.CreateToken(user);
      return Results.Ok(new LoginResponse(jwt));
    })
    .WithSummary("Login")
    .WithDescription("Verifies user email and password and returns a signed access token")
    .Produces<LoginResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);
  }
}

public record LoginResponse(string Token);
public record LoginRequest(string Email, string Password);