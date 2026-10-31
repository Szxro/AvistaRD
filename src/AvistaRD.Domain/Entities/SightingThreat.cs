namespace AvistaRD.Domain;

public class SightingThreat
{
    public int ThreatId { get; set; }

    public Threat Threat { get; set; } = null!;

    public int SightingId { get; set; }

    public Sighting Sighting { get; set; } = null!;

    public string Description { get; set; } = string.Empty;
}