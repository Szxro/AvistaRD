using AvistaRD.Domain.Common;

namespace AvistaRD.Domain;

public class Threat : AuditableEntity
{
    public Threat()
    {
        SightingThreats = new List<SightingThreat>();
    }
    
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public ICollection<SightingThreat> SightingThreats { get; set; }
}