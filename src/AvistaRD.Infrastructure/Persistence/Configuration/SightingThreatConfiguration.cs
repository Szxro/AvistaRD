using AvistaRD.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AvistaRD.Infrastructure.Persistence.Configuration;

public class SightingThreatConfiguration : IEntityTypeConfiguration<SightingThreat>
{
    public void Configure(EntityTypeBuilder<SightingThreat> builder)
    {
        builder.HasKey(x => new { x.SightingId, x.ThreatId });
    }
}