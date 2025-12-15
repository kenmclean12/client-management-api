using System.ComponentModel.DataAnnotations;
using api.DTOs.Request;
using api.Models.Clients;

namespace api.Models.Requests;

public class Request
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = null!;

    [Required]
    [MaxLength(4000)]
    public string Description { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public RequestStatus Status { get; set; } = RequestStatus.New;

    public RequestPriority Priority { get; set; } = RequestPriority.Normal;

    public int? AssignedUserId { get; set; }
    public int? ProjectId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }

    public static Request Create(RequestCreateDto dto)
    {
        var request = new Request
        {
            Title = dto.Title,
            Description = dto.Description,
            ClientId = dto.ClientId,
            Priority = dto.Priority,
            CreatedAt = DateTime.UtcNow
        };

        if (dto.ProjectId is int projectId) request.ProjectId = projectId;

        return request;
    }

    public void Update(RequestUpdateDto dto)
    {
        if (dto.Title is not null) Title = dto.Title;
        if (dto.Description is not null) Description = dto.Description;
        if (dto.ClientId is int clientId) ClientId = clientId;
        if (dto.Status is RequestStatus status) Status = status;
        if (dto.Priority is RequestPriority priority) Priority = priority;
        if (dto.AssignedUserId is int userId) AssignedUserId = userId;
        if (dto.ProjectId is int projectId) ProjectId = projectId;
        if (dto.ReviewedAt is DateTime reviewedAt) ReviewedAt = reviewedAt;
    }
}
