using Microsoft.EntityFrameworkCore;

namespace Digin_Kompetanse.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<FagområdeModel> Fagområde { get; set; }
    public DbSet<KompetanseModel> Kompetanse { get; set; }
}