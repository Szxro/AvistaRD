using AvistaRD.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AvistaRD.Infrastructure.Persistence.Configuration;

public class ValidationConfiguration : IEntityTypeConfiguration<Validation>
{
    public void Configure(EntityTypeBuilder<Validation> builder)
    {
        builder.HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}