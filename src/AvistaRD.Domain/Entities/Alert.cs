using AvistaRD.Domain.Common;

namespace AvistaRD.Domain;

public class Alert : AuditableEntity
{
    public int ZoneId { get; set; }

    public Zone Zone { get; set; } = null!;

    public string Level { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;
}