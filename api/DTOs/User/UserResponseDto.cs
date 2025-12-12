using api.Models.Users;

namespace api.DTOs.User;

public class UserResponseDto
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public UserRole Role { get; set; } = UserRole.ReadOnly;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string AvatarUrl { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = null;
}
