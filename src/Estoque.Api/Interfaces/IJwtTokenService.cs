using Estoque.Api.Domain;

namespace Estoque.Api.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}