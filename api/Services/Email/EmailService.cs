namespace api.Services.Email;

public interface IEmailService
{
    Task SendUserInviteAync(
      string toEmail,
      string inviteLink,
      CancellationToken token = default
    );
}