using AcopioYA.Api.Domain.Enums;

namespace AcopioYA.Api.Domain.Entities;

public class CollectionCenter
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    // Ubicación
    public string Address { get; set; } = null!;
    public string State { get; set; } = null!;        // estado venezolano (filtro)
    public string? Municipality { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Contacto (embebido a propósito, sin tabla aparte)
    public string? Phone { get; set; }
    public string? WhatsApp { get; set; }
    public string? OpeningHours { get; set; }
    public string? OrganizationName { get; set; }

    public CenterStatus Status { get; set; } = CenterStatus.Active;

    // Confiabilidad / antifake
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedByUserId { get; set; }

    // Auditoría
    public string CreatedByUserId { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<CenterNeed> Needs { get; set; } = new List<CenterNeed>();
}
