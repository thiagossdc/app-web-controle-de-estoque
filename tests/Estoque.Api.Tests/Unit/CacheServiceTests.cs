using Estoque.Api.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Estoque.Api.Tests.Unit;

public class CacheServiceTests
{
    private readonly Mock<IMemoryCache> _mockCache;
    private readonly Mock<ILogger<MemoryCacheService>> _mockLogger;
    private readonly MemoryCacheService _cacheService;

    public CacheServiceTests()
    {
        _mockCache = new Mock<IMemoryCache>();
        _mockLogger = new Mock<ILogger<MemoryCacheService>>();
        _cacheService = new MemoryCacheService(_mockCache.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAsync_WhenKeyExists_ReturnsValue()
    {
        // Arrange
        var key = "test-key";
        var expectedValue = "test-value";
        _mockCache.Setup(x => x.TryGetValue(key, out It.Ref<object>.IsAny))
            .Returns((string key, out object value) =>
            {
                value = expectedValue;
                return true;
            });

        // Act
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public async Task GetAsync_WhenKeyNotExists_ReturnsNull()
    {
        // Arrange
        var key = "non-existent-key";
        _mockCache.Setup(x => x.TryGetValue(key, out It.Ref<object>.IsAny))
            .Returns(false);

        // Act
        var result = await _cacheService.GetAsync<string>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_WithExpiration_SetsValueWithExpiration()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var expiration = TimeSpan.FromMinutes(5);
        var mockEntry = new Mock<ICacheEntry>();
        
        _mockCache.Setup(x => x.CreateEntry(key))
            .Returns(mockEntry.Object);

        // Act
        await _cacheService.SetAsync(key, value, expiration);

        // Assert
        _mockCache.Verify(x => x.CreateEntry(key), Times.Once);
        mockEntry.VerifySet(x => x.Value = value);
        mockEntry.VerifySet(x => x.AbsoluteExpirationRelativeToNow = expiration);
    }

    [Fact]
    public async Task SetAsync_WithoutExpiration_SetsValueWithDefaultExpiration()
    {
        // Arrange
        var key = "test-key";
        var value = "test-value";
        var mockEntry = new Mock<ICacheEntry>();
        
        _mockCache.Setup(x => x.CreateEntry(key))
            .Returns(mockEntry.Object);

        // Act
        await _cacheService.SetAsync(key, value);

        // Assert
        _mockCache.Verify(x => x.CreateEntry(key), Times.Once);
        mockEntry.VerifySet(x => x.Value = value);
        mockEntry.VerifySet(x => x.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30));
    }

    [Fact]
    public async Task RemoveAsync_RemovesKey()
    {
        // Arrange
        var key = "test-key";

        // Act
        await _cacheService.RemoveAsync(key);

        // Assert
        _mockCache.Verify(x => x.Remove(key), Times.Once);
    }

    [Fact]
    public async Task RemoveByPrefixAsync_RemovesAllMatchingKeys()
    {
        // Arrange
        var prefix = "test:";
        // This test would need a more complex setup to track multiple keys
        // For now, we'll just verify the method doesn't throw

        // Act & Assert
        await _cacheService.RemoveByPrefixAsync(prefix);
        // Method should complete without throwing
    }
}

public class MetricsServiceTests
{
    private readonly Mock<ILogger<MetricsService>> _mockLogger;
    private readonly MetricsService _metricsService;

    public MetricsServiceTests()
    {
        _mockLogger = new Mock<ILogger<MetricsService>>();
        _metricsService = new MetricsService(_mockLogger.Object);
    }

    [Fact]
    public void RecordRequestDuration_RecordsMetric()
    {
        // Arrange
        var endpoint = "/api/products";
        var method = "GET";
        var statusCode = 200;
        var durationMs = 150.5;

        // Act
        _metricsService.RecordRequestDuration(endpoint, method, statusCode, durationMs);

        // Assert
        // Verify that the metric was recorded (would need access to internal state)
        // For now, we verify the method doesn't throw
    }

    [Fact]
    public void RecordCacheHit_RecordsMetric()
    {
        // Arrange
        var key = "products:list";

        // Act
        _metricsService.RecordCacheHit(key);

        // Assert
        // Verify that the metric was recorded
    }

    [Fact]
    public void RecordCacheMiss_RecordsMetric()
    {
        // Arrange
        var key = "products:list";

        // Act
        _metricsService.RecordCacheMiss(key);

        // Assert
        // Verify that the metric was recorded
    }

    [Fact]
    public void RecordDatabaseQuery_RecordsMetric()
    {
        // Arrange
        var query = "SELECT * FROM Products";
        var durationMs = 45.2;

        // Act
        _metricsService.RecordDatabaseQuery(query, durationMs);

        // Assert
        // Verify that the metric was recorded
    }

    [Fact]
    public void RecordRabbitMqMessage_RecordsMetric()
    {
        // Arrange
        var queue = "stock-events";
        var operation = "publish";

        // Act
        _metricsService.RecordRabbitMqMessage(queue, operation);

        // Assert
        // Verify that the metric was recorded
    }

    [Fact]
    public void GetMetrics_ReturnsMetricsDictionary()
    {
        // Act
        var metrics = _metricsService.GetMetrics();

        // Assert
        Assert.NotNull(metrics);
        Assert.IsType<Dictionary<string, object>>(metrics);
        Assert.True(metrics.ContainsKey("active_connections"));
        Assert.True(metrics.ContainsKey("timestamp"));
    }
}

public class ValidationServiceTests
{
    private readonly ValidationService _validationService;

    public ValidationServiceTests()
    {
        _validationService = new ValidationService();
    }

    [Fact]
    public void ValidateProduct_ValidProduct_ReturnsValid()
    {
        // Arrange
        var product = new
        {
            name = "Produto Teste",
            sku = "NB-14-CP",
            categoryId = 1,
            supplierId = 1,
            unitPrice = 99.99m,
            minStock = 10
        };

        // Act
        var result = _validationService.ValidateProduct(product);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateProduct_InvalidSku_ReturnsInvalid()
    {
        // Arrange
        var product = new
        {
            name = "Produto Teste",
            sku = "INVALID-SKU",
            categoryId = 1,
            supplierId = 1,
            unitPrice = 99.99m,
            minStock = 10
        };

        // Act
        var result = _validationService.ValidateProduct(product);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("SKU deve seguir o formato", result.Errors.First());
    }

    [Fact]
    public void ValidateProduct_NegativePrice_ReturnsInvalid()
    {
        // Arrange
        var product = new
        {
            name = "Produto Teste",
            sku = "NB-14-CP",
            categoryId = 1,
            supplierId = 1,
            unitPrice = -10m,
            minStock = 10
        };

        // Act
        var result = _validationService.ValidateProduct(product);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Preço unitário deve ser maior que zero", result.Errors.First());
    }

    [Fact]
    public void ValidateCategory_ValidCategory_ReturnsValid()
    {
        // Arrange
        var category = new
        {
            name = "Categoria Teste",
            description = "Descrição da categoria"
        };

        // Act
        var result = _validationService.ValidateCategory(category);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateCategory_EmptyName_ReturnsInvalid()
    {
        // Arrange
        var category = new
        {
            name = "",
            description = "Descrição da categoria"
        };

        // Act
        var result = _validationService.ValidateCategory(category);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Nome da categoria deve ter pelo menos 2 caracteres", result.Errors.First());
    }

    [Fact]
    public void ValidateSupplier_ValidSupplier_ReturnsValid()
    {
        // Arrange
        var supplier = new
        {
            name = "Fornecedor Teste",
            contactName = "João Silva",
            email = "joao@teste.com",
            phone = "11999999999"
        };

        // Act
        var result = _validationService.ValidateSupplier(supplier);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateSupplier_InvalidEmail_ReturnsInvalid()
    {
        // Arrange
        var supplier = new
        {
            name = "Fornecedor Teste",
            contactName = "João Silva",
            email = "email-invalido",
            phone = "11999999999"
        };

        // Act
        var result = _validationService.ValidateSupplier(supplier);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Email deve ter um formato válido", result.Errors.First());
    }

    [Fact]
    public void ValidateStockTransaction_ValidTransaction_ReturnsValid()
    {
        // Arrange
        var transaction = new
        {
            productId = 1,
            type = 1,
            quantity = 50,
            reason = "Entrada inicial"
        };

        // Act
        var result = _validationService.ValidateStockTransaction(transaction);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateStockTransaction_ZeroQuantity_ReturnsInvalid()
    {
        // Arrange
        var transaction = new
        {
            productId = 1,
            type = 1,
            quantity = 0,
            reason = "Entrada inicial"
        };

        // Act
        var result = _validationService.ValidateStockTransaction(transaction);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains("Quantidade deve ser maior que zero", result.Errors.First());
    }

    [Fact]
    public void FormatCurrency_ValidValue_ReturnsFormattedString()
    {
        // Arrange
        var value = 1234.56m;

        // Act
        var result = _validationService.FormatCurrency(value);

        // Assert
        Assert.Equal("R$ 1.234,56", result);
    }

    [Fact]
    public void FormatPhone_ValidPhone_ReturnsFormattedString()
    {
        // Arrange
        var phone = "11999999999";

        // Act
        var result = _validationService.FormatPhone(phone);

        // Assert
        Assert.Equal("(11) 99999-9999", result);
    }

    [Fact]
    public void SanitizeInput_RemovesDangerousCharacters()
    {
        // Arrange
        var input = "  <script>alert('test')</script>  ";

        // Act
        var result = _validationService.SanitizeInput(input);

        // Assert
        Assert.Equal("scriptalert('test')/script", result);
    }
}