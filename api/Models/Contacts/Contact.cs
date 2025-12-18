using System.ComponentModel.DataAnnotations;
using api.DTOs.Contact;
using api.DTOs.Contacts;
using api.Models.Clients;

namespace api.Models.Contacts;

public class Contact
{
  public int Id { get; set; }

  [Required]
  [MaxLength(100)]
  public string Name { get; set; } = null!;

  [Required]
  [EmailAddress]
  [MaxLength(100)]
  public string Email { get; set; } = null!;

  [Phone]
  [MaxLength(20)]
  public string? Phone { get; set; }

  [Required]
  public int ClientId { get; set; }

  public Client Client { get; set; } = null!;

  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

  public DateTime? UpdatedAt { get; set; }

  public static Contact Create(ContactCreateDto dto)
  {
    return new Contact
    {
      Name = dto.Name,
      Email = dto.Email,
      Phone = dto.Phone,
      ClientId = dto.ClientId,
    };
  }

  public void Update(ContactUpdateDto dto)
  {
    UpdatedAt = DateTime.UtcNow;

    if (dto.Name is not null) Name = dto.Name;
    if (dto.Email is not null) Email = dto.Email;
    if (dto.Phone is not null) Phone = dto.Phone;
    if (dto.ClientId is int clientId) ClientId = clientId;
  }

  public ContactResponseDto ToResponse()
  {
    var dto = new ContactResponseDto
    {
      Id = Id,
      Name = Name,
      Email = Email,
      ClientId = ClientId,
      Client = Client.ToResponse(),
      CreatedAt = CreatedAt,
    };

    if (Phone is not null) dto.Phone = Phone;
    if (UpdatedAt is not null) dto.UpdatedAt = UpdatedAt;

    return dto;
  }
}
