using AcopioYA.Api.Domain.Enums;

namespace AcopioYA.Api.Domain.Entities;

public class CenterNeed
{
    public Guid Id { get; set; }

    public Guid CollectionCenterId { get; set; }
    public CollectionCenter CollectionCenter { get; set; } = null!;

    public NeedCategory Category { get; set; }
    public string? Note { get; set; }
    public UrgencyLevel Urgency { get; set; } = UrgencyLevel.Medium;
}
