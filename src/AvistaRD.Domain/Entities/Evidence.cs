using AvistaRD.Domain.Common;

namespace AvistaRD.Domain;

public class Evidence : AuditableEntity
{
    public int SightingId { get; set; }

    public Sighting Sighting { get; set; } = null!;

    public string Path { get; set; } = string.Empty;

    public int Size { get; set; }

    public int Height { get; set; }
    
    public int Width { get; set; }
}