using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Contacts;

public class ContactCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = null!;

    [Required]
    public int ClientId { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }
}
