using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Request;

public class RequestCreateDto
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(4000)]
    public string Description { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }

    public RequestPriority Priority { get; set; } = RequestPriority.Normal;

    public int? ProjectId { get; set; }

    public int? JobId { get; set; }
}
