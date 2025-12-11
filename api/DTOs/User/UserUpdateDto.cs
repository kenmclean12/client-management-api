using System.ComponentModel.DataAnnotations;

namespace api.DTOs.User;

public class UserUpdateDto
{
  [MaxLength(30)]
  public string? UserName { get; set; } = null;

  [MaxLength(100)]
  public string? Email { get; set; } = null;

  [MaxLength(30)]
  public string? FirstName { get; set; } = null;

  [MaxLength(30)]
  public string? LastName { get; set; } = null;
}