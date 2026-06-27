using System.ComponentModel.DataAnnotations;

namespace AcopioYA.Api.Dtos;

public record LoginRequestDto(
    [Required][EmailAddress] string Email,
    [Required] string Password
);

public record LoginResponseDto(string Token, string Email, DateTime ExpiresAt);

public record RegisterAdminRequestDto(
    [Required][EmailAddress] string Email,
    [Required][MinLength(8)] string Password
);
