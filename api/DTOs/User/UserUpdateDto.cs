using System.ComponentModel.DataAnnotations;
using api.Models.User;

namespace api.DTOs.User;

public class UserUpdateDto
{
  [MaxLength(30)]
  public string? UserName { get; set; }

  [MaxLength(100)]
  public string? Email { get; set; }

  [MaxLength(30)]
  public string? FirstName { get; set; }

  [MaxLength(30)]
  public string? LastName { get; set; }

  [Required]
  public UserRole? Role { get; set; }
}