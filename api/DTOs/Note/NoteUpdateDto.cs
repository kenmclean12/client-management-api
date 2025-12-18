using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Note;

public class NoteUpdateDto
{
  [Required]
  [MaxLength(1000)]
  public string Content { get; set; } = null!;
}