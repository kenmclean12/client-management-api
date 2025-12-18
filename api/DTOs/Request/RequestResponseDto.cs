using System.ComponentModel.DataAnnotations;
using api.DTOs.Client;
using api.Models.Request;

namespace api.DTOs.Request;

public class RequestResponseDto
{
  public int Id { get; set; }

  [Required]
  [MaxLength(150)]
  public string Title { get; set; } = null!;

  [Required]
  [MaxLength(4000)]
  public string Description { get; set; } = null!;

  [Required]
  public int ClientId { get; set; }
  public ClientResponseDto Client { get; set; } = null!;

  public RequestStatus Status { get; set; } = RequestStatus.New;

  public RequestPriority Priority { get; set; } = RequestPriority.Normal;

  public int? ProjectId { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? ReviewedAt { get; set; }
}