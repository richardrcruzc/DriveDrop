﻿using DriveDrop.Core.Models.Basket;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace DriveDrop.Core.Services.Order
{
    public interface IOrderService
    {
        //Task CreateOrderAsync(Models.Orders.Order newOrder, string token);
        Task<ObservableCollection<Models.Orders.Order>> GetOrdersAsync(string token);
        Task<Models.Orders.Order> GetOrderAsync(int orderId, string token);
        Task<ObservableCollection<Models.Orders.CardType>> GetCardTypesAsync(string token);
        Task<bool> CancelOrderAsync(int orderId, string token);
        BasketCheckout MapOrderToBasket(Models.Orders.Order order);
    }
}