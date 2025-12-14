using System.ComponentModel.DataAnnotations;
using api.DTOs.User;

namespace api.Models.Users;

public class UserInvite
{
  public int Id { get; set; }

  [Required]
  [EmailAddress]
  [MaxLength(100)]
  public string Email { get; set; } = null!;

  [Required]
  public string UserId { get; set; } = null!;

  public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(7);

  public static UserInvite Create(UserInviteCreateDto dto)
  {
    return new UserInvite
    {
      Email = dto.Email,
      UserId = dto.UserId,
    };
  }
}