using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Features.Orders.Commands.CreateOrder;
using OrderService.Application.Features.Orders.Queries.GetOrders;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{buyerId}")]
    public async Task<IActionResult> GetOrdersByBuyerIdAsync(string buyerId)
    {
        var query = new GetOrdersQuery(buyerId);
        var orders =  await _mediator.Send(query);
        return Ok(orders);
    }

    [HttpGet("by-id/{orderId:guid}")]
    public async Task<IActionResult> GetOrderByIdAsync(Guid orderId)
    {
        var dto = await _mediator.Send(new GetOrderByIdQuery(orderId));
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync(CreateOrderCommand command)
    {
        var orderId =  await _mediator.Send(command);
        return Ok(orderId);
    }
}