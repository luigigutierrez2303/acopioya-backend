using System.ComponentModel.DataAnnotations;

namespace AcopioYA.Api.Dtos;

public record CenterNeedCreateDto(
    [Required] string Category,
    string? Note,
    [Required] string Urgency
);

public record CenterNeedUpdateDto(
    [Required] string Category,
    string? Note,
    [Required] string Urgency
);

public record CenterNeedResponseDto(Guid Id, string Category, string? Note, string Urgency);
