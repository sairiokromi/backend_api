using Kromi.Domain.Entities;
using Kromi.Domain.Entities.Contract;
using Kromi.Domain.Entities.StoredProcedures;
using Kromi.Infrastructure.Contracts.Identity;
using Kromi.Infrastructure.Database.Audit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace Kromi.Infrastructure.Database.Persistence
{
    public class KromiContext : IdentityDbContext<KromiUser>
    {
        public virtual DbSet<KromiUser> KromiUsers { get; set; } = null!;
        public virtual DbSet<AuditTrail> AuditTrails { get; set; }
        public virtual DbSet<UserRefreshTokens> UserRefreshTokens { get; set; }
        public virtual DbSet<PreguntaSeguridad> PreguntasSeguridad { get; set; } = null!;
        public virtual DbSet<PreguntaSeguridadUsuario> PreguntaSeguridadUsuarios { get; set; } = null!;
        public virtual DbSet<Sucursal> Sucursal { get; set; } = null!;

        #region Stored procedures
        public virtual DbSet<GenericStringString> GenericStringString { get; set; }
        #endregion

        private readonly IJwtService _currentSessionProvider;
        private readonly JsonSerializerOptions _jsonConfig = new(){ PropertyNameCaseInsensitive = true };

        public KromiContext(DbContextOptions<KromiContext> options,
            IJwtService currentSessionProvider) : base(options)
        {
            _currentSessionProvider = currentSessionProvider;
        }

        public KromiContext(DbContextOptions<KromiContext> options) : base(options)
        { }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            string? sessionId = _currentSessionProvider?.GetSessionUserId();
            Guid? userId = !string.IsNullOrWhiteSpace(sessionId) ? Guid.Parse(sessionId) : null;

            if (userId != null)
            {
                SetAuditableProperties(userId);

                var auditEntries = HandleAuditingBeforeSaveChanges(userId).ToList();
                if (auditEntries.Count > 0)
                {
                    await AuditTrails.AddRangeAsync(auditEntries, cancellationToken);
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        private List<AuditTrail> HandleAuditingBeforeSaveChanges(Guid? userId)
        {
            var auditableEntries = ChangeTracker.Entries<IAuditable>()
                .Where(x => x.State is EntityState.Added or EntityState.Deleted or EntityState.Modified)
                .Select(x => CreateTrailEntry(userId, x))
                .ToList();

            return auditableEntries;
        }

        private static AuditTrail CreateTrailEntry(Guid? userId, EntityEntry<IAuditable> entry)
        {
            var trailEntry = new AuditTrail
            {
                Id = Guid.NewGuid(),
                EntityName = entry.Entity.GetType().Name,
                UserId = userId,
                DateUtc = DateTime.UtcNow
            };

            SetAuditTrailPropertyValues(entry, trailEntry);
            SetAuditTrailNavigationValues(entry, trailEntry);
            SetAuditTrailReferenceValues(entry, trailEntry);

            return trailEntry;
        }

        /// <summary>
        /// Sets auditable properties for entities that are inherited from <see cref="IAuditableEntity"/>
        /// </summary>
        /// <param name="userId">User identifier that performed an action</param>
        /// <returns>Collection of auditable entities</returns>
        private void SetAuditableProperties(Guid? userId)
        {
            const string systemSource = "system";
            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy = userId?.ToString() ?? systemSource;
                        break;

                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedBy = userId?.ToString() ?? systemSource;
                        break;
                }
            }
        }

        /// <summary>
        /// Sets column values to audit trail entity
        /// </summary>
        /// <param name="entry">Current entity entry ef core model</param>
        /// <param name="trailEntry">Audit trail entity</param>
        private static void SetAuditTrailPropertyValues(EntityEntry entry, AuditTrail trailEntry)
        {
            // Skip temp fields (that will be assigned automatically by ef core engine, for example: when inserting an entity
            foreach (var property in entry.Properties.Where(x => !x.IsTemporary))
            {
                if (property.Metadata.IsPrimaryKey())
                {
                    trailEntry.PrimaryKey = property.CurrentValue?.ToString();
                    continue;
                }

                // Filter properties that should not appear in the audit list
                if (property.Metadata.Name.Equals("PasswordHash"))
                {
                    continue;
                }

                SetAuditTrailPropertyValue(entry, trailEntry, property);
            }
        }

        /// <summary>
        /// Sets a property value to the audit trail entity
        /// </summary>
        /// <param name="entry">Current entity entry ef core model</param>
        /// <param name="trailEntry">Audit trail entity</param>
        /// <param name="property">Entity property ef core model</param>
        private static void SetAuditTrailPropertyValue(EntityEntry entry, AuditTrail trailEntry, PropertyEntry property)
        {
            var propertyName = property.Metadata.Name;

            switch (entry.State)
            {
                case EntityState.Added:
                    trailEntry.TrailType = TrailType.Create;
                    trailEntry.NewValues[propertyName] = property.CurrentValue;

                    break;

                case EntityState.Deleted:
                    trailEntry.TrailType = TrailType.Delete;
                    trailEntry.OldValues[propertyName] = property.OriginalValue;

                    break;

                case EntityState.Modified:
                    if (property.IsModified && (property.OriginalValue is null || !property.OriginalValue.Equals(property.CurrentValue)))
                    {
                        trailEntry.ChangedColumns.Add(propertyName);
                        trailEntry.TrailType = TrailType.Update;
                        trailEntry.OldValues[propertyName] = property.OriginalValue;
                        trailEntry.NewValues[propertyName] = property.CurrentValue;
                    }

                    break;
            }

            if (trailEntry.ChangedColumns.Count > 0)
            {
                trailEntry.TrailType = TrailType.Update;
            }
        }

        private static void SetAuditTrailNavigationValues(EntityEntry entry, AuditTrail trailEntry)
        {
            foreach (var navigation in entry.Navigations.Where(x => x.Metadata.IsCollection && x.IsModified))
            {
                if (navigation.CurrentValue is not IEnumerable<object> enumerable)
                {
                    continue;
                }

                var collection = enumerable.ToList();
                if (collection.Count == 0)
                {
                    continue;
                }

                var navigationName = collection.First().GetType().Name;
                trailEntry.ChangedColumns.Add(navigationName);
            }
        }

        private static void SetAuditTrailReferenceValues(EntityEntry entry, AuditTrail trailEntry)
        {
            foreach (var reference in entry.References.Where(x => x.IsModified))
            {
                var referenceName = reference.EntityEntry.Entity.GetType().Name;
                trailEntry.ChangedColumns.Add(referenceName);
            }
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is ITimeStamp && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow;

                if (entity.State == EntityState.Added)
                {
                    ((ITimeStamp)entity.Entity).CreatedAt = now;
                }
                ((ITimeStamp)entity.Entity).UpdatedAt = now;
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<KromiUser>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Nombres).HasMaxLength(100);
                entity.Property(x => x.Apellidos).HasMaxLength(100);
                entity.Property(x => x.Firma).HasMaxLength(150);
                entity.Property(x => x.PhoneNumber).HasMaxLength(20);
                entity.Property(x => x.Ficha).HasMaxLength(40);
                entity.Property(x => x.Id).HasMaxLength(36);
                entity.Property(x => x.NormalizedUserName).HasMaxLength(90);

                entity.HasMany(a => a.PreguntaSeguridadUsuario)
                    .WithOne(b => b.KromiUser)
                    .HasForeignKey(b => b.UsuarioId);

                //entity.HasOne(d => d.Sucursal).WithMany(p => p.Usuarios)
                //.HasForeignKey(d => d.SucursalId)
                //.IsRequired(false);
            });

            builder.Entity<AuditTrail>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.EntityName);

                entity.Property(e => e.Id);

                entity.Property(e => e.UserId);
                entity.Property(e => e.EntityName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DateUtc).IsRequired();
                entity.Property(e => e.PrimaryKey).HasMaxLength(100);

                entity.Property(e => e.TrailType).HasConversion<string>();
                entity.Property(e => e.ChangedColumns).HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonConfig),
                    v => JsonSerializer.Deserialize<List<string>>(v, _jsonConfig) ?? new List<string>()
                    );
                entity.Property(e => e.OldValues).HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonConfig),
                    v => JsonSerializer.Deserialize<Dictionary<string, object?>>(v, _jsonConfig) ?? new Dictionary<string, object?>()
                    ).HasColumnType("nvarchar(max)");
                entity.Property(e => e.NewValues).HasConversion(
                    v => JsonSerializer.Serialize(v, _jsonConfig),
                    v => JsonSerializer.Deserialize<Dictionary<string, object?>>(v, _jsonConfig) ?? new Dictionary<string, object?>()
                    ).HasColumnType("nvarchar(max)");
            });

            builder.Entity<PreguntaSeguridad>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Pregunta).HasMaxLength(150);
                entity.Property(x => x.EstaActivo).HasDefaultValue(true);

                entity.HasMany(a => a.PreguntaSeguridadUsuario)
                    .WithOne(b => b.PreguntaSeguridad)
                    .HasForeignKey(b => b.PreguntaSeguridadId);
            });

            builder.Entity<PreguntaSeguridadUsuario>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.UsuarioId).HasMaxLength(36);

                entity.HasOne(b => b.KromiUser)
                  .WithMany(a => a.PreguntaSeguridadUsuario)
                  .HasForeignKey(b => b.UsuarioId);

                entity.HasOne(b => b.PreguntaSeguridad)
                  .WithMany(a => a.PreguntaSeguridadUsuario)
                  .HasForeignKey(b => b.PreguntaSeguridadId);
            });

            builder.Entity<UserRefreshTokens>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName).HasMaxLength(100);
                entity.Property(e => e.RefreshToken).HasMaxLength(250);
                entity.HasIndex(p => p.UserName, "IX_UserRefreshTokens_UserName");
            });

            builder.Entity<Sucursal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).HasMaxLength(30);
                entity.Property(e => e.Nombre).HasMaxLength(60);
                entity.Property(e => e.Direccion).HasMaxLength(150);
                entity.HasIndex(i => i.Codigo);

                //entity.HasMany(a => a.Usuarios)
                //    .WithOne(b => b.Sucursal)
                //    .HasForeignKey(b => b.SucursalId)
                //    .IsRequired(false);
            });

            base.OnModelCreating(builder);
        }
    }
}
