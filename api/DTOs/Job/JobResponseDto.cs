using System.ComponentModel.DataAnnotations;
using api.DTOs.User;
using api.Models.Jobs;
using ModelClient = api.Models.Clients.Client;

namespace api.DTOs.Jobs;

public class JobResponseDto
{
  public int Id { get; set; }

  [Required]
  [MaxLength(100)]
  public string Name { get; set; } = null!;

  [Required]
  [MaxLength(1000)]
  public string Description { get; set; } = null!;

  [Required]
  public int ClientId { get; set; }

  public ModelClient Client { get; set; } = null!;

  public int? AssignedUserId { get; set; }

  public UserResponseDto? AssignedUser { get; set;}

  public JobStatus Status { get; set; } = JobStatus.Pending;

  public JobPriority Priority { get; set; } = JobPriority.Medium;

  [Required]
  public DateTime DueDate { get; set; }

  [Required]
  public int ProjectId { get; set; }

  public DateTime? EstimatedFinish { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  public DateTime? CompletedAt { get; set; }
}