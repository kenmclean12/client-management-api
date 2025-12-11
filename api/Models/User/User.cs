using api.DTOs.User;

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

  public static User Create(UserCreateDto dto)
  {
    return new User
    {
      UserName = dto.UserName,
      Email = dto.Email,
      FirstName = dto.FirstName,
      LastName = dto.LastName,
      PasswordHash = HashPassword(dto.Password),
    };
  }

  public void Update(UserUpdateDto dto)
  {
    if (dto.UserName is not null) UserName = dto.UserName;
    if (dto.Email is not null) Email = dto.Email;
    if (dto.Password is not null) PasswordHash = HashPassword(dto.Password);
    if (dto.FirstName is not null) FirstName = dto.FirstName;
    if (dto.LastName is not null) LastName = dto.LastName;

    UpdatedAt = DateTime.UtcNow;
  }

  private static string HashPassword(string password)
  {
    return password;
  }
}
