using api.Data;
using api.DTOs.Project;
using api.Helpers.Token;
using api.Models.Projects;
using api.Models.Users;
using api.Services.Email;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Project;

public static class ProjectService
{
  public static async Task MapProjectEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/project").WithTags("Project");

    group.MapGet("/", async (AppDbContext db) =>
      {
        return Results.Ok(await db.Projects
          .Where((p) =>
            p.EndDate == null
            && p.ProjectStatus != ProjectStatus.Done
          )
          .Include(p => p.Jobs)
          .Include(p => p.Client)
          .Include(p => p.AssignedUser)
          .Select(p => p.ToResponse())
          .ToListAsync()
        );
      }
    )
    .RequireJwt()
    .WithSummary("Find all active projects")
    .WithDescription("Returns all active project records")
    .Produces<List<ProjectResponseDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

    group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
      {
        var project = await db.Projects
          .Include(p => p.Jobs)
          .Include(p => p.Client)
          .Include((p) => p.AssignedUser)
          .FirstOrDefaultAsync(p => p.Id == id);

        if (project is null) return Results.NotFound();
        return Results.Ok(project.ToResponse());
      }
    )
    .RequireJwt()
    .WithSummary("Find a project by ID")
    .WithDescription("Returns a project record")
    .Produces<ProjectResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);

    group.MapGet("/user/{id:int}", async (AppDbContext db, int id) =>
      {
        return Results.Ok(
          await db.Projects
            .Where(p => p.AssignedUserId == id)
            .Include(p => p.Jobs)
            .Include((p) => p.AssignedUser)
            .Include(p => p.Client)
            .Select(p => p.ToResponse())
            .ToListAsync()
        );
      }
    )
    .RequireJwt()
    .WithSummary("Find assigned Projects by User ID")
    .WithDescription("Returns assigned projects for a particular user")
    .Produces<List<ProjectResponseDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

    group.MapGet("/client/{id:int}", async (AppDbContext db, int id) =>
      {
        return Results.Ok(
          await db.Projects
            .Where(p =>
              p.ClientId == id
              && p.EndDate == null
            )
            .Include(p => p.Jobs)
            .Include(p => p.AssignedUser)
            .Include(p => p.Client)
            .Select(p => p.ToResponse())
            .ToListAsync()
        );
      }
    )
    .RequireJwt()
    .WithSummary("Find projects by Client ID")
    .WithDescription("Returns all projects for a particular Client")
    .Produces<List<ProjectResponseDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

    group.MapPut("/{id:int}",
      async (
        AppDbContext db,
        ProjectUpdateDto dto,
        int id,
        CancellationToken token,
        IEmailService emailService
      ) =>
      {
        var project = await db.Projects.FindAsync(id);
        if (project is null) return Results.NotFound();
        project.Update(dto);

        if (dto.ProjectStatus is not null && dto.ProjectStatus == ProjectStatus.Done)
        {
          project.EndDate = DateTime.UtcNow;
          var contacts = await db.Contacts.Where(c => c.ClientId == project.ClientId).ToListAsync(token);
          foreach (var contact in contacts)
          {
            await emailService.SendProjectFinishedAsync(
              contact.Email,
              project.Name,
              project.Description ?? "n/a",
              project.StartDate,
              project.DueDate,
              project.EndDate,
              token
            );
          }
        }

        await db.SaveChangesAsync(token);
        await db.Entry(project).Reference((p) => p.AssignedUser).LoadAsync(token);
        await db.Entry(project).Reference((p) => p.Client).LoadAsync(token);

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
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden);

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
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status404NotFound);
  }
}