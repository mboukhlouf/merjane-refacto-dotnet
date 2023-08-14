using Microsoft.AspNetCore.Mvc;
using MerjaneRefacto.Presentation.Dtos.Product;
using MerjaneRefacto.Presentation.Services.Impl;
using MerjaneRefacto.Core.Entities;
using Core.Repositories;

namespace MerjaneRefacto.Presentation
{
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

            foreach (Product p in products)
            {
                if (p.Type == "NORMAL")
                {
                    if (p.Available > 0)
                    {
                        p.Available -= 1;
                        await unitOfWork.SaveAsync(CancellationToken.None);

                    }
                    else
                    {
                        int leadTime = p.LeadTime;
                        if (leadTime > 0)
                        {
                            _ps.NotifyDelay(leadTime, p);
                        }
                    }
                }
                else if (p.Type == "SEASONAL")
                {
                    if (DateTime.Now.Date > p.SeasonStartDate && DateTime.Now.Date < p.SeasonEndDate && p.Available > 0)
                    {
                        p.Available -= 1;
                        await unitOfWork.SaveAsync(CancellationToken.None);
                    }
                    else
                    {
                        _ps.HandleSeasonalProduct(p);
                    }
                }
                else if (p.Type == "EXPIRABLE")
                {
                    if (p.Available > 0 && p.ExpiryDate > DateTime.Now.Date)
                    {
                        p.Available -= 1;
                        await unitOfWork.SaveAsync(CancellationToken.None);
                    }
                    else
                    {
                        _ps.HandleExpiredProduct(p);
                    }
                }
            }

            return new ProcessOrderResponse(order.Id);
        }
    }
}
