using AvistaRD.Domain.Common;

namespace AvistaRD.Domain;

public class ValidationStatus : AuditableEntity
{
    public ValidationStatus()
    {
        Validations = new List<Validation>();
    }
    
    public string Name { get; set; } = string.Empty;

    public ICollection<Validation> Validations { get; set; }
}