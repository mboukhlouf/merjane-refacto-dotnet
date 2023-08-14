using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using MerjaneRefacto.Presentation.Database.Context;
using MerjaneRefacto.Presentation.Services;
using MerjaneRefacto.Presentation.Services.Impl;
using MerjaneRefacto.Core.Entities;

namespace MerjaneRefacto.Presentation.Tests.Services
{
    public class MyUnitTests
    {
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly ProductService _productService;

        public MyUnitTests()
        {
            _mockNotificationService = new Mock<INotificationService>();
            _productService = new ProductService(_mockNotificationService.Object);
        }

        [Fact]
        public void Test()
        {
            // GIVEN
            Product product = new()
            {
                LeadTime = 15,
                Available = 0,
                Type = "NORMAL",
                Name = "RJ45 Cable"
            };

            // WHEN
            _productService.NotifyDelay(product.LeadTime, product);

            // THEN
            Assert.Equal(0, product.Available);
            Assert.Equal(15, product.LeadTime);
            _mockNotificationService.Verify(service => service.SendDelayNotification(product.LeadTime, product.Name), Times.Once());
        }
    }
}
