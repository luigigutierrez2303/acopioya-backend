using AcopioYA.Api.Domain.Entities;
using AcopioYA.Api.Dtos;
using AcopioYA.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AcopioYA.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, TokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null) return Unauthorized("Credenciales inválidas.");

        var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!valid) return Unauthorized("Credenciales inválidas.");

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _tokenService.GenerateToken(user, roles);

        return Ok(new LoginResponseDto(token, user.Email!, expiresAt));
    }

    [HttpPost("register-admin")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> RegisterAdmin(RegisterAdminRequestDto dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing is not null) return Conflict("Ya existe un usuario con ese email.");

        var newAdmin = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
        var result = await _userManager.CreateAsync(newAdmin, dto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(newAdmin, "Admin");
        return Ok(new { message = "Admin creado correctamente.", email = dto.Email });
    }
}
