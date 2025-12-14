using api.Data;
using api.Helpers.Token;
using api.Models.Projects;
using api.Models.Users;
using api.Services.Email;
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
        var projects = await db.Projects.Include(p => p.Jobs).ToListAsync();
        var response = projects.Select(p => p.ToResponse());
        return Results.Ok(response);
      }
    )
    .RequireJwt()
    .WithSummary("Find all projects")
    .WithDescription("Returns all project records")
    .Produces<List<ProjectResponseDto>>(StatusCodes.Status200OK);

    group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
      {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return Results.NotFound();
        return Results.Ok(project.ToResponse());
      }
    )
    .RequireJwt()
    .WithSummary("Find a project by ID")
    .WithDescription("Returns a project record")
    .Produces<ProjectResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapGet("/client/{id:int}", async (AppDbContext db, int id) =>
      {
        var projects = await db.Projects
          .Where(p => p.ClientId == id)
          .Include(p => p.Jobs)
          .ToListAsync();

        if (projects is null) return Results.NotFound();
        var response = projects.Select((p) => p.ToResponse());
        return Results.Ok(response);
      }
    )
    .RequireJwt()
    .WithSummary("Find projects by Client ID")
    .WithDescription("Returns all projects for a particular Client")
    .Produces<List<ProjectResponseDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/", async (AppDbContext db, ProjectCreateDto dto) =>
      {
        var project = ModelProject.Create(dto);
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        return Results.Created($"/project/{project.Id}", project.ToResponse());
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Create a new project")
    .WithDescription("Creates a project and returns the newly created record.")
    .Produces<ProjectResponseDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

    group.MapPut("/{id:int}", async (AppDbContext db, ProjectUpdateDto dto, int id) =>
      {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return Results.NotFound();

        project.Update(dto);
        await db.SaveChangesAsync();

        // if (dto.ProjectStatus is not null && dto.ProjectStatus == ProjectStatus.Done)
        // {
        //    await EmailService.SendUserInviteAsync(
        //     email,
        //     inviteLink,
        //     token
        //   );
        // }

        return Results.Ok(project.ToResponse());
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Update project information")
    .WithDescription("Updates a project and returns the newly created record.")
    .Produces<ProjectResponseDto>(StatusCodes.Status200OK)
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