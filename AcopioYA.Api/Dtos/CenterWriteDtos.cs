using System.ComponentModel.DataAnnotations;

namespace AcopioYA.Api.Dtos;

public record CenterCreateDto(
    [Required][MaxLength(150)] string Name,
    string? Description,
    [Required][MaxLength(300)] string Address,
    [Required][MaxLength(80)] string State,
    string? Municipality,
    [Required] double Latitude,
    [Required] double Longitude,
    string? Phone,
    string? WhatsApp,
    string? OpeningHours,
    string? OrganizationName
);

public record CenterUpdateDto(
    [Required][MaxLength(150)] string Name,
    string? Description,
    [Required][MaxLength(300)] string Address,
    [Required][MaxLength(80)] string State,
    string? Municipality,
    [Required] double Latitude,
    [Required] double Longitude,
    string? Phone,
    string? WhatsApp,
    string? OpeningHours,
    string? OrganizationName,
    [Required] string Status
);

public record CenterDetailDto(
    Guid Id,
    string Name,
    string? Description,
    string Address,
    string State,
    string? Municipality,
    double Latitude,
    double Longitude,
    string? Phone,
    string? WhatsApp,
    string? OpeningHours,
    string? OrganizationName,
    string Status,
    bool IsVerified,
    DateTime? VerifiedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IEnumerable<NeedDto> Needs
);
