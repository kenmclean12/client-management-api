namespace api.Services.Email;

public interface IEmailService
{
    Task SendUserInviteAsync(
      string toEmail,
      string inviteLink,
      CancellationToken token = default
    );
}