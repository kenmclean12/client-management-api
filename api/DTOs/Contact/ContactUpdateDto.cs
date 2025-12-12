using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Contacts;

public class ContactUpdateDto
{
    [MaxLength(100)]
    public string? Name { get; set; }

    [EmailAddress]
    [MaxLength(100)]
    public string? Email { get; set; }

    [Phone]
    [MaxLength(20)]
    public string? Phone { get; set; }

    public int? ClientId { get; set; }
}
