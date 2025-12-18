using api.Data;
using api.DTOs.Contact;
using api.DTOs.Contacts;
using api.Helpers.Token;
using api.Models.Users;
using Microsoft.EntityFrameworkCore;
using ModelContact = api.Models.Contacts.Contact;
namespace api.Services.Contact;

public static class ContactService
{
  public static async Task MapContactEndPoints(this WebApplication app)
  {
    var group = app.MapGroup("/contact").WithTags("Contact");

    group.MapGet("/", async (AppDbContext db) =>
      {
        return Results.Ok(
          await db.Contacts
            .Include(c => c.Client)
            .Select(c => c.ToResponse())
            .ToListAsync()
        );
      }
    )
    .RequireJwt()
    .WithSummary("Find all contacts")
    .WithDescription("Returns all contact records")
    .Produces<List<ContactResponseDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

    group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
      {
        var contact = await db.Contacts
          .Include(c => c.Client)
          .FirstOrDefaultAsync(c => c.Id == id);

        if (contact is null) return Results.NotFound();

        return Results.Ok(contact.ToResponse());
      }
    )
    .RequireJwt()
    .WithSummary("Find a contact by ID")
    .WithDescription("Returns a contact record")
    .Produces<ContactResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);

    group.MapGet("/client/{id:int}", async (AppDbContext db, int id) =>
      {
        return Results.Ok(
          await db.Contacts
            .Where(c => c.ClientId == id)
            .Include(c => c.Client)
            .Select(c => c.ToResponse())
            .ToListAsync()
        );
      }
    )
    .RequireJwt()
    .WithSummary("Find all contacts by Client ID")
    .WithDescription("Returns all client records for a particular client")
    .Produces<List<ContactResponseDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status404NotFound);

    group.MapPost("/", async (AppDbContext db, ContactCreateDto dto) =>
      {
        var contact = ModelContact.Create(dto);

        db.Contacts.Add(contact);
        await db.SaveChangesAsync();
        await db.Entry(contact).Reference(c => c.Client).LoadAsync();

        return Results.Created($"contact/{contact.Id}", contact.ToResponse());
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Create a new contact")
    .WithDescription("Creates a contact and returns the newly created record.")
    .Produces<ContactResponseDto>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden);

    group.MapPut("/{id:int}", async (AppDbContext db, ContactUpdateDto dto, int id) =>
      {
        var contact = await db.Contacts.FindAsync(id);
        if (contact is null) return Results.NotFound();

        contact.Update(dto);
        await db.SaveChangesAsync();
        await db.Entry(contact).Reference(c => c.Client).LoadAsync();

        return Results.Ok(contact.ToResponse());
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin),
      nameof(UserRole.Standard)
    )
    .WithSummary("Update contact information")
    .WithDescription("Updates a contact and returns the newly created record.")
    .Produces<ContactResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status400BadRequest);

    group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
      {
        var contact = await db.Contacts.FindAsync(id);
        if (contact is null) return Results.NotFound();

        db.Contacts.Remove(contact);
        await db.SaveChangesAsync();

        return Results.NoContent();
      }
    )
    .RequireJwt(
      nameof(UserRole.Admin)
    )
    .WithSummary("Remove a contact")
    .WithDescription("Removes a contact and returns a 204 Response on success.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status401Unauthorized)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status404NotFound);
  }
}