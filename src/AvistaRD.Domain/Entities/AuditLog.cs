namespace AvistaRD.Domain;

public class AuditLog
{
    public int Id { get; set; }
    
    public string UserId { get; set; } =  string.Empty;
    
    public string RequestName { get; set; } = null!; // CommandName, QueryName

    public string? RequestData { get; set; } // Payload

    public string? ResourceName { get; set; } // refer to the entity (User)
    
    public string? ResourceId { get; set; }  // id of the entity

    public string IpAddress { get; init; } = string.Empty;

    public string UserAgent { get; init; } = string.Empty; // refer to the device that the user used
    
    public bool IsSuccessful { get; set; }

    public string? ErrorMessage { get; set; }

    public long? ExecutionTimeInMs { get; set; }
    
    public DateTime CreatedAt { get; init; }
}