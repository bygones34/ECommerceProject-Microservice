using ProductService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace ProductService.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ILogger<AuditLogService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(ILogger<AuditLogService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task AuditLog(string action, string entityName, string entityId, string performedBy)
        {
            var correlationId = _httpContextAccessor.HttpContext?.Items["X-Correlation-ID"]?.ToString() ?? "N/A";
            var message = $"[AUDIT] Action: {action}, Entity: {entityName}, Id: {entityId}, By: {performedBy}, CorrelationId: {correlationId}, At: {DateTime.UtcNow}";
            _logger.LogInformation(message);
            return Task.CompletedTask;
        }
    }
}
