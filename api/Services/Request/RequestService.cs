using api.Data;
using api.DTOs.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RequestModel = api.Models.Requests.Request;

namespace api.Services.Request;

public static class RequestService
{
  public static async Task MapRequestEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/request").WithTags("Request");

    group.MapGet("/", [Authorize] async (AppDbContext db) =>
      {
        return Results.Ok(await db.Requests.ToListAsync());
      }
    )
    .WithSummary("Find all requests")
    .WithDescription("Returns all request records")
    .Produces<List<RequestModel>>(StatusCodes.Status200OK);

    group.MapGet("/{id:int}", [Authorize] async (AppDbContext db, int id) =>
      {
        var request = await db.Requests.FindAsync(id);
        if (request is null) return Results.NotFound();

        return Results.Ok(request);
      }
    )
    .WithSummary("Find a request by ID")
    .WithDescription("Returns a request record")
    .Produces<RequestModel>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapGet("/client/{id:int}", [Authorize] async (AppDbContext db, int id) =>
      {
        var requests = await db.Requests
          .Where(r => r.ClientId == id)
          .ToListAsync();

        if (requests.Count == 0) return Results.NotFound();

        return Results.Ok(requests);
      }
    )
    .WithSummary("Find all requests by client")
    .WithDescription("Returns all requests for a specific client")
    .Produces<List<RequestModel>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/", [Authorize] async (AppDbContext db, RequestCreateDto dto) =>
      {
        var request = RequestModel.Create(dto);
        db.Requests.Add(request);
        await db.SaveChangesAsync();

        return Results.Created($"/request/{request.Id}", request);
      }
    )
    .WithSummary("Create a new request")
    .WithDescription("Creates a request and returns the newly created record.")
    .Produces<RequestModel>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

    group.MapPut("/{id:int}", [Authorize(Roles = "ADMIN,STANDARD")] async (AppDbContext db, RequestUpdateDto dto, int id) =>
      {
        var request = await db.Requests.FindAsync(id);
        if (request is null) return Results.NotFound();

        request.Update(dto);
        await db.SaveChangesAsync();

        return Results.Ok(request);
      }
    )
    .WithSummary("Update request information")
    .WithDescription("Updates a request and returns the updated record.")
    .Produces<RequestModel>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

    group.MapDelete("/{id:int}", [Authorize(Roles = "ADMIN")] async (AppDbContext db, int id) =>
      {
        var request = await db.Requests.FindAsync(id);
        if (request is null) return Results.NotFound();

        db.Requests.Remove(request);
        await db.SaveChangesAsync();

        return Results.NoContent();
      }
    )
    .WithSummary("Remove a request")
    .WithDescription("Removes a request and returns a 204 Response on success.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);
  }
}
