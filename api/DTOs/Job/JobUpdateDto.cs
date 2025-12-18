using System.ComponentModel.DataAnnotations;
using api.Models.Jobs;

namespace api.DTOs.Job;

public class JobUpdateDto
{
  [MaxLength(100)]
  public string? Name { get; set; }

  [MaxLength(1000)]
  public string? Description { get; set; }

  public int? ClientId { get; set; }

  public int? AssignedUserId { get; set; }

  public int? ProjectId { get; set; }

  public JobStatus? Status { get; set; }

  public JobPriority? Priority { get; set; }

  public DateTime? DueDate { get; set; }

  public DateTime? EstimatedFinish { get; set; }

  public DateTime? CompletedAt { get; set; }
}
