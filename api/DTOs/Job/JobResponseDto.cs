using api.DTOs.Client;
using api.DTOs.User;
using api.Models.Jobs;

namespace api.DTOs.Jobs;

public class JobResponseDto
{
  public int Id { get; set; }

  public string Name { get; set; } = null!;

  public string Description { get; set; } = null!;

  public int ClientId { get; set; }

  public ClientResponseDto Client { get; set; } = null!;

  public int? AssignedUserId { get; set; }

  public UserResponseDto? AssignedUser { get; set; }

  public JobStatus Status { get; set; } = JobStatus.Pending;

  public JobPriority Priority { get; set; } = JobPriority.Medium;

  public DateTime DueDate { get; set; }

  public int ProjectId { get; set; }

  public DateTime? EstimatedFinish { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  public DateTime? CompletedAt { get; set; }
}