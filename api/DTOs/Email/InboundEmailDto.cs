namespace api.DTOs.Email;

public class InboundEmailDto
{
  public string From { get; set; } = null!;
  public string Subject { get; set; } = null!;
  public string Text { get; set; } = null!;
  public string Html { get; set; } = null!;
}