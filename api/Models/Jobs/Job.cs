using api.Models.Clients;

namespace api.Models.Jobs;

public class Job
{
  public int Id { get; set; }

  public string Name { get; set; } = null!;

  public string Description { get; set; } = null!;

  public int ClientId { get; set; }
  public Client Client { get; set; } = new Client();
}