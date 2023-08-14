using Microsoft.AspNetCore.Mvc;
using MerjaneRefacto.Presentation.Dtos.Product;
using MerjaneRefacto.Core.Entities;
using Core.Abstractions.Repositories;
using MerjaneRefacto.Core.Services;
using MerjaneRefacto.Core.Abstractions.Services;

namespace MerjaneRefacto.Presentation;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IProductService productService;
    private readonly IUnitOfWork unitOfWork;

    public OrdersController(IProductService ps, IUnitOfWork unitOfWork)
    {
        productService = ps;
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
                        productService.NotifyDelay(leadTime, product);
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
                    productService.HandleSeasonalProduct(product);
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
                    productService.HandleExpiredProduct(product);
                    unitOfWork.Products.Update(product);
                }
            }
        }

        await unitOfWork.SaveAsync(CancellationToken.None);

        return new ProcessOrderResponse(order.Id);
    }
}
