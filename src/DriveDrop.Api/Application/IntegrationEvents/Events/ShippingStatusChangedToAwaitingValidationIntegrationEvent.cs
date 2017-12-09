namespace DriveDrop.Api.Application.IntegrationEvents.Events
{
    using System.Collections.Generic;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class ShippingStatusChangedToAwaitingValidationIntegrationEvent : IntegrationEvent
    {
        public int ShippingId { get; }
        //public IEnumerable<OrderStockItem> OrderStockItems { get; }

        public ShippingStatusChangedToAwaitingValidationIntegrationEvent(int shippingId
            //IEnumerable<OrderStockItem> orderStockItems
            )
        {
            ShippingId = shippingId;
         //   OrderStockItems = orderStockItems;
        }
    }

    //public class OrderStockItem
    //{
    //    public int ProductId { get; }
    //    public int Units { get; }

    //    public OrderStockItem(int productId, int units)
    //    {
    //        ProductId = productId;
    //        Units = units;
    //    }
    //}
}