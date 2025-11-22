using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthAPI.Infrastructure.Persistence.Configurations;

public class WeatherForecastConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.UserId);

    }
}