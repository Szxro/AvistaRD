using AvistaRD.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AvistaRD.Infrastructure.Persistence.Configuration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(user => user.FullName)
            .HasMaxLength(200);

        // Stored as text (Pending, Active, Deactivated) so the column is readable in SQL
        builder.Property(user => user.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(user => user.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        // The administrator's list filters by status
        builder.HasIndex(user => user.Status);
    }
}
