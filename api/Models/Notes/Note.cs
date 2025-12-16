using System.ComponentModel.DataAnnotations;
using api.DTOs.Note;
using api.Models.Users;

namespace api.Models.Notes;

public class Note
{
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }

    public int UserId { get; set; }

    [Required]
    public User User { get; set; } = null!;

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
            UserId = dto.UserId,
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

    public NoteResponseDto ToResponse()
    {
        var dto = new NoteResponseDto
        {
            Id = Id,
            Content = Content,
            ClientId = ClientId,
            UserId = UserId,
            User = User.ToResponse(),
            CreatedAt = CreatedAt,
        };

        if (ProjectId is not null) dto.ProjectId = ProjectId;
        if (JobId is not null) dto.JobId = JobId;
        if (UpdatedAt is not null) dto.UpdatedAt = UpdatedAt;

        return dto;
    }
}
