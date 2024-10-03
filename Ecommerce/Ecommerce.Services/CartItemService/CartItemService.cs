using Ecommerce.Domain.Filters;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Models;
using Ecommerce.Services.CartItemService.DTO;
using Ecommerce.Services.CartItemService.Interfaces;
using Ecommerce.Services.Common;
using Microsoft.Extensions.Caching.Distributed;

namespace Ecommerce.Services.CartItemService
{
    public class CartItemService(
        ICartItemRepo cartItemRepo,
        IDistributedCache cache
        ) :
        BaseService<CartItem, CartItemFilterOptions, GetCartItemDto, CreateCartItemDto, UpdateCartItemDto, PartialUpdateCartItemDto>(cartItemRepo, cache),
        ICartItemService
    { }
}