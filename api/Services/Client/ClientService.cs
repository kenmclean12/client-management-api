using api.Data;
using api.DTOs.Client;
using ClientModel = api.Models.Clients.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Client;

public static class ClientService
{
  public static async Task MapClientEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/client").WithTags("Client");

    group.MapGet("/", [Authorize] async (AppDbContext db) =>
      {
        return Results.Ok(await db.Clients.ToListAsync());
      }
    )
    .WithSummary("Find all clients")
    .WithDescription("Returns all client records")
    .Produces<ClientModel>(StatusCodes.Status200OK);

    group.MapGet("/{id:int}", [Authorize] async (AppDbContext db, int id) =>
      {
        var client = await db.Clients.FindAsync(id);
        if (client is null) return Results.NotFound();
        return Results.Ok(client);
      }
    )
    .WithSummary("Find all clients")
    .WithDescription("Returns all client records")
    .Produces<ClientModel>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/", [Authorize(Roles = "ADMIN")] async (AppDbContext db, ClientCreateDto dto) =>
      {
        var client = ClientModel.Create(dto);
        db.Clients.Add(client);
        await db.SaveChangesAsync();
        return Results.Created($"/client/{client.Id}", client);
      }
    )
    .WithSummary("Create a new client")
    .WithDescription("Creates a client and returns the newly created record.")
    .Produces<ClientModel>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status409Conflict);

    group.MapPut("/{id:int}", [Authorize(Roles = "ADMIN")] async (AppDbContext db, ClientUpdateDto dto, int id) =>
      {
        var client = await db.Clients.FindAsync(id);
        if (client is null) return Results.NotFound();

        client.Update(dto);
        await db.SaveChangesAsync();
        return Results.Ok(client);
      }
    )
    .WithSummary("Update client information")
    .WithDescription("Updates a client and returns the newly created record.")
    .Produces<ClientModel>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status409Conflict);

    group.MapDelete("/{id:int}", [Authorize(Roles = "ADMIN")] async (AppDbContext db, int id) =>
      {
        var client = await db.Clients.FindAsync(id);
        if (client is null) return Results.NotFound();

        db.Clients.Remove(client);
        await db.SaveChangesAsync();

        return Results.NoContent();
      }
    )
    .WithSummary("Remove a client")
    .WithDescription("Removes a client and returns a 204 Response on success.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);
  }
}