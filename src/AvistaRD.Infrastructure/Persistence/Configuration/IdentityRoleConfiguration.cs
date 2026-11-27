using AvistaRD.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AvistaRD.Infrastructure.Persistence.Configuration;

// The roles are created by a migration, so they exist wherever the migrations are applied
public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    // Fixed ids (and concurrency stamps) keep the seed stable: random values would change every migration
    public const string AdministratorRoleId = "eedda775-1352-41e7-9574-caf858748797";
    public const string ValidatorRoleId = "9e1d9be6-3fad-4dc6-9d03-f37d001c63b6";
    public const string ParkRangerRoleId = "6fac0b57-765d-4bc7-a257-b800e8d13c35";
    public const string CitizenRoleId = "495db6b9-9e46-46a6-8eb2-902af489b3c2";

    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            CreateRole(AdministratorRoleId, Roles.Administrator),
            CreateRole(ValidatorRoleId, Roles.Validator),
            CreateRole(ParkRangerRoleId, Roles.ParkRanger),
            CreateRole(CitizenRoleId, Roles.Citizen));
    }

    private static IdentityRole CreateRole(string id, string name) => new IdentityRole
    {
        Id = id,
        Name = name,
        NormalizedName = name.ToUpperInvariant(),
        ConcurrencyStamp = id
    };
}
