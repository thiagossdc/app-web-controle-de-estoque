using Estoque.Api.Contracts;
using Estoque.Api.Data;
using Estoque.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Estoque.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext dbContext, JwtTokenService tokenService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Credenciais invalidas." });
        }

        var token = tokenService.GenerateToken(user);
        return Ok(new LoginResponse(token, user.Name, user.Role.ToString()));
    }
}
