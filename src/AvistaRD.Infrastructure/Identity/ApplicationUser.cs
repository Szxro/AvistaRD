using AvistaRD.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace AvistaRD.Infrastructure.Identity;

// Identity user (table AspNetUsers) with the data the platform needs to manage accounts
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public UserStatus Status { get; set; } = UserStatus.Pending;

    // Filled by the database (SYSUTCDATETIME) when the account is created
    public DateTime CreatedAt { get; set; }
}
