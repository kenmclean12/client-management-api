using api.DTOs.User;

namespace api.DTOs.Note;

public class NoteResponseDto
{
  public int Id { get; set; }

  public string Content { get; set; } = null!;

  public int ClientId { get; set; }

  public int UserId { get; set; }

  public UserResponseDto User { get; set; } = null!;

  public int? ProjectId { get; set; }

  public int? JobId { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }
}
