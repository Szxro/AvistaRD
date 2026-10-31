using AvistaRD.Domain.Common;

namespace AvistaRD.Domain;

public class Zone : AuditableEntity
{
    public Zone()
    {
        Sightings = new List<Sighting>();
        Alerts = new List<Alert>();
    }
    
    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<Sighting> Sightings { get; set; }

    public ICollection<Alert> Alerts { get; set; }
}