using api.Data;
using api.DTOs.User;
using ModelUser = api.Models.Users.User;
using Microsoft.EntityFrameworkCore;
using api.Helpers.Token;

namespace api.Services.User;

public static class UserService
{
  public static async Task MapUserEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/user").WithTags("User");

    group.MapGet("/", async (AppDbContext db) =>
        {
          var users = await db.Users.ToListAsync();
          var dtos = users.Select(u => u.ToResponse()).ToList();
          return Results.Ok(dtos);
        }
    )
    .RequireJwt()
    .WithSummary("Find all users")
    .WithDescription("Returns all user records")
    .Produces<List<UserResponseDto>>(StatusCodes.Status200OK);

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
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/", async (UserCreateDto dto, AppDbContext db) =>
      {
        var user = ModelUser.Create(dto);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Results.Created($"/user/{user.Id}", user.ToResponse());
      }
    )
    .RequireJwt()
    .WithSummary("Create a new user")
    .WithDescription("Creates a user and returns the newly created record.")
    .Produces<UserResponseDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
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
    .RequireJwt()
    .WithSummary("Update user information")
    .WithDescription("Updates a user and returns the newly created record.")
    .Produces<UserResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
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
    .RequireJwt()
    .WithSummary("Reset a users password")
    .WithDescription("Resets a users password and returns the newly created record.")
    .Produces<UserResponseDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status409Conflict);

    group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        if (user is null) return Results.NotFound();

        db.Users.Remove(user);
        await db.SaveChangesAsync();

        return Results.NoContent();
      }
    )
    .RequireJwt()
    .WithSummary("Remove a user")
    .WithDescription("Removes a user and returns a 204 Response on success.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);
  }
}