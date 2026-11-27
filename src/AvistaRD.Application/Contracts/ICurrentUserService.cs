namespace AvistaRD.Application.Contracts;

public interface ICurrentUserService
{
    string? UserName { get; }

    // Identity user ids are strings (GUIDs)
    string? UserId { get; }
    
    string IpAddress { get; }
    
    string UserAgent { get; }
}