using Estoque.Api.Contracts;
using Estoque.Api.Data;
using Estoque.Api.Domain;
using Estoque.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Estoque.Api.Tests.Unit;

public class InventoryServiceTests
{
    [Fact]
    public async Task RegisterTransaction_WhenInsufficientStock_ThrowsException()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"InventoryServiceTests-{Guid.NewGuid()}")
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();
        var publisher = new RabbitMqPublisher(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["RabbitMq:HostName"] = "localhost",
            ["RabbitMq:QueueName"] = "stock-events"
        }).Build());
        var service = new InventoryService(db, publisher);

        var request = new StockTransactionRequest(
            ProductId: 1,
            Type: StockTransactionType.Saida,
            Quantity: 9999,
            Reason: "Teste de estoque insuficiente");

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegisterTransactionAsync(1, request));
    }
}
