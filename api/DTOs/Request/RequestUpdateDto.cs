using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Request;

public class RequestUpdateDto
{
    [MaxLength(150)]
    public string? Title { get; set; }

    [MaxLength(4000)]
    public string? Description { get; set; }

    public int? ClientId { get; set; }

    public RequestStatus? Status { get; set; }

    public DateTime? DueDate { get; set; }

    public RequestPriority? Priority { get; set; }

    public int? AssignedUserId { get; set; }

    public int? ProjectId { get; set; }

    public DateTime? ReviewedAt { get; set; }
}
