using System.ComponentModel.DataAnnotations;
using api.DTOs.Jobs;
using api.Models.Clients;
using api.Models.Projects;
using api.Models.Users;

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

  public int? AssignedUserId { get; set; }

  public User? AssignedUser { get; set; }

  public JobStatus Status { get; set; } = JobStatus.Pending;

  public JobPriority Priority { get; set; } = JobPriority.Medium;

  [Required]
  public DateTime DueDate { get; set; }

  [Required]
  public int ProjectId { get; set; }

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
      ProjectId = dto.ProjectId,
    };

    if (dto.EstimatedFinish is not null) job.EstimatedFinish = dto.EstimatedFinish;
    if (dto.AssignedUserId is not null) job.AssignedUserId = dto.AssignedUserId;

    return job;
  }

  public void Update(JobUpdateDto dto)
  {
    UpdatedAt = DateTime.UtcNow;

    if (dto.Name is not null) Name = dto.Name;
    if (dto.Description is not null) Description = dto.Description;
    if (dto.ClientId is int clientId) ClientId = clientId;
    if (dto.AssignedUserId is int assignedUserId) AssignedUserId = assignedUserId;
    if (dto.ProjectId is int projectId) ProjectId = projectId;
    if (dto.Status is JobStatus jobStatus) Status = jobStatus;
    if (dto.Priority is JobPriority jobPriority) Priority = jobPriority;
    if (dto.DueDate is DateTime dueDate) DueDate = dueDate;
    if (dto.EstimatedFinish is DateTime estimatedFinish) EstimatedFinish = estimatedFinish;
    if (dto.CompletedAt is DateTime completedAt) CompletedAt = completedAt;
  }

  public JobResponseDto ToResponse()
  {
    var response = new JobResponseDto
    {
      Name = Name,
      Description = Description,
      ClientId = ClientId,
      Client = Client,
      Status = Status,
      Priority = Priority,
      DueDate = DueDate,
      ProjectId = ProjectId,
      CreatedAt = CreatedAt,
    };

    if (AssignedUserId is not null) response.AssignedUserId = AssignedUserId;
    if (AssignedUser is not null) response.AssignedUser = AssignedUser.ToResponse();
    if (EstimatedFinish is not null) response.EstimatedFinish = EstimatedFinish;
    if (UpdatedAt is not null) response.UpdatedAt = UpdatedAt;
    if (CompletedAt is not null) response.CompletedAt = CompletedAt;

    return response;
  }
}