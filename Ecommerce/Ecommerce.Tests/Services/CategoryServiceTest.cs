using Moq;
using Ecommerce.Domain.Interfaces;
using Ecommerce.Services.CategoryService;
using Ecommerce.Services.CategoryService.DTO;
using Ecommerce.Domain.Models;
using Microsoft.AspNetCore.Http;
using Ecommerce.Domain.Common.Exceptions;
using Ecommerce.Domain.Filters;
using Ecommerce.Services.Common;
using Microsoft.Extensions.Caching.Distributed;
using Ecommerce.Services.Common.Interfaces;

namespace Ecommerce.Tests.Services
{
    public class CategoryServiceTest
    {
        private readonly Mock<ICategoryRepo> _mockCategoryRepo;
        private readonly CategoryService _categoryService;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly Mock<IFileService> _mockFileService;

        public CategoryServiceTest()
        {
            _mockCategoryRepo = new Mock<ICategoryRepo>();
            _mockCache = new Mock<IDistributedCache>();
            _mockFileService = new Mock<IFileService>();
            _categoryService = new CategoryService(_mockCategoryRepo.Object, _mockCache.Object, _mockFileService.Object);
        }

        [Fact]
        public async Task PartialUpdateByIdAsync_WhenCalledWithPartialUpdateDto_ReturnsGetDto()
        {
            var category = new Category
            {
                Id = 2,
                Name = "Category1",
                CategoryImage = "Image1",
                Products = [],
                SubCategories = []
            };

            _mockCategoryRepo
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(category);

            _mockCategoryRepo
                .Setup(repo => repo.UpdateAsync(It.IsAny<Category>()))
                .ReturnsAsync((Category category) => category);

            var dto = new PartialUpdateCategoryDto
            {
                Name = "Category2",
                CategoryImagePath = "Image2",
                ParentCategoryId = 1,
            };

            var result = await _categoryService.UpdateAsync(dto, 1);

            Assert.NotNull(result);
            Assert.IsType<GetCategoryDto>(result);
            Assert.Equal("Category2", result.Name);
            Assert.Equal("Image2", result.CategoryImage);
            Assert.Equal(1, result.ParentCategoryId);

            _mockCategoryRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _mockCategoryRepo.Verify(repo => repo.UpdateAsync(category), Times.Once);
        }

        [Fact]
        public async Task PartialUpdateByIdAsync_WhenCalledWithPartialUpdateDtoAndCategoryNotFound_ThrowsException()
        {
            _mockCategoryRepo
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new CategoryNotFoundException());

            var dto = new PartialUpdateCategoryDto
            {
                Name = "Category2",
                CategoryImagePath = "Image2",
                ParentCategoryId = 1,
            };

            await Assert.ThrowsAsync<CategoryNotFoundException>(() => _categoryService.UpdateAsync(dto, 1));

            _mockCategoryRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
            _mockCategoryRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenCalledWithExistingId_ReturnsTrue()
        {
            _mockCategoryRepo
                .Setup(repo => repo.DeleteByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(true);

            var result = await _categoryService.DeleteByIdAsync(1);

            Assert.True(result);

            _mockCategoryRepo.Verify(repo => repo.DeleteByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenCalledWithWrongId_ReturnsFalse()
        {
            _mockCategoryRepo
                .Setup(repo => repo.DeleteByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(false);

            var result = await _categoryService.DeleteByIdAsync(1);

            Assert.False(result);

            _mockCategoryRepo.Verify(repo => repo.DeleteByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalledWithExistingId_ReturnsGetDtoAsync()
        {
            var category = new Category
            {
                Id = 1,
                Name = "Category",
                CategoryImage = "Image",
                Products = [],
                SubCategories = []
            };

            _mockCategoryRepo
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(category);

            var result = await _categoryService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.IsType<GetCategoryDto>(result);
            Assert.Equal("Category", result.Name);
            Assert.Equal("Image", result.CategoryImage);

            _mockCategoryRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalledWithWrongId_ThrowsException()
        {
            _mockCategoryRepo
                .Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new CategoryNotFoundException());

            await Assert.ThrowsAsync<CategoryNotFoundException>(() => _categoryService.GetByIdAsync(1));

            _mockCategoryRepo.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnsPaginatedResult()
        {
            var category = new Category
            {
                Id = 1,
                Name = "Category",
                CategoryImage = "Image",
                Products = [],
                SubCategories = []
            };

            _mockCategoryRepo
                .Setup(repo => repo.GetAllAsync(It.IsAny<CategoryFilterOptions>()))
                .ReturnsAsync([category]);

            _mockCategoryRepo
                .Setup(repo => repo.CountAsync(It.IsAny<CategoryFilterOptions>()))
                .ReturnsAsync(1);

            var filteringOptions = new CategoryFilterOptions
            {
                Page = 1,
                PerPage = 10
            };

            var result = await _categoryService.GetAllAsync(filteringOptions);

            Assert.NotNull(result);
            Assert.IsType<PaginatedResult<Category, GetCategoryDto>>(result);
            Assert.Equal(10, result.ItemsPerPage);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(1, result.TotalItems);
            Assert.Single(result.Items);
            Assert.Equal("Category", result.Items.First().Name);
            Assert.Equal("Image", result.Items.First().CategoryImage);

            _mockCategoryRepo.Verify(repo => repo.GetAllAsync(filteringOptions), Times.Once);
        }
    }
}