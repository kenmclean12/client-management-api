using api.Data;
using api.Helpers.Token;
using api.Models.Users;
using Microsoft.EntityFrameworkCore;
using ModelProject = api.Models.Projects.Project;

namespace api.Services.Project;

public static class ProjectService
{
  public static async Task MapProjectEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/project").WithTags("Project");

    group.MapGet("/", async (AppDbContext db) =>
      {
        return Results.Ok(await db.Projects.Include(p => p.Jobs).ToListAsync());
      }
    )
    .RequireJwt()
    .WithSummary("Find all projects")
    .WithDescription("Returns all project records")
    .Produces<List<ModelProject>>(StatusCodes.Status200OK);

    group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
      {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return Results.NotFound();

        return Results.Ok(project);
      }
    )
    .RequireJwt()
    .WithSummary("Find a project by ID")
    .WithDescription("Returns a project record")
    .Produces<ModelProject>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapGet("/client/{id:int}", async (AppDbContext db, int id) =>
      {
        var projects = await db.Projects
          .Where(p => p.ClientId == id)
          .Include(p => p.Jobs)
          .ToListAsync();

        if (projects is null) return Results.NotFound();

        return Results.Ok(projects);
      }
    )
    .RequireJwt()
    .WithSummary("Find projects by Client ID")
    .WithDescription("Returns all projects for a particular Client")
    .Produces<List<ModelProject>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/", async (AppDbContext db, ProjectCreateDto dto) =>
      {
        var project = ModelProject.Create(dto);
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        return Results.Created($"/project/{project.Id}", project);
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Create a new project")
    .WithDescription("Creates a project and returns the newly created record.")
    .Produces<ModelProject>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

    group.MapPut("/{id:int}", async (AppDbContext db, ProjectUpdateDto dto, int id) =>
      {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return Results.NotFound();

        project.Update(dto);
        await db.SaveChangesAsync();

        return Results.Ok(project);
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Update project information")
    .WithDescription("Updates a project and returns the newly created record.")
    .Produces<ModelProject>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

    group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
      {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return Results.NotFound();

        db.Projects.Remove(project);
        await db.SaveChangesAsync();

        return Results.NoContent();
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin)
    )
    .WithSummary("Remove a project")
    .WithDescription("Removes a project and returns a 204 Response on success.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);
  }
}