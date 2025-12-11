using System.ComponentModel.DataAnnotations;

namespace api.DTOs.User;

public class UserPasswordResetDto
{
  [Required]
  public string Password { get; set; } = null!;
  [Required]
  public string NewPassword { get; set; } = null!;
}