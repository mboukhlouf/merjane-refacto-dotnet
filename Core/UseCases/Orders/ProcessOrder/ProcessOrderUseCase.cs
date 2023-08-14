using MerjaneRefacto.Core.Abstractions.Repositories;
using MerjaneRefacto.Core.Abstractions.Services;
using MerjaneRefacto.Core.Entities;
using Microsoft.Extensions.Logging;

namespace MerjaneRefacto.Core.UseCases.Orders.ProcessOrder
{
    internal sealed class ProcessOrderUseCase : IProcessOrderUseCase
    {
        private readonly ILogger<ProcessOrderUseCase> logger;
        private readonly IProductService productService;
        private readonly IUnitOfWork unitOfWork;
        private readonly IDateTimeProvider dateTimeProvider;

        public ProcessOrderUseCase(ILogger<ProcessOrderUseCase> logger,
            IProductService productService,
            IUnitOfWork unitOfWork,
            IDateTimeProvider dateTimeProvider)
        {
            this.productService = productService;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.dateTimeProvider = dateTimeProvider;
        }

        public async Task<ProcessOrderResponse> HandleAsync(ProcessOrderRequest request)
        {
            Order? order = unitOfWork.Orders.GetOrder(request.orderId);

            logger.LogInformation($"Processing order with id {order.Id}");

            // Unused variable
            //var ids = new[]
            //{
            //    request.orderId
            //};

            ICollection<Product>? products = order.Items;

            foreach (var product in products)
            {
                switch (product.Type)
                {
                    case "NORMAL":
                        ProcessNormalProduct(product);
                        break;
                    case "SEASONAL":
                        ProcessSeasonalProduct(product);
                        break;
                    case "EXPIRABLE":
                        ProcessExpirableProduct(product);
                        break;
                    case "FLASHSALE":
                        ProcessFlashSaleProduct(product);
                        break;
                    default:
                        throw new Exception($"Unknown product type: {product.Type}");
                }
            }

            await unitOfWork.SaveAsync(CancellationToken.None);

            return new ProcessOrderResponse(order.Id);
        }

        private void ProcessNormalProduct(Product product)
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

        private void ProcessSeasonalProduct(Product product)
        {
            if (dateTimeProvider.Today > product.SeasonStartDate && dateTimeProvider.Today < product.SeasonEndDate && product.Available > 0)
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

        private void ProcessExpirableProduct(Product product)
        {
            if (product.Available > 0 && product.ExpiryDate > dateTimeProvider.Today)
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

        private void ProcessFlashSaleProduct(Product product)
        {
            if (product.Available == 0)
            {
                logger.LogInformation($"FlashSale Product {product.Id} is not available anymore.");
                return;
            }

            if (dateTimeProvider.Today > product.FlashSaleEndDate)
            {
                productService.HandleFlashSaleProductPeriodEnded(product);
                return;
            }

            if(dateTimeProvider.Today < product.FlashSaleStartDate)
            {
                logger.LogInformation($"FlashSale period for Product {product.Id} didn't start yet.");
                return;
            }

            product.Available -= 1;
            unitOfWork.Products.Update(product);
        }
    }
}
