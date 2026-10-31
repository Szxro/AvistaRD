using AvistaRD.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AvistaRD.Infrastructure.Persistence.Configuration;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}