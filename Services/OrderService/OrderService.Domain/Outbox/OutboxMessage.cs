
using System.Text.Json;

namespace OrderService.Domain.Outbox
{
    public class OutboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime OccurredOnUtc { get; private set; } = DateTime.UtcNow;
        public string TypeName { get; private set; } = null!;
        public string Payload { get; private set; } = null!;
        public OutboxStatus Status { get; private set; } = OutboxStatus.Pending;
        public int TryCount { get; private set; } = 0;
        public string? LastError { get; private set; }
        public DateTime? NextAttemptUtc { get; private set; }

        private OutboxMessage() { }

        public static OutboxMessage FromEvent(object @event, JsonSerializerOptions? options = null)
        {
            var type = @event.GetType();
            return new OutboxMessage
            {
                TypeName = type.AssemblyQualifiedName ?? type.FullName!,
                Payload = JsonSerializer.Serialize(@event, type, options ?? new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
            };
        }

        public void MarkPublished()
        {
            Status = OutboxStatus.Published;
            LastError = null!;
            NextAttemptUtc = null;
        }

        public void MarkFailed(string error, TimeSpan? delay = null)
        {
            Status = OutboxStatus.Failed;
            TryCount++;
            LastError = error;
            NextAttemptUtc = DateTime.UtcNow.Add(delay ?? TimeSpan.FromSeconds(Math.Min(300, Math.Pow(2, TryCount)))); // Simple exponential backoff (5 mins max)
            Status = OutboxStatus.Pending; // to be tried again.
        }
    }
}
