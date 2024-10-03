using Moq;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Services.CartItemService;
using Ecommerce.Services.CartItemService.DTO;
using Ecommerce.Domain.Models;
using Ecommerce.Domain.Common.Exceptions;
using Ecommerce.Domain.Filters;
using Ecommerce.Services.Common;
using Microsoft.Extensions.Caching.Distributed;

namespace Ecommerce.Tests.Services
{
    public class CartItemServiceTest
    {
        private readonly Mock<ICartItemRepo> _mockCartItemRepo;
        private readonly CartItemService _cartItemService;
        private readonly Mock<IDistributedCache> _mockCache;

        public CartItemServiceTest()
        {
            _mockCartItemRepo = new Mock<ICartItemRepo>();
            _mockCache = new Mock<IDistributedCache>();
            _cartItemService = new CartItemService(_mockCartItemRepo.Object, _mockCache.Object);
        }

        [Fact]
        public async Task PartialUpdateByIdAsync_WhenCalledWithPartialUpdateDtoAndCartItemNotFound_ThrowsException()
        {
            _mockCartItemRepo
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new CartItemNotFoundExcepiton());

            var dto = new PartialUpdateCartItemDto
            {
                Quantity = 3
            };

            await Assert.ThrowsAsync<CartItemNotFoundExcepiton>(() => _cartItemService.UpdateAsync(dto, 1));

            _mockCartItemRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _mockCartItemRepo.Verify(repo => repo.UpdateAsync(It.IsAny<CartItem>()), Times.Never);
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenCalledWithExistingId_ReturnsTrue()
        {
            _mockCartItemRepo
                .Setup(repo => repo.DeleteByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var result = await _cartItemService.DeleteByIdAsync(1);

            Assert.True(result);

            _mockCartItemRepo.Verify(repo => repo.DeleteByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenCalledWithWrongId_ReturnsFalse()
        {
            _mockCartItemRepo
                .Setup(repo => repo.DeleteByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var result = await _cartItemService.DeleteByIdAsync(1);

            Assert.False(result);

            _mockCartItemRepo.Verify(repo => repo.DeleteByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalledWithExistingId_ReturnsGetDtoAsync()
        {
            var cartItem = new CartItem
            {
                Id = 2,
                ProductId = 1,
                Quantity = 2,
                UserId = 1,
                User = new User
                {
                    Id = 1,
                    Email = "test@email.com",
                    FirstName = "John",
                    LastName = "Doe",
                    Password = "hashedpassword",
                    Role = Role.User,

                    CartItems = null!,
                    Orders = null!,
                    SaltUser = null!,
                },
                Product = new Product
                {
                    Id = 1,
                    Title = "Product",
                    Description = "Product Description",
                    Price = 99,
                    CategoryId = 1,
                    Stock = 12,
                    Category = null!,
                    Reviews = null!,
                }
            };

            _mockCartItemRepo
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(cartItem);

            var result = await _cartItemService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.IsType<GetCartItemDto>(result);
            Assert.Equal(2, result.Id);
            Assert.Equal(2, result.Quantity);
            Assert.Equal(1, result.UserId);

            _mockCartItemRepo.Verify(repo => repo.GetByIdAsync(1), Times.AtMost(2));
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalledWithWrongId_ThrowsException()
        {
            _mockCartItemRepo
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new CartItemNotFoundExcepiton());

            await Assert.ThrowsAsync<CartItemNotFoundExcepiton>(() => _cartItemService.GetByIdAsync(1));

            _mockCartItemRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }
    }
}