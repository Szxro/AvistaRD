using AvistaRD.Domain.Common;

namespace AvistaRD.Domain;

public class Species : AuditableEntity
{
    public Species()
    {
        Sightings = new List<Sighting>();
    }
    
    public string CommonName { get; set; } = string.Empty;

    public string ScientificName { get; set; } =  string.Empty;

    public bool IsEndemic { get; set; }

    public bool IsSensible { get; set; }

    public ICollection<Sighting> Sightings { get; set; }
}