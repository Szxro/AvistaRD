using AvistaRD.Domain.Common;

namespace AvistaRD.Domain;

public class Sighting : AuditableEntity
{
    public Sighting()
    {
        Evidences = new List<Evidence>();
        SightingThreats = new List<SightingThreat>();
    }
    
    public string UserId { get; set; }

    public int SpeciesId { get; set; }
    
    public Species Species { get; set; } = null!;

    public int ZoneId { get; set; }

    public Zone Zone { get; set; } = null!;

    public double Latitude { get; set; }
    
    public double Longitude { get; set; }

    public string Notes { get; set; } = string.Empty;

    public string Behavior { get; set; } = string.Empty;

    public ICollection<Evidence> Evidences { get; set; }

    public ICollection<SightingThreat> SightingThreats { get; set; }
}