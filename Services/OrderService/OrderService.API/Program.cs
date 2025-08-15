using MassTransit;
using OrderService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using OrderService.Application;
using OrderService.Application.Consumers;
using OrderService.Application.Features.Orders.Commands.CreateOrder;
using OrderService.Application.Services;
using OrderService.Infrastructure;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDb")));
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly));
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();

builder.Services.AddHostedService<OrderService.Infrastructure.Outbox.OutboxDispatcher>();

builder.Services.AddHttpClient<BasketServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://ecommerce.gateway:7000");
});

builder.Services.AddHttpClient<ProductServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://ecommerce.gateway:7000");
});

builder.Services.AddScoped<CreateOrderCommandHandler>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<PaymentSucceededConsumer>();
    x.AddConsumer<PaymentFailedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("order-created-queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        });

        cfg.ReceiveEndpoint("payment-succeeded-to-order", e =>
        {
            e.ConfigureConsumer<PaymentSucceededConsumer>(context);
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        });

        cfg.ReceiveEndpoint("payment-failed-to-order", e =>
        {
            e.ConfigureConsumer<PaymentFailedConsumer>(context);
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        });
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();