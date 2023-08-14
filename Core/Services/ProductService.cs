﻿using MerjaneRefacto.Core.Abstractions.Services;
using MerjaneRefacto.Core.Entities;

namespace MerjaneRefacto.Core.Services;

public sealed class ProductService : IProductService
{
    private readonly INotificationService _ns;
    private readonly IDateTimeProvider dateTimeProvider;

    public ProductService(INotificationService ns,
        IDateTimeProvider dateTimeProvider)
    {
        _ns = ns;
        this.dateTimeProvider = dateTimeProvider;
    }

    public void NotifyDelay(int leadTime, Product p)
    {
        p.LeadTime = leadTime;
        _ns.SendDelayNotification(leadTime, p.Name);
    }

    public void HandleSeasonalProduct(Product p)
    {
        if (dateTimeProvider.Now.AddDays(p.LeadTime) > p.SeasonEndDate)
        {
            _ns.SendOutOfStockNotification(p.Name);
            p.Available = 0;
        }
        else if (p.SeasonStartDate > dateTimeProvider.Now)
        {
            _ns.SendOutOfStockNotification(p.Name);
        }
        else
        {
            NotifyDelay(p.LeadTime, p);
        }
    }

    public void HandleExpiredProduct(Product p)
    {
        if (p.Available > 0 && p.ExpiryDate > dateTimeProvider.Now)
        {
            p.Available -= 1;
        }
        else
        {
            _ns.SendExpirationNotification(p.Name, (DateTime)p.ExpiryDate);
            p.Available = 0;
        }
    }

    public void HandleFlashSaleProductPeriodEnded(Product product)
    {
        product.Available = 0;
    }
}
