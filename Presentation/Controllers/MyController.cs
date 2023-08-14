using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerjaneRefacto.Presentation.Database.Context;
using MerjaneRefacto.Presentation.Dtos.Product;
using MerjaneRefacto.Presentation.Services.Impl;

namespace MerjaneRefacto.Presentation
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly ProductService _ps;
        private readonly AppDbContext _ctx;

        public OrdersController(ProductService ps, AppDbContext ctx)
        {
            _ps = ps;
            _ctx = ctx;
        }

        [HttpPost("{orderId}/processOrder")]
        [ProducesResponseType(200)]
        public ActionResult<ProcessOrderResponse> ProcessOrder(long orderId)
        {
            Entities.Order? order = _ctx.Orders
                .Include(o => o.Items)
                .SingleOrDefault(o => o.Id == orderId);
            Console.WriteLine(order);
            List<long> ids = new() { orderId };
            ICollection<Entities.Product>? products = order.Items;

            foreach (Entities.Product p in products)
            {
                if (p.Type == "NORMAL")
                {
                    if (p.Available > 0)
                    {
                        p.Available -= 1;
                        _ctx.Entry(p).State = EntityState.Modified;
                        _ = _ctx.SaveChanges();

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
                        _ = _ctx.SaveChanges();
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
                        _ = _ctx.SaveChanges();
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
