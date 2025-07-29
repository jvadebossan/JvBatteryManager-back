using BatteryManager.Application.Users.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BatteryManager.API.DTOs;
using BatteryManager.Domain.Entities;
using BatteryManager.Infrastructure.Repositories;

namespace BatteryManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly UserRepository _userRepository;

    public AuthController(AuthService authService, UserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.AuthenticateAsync(request.Email, request.Password);
        if (token == null)
            return Unauthorized(ApiResponse<object>.ErrorResponse("Credenciais inválidas", null, "UNAUTHORIZED"));

        return Ok(ApiResponse<object>.SuccessResponse("Login realizado com sucesso", new { token }));
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var errors = request.Validate();
        if (errors.Count > 0)
            return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos", errors, "VALIDATION_ERROR"));

        // User Already Exists Verification
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            return BadRequest(ApiResponse<object>.ErrorResponse("Dados inválidos", new { email = "Email já está em uso" }, "VALIDATION_ERROR"));

        User user = new(
            request.Name,
            request.Email,
            request.Phone,
            _authService.HashPassword(request.Password)
        );

        await _authService.CreateUserAsync(user);

        user.PasswordHash = "*********";
        var responseData = new
        {
            user
        };
        return StatusCode(201, ApiResponse<object>.SuccessResponse("Usuário criado com sucesso", responseData));
    }

    [Authorize(Roles = "user,admin")]
    [HttpGet("me")]
    public IActionResult GetProfile()
    {
        var email = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
        var name = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        return Ok(new { email, name, role });
    }

    [Authorize(Roles = "admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminEndpoint()
    {
        return Ok(new { message = "You are an admin!" });
    }
}