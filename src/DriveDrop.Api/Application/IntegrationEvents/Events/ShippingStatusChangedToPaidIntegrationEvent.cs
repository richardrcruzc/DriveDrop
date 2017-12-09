namespace DriveDrop.Api.Application.IntegrationEvents.Events
{
    using System.Collections.Generic;
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

    public class ShippingStatusChangedToPaidIntegrationEvent : IntegrationEvent
    {
        public int ShippingId { get; }
        //public IEnumerable<OrderStockItem> OrderStockItems { get; }

        public ShippingStatusChangedToPaidIntegrationEvent(int shippingId 
           // IEnumerable<OrderStockItem> orderStockItems
           )
        {
            ShippingId = shippingId;
            //OrderStockItems = orderStockItems;
        }
    }
}