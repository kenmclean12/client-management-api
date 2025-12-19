using System.ComponentModel.DataAnnotations;
using api.Models.Projects;
using api.Models.Requests;

namespace api.DTOs.Project;

public class ProjectUpdateDto
{
  [MaxLength(150)]
  public string? Name { get; set; }

  [MaxLength(2000)]
  public string? Description { get; set; }

  public int? ClientId { get; set; }

  public int? AssignedUserId { get; set; }

  public DateTime? StartDate { get; set; }

  public DateTime? DueDate { get; set; }

  public RequestPriority? ProjectPriority { get; set; }

  [Required]
  public ProjectStatus? ProjectStatus { get; set; }
}
