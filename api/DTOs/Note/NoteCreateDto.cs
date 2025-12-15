using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Note;

public class NoteCreateDto
{
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }

    public int? ProjectId { get; set; }

    public int? JobId { get; set; }
}