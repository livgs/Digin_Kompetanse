using Digin_Kompetanse.Models;
using Microsoft.EntityFrameworkCore;

namespace Digin_Kompetanse.data;

public class KompetanseContext : DbContext
{
    public KompetanseContext()
    {
    }

    public KompetanseContext(DbContextOptions<KompetanseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bedrift> Bedrift { get; set; }

    public virtual DbSet<Fagområde> Fagområde { get; set; }

    public virtual DbSet<Kompetanse> Kompetanse { get; set; }

    public virtual DbSet<UnderKompetanse> UnderKompetanse { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // La DI håndtere konfigurering
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bedrift>(entity =>
        {
            entity.HasKey(e => e.BedriftId).HasName("bedrift_pkey");

            entity.ToTable("bedrift");

            entity.Property(e => e.BedriftId)
                .ValueGeneratedOnAdd() // <-- auto-increment
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
            entity.HasKey(e => e.FagområdeId).HasName("Fagområde_pkey");

            entity.ToTable("Fagområde");

            entity.Property(e => e.FagområdeId)
                .ValueGeneratedOnAdd() // <-- auto-increment
                .HasColumnName("fagområde_id");
            entity.Property(e => e.BedriftId).HasColumnName("bedrift_id");
            entity.Property(e => e.FagområdeNavn)
                .HasMaxLength(45)
                .HasColumnName("fagområde_navn");

            entity.HasOne(d => d.Bedrift).WithMany(p => p.Fagområdes)
                .HasForeignKey(d => d.BedriftId)
                .HasConstraintName("fk_bedrift");

            entity.HasMany(d => d.Kompetanses).WithMany(p => p.Fagområdes)
                .UsingEntity<Dictionary<string, object>>(
                    "FagområdeHasKompetanse",
                    r => r.HasOne<Kompetanse>().WithMany()
                        .HasForeignKey("KompetanseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fagområde_has_kompetanse_kompetanse_id_fkey"),
                    l => l.HasOne<Fagområde>().WithMany()
                        .HasForeignKey("FagområdeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fagområde_has_kompetanse_fagområde_id_fkey"),
                    j =>
                    {
                        j.HasKey("FagområdeId", "KompetanseId").HasName("fagområde_has_kompetanse_pkey");
                        j.ToTable("fagområde_has_kompetanse");
                        j.IndexerProperty<int>("FagområdeId").HasColumnName("fagområde_id");
                        j.IndexerProperty<int>("KompetanseId").HasColumnName("kompetanse_id");
                    });
        });

        modelBuilder.Entity<Kompetanse>(entity =>
        {
            entity.HasKey(e => e.KompetanseId).HasName("kompetanse_pkey");

            entity.ToTable("kompetanse");

            entity.Property(e => e.KompetanseId)
                .ValueGeneratedOnAdd() // <-- auto-increment
                .HasColumnName("kompetanse_id");
            entity.Property(e => e.KompetanseKategori)
                .HasMaxLength(45)
                .HasColumnName("kompetanse_kategori");
        });

        modelBuilder.Entity<UnderKompetanse>(entity =>
        {
            entity.HasKey(e => e.UnderkompetanseId).HasName("under_kompetanse_pkey");

            entity.ToTable("under_kompetanse");

            entity.Property(e => e.UnderkompetanseId)
                .ValueGeneratedOnAdd() // <-- auto-increment
                .HasColumnName("underkompetanse_id");
            entity.Property(e => e.KompetanseId).HasColumnName("kompetanse_id");
            entity.Property(e => e.UnderkompetanseNavn)
                .HasMaxLength(45)
                .HasColumnName("underkompetanse_navn");

            entity.HasOne(d => d.Kompetanse).WithMany(p => p.UnderKompetanses)
                .HasForeignKey(d => d.KompetanseId)
                .HasConstraintName("under_kompetanse_kompetanse_id_fkey");
        });
    }
}
    
    

     
