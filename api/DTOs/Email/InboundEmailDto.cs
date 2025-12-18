using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Email;

public class InboundEmailDto
{
  [Required]
  [EmailAddress]
  public string From { get; set; } = null!;

  [Required]
  public string Subject { get; set; } = null!;

  [Required]
  public string Text { get; set; } = null!;

  public string Html { get; set; } = null!;
}
