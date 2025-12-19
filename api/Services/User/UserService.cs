using api.Data;
using api.DTOs.User;
using ModelUser = api.Models.Users.User;
using Microsoft.EntityFrameworkCore;
using api.Helpers.Token;
using api.Models.Users;
using System.Security.Claims;
using api.Services.Email;

namespace api.Services.User;

public static class UserService
{
  public static async Task MapUserEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/user").WithTags("User");

    group.MapGet("/", async (AppDbContext db) =>
      {
        return Results.Ok(
          await db.Users
            .Select(u => u.ToResponse())
            .ToListAsync()
        );
      }
    )
    .RequireJwt()
    .WithSummary("Find all users")
    .WithDescription("Returns all user records")
    .Produces<List<UserResponseDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

    group.MapGet("/admin", async (AppDbContext db) =>
      {
        return Results.Ok(
          await db.Users
          .Where(u => u.Role == UserRole.Admin)
          .Select(u => u.ToResponse())
          .ToListAsync()
        );
      }
    )
    .RequireJwt()
    .WithSummary("Find all admin users")
    .WithDescription("Returns all admin user records")
    .Produces<List<UserResponseDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);


    group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        if (user is null) return Results.NotFound();

        return Results.Ok(user.ToResponse());
      }
    )
    .RequireJwt()
    .WithSummary("Find a user by ID")
    .WithDescription("Find a user record by ID")
    .Produces<UserResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/invite-user",
      async (
        AppDbContext db,
        UserInviteCreateDto dto,
        HttpContext context,
        IEmailService emailService,
        IConfiguration config,
        IWebHostEnvironment env,
        CancellationToken token
      ) =>
    {
      var user = context.User;
      var email = dto.Email.Trim().ToLowerInvariant();
      if (await db.UserInvites.AnyAsync(i => i.Email == email, token))
      {
        return Results.Conflict();
      }

      var userId = int.Parse(
        user.FindFirstValue(ClaimTypes.NameIdentifier)!
      );

      var invite = UserInvite.Create(email, userId);
      db.UserInvites.Add(invite);
      await db.SaveChangesAsync(token);

      var inviteLink =
        $"{config["App:FrontendUrl"]}/register?email={email}";

      await emailService.SendUserInviteAsync(email, inviteLink, env, token);

      return Results.Created($"/user/invite-user/{invite.Id}", invite);
    })
   .RequireJwt(
     nameof(UserRole.Admin),
     nameof(UserRole.Standard)
   )
   .WithSummary("Create and send a user invite for a particular email address")
   .WithDescription("Creates and sends a user invite and returns the newly created record.")
   .Produces<UserInvite>(StatusCodes.Status201Created)
   .Produces(StatusCodes.Status401Unauthorized)
   .Produces(StatusCodes.Status403Forbidden)
   .Produces(StatusCodes.Status409Conflict);

    group.MapPut("/{id:int}", async (AppDbContext db, UserUpdateDto dto, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        if (user is null) return Results.NotFound();

        user.Update(dto);
        await db.SaveChangesAsync();

        return Results.Ok(user.ToResponse());
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Update user information")
    .WithDescription("Updates a user and returns the newly created record.")
    .Produces<UserResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status409Conflict);

    group.MapPut("/reset-password/{id:int}", async (AppDbContext db, UserPasswordResetDto dto, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        if (user is null) return Results.NotFound();

        var valid = user.VerifyPassword(dto.Password);
        if (!valid) return Results.BadRequest("Password invalid");

        user.PasswordHash = ModelUser.HashPassword(user, dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Results.Ok(user.ToResponse());
      }
    )
     .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Reset a users password")
    .WithDescription("Resets a users password and returns the newly created record.")
    .Produces<UserResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status404NotFound);

    group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        if (user is null) return Results.NotFound();

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return Results.NoContent();
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Remove a user")
    .WithDescription("Removes a user and returns a 204 Response on success.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status404NotFound);
  }
}