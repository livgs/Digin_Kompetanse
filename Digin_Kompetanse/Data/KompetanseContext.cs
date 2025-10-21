using Digin_Kompetanse.Models;
using Microsoft.EntityFrameworkCore;

namespace Digin_Kompetanse.data;

public class KompetanseContext : DbContext
{
    public KompetanseContext() { }
    public KompetanseContext(DbContextOptions<KompetanseContext> options) : base(options) { }

    public virtual DbSet<Bedrift> Bedrift { get; set; }
    public virtual DbSet<Fagområde> Fagområde { get; set; }
    public virtual DbSet<Kompetanse> Kompetanse { get; set; }
    public virtual DbSet<UnderKompetanse> UnderKompetanse { get; set; }
    public virtual DbSet<BedriftKompetanse> BedriftKompetanse { get; set; }
    public virtual DbSet<LoginToken> LoginToken { get; set; }
    public virtual DbSet<Admin> Admin { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bedrift>(entity =>
        {
            entity.ToTable("bedrift");
            entity.HasKey(e => e.BedriftId).HasName("bedrift_pkey");

            entity.Property(e => e.BedriftId)
                .ValueGeneratedOnAdd()
                .HasColumnName("bedrift_id");

            entity.Property(e => e.BedriftEpost)
                .HasMaxLength(45)
                .HasColumnName("bedrift_epost");

            entity.Property(e => e.BedriftNavn)
                .HasMaxLength(45)
                .HasColumnName("bedrift_navn");

            entity.Property(e => e.Beskrivelse)
                .HasMaxLength(100)
                .HasColumnName("beskrivelse");
        });
        
        modelBuilder.Entity<Fagområde>(entity =>
        {
            entity.ToTable("fagomrade");
            entity.HasKey(e => e.FagområdeId).HasName("fagomrade_pkey");

            entity.Property(e => e.FagområdeId)
                .ValueGeneratedOnAdd()
                .HasColumnName("fagomrade_id");

            entity.Property(e => e.FagområdeNavn)
                .HasMaxLength(45)
                .HasColumnName("fagomrade_navn");

            entity.HasMany(d => d.Kompetanses)
                  .WithMany(p => p.Fagområdes)
                  .UsingEntity<Dictionary<string, object>>(
                      "fagomrade_has_kompetanse",
                      r => r.HasOne<Kompetanse>().WithMany()
                           .HasForeignKey("kompetanse_id")
                           .OnDelete(DeleteBehavior.ClientSetNull)
                           .HasConstraintName("fagomrade_has_kompetanse_kompetanse_id_fkey"),
                      l => l.HasOne<Fagområde>().WithMany()
                           .HasForeignKey("fagomrade_id")
                           .OnDelete(DeleteBehavior.ClientSetNull)
                           .HasConstraintName("fagomrade_has_kompetanse_fagomrade_id_fkey"),
                      j =>
                      {
                          j.ToTable("fagomrade_has_kompetanse");
                          j.HasKey("fagomrade_id", "kompetanse_id")
                           .HasName("fagomrade_has_kompetanse_pkey");

                          j.IndexerProperty<int>("fagomrade_id").HasColumnName("fagomrade_id");
                          j.IndexerProperty<int>("kompetanse_id").HasColumnName("kompetanse_id");
                      });
        });
        
        modelBuilder.Entity<Kompetanse>(entity =>
        {
            entity.ToTable("kompetanse");
            entity.HasKey(e => e.KompetanseId).HasName("kompetanse_pkey");

            entity.Property(e => e.KompetanseId)
                .ValueGeneratedOnAdd()
                .HasColumnName("kompetanse_id");

            entity.Property(e => e.KompetanseKategori)
                .HasMaxLength(45)
                .HasColumnName("kompetanse_kategori");
        });
        
        modelBuilder.Entity<UnderKompetanse>(entity =>
        {
            entity.ToTable("under_kompetanse");
            entity.HasKey(e => e.UnderkompetanseId).HasName("under_kompetanse_pkey");

            entity.Property(e => e.UnderkompetanseId)
                .ValueGeneratedOnAdd()
                .HasColumnName("underkompetanse_id");

            entity.Property(e => e.KompetanseId)
                .HasColumnName("kompetanse_id");

            entity.Property(e => e.UnderkompetanseNavn)
                .HasMaxLength(45)
                .HasColumnName("underkompetanse_navn");

            entity.HasOne(d => d.Kompetanse)
                .WithMany(p => p.UnderKompetanses)
                .HasForeignKey(d => d.KompetanseId)
                .HasConstraintName("under_kompetanse_kompetanse_id_fkey");
        });
        
        modelBuilder.Entity<BedriftKompetanse>(entity =>
        {
            entity.ToTable("bedrift_kompetanse");
            entity.HasKey(e => e.Id).HasName("bedrift_kompetanse_pkey");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            entity.Property(e => e.BedriftId).HasColumnName("bedrift_id");

            // C#-navn FagområdeId → DB-kolonne fagomrade_id
            entity.Property(e => e.FagområdeId).HasColumnName("fagomrade_id");

            entity.Property(e => e.KompetanseId).HasColumnName("kompetanse_id");
            entity.Property(e => e.UnderKompetanseId).HasColumnName("underkompetanse_id");

            entity.Property(e => e.Beskrivelse)
                .HasMaxLength(200)
                .HasColumnName("beskrivelse");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp with time zone")
                .HasDefaultValueSql("now()");
            
            entity.Property(e => e.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true);

            entity.Property(e => e.ModifiedAt)
                .HasColumnName("modified_at")
                .HasColumnType("timestamp with time zone");

            entity.Property(e => e.ModifiedByBedriftId)
                .HasColumnName("modified_by_bedrift_id");

            entity.HasOne(d => d.Bedrift)
                .WithMany(b => b.BedriftKompetanser)
                .HasForeignKey(d => d.BedriftId)
                .HasConstraintName("fk_bedrift_kompetanse_bedrift")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Fagområde)
                .WithMany()
                .HasForeignKey(d => d.FagområdeId)
                .HasConstraintName("fk_bedrift_kompetanse_fagomrade")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Kompetanse)
                .WithMany()
                .HasForeignKey(d => d.KompetanseId)
                .HasConstraintName("fk_bedrift_kompetanse_kompetanse")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.UnderKompetanse)
                .WithMany()
                .HasForeignKey(d => d.UnderKompetanseId)
                .HasConstraintName("fk_bedrift_kompetanse_underkompetanse")
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasIndex(e => e.BedriftId)
                  .HasDatabaseName("ix_bedrift_kompetanse_bedrift_id");

            entity.HasIndex(e => e.FagområdeId)
                  .HasDatabaseName("ix_bedrift_kompetanse_fagomrade_id");

            entity.HasIndex(e => e.KompetanseId)
                  .HasDatabaseName("ix_bedrift_kompetanse_kompetanse_id");

            entity.HasIndex(e => e.UnderKompetanseId)
                  .HasDatabaseName("ix_bedrift_kompetanse_underkompetanse_id");
            
            entity.HasIndex(e => new { e.BedriftId, e.FagområdeId, e.KompetanseId, e.UnderKompetanseId })
                  .IsUnique()
                  .HasDatabaseName("ux_bedrift_kompetanse_unique_choice_active")
                  .HasFilter("\"is_active\" = true"); // Postgres-filter
        });
        
        modelBuilder.Entity<LoginToken>(e =>
        {
            e.ToTable("login_token");
            e.HasKey(x => x.Id).HasName("login_token_pkey");

            e.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            e.Property(x => x.BedriftId)
                .HasColumnName("bedrift_id")
                .IsRequired();

            e.Property(x => x.CodeHash)
                .HasColumnName("code_hash")
                .IsRequired();

            e.Property(x => x.ExpiresAt)
                .HasColumnName("expires_at")
                .HasColumnType("timestamp with time zone");

            e.Property(x => x.Attempts)
                .HasColumnName("attempts");

            e.Property(x => x.ConsumedAt)
                .HasColumnName("consumed_at")
                .HasColumnType("timestamp with time zone");

            e.HasOne(x => x.Bedrift)
                .WithMany()
                .HasForeignKey(x => x.BedriftId)
                .HasConstraintName("fk_login_token_bedrift")
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => new { x.BedriftId, x.ExpiresAt })
             .HasDatabaseName("ix_login_token_bedrift_expires");
        });
    }
}
