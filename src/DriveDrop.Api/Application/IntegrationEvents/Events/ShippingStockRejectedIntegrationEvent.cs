namespace DriveDrop.Api.Application.IntegrationEvents.Events
{
    using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;
    using System.Collections.Generic;

    public class ShippingStockRejectedIntegrationEvent : IntegrationEvent
    {
        public int ShippingId { get; }

        //public List<ConfirmedOrderStockItem> OrderStockItems { get; }

        public ShippingStockRejectedIntegrationEvent(int shippingId
           // List<ConfirmedOrderStockItem> orderStockItems
           )
        {
            ShippingId = shippingId;
            //OrderStockItems = orderStockItems;
        }
    }

    //public class ConfirmedOrderStockItem
    //{
    //    public int ProductId { get; }
    //    public bool HasStock { get; }

    //    public ConfirmedOrderStockItem(int productId, bool hasStock)
    //    {
    //        ProductId = productId;
    //        HasStock = hasStock;
    //    }
    //}
}