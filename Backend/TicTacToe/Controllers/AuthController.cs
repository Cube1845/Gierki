using Games.Application.Authentication.Models;
using Games.Application.Authentication.Services;
using Games.Application.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TicTacToe;

[Route("[controller]")]
[ApiController]
public class AuthController(AuthService authService) : ControllerBase
{
    private readonly AuthService _authService = authService;

    [HttpPost("[action]")]
    public async Task<Result> RegisterUser([FromBody] AppUser user)
    {
        return await _authService.RegisterUser(user);
    }

    [HttpPost("[action]")]
    public async Task<Result> Login([FromBody] LoginUser user)
    {
        return await _authService.Login(user);
    }
}
