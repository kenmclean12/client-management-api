namespace api.DTOs.User;

public class UserPasswordResetDto
{
  public string Password { get; set; } = null!;
  public string NewPassword { get; set; } = null!;
}