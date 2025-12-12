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

  public JobStatus Status { get; set; } = JobStatus.Pending;

  public JobPriority Priority { get; set; } = JobPriority.Medium;

  [Required]
  public DateTime DueDate { get; set; }

  public int? ProjectId { get; set; }

  public Project? Project { get; set; }

  public DateTime? EstimatedFinish { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  public DateTime? CompletedAt { get; set; }

  public static Job Create(JobCreateDto dto)
  {
    var job = new Job
    {
      Name = dto.Name,
      Description = dto.Description,
      ClientId = dto.ClientId,
      Status = dto.Status,
      Priority = dto.Priority,
      DueDate = dto.DueDate,
      CreatedAt = DateTime.UtcNow,
    };

    if (dto.ProjectId is not null) job.ProjectId = dto.ProjectId;
    if (dto.EstimatedFinish is not null) job.EstimatedFinish = dto.EstimatedFinish;

    return job;
  }

  public void Update(JobUpdateDto dto)
  {
    UpdatedAt = DateTime.UtcNow;

    if (dto.Name is not null) Name = dto.Name;
    if (dto.Description is not null) Description = dto.Description;
    if (dto.ClientId is int clientId) ClientId = clientId;
    if (dto.ProjectId is int projectId) ProjectId = projectId;
    if (dto.Status is JobStatus jobStatus) Status = jobStatus;
    if (dto.Priority is JobPriority jobPriority) Priority = jobPriority;
    if (dto.DueDate is DateTime dueDate) DueDate = dueDate;
    if (dto.EstimatedFinish is DateTime estimatedFinish) EstimatedFinish = estimatedFinish;
    if (dto.CompletedAt is DateTime completedAt) CompletedAt = completedAt;
  }
}