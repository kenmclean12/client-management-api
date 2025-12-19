using api.Data;
using api.DTOs.Request;
using api.Models.Requests;
using Microsoft.EntityFrameworkCore;
using RequestModel = api.Models.Requests.Request;

namespace api.Services.InboundEmail;

public static class InboundEmailService
{
  public static async Task MapInboundEmailEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/inbound-emails");
    group.MapPost("/email/inbound", async (
        AppDbContext db,
        HttpRequest request
    ) =>
    {
      var form = await request.ReadFormAsync();
      var from = form["from"].ToString();
      var subject = string.IsNullOrWhiteSpace(form["subject"])
        ? "(No subject)"
        : form["subject"].ToString();
      var body = string.IsNullOrWhiteSpace(form["text"])
          ? "(No message body)"
          : form["text"].ToString();

      var fromEmail = from.Trim().ToLowerInvariant();
      var contact = await db.Contacts.FirstOrDefaultAsync(c => fromEmail.Contains(c.Email.ToLower()));
      if (contact is null) return Results.BadRequest("Unknown contact");

      var client = await db.Clients.FindAsync(contact.ClientId);
      if (client is null) return Results.BadRequest("Unknown client");

      var requestModel = new RequestModel
      {
        Title = subject.Length > 150
          ? subject[..150]
          : subject,
        Description = body,
        ClientId = client.Id,
        Priority = RequestPriority.Normal,
        Status = RequestStatus.New
      };

      db.Requests.Add(requestModel);
      await db.SaveChangesAsync();
      await db.Entry(requestModel).Reference(r => r.Client).LoadAsync();

      return Results.Ok(requestModel.ToResponse());
    })
    .AllowAnonymous()
    .WithTags("Inbound Email")
    .WithSummary("Receive inbound email from clients")
    .WithDescription("Processes incoming emails from clients and creates a new request automatically.")
    .Produces<RequestResponseDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);
  }
}