using System.ComponentModel.DataAnnotations;
using api.Models.Jobs;

public class JobCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }

    public JobStatus Status { get; set; } = JobStatus.Stopped;

    public JobPriority Priority { get; set; } = JobPriority.Low;

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public int ProjectId { get; set; }

    public DateTime? EstimatedFinish { get; set; }
}
