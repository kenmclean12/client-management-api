using System.ComponentModel.DataAnnotations;
using api.Models.Clients;
using api.Models.Jobs;

namespace api.Models.Projects;

public class Project
{
    public int Id { get; set; }

    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public List<Job>? Jobs { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public static Project Create(ProjectCreateDto dto)
    {
      var project = new Project
      {
          Name = dto.Name,
          ClientId = dto.ClientId,
          StartDate = dto.StartDate,
      };

      if (dto.Description is not null) project.Description = dto.Description;
      if (dto.EndDate is not null) project.EndDate = dto.EndDate;

      return project;
    }

    public void Update(ProjectUpdateDto dto)
    {
      UpdatedAt = DateTime.UtcNow;
      if (dto.Name is not null) Name = dto.Name;
      if (dto.ClientId is int clientId) ClientId = clientId;
      if (dto.Description is not null) Description = dto.Description;
      if (dto.StartDate is DateTime startDate) StartDate = startDate;
      if (dto.EndDate is DateTime endDate) EndDate = endDate;
    }

    public ProjectResponseDto ToResponse()
    {
      var response = new ProjectResponseDto
      {
          Name = Name,
          ClientId = ClientId,
          Client = Client,
          StartDate = StartDate,
          CreatedAt = CreatedAt,
      };

      if (Jobs is not null) response.Jobs = Jobs;
      if (Description is not null) response.Description = Description;
      if (EndDate is not null) response.EndDate = EndDate;
      if (UpdatedAt is not null) response.UpdatedAt = UpdatedAt;

      return response;
    }
}
