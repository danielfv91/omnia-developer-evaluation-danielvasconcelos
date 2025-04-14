using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sale;
using Ambev.DeveloperEvaluation.Unit.Sales.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Sales.Domain
{

    public class SaleDomainTests
    {
        [Theory]
        [MemberData(nameof(SaleDomainTestData.DiscountScenarios), MemberType = typeof(SaleDomainTestData))]
        public void Create_Should_ApplyDiscount_Correctly(int quantity, decimal unitPrice, decimal expectedDiscount, decimal expectedTotal)
        {
            // Arrange
            var items = SaleDomainTestData.BuildItemList(quantity, unitPrice);

            // Act
            var sale = Sale.Create(123, DateTime.UtcNow, Guid.NewGuid(), "Customer Test", "Branch X", items);

            // Assert
            var item = sale.Items.First();
            item.DiscountPercentage.Should().Be(expectedDiscount);
            item.TotalItemAmount.Should().BeApproximately(expectedTotal, 0.01m);
            sale.TotalAmount.Should().BeApproximately(expectedTotal, 0.01m);

            var @event = sale.DomainEvents.OfType<SaleCreatedDomainEvent>().FirstOrDefault();
            @event.Should().NotBeNull();
            @event!.TotalAmount.Should().BeApproximately(expectedTotal, 0.01m);
        }

        [Fact]
        public void Update_Should_Raise_ItemCancelled_And_SaleModified_Events()
        {
            // Arrange
            var (originalSale, updatedItems) = SaleDomainTestData.CreateSaleWithRemovedItem();
            originalSale.ClearDomainEvents();

            // Act
            originalSale.Update(999, DateTime.UtcNow, Guid.NewGuid(), "Updated", "Branch Y", updatedItems);

            // Assert
            var cancelledEvents = originalSale.DomainEvents.OfType<ItemCancelledDomainEvent>().ToList();
            cancelledEvents.Should().HaveCount(1);
            cancelledEvents[0].Reason.Should().Be("Item removed during sale update");

            var modifiedEvent = originalSale.DomainEvents.OfType<SaleModifiedDomainEvent>().FirstOrDefault();
            modifiedEvent.Should().NotBeNull();
            modifiedEvent!.SaleNumber.Should().Be(999);
        }

        [Fact]
        public void Cancel_Should_Set_IsCancelled_And_Raise_Event()
        {
            // Arrange
            var sale = SaleDomainTestData.CreateDefaultSale();
            sale.ClearDomainEvents();

            // Act
            sale.Cancel("Testing cancellation");

            // Assert
            sale.IsCancelled.Should().BeTrue();

            var cancelEvent = sale.DomainEvents.OfType<SaleCancelledDomainEvent>().FirstOrDefault();
            cancelEvent.Should().NotBeNull();
            cancelEvent!.Reason.Should().Be("Testing cancellation");
        }
    }
}