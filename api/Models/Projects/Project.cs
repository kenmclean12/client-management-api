using System.ComponentModel.DataAnnotations;
using api.DTOs.Project;
using api.Models.Clients;
using api.Models.Jobs;
using api.Models.Request;
using api.Models.Users;

namespace api.Models.Projects;

public class Project
{
  public int Id { get; set; }

  [Required]
  [MaxLength(150)]
  public string Name { get; set; } = null!;

  [MaxLength(2000)]
  public string? Description { get; set; }

  [Required]
  public int ClientId { get; set; }

  public Client Client { get; set; } = null!;

  [Required]
  public int AssignedUserId { get; set; }

  public User AssignedUser { get; set; } = null!;

  public List<Job>? Jobs { get; set; }

  [Required]
  public DateTime StartDate { get; set; }

  public DateTime? DueDate { get; set; }

  public DateTime? EndDate { get; set; }

  [Required]
  public RequestPriority ProjectPriority { get; set; } = RequestPriority.Low;

  [Required]
  public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Pending;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  public static Project Create(ProjectCreateDto dto)
  {
    var project = new Project
    {
      Name = dto.Name,
      ClientId = dto.ClientId,
      AssignedUserId = dto.AssignedUserId,
      StartDate = dto.StartDate,
      ProjectPriority = dto.ProjectPriority,
      ProjectStatus = dto.ProjectStatus,
    };

    if (dto.Description is not null) project.Description = dto.Description;
    if (dto.EndDate is not null) project.EndDate = dto.EndDate;
    if (dto.DueDate is not null) project.DueDate = dto.DueDate;

    return project;
  }

  public void Update(ProjectUpdateDto dto)
  {
    UpdatedAt = DateTime.UtcNow;
    if (dto.Name is not null) Name = dto.Name;
    if (dto.ClientId is int clientId) ClientId = clientId;
    if (dto.AssignedUserId is int assignedUserId) AssignedUserId = assignedUserId;
    if (dto.Description is not null) Description = dto.Description;
    if (dto.StartDate is DateTime startDate) StartDate = startDate;
    if (dto.DueDate is DateTime dueDate) DueDate = dueDate;
    if (dto.ProjectPriority is RequestPriority projectPriority) ProjectPriority = projectPriority;
    if (dto.ProjectStatus is ProjectStatus projectStatus) ProjectStatus = projectStatus;
  }

  public ProjectResponseDto ToResponse()
  {
    var response = new ProjectResponseDto
    {
      Id = Id,
      Name = Name,
      ClientId = ClientId,
      Client = Client,
      AssignedUserId = AssignedUserId,
      AssignedUser = AssignedUser.ToResponse(),
      StartDate = StartDate,
      CreatedAt = CreatedAt,
      ProjectPriority = ProjectPriority,
      ProjectStatus = ProjectStatus,
    };

    if (Jobs is not null) response.Jobs = [.. Jobs.Select(j => j.ToResponse())];
    if (Description is not null) response.Description = Description;
    if (DueDate is DateTime dueDate) response.DueDate = dueDate;
    if (EndDate is DateTime endDate) response.EndDate = endDate;
    if (UpdatedAt is not null) response.UpdatedAt = UpdatedAt;

    return response;
  }
}
