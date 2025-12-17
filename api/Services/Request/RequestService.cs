using api.Data;
using api.DTOs.Request;
using api.Helpers.Token;
using api.Models.Users;
using Microsoft.EntityFrameworkCore;
using RequestModel = api.Models.Requests.Request;
using ProjectModel = api.Models.Projects.Project;
using api.Models.Projects;
namespace api.Services.Request;

public static class RequestService
{
  public static async Task MapRequestEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/request").WithTags("Request");

    group.MapGet("/", async (AppDbContext db) =>
      {
        var requests = await db.Requests
          .Include(r => r.Client)
          .Where(r => r.Status != RequestStatus.Approved)
          .ToListAsync();
        return Results.Ok(requests);
      }
    )
    .RequireJwt()
    .WithSummary("Find all requests")
    .WithDescription("Returns all request records")
    .Produces<List<RequestModel>>(StatusCodes.Status200OK);

    group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
      {
        var request = await db.Requests.FindAsync(id);
        if (request is null) return Results.NotFound();

        return Results.Ok(request);
      }
    )
    .RequireJwt()
    .WithSummary("Find a request by ID")
    .WithDescription("Returns a request record")
    .Produces<RequestModel>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapGet("/client/{id:int}", async (AppDbContext db, int id) =>
      {
        var requests = await db.Requests
          .Where(r => r.ClientId == id)
          .Where(r => r.Status != RequestStatus.Approved)
          .Include(r => r.Client)
          .ToListAsync();

        if (requests.Count == 0) return Results.NotFound();
        return Results.Ok(requests);
      }
    )
    .RequireJwt()
    .WithSummary("Find all requests by client")
    .WithDescription("Returns all requests for a specific client")
    .Produces<List<RequestModel>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/", async (AppDbContext db, RequestCreateDto dto) =>
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

    group.MapPut("/{id:int}", async (AppDbContext db, RequestUpdateDto dto, int id) =>
      {
        var request = await db.Requests.FindAsync(id);
        if (request is null) return Results.NotFound();
        request.Update(dto);

        var isNowApproved = request.Status == RequestStatus.Approved;
        if (isNowApproved)
        {
          var projectDto = new ProjectCreateDto
          {
            Name = request.Title,
            Description = request.Description,
            ClientId = request.ClientId,
            AssignedUserId =
              dto.AssignedUserId
              ?? throw new Exception("AssignedUserId required to Create Project from Request"),
            StartDate = DateTime.UtcNow,
            DueDate = dto.DueDate,
            ProjectPriority = request.Priority,
            ProjectStatus = ProjectStatus.Pending
          };

          var project = ProjectModel.Create(projectDto);
          db.Projects.Add(project);
          await db.SaveChangesAsync();
          request.ProjectId = project.Id;
          request.ReviewedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return Results.Ok(request);
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Update request information")
    .WithDescription("Updates a request and returns the updated record.")
    .Produces<RequestModel>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

    group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
      {
        var request = await db.Requests.FindAsync(id);
        if (request is null) return Results.NotFound();

        db.Requests.Remove(request);
        await db.SaveChangesAsync();

        return Results.NoContent();
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin)
    )
    .WithSummary("Remove a request")
    .WithDescription("Removes a request and returns a 204 Response on success.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);
  }
}
