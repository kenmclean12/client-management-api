using System.ComponentModel.DataAnnotations;

namespace api.DTOs.User;

public class UserInviteCreateDto
{
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = null!;
}