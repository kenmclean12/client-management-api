using api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ModelProject = api.Models.Projects.Project;

namespace api.Services.Project;

public static class ProjectService
{
  public static async Task MapProjectEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/project").WithTags("Project");

    group.MapGet("/", [Authorize] async (AppDbContext db) =>
      {
        return Results.Ok(await db.Projects.Include(p => p.Jobs).ToListAsync());
      }
    )
    .WithSummary("Find all projects")
    .WithDescription("Returns all project records")
    .Produces<List<ModelProject>>(StatusCodes.Status200OK);

    group.MapGet("/{id:int}", [Authorize] async (AppDbContext db, int id) =>
      {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return Results.NotFound();

        return Results.Ok(project);
      }
    )
    .WithSummary("Find a project by ID")
    .WithDescription("Returns a project record")
    .Produces<ModelProject>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapGet("/client/{id:int}", [Authorize] async (AppDbContext db, int id) =>
      {
        var project = await db.Projects.Where(p => p.ClientId == id).ToListAsync();
        if (project is null) return Results.NotFound();

        return Results.Ok(project);
      }
    )
    .WithSummary("Find projects by Client ID")
    .WithDescription("Returns all projects for a particular Client")
    .Produces<List<ModelProject>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/", [Authorize(Roles = "ADMIN,STANDARD")] async (AppDbContext db, ProjectCreateDto dto) =>
      {
        var project = new ModelProject
        {
          Name = dto.Name,
          ClientId = dto.ClientId,
          StartDate = dto.StartDate,
        };

        if (dto.Description is not null) project.Description = dto.Description;
        if (dto.EndDate is not null) project.EndDate = dto.EndDate;

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        return Results.Created($"/project/{project.Id}", project);
      }
    )
    .WithSummary("Create a new project")
    .WithDescription("Creates a project and returns the newly created record.")
    .Produces<ModelProject>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

    group.MapPatch("/{id:int}", [Authorize(Roles = "ADMIN,STANDARD")] async (AppDbContext db, ProjectUpdateDto dto, int id) =>
      {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return Results.NotFound();

        project.Update(dto);
        await db.SaveChangesAsync();

        return Results.Ok(project);
      }
    )
    .WithSummary("Update project information")
    .WithDescription("Updates a project and returns the newly created record.")
    .Produces<ModelProject>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

    group.MapDelete("/{id:int}", [Authorize(Roles = "ADMIN")] async (AppDbContext db, int id) =>
      {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return Results.NotFound();

        db.Projects.Remove(project);
        await db.SaveChangesAsync();

        return Results.NoContent();
      }
    )
    .WithSummary("Remove a project")
    .WithDescription("Removes a project and returns a 204 Response on success.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);
  }
}