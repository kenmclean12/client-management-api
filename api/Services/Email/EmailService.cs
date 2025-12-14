
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace api.Services.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendUserInviteAsync(string toEmail, string inviteLink, CancellationToken token = default)
    {
        var message = new MimeMessage();
        message.From.Add(
          new MailboxAddress(
            _config["Email:FromName"],
            _config["Email:FromAddress"]
          )
        );

        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Client Management User Invite";

        var html = await LoadTemplateAsync("UserInvite.html");
        html = html
          .Replace("{{InviteLink}}", inviteLink)
          .Replace("{{AppName}}", _config["App:Name"]);

        message.Body = new BodyBuilder
        {
            HtmlBody = html
        }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(
          _config["Email:SmtpHost"],
          int.Parse(_config["Email:SmtpPort"]!),
          SecureSocketOptions.StartTls,
          token
        );

        await client.AuthenticateAsync(
          _config["Email:Username"],
          Environment.GetEnvironmentVariable("SENDGRID_API_KEY"),
          token
        );

        await client.SendAsync(message, token);
        await client.DisconnectAsync(true, token);
    }

    public async Task SendProjectFinishedAsync(string toEmail, string taskName, string taskDescription, DateTime taskStartDate, DateTime? taskDueDate, DateTime? taskFinishDate, CancellationToken token = default)
    {
        var message = new MimeMessage();
        message.From.Add(
          new MailboxAddress(
            _config["Email:FromName"],
            _config["Email:FromAddress"]
          )
        );

        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = $"Job Completed: {taskName}";

        var html = await LoadTemplateAsync("ProjectComplete.html");
        var dueDate = taskDueDate.HasValue ? taskDueDate.Value.ToString("MMMM d, yyyy") : "n/a";
        var finishDate = taskFinishDate.HasValue ? taskFinishDate.Value.ToString("MMMM d, yyyy") : "n/a";
        html = html
          .Replace("{{TaskName}}", taskName)
          .Replace("{{TaskDescription}}", taskDescription)
          .Replace("{{TaskStartDate}}", taskStartDate.ToString("MMMM d, yyyy"))
          .Replace("{{TaskDueDate}}", dueDate)
          .Replace("{{TaskFinishDate}}", finishDate)
          .Replace("{{AppFrontendUrl}}", _config["App:FrontendUrl"])
          .Replace("{{AppName}}", _config["App:Name"]);

        message.Body = new BodyBuilder
        {
            HtmlBody = html
        }.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(
          _config["Email:SmtpHost"],
          int.Parse(_config["Email:SmtpPort"]!),
          SecureSocketOptions.StartTls,
          token
        );

        await client.AuthenticateAsync(
          _config["Email:Username"],
          Environment.GetEnvironmentVariable("SENDGRID_API_KEY"),
          token
        );

        await client.SendAsync(message, token);
        await client.DisconnectAsync(true, token);
    }

    public static async Task<string> LoadTemplateAsync(string name)
    {
        var path = Path.Combine(
          AppContext.BaseDirectory,
          "Services",
          "Email",
          "Templates",
          name
        );

        return await File.ReadAllTextAsync(path);
    }
}