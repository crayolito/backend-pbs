using BancoSol.Application.DTOs;
using BancoSol.Application.Exceptions;
using BancoSol.Application.Services;
using BancoSol.Domain.Entities;
using BancoSol.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace BancoSol.Application.Tests;

public sealed class ProductServiceTests
{
    private const string ProductsListCacheKey = "products_list";

    [Fact]
    public async Task GetAllAsync_DebeLanzarBadRequest_CuandoMinPriceEsMayorQueMaxPrice()
    {
        // Mock del repositorio 
        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);

        // Cache en memoria real para la prueba
        using var cache = new MemoryCache(new MemoryCacheOptions());

        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(repoMock.Object, cache, logger);

        var query = new ProductQueryDto { MinPrice = 100, MaxPrice = 50 };

        await Assert.ThrowsAsync<BadRequestException>(() => service.GetAllAsync(query));

        repoMock.Verify(r => r.GetAllAsync(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<bool?>(),
            It.IsAny<decimal?>(),
            It.IsAny<decimal?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePriceAsync_DebeLanzarNotFound_CuandoProductoNoExiste()
    {
        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(repoMock.Object, cache, logger);

        var id = Guid.NewGuid();

        var request = new UpdatePriceDto { Price = 120, Currency = "BOB" };

        repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdatePriceAsync(id, request));

        repoMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        repoMock.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePriceAsync_DebeActualizarYLimpiarCache_CuandoRequestEsValido()
    {
        var repoMock = new Mock<IProductRepository>(MockBehavior.Strict);
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var logger = NullLogger<ProductService>.Instance;
        var service = new ProductService(repoMock.Object, cache, logger);
        
        var id = Guid.NewGuid();

        var product = new Product
        {
            Id = id,
            Sku = "SKU-001",
            Name = "Producto",
            Stock = 10,
            Price = 50,
            Currency = "BOB"
        };

        var request = new UpdatePriceDto
        {
            Price = 75,
            Currency = "EUR"
        };

        // El repo encuentra el producto
        repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        
        // Simulamos que guardar en repo fue exitoso
        repoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        // Precargamos cache para comprobar que se borra al actualizar precio
        cache.Set(ProductsListCacheKey, new PagedResultDto<ProductDto>());

        var result = await service.UpdatePriceAsync(id, request);

        // El DTO de salida debe tener los nuevos valores
        Assert.Equal(75, result.Price);
        Assert.Equal("EUR", result.Currency);


        // Verificamos que realmente mandó al repo el producto modificado
        repoMock.Verify(r => r.UpdateAsync(
            It.Is<Product>(p => p.Id == id && p.Price == 75 && p.Currency == "EUR"),
            It.IsAny<CancellationToken>()), Times.Once);

        // Al actualizar precio la cache debe eliminarse
        Assert.False(cache.TryGetValue(ProductsListCacheKey, out _));
    }

}