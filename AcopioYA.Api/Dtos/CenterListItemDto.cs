namespace AcopioYA.Api.Dtos;

public record CenterListItemDto(
    Guid Id,
    string Name,
    string State,
    string Address,
    double Latitude,
    double Longitude,
    string Status,
    bool IsVerified,
    string? OrganizationName,
    IEnumerable<NeedDto> Needs
);

public record NeedDto(Guid Id, string Category, string Urgency, string? Note);
