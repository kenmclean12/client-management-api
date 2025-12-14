namespace api.Services.Email;

public interface IEmailService
{
    Task SendUserInviteAsync(
      string toEmail,
      string inviteLink,
      CancellationToken token = default
    );

    Task SendProjectFinishedAsync(
      string toEmail,
      string taskName,
      string taskDescription,
      DateTime taskDueDate,
      DateTime taskFinishDate,
      CancellationToken token = default
    );
}