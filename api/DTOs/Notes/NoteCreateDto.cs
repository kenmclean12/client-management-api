using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Notes;

public class NoteCreateDto
{
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }
}