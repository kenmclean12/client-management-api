using System.ComponentModel.DataAnnotations;
using api.DTOs.User;

public class NoteResponseDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }

    [Required]
    public int UserId { get; set; }

    public UserResponseDto User { get; set; } = null!;

    public int? ProjectId { get; set; }

    public int? JobId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
