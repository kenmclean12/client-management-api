using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Notes;

public class NoteUpdateDto
{
    [MaxLength(1000)]
    public string? Content { get; set; }

    public int? ClientId { get; set; }
}