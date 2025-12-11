using api.Data;
using api.Models.User;
using api.DTOs.User;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace api.Endpoints;

public static class UserService
{
  public static async Task MapUserEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/user").WithTags("User");

    group.MapGet("/", async (AppDbContext db) =>
        {
          return Results.Ok(await db.Users.ToListAsync());
        }
    )
    .WithSummary("Find all users")
    .WithDescription("Returns all user records")
    .Produces<User>(StatusCodes.Status200OK);

    group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        return user is not null ? Results.Ok(user) : Results.NotFound();
      }
    )
    .WithSummary("Find a user by ID")
    .WithDescription("Find a user record by ID")
    .Produces<User>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/", async (UserCreateDto dto, AppDbContext db) =>
      {
        var user = User.Create(dto);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Results.Created($"/user/{user.Id}", user);
      }
    )
    .WithSummary("Create a new user")
    .WithDescription("Creates a user and returns the newly created record.")
    .Produces<User>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status409Conflict);

    group.MapPut("/{id:int}", async (AppDbContext db, UserUpdateDto dto, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        if (user is null) return Results.NotFound();
        user.Update(dto);
        await db.SaveChangesAsync();
        return Results.Ok(user);
      }
    )
    .WithSummary("Update user information")
    .WithDescription("Updates a user and returns the newly created record.")
    .Produces<User>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status409Conflict);

    group.MapPut("/reset-password/{id:int}", async (AppDbContext db, UserPasswordResetDto dto, int id) =>
    {
      var user = await db.Users.FindAsync(id);
      if (user is null) return Results.NotFound();

      var valid = user.VerifyPassword(dto.Password);
      if (!valid) return Results.BadRequest("Password invalid");

      user.PasswordHash = User.HashPassword(user, dto.NewPassword);
      user.UpdatedAt = DateTime.UtcNow;

      await db.SaveChangesAsync();
      return Results.Ok(user);
    })
    .WithSummary("Reset a users password")
    .WithDescription("Resets a users password and returns the newly created record.")
    .Produces<User>(StatusCodes.Status201Created)
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
    .WithSummary("Remove a user")
    .WithDescription("Removes a user and returns a 204 Response on success.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);
  }
}