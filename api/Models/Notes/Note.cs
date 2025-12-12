using System.ComponentModel.DataAnnotations;
using api.DTOs.Notes;
using api.Models.Clients;

namespace api.Models.Notes;

public class Note
{
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public static Note Create(NoteCreateDto dto)
    {
        return new Note
        {
            Content = dto.Content,
            ClientId = dto.ClientId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(NoteUpdateDto dto)
    {
        UpdatedAt = DateTime.UtcNow;
        if (dto.Content is not null) Content = dto.Content;
        if (dto.ClientId is int clientId) ClientId = clientId;
    }
}
