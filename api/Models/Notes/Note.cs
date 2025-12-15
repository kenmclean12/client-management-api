using System.ComponentModel.DataAnnotations;
using api.DTOs.Note;

namespace api.Models.Notes;

public class Note
{
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }

    public int? ProjectId { get; set; }

    public int? JobId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public static Note Create(NoteCreateDto dto)
    {
        var note = new Note
        {
            Content = dto.Content,
            ClientId = dto.ClientId,
            CreatedAt = DateTime.UtcNow
        };

        if (dto.JobId is not null) note.JobId = dto.JobId;
        if (dto.ProjectId is not null) note.ProjectId = dto.ProjectId;

        return note;
    }

    public void Update(NoteUpdateDto dto)
    {
        UpdatedAt = DateTime.UtcNow;
        Content = dto.Content;
    }
}
