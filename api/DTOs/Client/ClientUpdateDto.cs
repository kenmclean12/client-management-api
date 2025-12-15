using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Client;

public class ClientUpdateDto
{
    [MaxLength(50)]
    public string? Name { get; set; }

    [EmailAddress]
    [MaxLength(50)]
    public string? Email { get; set; }

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

    public bool? SoftDeleted { get; set; }
}