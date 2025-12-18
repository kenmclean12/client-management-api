using api.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Services.Background;

public class UserInviteCleanupService : BackgroundService
{
  private readonly IServiceScopeFactory _scopeFactory;

  public UserInviteCleanupService(IServiceScopeFactory scopeFactory)
  {
    _scopeFactory = scopeFactory;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      await PurgeExpiredInvites(stoppingToken);
      await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
    }
  }

  private async Task PurgeExpiredInvites(CancellationToken token)
  {
    using var scope = _scopeFactory.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var expiredInvites = await db.UserInvites
      .Where(i => i.ExpiryDate <= DateTime.UtcNow)
      .ToListAsync(token);

    if (expiredInvites.Count == 0) return;

    db.UserInvites.RemoveRange(expiredInvites);
    await db.SaveChangesAsync(token);
  }
}
