using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.API.Consumers;
using PaymentService.Application.Interfaces;
using PaymentService.Application.Payments;
using PaymentService.Application.Services;
using PaymentService.Infrastructure.Context;
using PaymentService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<PaymentDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("PaymentDb")));

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentProcessor, FakePaymentProcessor>();
builder.Services.AddScoped<IOrderCreatedEventHandler, OrderCreatedEventHandler>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentOrderCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h => { h.Username("guest"); h.Password("guest"); });
        cfg.ReceiveEndpoint("order-created-to-payment", e =>
        {
            e.ConfigureConsumer<PaymentOrderCreatedConsumer>(context);
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        });
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
