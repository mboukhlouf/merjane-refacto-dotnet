using Microsoft.AspNetCore.Mvc;
using MerjaneRefacto.Core.UseCases.Orders.ProcessOrder;

namespace MerjaneRefacto.Presentation;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IProcessOrderUseCase processOrderUseCase;

    public OrdersController(IProcessOrderUseCase processOrderUseCase)
    {
        this.processOrderUseCase = processOrderUseCase;
    }

    [HttpPost("{orderId}/processOrder")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<ProcessOrderResponse>> ProcessOrder(long orderId)
    {
        var request = new ProcessOrderRequest(orderId);
        var response = await processOrderUseCase.HandleAsync(request);
        return response;
    }
}
