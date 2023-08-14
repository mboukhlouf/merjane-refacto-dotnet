using Microsoft.AspNetCore.Mvc;
using MerjaneRefacto.Presentation.Dtos.Product;
using MerjaneRefacto.Presentation.Services.Impl;
using MerjaneRefacto.Core.Entities;
using Core.Abstractions.Repositories;

namespace MerjaneRefacto.Presentation;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly ProductService _ps;
    private readonly IUnitOfWork unitOfWork;

    public OrdersController(ProductService ps, IUnitOfWork unitOfWork)
    {
        _ps = ps;
        this.unitOfWork = unitOfWork;
    }

    [HttpPost("{orderId}/processOrder")]
    [ProducesResponseType(200)]
    public async Task<ActionResult<ProcessOrderResponse>> ProcessOrder(long orderId)
    {
        Order? order = unitOfWork.Orders.GetOrder(orderId);

        Console.WriteLine(order);
        List<long> ids = new() { orderId };
        ICollection<Product>? products = order.Items;

        foreach (var product in products)
        {
            if (product.Type == "NORMAL")
            {
                if (product.Available > 0)
                {
                    product.Available -= 1;
                    unitOfWork.Products.Update(product);
                }
                else
                {
                    int leadTime = product.LeadTime;
                    if (leadTime > 0)
                    {
                        _ps.NotifyDelay(leadTime, product);
                        unitOfWork.Products.Update(product);
                    }
                }
            }
            else if (product.Type == "SEASONAL")
            {
                if (DateTime.Now.Date > product.SeasonStartDate && DateTime.Now.Date < product.SeasonEndDate && product.Available > 0)
                {
                    product.Available -= 1;
                    unitOfWork.Products.Update(product);
                }
                else
                {
                    _ps.HandleSeasonalProduct(product);
                    unitOfWork.Products.Update(product);
                }
            }
            else if (product.Type == "EXPIRABLE")
            {
                if (product.Available > 0 && product.ExpiryDate > DateTime.Now.Date)
                {
                    product.Available -= 1;
                    unitOfWork.Products.Update(product);
                }
                else
                {
                    _ps.HandleExpiredProduct(product);
                    unitOfWork.Products.Update(product);
                }
            }
        }

        await unitOfWork.SaveAsync(CancellationToken.None);

        return new ProcessOrderResponse(order.Id);
    }
}
