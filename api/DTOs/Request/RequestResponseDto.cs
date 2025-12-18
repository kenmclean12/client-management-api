using api.DTOs.Client;
using api.Models.Request;

namespace api.DTOs.Request;

public class RequestResponseDto
{
  public int Id { get; set; }

  public string Title { get; set; } = null!;

  public string Description { get; set; } = null!;

  public int ClientId { get; set; }

  public ClientResponseDto Client { get; set; } = null!;

  public RequestStatus Status { get; set; } = RequestStatus.New;

  public RequestPriority Priority { get; set; } = RequestPriority.Normal;

  public int? ProjectId { get; set; }

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? ReviewedAt { get; set; }
}