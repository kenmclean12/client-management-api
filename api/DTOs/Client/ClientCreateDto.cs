using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Client;

public class ClientCreateDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; set; } = null!;

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(50)]
    public string? Address { get; set; }

    [MaxLength(20)]
    public string? City { get; set; }

    [MaxLength(20)]
    public string? State { get; set; }

    [MaxLength(6)]
    public string? ZipCode { get; set; }

    [MaxLength(20)]
    public string? Country { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}