using api.Data;
using api.DTOs.User;
using ModelUser = api.Models.Users.User;
using api.Services.Token;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Auth;

public static class AuthService
{
  public static async Task MapAuthEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/auth").WithTags("Auth");

    group.MapPost("/login", async (LoginRequest req, AppDbContext db, TokenService tokenService) =>
      {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user is null || !user.VerifyPassword(req.Password))
          return Results.Unauthorized();

        var jwt = tokenService.CreateToken(user);

        return Results.Ok(new TokenResponse(jwt, user.ToResponse()));
      }
    )
    .WithSummary("Login")
    .WithDescription("Verifies user email and password and returns a signed access token")
    .Produces<TokenResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

    group.MapPost("/register", async (UserCreateDto dto, AppDbContext db, TokenService tokenService) =>
    {
      var validInvite = await db.UserInvites.FirstOrDefaultAsync(i => 
        i.Email == dto.Email
        && i.ExpiryDate > DateTime.UtcNow
      );

      if (validInvite is null) return Results.Unauthorized();
      var newUser = ModelUser.Create(dto);
      await db.Users.AddAsync(newUser);
      await db.SaveChangesAsync();

      var user = await db.Users.FindAsync(newUser.Id);
      if (user is null) return Results.NotFound();

      var jwt = tokenService.CreateToken(user);
      return Results.Ok(new TokenResponse(jwt, user.ToResponse()));
    })
    .WithSummary("Register")
    .WithDescription("Creates a user and returns a signed access token")
    .Produces<TokenResponse>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);
  }
}

public record TokenResponse(string Token, UserResponseDto User);
public record LoginRequest(string Email, string Password);