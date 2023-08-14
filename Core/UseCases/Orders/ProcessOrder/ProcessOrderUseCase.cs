using MerjaneRefacto.Core.Abstractions.Repositories;
using MerjaneRefacto.Core.Abstractions.Services;
using MerjaneRefacto.Core.Entities;

namespace MerjaneRefacto.Core.UseCases.Orders.ProcessOrder
{
    internal sealed class ProcessOrderUseCase : IProcessOrderUseCase
    {
        private readonly IProductService productService;
        private readonly IUnitOfWork unitOfWork;

        public ProcessOrderUseCase(IProductService productService,
            IUnitOfWork unitOfWork)
        {
            this.productService = productService;
            this.unitOfWork = unitOfWork;
        }

        public async Task<ProcessOrderResponse> HandleAsync(ProcessOrderRequest request)
        {
            Order? order = unitOfWork.Orders.GetOrder(request.orderId);

            Console.WriteLine(order);
            List<long> ids = new() { request.orderId };
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
}
