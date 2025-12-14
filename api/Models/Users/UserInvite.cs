using System.ComponentModel.DataAnnotations;
using api.DTOs.User;
using System.Security.Cryptography;

namespace api.Models.Users;

public class UserInvite
{
  public int Id { get; set; }

  [Required]
  [EmailAddress]
  [MaxLength(100)]
  public string Email { get; set; } = null!;

  [Required]
  public int UserId { get; set; }

  public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(7);

  [Required]
  [MaxLength(128)]
  public string Token { get; private set; } = null!;

  public static UserInvite Create(string Email, int UserId)
  {
    return new UserInvite
    {
      Email = Email,
      UserId = UserId,
      Token = GenerateToken(),
    };
  }

  private static string GenerateToken()
  {
    var bytes = RandomNumberGenerator.GetBytes(32);
    return Convert.ToBase64String(bytes)
      .Replace("+", "-")
      .Replace("/", "_")
      .Replace("=", "");
  }
}