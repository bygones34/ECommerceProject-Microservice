using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProductService.API.Middlewares
{
    public class ValidationMiddleware
    {
        private readonly RequestDelegate _next;
        public ValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<ControllerActionDescriptor>() is not null)
            {
                context.Request.EnableBuffering();

                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(body))
                {
                    var routeData = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

                    var parameter = routeData?.Parameters
                        .FirstOrDefault(p => p.BindingInfo != null &&
                                             p.BindingInfo.BindingSource != null &&
                                             p.BindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Body));

                    var parameterType = parameter?.ParameterType;
                    
                    Console.WriteLine($"[Middleware] Trying to deserialize into type: {parameterType?.Name}");

                    if (parameterType != null)
                    {
                        var model = JsonSerializer.Deserialize(body, parameterType);
                        if (model != null)
                        {
                            var validatorType = typeof(IValidator<>).MakeGenericType(parameterType);
                            var validator = context.RequestServices.GetService(validatorType) as IValidator;
                            if (validator != null)
                            {
                                var validationContext = new ValidationContext<object>(model);
                                var result = await validator.ValidateAsync(validationContext);

                                if (!result.IsValid)
                                {
                                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                    await context.Response.WriteAsJsonAsync(new
                                    {
                                        errors = result.Errors.GroupBy(e => e.PropertyName)
                                                .ToDictionary(
                                                    g => g.Key,
                                                    g => g.Select(x => x.ErrorMessage).ToArray()
                                                )
                                    });
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            await _next(context);
        }
    }

}

