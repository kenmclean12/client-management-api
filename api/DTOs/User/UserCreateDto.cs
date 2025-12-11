using System.ComponentModel.DataAnnotations;
using api.Models.User;

namespace api.DTOs.User;

public class UserCreateDto
{
  [Required]
  [MaxLength(30)]
  public string UserName { get; set; } = null!;

  [Required]
  public string Password { get; set; } = null!;

  [Required]
  [EmailAddress]
  [MaxLength(100)]
  public string Email { get; set; } = null!;

  [Required]
  [MaxLength(30)]
  public string FirstName { get; set; } = null!;

  [Required]
  [MaxLength(30)]
  public string LastName { get; set; } = null!;

  [Required]
  public UserRole Role { get; set; } = UserRole.ReadOnly;

  [MaxLength(200)]
  public string? AvatarUrl { get; set; }
}
