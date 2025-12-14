using System.ComponentModel.DataAnnotations;
using api.Models.Projects;

public class ProjectUpdateDto
{
    [MaxLength(150)]
    public string? Name { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int? ClientId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public RequestPriority? ProjectPriority { get; set; }

    [Required]
    public ProjectStatus? ProjectStatus { get; set; }
}
