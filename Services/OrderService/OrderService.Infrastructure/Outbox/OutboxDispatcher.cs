using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using OrderService.Domain.Outbox;
using OrderService.Infrastructure.Context;
using System.Text.Json;

namespace OrderService.Infrastructure.Outbox
{
    public class OutboxDispatcher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxDispatcher> _logger;

        public OutboxDispatcher(IServiceScopeFactory scopeFactory, ILogger<OutboxDispatcher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OutboxDispatcher started!");
            while (stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                    var publish = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    var now = DateTime.UtcNow;

                    var batch = await db.OutboxMessages
                   .Where(x => x.Status == OutboxStatus.Pending && (x.NextAttemptUtc == null || x.NextAttemptUtc <= now))
                   .OrderBy(x => x.OccurredOnUtc)
                   .Take(20)
                   .ToListAsync(stoppingToken);

                    if (batch.Count == 0)
                    {
                        await Task.Delay(1500, stoppingToken);
                        continue;
                    }

                    foreach ( var message in batch)
                    {
                        try
                        {
                            var type = Type.GetType(message.TypeName, throwOnError: true)!;
                            var obj = JsonSerializer.Deserialize(message.Payload, type, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                            if (obj == null)
                            {
                                message.MarkFailed("Payload deserialize null!");
                                continue;
                            }

                            await publish.Publish(obj, stoppingToken);
                            message.MarkPublished();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Outbox publish failed, MessageId = {Id}", message.Id);
                            message.MarkFailed(ex.Message);
                        }
                    }

                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "OutboxDispatcher cycle error!");
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}
