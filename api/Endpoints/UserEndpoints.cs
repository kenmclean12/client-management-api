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
    var group = app.MapGroup("/user");

    group.MapGet("/", async (AppDbContext db) =>
        {
          return await db.Users.ToListAsync();
        }
    );

    group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        return user is not null ? Results.Ok(user) : Results.NotFound();
      }
    );

    group.MapPost("/", async (UserCreateDto dto, AppDbContext db) =>
      {
        var user = User.Create(dto);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Results.Created($"/user/{user.Id}", user);
      }
    );

    group.MapPut("/{id:int}", async (AppDbContext db, UserUpdateDto dto, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        if (user is null) return Results.NotFound();
        user.Update(dto);
        await db.SaveChangesAsync();
        return Results.Ok(user);
      }
    );

    group.MapPut("/reset-password/{id:int}", async (AppDbContext db, UserPasswordResetDto dto, int id) =>
    {
        var user = await db.Users.FindAsync(id);
        if (user is null) return Results.NotFound();

        var valid = user.VerifyPassword(dto.password);
        if (!valid) return Results.BadRequest("Password invalid");

        user.PasswordHash = User.HashPassword(user, dto.newPassword);
        user.UpdatedAt = DateTime.UtcNow;
  
        await db.SaveChangesAsync();
        return Results.Ok(user);
    });

    group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
      {
        var user = await db.Users.FindAsync(id);
        if (user is null) return Results.NotFound();
        db.Users.Remove(user);
        await db.SaveChangesAsync();
        return Results.NoContent();
      }
    );
  }
}