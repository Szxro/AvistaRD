using AvistaRD.Domain;
using AvistaRD.Domain.Entities;
using AvistaRD.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AvistaRD.Infrastructure.Persistence.Configuration;

public class SightingConfiguration : IEntityTypeConfiguration<Sighting> 
{
    public void Configure(EntityTypeBuilder<Sighting> builder)
    {
        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}