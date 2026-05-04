using GoldenCrown.Database;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.BackGroundServices
{
    public class SessionCleanupService : BackgroundService
    {
        private static readonly TimeSpan Delay = TimeSpan.FromMinutes(10);

        private readonly ILogger<SessionCleanupService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public SessionCleanupService(IServiceScopeFactory scopeFactory,ILogger<SessionCleanupService> logger)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Используем отдельный scope для получения свежего экземпляра DbContext
                    using var scope = _scopeFactory.CreateScope();
                        
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                        // Удаляем сессии, у которых срок истёк
                    int deletedCount = await dbContext.Sessions
                            .Where(s => s.ExpiresAt <= DateTime.UtcNow)
                            .ExecuteDeleteAsync(stoppingToken);

                    _logger.LogInformation("Removed Sessions: {Count}", deletedCount);
                    
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Нормальная остановка приложения
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while removing sessions");
                }

                // Ждём 10 минут перед следующей итерацией
                await Task.Delay(Delay, stoppingToken);
            }
        }
    }
}
