using api.Data;
using api.DTOs.Job;
using api.DTOs.Jobs;
using api.Helpers.Token;
using api.Models.Users;
using Microsoft.EntityFrameworkCore;
using ModelJob = api.Models.Jobs.Job;

namespace api.Services.Job;

public static class JobService
{
  public static async Task MapJobEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/job").WithTags("Job");

    group.MapGet("/", async (AppDbContext db) =>
      {
        return Results.Ok(
          await db.Jobs
            .Include(j => j.Client)
            .Include(j => j.AssignedUser)
            .Select(j => j.ToResponse())
            .ToListAsync()
        );
      }
    )
    .RequireJwt()
    .WithSummary("Find all jobs")
    .WithDescription("Returns all job records")
    .Produces<List<JobResponseDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

    group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
      {
        var job = await db.Jobs
          .Include(j => j.Client)
          .Include(j => j.AssignedUser)
          .FirstOrDefaultAsync(j => j.Id == id);

        if (job is null) return Results.NotFound();

        return Results.Ok(job.ToResponse());
      }
    )
    .RequireJwt()
    .WithSummary("Find a job by ID")
    .WithDescription("Returns a job record")
    .Produces<JobResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);

    group.MapGet("/user/{id:int}", async (AppDbContext db, int id) =>
      {
        return Results.Ok(
          await db.Jobs
            .Include(j => j.Client)
            .Include(j => j.AssignedUser)
            .Where(j => j.AssignedUserId == id)
            .Select(j => j.ToResponse())
            .ToListAsync()
        );
      }
    )
    .RequireJwt()
    .WithSummary("Find assigned jobs by User ID")
    .WithDescription("Returns job records assigned to a particular user")
    .Produces<List<JobResponseDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

    group.MapPost("/", async (AppDbContext db, JobCreateDto dto) =>
      {
        var job = ModelJob.Create(dto);

        db.Jobs.Add(job);
        await db.SaveChangesAsync();
        await db.Entry(job).Reference(j => j.Client).LoadAsync();
        await db.Entry(job).Reference(j => j.AssignedUser).LoadAsync();

        return Results.Created($"job/{job.Id}", job.ToResponse());
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Create a new job")
    .WithDescription("Creates a job and returns the newly created record.")
    .Produces<JobResponseDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status400BadRequest);

    group.MapPut("/{id:int}", async (AppDbContext db, JobUpdateDto dto, int id) =>
      {
        var job = await db.Jobs.FindAsync(id);
        if (job is null) return Results.NotFound();

        job.Update(dto);
        await db.SaveChangesAsync();
        await db.Entry(job).Reference(j => j.Client).LoadAsync();
        await db.Entry(job).Reference(j => j.AssignedUser).LoadAsync();

        return Results.Ok(job.ToResponse());
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Update job information")
    .WithDescription("Updates a job and returns the newly created record.")
    .Produces<JobResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status404NotFound);

    group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
      {
        var job = await db.Jobs.FindAsync(id);
        if (job is null) return Results.NotFound(id);

        db.Jobs.Remove(job);
        await db.SaveChangesAsync();

        return Results.NoContent();
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin)
    )
    .WithSummary("Remove a job")
    .WithDescription("Removes a job and returns a 204 Response on success.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status404NotFound);
  }
}