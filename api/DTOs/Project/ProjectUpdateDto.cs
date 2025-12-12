using System.ComponentModel.DataAnnotations;

public class ProjectUpdateDto
{
    [MaxLength(150)]
    public string? Name { get; set; }

    [MaxLength(2000)]
    public string? Description { get; set; }

    public int? ClientId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}
