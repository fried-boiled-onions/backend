using Backend.Models.Dtos;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var token = await _userService.Register(dto);
        if (token == null)
            return BadRequest("Username is already taken");

        return Ok(new AuthResponseDto(token));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _userService.Login(dto);
        if (token == null)
            return Unauthorized("Invalid credentials");

        return Ok(new AuthResponseDto(token));
    }
}