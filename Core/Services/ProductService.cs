using MerjaneRefacto.Core.Abstractions.Services;
using MerjaneRefacto.Core.Entities;

namespace MerjaneRefacto.Core.Services;

public sealed class ProductService : IProductService
{
    private readonly INotificationService notificationService;
    private readonly IDateTimeProvider dateTimeProvider;

    public ProductService(INotificationService notificationService,
        IDateTimeProvider dateTimeProvider)
    {
        this.notificationService = notificationService;
        this.dateTimeProvider = dateTimeProvider;
    }

    public void NotifyDelay(int leadTime, Product product)
    {
        product.LeadTime = leadTime;
        notificationService.SendDelayNotification(leadTime, product.Name);
    }

    public void HandleSeasonalProduct(Product product)
    {
        if (dateTimeProvider.Now.AddDays(product.LeadTime) > product.SeasonEndDate)
        {
            notificationService.SendOutOfStockNotification(product.Name);
            product.Available = 0;
        }
        else if (product.SeasonStartDate > dateTimeProvider.Now)
        {
            notificationService.SendOutOfStockNotification(product.Name);
        }
        else
        {
            NotifyDelay(product.LeadTime, product);
        }
    }

    public void HandleExpiredProduct(Product product)
    {
        if (product.Available > 0 && product.ExpiryDate > dateTimeProvider.Now)
        {
            product.Available -= 1;
        }
        else
        {
            notificationService.SendExpirationNotification(product.Name, (DateTime)product.ExpiryDate);
            product.Available = 0;
        }
    }

    public void HandleFlashSaleProductPeriodEnded(Product product)
    {
        product.Available = 0;
    }
}
