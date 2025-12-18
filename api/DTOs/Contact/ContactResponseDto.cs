using api.DTOs.Client;

namespace api.DTOs.Contact;

public class ContactResponseDto
{
  public int Id { get; set; }

  public string Name { get; set; } = null!;

  public string Email { get; set; } = null!;

  public string? Phone { get; set; }

  public int ClientId { get; set; }

  public ClientResponseDto Client { get; set; } = null!;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }
}