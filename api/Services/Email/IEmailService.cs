namespace api.Services.Email;

public interface IEmailService
{
  Task SendUserInviteAsync(
    string toEmail,
    string inviteLink,
    IWebHostEnvironment env,
    CancellationToken token = default
  );

  Task SendProjectStartedAsync(
    string toEmail,
    string taskName,
    string taskDescription,
    DateTime taskStartDate,
    DateTime? taskDueDate,
    IWebHostEnvironment env,
    CancellationToken token = default
  );

  Task SendProjectFinishedAsync(
    string toEmail,
    string taskName,
    string taskDescription,
    DateTime taskStartDate,
    DateTime? taskDueDate,
    DateTime? taskFinishDate,
    IWebHostEnvironment env,
    CancellationToken token = default
  );
}