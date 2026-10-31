namespace AvistaRD.Domain;

public class Validation
{
    public int SightingId { get; set; }

    public Sighting Sighting { get; set; } = null!;

    public int UserId { get; set; }

    public int ValidationStatusId { get; set; }

    public ValidationStatus ValidationStatus { get; set; } = null!;

    public string Commentary { get; set; } = string.Empty;
}