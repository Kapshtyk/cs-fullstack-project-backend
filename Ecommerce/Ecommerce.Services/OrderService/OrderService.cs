using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Services.Common;
using Ecommerce.Services.OrderService.DTO;
using Ecommerce.Services.OrderService.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Ecommerce.Services.OrderService
{
    public class OrderService(
        IOrderRepo orderRepo,
        IDistributedCache cache
        ) :
        BaseService<Order, OrderFilterOptions, GetOrderDto, CreateOrderDto, UpdateOrderDto, PartialUpdateOrderDto>(orderRepo, cache),
        IOrderService
    { }
}