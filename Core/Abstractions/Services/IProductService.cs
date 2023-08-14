using MerjaneRefacto.Core.Entities;

namespace MerjaneRefacto.Core.Abstractions.Services;

public interface IProductService
{
    void HandleExpiredProduct(Product p);

    void HandleSeasonalProduct(Product p);

    void HandleFlashSaleProductPeriodEnded(Product product);

    void NotifyDelay(int leadTime, Product p);
}
