using api.Data;
using api.DTOs.User;
using api.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace api.Endpoints.Users;

public static class UserService
{
  public static async Task MapUserEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/user").WithTags("User");

    group.MapGet("/", [Authorize] async (AppDbContext db) =>
        {
          var users = await db.Users.ToListAsync();
          var dtos = users.Select(u => u.ToResponse()).ToList();
          return Results.Ok(dtos);
        }
    )
    .WithSummary("Find all users")
    .WithDescription("Returns all user records")
    .Produces<UserResponseDto>(StatusCodes.Status200OK);

    group.MapGet("/{id:int}", [Authorize] async (AppDbContext db, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        return user is not null ? Results.Ok(user.ToResponse()) : Results.NotFound();
      }
    )
    .WithSummary("Find a user by ID")
    .WithDescription("Find a user record by ID")
    .Produces<UserResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/", [Authorize] async (UserCreateDto dto, AppDbContext db) =>
      {
        var user = User.Create(dto);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Results.Created($"/user/{user.Id}", user.ToResponse());
      }
    )
    .WithSummary("Create a new user")
    .WithDescription("Creates a user and returns the newly created record.")
    .Produces<UserResponseDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status409Conflict);

    group.MapPut("/{id:int}", [Authorize] async (AppDbContext db, UserUpdateDto dto, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        if (user is null) return Results.NotFound();
        user.Update(dto);
        await db.SaveChangesAsync();
        return Results.Ok(user.ToResponse());
      }
    )
    .WithSummary("Update user information")
    .WithDescription("Updates a user and returns the newly created record.")
    .Produces<UserResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status409Conflict);

    group.MapPut("/reset-password/{id:int}", [Authorize] async (AppDbContext db, UserPasswordResetDto dto, int id) =>
    {
      var user = await db.Users.FindAsync(id);
      if (user is null) return Results.NotFound();

      var valid = user.VerifyPassword(dto.Password);
      if (!valid) return Results.BadRequest("Password invalid");

      user.PasswordHash = User.HashPassword(user, dto.NewPassword);
      user.UpdatedAt = DateTime.UtcNow;

      await db.SaveChangesAsync();
      return Results.Ok(user.ToResponse());
    })
    .WithSummary("Reset a users password")
    .WithDescription("Resets a users password and returns the newly created record.")
    .Produces<UserResponseDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status409Conflict);

    group.MapDelete("/{id:int}", [Authorize(Roles = "Admin")] async (AppDbContext db, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        if (user is null) return Results.NotFound();
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.NoContent();
      }
    )
    .WithSummary("Remove a user")
    .WithDescription("Removes a user and returns a 204 Response on success.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);
  }
}