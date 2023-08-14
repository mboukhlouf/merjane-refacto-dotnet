﻿using MerjaneRefacto.Core.Entities;

namespace MerjaneRefacto.Presentation.Services.Impl
{
    public sealed class ProductService
    {
        private readonly INotificationService _ns;

        public ProductService(INotificationService ns)
        {
            _ns = ns;
        }

        public void NotifyDelay(int leadTime, Product p)
        {
            p.LeadTime = leadTime;
            _ns.SendDelayNotification(leadTime, p.Name);
        }

        public void HandleSeasonalProduct(Product p)
        {
            if (DateTime.Now.AddDays(p.LeadTime) > p.SeasonEndDate)
            {
                _ns.SendOutOfStockNotification(p.Name);
                p.Available = 0;
            }
            else if (p.SeasonStartDate > DateTime.Now)
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
            if (p.Available > 0 && p.ExpiryDate > DateTime.Now)
            {
                p.Available -= 1;
            }
            else
            {
                _ns.SendExpirationNotification(p.Name, (DateTime)p.ExpiryDate);
                p.Available = 0;
            }
        }
    }
}
