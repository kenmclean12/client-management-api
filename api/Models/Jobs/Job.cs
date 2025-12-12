using System.ComponentModel.DataAnnotations;
using api.Models.Clients;
using api.Models.Projects;

namespace api.Models.Jobs;

public class Job
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

  public Client Client { get; set; } = null!;

  public int? ProjectId { get; set; }

  public Project? Project { get; set; }

  public JobStatus Status { get; set; } = JobStatus.Pending;

  public JobPriority Priority { get; set; } = JobPriority.Medium;

  [Required]
  public DateTime DueDate { get; set; }
  public DateTime? EstimatedFinish { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

   public DateTime? CompletedAt { get; set; }
}