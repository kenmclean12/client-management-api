using api.DTOs.Jobs;
using api.DTOs.User;
using ModelClient = api.Models.Clients.Client;
using api.Models.Requests;
using api.Models.Projects;

namespace api.DTOs.Project;

public class ProjectResponseDto
{
  public int Id { get; set; }

  public string Name { get; set; } = null!;

  public string? Description { get; set; }

  public int ClientId { get; set; }

  public ModelClient Client { get; set; } = null!;

  public int AssignedUserId { get; set; }

  public UserResponseDto AssignedUser { get; set; } = null!;

  public List<JobResponseDto>? Jobs { get; set; }

  public DateTime StartDate { get; set; }

  public DateTime? DueDate { get; set; }

  public DateTime? EndDate { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  public RequestPriority ProjectPriority { get; set; } = RequestPriority.Low;

  public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Pending;
}
