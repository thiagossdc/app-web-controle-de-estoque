using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Estoque.Api.Contracts;
using Estoque.Api.Data;
using Estoque.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RabbitMQ.Client;

namespace Estoque.Api.Services;

public class JwtTokenService(IConfiguration configuration)
{
    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class RabbitMqPublisher : IDisposable
{
    private readonly IConnection? _connection;
    private readonly IChannel? _channel;
    private readonly string _queueName;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _queueName = configuration["RabbitMq:QueueName"] ?? "stock-events";
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:HostName"] ?? "localhost",
            Port = int.TryParse(configuration["RabbitMq:Port"], out var port) ? port : 5672,
            UserName = configuration["RabbitMq:UserName"] ?? "guest",
            Password = configuration["RabbitMq:Password"] ?? "guest"
        };

        try
        {
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _channel.QueueDeclareAsync(_queueName, durable: true, exclusive: false, autoDelete: false)
                .GetAwaiter().GetResult();
        }
        catch
        {
            _connection = null;
            _channel = null;
        }
    }

    public async Task PublishStockEventAsync(StockTransaction transaction)
    {
        if (_channel is null)
        {
            return;
        }

        var payload = JsonSerializer.SerializeToUtf8Bytes(new
        {
            transaction.Id,
            transaction.ProductId,
            transaction.Type,
            transaction.Quantity,
            transaction.Reason,
            transaction.CreatedAtUtc
        });

        await _channel.BasicPublishAsync(exchange: "", routingKey: _queueName, body: payload);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}

public class InventoryService(AppDbContext dbContext, RabbitMqPublisher publisher)
{
    public async Task<StockTransaction> RegisterTransactionAsync(int userId, StockTransactionRequest request)
    {
        var product = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == request.ProductId)
                      ?? throw new InvalidOperationException("Produto nao encontrado.");

        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException("Quantidade deve ser maior que zero.");
        }

        var resultingStock = product.CurrentStock;
        switch (request.Type)
        {
            case StockTransactionType.Entrada:
                resultingStock += request.Quantity;
                break;
            case StockTransactionType.Saida:
                resultingStock -= request.Quantity;
                break;
            case StockTransactionType.Ajuste:
                resultingStock = request.Quantity;
                break;
            default:
                throw new InvalidOperationException("Tipo de movimentacao invalido.");
        }

        if (resultingStock < 0)
        {
            throw new InvalidOperationException("Estoque insuficiente para a saida solicitada.");
        }

        product.CurrentStock = resultingStock;
        var tx = new StockTransaction
        {
            ProductId = product.Id,
            UserId = userId,
            Type = request.Type,
            Quantity = request.Quantity,
            Reason = request.Reason,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.StockTransactions.Add(tx);
        await dbContext.SaveChangesAsync();
        await publisher.PublishStockEventAsync(tx);
        return tx;
    }
}
