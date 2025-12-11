namespace api.DTOs.User;

public class UserPasswordResetDto
{
  public string password { get; set; } = null!;
  public string newPassword { get; set; } = null!;
}