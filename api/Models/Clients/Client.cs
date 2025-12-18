using System.ComponentModel.DataAnnotations;
using api.DTOs.Client;

namespace api.Models.Clients;

public class Client
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

  public DateTime? UpdatedAt { get; set; }

  public bool SoftDeleted { get; set; } = false;

  public static Client Create(ClientCreateDto dto)
  {
    return new Client
    {
      Name = dto.Name,
      Email = dto.Email,
      PhoneNumber = dto.PhoneNumber,
      Address = dto.Address,
      City = dto.City,
      State = dto.State,
      ZipCode = dto.ZipCode,
      Country = dto.Country,
    };
  }

  public void Update(ClientUpdateDto dto)
  {
    if (dto.Name != null) Name = dto.Name;
    if (dto.Email != null) Email = dto.Email;
    if (dto.PhoneNumber != null) PhoneNumber = dto.PhoneNumber;
    if (dto.Address != null) Address = dto.Address;
    if (dto.City != null) City = dto.City;
    if (dto.State != null) State = dto.State;
    if (dto.ZipCode != null) ZipCode = dto.ZipCode;
    if (dto.Country != null) Country = dto.Country;

    UpdatedAt = DateTime.UtcNow;
  }

  public ClientResponseDto ToResponse()
  {
    var dto = new ClientResponseDto
    {
      Id = Id,
      Name = Name,
      Email = Email,
      CreatedAt = CreatedAt,
      SoftDeleted = SoftDeleted,
    };

    if (PhoneNumber is not null) dto.PhoneNumber = PhoneNumber;
    if (Address is not null) dto.Address = Address;
    if (City is not null) dto.City = City;
    if (State is not null) dto.State = State;
    if (ZipCode is not null) dto.ZipCode = ZipCode;
    if (Country is not null) dto.Country = Country;
    if (UpdatedAt is not null) dto.UpdatedAt = UpdatedAt;

    return dto;
  }
}