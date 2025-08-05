namespace ProductService.API.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationOrder = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(CorrelationOrder, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                context.Request.Headers[CorrelationOrder] = correlationId;
            }

            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(CorrelationOrder))
                {
                    context.Response.Headers.Add(CorrelationOrder, correlationId);
                }
                return Task.CompletedTask;
            });

            context.Items[CorrelationOrder] = correlationId;

            await _next(context);
        }
    }
}
