using api.Data;
using api.DTOs.Client;
using ClientModel = api.Models.Clients.Client;
using Microsoft.EntityFrameworkCore;
using api.Helpers.Token;
using api.Models.Users;

namespace api.Services.Client;

public static class ClientService
{
  public static async Task MapClientEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/client").WithTags("Client");

    group.MapGet("/", async (AppDbContext db) =>
      {
        return Results.Ok(
          await db.Clients
            .Where(c => c.SoftDeleted == false)
            .Select(c => c.ToResponse())
            .ToListAsync()
        );
      }
    )
    .RequireJwt()
    .WithSummary("Find all active clients")
    .WithDescription("Returns all active client records")
    .Produces<List<ClientResponseDto>>(StatusCodes.Status200OK);

    group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
      {
        var client = await db.Clients.FindAsync(id);
        if (client is null) return Results.NotFound();
        return Results.Ok(client.ToResponse());
      }
    )
    .RequireJwt()
    .WithSummary("Find client by id")
    .WithDescription("Returns a client record")
    .Produces<ClientResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/", async (AppDbContext db, ClientCreateDto dto) =>
      {
        var client = ClientModel.Create(dto);
        db.Clients.Add(client);
        await db.SaveChangesAsync();
        return Results.Created($"/client/{client.Id}", client.ToResponse());
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Create a new client")
    .WithDescription("Creates a client and returns the newly created record.")
    .Produces<ClientResponseDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status409Conflict);

    group.MapPut("/{id:int}", async (AppDbContext db, ClientUpdateDto dto, int id) =>
      {
        var client = await db.Clients.FindAsync(id);
        if (client is null) return Results.NotFound();

        client.Update(dto);
        await db.SaveChangesAsync();
        return Results.Ok(client.ToResponse());
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Update client information")
    .WithDescription("Updates a client and returns the newly created record.")
    .Produces<ClientResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status409Conflict);

    group.MapPut("/soft-delete/{id:int}", async (AppDbContext db, int id) =>
      {
        var client = await db.Clients.FindAsync(id);
        if (client is null) return Results.NotFound();

        client.SoftDeleted = true;
        await db.SaveChangesAsync();

        return Results.Ok(client.ToResponse());
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin)
    )
    .WithSummary("Soft Delete a client")
    .WithDescription("Soft Deletes a client and returns a 204 Response on success.")
    .Produces<ClientResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);
  }
}