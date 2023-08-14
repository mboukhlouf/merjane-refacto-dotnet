using MerjaneRefacto.Core.Entities;

namespace MerjaneRefacto.Core.Abstractions.Services;

public interface IProductService
{
    void HandleExpiredProduct(Product product);

    void HandleSeasonalProduct(Product product);

    void HandleFlashSaleProductPeriodEnded(Product product);

    void NotifyDelay(int leadTime, Product p);
}
