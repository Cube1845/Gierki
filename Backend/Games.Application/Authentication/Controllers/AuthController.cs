using Games.Application.Authentication.Models;
using Games.Application.Authentication.Services;
using Games.Application.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TicTacToe;

[Route("[controller]")]
[ApiController]
public class AuthController(AuthService authService, JwtService jwtService) : ControllerBase
{
    private readonly AuthService _authService = authService;
    private readonly JwtService _jwt = jwtService;

    [HttpPost("[action]")]
    public async Task<Result> RegisterUser([FromBody] RegisterDto dto)
    {
        return await _authService.RegisterUser(dto);
    }

    [HttpPost("[action]")]
    public async Task<Result<string>> Login([FromBody] LoginDto dto)
    {
        Result<string> result = await _authService.Login(dto);

        if (!result.IsSuccess)
        {
            return result;
        }

        string token = await _jwt.AddAuthenticationTokenAndGetTokenValue(dto);
        return Result<string>.Success(token, result.Message);
    }

    [Authorize]
    [HttpGet("[action]")]
    public IActionResult IsUserAuthorized()
    {
        return Ok(true);
    }
}
