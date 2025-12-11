using api.DTOs.User;
using Microsoft.AspNetCore.Identity;


namespace api.Models.User;

public class User
{
  public int Id { get; set; }

  public string UserName { get; set; } = null!;
  public string Email { get; set; } = null!;
  public string PasswordHash { get; set; } = null!;

  public string FirstName { get; set; } = null!;
  public string LastName { get; set; } = null!;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public DateTime? UpdatedAt { get; set; } = null;

  private static readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

  public static User Create(UserCreateDto dto)
  {
    var user = new User
    {
      UserName = dto.UserName,
      Email = dto.Email,
      FirstName = dto.FirstName,
      LastName = dto.LastName,
    };

    user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
    return user;
  }

  public void Update(UserUpdateDto dto)
  {
    if (dto.UserName is not null) UserName = dto.UserName;
    if (dto.Email is not null) Email = dto.Email;
    if (dto.FirstName is not null) FirstName = dto.FirstName;
    if (dto.LastName is not null) LastName = dto.LastName;

    UpdatedAt = DateTime.UtcNow;
  }
  public bool VerifyPassword(string password)
  {
    var result = _passwordHasher.VerifyHashedPassword(this, PasswordHash, password);
    return result == PasswordVerificationResult.Success;
  }

  public static string HashPassword(User user, string password)
  {
    return _passwordHasher.HashPassword(user, password);
  }
}
