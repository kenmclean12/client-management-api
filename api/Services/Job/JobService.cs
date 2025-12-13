using api.Data;
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
        return Results.Ok(await db.Jobs.ToListAsync());
      }
    )
    .RequireJwt()
    .WithSummary("Find all jobs")
    .WithDescription("Returns all job records")
    .Produces<List<ModelJob>>(StatusCodes.Status200OK);

    group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
      {
        var job = await db.Jobs.FindAsync(id);
        if (job is null) return Results.NotFound();

        return Results.Ok(job);
      }
    )
    .RequireJwt()
    .WithSummary("Find a job by ID")
    .WithDescription("Returns a job record")
    .Produces<ModelJob>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/", async (AppDbContext db, JobCreateDto dto) =>
      {
        var job = ModelJob.Create(dto);
        db.Jobs.Add(job);
        await db.SaveChangesAsync();
        return Results.Created($"job/{job.Id}", job);
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Create a new job")
    .WithDescription("Creates a job and returns the newly created record.")
    .Produces<ModelJob>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

    group.MapPut("/{id:int}", async (AppDbContext db, JobUpdateDto dto, int id) =>
      {
        var job = await db.Jobs.FindAsync(id);
        if (job is null) return Results.NotFound();

        job.Update(dto);
        await db.SaveChangesAsync();

        return Results.Ok(job);
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Update job information")
    .WithDescription("Updates a job and returns the newly created record.")
    .Produces<ModelJob>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
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
    .Produces(StatusCodes.Status404NotFound);
  }
}