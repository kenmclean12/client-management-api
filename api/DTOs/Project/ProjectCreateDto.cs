using System.ComponentModel.DataAnnotations;
using api.Models.Projects;
using api.Models.Request;

namespace api.DTOs.Project;

public class ProjectCreateDto
{
  [Required]
  [MaxLength(150)]
  public string Name { get; set; } = null!;

  [MaxLength(2000)]
  public string? Description { get; set; }

  [Required]
  public int ClientId { get; set; }

  [Required]
  public int AssignedUserId { get; set; }

  [Required]
  public DateTime StartDate { get; set; }

  public DateTime? DueDate { get; set; }

  public DateTime? EndDate { get; set; }

  [Required]
  public RequestPriority ProjectPriority { get; set; } = RequestPriority.Low;

  [Required]
  public ProjectStatus ProjectStatus { get; set; } = ProjectStatus.Pending;
}
