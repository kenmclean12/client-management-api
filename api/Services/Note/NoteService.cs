using api.Data;
using api.DTOs.Notes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ModelNote = api.Models.Notes.Note;

namespace api.Services.Note;

public static class NoteService
{
  public static async Task MapNoteEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/notes").WithTags("Note");

    group.MapGet("/", [Authorize] async (AppDbContext db) =>
    {
      return Results.Ok(await db.Notes.ToListAsync());
    }
    )
    .WithSummary("Find all notes")
    .WithDescription("Returns all contact records")
    .Produces<List<ModelNote>>(StatusCodes.Status200OK);

    group.MapGet("/{id:int}", [Authorize] async (AppDbContext db, int id) =>
    {
      var notes = await db.Notes.Where(n => n.ClientId == id).ToListAsync();
      if (notes.Count == 0) return Results.NotFound();

      return Results.Ok(notes);
    }
    )
    .WithSummary("Find all notes")
    .WithDescription("Returns all note records for a particular client")
    .Produces<List<ModelNote>>(StatusCodes.Status200OK)
    .Produces<List<ModelNote>>(StatusCodes.Status404NotFound);

    group.MapPost("/", [Authorize] async (AppDbContext db, NoteCreateDto dto) =>
    {
      var note = ModelNote.Create(dto);
      db.Notes.Add(note);
      await db.SaveChangesAsync();
      return Results.Created($"/note/{note.Id}", note);
    })
    .WithSummary("Create a new note")
    .WithDescription("Creates a note and returns the newly created record.")
    .Produces<ModelNote>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

    group.MapPut("/{id:int}", [Authorize] async (AppDbContext db, NoteUpdateDto dto, int id) =>
      {
        var note = await db.Notes.FindAsync(id);
        if (note is null) return Results.NotFound();

        note.Update(dto);
        await db.SaveChangesAsync();

        return Results.Ok(note);
      }
    )
    .WithSummary("Update a note by ID")
    .WithDescription("Updates a note and returns the newly created record.")
    .Produces<ModelNote>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);

    group.MapDelete("/{id:int}", [Authorize] async (AppDbContext db, int id) =>
    {
      var note = await db.Notes.FindAsync(id);
      if (note is null) return Results.NotFound();

      db.Notes.Remove(note);
      await db.SaveChangesAsync();

      return Results.NoContent();
    })
    .WithSummary("Delete a note by ID")
    .WithDescription("Delete a note and returns the newly created record.")
    .Produces<ModelNote>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status404NotFound);
  }
}