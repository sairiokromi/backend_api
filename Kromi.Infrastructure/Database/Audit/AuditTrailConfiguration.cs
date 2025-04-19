using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kromi.Infrastructure.Database.Audit
{
    public class AuditTrailConfiguration : IEntityTypeConfiguration<AuditTrail>
    {
        public void Configure(EntityTypeBuilder<AuditTrail> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasIndex(e => e.EntityName);
            builder.Property(e => e.Id);
            builder.Property(e => e.UserId);
            builder.Property(e => e.EntityName).HasMaxLength(100).IsRequired();
            builder.Property(e => e.DateUtc).IsRequired();
            builder.Property(e => e.PrimaryKey).HasMaxLength(100);

            builder.Property(e => e.TrailType).HasConversion<string>();
            builder.Property(e => e.ChangedColumns).HasConversion<string>().HasColumnType("Nvarchar(max)");
            builder.Property(e => e.OldValues).HasConversion<string>().HasColumnType("Nvarchar(max)");
            builder.Property(e => e.NewValues).HasConversion<string>().HasColumnType("Nvarchar(max)");

            /*builder.Property(e => e.ChangedColumns)HasColumnType("jsonb");
            builder.Property(e => e.OldValues).HasColumnType("jsonb");
            builder.Property(e => e.NewValues).HasColumnType("jsonb");*/
        }
    }
}
