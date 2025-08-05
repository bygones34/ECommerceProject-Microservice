using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace ProductService.API.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Global hata yakalandı!");

                context.Response.ContentType = "application/json";

                int statusCode = ex switch
                {
                    FluentValidation.ValidationException => (int)HttpStatusCode.BadRequest,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                context.Response.StatusCode = statusCode;

                var response = new
                {
                    title = statusCode switch
                    {
                        400 => "Geçersiz Veri",
                        404 => "Kaynak Bulunamadı",
                        409 => "Çakışma Hatası",
                        500 => "Sunucu Hatası",
                        _ => "Hata"
                    },
                    status = statusCode,
                    detail = ex.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
